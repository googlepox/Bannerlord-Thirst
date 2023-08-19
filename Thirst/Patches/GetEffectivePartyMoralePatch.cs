using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Localization;
using Thirst.Managers;
using Thirst.Models;

namespace Thirst.Patches
{
    //[HarmonyPatch(typeof(DefaultPartyMoraleModel), "GetEffectivePartyMorale")]
    internal class GetEffectivePartyMoralePatchDefault
    {
        public static void MoraleFinalizer(ExplainedNumber __result, MobileParty mobileParty)
        {
            if (ThirstManager.partyThirst.ContainsKey(mobileParty))
            {
                PartyWaterConsumptionModel partyModel = ThirstManager.partyThirst[mobileParty];
                if (partyModel != null && partyModel.IsDehydrated)
                {
                    __result.Add(-30, new TextObject("No Water"));
                }
            }
        }
    }
}
