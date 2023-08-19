using HarmonyLib;
using TaleWorlds.CampaignSystem.CharacterCreationContent;

namespace Thirst.Patches
{
    [HarmonyPatch(typeof(CharacterCreationState), "FinalizeCharacterCreation")]
    internal class FinalizeCharacterCreationPatch
    {
        public static void Prefix()
        {
            SubModule.thirst.InitializePlayer();
        }
    }
}
