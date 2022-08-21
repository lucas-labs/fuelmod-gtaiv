using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GTA;
using Newtonsoft.Json;
using FuelScript.utils;

namespace FuelScript.GameObjects {
    class MyVehiclesController {
        private static readonly string JSON_VEHICLES_FILE_PATH = "myVehicles.json";

        public delegate void SendFuelHandler(float fuel, Vehicle vehicle);
        public event SendFuelHandler SendFuel;
        public delegate void UpdateFuelHandler();
        public event UpdateFuelHandler UpdateFuel;

        public bool LoadVehiclesAtConstructionTime { get; set; }
        private List<OwnedVehicle> myVehicleList;
        private readonly List<Blip> blipsList;
        private int intents = 0;
        private readonly Player player;
        
        public MyVehiclesController(Player player) {
            this.player = player;
            blipsList = new List<Blip>();
        }

        // TODO: Necesito un "receptor" de fuel, que desde FuelScript envie cuanta nafta tiene
        // cuando reciba (o le pida a FuelScript que me la mande) actualizo el OwnedVehicle.fuel
        // y necesito un "enviador" de Fuel, para cuando apenas creo el vehicle le mando a FuelScript
        // el valor de la "ultima partida" para que lo setee en vez de setear al azar

        public void Load() {
            try {
                Log.debug("Loading cars...");
                TextReader textReader = new StreamReader(JSON_VEHICLES_FILE_PATH);
                string jsonVehiclesFile = textReader.ReadLine();
                textReader.Close();

                jsonVehiclesFile = (jsonVehiclesFile == null) ? "" : jsonVehiclesFile;

                myVehicleList = JsonConvert.DeserializeObject<List<OwnedVehicle>>(jsonVehiclesFile);
            } catch (Exception crap) {
                Log.error(crap.Message);
                myVehicleList = new List<OwnedVehicle>();
            }
        }

        public void SaveVehicles() {
            try {
                Log.debug("Saving cars");
                TextWriter jsonWriter = new StreamWriter(JSON_VEHICLES_FILE_PATH);
                var json = JsonConvert.SerializeObject(myVehicleList);
                jsonWriter.Write(json);
                jsonWriter.Close();
                Log.debug("Vehicles saved.");
            } catch (Exception ex) {
                Log.error(ex.Message);
            }
        }

        public bool AddVehicle(Vehicle vehicle) {
            if (myVehicleList.Count < 10 && !ImOwnerOf(vehicle)) {
                Log.debug("Adding vehicle " + vehicle.Name);
                myVehicleList.Add(new OwnedVehicle(vehicle));
                Log.debug("Added");
                return true;
            } else return false;
        }

        private bool ImOwnerOf(Vehicle vehicle) {
            foreach (var myVeh in myVehicleList) {
                if (myVeh.vehicleInGame == vehicle) {
                    return true;
                }
            }

            return false;
        }

        private OwnedVehicle GetCurrentOwnedVehicle()
        {
            if (player.Character.isInVehicle())
            {
                var currentVehicle = player.Character.CurrentVehicle;
                // si el current vehicule existe y yo soy el owner
                if (currentVehicle != null && currentVehicle.Exists())
                {
                    var ownVeh = GetOwnedVehicleByInGameVehicle(currentVehicle);
                    if (ownVeh != null)
                    {
                        return ownVeh;
                    }
                    else
                    {
                        Log.debug("Not inside one of my vehicles");
                    }
                }
                {
                    Log.debug("Current vehicule is null....");
                }
            }

            return null;
        }

        private OwnedVehicle GetOwnedVehicleByInGameVehicle(Vehicle vehicle)
        {
            foreach (var myVeh in myVehicleList) {
                if (myVeh.vehicleInGame == vehicle) {
                    return myVeh;
                }
            }

            return null;
        }

        public bool DeleteVehicle(Vehicle vehicle) {
            foreach (var veh in myVehicleList) {
                if (vehicle == veh.vehicleInGame) {
                    Log.debug("Removing vehicle!");
                    return myVehicleList.Remove(veh);
                }
            }

            return false;
        }

        public void UpdateVehicles() {
            foreach (var ownedVeh in myVehicleList) {
                if (ownedVeh.IsInGame()) {
                    ownedVeh.UpdateOwnedVehicleData();
                }
            }
        }

        public void SetFuel(float fuel)
        {
            if (fuel < 0)
            {
                Log.debug("Fuel not Setted, was " + fuel);
                return;
            }

            var ownVeh = GetCurrentOwnedVehicle();
            if (ownVeh == null)
            {
                Log.debug("Cant set fuel, vehicule is null");
                return;
            }

            
            // get fuel if im on an owned vehicle
            try
            {
                ownVeh.Fuel = fuel;
                Log.debug("Setted fuel (" + fuel + ") for " + ownVeh.GameName);
            }
            catch (Exception ex)
            {
                Log.error("Setted not updated, error getting Fuel: " + ex.Message);
            }
        }

