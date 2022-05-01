

using System.IO;
using System.Reflection;
using UnityEngine;
using ModSettings;


namespace ArrowTracker
{


    internal class SettingsMain : JsonModSettings
    {
        [Section("General settings")]

        [Name("Enable mod")]
        [Description("")]
        public bool EnableMod = true;

        //[Name("Style")]
        //[Description("")]
        //[Choice("GEAR_ARROW @ 3 meters", "GEAR_ARROW @ 3 meters front", "GEAR_ARROW ↑ 3 meters", "↑ 3 meters", "3 meters")]
        //public int Style = 1;

        [Name("Light")]
        [Description("the arrow sparkles")]
        public bool EnableLight = true;

        [Name("Direction")]
        [Description("")]
        [Choice("hide", "front/left/...", "←↑→↓")]
        public int ShowDir = 2;

        [Name("Item")]
        [Description("")]
        [Choice("hide", "show")]
        public int ShowItem = 1;

        [Name("Distance")]
        [Description("")]
        [Choice("hide", "show")]
        public int ShowDist = 1;

        [Name("Compass")]
        [Description("")]
        [Choice("hide", "show")]
        public int Compass = 1;

        //[Name("Enable location logging to disk")]
        //[Description("logs current location to file tracking.csv")]
        //public bool EnableCSV = false;

        protected override void OnChange(FieldInfo field, object oldValue, object newValue)
        {
            //GameManager.GetCameraEffects().DepthOfFieldTurnOff(true);
            //GameManager.GetCameraEffects().VignettingEnable(false);
            //GameManager.GetCameraEffects().ContrastEnhanceEnable(false);

            //Vars.SettingsBackgroundSprite = GameObject.Find("Panel_OptionsMenu/Pages/ModSettings/bg (2)");
            //Vars.SettingsBackgroundSprite.GetComponent<UISprite>().mColor = new Color(0f, 0f, 0f, 0f);
            //Vars.SettingsBackgroundMark = GameObject.Find("Panel_OptionsMenu/Pages/ModSettings/mark (2)");
            //Vars.SettingsBackgroundMark.GetComponent<UISprite>().mColor = new Color(0f, 0f, 0f, 0f);
            //Vars.SettingsBackgroundVignette = GameObject.Find("Panel_OptionsMenu/Pages/ModSettings/Vignette (2)");
            //Vars.SettingsBackgroundVignette.GetComponent<UISprite>().mColor = new Color(0f, 0f, 0f, 0f);
        }

        //protected override void OnConfirm()
        //{            
        //    base.OnConfirm();
        //}
    }








    internal static class Settings
    {
        public static SettingsMain options;

        public static void OnLoad()
        {
            options = new SettingsMain();
            options.AddToModSettings("Arrow Tracker Settings");
        }
    }
}


