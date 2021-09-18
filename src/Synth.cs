



using System;
using UnityEngine;


namespace ArrowTracker
{
    
    public class Synth
    {       
        const int samplerate = 8000;

        // parametri per OnAudioRead
        static float frequency;
        static float attack = 0.1f;
        static float release = 0.01f;

        void OnAudioRead(float[] data)
        {
            // genera una sinusoide con fasi di attack e release
            
            int release_start = (int)(samplerate * 0.1);

            float gain_tgt = 1;
            float rate = attack;

            float gain = 0;
            int count = 0;
            while (count < data.Length)
            {
                data[count] = (float)Math.Sin(2 * Math.PI * frequency * count / samplerate) * gain;
                gain += (gain_tgt-gain)*rate;
                if(count==release_start){
                    gain_tgt = 0;
                    rate = release;
                }
                count++;
            }
        }

        public static AudioClip ping( string name, float hz ){
            frequency = hz;
            int cliplen = (int)(samplerate * 0.1);
            return AudioClip.Create(name, cliplen, 1, samplerate, false, OnAudioRead);
        }


    }
}