        public void UpdateAndSaveVehicles() {
            AskForFuelUpdate();
            UpdateVehicles();
            SaveVehicles();
        }

        private void AskForFuelUpdate()
        {
            if(GetCurrentOwnedVehicle() != null)
            {
                UpdateFuel();
            } else
            {
                Log.debug("Current Owned Vehicle couldn't be obtained");
            }
        }

        public void ManageInGameVehiclesCreation(Vector3 actualPlayerPositon) {
            foreach (var ownedVehicle in myVehicleList) {
                if (actualPlayerPositon.DistanceTo(ownedVehicle.Position) < 200 && !ownedVehicle.IsInGame()) {
                    Log.debug("Creating vehicle " + ownedVehicle.GameName);
                    //Create it
                    SpawnVehicle(ownedVehicle, false);
                }
            }

        }

        public void CreateBlips() {
            foreach (var ownedVehicle in myVehicleList) {
                AttachBlip(ownedVehicle);
            }
        }

        public void DeleteAllBlips() {
            foreach (var blip in blipsList) {
                var vehicle = ((Vehicle)blip.GetAttachedItem());
                
                if (vehicle != null && vehicle.Exists()) vehicle.NoLongerNeeded();
                if (blip != null && blip.Exists()) blip.Delete();
            }

            blipsList.Clear();
        }

        private void AttachBlip(OwnedVehicle ownedVehicle) {
            try {
                CreateVehicleIfNotExists(ownedVehicle);
                var blip = ownedVehicle.vehicleInGame.AttachBlip();
                blip.Icon = BlipIcon.Building_Garage;
                blip.Color = BlipColor.Red;
                blip.ShowOnlyWhenNear = false;
                blip.Name = "My " + ownedVehicle.GameName;

                blipsList.Add(blip);
            } catch (Exception ex) {
                if (ex is NullReferenceException || ex is NonExistingObjectException) {
                    intents++;

                    if (intents < 6) {
                        AttachBlip(ownedVehicle);
                    } else {
                        Game.DisplayText("We can't trace your " + ownedVehicle.GameName + " at this moment.");
                    }
                } else {
                    Log.error(ex.Message);
                }
            }

            intents = 0;
        }

        private void CreateVehicleIfNotExists(OwnedVehicle ownedVehicle) {
            if (!ownedVehicle.IsInGame()) {
                Log.debug("Creating vehicle " + ownedVehicle.GameName);
                //Create it
                var vehInGame = SpawnVehicle(ownedVehicle, true);
            } else {
                ownedVehicle.vehicleInGame.isRequiredForMission = true;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">Throws Exception: Null, </exception>
        private Vehicle SpawnVehicle(OwnedVehicle ownedVehicle, bool needed) {
            try {
                // Some model names are not equal than theirs vehicle game names. 
                // So we get the modelName from settings file 
                // (example: Super GT modelName= 'supergt', since Super GT gameName = 'super').

                Log.debug("Creating vehicle -> " + ownedVehicle.GameName + ":" + GameNameModelNameTransformer.transformOrGetKey(ownedVehicle.GameName));
                var vehInGame = World.CreateVehicle(GameNameModelNameTransformer.transformOrGetKey(ownedVehicle.GameName), ownedVehicle.Position);
                
                if (needed) {
                    vehInGame.isRequiredForMission = true;
                } else {
                    vehInGame.isRequiredForMission = false;
                    vehInGame.NoLongerNeeded();
                }

                vehInGame.Heading = ownedVehicle.Heading;
                vehInGame.Color = ownedVehicle.color;
                vehInGame.SpecularColor = ownedVehicle.SpecularColor;
                vehInGame.FeatureColor1 = ownedVehicle.featuredColor1;
                vehInGame.FeatureColor2 = ownedVehicle.featuredColor2;
                vehInGame.Rotation = ownedVehicle.Rotation;
                vehInGame.RotationQuaternion = ownedVehicle.RotationQuaternion;
                vehInGame.PreviouslyOwnedByPlayer = true;
                vehInGame.Visible = true;
                vehInGame.PlaceOnGroundProperly();

                if(ownedVehicle.Fuel >= 0)
                {
                    Log.debug("Sending fuel to FuelMod: " + ownedVehicle.Fuel);
                    SendFuel(ownedVehicle.Fuel, vehInGame);
                }
                

                ownedVehicle.vehicleInGame = vehInGame;
                return vehInGame;
            } catch (Exception ex) {
                Log.error("Error while spawning " + ownedVehicle.GameName + " vehicle: " + ex.Message);
                return null;
            }
        }

        public void ShowMyVehiclesList() {
            string textToDraw = "";
            
            foreach (var ownedVehicle in myVehicleList) {
                textToDraw += ownedVehicle.GameName + ": " + (ownedVehicle.IsInGame() ? "E" : "N") + "\n";
            }

            if (textToDraw != null && !textToDraw.Equals("")) Game.DisplayText(textToDraw);
        }

    }
}
