using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Localization;
using Thirst.Managers;

namespace Thirst.Patches
{
    //[HarmonyPatch(typeof(DefaultPartySpeedCalculatingModel), "CalculateFinalSpeed")]
    internal class CalculateFinalSpeedPatch
    {
        public static void SpeedFinalizer(ExplainedNumber __result, MobileParty mobileParty)
        {
            if (mobileParty != null && ThirstManager.partyThirst.ContainsKey(mobileParty) && ThirstManager.partyThirst[mobileParty].IsDehydrated)
            {
                __result.AddFactor(-0.5f, new TextObject("Dehydrated"));
            }
        }
    }
}
