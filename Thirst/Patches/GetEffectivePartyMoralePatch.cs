using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Localization;
using Thirst.Models;

namespace Thirst.Patches
{
    //[HarmonyPatch(typeof(DefaultPartyMoraleModel), "GetEffectivePartyMorale")]
    internal class GetEffectivePartyMoralePatchDefault
    {
        public static void Postfix(ExplainedNumber __result, MobileParty mobileParty)
        {
            if (SubModule.thirst.partyThirst.ContainsKey(mobileParty) && mobileParty != MobileParty.MainParty)
            {
                PartyWaterConsumptionModel partyModel = SubModule.thirst.partyThirst[mobileParty];
                if (partyModel != null && partyModel.IsDehydrated)
                {
                    __result.Add((float)-30, new TextObject("No Water"));
                }
            }
        }
    }
}
