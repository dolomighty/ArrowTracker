



using MelonLoader;
using System.IO;
using System;
using UnityEngine;
using ModSettings;


namespace ArrowTracker
{

    public class Main : MelonMod
    {
        public static class BuildInfo
        {
            public const string Name = "ArrawTracker";
            public const string Description = "ArrawTracker";
            public const string Author = "dolomighty";
            public const string Company = null;
            public const string Version = "0";
            public const string DownloadLink = null;
        }
    

        public override void OnApplicationStart()
        {
            //string tld_path = Path.GetFullPath(typeof(MelonMod).Assembly.Location + @"\..\..\");
            //MelonLogger.Msg(tld_path);
            //Vars.location_filename = Path.Combine(tld_path, "location.txt");
            //MelonLogger.Msg(Vars.location_filename);
            ArrowTracker.Settings.OnLoad();
        }



//        public override void OnLevelWasInitialized(int level)
//        {
//            Vars.scan_deadline = 0f;
//        }



        //public void log_location(string region, Vector3 pos)
        //{
        //    //var ingame_secs = Time.time;
        //    string local_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //    string msg = local_time + "," + region + "," + pos.x + "," + pos.y + "," + pos.z;

        //    const string csv_filename = "tracking.csv";   // lo piazza nella dir base di TLD

        //    if (!File.Exists(csv_filename)) {
        //        using (StreamWriter sw = File.CreateText(csv_filename))
        //            sw.WriteLine("local_time,region,x,y,z");
        //    }

        //    using (StreamWriter sw = File.AppendText(csv_filename))
        //        sw.WriteLine(msg);

        //    //HUDMessage.AddMessage(msg, 0.5f, true);
        //    //MelonLogger.Msg(msg);
        //}



        string[] dirs_arrows = { "↑", "↗", "→", "↘", "↓", "↙", "←", "↖" };
        string[] dirs_text = { "front", "right", "back", "left" };


        string facing_from_angle(float angle,string[] dirs)
        {
            // angle ±[0,180]
            // angle   target bearing
            // 0       forward
            // >0      right
            // <0      left
            int idx = (int)Math.Floor(angle * dirs.Length / 360 + 0.5);
            if (idx < 0) idx += dirs.Length;
            return dirs[idx];
        }


        string mk_msg(string item, int dist, float angle)
        {
            //switch (Settings.options.Style)
            //{
            //    case 0: return item + " @ " + dist + " meters";                    
            //    case 1: return item + " @ " + dist + " meters " + facing_from_angle(angle, dirs_text);                    
            //    case 2: return item + " " + facing_from_angle(angle, dirs_arrows) + " " + dist + " meters";                    
            //}
            //return "wtf?!";

            string msg = "";

            switch (Settings.options.ShowItem)
            {
                case 1: msg += item; break;
            }

            switch (Settings.options.ShowDir)
            {
                case 0: msg += " @"; break;
                case 1: msg += " " + facing_from_angle(angle, dirs_text); break;
                case 2: msg += " " + facing_from_angle(angle, dirs_arrows); break;
            }

            switch (Settings.options.ShowDist)
            {
                case 1: msg += " " + dist + " meters"; break;
            }

            return msg;
        }




