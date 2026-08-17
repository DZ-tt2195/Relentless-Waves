using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;

public class Rule : MonoBehaviour
{
    [Foldout("Rules info", true)]
    float timer = 0f;
    [SerializeField] float maxTimer = 10f; public float GetTime => maxTimer;
    [SerializeField] bool beOnPlayer;
    protected HashSet<Entity> entitiesInRange = new();
    Slider slider;
    protected virtual void Awake()
    {
        this.name = this.name.Replace("(Clone)", "");
    }
    public void AssignSlider(RulesSlider rulesslider)
    {
        this.slider = rulesslider.slider;
        this.slider.value = 0f;
        this.slider.gameObject.SetActive(true);
        rulesslider.textBox.text = Translator.inst.Translate(this.name);
    }
    void Update()
    {
        if (WaveManager.state == GameState.Playing)
        {
            EveryFrame();
            timer = Mathf.Min(timer+Time.deltaTime, maxTimer);
            if (CanUse())
            {
                timer = 0f;
                ActivateRule();
            }
            this.slider.value = timer/maxTimer;
        }
    }
    protected virtual void EveryFrame()
    {
        if (beOnPlayer)
        {
            this.transform.position = Player.instance.transform.position;
        }
    }
    protected virtual bool CanUse()
    { 
        return timer == maxTimer;
    }
    protected virtual void ActivateRule()
    {
    }
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Entity entity))
            entitiesInRange.Add(entity);
    }
    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Entity entity))
            entitiesInRange.Remove(entity);        
    }
}
