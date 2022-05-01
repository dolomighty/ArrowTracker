
using MelonLoader;
using UnityEngine;


namespace ArrowTracker
{
    public class Vars : MelonMod
    {     
        public static float scan_period = 0.1f;
        public static float scan_deadline = 0f;
        //public static string location_filename = "";
        // public static AudioClip ping_lo;
        // public static AudioClip ping_hi;
        public static string prev_txt = "";
        //public static float sound_deadline = 0f;
        public static bool nearest_found = false;
        public static Vector3 nearest_xyz = new Vector3();
        public static GameObject target_light = null;

        //public static GameObject SettingsBackgroundSprite;
        //public static GameObject SettingsBackgroundMark;
        //public static GameObject SettingsBackgroundVignette;
    }
}

