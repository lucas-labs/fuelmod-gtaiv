using System;
using GTA;
using Newtonsoft.Json;

namespace FuelScript.GameObjects {
    class OwnedVehicle {
        [JsonIgnore]
        public Vehicle vehicleInGame { get; set;}

        public ColorIndex color { get; set; }
        public ColorIndex featuredColor1 { get; set; }
        public ColorIndex featuredColor2 { get; set; }   
        public int Health { get; set; }
        public float Heading { get; set; }
        public ColorIndex SpecularColor { get; set; }
        public string GameName { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Quaternion RotationQuaternion { get; set; }
        public float Fuel { get; set; }

        public OwnedVehicle (Vehicle vehicle) {
            vehicleInGame = vehicle;
            SetAllFromVehicle(vehicle);
        }

        public OwnedVehicle() { }

        public bool IsInGame()
        {
            return (vehicleInGame != null && vehicleInGame.Exists());
        }

        public void UpdateOwnedVehicleData() {
            SetAllFromVehicle(vehicleInGame);
        }

        /// <exception cref="Exception">Throws exception when null or invalid vehicle is passed as parameter</exception>
        private void SetAllFromVehicle(Vehicle vehicle) {
            if(vehicle == null) throw new Exception("Null Vehicle");
            if (vehicleInGame != null && !vehicleInGame.Exists() && vehicle != vehicleInGame) throw new Exception("Vehicle is not the owned vehicle");
            color = vehicle.Color;
            featuredColor1 = vehicle.FeatureColor1;
            featuredColor2 = vehicle.FeatureColor2;
            Health = vehicle.Health;
            GameName = vehicle.Name;
            Position = vehicle.Position;
            Rotation = vehicle.Rotation;
            Heading = vehicle.Heading;
            RotationQuaternion = vehicle.RotationQuaternion;
            SpecularColor = vehicle.SpecularColor;

            if(Fuel != -1)
            {
                this.Fuel = Fuel;
            }
        }
    }
}
