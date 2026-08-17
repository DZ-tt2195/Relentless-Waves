using UnityEngine;
using UnityEngine.UI;
using MyBox;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using System.Diagnostics;
public enum GameState {Setup, Playing, Paused, Over}
[System.Serializable]
public class RulesSlider
{
    public Slider slider;
    public TMP_Text textBox;
}
public class WaveManager : MonoBehaviour
{
#region Setup

    public static WaveManager instance;
    [Foldout("Gameplay", true)]
    public static GameState state = GameState.Setup;
    List<BaseEnemy> allEnemies = new();
    List<Wave<Collection>> waveList = new();
    public int currentWave { get; private set; }
    Stopwatch gameTimer;
    [SerializeField] GameObject blackOutObject;
    [ReadOnly] public float blackOutTime = 0f;

    [Foldout("Game UI", true)]
    public Camera mainCamera;
    [SerializeField] Slider waveSlider;
    [SerializeField] TMP_Text waveCounter;
    [SerializeField] Slider enemySlider;
    [SerializeField] TMP_Text enemyCounter;
    [SerializeField] TMP_Text timerText;
    [SerializeField] Slider energySlider;
    [SerializeField] TMP_Text energyCounter;
    [SerializeField] TMP_Text tutorialText;
    [SerializeField] List<RulesSlider> rulesSliders = new();

    [Foldout("Pause screen", true)]
    [SerializeField] GameObject pauseScreen;
    [SerializeField] Button playButton;
    [SerializeField] Button restartButton;
    [SerializeField] Button quitButton;
    [SerializeField] TMP_Text endText;
    [SerializeField] Slider difficultySlider;
    [SerializeField] TMP_Text difficultyLabel;
    [SerializeField] List<RulesDisplay> displaysOnScreen;

    [Foldout("Audio", true)]
    [SerializeField] AudioClip winSound;
    [SerializeField] AudioClip loseSound;
    
    [Foldout("FPS", true)]
    int lastframe = 0;
    int lastupdate = 60;
    float[] framearray = new float[60];
    public static float minX { get; private set; }
    public static float maxX { get; private set; }
    public static float minY { get; private set; }
    public static float maxY { get; private set; }

    private void Awake()
    {
        instance = this;
        Time.timeScale = 1f;

        float cameraHeight = 2f * mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        minX = mainCamera.transform.position.x - cameraWidth / 2f;
        maxX = mainCamera.transform.position.x + cameraWidth / 2f;
        minY = mainCamera.transform.position.y - cameraHeight / 2f;
        maxY = 4f;

        restartButton.transform.GetComponentInChildren<TMP_Text>().text = AutoTranslate.Restart();
        quitButton.transform.GetComponentInChildren<TMP_Text>().text = AutoTranslate.Quit();
        playButton.transform.GetComponentInChildren<TMP_Text>().text = AutoTranslate.Play();
        endText.text = AutoTranslate.Blank();

        if (!PlayerPrefs.HasKey(PrefManager.Difficulty)) PrefManager.SetDifficulty(1f);
        difficultySlider.onValueChanged.AddListener(UpdateDifficultyText);
        difficultySlider.value = PrefManager.GetDifficulty();
        UpdateDifficultyText(PrefManager.GetDifficulty());

        void UpdateDifficultyText(float value)
        {
            difficultyLabel.text = AutoTranslate.Difficulty($"{value*100:F0}");
            PrefManager.SetDifficulty(value);
        }

        state = GameState.Setup;
        gameTimer = new Stopwatch();

        if (GameFiles.inst.CurrentLevel().includeRules)
        {
            List<Rule> selectedRules = GameFiles.inst.SavedRules().ToList();
            for (int i = 0; i<selectedRules.Count; i++)
            {
                Rule newRule = Instantiate(selectedRules[i]);
                newRule.AssignSlider(rulesSliders[i]);
                displaysOnScreen[i].AssignRule(newRule);
            }
        }
    }
    public void BeginGame()
    {
        AudioManager.instance.Menu();
        playButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);
        pauseScreen.SetActive(false);

        Level currentLevel = GameFiles.inst.CurrentLevel();
        waveList = currentLevel.listOfWaves;
        if (currentLevel.levelType == LevelType.Shuffled)
            waveList = waveList.Shuffle();

        waveSlider.gameObject.SetActive(true);
        enemySlider.gameObject.SetActive(true);
        timerText.gameObject.SetActive(true);
        energySlider.gameObject.SetActive(true);

        difficultySlider.gameObject.SetActive(false);

        state = GameState.Playing;
        EnergyManager.inst.BeginGame();
        gameTimer.Start();
        NewWave();
    }

#endregion

