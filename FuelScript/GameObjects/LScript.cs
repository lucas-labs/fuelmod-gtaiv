using FuelScript.utils;
using GTA;
using System;

namespace FuelScript.GameObjects
{
    public abstract class LuScript : Script
    {
        private readonly Random rand;
        protected readonly Logger Log;
        public LuScript(string name)
        {
            rand = new Random();
            Log = new Logger(name);
        }

        protected void PerformSequence(string name, TaskSequence seq, Ped ped)
        {
            PerformSequence(name, seq, ped, true);
        }

        protected void PerformSequence(string name, TaskSequence seq, Ped ped, bool wait)
        {
            Log.Debug("Starting sequence " + name);
            var datingSet = new AnimationSet("amb@dating");

            // agrego esta animacion al final de la secuencia como "indicador" de que termino... una truchada pero bue, otra no me anduvo
            // esta animacion solo funciona parado y tambien en auto y creo que no es muy comun (se da en algun momento en las citas)...
            // la idea es detectar cuando se esté ejecutando esta animacion para determinar el fin de la secuencia.
            if (wait)
            {
                seq.AddTask.PlayAnimation(datingSet, "niko_incar_partial", 1.0f);
            }

            // ejecuto la secuencia
            ped.Task.PerformSequence(seq);

            if (wait)
            {
                bool hasEnded;
                var count = 0;
                do
                {
                    hasEnded = ped.Animation.isPlaying(datingSet, "niko_incar_partial");
                    if (!hasEnded) Wait(100);

                    if (count >= 1200) // espero 2 minutos
                    {
                        throw new Exception("2 minutos sin finalizar la tarea");
                    }
                } while (!hasEnded);

                ped.Task.ClearAllImmediately();
                ped.Task.ClearAll();
                Log.Debug("Sequence " + name + " has ended");
                return;
            }

            // sequence
            Log.Debug("Sequence " + name + " is running (but we're not waiting for it to finish)");
        }

        protected void ShowMessage(string message)
        {
            ShowMessage(message, 4000, false);
        }

        protected void ShowMessage(string message, int time)
        {
            ShowMessage(message, time, false);
        }

        protected void ShowMessage(string message, int time, bool logIt)
        {
            if(logIt) Log.Debug(message);

            GTA.Native.Function.Call("PRINT_STRING_WITH_LITERAL_STRING_NOW", "STRING", message, time, true);
        }

        protected bool ImCloseTo(Vector3 position)
        {
            return ImCloseTo(position, 5f);
        }

        protected bool ImCloseTo(Vector3 position, float howClose)
        {
            if (position == null) return false;
            var distance = Player.Character.Position.DistanceTo(position);
            if (distance <= howClose) return true;
            else return false;
        }

        protected int RandomInt(int min, int max)
        {
            return rand.Next(min, max);
        }

        protected float RandomFloat(float min, float max)
        {
            return (float) rand.NextDouble() * (min - max) + min;
        }
    }

    
}
