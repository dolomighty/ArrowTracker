
using MelonLoader;
using UnityEngine;


namespace ArrowTracker
{
    public class Vars : MelonMod
    {     
        public static float scan_period = 1f;
        public static float scan_deadline = 0f;
        public static string location_filename = "";
        // public static AudioClip ping_lo;
        // public static AudioClip ping_hi;
        public static string prev_txt = "";
        public static float sound_deadline = 0f;

        public static GameObject SettingsBackgroundSprite;
        public static GameObject SettingsBackgroundMark;
        public static GameObject SettingsBackgroundVignette;
    }
}

