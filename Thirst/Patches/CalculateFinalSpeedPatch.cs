using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Localization;

namespace Thirst.Patches
{
    [HarmonyPatch(typeof(DefaultPartySpeedCalculatingModel), "CalculateFinalSpeed")]
    internal class CalculateFinalSpeedPatch
    {
        public static void Postfix(ExplainedNumber __result, MobileParty mobileParty)
        {
            if (mobileParty != null && SubModule.thirst.partyThirst.ContainsKey(mobileParty) && SubModule.thirst.partyThirst[mobileParty].IsDehydrated)
            {
                __result.AddFactor(-0.5f, new TextObject("Dehydrated"));
            }
        }
    }
}
