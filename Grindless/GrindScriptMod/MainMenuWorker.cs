using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using SoG;
using System.Linq;

namespace Grindless
{
    /// <summary>
    /// Handles callbacks for Main Menu patches.
    /// This includes save mod compatibility information, and the mod menu.
    /// </summary>
    internal static class MainMenuWorker
    {
        private class ModMenu
        {
            private int _selection = 0;
            public int Selection
            {
                get => _selection;
                set => _selection = Math.Min(Math.Max(value, MinSelection), MaxSelection);
            }

            public readonly int MinSelection = 0;

            public readonly int MaxSelection = 1;
        }

        public static readonly GlobalData.MainMenu.MenuLevel ReservedModMenuID = (GlobalData.MainMenu.MenuLevel)300;

        private static Dictionary<int, SaveCompatibility> _storySaves = new();

        private static SaveCompatibility _arcadeSave;

        private static ModMenu _modMenu = new();

        private static int _previousTopMenuSelection;

        public static void UpdateStorySaveCompatibility()
        {
            for (int index = 0; index < Globals.Game.xGlobalData.lxCharacterSaves.Count; index++)
            {
                var save = Globals.Game.xGlobalData.lxCharacterSaves[index];

                if (save.bIncompatibleSave || save.sCharacterName == "")
                {
                    continue;
                }

                string path = $"{Globals.Game.sAppData}Characters/{index}.cha{ModSaving.SaveFileExtension}";

                if (File.Exists(path))
                {
                    using (FileStream stream = new(path, FileMode.Open, FileAccess.Read))
                    {
                        _storySaves[index] = ModSaving.CheckCompatibility(stream);
                    }
                }
            }
        }

        public static void UpdateArcadeSaveCompatibility()
        {
            if (RogueLikeMode.LockedOutDueToHigherVersionSaveFile)
            {
                _arcadeSave = null;
                return;
            }

            string appData = Globals.Game.sAppData;

            string path = appData + $"arcademode.sav{ModSaving.SaveFileExtension}";

            if (File.Exists(path))
            {
                using (FileStream stream = new(path, FileMode.Open, FileAccess.Read))
                {
                    _arcadeSave = ModSaving.CheckCompatibility(stream);
                }
            }
        }

        public static void CheckStorySaveCompatibility()
        {
            var menuData = Globals.Game.xGlobalData.xMainMenuData;

            int slot = menuData.iSelectedChar + menuData.iCurrentCharSelectPage * 3;
            //float alpha = menuData.fCurrentMenuAlpha;

            if (slot < 0 || slot > 8)
                return;

            if (!_storySaves.ContainsKey(slot))
            {
                RenderMessage("No data available (vanilla save?).", 444, 90 + 65);
            }
            else
            {
                RenderMessage(_storySaves[slot].GetCompatibilityText(), 444, 90 + 65);
            }
        }

        public static void CheckArcadeSaveCompatiblity()
        {
            if (Globals.Game.xGlobalData.xMainMenuData.iTopMenuSelection != 1)
                return;

            if (_arcadeSave == null)
            {
                RenderMessage("No data available (vanilla save?).", 422, 243);
            }
            else
            {
                RenderMessage(_arcadeSave.GetCompatibilityText(), 422, 243);
            }
        }

        public static void RenderMessage(string message, int x, int y)
        {
            float alpha = Globals.Game.xGlobalData.xMainMenuData.fCurrentMenuAlpha;

            Vector2 measure = FontManager.GetFont(FontManager.FontType.Reg7).MeasureString(message);

            Globals.Game._Menu_RenderNotice(Globals.SpriteBatch, 1f, new Rectangle(x - 4, y - (int)measure.Y / 2 - 4, (int)measure.X + 8, (int)measure.Y + 8), false);

            Globals.SpriteBatch.DrawString(FontManager.GetFont(FontManager.FontType.Reg7), message, new Vector2(x, y - (int)measure.Y / 2), Color.White * alpha);
        }


        public static void PostTopMenuInterface()
        {
            var menuData = Globals.Game.xGlobalData.xMainMenuData;
            var inputData = Globals.Game.xInput_Menu;
            var audio = Globals.Game.xSoundSystem;

            if (inputData.Left.bPressed && menuData.iTopMenuSelection != 4)
            {
                _previousTopMenuSelection = menuData.iTopMenuSelection;
                menuData.iTopMenuSelection = 4;
                audio.PlayInterfaceCue("Menu_Move");
            }
            else if (inputData.Right.bPressed && menuData.iTopMenuSelection == 4)
            {
                menuData.iTopMenuSelection = _previousTopMenuSelection;
                audio.PlayInterfaceCue("Menu_Move");
            }

            if (inputData.Action.bPressed && menuData.iTopMenuSelection == 4)
            {
                audio.PlayInterfaceCue("Menu_Changed");

                // TODO: Lol
                Globals.Game.xGlobalData.xMainMenuData.Transition(ReservedModMenuID);
            }
        }

