using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using xTile.Dimensions;
using static StardewValley.Minigames.MineCart;


namespace riggedspinningwheel
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        private int _cooldown = 0;

        //private ModConfig Config;
        public override void Entry(IModHelper helper)
        {
            //helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            //helper.Events.Content.AssetRequested += 
            //this.Config = this.Helper.ReadConfig<ModConfig>();
            helper.Events.GameLoop.UpdateTicked += GameLoop_SpinningWheel;
            helper.Events.GameLoop.DayStarted += ChangeDateToFair;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.Display.RenderedActiveMenu += MenuHandler;
            // festival

        }
        private void MenuHandler(object? sender, RenderedActiveMenuEventArgs e)
        {
            if (!Context.IsWorldReady) return;
            if (Game1.currentLocation.lastQuestionKey != "wheelBet") return;
            if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is DialogueBox dialogueBox)
            {
                dialogueBox.safetyTimer = 0;
                dialogueBox.selectedResponse = 1;
                dialogueBox.receiveLeftClick(0, 0, true);
                //this.Monitor.Log($"HAS MENU {dialogueBox.responses.Length} {dialogueBox.allClickableComponents.Count}", LogLevel.Debug);
                //if (dialogueBox.allClickableComponents != null)
                //{

                //    for (int i = 0; i < dialogueBox.allClickableComponents.Count; i++)
                //    {
                //        this.Monitor.Log($": {dialogueBox.allClickableComponents[i].bounds} ", LogLevel.Debug);
                //    }
                //}

            }
            else
            {
                this.Monitor.Log("NO MENU ", LogLevel.Debug);
            }
        }
        private void GameLoop_SpinningWheel(object? sender, UpdateTickedEventArgs e)
        {
            // click order
            // tile at X: 33 Y: 70
            // green button: X: 468, Y: 1022
            // ok: X: 1391, Y: 663


            // make sure u are in spinning wheel game
            if (Game1.CurrentEvent?.isSpecificFestival("fall16") is not true)
            {
                return;
            }
            //Game1.currentLocation.lastQuestionKey != "wheelBet"
            var tile = new Vector2(33, 70);
            if (_cooldown > 0)
            {
                _cooldown--;
                return;
            }

            float distance = Vector2.Distance(Game1.player.Position, tile);
            //this.Monitor.Log($"distance: {distance}", LogLevel.Debug);
            if (distance > 4900 && distance < 5000)
            {
                Game1.currentLocation?.checkAction(new Location((int)tile.X, (int)tile.Y), Game1.viewport, Game1.player);
                //this.Monitor.Log($"Clicked tile ", LogLevel.Debug);
                //this.Monitor.Log($"{Game1.currentLocation.objects.ToString()}", LogLevel.Debug);
                //return;

                _cooldown = 1000;
            }


            //this.Monitor.Log($"has key", LogLevel.Debug);
            //var obj = Game1.currentLocation.objects[tile];
            //obj.checkForAction(Game1.player);
            


            //this.Monitor.Log($"{tile.X} {tile.Y}", LogLevel.Debug);
            //this.Monitor.Log($"Lastkey: {Game1.currentLocation.lastQuestionKey}", LogLevel.Debug);
            //var pos = Game1.getMousePosition();
            //this.Monitor.Log($"X: {pos.X}, Y: {pos.Y}", LogLevel.Debug);

        }

        //*
        // RENDER FALL
        //*//
        private void ChangeDateToFair(object? sender, DayStartedEventArgs e)
        {
            if (!Context.IsWorldReady) return;


            String season = "fall";
            bool seasonChanged =  season != Game1.currentSeason;
            Game1.dayOfMonth = 16;
            Game1.currentSeason = season;
            
            if (seasonChanged)
                Game1.setGraphicsForSeason();
            SafelySetTime(900);

            // also set npc
        }
        private void SafelySetTime(int time)
        {
            // move time back
            int intervals = Utility.CalculateMinutesBetweenTimes(Game1.timeOfDay, time) / 10;
            if (intervals > 0)
            {
                for (int i = 0; i < intervals; i++)
                {

                    Game1.performTenMinuteClockUpdate();
                }

            }
            else if (intervals < 0)
            {
                for (int i = 0; i > intervals; i--)
                {
                    Game1.timeOfDay = Utility.ModifyTime(Game1.timeOfDay, -20); // offset 20 minutes so game updates to next interval
                    Game1.performTenMinuteClockUpdate();
                }
            }

            // reset ambient light
            // White is the default non-raining color. If it's raining or dark out, UpdateGameClock
            // below will update it automatically.
            Game1.outdoorLight = Color.White;
            Game1.ambientLight = Color.White;

            // run clock update (to correct lighting, etc)
            Game1.gameTimeInterval = 0;
            Game1.UpdateGameClock(Game1.currentGameTime);
        }
        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (e.Button == SButton.F6)
            {
                var mousePos = Game1.getMousePosition();
                this.Monitor.Log($"Mouse position: X={mousePos.X}, Y={mousePos.Y}", LogLevel.Info);

                if (Game1.activeClickableMenu != null)
                {
                    this.Monitor.Log($"Menu type: {Game1.activeClickableMenu.GetType().Name}", LogLevel.Info);

                    if (Game1.activeClickableMenu is DialogueBox dialogueBox)
                    {
                        this.Monitor.Log($"DialogueBox - Current response: {dialogueBox.getCurrentString()}", LogLevel.Info);
                        this.Monitor.Log($"DialogueBox - Number of responses: {dialogueBox.responses.Length}", LogLevel.Info);
                        this.Monitor.Log($"DialogueBox - Clickable componnents: {dialogueBox.allClickableComponents.Count}", LogLevel.Info);
                    }
                }
            }
        }
    }
}
