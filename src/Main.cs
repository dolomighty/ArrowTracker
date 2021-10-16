



using MelonLoader;
using System.IO;
using System;
using UnityEngine;
using ModSettings;


namespace ArrowTracker
{
    
    public class Main : MelonMod
    {
        public override void OnApplicationStart()
        {
            string tld_path = Path.GetFullPath(typeof(MelonMod).Assembly.Location + @"\..\..\");
//            MelonLogger.Msg(tld_path);
            Vars.location_filename = Path.Combine(tld_path, "location.txt");
//            MelonLogger.Msg(Vars.location_filename);
            ArrowTracker.Settings.OnLoad();
        }



//        public override void OnLevelWasInitialized(int level)
//        {
//            Vars.scan_deadline = 0f;
//        }



        public void log_location(string region, Vector3 pos)
        {
            //var ingame_secs = Time.time;
            string local_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string msg = local_time + "," + region + "," + pos.x + "," + pos.y + "," + pos.z;

            const string csv_filename = "tracking.csv";   // lo piazza nella dir base di TLD

            if (!File.Exists(csv_filename)) {
                using (StreamWriter sw = File.CreateText(csv_filename))
                    sw.WriteLine("local_time,region,x,y,z");
            }

            using (StreamWriter sw = File.AppendText(csv_filename))
                sw.WriteLine(msg);

            //HUDMessage.AddMessage(msg, 0.5f, true);
            //MelonLogger.Msg(msg);
        }



        static string[] dirs_arrows = { "↑", "↗", "→", "↘", "↓", "↙", "←", "↖" };
        static string[] dirs_text = { "front", "right", "back", "left" };


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
            Vars.scan_deadline -= Time.deltaTime;
            if (Vars.scan_deadline >= 0) return;  // nop
            Vars.scan_deadline += Vars.scan_period;

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
            Vector3 nearest_xyz = new Vector3();
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
                nearest_xyz = xyz;
            }

            //MelonLogger.Msg("elap "+(Time.realtimeSinceStartup - start));

            if (nearest_name == "") return;  // non abbiamo nearest, ciao

            // vediamo dove sta il target rispetto al mio facing
            // alto e basso non li considero
            Vector3 tgt = nearest_xyz - pos;
            float dx = Vector3.Dot(tgt, GameManager.GetPlayerTransform().right);
            float dy = Vector3.Dot(tgt, GameManager.GetPlayerTransform().forward);
            float angle = (float)(Math.Atan2(dx, dy) * 180 / Math.PI);  // -180 <= angle <= +180

            //string txt = nearest_name + " dist " + (int)dist + " angle " + (int)(angle) + " facing " + facing_from_angle(angle);
            string txt = mk_msg(nearest_name, (int)dist, angle);

            //MelonLogger.Msg(txt);

            // mandiamo il msg in HUD solo se cambia
            // cosi da fermo non appare
            if (txt == Vars.prev_txt) return;
            Vars.prev_txt = txt;

            // i messaggi verso l'HUD internamente vengono aggiunti ad una coda
            // e ne aggiungo worst case 1 al secondo (Vars.period)
            // per evitare di affollare la coda (con forse perdita dei messaggi, ma sicuramente con msg fuori ordine)
            // setto il ttl del messaggio <1sec. se mettessi 0 avremmo subito il fadeout
            // sarebbe visibile ma meno leggibile a mio parere
            HUDMessage.AddMessage(txt, 0.5f, true);
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





        public override void OnUpdate()
        {
            if(!Settings.options.EnableMod)return;

            // GameManager esiste sempre, ma i suoi membri no
            // quindi bisogna evitare di invocar roba che non c'è per non generar errori
            // la via del try/catch sembra esser l'unica maniera in cui funzioni sempre
            try { var gpt = GameManager.GetPlayerTransform().position; } catch { return; }

            scan();
            //click();
        }





    }
}


