using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Thirst.Models
{
    public class SettlementWaterConsumptionModel
    {
        public bool IsDehydrated = false;

        public float WaterChange;

        public static int NumberOfMenOnMapToDrinkOneWater => 15;
        public float RemainingWaterPercentage;

        public SettlementWaterConsumptionModel()
        {
            IsDehydrated = false;
            WaterChange = 0;
            RemainingWaterPercentage = 100;
        }

        public float GetWaterChange(MobileParty mainParty)
        {
            WaterChange = CalculateDailyBaseWaterConsumptionf(mainParty).ResultNumber;
            return WaterChange;
        }

        public ExplainedNumber WaterChangeExplained(MobileParty mainParty)
        {
            ExplainedNumber baseConsumption = this.CalculateDailyBaseWaterConsumptionf(mainParty, includeDescription: true);
            return this.CalculateDailyWaterConsumptionf(mainParty, baseConsumption);
        }

        public ExplainedNumber CalculateDailyBaseWaterConsumptionf(
          MobileParty party,
          bool includeDescription = false)
        {
            int num = party.Party.NumberOfAllMembers + party.Party.NumberOfPrisoners / 2;
            return new ExplainedNumber((float)-(num < 1 ? 1.0 : (double)num) / (float)NumberOfMenOnMapToDrinkOneWater, includeDescription);
        }

        public ExplainedNumber CalculateDailyWaterConsumptionf(
          MobileParty party,
          ExplainedNumber baseConsumption)
        {
            ;
            return baseConsumption;
        }
        public static string GetDaysUntilNoWater(float totalWater, float waterChange)
        {
            if ((double)totalWater <= 1.4012984643248171E-45)
            {
                return new TextObject("{=koX9okuG}None").ToString();
            }

            return (double)waterChange >= -1.4012984643248171E-45 ? "Never" : MathF.Ceiling(MathF.Abs(totalWater / waterChange)).ToString();
        }

        public int GetWater(Settlement settlement)
        {
            int waterItems = 0;
            for (int index = 0; index < settlement.ItemRoster.Count; ++index)
            {
                ItemRosterElement itemRosterElement = settlement.ItemRoster[index];
                if (!itemRosterElement.IsEmpty)
                {
                    EquipmentElement equipmentElement = itemRosterElement.EquipmentElement;
                    ItemObject itemObject1 = equipmentElement.Item;
                    if (itemObject1 == SubModule.thirst.Water)
                    {
                        waterItems += itemRosterElement.Amount;
                    }
                }
            }
            return waterItems;
        }

        public int GetNumDaysForWaterToLast(Settlement settlement)
        {
            int num = GetWater(settlement) * 100;
            return (int)((double)num / (100.0 * -(double)this.WaterChange));
        }

        private void MakeWaterConsumption(MobileParty party, ref int partyRemainingWaterPercentage)
        {
            ItemRoster itemRoster = party.ItemRoster;
            int maxValue = 0;
            for (int index = 0; index < itemRoster.Count; ++index)
            {
                if (itemRoster.GetItemAtIndex(index).ItemCategory.StringId == ("water"))
                {
                    ++maxValue;
                }
            }
            while (maxValue > 0 && partyRemainingWaterPercentage < 0)
            {
                int num1 = MBRandom.RandomInt(maxValue);
                bool flag2 = false;
                int num2 = 0;
                for (int index = itemRoster.Count - 1; index >= 0 && !flag2; --index)
                {
                    if (itemRoster.GetItemAtIndex(index).ItemCategory.StringId == ("water"))
                    {
                        int elementNumber = itemRoster.GetElementNumber(index);
                        if (elementNumber > 0)
                        {
                            ++num2;
                            if (num1 < num2)
                            {
                                itemRoster.AddToCounts(itemRoster.GetItemAtIndex(index), -1);
                                partyRemainingWaterPercentage += 100;
                                if (elementNumber == 1)
                                {
                                    --maxValue;
                                }

                                flag2 = true;
                            }
                        }
                    }
                }
            }
        }

        public void PartyConsumeWater(MobileParty party)
        {
            bool isDehydrated1 = this.IsDehydrated;
            float waterChange = this.GetWaterChange(party);
            double num1 = (double)waterChange < 0.0 ? -(double)waterChange : 0.0;
            float percentage = (float)this.RemainingWaterPercentage;
            int partyRemainingWaterPercentage = (int)(percentage < 0 ? 0 : percentage) - MathF.Round((float)(num1 * 100));
            this.MakeWaterConsumption(party, ref partyRemainingWaterPercentage);
            this.RemainingWaterPercentage = partyRemainingWaterPercentage < 0 ? 0 : partyRemainingWaterPercentage;
            if (this.RemainingWaterPercentage <= 0)
            {
                this.IsDehydrated = true;
            }
            bool isDehydrated2 = this.IsDehydrated;
            CampaignTime campaignTime = CampaignData.CampaignStartTime;
            int toDays1 = (int)campaignTime.ToDays;
            campaignTime = CampaignTime.Now;
            int toDays2 = (int)campaignTime.ToDays;
            if (toDays1 != toDays2)
            {
                if (isDehydrated1 & isDehydrated2)
                {
                    int dehydrationMoralePenalty = Campaign.Current.Models.PartyMoraleModel.GetDailyStarvationMoralePenalty(party.Party);
                    party.RecentEventsMorale += (float)dehydrationMoralePenalty;
                    if (party.IsMainParty)
                    {
                        MBTextManager.SetTextVariable("MORALE_PENALTY", -dehydrationMoralePenalty);
                        MBInformationManager.AddQuickInformation(new TextObject("{=qhL5o55i}Your party is dehydrated. You lose {MORALE_PENALTY} morale."));
                        campaignTime = CampaignTime.Now;
                        if ((int)campaignTime.ToDays % 3 == 0 && party.MemberRoster.TotalManCount > 1)
                        {
                            TraitLevelingHelper.OnPartyStarved();
                        }
                    }
                }
                if (party.MemberRoster.TotalManCount > 1)
                {
                    SkillLevelingManager.OnFoodConsumed(party, isDehydrated2);
                    if (!isDehydrated1 && !isDehydrated2 && party.IsMainParty && (double)party.Morale >= 90.0 && party.MemberRoster.TotalRegulars >= 20)
                    {
                        campaignTime = CampaignTime.Now;
                        if ((int)campaignTime.ToDays % 10 == 0)
                        {
                            TraitLevelingHelper.OnPartyTreatedWell();
                        }
                    }
                }
            }
        }
    }
}
