



using MelonLoader;
using System.IO;
using System;
using UnityEngine;
using ModSettings;


namespace ArrowTracker
{
    
    public class Main : MelonMod
    {


        //        const int samplerate = 8000;
        //
        //
        //        AudioClip synth_ping( string name="ping", float hz=1000f, float attack_rate=0.1f, float release_rate=0.01f, float release_time=0.1f  ){
        //            // genera una sinusoide con fasi di attack e release
        //
        //            int cliplen = (int)(samplerate * 0.1);
        ////            MelonLogger.Msg("samplerate "+samplerate+" cliplen "+cliplen);
        //
        //            AudioClip ac = AudioClip.Create(name, cliplen, 1, samplerate, false );
        //            MelonLogger.Msg(ac.name+" ac.samples "+ac.samples);
        //
        //            float[] data = new float [cliplen];
        ////            MelonLogger.Msg("data "+data.Length);
        //
        //            int release_start = (int)(samplerate * release_time);
        //
        //            float gain_tgt = 1;
        //            float rate = attack_rate;
        //
        //            float gain = 0;
        //            int count = 0;
        //            while (count < data.Length)
        //            {
        //                data[count] = (float)Math.Sin(2 * Math.PI * hz * count / samplerate) * gain;
        //                gain += (gain_tgt-gain)*rate;
        //
        //                if(count==release_start){
        //                    gain_tgt = 0;
        //                    rate = release_rate;
        //                }
        //
        //                count++;
        //            }
        //
        //            MelonLogger.Msg(name+" data.Length "+data.Length);
        //            ac.SetData(data,0);
        //            MelonLogger.Msg(name+" ac.samples "+ac.samples);
        //
        //            return ac;
        //        }



        public override void OnApplicationStart()
        {
            string tld_path = Path.GetFullPath(typeof(MelonMod).Assembly.Location + @"\..\..\");
//            MelonLogger.Msg(tld_path);
            Vars.location_filename = Path.Combine(tld_path, "location.txt");
//            MelonLogger.Msg(Vars.location_filename);
//            Vars.ping_lo = synth_ping("ping_lo", 1000);
//            Vars.ping_hi = synth_ping("ping_hi", 1200);


//            var ac = AudioClip.Create("diocan", 44100, 1, 44100, false );
//            ac.CreateUserSound("dioc", 44100, 1, 44100, false);
////            AudioClip.SetName(ac, "qweasdqwe");
////            ac.name = "qweasdqwe";
//            MelonLogger.Msg("ac.GetName() " + ac.GetName() + " ac.samples " + ac.samples);
//            //float[] asd = new float[1000];
//            //AudioClip.SetData(ac,asd,asd.Length,0);

            ArrowTracker.Settings.OnLoad();
        }



//        public override void OnLevelWasInitialized(int level)
//        {
//            Vars.scan_deadline = 0f;
//        }




        //public void log_location(string msg)
        //{
        //    if (File.Exists(Vars.location_filename)) return;
        //    using (StreamWriter sw = new StreamWriter(Vars.location_filename))
        //        sw.WriteLine(msg);
        //}


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



        static string[] dirs_arrows = { "←", "→", "↑", "↓" };
        static string[] dirs_text = { "left", "right", "front", "back" };

        string facing_from_angle(float angle,string[] dirs)
        {
            // angle   target bearing
            // 0       forward
            // >0      right
            // <0      left
            if (angle >= -135 && angle <=  -45) return dirs[0];
            if (angle >=  -45 && angle <=  +45) return dirs[2];
            if (angle >=  +45 && angle <= +135) return dirs[1];
            return dirs[3];
        }



        string mk_msg(string item, int dist, float angle)
        {
            switch (Settings.options.Style)
            {
                case 0: return item + " @ " + dist + " meters";                    
                case 1: return item + " @ " + dist + " meters " + facing_from_angle(angle, dirs_text);                    
                case 2: return item + " " + facing_from_angle(angle, dirs_arrows) + " " + dist + " meters";                    
            }
            return "wtf?!";
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



        
        

        
        void click() {
            Vars.sound_deadline -= Time.deltaTime;
            if (Vars.sound_deadline >= 0) return;  // nop
            Vars.sound_deadline += 0.1f;

            GameAudioManager.PlayGUIScroll();
            
            // AudioSource audio = GetComponent<AudioSource>();
            // audio.clip = Vars.ping_lo;
            // audio.Play();

            // GameAudioManager.PlayOneShot(Vars.ping_lo,1);
            // GameAudioManager.Play3DSound("asd", Vars.ping_lo);
            // AudioSource.PlayClipAtPoint( Vars.ping_lo, pos );

            //// da sempre len=0, perche?
            //// ed anche il name non è mai settato
            //AudioClip ac = AudioClip.Create("diocan", 44100, 1, 44100, false);
            //AudioClip.SetName(ac, "qweasdqwe");
            //MelonLogger.Msg(ac.GetName() + " ac.samples " + ac.samples);
            //float[] asd = new float[1000];
            //AudioClip.SetData(ac, asd, asd.Length, 0);
            //MelonLogger.Msg(ac.GetName() + " ac.samples " + ac.samples);

            // allora, leggendo sul discord dei modders di TLD
            // TLD non usa l'audio base di unity, ma una lib di audiokinetic che si chiama wwave
            // penso serva per l'audio posizionale, il calcolo dei riverberi, occlusioni, cose cosi

            // TLDR: non è possibile al momento playare clips custom

            // quindi l'unica è mostrare la distanza
            // il massimo che si può fare ora è usare uno dei suoni stock
        }





        public override void OnUpdate()
        {
            if(!Settings.options.EnableMod)return;

            // GameManager esiste sempre, ma i suoi membri no
            // quindi bisogna evitare di invocar roba che non c'è per non generar errori
            // la via del try/catch sembra esser l'unica maniera in cui funzioni sempre
            try { var gpt = GameManager.GetPlayerTransform(); } catch { return; }

            scan();
            //click();
        }









    }
}


