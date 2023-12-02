using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GTA;
using GTA.Math;
using GTA.Native;
using static System.TimeZoneInfo;

namespace CollisionFX
{
    public class Main : Script
    {
        float oldSpeed = 0.0f;
        int frameSkip = 0;
        int soundTimer = 0;
        float blurThreshold = 7.0f;
        float blurFadeOutTime = 0;
        bool timecycFadedOut = true;
        bool blurFadedOut = true;
        float progress = 0.0f;
        float maxDelta = 0.0f;
        float minDelta = 0.0f;

        public Main()
        {
            Tick += onTick;
        }

        public void onTick(object sender, EventArgs e)
        {
            Ped player = Game.Player.Character;

            if (player.IsInVehicle())
            {
                Vehicle veh = player.CurrentVehicle;
                float deltaSpeed = veh.Speed - oldSpeed;

                if(deltaSpeed > maxDelta)
                {
                    maxDelta = deltaSpeed;
                }
                if (deltaSpeed < minDelta)
                {
                    minDelta = deltaSpeed;
                }

                //GTA.UI.Screen.ShowSubtitle("maxDelta: " + maxDelta + " minDelta: " + minDelta);

                if (frameSkip > 3)
                {
                    oldSpeed = veh.Speed;
                }
                else
                {
                    frameSkip++;
                }

                // Timecycle effect
                if (Math.Abs(deltaSpeed) > blurThreshold && timecycFadedOut)
                {
                    progress = 1.4f;
                    if (progress > 1.4f)
                    {
                        progress = 1.4f;
                    }
                    Function.Call(Hash.SET_TIMECYCLE_MODIFIER, "NG_filmic18");
                    Function.Call(Hash.SET_TIMECYCLE_MODIFIER_STRENGTH, progress);
                    timecycFadedOut = false;
                }
                else if (!timecycFadedOut)
                {
                    progress -= 0.002f;
                    Function.Call(Hash.SET_TIMECYCLE_MODIFIER_STRENGTH, progress);
                    if (progress <= 0.0f)
                    {
                        progress = 0.0f;
                        timecycFadedOut = true;
                    }
                }

                // Blur
                if (Math.Abs(deltaSpeed) > blurThreshold && blurFadedOut)
                {
                    Function.Call(Hash.SHAKE_CAM, GameplayCamera.MemoryAddress, "JOLT_SHAKE", 1.0f);
                    Function.Call(Hash.TRIGGER_SCREENBLUR_FADE_IN, 50.0f);
                    blurFadeOutTime = 10000;
                }
                float blurFadeInState = Function.Call<float>(Hash.GET_SCREENBLUR_FADE_CURRENT_TIME);
                if (blurFadeInState >= 50.0f && blurFadedOut)
                {
                    Function.Call(Hash.TRIGGER_SCREENBLUR_FADE_OUT, blurFadeOutTime);
                    blurFadedOut = false;
                }
                else if (blurFadeInState <= 0.0f && !blurFadedOut)
                {
                    blurFadeOutTime = 0.0f;
                    blurFadedOut = true;
                }
                // Sound blur
                if (Math.Abs(deltaSpeed) > blurThreshold)
                {
                    soundTimer = 500;
                }
                if (soundTimer > 0)
                {
                    Function.Call(Hash.SET_AUDIO_SPECIAL_EFFECT_MODE, 2);
                    soundTimer--;
                }
            }
            else
            {
                blurFadeOutTime = 0.0f;
                Function.Call(Hash.TRIGGER_SCREENBLUR_FADE_OUT, 1.0f);
            }
        }
    }    
}
