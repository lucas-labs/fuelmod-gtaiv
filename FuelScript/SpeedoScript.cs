using System;
using GTA;
using System.Drawing;

namespace FuelScript {
    class SpeedoScript : Script {
        readonly GTA.Font pixelFont;
        readonly Color pixelBoxColor = Color.FromArgb(0, 205, 205, 205);
        readonly Color AlphaGray = Color.FromArgb(127, 205, 205, 205);
        readonly Texture engineTexture;
        readonly Texture chasisTexture;

        public SpeedoScript() {
            pixelFont = new GTA.Font(48.0F, FontScaling.Pixel);
            pixelFont.Color = AlphaGray;

            engineTexture = new Texture(
                (byte[]) new ImageConverter().ConvertTo(
                    Properties.Resources.engine, 
                    typeof(byte[])
                )
            );

            chasisTexture = new Texture(
                (byte[])new ImageConverter().ConvertTo(
                    Properties.Resources.chasis,
                    typeof(byte[])
                )
            );

            this.PerFrameDrawing += new GraphicsEventHandler(this.DrawingExample_PerFrameDrawing);
        }

        private void DrawingExample_PerFrameDrawing(object sender, GraphicsEventArgs e) {
            if (Player.Character.isInVehicle()) {
                Vehicle vehicle = Player.Character.CurrentVehicle;

                if (!(vehicle != null && vehicle.Exists())) return;

                float kph = vehicle.Speed * 2.609344f;

                e.Graphics.Scaling = FontScaling.Pixel; // fixed amount of pixels, size on screen will differ for each resolution
                RectangleF speedRect = new RectangleF(Game.Resolution.Width-180-128, 0, 128, 128);
                e.Graphics.DrawRectangle(speedRect, pixelBoxColor);
                e.Graphics.DrawText(Convert.ToInt32(kph).ToString(), speedRect, TextAlignment.Center | TextAlignment.VerticalCenter, pixelFont);

                RectangleF engineRect = new Rectangle(Game.Resolution.Width - 255 - 128, 0 - 2, 128, 128);
                e.Graphics.DrawSprite(
                    engineTexture, 
                    engineRect, 
                    GetHealthColor(Convert.ToInt32(vehicle.EngineHealth))
                );

                RectangleF chasisRect = new Rectangle(Game.Resolution.Width - 255 - 128 - 66, 0 - 2, 128, 128);
                e.Graphics.DrawSprite(
                    chasisTexture,
                    chasisRect,
                    GetHealthColor(vehicle.Health)
                );
            }
        }

        private Color GetHealthColor(int health)
        {
            if (health > 600)
            {
                return pixelBoxColor;
            }
            else if (health > 500)
            {
                return Color.Yellow;
            }
            else if (health > 300)
            {
                return Color.Orange;
            }
            else
            {
                return Color.Red;
            }
        }
    }
}