#region Gameplay

    void NewWave()
    {
        StartCoroutine(Player.instance.Immunity(false));
        Level currentLevel = GameFiles.inst.CurrentLevel();

        if (currentWave < waveList.Count() || currentLevel.levelType == LevelType.Endless)
        {
            foreach (Collection collection in waveList[Mathf.Min(waveList.Count-1, currentWave)].enemies)
                CreateEnemy(collection.position, collection.toCreate);

            waveSlider.value = (currentWave + 1) / (float)waveList.Count;

            if (currentLevel.levelType != LevelType.Endless)
            {
                waveCounter.text = AutoTranslate.Wave((currentWave+1).ToString(), waveList.Count.ToString());
                tutorialText.text = Translator.inst.Translate(waveList[currentWave].tutorialKey);;
            }
            else
            {
                waveCounter.text = AutoTranslate.Wave((currentWave+1).ToString(), "\u221E");
                tutorialText.text = "";
            }
        }
        else
        {
            int damageTaken = Player.instance.DamageTaken();
            int score = (int)(PrefManager.GetDifficulty() * 100) - damageTaken*2;
            EndGame(AutoTranslate.Victory(), damageTaken, score);
        }
    }
    void CreateEnemy(Vector2 start, BaseEnemy prefab)
    {
        BaseEnemy enemy = Instantiate(prefab != null ? prefab : GameFiles.inst.RandomEnemy());
        enemy.EnemySetup();
        enemy.transform.position = start;
        allEnemies.Add(enemy);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            endText.text = AutoTranslate.Paused();
            if (state == GameState.Playing)
            {
                state = GameState.Paused;
                gameTimer.Stop();
                pauseScreen.SetActive(true);
                Time.timeScale = 0f;                
            }
            else if (state == GameState.Paused)
            {
                state = GameState.Playing;
                gameTimer.Start();
                pauseScreen.SetActive(false);
                Time.timeScale = 1f;
            }
        }

        if (state != GameState.Playing) return;

        allEnemies.RemoveAll(enemy => enemy == null);
        int currentEnemies = 0;
        if (allEnemies.Count > 0)
        {
            foreach (BaseEnemy enemy in allEnemies)
            {
                if (enemy.GetHealth() > 0)
                    currentEnemies++;
            }

            enemySlider.value = (float)currentEnemies / allEnemies.Count;
            enemyCounter.text = AutoTranslate.Enemies(currentEnemies.ToString(), allEnemies.Count().ToString());

            if (currentEnemies == 0)
            {
                for (int i = allEnemies.Count - 1; i >= 0; i--)
                    Destroy(allEnemies[i].gameObject);
                currentWave++;
                NewWave();
            }
        }

        UpdateTexts();
        blackOutTime = Mathf.Max(0f, blackOutTime - Time.deltaTime);
        blackOutObject.SetActive(blackOutTime > 0f);
        blackOutObject.transform.SetAsLastSibling();
        blackOutObject.transform.position = Player.instance.transform.position;
    }
    public void EndGame(string text, int tookDamage, int score)
    {
        if (!pauseScreen.activeSelf)
        {
            state = GameState.Over;
            pauseScreen.SetActive(true);
            UpdateTexts();
            Time.timeScale = 0f;

            endText.text = $"{text}\n\n" + $"{AutoTranslate.Damage_Taken(tookDamage.ToString())}\n";
            if (score > 0)
            {
                endText.text += $"\n{AutoTranslate.Score(score.ToString())}";
                AudioManager.instance.PlaySound(winSound, 0.3f);
            }
            else
            {
                AudioManager.instance.PlaySound(loseSound, 0.3f);                
            }
            Level currentLevel = GameFiles.inst.CurrentLevel();
            if (score > PrefManager.GetScore(currentLevel.levelName.ToString()))
                PrefManager.SetScore(currentLevel.levelName.ToString(), score);
        }
    }
    void UpdateTexts()
    {
        (int energy, int maxEnergy) playerStats = Player.instance.EnergyInfo();
        energySlider.value = playerStats.energy / (float)playerStats.maxEnergy;
        energyCounter.text = AutoTranslate.Energy(playerStats.energy.ToString(), playerStats.maxEnergy.ToString());
        timerText.text = $"{AutoTranslate.Difficulty($"{PrefManager.GetDifficulty()*100:F0}")}\n{MyExtensions.StopwatchTime(gameTimer)}";
        timerText.text += $" | {AutoTranslate.FPS(GetFPS())}";

        string GetFPS()
        {
            framearray[lastframe] = Time.deltaTime;
            lastframe++;
            if (lastframe == 60)
            {
                lastframe = 0;
                float total = 0;
                for (int i = 0; i < framearray.Length; i++)
                    total += framearray[i];
                lastupdate = (int)(framearray.Length / total);
                return lastupdate.ToString();
            }
            return (lastupdate > Application.targetFrameRate) ? Application.targetFrameRate.ToString() : lastupdate.ToString();
        }        
    }

#endregion

#region Helpers
    public List<BaseEnemy> GetEnemies() => allEnemies;
    public static float RandomX(float offset) => Random.Range(minX + offset, maxX - offset);
    public static float RandomY(float offset) => Random.Range(minY + offset, maxY - offset);

#endregion

}