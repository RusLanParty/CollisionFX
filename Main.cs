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
        float oldSpeed = 0.0f;
        bool fadedOut = true;
        float modifier = 0.0f;
        int frameSkip = 0;

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
                if(frameSkip > 3)
                {                    
                    oldSpeed = veh.Speed;
                }
                else
                {
                    frameSkip++;
                }
                if(Math.Abs(deltaSpeed) > 1.0f && fadedOut)
                {
                    modifier = Math.Abs(deltaSpeed) * 2;
                    if (modifier > 1.0f)
                    {
                        modifier = 1.0f;
                    }                    
                    Function.Call(Hash.SET_TIMECYCLE_MODIFIER, "NG_filmic18");                    
                    Function.Call(Hash.SET_TIMECYCLE_MODIFIER_STRENGTH, modifier);
                    fadedOut = false;
                }
                else if (!fadedOut)
                {
                    modifier -= 0.01f;
                    Function.Call(Hash.SET_TIMECYCLE_MODIFIER_STRENGTH, modifier);
                    if (modifier <= 0.0f)
                    {
                        modifier = 0.0f;
                        fadedOut = true;
                    }
                }
            }
        }       
    }
}
