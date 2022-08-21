using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuelScript.utils;

namespace FuelScript.GameObjects {
    static class GameNameModelNameTransformer {
        private static Dictionary<string, string> collection = new Dictionary<string, string>() {
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


        public static string transformOrGetKey(string key) {
            try {
                string value;

                if (collection.TryGetValue(key, out value)) {
                    return value;
                } else return key;
            } catch (Exception ex) {
                Log.error("Error obteniendo transformacion key-value <GameName-ModelName> para key: " + key);
                return key;
            }
        }


    }
}
