using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.SaveSystem;

namespace Thirst.Managers
{
    public class ThirstManager
    {
        [SaveableField(1)]
        public static Dictionary<MobileParty, ThirstData> partyThirst;

        [SaveableField(2)]
        public static bool IsWaterGiven;

        [SaveableField(3)]
        public static float mainPartyRemainingWaterPercentage;

        [SaveableField(4)]
        public static bool mainPartyIsDehydrated;

        public static int NumberOfMenOnMapToDrinkOneWater;

        public ThirstManager()
        {
            partyThirst = new Dictionary<MobileParty, ThirstData>();
            IsWaterGiven = false;
            NumberOfMenOnMapToDrinkOneWater = 20;
            mainPartyRemainingWaterPercentage = 0;
            mainPartyIsDehydrated = false;
        }

        public static void AddWaterOnGameStart()
        {
            if (!IsWaterGiven)
            {
                PartyThirstManager.AddWaterToParties();
                SettlementThirstManager.AddWaterToSettlements();
                IsWaterGiven = true;
            }
        }
    }
}
