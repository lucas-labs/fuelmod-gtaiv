using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTA;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;
using FuelScript.GameObjects;
using FuelScript.utils;

namespace FuelScript {
    class VehicleOwnerScript : Script {
        readonly MyVehiclesController myVehiclesController;
        private Guid FuelMod = new Guid("3583e09d-6c44-4820-85e9-93926307d4f8");
        private bool blipsEnabled = false;
        private bool carColorsEnabled = false;
        private readonly Logger Log = new Logger(typeof(VehicleOwnerScript).Name);

        public VehicleOwnerScript() {
            KeyUp += new GTA.KeyEventHandler(VehicleOwnerScript_KeyUp);

            // Load my vehicles (El constructor se encargará de cargar todos los vehiculos mios si se le pasa true)
            Log.Debug("Intializizing");

            BindScriptCommand("CurrentFuel", new ScriptCommandDelegate(CurrentFuel));

            myVehiclesController = new MyVehiclesController(Player);
            myVehiclesController.UpdateFuel += new MyVehiclesController.UpdateFuelHandler(AskForFuelStatus);
            myVehiclesController.SendFuel += new MyVehiclesController.SendFuelHandler(SetFuel);
            myVehiclesController.Load();

            Interval = 2000;
            this.Tick += new EventHandler(VehicleOwnerScript_Tick);
        }

        private void CurrentFuel(Script sender, ObjectCollection Parameter)
        {
            try
            {
                Log.Debug("Received FuelUpdate from FuelMod #" + Parameter.Count());
                float fuel = Parameter.Convert<float>(0);
                Log.Debug("Fuel is " + fuel);
                myVehiclesController.SetFuel(fuel);
            }
            catch (Exception crap) { Log.Error("ERROR: SetVehicleFuel " + crap.Message); }
        }

        private void AskForFuelStatus()
        {
            Log.Debug("Asking for current fuel to FuelMod");
            SendScriptCommand(FuelMod, "GetCurrentFuel");
        }

        private void SetFuel(float fuel, Vehicle vehicle)
        {
            Log.Debug("Sending vehicle fuel to FuelMod " + vehicle.Name + " -> " + fuel);
            SendScriptCommand(FuelMod, "SetVehicleFuel", fuel, vehicle, "sendingFuel", ""+fuel);
        }

        void VehicleOwnerScript_Tick(object sender, EventArgs e) {
            myVehiclesController.UpdateAndSaveVehicles();
            myVehiclesController.ManageInGameVehiclesCreation(Player.Character.Position);
        }

        void VehicleOwnerScript_KeyUp(object sender, GTA.KeyEventArgs e) {
            //TODO : Menu!
            if (e.Key == Keys.O) {
                if (Player.Character.CurrentVehicle != null && Player.Character.CurrentVehicle.Exists()) {
                    if (myVehiclesController.AddVehicle(Player.Character.CurrentVehicle)) {
                        Game.DisplayText("Ahora este vehiculo es tuyo!");
                    } else {
                        Game.DisplayText("No puedes poseer este vehiculo");
                    }
                }
            }

            if (e.Key == Keys.NumLock) manageBlips();

            if (e.Key == Keys.PageDown) bringMyCarHere();

            if (e.Key == Keys.L) myVehiclesController.ShowMyVehiclesList();

            if (e.Key == Keys.PageUp) {

                if (Player.Character.isInVehicle()) carColorsEnabled = !carColorsEnabled;
                else return;

                /*
                if (carColorsEnabled) {
                    Log.Debug("PAUSE_GAME!");
                    GTA.Native.Function.Call("PAUSE_GAME");
                } else {
                    Log.Debug("Unpausing game");
                    GTA.Native.Function.Call("UNPAUSE_GAME");                    
                }*/
            }

            if (carColorsEnabled) {
                var currentVehicle = Player.Character.CurrentVehicle;
                if (!(currentVehicle != null && currentVehicle.Exists())) return;
                carColorManage(e, currentVehicle);
            }
        }

        private void carColorManage(GTA.KeyEventArgs e, Vehicle currentVehicle) {
            if (e.Key == Keys.NumPad7) currentVehicle.Color = new ColorIndex(getNewColorIndex(currentVehicle.Color.Index, false));
            if (e.Key == Keys.NumPad9) currentVehicle.Color = new ColorIndex(getNewColorIndex(currentVehicle.Color.Index, true));
            if (e.Key == Keys.NumPad4) currentVehicle.FeatureColor1 = new ColorIndex(getNewColorIndex(currentVehicle.FeatureColor1.Index, false));
            if (e.Key == Keys.NumPad6) currentVehicle.FeatureColor1 = new ColorIndex(getNewColorIndex(currentVehicle.FeatureColor1.Index, true));
            if (e.Key == Keys.NumPad1) currentVehicle.FeatureColor2 = new ColorIndex(getNewColorIndex(currentVehicle.FeatureColor2.Index, false));
            if (e.Key == Keys.NumPad3) currentVehicle.FeatureColor2 = new ColorIndex(getNewColorIndex(currentVehicle.FeatureColor2.Index, true));
            if (e.Key == Keys.NumPad2) currentVehicle.SpecularColor = new ColorIndex(getNewColorIndex(currentVehicle.SpecularColor.Index, false));
            if (e.Key == Keys.NumPad8) currentVehicle.SpecularColor = new ColorIndex(getNewColorIndex(currentVehicle.SpecularColor.Index, true));
        }

        private int getNewColorIndex(int index, bool sum) {
            var newColorIndex = sum ? (index + 1) : (index - 1);
            if (newColorIndex > 133) newColorIndex = 0;
            if (newColorIndex < 0) newColorIndex = 133;

            return newColorIndex;
        }

        private void bringMyCarHere() {
            Blip waypoint = Game.GetWaypoint();

            if (waypoint != null && waypoint.Exists()) {
                try {
                    Log.Debug("bringMyCarHere start");
                    var vehicle = World.GetClosestVehicle(waypoint.Position, 100);

                    if (vehicle != null && vehicle.Exists()) {
                        Log.Debug("Ok, the waypoint is a vehicle!");
                        Game.FadeScreenOut(1000, true);
                        vehicle.Position = Player.Character.Position.Around(50.0f);
                        vehicle.PlaceOnNextStreetProperly();
                        Game.FadeScreenIn(1000, true);
                        AdvancedHookManaged.AGame.PrintText("Now your vehicle " + ((Vehicle)vehicle).Name + " is near to you!");
                    } else {
                        Log.Error("Vehicle is null or not exist in game :(");
                    }
                } catch (Exception ex) {
                    Log.Error("Error @ bringMyCarHere " + ex.Message);
                }
            }
        }

        private void manageBlips() {
            if(blipsEnabled){
                myVehiclesController.DeleteAllBlips();
            } else {
                myVehiclesController.CreateBlips();
            }

            blipsEnabled = !blipsEnabled;
        }



    }
}