        void scan()
        {
            // la scansione delle frecce è relativamente lenta
            // quindi non la facciamo ogni frame
            if (Time.time < Vars.scan_deadline) return;
            Vars.scan_deadline = Time.time + 0.02f;

            arrow_hide();
            message_hide();

            // debug
            //float rads = (float)(GameManager.GetPlayerTransform().rotation.eulerAngles.y * Math.PI / 180F);
            //arrow_show(rads);
            // arrow_show(GameManager.GetPlayerTransform().rotation.eulerAngles.y);


            //MelonLogger.Msg(InterfaceManager.m_Panel_HUD.ToString());
            //MelonLogger.Msg(InterfaceManager.m_Panel_HUD.enabled.ToString());   // True
            //MelonLogger.Msg(InterfaceManager.m_Panel_HUD.isActiveAndEnabled.ToString());    // true when ingame
            //MelonLogger.Msg(InterfaceManager.m_Panel_HUD.IsOverlayActive().ToString()); // false
            //MelonLogger.Msg(InterfaceManager.m_Panel_HUD.IsShowingGenericInteractionPrompt().ToString());   // false
            //InterfaceManager.m_Panel_HUD.DisplayWarningMessage("asdasdasd");    // strobing red centered message
            //MelonLogger.Msg(InterfaceManager.m_Panel_HUD.IsInvoking().ToString());  // false
            //MelonLogger.Msg(InterfaceManager.m_Panel_HUD.m_EssentialHud.active.ToString()); // true when ingame
            //MelonLogger.Msg(InterfaceManager.m_Panel_HUD.m_EssentialHud.


            Vector3 pos = GameManager.GetPlayerTransform().position;
            //string msg = GameManager.m_ActiveScene + " " + pos.x + " " + pos.y + " " + pos.z;
            //HUDMessage.AddMessage(msg, 0.5f, true);
            //MelonLogger.Msg(msg);

            //if(Settings.options.EnableCSV) log_location(GameManager.m_ActiveScene, pos);

            // profiling
            float start = Time.realtimeSinceStartup;

            // cerchiamo le frecce
            int layerMask = 0;
            layerMask |= 1 << 17; // gear layer
            layerMask |= 1 << 19; // gear layer
            // troviamo la più vicina
            float dist = 1E6f;  // cerchiamo in tutta la regione
            string nearest_name="";
            Vars.nearest_found = false;
            Collider[] hits = Physics.OverlapSphere( pos, dist, layerMask );
            foreach (var item in hits)
            {
                if (!item.name.Contains("Arrow")) continue; // se non è una freccia&c ciao
                //if (!item.name.Contains("GEAR_")) continue; // ogni cosa collezionabile (debug)

                Vector3 xyz = item.transform.position;
                float d = Vector3.Distance(xyz, pos);
                if (dist < d)continue;   // se è più distante ciao

                // trovata una freccia più vicina, segnamocela
                dist = d;
                nearest_name = item.name;
                Vars.nearest_xyz = xyz;
                Vars.nearest_found = true;
            }

            //MelonLogger.Msg("elap "+(Time.realtimeSinceStartup - start));

            if (!Vars.nearest_found) return;

            // vediamo dove sta il target rispetto al mio facing
            // alto e basso non li considero
            Vector3 tgt = Vars.nearest_xyz - pos;
            float dx = Vector3.Dot(tgt, GameManager.GetPlayerTransform().right);
            float dy = Vector3.Dot(tgt, GameManager.GetPlayerTransform().forward);
            float angle = (float)(Math.Atan2(dx, dy) * 180 / Math.PI);  // -180 <= angle <= +180

            arrow_show(180-angle);

            //string txt = nearest_name + " dist " + (int)dist + " angle " + (int)(angle) + " facing " + facing_from_angle(angle);
            string txt = mk_msg(nearest_name, (int)dist, angle);

            //MelonLogger.Msg(txt);
            if(Settings.options.Compass==1) message_show(txt);
        }







        //        void click() {
        //            Vars.sound_deadline -= Time.deltaTime;
        //            if (Vars.sound_deadline >= 0) return;  // nop
        //            Vars.sound_deadline += 0.1f;
        //
        //            GameAudioManager.PlayGUIScroll();
        //            
        //            // AudioSource audio = GetComponent<AudioSource>();
        //            // audio.clip = Vars.ping_lo;
        //            // audio.Play();
        //
        //            // GameAudioManager.PlayOneShot(Vars.ping_lo,1);
        //            // GameAudioManager.Play3DSound("asd", Vars.ping_lo);
        //            // AudioSource.PlayClipAtPoint( Vars.ping_lo, pos );
        //
        //            //// da sempre len=0, perche?
        //            //// ed anche il name non è mai settato
        //            //AudioClip ac = AudioClip.Create("diocan", 44100, 1, 44100, false);
        //            //AudioClip.SetName(ac, "qweasdqwe");
        //            //MelonLogger.Msg(ac.GetName() + " ac.samples " + ac.samples);
        //            //float[] asd = new float[1000];
        //            //AudioClip.SetData(ac, asd, asd.Length, 0);
        //            //MelonLogger.Msg(ac.GetName() + " ac.samples " + ac.samples);
        //
        //            // allora, leggendo sul discord dei modders di TLD
        //            // TLD non usa l'audio base di unity, ma una lib di audiokinetic che si chiama wwave
        //            // penso serva per l'audio posizionale, il calcolo dei riverberi, occlusioni, cose cosi
        //
        //            // TLDR: non è possibile al momento playare clips custom
        //
        //            // quindi l'unica è mostrare la distanza
        //            // il massimo che si può fare ora è usare uno dei suoni stock
        //        }




