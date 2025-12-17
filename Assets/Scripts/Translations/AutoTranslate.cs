using System.Collections.Generic;
public static class AutoTranslate
{

public static string Start_on_Wave (string Num)  { return(Translator.inst.Translate("Start_on_Wave", new(){("Num", Num)})); }

public static string Best_Score (string Num)  { return(Translator.inst.Translate("Best_Score", new(){("Num", Num)})); }

public static string Difficulty (string Num)  { return(Translator.inst.Translate("Difficulty", new(){("Num", Num)})); }

public static string Bullets_Missed (string Num)  { return(Translator.inst.Translate("Bullets_Missed", new(){("Num", Num)})); }

public static string Health_Lost (string Num)  { return(Translator.inst.Translate("Health_Lost", new(){("Num", Num)})); }

public static string Wave (string Num,string Max)  { return(Translator.inst.Translate("Wave", new(){("Num", Num),("Max", Max)})); }

public static string Skipped_Ahead (string Num)  { return(Translator.inst.Translate("Skipped_Ahead", new(){("Num", Num)})); }

public static string Enemies (string Num,string Max)  { return(Translator.inst.Translate("Enemies", new(){("Num", Num),("Max", Max)})); }

public static string Bullets (string Num,string Max)  { return(Translator.inst.Translate("Bullets", new(){("Num", Num),("Max", Max)})); }

public static string Health (string Num,string Max)  { return(Translator.inst.Translate("Health", new(){("Num", Num),("Max", Max)})); }

public static string Score (string Num)  { return(Translator.inst.Translate("Score", new(){("Num", Num)})); }

public static string FPS (string Num)  { return(Translator.inst.Translate("FPS", new(){("Num", Num)})); }

public static string DoEnum(ToTranslate thing) {return(Translator.inst.Translate(thing.ToString()));}
}
public enum ToTranslate {
Description,Designer,Last_Update,Translator_Credit,Language,Choose_Level,Controls,Play,Delete,Replay,Quit,Paused,Restart,No_Score,Victory,Lost,Resupply,Health_Pack,Juggle_Balls,Infinite_Bullets,Tutorial_1,Tutorial_2,Enemy_Rush,Random,Endless,First_Tutorial,Second_Tutorial,Third_Tutorial,Fourth_Tutorial,Update_History,Blank,Update_0,Update_0_Text,Update_1,Update_1_Text}
