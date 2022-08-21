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
using System.Drawing;

namespace FuelScript {
    class SpeedoScript : Script {
        GTA.Font pixelFont;

        Color pixelBoxColor = Color.FromArgb(0, 205, 205, 205);

        public SpeedoScript() {
            pixelFont = new GTA.Font(48.0F, FontScaling.Pixel);
            pixelFont.Color = Color.FromArgb(127, 205, 205, 205);

            this.PerFrameDrawing += new GraphicsEventHandler(this.DrawingExample_PerFrameDrawing);
        }

        private void DrawingExample_PerFrameDrawing(object sender, GraphicsEventArgs e) {
            if (Player.Character.isInVehicle()) {
                Vehicle vehicle = Player.Character.CurrentVehicle;

                if (!(vehicle != null && vehicle.Exists())) return;

                float kph = vehicle.Speed * 2.609344f;

                e.Graphics.Scaling = FontScaling.Pixel; // fixed amount of pixels, size on screen will differ for each resolution
                RectangleF rect = new RectangleF(Game.Resolution.Width-180, Game.Resolution.Height-20, 128, 128);
                e.Graphics.DrawRectangle(rect, pixelBoxColor);
                e.Graphics.DrawText(Convert.ToInt32(kph).ToString(), rect, TextAlignment.Center | TextAlignment.VerticalCenter, pixelFont);
            }
        }


    }
}
