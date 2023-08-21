using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Localization;
using Thirst.Managers;

namespace Thirst.Patches
{
    //[HarmonyPatch(typeof(DefaultPartyMoraleModel), "GetEffectivePartyMorale")]
    internal class GetEffectivePartyMoralePatchDefault
    {
        public static void MoraleFinalizer(ref ExplainedNumber __result, MobileParty mobileParty)
        {
            if (ThirstManager.mainPartyIsDehydrated)
            {
                __result.Add(-30, new TextObject("No Water"));
            }
        }
    }
}
