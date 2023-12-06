using System;
using System.Drawing;
using System.Windows.Forms;
using GTA;
using LemonUI;
using LemonUI.Menus;

namespace CollisionFX
{    
    internal class Menu : Script
    {

        // Controls
        static Keys menuKey;

        ObjectPool pool = new ObjectPool();
        NativeMenu collisionFXMenu = new NativeMenu("      CollisionFX");
        NativeSliderItem lengthSlider = new NativeSliderItem("Length Of The Effect", 50, 0);
        NativeSliderItem deltaSlider = new NativeSliderItem("Delta Speed Threshold", 50, 0);

        NativeCheckboxItem toggleTimeCyc = new NativeCheckboxItem("Enable Timecycle");
        NativeCheckboxItem toggleBlur = new NativeCheckboxItem("Enable Blur");
        NativeCheckboxItem toggleReverb = new NativeCheckboxItem("Enable Reverb");
        NativeCheckboxItem toggleShake = new NativeCheckboxItem("Enable Shake");
        public Menu()
        {
            LoadMenu();
            Tick += Menu_Tick;
            KeyDown += onKeyDown;

            // Events
            deltaSlider.ValueChanged += DeltaSlider_ValueChanged;
            lengthSlider.ValueChanged += LengthSlider_ValueChanged;
            toggleTimeCyc.CheckboxChanged += ToggleTimeCyc_CheckboxChanged;
            toggleBlur.CheckboxChanged += ToggleBlur_CheckboxChanged;
            toggleReverb.CheckboxChanged += ToggleReverb_CheckboxChanged;
            toggleShake.CheckboxChanged += ToggleShake_CheckboxChanged;
        }

        void LoadMenu()
        {
            menuKey = Main.CollisionFXSettings.GetValue<Keys>("CONTROLS", "menuKey", Keys.F9);
            Main.CollisionFXSettings.SetValue("CONTROLS", "menuKey", menuKey);
            Main.CollisionFXSettings.Save();
            collisionFXMenu.Width = 520;
            collisionFXMenu.Banner.Color = Color.Empty;
            collisionFXMenu.BannerText.Shadow = true;            
            collisionFXMenu.RotateCamera = true;            

            collisionFXMenu.Add(deltaSlider);
            collisionFXMenu.Add(lengthSlider);
            collisionFXMenu.Add(toggleTimeCyc);
            collisionFXMenu.Add(toggleBlur);
            collisionFXMenu.Add(toggleReverb);
            collisionFXMenu.Add(toggleShake);
            pool.Add(collisionFXMenu);
        }

        void Reload()
        {
            deltaSlider.Value = Main.CollisionFXSettings.GetValue<int>("SETTINGS", "deltaSpeedThreshold", 10);
            lengthSlider.Value = Main.CollisionFXSettings.GetValue<int>("SETTINGS", "effectLength", 10);
            toggleTimeCyc.Checked = Main.CollisionFXSettings.GetValue<bool>("SETTINGS", "toggleTimeCyc", true);
            toggleBlur.Checked = Main.CollisionFXSettings.GetValue<bool>("SETTINGS", "toggleBlur", true);
            toggleReverb.Checked = Main.CollisionFXSettings.GetValue<bool>("SETTINGS", "toggleReverb", true);
            toggleShake.Checked = Main.CollisionFXSettings.GetValue<bool>("SETTINGS", "toggleShake", true);
        }

        void onKeyDown(object sender, KeyEventArgs e)
        {
            // Main menu
            if (e.KeyCode == menuKey && Game.Player.Character.IsInVehicle())
            {
                Toggle();
            }
        }
        private void Menu_Tick(object sender, EventArgs e)
        {
            pool.Process();

            if (collisionFXMenu.Visible)
            {     
                if(Main.deltaSpeed > 1)
                {
                    collisionFXMenu.Name = "Last Significant Delta Speed: ~y~" + Main.deltaSpeed;
                }                
            }           
        }
        void Toggle()
        {
            collisionFXMenu.Visible = !collisionFXMenu.Visible;

            if (collisionFXMenu.Visible)
            {
                Reload();                
            }           
        }
        private void LengthSlider_ValueChanged(object sender, EventArgs e)
        {
            lengthSlider.Description = "Length Multiplier: ~y~" + ((float)lengthSlider.Value / 10);
            Main.CollisionFXSettings.SetValue("SETTINGS", "effectLength", lengthSlider.Value);
            Main.CollisionFXSettings.Save();
            Main.LoadConfig();
        }
        private void DeltaSlider_ValueChanged(object sender, EventArgs e)
        {
            deltaSlider.Description = "Minimum Speed Difference To Trigger The Effect: ~y~" + (deltaSlider.Value);
            Main.CollisionFXSettings.SetValue("SETTINGS", "deltaSpeedThreshold", deltaSlider.Value);
            Main.CollisionFXSettings.Save();
            Main.LoadConfig();
        }
        private void ToggleTimeCyc_CheckboxChanged(object sender, EventArgs e)
        {            
            Main.CollisionFXSettings.SetValue("SETTINGS", "toggleTimeCyc", toggleTimeCyc.Checked);
            Main.CollisionFXSettings.Save();
            Main.LoadConfig();
        }
        private void ToggleBlur_CheckboxChanged(object sender, EventArgs e)
        {
            Main.CollisionFXSettings.SetValue("SETTINGS", "toggleBlur", toggleBlur.Checked);
            Main.CollisionFXSettings.Save();
            Main.LoadConfig();
        }
        private void ToggleReverb_CheckboxChanged(object sender, EventArgs e)
        {
            Main.CollisionFXSettings.SetValue("SETTINGS", "toggleReverb", toggleReverb.Checked);
            Main.CollisionFXSettings.Save();
            Main.LoadConfig();
        }
        private void ToggleShake_CheckboxChanged(object sender, EventArgs e)
        {
            Main.CollisionFXSettings.SetValue("SETTINGS", "toggleShake", toggleShake.Checked);
            Main.CollisionFXSettings.Save();
            Main.LoadConfig();
        }
    }

}
