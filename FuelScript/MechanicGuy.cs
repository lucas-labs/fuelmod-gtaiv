using AdvancedHookManaged;
using FuelScript.utils;
using GTA;
using NativeUI;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace FuelScript
{
    class MechanicGuy : Script
    {
        #region Other
        private readonly Image CoverGif;
        #endregion

        #region NativeUI
        private readonly UIMenu uiMenu1; // Main menu

        // Items
        private readonly UI.UIMenuItem callMikeButton;
        private readonly float MikesBaseRate;
        private readonly float MikesRateDamageMultiplier;
        private float CurrentHealth = -1;
        private float CurrentEngineHeaalth = -1;
        private readonly Logger Log = new Logger(typeof(MechanicGuy).Name);
        #endregion

        public MechanicGuy()
        {
            SettingsFile.Open("FuelScript.ini");
            Settings.Load();
            Log.Info("Settings FuelScript.ini Opened");

            MikesBaseRate = Settings.GetValueFloat("BASE_RATE", "MIKES", 30f);
            MikesRateDamageMultiplier = Settings.GetValueFloat("DAMAGE_MULTIPLIER", "MIKES", 0.1f);

            Log.Info("Mikes Base Rate: $" + MikesBaseRate);
            Log.Info("Damage Rate Multiplier: " + MikesRateDamageMultiplier);

            CoverGif = Properties.Resources.MechCover;

            // Set settings for all menus
            UIMenu.Options.UpKey = Keys.Up;
            UIMenu.Options.DownKey = Keys.Down;
            UIMenu.Options.LeftKey = Keys.Left;
            UIMenu.Options.RightKey = Keys.Right;
            UIMenu.Options.AcceptKey = Keys.Enter;
            UIMenu.Options.disablePhoneWhenMenuIsOpened = true;
            UIMenu.Options.enableControllerSupport = true;
            UIMenu.Options.enableMenuSounds = true;
            UIMenu.Options.AnimatedBannerFrameRate = 120;

            #region Menu1
            // Create a new menu
            //uiMenu2 = new UIMenu("NestedMenu", "Only a test menu.");
            // Create default items
            // If item gets pressed.
            //uiMenuItem2 = new UI.UIMenuItem("TestItem2", "I'm a disabled button", "You can't click on me... :(", false);
            //uiMenuItem2.OnClick += UiMenuItem2_OnClick; // If item gets pressed.
            //uiMenuItem3 = new UI.UIMenuItem("TestItem3", "Showcase button", "This is a long text. But wait, it can get even longer! It doesn't stop getting longer! Send help please! Quick! ...", true);

            // Create default item with an icon
            //uiMenuItemWithImage4 = new UI.UIMenuItem("TestItem4", "More information...", "This is a default menu item just with an icon!", true);
            //uiMenuItemWithImage4.DefaultIcon = new Texture(Properties.Resources.InfoSymbolDefault); // The icon that shows if the item is not selected.
            //uiMenuItemWithImage4.SelectedIcon = new Texture(Properties.Resources.InfoSymbolSelected); // The icon that shows if the item is selected.
            //uiMenuItemWithImage4.DisabledIcon = new Texture(Properties.Resources.InfoSymbolDisabled); // The icon that shows if the item is disabled.
            //uiMenuItemWithImage4.IconLocation = UI.UIMenuItem.iconLocations.Right; // Location of your item (Left is default)
            //uiMenuItemWithImage4.IconSize = new SizeF(32f, 32f); // The size of your icon. This is very important! (Maximum recommended size for your icon(s): 32x32)
            // uiMenuItemWithImage4.IconOffset(0f, 0f); // If your icon size is not 32x32 then you might have to adjust the offset a bit.
            //uiMenuItemWithImage4.DrawIcon = true; // Allows the item to draw your icon

            // Create default item with a nested menu.
            //uiMenuItem5 = new UI.UIMenuItem("TestItem5", "Show another menu...", "This shows the nested menu set for this item.", true);
            //uiMenuItem5.NestedMenu = uiMenu2;

            // Create checkbox items
            //uIMenuCheckboxItem1 = new UI.UIMenuCheckboxItem("TestCheckbox1", "I'm a checked checkbox", "You can also uncheck me if you want.", true, true);
            //uIMenuCheckboxItem1.OnCheckedChanged += UIMenuCheckboxItem1_OnCheckedChanged; // If checkbox check state changes.
            //uIMenuCheckboxItem2 = new UI.UIMenuCheckboxItem("TestCheckbox2", "I'm a unchecked checkbox", "You can also check me if you want.", true, false);
            //uIMenuCheckboxItem3 = new UI.UIMenuCheckboxItem("TestCheckbox3", "I'm a checked disabled checkbox", "You can't uncheck me!", false, true);
            //uIMenuCheckboxItem4 = new UI.UIMenuCheckboxItem("TestCheckbox3", "I'm a unchecked disabled checkbox", "You can't check me!", false, false);

            // Create list item
            //uiListItem1 = new UI.UIMenuListItem("TestListItem1", "I'm a item with an list!", "Hit left, right to navigate through the list. Hit enter to show get the current selected item.", true);
            //uiListItem1.OnSelectedIndexChanged += UiListItem1_OnSelectedIndexChanged;
            //uiListItem1.OnClick += UiListItem1_OnClick;
            // Add items to list
            //uiListItem1.AddItem("-");
            //uiListItem1.AddItem("Item1");
            //uiListItem1.AddItem("Item2");
            //uiListItem1.AddItem("And this is item 3");

            // Add items to the menu

            //uiMenu1.AddItem(uiMenuItem2);
            //uiMenu1.AddItem(uIMenuCheckboxItem1);
            //uiMenu1.AddItem(uIMenuCheckboxItem2);
            //uiMenu1.AddItem(uIMenuCheckboxItem3);
            //uiMenu1.AddItem(uIMenuCheckboxItem4);
            //uiMenu1.AddItem(uiMenuItem3);
            //uiMenu1.AddItem(uiListItem1);
            //uiMenu1.AddItem(uiMenuItemWithImage4);
            //uiMenu1.AddItem(uiMenuItem5);
            #endregion

            uiMenu1 = new UIMenu("Smallcar Mike", "Mechanical Services", CoverGif)
            {
                MaxItemsVisibleAtOnce = 4 // Only 4 items will be displayed at once in the menu.
            };

            callMikeButton = new UI.UIMenuItem("Call Mike", "Click here to call mike", "Call me baby!", true);
            callMikeButton.OnClick += CallMike_OnClick;
            uiMenu1.AddItem(callMikeButton);

            // Scripts stuff...
            this.Interval = 500;
            this.Tick += MechanickGuy_Tick;
            this.PerFrameDrawing += MechanicGuy_PerFrameDrawing;
            this.KeyDown += MechanicGuy_KeyDown;
        }

        private void CallMike_OnClick(UI.BaseElement sender)
        {
            if (UIMenu.IsAnyMenuOpen())
            {
                UIMenu.HideAllMenus();
            }

            if (Player.Character.isInVehicle())
            {
                var veh = Player.Character.CurrentVehicle;
                bool Charged = false;
                float ServiceCost = CalculateServiceCost(veh);

                // check if i have enough money to pay mike
                if (Player.Money <= ServiceCost)
                {
                    DisplayText("Sorry! You don't have enough money to pay for Mike's services ($" + ServiceCost + ")!");
                    return;
                }

                var MikesBike = World.CreateVehicle(new Model("bobber"), World.GetNextPositionOnStreet(Player.Character.Position.Around(100.0f)));
                while (!MikesBike.Exists())
                {
                    Wait(500);
                }
                MikesBike.MakeProofTo(true, true, true, true, true);
                MikesBike.PlaceOnNextStreetProperly();
                var Mike = MikesBike.CreatePedOnSeat(VehicleSeat.Driver, new Model("m_y_mechanic_02"));
                Mike.Voice = "M_M_PITALIAN_02";
                Blip ServiceBlip = Mike.AttachBlip();

                try
                {
                    // lets trap niko inside the vehicle
                    veh.DoorLock = DoorLock.ImpossibleToOpen;
                    veh.EngineRunning = false;

                    // mike's Service Drive To position.
                    Vector3 DriveToPosition = veh.GetOffsetPosition(new Vector3(5.0f, 0.0f, 0.0f));
                    // Hood position.
                    Vector3 HoodPosition = veh.GetOffsetPosition(new Vector3(0.0f, 3.0f, 0.0f));
                    

                    while (!Mike.Exists())
                    {
                        Wait(500);
                    }

                    ServiceBlip.Icon = BlipIcon.Building_Garage;
                    ServiceBlip.Color = BlipColor.Orange;
                    ServiceBlip.ShowOnlyWhenNear = false;
                    ServiceBlip.Name = "Mike";
                    Mike.Invincible = true;

                    // Block his permenent events.
                    Mike.BlockPermanentEvents = true;
                    Mike.ChangeRelationship(RelationshipGroup.Player, Relationship.Respect);
                    Mike.Task.ClearAll();
                    Mike.Task.AlwaysKeepTask = true;
                    Mike.Task.Wait(-1);
                    Game.LoadAllPathNodes = true;
                    if (!Mike.isInVehicle())
                    {
                        Mike.Task.EnterVehicle(MikesBike, VehicleSeat.Driver);
                    }
                    while (!Mike.isInVehicle())
                    {
                        Wait(500);
                    }

                    var mikeAdv = TypeConverter.ConvertToAPed(Mike);

                    // Drive to Niko
                    TaskSequence DriveToNiko = new TaskSequence();
                    DriveToNiko.AddTask.DriveTo(DriveToPosition, 15.0f, false, true);
                    DriveToNiko.AddTask.LeaveVehicle();
                    Mike.Task.PerformSequence(DriveToNiko);
                    ShowMessage("Mike ya esta de camino. Esperalo aquí.", 3000);
                    WaitForTask(DriveToNiko, mikeAdv, Mike);

                    Mike.SayAmbientSpeech("GENERIC_HI");

                    // Run to car and open hood
                    TaskSequence RunToCarAndOpenHood = new TaskSequence();
                    RunToCarAndOpenHood.AddTask.RunTo(HoodPosition, false);
                    RunToCarAndOpenHood.AddTask.TurnTo(veh.Position);
                    RunToCarAndOpenHood.AddTask.PlayAnimation(new AnimationSet("amb@bridgecops"), "open_boot", 4.0f);
                    Mike.Task.PerformSequence(RunToCarAndOpenHood);
                    WaitForTask(RunToCarAndOpenHood, mikeAdv, Mike);
                    veh.Door(VehicleDoor.Hood).Open();
                    Wait(1200);

                    // Fix it
                    TaskSequence FixTheCar = new TaskSequence();
                    FixTheCar.AddTask.PlayAnimation(new AnimationSet("misstaxidepot"), "workunderbonnet", 4.0f);
                    FixTheCar.AddTask.PlayAnimation(new AnimationSet("amb@bridgecops"), "close_boot", 4.0f);
                    Mike.Task.PerformSequence(FixTheCar);
                    WaitForTask(FixTheCar, mikeAdv, Mike);
                    veh.Door(VehicleDoor.Hood).Close();
                    veh.Repair();

                    Wait(1000);
                    Player.Money -= Convert.ToInt32(ServiceCost);

                    ShowMessage("Todo listo. El costo fue de $" + ServiceCost, 3000);

                    Charged = true;
                    GTA.Native.Function.Call("DISPLAY_CASH", true);
                    Mike.SayAmbientSpeech("THANKS");
                    Wait(1000);
                    Mike.SayAmbientSpeech("GENERIC_BYE");

                    veh.DoorLock = DoorLock.None;
                    Mike.BlockPermanentEvents = true;
                    // Clear previous tasks.
                    Mike.Task.ClearAll();
                    Mike.Task.AlwaysKeepTask = true;

                    // Get Out
                    TaskSequence GetOut = new TaskSequence();
                    GetOut.AddTask.EnterVehicle(MikesBike, VehicleSeat.Driver);
                    GetOut.AddTask.CruiseWithVehicle(MikesBike, 35.0f, true);
                    ServiceBlip.Delete();
                    MikesBike.MakeProofTo(false, false, false, false, false);
                    Mike.NoLongerNeeded();
                    MikesBike.NoLongerNeeded();
                    Mike.Task.PerformSequence(GetOut);
                }
                catch (Exception ex)
                {
                    try { 
                        veh.EngineRunning = true;
                        veh.DoorLock = DoorLock.None;
                        veh.Repair();
                        if(!Charged)
                        {
                            Player.Money -= Convert.ToInt32(ServiceCost);
                        }
                        ServiceBlip.Delete();
                        MikesBike.MakeProofTo(false, false, false, false, false);
                        Mike.NoLongerNeeded();
                        MikesBike.NoLongerNeeded();
                    } catch { }
                    
                    Log.Error(ex.Message);
                }
                
                
            }
        }

        private void WaitForTask(TaskSequence task, APed advPed, Ped ped)
        {
            var aTaskActive = advPed.IsTaskActive(task.Handle);
            var count = 0;
            while(!aTaskActive)
            {
                Wait(100);
                aTaskActive = advPed.IsTaskActive(task.Handle);
                count++;

                if(count >= 1200 ) // espero 2 minutos
                {
                    throw new Exception("2 minutos sin finalizar la tarea");
                }
            }

            count = 0;
            while(aTaskActive)
            {
                Wait(100);
                aTaskActive = advPed.IsTaskActive(task.Handle);
                if (count >= 1200) // espero 2 minutos
                {
                    throw new Exception("2 minutos sin finalizar la tarea");
                }
            }
            advPed.AbortTask(task.Handle, 1);
            ped.Task.ClearAllImmediately();
            ped.Task.ClearAll();
        }

        private void DisplayText(string message)
        {
            AGame.PrintText(message);
        }

        private float CalculateServiceCost(Vehicle vehicle)
        {
            var InverseDamage = 1000 - vehicle.Health.Clamp(0, 1000);
            return (float) Math.Floor(MikesBaseRate + (InverseDamage * MikesRateDamageMultiplier));
        }

        private void MechanickGuy_Tick(object sender, EventArgs e)
        {
            UIMenu.ProcessController();

            if (!Player.Character.isInVehicle() && UIMenu.IsAnyMenuOpen())
            {
                UIMenu.HideAllMenus();
            }


            if(Player.Character.isInVehicle())
            {
                var vehicle = Player.Character.CurrentVehicle;
                if(vehicle != null)
                {
                    CurrentHealth = vehicle.Health;
                    CurrentEngineHeaalth = vehicle.EngineHealth;
                    if(CurrentHealth >= 0 && CurrentEngineHeaalth >= 0)
                    {
                        callMikeButton.Description = "GH: " + Convert.ToInt32(CurrentHealth) + " | EH: " + Convert.ToInt32(CurrentEngineHeaalth);
                    }
                }
            }
        }
        private void MechanicGuy_PerFrameDrawing(object sender, GraphicsEventArgs e)
        {
            UIMenu.ProcessDrawing(e); 
        }
        private void MechanicGuy_KeyDown(object sender, GTA.KeyEventArgs e)
        {
            UIMenu.ProcessKeyPress(e);

            if (e.Key == Keys.M)
            {
                if (Player.Character.isInVehicle())
                {
                    if (UIMenu.IsAnyMenuOpen())
                    {
                        UIMenu.HideAllMenus();
                    }
                    else
                    {
                        UIMenu.Show(uiMenu1);
                    }
                }
            }
            
        }

        internal void ShowMessage(string message, int time)
        {
            Log.Debug(message);
            GTA.Native.Function.Call("PRINT_STRING_WITH_LITERAL_STRING_NOW", "STRING", message, time, true);
        }

        internal bool IsFreeForAmbientTask(Ped ped)
        {
            var areThey = GTA.Native.Function.Call<object>("IS_PLAYER_FREE_FOR_AMBIENT_TASK", ped) != null;
            Log.Debug("Are they? " + !areThey);

            return Convert.ToBoolean(!areThey);
        }
    }
}