        public static void RenderReloadModsButton()
        {
            var menuData = Globals.Game.xGlobalData.xMainMenuData;

            Color selected = Color.White;
            Color notSelected = Color.Gray * 0.8f;

            var text = "Reload Mods";
            var font = FontManager.GetFont(FontManager.FontType.Verdana12);
            Vector2 center = font.MeasureString(text) / 2f;

            Color colorToUse = menuData.iTopMenuSelection == 4 ? selected : notSelected;

            Globals.Game._RenderMaster_RenderTextWithOutline(font, text, new Vector2(160, 268) - center, Vector2.Zero, 1f, colorToUse, Color.Black);
        }

        public static void RenderModMenuButton()
        {
            var menuData = Globals.Game.xGlobalData.xMainMenuData;
            var spriteBatch = Globals.SpriteBatch;

            Color selected = Color.White;
            Color notSelected = Color.Gray * 0.8f;
            float alpha = menuData.fCurrentMenuAlpha;

            var texture = GrindlessResources.ModMenu;
            Vector2 center = new(texture.Width / 2, texture.Height / 2);

            Color colorToUse = menuData.iTopMenuSelection == 4 ? selected : notSelected;

            spriteBatch.Draw(texture, new Vector2(160 - center.X, 245 - center.Y), null, colorToUse, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            //var font = FontManager.GetFont(FontManager.FontType.Bold10);
            //center = font.MeasureString("(Unimplemented)") / 2f;
            //Globals.Game._RenderMaster_RenderTextWithOutline(font, "(Unimplemented)", new Vector2(160, 268) - center, Vector2.Zero, 1f, colorToUse, Color.Black);
        }

        public static void MenuUpdate()
        {
            if (Globals.Game.xGlobalData.xMainMenuData.enTargetMenuLevel != GlobalData.MainMenu.MenuLevel.Null)
            {
                return;
            }

            if (Globals.Game.xGlobalData.xMainMenuData.enMenuLevel == ReservedModMenuID)
            {
                ModMenuInterface();
            }
        }

        private static void ModMenuInterface()
        {
            var input = Globals.Game.xInput_Menu;

            var previousSelection = _modMenu.Selection;

            if (input.Down.bPressed)
            {
                _modMenu.Selection++;
            }
            else if (input.Up.bPressed)
            {
                _modMenu.Selection--;
            }

            if (previousSelection != _modMenu.Selection)
            {
                Globals.Game.xSoundSystem.PlayInterfaceCue("Menu_Move");
            }

            if (input.Action.bPressed)
            {
                Globals.Game.xSoundSystem.PlayInterfaceCue("Menu_Change");

                switch (_modMenu.Selection)
                {
                    case 0:
                        ModManager.Reload();
                        break;
                    case 1:
                        break;
                }

            }
            else if (input.MenuBack.bPressed)
            {
                Globals.Game.xSoundSystem.PlayInterfaceCue("Menu_Cancel");

                Globals.Game.xGlobalData.xMainMenuData.Transition(GlobalData.MainMenu.MenuLevel.TopMenu);
            }
        }

        public static void ModMenuRender()
        {
            float alpha = Globals.Game.xGlobalData.xMainMenuData.fCurrentMenuAlpha;
            Color selected = Color.White;
            Color notSelected = Color.Gray * 0.8f;

            SpriteBatch spriteBatch = Globals.SpriteBatch;

            Globals.Game._Menu_RenderContentBox(spriteBatch, alpha, new Rectangle(235, 189, 173, 138));

            Texture2D reloadModsTex = GrindlessResources.ReloadMods;
            Vector2 reloadModsCenter = new(reloadModsTex.Width / 2, reloadModsTex.Height / 2);
            Color reloadModsColor = _modMenu.Selection == 0 ? selected : notSelected;

            spriteBatch.Draw(reloadModsTex, new Vector2(320, 225), null, reloadModsColor, 0f, reloadModsCenter, 1f, SpriteEffects.None, 0f);

            Texture2D modListTex = GrindlessResources.ModList;
            Vector2 modListCenter = new(modListTex.Width / 2, modListTex.Height / 2);
            Color modListColor = _modMenu.Selection == 1 ? selected : notSelected;
            modListColor *= 0.6f;

            spriteBatch.Draw(modListTex, new Vector2(320, 251), null, modListColor, 0f, modListCenter, 1f, SpriteEffects.None, 0f);

            string message = "Mods loaded:\n";

            foreach (Mod mod in ModManager.Mods.Where(x => x.Name != VanillaMod.ModName))
            {
                message += mod.Name + " v." + (mod.Version?.ToString() ?? "Unknown") + "\n";
            }

            RenderMessage(message.TrimEnd('\n'), 422, 243);
        }
    }
}
