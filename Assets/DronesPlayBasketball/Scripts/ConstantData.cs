using UnityEngine;

public class ConstantData : MonoBehaviour
{
    public static bool IsMultiplayer = true;
    public static string Android_Link = "";
    public static string iOS_Link = "";

    public static string musicPlayerPrefs = "MusicVolume";
    public static string soundPlayerPrefs = "SoundVolume";

    public enum Country
    {
        India = 0,
        America = 1,
        Australia = 2,
        England = 3,
        Romania = 4,
        Vietnam = 5,
        Venezuela = 6,
        Jamaica = 7,
        Libya = 8
    }

    public enum BasketSide
    {
        Left,
        Right
    }
}
