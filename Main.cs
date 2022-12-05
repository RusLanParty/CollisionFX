using System;
using System.Collections.Generic;
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
        Vehicle car;
        float speed;
        float afterSpeed;
        float force;
        int i;
        float mod;
        float tth;
        float dif;
        float inRecColSpeed;
        float inRecAfterColSpeed;
        float inRecDifSpd;
        int camType;
        bool fp;
        double flashThresh;
        double effectThresh;
        int effectType;
        bool flashEnabled;



public Main()
        {
            camType = Settings.GetValue("SETTINGS", "ShakeType", 0);
            fp = Settings.GetValue("SETTINGS", "FirstPersonOnly", false);
            flashThresh = Settings.GetValue("SETTINGS", "FlashThreshold", 1.0f);
            effectThresh = Settings.GetValue("SETTINGS", "BlurThreshold", 2.0f);
            effectType = Settings.GetValue("SETTINGS", "effectType", 2);
            flashEnabled = Settings.GetValue("SETTINGS", "flash", true);
            Tick += onTick;

        }

        private void flash()
        {
            //FLASH


            Function.Call(Hash.SET_TIMECYCLE_MODIFIER, "PlayerSwitchPulse");
            i = 0;
            while (i < 1)
            {
                Function.Call(Hash.SET_TIMECYCLE_MODIFIER_STRENGTH, 1.0f);
                Wait(0);
                i++;
            }
            while (i > 0)
            {
                Function.Call(Hash.SET_TIMECYCLE_MODIFIER_STRENGTH, 0.0f);
                Wait(0);
                i--;
            }

            //\FLASH
        }
        public void effect()
        {
            Ped player = Game.Player.Character;

            car = player.CurrentVehicle;
         
                speed = car.Speed;
                force = 0;

            if (car.HasCollided)
            {
                Wait(6);
                    afterSpeed = car.Speed;
                    force = (int)speed - (int)afterSpeed;

                    if (force < 0)
                    {
                        force = force * -1;
                    }
            }
           
            if (force > flashThresh && flashEnabled)
            {
                flash();
            }


            if (force > effectThresh)
            {

                mod = force / 50;

                if (mod > 1.0f)
                {
                    mod = 1.0f;
                }

                GTA.GameplayCamera.StopShaking();

                if (camType == 0) { GTA.GameplayCamera.Shake(CameraShake.DeathFail, mod); }
                if (camType == 1) { GTA.GameplayCamera.Shake(CameraShake.SmallExplosion, mod); }
                if (camType == 2) { GTA.GameplayCamera.Shake(CameraShake.Jolt, mod); }

                force = force * 15;

                if (force >= 200)
                {
                    force = 200;
                }

                switch (effectType)
                {
                    case 0:
                        Function.Call(Hash._CLEAR_EXTRA_TIMECYCLE_MODIFIER);
                        Function.Call(Hash.SET_TIMECYCLE_MODIFIER, "NG_filmic18");
                         break;

                    case 1:
                        Function.Call(Hash._CLEAR_EXTRA_TIMECYCLE_MODIFIER);
                        Function.Call(Hash.SET_TIMECYCLE_MODIFIER, "hud_def_blur");
                        break;
                    case 2:
                        Function.Call(Hash.SET_TIMECYCLE_MODIFIER, "NG_filmic18");
                        Function.Call(Hash._SET_EXTRA_TIMECYCLE_MODIFIER, "hud_def_blur");
                        break;
                }
              
                i = 0;
                mod = 1.0f;
                tth = force;
                dif = 1 / tth;
                
                while (force >= i)
                {
                    Function.Call(Hash.SET_TIMECYCLE_MODIFIER_STRENGTH, (float)mod);
                    Function.Call(Hash._SET_EXTRA_TIMECYCLE_MODIFIER_STRENGTH, (float)mod);
                    tth = force - i;
                    Wait(1);
                    mod = mod - dif;
                    i++;
                    inRecColSpeed = car.Speed;

                    if (mod < 0)
                    {
                        mod = 0;
                    }

                   // GTA.UI.Screen.ShowHelpText("Effect: " + mod + " TTH: " + tth + " DIF: " + dif + " Force: " + force, 5000, false, false);

                    if (car.HasCollided)
                    {
                        Wait(2);
                        inRecAfterColSpeed = car.Speed;
                        inRecDifSpd = inRecColSpeed - inRecAfterColSpeed;

                        if (inRecDifSpd < 0)
                        {
                            inRecDifSpd = inRecDifSpd * -1;
                        }

                        if (inRecDifSpd > effectThresh)
                        {
                            inRecDifSpd = inRecDifSpd * 10;
                            force = inRecDifSpd + tth;
                            mod = 1.0f;
                            dif = 1 / force;
                            i = 0;
                        }
                    }
                }
            }
        }

        public void onTick(object sender, EventArgs e)
        {
            Ped player = Game.Player.Character;
           
            
            if (player.IsInVehicle())
            {
                if (fp)
                    camType = Function.Call<int>(Hash.GET_FOLLOW_VEHICLE_CAM_VIEW_MODE);
                {
                    if (camType == 4) 
                    {
                        effect();
                    }
                }    if (!fp)
                {
                    effect();
                }
               
            } else
            {
                i = 0;
                tth = 0;
                force = 0;
                
            }
        }
    }
}
