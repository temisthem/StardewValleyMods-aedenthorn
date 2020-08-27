﻿using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Network;
using System;
using System.Collections.Generic;

namespace ZombieVirus
{
    internal class ZombiePatches
    {
        private static IModHelper helper;
        private static IMonitor monitor;
        private static ModConfig config;

        public static void Initialize(IModHelper _helper, IMonitor _monitor, ModConfig _config)
        {
            helper = _helper;
            monitor = _monitor;
            config = _config;
        }
        public static void NPC_draw_prefix(NPC __instance)
        {
            if (ModEntry.zombieTextures.ContainsKey(__instance.name) && ModEntry.zombieTextures[__instance.name] != null)
            {
                helper.Reflection.GetField<Texture2D>(__instance.sprite.Value, "spriteTexture").SetValue(ModEntry.zombieTextures[__instance.Name]);
            }
        }
        public static void Farmer_draw_prefix(Farmer __instance)
        {
            if (ModEntry.playerZombies.ContainsKey(__instance.UniqueMultiplayerID) && ModEntry.playerZombies[__instance.uniqueMultiplayerID] != null)
            {
                helper.Reflection.GetField<Texture2D>(__instance.sprite.Value, "spriteTexture").SetValue(ModEntry.playerZombies[__instance.uniqueMultiplayerID]);
            }
        }

        public static void DialogueBox_drawPortrait_prefix(DialogueBox __instance, Dialogue ___characterDialogue)
        {
            if (ModEntry.zombieTextures.ContainsKey(___characterDialogue.speaker.Name))
            {
                ___characterDialogue.speaker.Portrait = ModEntry.zombiePortraits[___characterDialogue.speaker.Name];
            }
        }
        public static void DialogueBox_complex_prefix(ref List<Response> responses)
        {
            if (ModEntry.playerZombies.ContainsKey(Game1.player.uniqueMultiplayerID))
            {
                for(int i = 0; i < responses.Count; i++)
                {
                    //Utils.MakeZombieSpeak(ref responses[i].responseText);
                }
            }
        }

        public static void Dialogue_prefix(Dialogue __instance, ref string masterString)
        {
            if (ModEntry.zombieTextures.ContainsKey(__instance.speaker.Name))
            {
                Utils.MakeZombieSpeak(ref masterString);
            }
        }

        public static void setUpShopOwner_postfix(ShopMenu __instance, string who)
        {
            if(who != null && ModEntry.zombieTextures.ContainsKey(who))
            {
                Utils.MakeZombieSpeak(ref __instance.potraitPersonDialogue);
            }
        }

        internal static bool NPC_tryToReceiveActiveObject_prefix(NPC __instance, Farmer who)
        {
            if (who.ActiveObject.Name == "Zombie Cure" && ModEntry.zombieTextures.ContainsKey(__instance.Name))
            {
                who.currentLocation.playSound("slimedead", NetAudio.SoundContext.Default);
                Utils.RemoveZombie(__instance.Name);
                __instance.CurrentDialogue.Clear();
                __instance.CurrentDialogue.Push(new Dialogue(helper.Translation.Get("cured"), __instance));
                return false;
            }
            return true;
        }
    }
}