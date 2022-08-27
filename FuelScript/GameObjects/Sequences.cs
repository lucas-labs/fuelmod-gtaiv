using GTA;
using System;

namespace FuelScript.GameObjects
{
    internal class Anim
    {
        public AnimationSet set;
        public string name;
    }

    public static class Sequences
    {
        public static TaskSequence OpenHood {
            get
            {
                TaskSequence OpenHood = new TaskSequence();
                OpenHood.AddTask.PlayAnimation(new AnimationSet("amb@bridgecops"), "open_boot", 2.0f);
                return OpenHood;
            }
        }

        public static TaskSequence CloseHood
        {
            get
            {
                TaskSequence CloseHood = new TaskSequence();
                CloseHood.AddTask.PlayAnimation(new AnimationSet("amb@bridgecops"), "close_boot", 4.0f);
                return CloseHood;
            }
        }

        public static TaskSequence NikoFixCar
        {
            get
            {
                TaskSequence FixCar = new TaskSequence();
                FixCar.AddTask.PlayAnimation(new AnimationSet("missbrucie2"), "mechanic_look_at_car", 4.0f);
                FixCar.AddTask.PlayAnimation(new AnimationSet("misstaxidepot"), "workunderbonnet", 4.0f);

                return FixCar;
            }
        }

        public static TaskSequence NikoFixCar2
        {
            get
            {
                TaskSequence FixCar = new TaskSequence();
                FixCar.AddTask.PlayAnimation(new AnimationSet("misstaxidepot"), "workunderbonnet", 2.0f);

                return FixCar;
            }
        }
        

        public static TaskSequence DamnIt
        {
            get
            {
                var rage = GetRandomRage();
                TaskSequence DamnIt = new TaskSequence();
                DamnIt.AddTask.PlayAnimation(rage.set, rage.name, 1f);
                return DamnIt;
            }
        }

        public static TaskSequence NikoFixBike
        {
            get
            {
                var fixAnim = GetRandomBikeFix();
                TaskSequence FixBike = new TaskSequence();
                FixBike.AddTask.PlayAnimation(fixAnim.set, fixAnim.name, 8f);
                FixBike.AddTask.PlayAnimation(new AnimationSet("gestures@niko"), "agree", 8f);
                return FixBike;
            }
        }

        private static Anim GetRandomRage()
        {
            var rand = new Random();
            int which = rand.Next(1, 4);

            switch (which)
            {
                case 1:
                    return new Anim { set = new AnimationSet("amb@beg_standing"), name = "crazy_rant_01" };
                case 2:
                    return new Anim { set = new AnimationSet("amb@bum_a"), name = "stand_rant_a" };
                case 3:
                    return new Anim { set = new AnimationSet("amb@bum_a"), name = "stand_rant_b" };
                case 4:
                    return new Anim { set = new AnimationSet("gestures@niko"), name = "unbelievable" };
                case 5:
                    return new Anim { set = new AnimationSet("gestures@niko"), name = "u_serious" };
                default:
                    return new Anim { set = new AnimationSet("amb@beg_standing"), name = "crazy_rant_01" };
            }
        }

        private static Anim GetRandomBikeFix()
        {
            var rand = new Random();
            int which = rand.Next(1, 4);

            switch (which)
            {
                case 1:
                    return new Anim { set = new AnimationSet("amb@broken_d_idles_a"), name = "idle_a" };
                case 2:
                    return new Anim { set = new AnimationSet("amb@broken_d_idles_a"), name = "idle_b" };
                case 3:
                    return new Anim { set = new AnimationSet("amb@broken_d_idles_b"), name = "idle_c" };
                default:
                    return new Anim { set = new AnimationSet("amb@beg_standing"), name = "crazy_rant_01" };
            }
        }

        public static string GetRandomCarRageSpeech()
        {
            var rand = new Random();
            int which = rand.Next(1, 3);

            switch (which)
            {
                case 1:
                    return "START_CAR_PANIC";
                case 2:
                    return "GENERIC_CURSE";
                default:
                    return "START_CAR_PANIC";
            }
        }
    }
}
