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
        private readonly Logger Log = new Logger(typeof(MyVehiclesController).Name);
        private static readonly string JSON_VEHICLES_FILE_PATH = "myVehicles.json";
        private static readonly string JSON_NAME_MAPS = "scripts\\custom-vehicles-names-map.json";
        private static readonly string JSON_CUSTOM_LABELS = "scripts\\custom-labels.json";

        public delegate void SendFuelHandler(float fuel, Vehicle vehicle);
        public event SendFuelHandler SendFuel;
        public delegate void UpdateFuelHandler();
        public event UpdateFuelHandler UpdateFuel;

        public bool LoadVehiclesAtConstructionTime { get; set; }
        private List<OwnedVehicle> myVehicleList;
        private Dictionary<string, string> customVehicleNames;
        private Dictionary<string, string> customLabels;
        private string labelPrefix;
        private readonly List<Blip> blipsList;
        private int intents = 0;
        private readonly Player player;
        private GameNameModelNameTransformer GameNameTransformer;

        public MyVehiclesController(Player player) {
            this.player = player;
            blipsList = new List<Blip>();
        }

        public void Load() {
            Log.Debug("Loading configs");
            myVehicleList = LoadJsonConfig(JSON_VEHICLES_FILE_PATH, new List<OwnedVehicle>());
            customVehicleNames = LoadJsonConfig(JSON_NAME_MAPS, new Dictionary<string, string>());
            customLabels = LoadJsonConfig(JSON_CUSTOM_LABELS, new Dictionary<string, string>());
            customLabels.TryGetValue("_prefix", out labelPrefix); // get the prefix

            if (labelPrefix == null)
            {
                labelPrefix = "";
            }

            Log.Debug("Blip prefix: " + labelPrefix);

            GameNameTransformer = new GameNameModelNameTransformer(customVehicleNames);
        }

        public Dictionary<string, string> GetNamesReplacers()
        {
            return GameNameTransformer.Collection;
        }

        public void SaveVehicles() {
            try {
                TextWriter jsonWriter = new StreamWriter(JSON_VEHICLES_FILE_PATH);
                var json = JsonConvert.SerializeObject(myVehicleList);
                jsonWriter.Write(json);
                jsonWriter.Close();
            } catch (Exception ex) {
                 Log.Error(ex.Message);
            }
        }

        public bool AddVehicle(Vehicle vehicle) {
            if (myVehicleList.Count < 10 && !ImOwnerOf(vehicle)) {
                // Logger.debug("Adding vehicle " + vehicle.Name);
                myVehicleList.Add(new OwnedVehicle(vehicle));
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
                        // Logger.debug("Not inside one of my vehicles");
                    }
                }
                {
                    // Logger.debug("Current vehicule is null....");
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

        private T LoadJsonConfig<T>(string path, T defVal)
        {
            try
            {
                TextReader reader = new StreamReader(path);
                string jsonString = reader.ReadToEnd();
                reader.Close();
                jsonString = jsonString ?? "";
                Log.Debug(path + " loaded: " + jsonString);
                return JsonConvert.DeserializeObject<T>(jsonString);
            }
            catch (Exception ex)
            {
                Log.Error("Error loading " + path + ": " + ex.Message);
                return defVal;
            }
        }

        public bool DeleteVehicle(Vehicle vehicle) {
            foreach (var veh in myVehicleList) {
                if (vehicle == veh.vehicleInGame) {
                    // Logger.debug("Removing vehicle!");
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
                // Logger.debug("Fuel not Setted, was " + fuel);
                return;
            }

            var ownVeh = GetCurrentOwnedVehicle();
            if (ownVeh == null)
            {
                // Logger.debug("Cant set fuel, vehicule is null");
                return;
            }

            
            // get fuel if im on an owned vehicle
            try
            {
                ownVeh.Fuel = fuel;
                // Logger.debug("Setted fuel (" + fuel + ") for " + ownVeh.GameName);
            }
            catch (Exception ex)
            {
                // Logger.error("Setted not updated, error getting Fuel: " + ex.Message);
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
                // Logger.debug("Current Owned Vehicle couldn't be obtained");
            }
        }

        public void ManageInGameVehiclesCreation(Vector3 actualPlayerPositon) {
            foreach (var ownedVehicle in myVehicleList) {
                if (actualPlayerPositon.DistanceTo(ownedVehicle.Position) < 200 && !ownedVehicle.IsInGame()) {
                    // Logger.debug("Creating vehicle " + ownedVehicle.GameName);
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
                blip.Name = GetLabel(ownedVehicle.GameName);

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
                    // Logger.error(ex.Message);
                }
            }

            intents = 0;
        }

        private string GetLabel(string vehicleName)
        {
            if (customLabels.TryGetValue(vehicleName, out var label))
            {
                return labelPrefix + label;
            }
            else return labelPrefix + vehicleName;            
        }

        private void CreateVehicleIfNotExists(OwnedVehicle ownedVehicle) {
            if (!ownedVehicle.IsInGame()) {
                // Logger.debug("Creating vehicle " + ownedVehicle.GameName);
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
                // (example: Super GT modelName= 'supergt', Super GT gameName = 'super').
                // Logger.debug("Creating vehicle -> " + ownedVehicle.GameName + ":" + GameNameModelNameTransformer.transformOrGetKey(ownedVehicle.GameName));
                var vehInGame = World.CreateVehicle(GameNameTransformer.TransformOrGetKey(ownedVehicle.GameName), ownedVehicle.Position);
                
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
                    // Logger.debug("Sending fuel to FuelMod: " + ownedVehicle.Fuel);
                    SendFuel(ownedVehicle.Fuel, vehInGame);
                }
                

                ownedVehicle.vehicleInGame = vehInGame;
                return vehInGame;
            } catch (Exception ex) {
                // Logger.error("Error while spawning " + ownedVehicle.GameName + " vehicle: " + ex.Message);
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
