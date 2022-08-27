using AdvancedHookManaged;
using FuelScript.GameObjects;
using FuelScript.utils;
using GTA;
using NativeUI;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace FuelScript
{
    class ICanFixIt : LuScript
    {
        public ICanFixIt() : base(typeof(ICanFixIt).Name)
        {
            Interval = 400;
            KeyDown += ICanFixIt_KeyDown;
        }

        private void ICanFixIt_KeyDown(object sender, GTA.KeyEventArgs e)
        {
            if (e.Key == Keys.F12)
            {
                if (Player.Character.isInVehicle()) TurnOnOffEngine();
                else OpenCloseHood();
            }

            if (e.Key == Keys.F11)
            {
                TryToFixEngine();
            }
        }



        private void TurnOnOffEngine()
        {
            var vehicle = Player.LastVehicle ?? Player.Character.CurrentVehicle;
            if (vehicle == null) return;
            if (vehicle.Metadata.EngineStarted == null) vehicle.Metadata.EngineStarted = true;

            if (vehicle.Metadata.EngineStarted)
            {
                ShowMessage("Setting Off!", 1000);
                vehicle.PetrolTankHealth = 0;
                vehicle.EngineRunning = false;
                vehicle.Metadata.EngineStarted = false;
                vehicle.isRequiredForMission = true;
            }
            else
            {
                ShowMessage("Setting On!", 1000);
                vehicle.PetrolTankHealth = 1000f;
                vehicle.EngineRunning = true;
                vehicle.Metadata.EngineStarted = true;
                vehicle.isRequiredForMission = false;
            }

        }

        private void OpenCloseHood()
        {
            var vehicle = Player.LastVehicle ?? Player.Character.CurrentVehicle;
            if (vehicle == null) return;
            if (!vehicle.Model.isCar) return;

            Vector3 hoodPosition = vehicle.GetOffsetPosition(new Vector3(0.0f, 2.8f, 0.0f));

            if (ImCloseTo(hoodPosition, 3f))
            {
                var hood = vehicle.Door(VehicleDoor.Hood);

                if (hood.isOpen)
                {
                    PerformSequence("close-hood", Sequences.CloseHood, Player.Character, false);
                    Wait(500);
                    hood.Close();
                }
                else
                {
                    PerformSequence("open-hood", Sequences.OpenHood, Player.Character, false);
                    Wait(100);
                    hood.Open();
                }
            }
        }

        private void TryToFixEngine()
        {
            var vehicle = Player.LastVehicle ?? Player.Character.CurrentVehicle;
            if (vehicle == null) return;
            vehicle.Metadata.LastHealedEngine = 0;
            vehicle.Metadata.LastHealed = 0;

            Vector3 position = vehicle.Model.isCar
                ? vehicle.GetOffsetPosition(new Vector3(0.0f, 2.8f, 0.0f))
                : vehicle.Position;

            if (ImCloseTo(position, 3f))
            {
                var hood = vehicle.Door(VehicleDoor.Hood);

                if ((hood != null && hood.isOpen) || vehicle.Model.isBike)
                {
                    int max = vehicle.Metadata.LastHealed > 0 ? vehicle.Metadata.LastHealed : 1000;
                    float maxEngine = vehicle.Metadata.LastHealedEngine > 0 ? vehicle.Metadata.LastHealedEngine : 1000f;

                    if (vehicle.Model.isCar) DoFixCarAnimation(vehicle);
                    if (vehicle.Model.isBike) DoFixBikeAnimation(vehicle);

                    var fixAmmount = RandomInt(150, 250);
                    var engineHealth = (vehicle.EngineHealth + fixAmmount).Clamp(300, maxEngine);
                    var health = (vehicle.Health + fixAmmount).Clamp(300, max);

                    vehicle.EngineHealth = engineHealth;
                    vehicle.Health = health;
                    vehicle.Metadata.LastHealedEngine = engineHealth;
                    vehicle.Metadata.LastHealed = health;

                    ShowMessage("Algo se pudo arreglar...");
                }
                
            }
        }

        private void DoFixBikeAnimation(Vehicle vehicle)
        {
            PerformSequence("fix-bike", Sequences.NikoFixBike, Player.Character);
            if (vehicle.Health <= 300 || vehicle.EngineHealth <= 300)
            {
                // si el vehiculo esta muy roto, insultar un poquito
                PerformSequence("damn-it", Sequences.DamnIt, Player.Character);
                Wait(100);
                Player.Character.SayAmbientSpeech(Sequences.GetRandomCarRageSpeech());
                PerformSequence("damn-it", Sequences.DamnIt, Player.Character);
                Wait(50);
                PerformSequence("fix-bike", Sequences.NikoFixCar2, Player.Character);
            }
        }

        private void DoFixCarAnimation(Vehicle vehicle)
        {
            PerformSequence("fix-car", Sequences.NikoFixCar, Player.Character);
            if (vehicle.Health <= 300 || vehicle.EngineHealth <= 300)
            {
                // si el vehiculo esta muy roto, insultar un poquito
                PerformSequence("damn-it", Sequences.DamnIt, Player.Character);
                Wait(100);
                Player.Character.SayAmbientSpeech(Sequences.GetRandomCarRageSpeech());
                PerformSequence("damn-it", Sequences.DamnIt, Player.Character);
                Wait(50);
                PerformSequence("fix-car", Sequences.NikoFixCar2, Player.Character);
            }
        }
    }
}

