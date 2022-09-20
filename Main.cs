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

namespace CrashBlur
{
    public class Main : Script
    {
        Vehicle car;
        float speed;
        float afterSpeed;
        int time;
        int aftertime;
        int res;
        bool skiptime=false;
        float force;
        float i;
        float mod;
        float tth=0;
        float dif;
        bool inRec=false;
        float inRecColSpeed;
        float inRecAfterColSpeed;
        float inRecDifSpd;
        int camType;
        bool fp;
        


public Main()
        {
            camType = Settings.GetValue("SETTINGS", "ShakeType", 200);
            fp = Settings.GetValue("SETTINGS", "FirstPersonOnly", false);
            Tick += onTick;

        }

        public void effect()
        {
            Ped player = Game.Player.Character;

            car = player.CurrentVehicle;

            if ((!skiptime) && (!inRec))
            {
                speed = car.Speed;
                time = Game.GameTime;
                skiptime = true;
            }
            aftertime = Game.GameTime;
            res = aftertime - time;
            if (res > 2)
            {
                skiptime = false;
                inRec = false;
                afterSpeed = car.Speed;
                force = speed - afterSpeed;

                if (force < 0)
                {
                    force = force * -1;
                }



            }
            if ((force > 2) && (car.HasCollided) || (tth > 0))
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





                if (force > 2 || (tth > 0))
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

                    Function.Call(Hash.SET_TIMECYCLE_MODIFIER, "NG_filmic18");

                    //BLUR
                    i = 0;
                    while (i <= 2)
                    {
                        Function.Call(Hash.SET_TIMECYCLE_MODIFIER_STRENGTH, (float)i / 3);
                        Wait(0);
                        i++;
                    }


                    force = force + tth;


                    i = 0;
                    mod = 1.0f;

                    while ((force >= i) && (!inRec))
                    {
                        tth = (int)force - (int)i;

                        Function.Call(Hash.SET_TIMECYCLE_MODIFIER_STRENGTH, (float)mod);
                        Wait(1);
                        dif = 1 / tth;

                        mod = mod - dif;
                        i++;
                        // GTA.UI.Screen.ShowSubtitle("You're ~r~injured~s~ (inside a car), recovering: ~b~" + tth + " ~s~MODIFIER STR: ~r~" + mod, 3000);
                        inRecColSpeed = car.Speed;
                        if (car.HasCollided)
                        {
                            Wait(1);
                            inRecAfterColSpeed = car.Speed;
                            inRecDifSpd = inRecColSpeed - inRecAfterColSpeed;
                            if (inRecDifSpd < 0)
                            {
                                inRecDifSpd = inRecDifSpd * -1;
                            }
                            if (inRecDifSpd > 2)
                            {
                                i = 0;
                                inRec = true;
                                speed = car.Speed;
                                time = Game.GameTime;
                                mod = 1.0f;
                            }


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
                inRec = false;
                
            }
         


        }
    }
}
