using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using xTile.Dimensions;


namespace automatespinningwheel
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        private int _cooldown = 0;
        private const int MAX_STAR = 9999;
        //private ModConfig Config;
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.UpdateTicked += GameLoop_SpinningWheel;
            helper.Events.Display.RenderedActiveMenu += MenuHandler;

        }
        private void MenuHandler(object? sender, RenderedActiveMenuEventArgs e)
        {
            if (!Context.IsWorldReady) return;
            if (Game1.currentLocation.lastQuestionKey != "wheelBet") return;
            if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is DialogueBox dialogueBox)
            {
                this.Monitor.Log($"then: {Game1.activeClickableMenu.GetType()}", LogLevel.Debug);
                dialogueBox.safetyTimer = 0;
                dialogueBox.selectedResponse = 1;
                dialogueBox.receiveLeftClick(0, 0, true);
                //this.Monitor.Log($"now: {.GetType()}", LogLevel.Debug);
                if (Game1.activeClickableMenu is NumberSelectionMenu tokenInputMenu)
                {
                    //this.Monitor.Log($"is startoken number selection", LogLevel.Debug);
                    SetValue(tokenInputMenu);
                }
            }
        }
        private void SetValue(NumberSelectionMenu nsm)
        {
            int starToken = Game1.player.festivalScore;

            // bet 45% of the star token
            int bet = starToken == 1 ? 1 : 0;
            bet = (int)MathF.Ceiling((float)starToken * 0.45f);

            Helper.Reflection.GetField<int>(nsm, "currentValue").SetValue(bet);
            TextBox tb = Helper.Reflection.GetField<TextBox>(nsm, "numberSelectedBox").GetValue();
            tb.Text = bet.ToString();

            // click at the ok button coordinates
            ClickableTextureComponent okButton = Helper.Reflection.GetField<ClickableTextureComponent>(nsm, "okButton").GetValue();
            if (okButton != null)
            {
                nsm.receiveLeftClick(okButton.bounds.Center.X, okButton.bounds.Center.Y, true);
                this.Monitor.Log("not null ", LogLevel.Debug);
            }
        }
        private void GameLoop_SpinningWheel(object? sender, UpdateTickedEventArgs e)
        {
            // make sure u are in stardew valley fair
            if (Game1.CurrentEvent?.isSpecificFestival("fall16") is not true)
            {
                return;
            }
            var tile = new Vector2(33, 70);
            if (_cooldown > 0)
            {
                _cooldown--;
                return;
            }

            // make sure to be near at the spinning wheel
            float distance = Vector2.Distance(Game1.player.Position, tile);
            if (distance > 4900 && distance < 5000)
            {
                int starTokens = Game1.player.festivalScore;
                if (starTokens >= MAX_STAR || starTokens == 0)
                {
                    //this.Monitor.Log($"startoken invalid, not running the dialogue", LogLevel.Debug);
                    return;
                }
                else
                {
                    Game1.currentLocation?.checkAction(new Location((int)tile.X, (int)tile.Y), Game1.viewport, Game1.player);
                }

                _cooldown = 1000;
            }
        }


    }
}