        void light_set_xyz_rgb( Vector3 xyz, Color rgb )
        {
            if (!Vars.target_light) {
                Vars.target_light = new GameObject("arrow light");
                var lc = Vars.target_light.AddComponent<Light>();
                lc.intensity = 3;
            }
            Vars.target_light.GetComponent<Light>().color = rgb;
            Vars.target_light.transform.position = xyz;
        }

        void light_destroy()
        {
            if (!Vars.target_light) return;
            UnityEngine.Object.Destroy(Vars.target_light);
            Vars.target_light = null;
        }

        void light_update() {
            // mettiamo una luce sulla freccia
            if (!Settings.options.EnableLight || !Vars.nearest_found) {
                light_destroy();
                return;
            }
            var p = Vars.nearest_xyz + Vector3.up * 0.5;
            light_set_xyz_rgb(p, UnityEngine.Random.ColorHSV(0f, 1f));
        }








        GameObject arrow_go=null;

        GameObject arrow_get() {
            if (arrow_go) return arrow_go;

            // cloniamo una delle icone dell'HUD
            var hud_icon = InterfaceManager.m_Panel_HUD.m_Sprite_Crosshair.gameObject;    // center
            var parent_go = hud_icon.transform.parent.gameObject;
            arrow_go = UnityEngine.Object.Instantiate(hud_icon, parent_go.transform);
            UnityEngine.Object.Destroy(arrow_go.GetComponent<UIAnchor>());
            //arrow_go.transform.position = new Vector3(0.9F,-0.9F);

            // widget creato, ora lo specializziamo
            var comp = arrow_go.GetComponent<UISprite>(); // son tutti sprites
            comp.spriteName = "arrow";  // triangolo verso il basso
            //comp.spriteName = "arrow_black"; // triangolo verso il basso
            comp.SetDimensions(20, 80);
            comp.alpha = 0.5F;
            comp.color = Color.white;

            return arrow_go;
        }

        void arrow_show( float degs ) {
            var go = arrow_get();
            go.transform.position = Vector3.zero;
            go.transform.rotation = new Quaternion();
            go.transform.Rotate(0, 0, degs);
            go.transform.Translate(0, -0.1F, 0);
            // var comp = go.GetComponent<UISprite>();
            go.SetActive(true);
        }


        void arrow_hide() {
            var go = arrow_get();
            go.SetActive(false);
        }






        GameObject message_go = null;

        GameObject message_get()
        {
            if (message_go) return message_go;

            // magie varie per agganciarsi all'hud
            // in pratica, clona la label che appare in alto in mezzo
            var hud_msg_go = InterfaceManager.m_Panel_HUD.m_Label_Message.gameObject;
            var par = hud_msg_go.transform.parent.gameObject;
            message_go = UnityEngine.Object.Instantiate(hud_msg_go, par.transform);
            UnityEngine.Object.Destroy(message_go.GetComponent<UIAnchor>());
            message_go.transform.position = new Vector3(0.5F, -0.9F);

            // piazziamo dei defaults appropriati
            var lc = message_go.GetComponent<UILabel>();
            lc.pivot = UIWidget.Pivot.Left;
            lc.alpha = 1;
            lc.color = Color.white;

            return message_go;
        }


        void message_show(string txt)
        {
            var go = message_get();
            var comp = go.GetComponent<UILabel>();
            comp.text = txt;
            go.SetActive(true);
        }


        void message_hide()
        {
            var go = message_get();
            go.SetActive(false);
        }




        public override void OnUpdate()
        {
            if(!Settings.options.EnableMod)return;

            // GameManager esiste sempre, ma i suoi membri no
            // quindi bisogna evitare di invocar roba che non c'è per non generar errori
            // la via del try/catch sembra esser l'unica maniera in cui funzioni sempre
            try { var gpt = GameManager.GetPlayerTransform().position; } catch { return; }

            //// mostra posizione,regione,heading,ecc come quando si fa il debug screenshot
            //HUDManager.m_HudDisplayMode = HudDisplayMode.DebugInfo;

            // TLD modding discord
            // https://discord.gg/nb2jQez

            //UI();

            light_update();
            scan();
            //click();
        }





    }
}


