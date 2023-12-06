using System;
using GTA;
using GTA.Native;

namespace CollisionFX
{
    public class Main : Script
    {
        static float oldSpeed = 0.0f;
        static int frameSkip = 0;
        public static float deltaSpeed = 0.0f;
        float reverbTimeLeft = 0;      
        bool blurFadingOut = false;

        // Settings
        public static ScriptSettings CollisionFXSettings;
        string path = "scripts\\CollisionFX.ini";

        static int deltaThreshold = 0;
        static float length = 0f;
        static bool reverb = false;
        static bool timeCyc = false;
        static bool blur = false;
        static bool shake = false;

        float reverbTime = 500;
        float blurTime = 5000f;
        float timeCycTime = 45f;

        public static void LoadConfig()
        {
            length = (float)CollisionFXSettings.GetValue<int>("SETTINGS", "effectLength", 10) / 10;
            deltaThreshold = CollisionFXSettings.GetValue<int>("SETTINGS", "deltaSpeedThreshold", 10);
            timeCyc = CollisionFXSettings.GetValue<bool>("SETTINGS", "toggleTimeCyc", true);
            blur = CollisionFXSettings.GetValue<bool>("SETTINGS", "toggleBlur", true);
            reverb = CollisionFXSettings.GetValue<bool>("SETTINGS", "toggleReverb", true);
            shake = CollisionFXSettings.GetValue<bool>("SETTINGS", "toggleShake", true);
        }
        public Main()
        {
            LoadScriptSettings();
            LoadConfig();
            Tick += onTick;
        }

        public void onTick(object sender, EventArgs e)
        {           
            Ped player = Game.Player.Character;
            FadeOutEffects();

            if (player.IsInVehicle())
            {
                deltaSpeed = GetDeltaSpeed();

                if(deltaSpeed > deltaThreshold)
                {
                    TriggerEffect();
                }

                // Debug
                float blurTime = Function.Call<float>(Hash.GET_SCREENBLUR_FADE_CURRENT_TIME);                
                //GTA.UI.Screen.ShowSubtitle("deltaSpeedThreshold: " + deltaThreshold + " blurTime: " + blurTime);
            }            
        }
        public static float GetDeltaSpeed()
        {
            Ped player = Game.Player.Character;
            Vehicle veh = player.CurrentVehicle;
            float ds = veh.Speed - oldSpeed;

            if (frameSkip > 3)
            {
                oldSpeed = veh.Speed;
            }
            else
            {
                frameSkip++;
            }            
            return Math.Abs(ds);
        }
        private void TriggerEffect()
        {
            // Shake
            if (shake)
            {
                Function.Call(Hash.SHAKE_GAMEPLAY_CAM, "JOLT_SHAKE", 1.0f);
            }

            // Timecycle effect
            if (timeCyc)
            {
                Function.Call(Hash.SET_TIMECYCLE_MODIFIER, "NG_filmic18");
                Function.Call(Hash.SET_TIMECYCLE_MODIFIER_STRENGTH, 1.0f);
                Function.Call(Hash.SET_TRANSITION_OUT_OF_TIMECYCLE_MODIFIER, timeCycTime * length);
            }

            // Blur
            if (blur)
            {
                Function.Call(Hash.TRIGGER_SCREENBLUR_FADE_IN, 1.0f);
                blurFadingOut = false;
            }

            // Reverb
            if (reverb)
            {
                reverbTimeLeft = reverbTime * length;
            }
        }
        private void FadeOutEffects()
        {
            // Blur            
            if (!blurFadingOut)
            {
                Function.Call(Hash.TRIGGER_SCREENBLUR_FADE_OUT, blurTime * length);
                blurFadingOut = true;
            }           

            // Reverb
            if (reverbTimeLeft > 0)
            {
                Function.Call(Hash.SET_AUDIO_SPECIAL_EFFECT_MODE, 2);
                reverbTimeLeft--;
            }
        }
        private void LoadScriptSettings()
        {
            CollisionFXSettings = ScriptSettings.Load(path);
        }
    }    
}
