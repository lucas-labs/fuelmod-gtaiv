using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace FuelScript.GameObjects {
    public class GameNameModelNameTransformer {
        public readonly Dictionary<string, string> Collection = new Dictionary<string, string>() {
            {"AMBULAN","ambulance"},
            {"CAVCADE","cavalcade"},
            {"CHAV","chavos"},
            {"COGNONTI","cognoscenti"},
            {"DILANTE","dilettante"},
            {"EMPEROR","emperor"},
            {"ESPERNTO","esperanto"},
            {"FORK","forklift"},
            {"HABANRO","habanero"},
            {"HUNT","huntley"},
            {"INTRUD","intruder"},
            {"LANSTALK","landstalker"},
            {"MINVAN","minivan"},
            {"MOONB","moonbeam"},
            {"NSTOCK","nstockade"},
            {"PEREN","perennial"},
            {"PEREN2","perennial2"},
            {"PINCLE","pinnacle"},
            {"POLPAT","polpatriot"},
            {"PSTOCK","pstockade"},
            {"ALBANY","rom"},
            {"STOCK","stockade"},
            {"SUPER","supergt"},
            {"TRUSH","trash"},
            {"WILARD","willard"},
        };

        public GameNameModelNameTransformer(Dictionary<string, string> customVehiclesNames)
        {
            // agrego los que vengan por json
            foreach (var veh in customVehiclesNames) 
            {
                Collection[veh.Key] = veh.Value;
            }
        }

        public string TransformOrGetKey(string key) {
            try {
                if (Collection.TryGetValue(key, out string value))
                {
                    return value;
                }
                else return key;
            } catch {
                //Logger.error("Error obteniendo transformacion key-value <GameName-ModelName> para key: " + key);
                return key;
            }
        }
    }
}
