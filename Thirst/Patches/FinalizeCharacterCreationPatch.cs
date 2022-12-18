using HarmonyLib;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.CampaignSystem.Party;
using Thirst.UI;

namespace Thirst.Patches
{
    [HarmonyPatch(typeof(CharacterCreationState), "FinalizeCharacterCreation")]
    internal class FinalizeCharacterCreationPatch
    {
        public static void Prefix()
        {
            SubModule.thirst.InitializePlayer();
            //ThirstMapBarMixin.partyModel = SubModule.thirst.partyThirst[MobileParty.MainParty];
        }
    }
}
