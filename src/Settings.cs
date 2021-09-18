

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

        [Name("Enable direction")]
        [Description("Show left/right/front/back")]
        public bool EnableDir = true;

        [Name("Enable location logging to disk")]
        [Description("logs current location to file tracking.csv")]
        public bool EnableCSV = true;

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

        protected override void OnConfirm()
        {            
            base.OnConfirm();
        }
    }








    internal static class Settings
    {
        public static SettingsMain options;
        public static KeyCode returnKeyValue;

        public static void OnLoad()
        {
            options = new SettingsMain();
            ///options.RefreshFields();
            options.AddToModSettings("Arrow Tracker Settings");
        }


        public static KeyCode GetInputKeyFromString(int keyStringInt)
        {
            switch(keyStringInt)
            {
                case 0:  return returnKeyValue = KeyCode.B;
                case 1:  return returnKeyValue = KeyCode.C;
                case 2:  return returnKeyValue = KeyCode.F;
                case 3:  return returnKeyValue = KeyCode.G;
                case 4:  return returnKeyValue = KeyCode.H;
                case 5:  return returnKeyValue = KeyCode.I;
                case 6:  return returnKeyValue = KeyCode.K;
                case 7:  return returnKeyValue = KeyCode.K;
                case 8:  return returnKeyValue = KeyCode.L;
                case 9:  return returnKeyValue = KeyCode.M;
                case 10: return returnKeyValue = KeyCode.N;
                case 11: return returnKeyValue = KeyCode.O;
                case 12: return returnKeyValue = KeyCode.P;
                case 13: return returnKeyValue = KeyCode.R;
                case 14: return returnKeyValue = KeyCode.T;
                case 15: return returnKeyValue = KeyCode.U;
                case 16: return returnKeyValue = KeyCode.V;
                case 17: return returnKeyValue = KeyCode.X;
                case 18: return returnKeyValue = KeyCode.Y;
                case 19: return returnKeyValue = KeyCode.Z;
                case 20: return returnKeyValue = KeyCode.Insert;
                case 21: return returnKeyValue = KeyCode.Home;
                case 22: return returnKeyValue = KeyCode.End;
                case 23: return returnKeyValue = KeyCode.PageUp;
                case 24: return returnKeyValue = KeyCode.PageDown;
                case 25: return returnKeyValue = KeyCode.Pause;
                case 26: return returnKeyValue = KeyCode.Clear;
            }
            return returnKeyValue;
        }

    }
}


