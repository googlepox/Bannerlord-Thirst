using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace Thirst.Managers
{
    internal class PartyThirstManager
    {
        public static void InitializeParties()
        {
            if (ThirstManager.partyThirst.Count > 0)
            {
                return;
            }

            foreach (MobileParty party in MobileParty.All)
            {
                ThirstManager.partyThirst.Add(party, new ThirstData(0, false));
            }
        }

        public static void InitializePlayer()
        {
            if (!ThirstManager.partyThirst.ContainsKey(MobileParty.MainParty))
            {
                ThirstManager.partyThirst.Add(MobileParty.MainParty, new ThirstData(0, false));
            }
        }
        public static void AddWaterToParties()
        {
            foreach (MobileParty party in MobileParty.All)
            {
                int sections = 0;
                for (int i = 0; i < party.MemberRoster.TotalManCount/20; i++)
                {
                    sections++;
                }
                while (sections > 0)
                {
                    party.ItemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>(x => x.ItemCategory.StringId == "water"), 2);
                }
            }
            MobileParty.MainParty.ItemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>(x => x.ItemCategory.StringId == "water"), 1);
        }

        public static ExplainedNumber CalculateDailyBaseWaterConsumptionf(
          MobileParty party,
          bool includeDescription = false)
        {
            int num = party.Party.NumberOfAllMembers + party.Party.NumberOfPrisoners / 2;
            return new ExplainedNumber((float)-(num < 1 ? 1.0 : num) / ThirstManager.NumberOfMenOnMapToDrinkOneWater, includeDescription);
        }

        public static ExplainedNumber CalculateDailyWaterConsumptionf(
          MobileParty party,
          ExplainedNumber baseConsumption)
        {
            TerrainType faceTerrainType = Campaign.Current.MapSceneWrapper.GetFaceTerrainType(party.CurrentNavigationFace);
            if (faceTerrainType == TerrainType.Desert && party.LeaderHero != null && party.LeaderHero.Culture != null &&
                party.LeaderHero.Culture.StringId != "aserai" &&
                party.LeaderHero.Culture.StringId != "lyrion" &&
                party.LeaderHero.Culture.StringId != "apolssalian")
            {
                baseConsumption.AddFactor(2.0f, new TextObject("in Desert"));
            }
            else if (faceTerrainType == TerrainType.Dune && party.LeaderHero != null && party.LeaderHero.Culture != null &&
                party.LeaderHero.Culture.StringId != "aserai" &&
                party.LeaderHero.Culture.StringId != "lyrion" &&
                party.LeaderHero.Culture.StringId != "apolssalian")
            {
                baseConsumption.AddFactor(1.5f, new TextObject("in Dune"));
            }
            else if (faceTerrainType == TerrainType.River || faceTerrainType == TerrainType.ShallowRiver)
            {
                baseConsumption.AddFactor(-0.5f, new TextObject("in River"));
            }
            else if (faceTerrainType == TerrainType.Snow)
            {
                baseConsumption.AddFactor(-0.25f, new TextObject("in Snow"));
            }
            else if (faceTerrainType == TerrainType.Lake)
            {
                baseConsumption.AddFactor(-0.5f, new TextObject("in Lake"));
            }
            return baseConsumption;
        }

        public static ExplainedNumber WaterChangeExplained(MobileParty mainParty)
        {
            ExplainedNumber baseConsumption = CalculateDailyBaseWaterConsumptionf(mainParty, includeDescription: true);
            return CalculateDailyWaterConsumptionf(mainParty, baseConsumption);
        }

        public static float GetWaterChange(MobileParty party)
        {
            return WaterChangeExplained(party).ResultNumber;
        }

        public static string GetDaysUntilNoWater(int daysForWaterToLast, float waterChange)
        {
            if (daysForWaterToLast <= 1.4012984643248171E-45)
            {
                return new TextObject("{=koX9okuG}None").ToString();
            }

            return (double)waterChange >= -1.4012984643248171E-45 ? "Never" : daysForWaterToLast.ToString();
        }
        public static int GetNumDaysForWaterToLast(MobileParty party)
        {
            float num1;
            if (party == MobileParty.MainParty)
            {
                num1 = GetWater(party) * 100f;
                num1 += ThirstManager.mainPartyRemainingWaterPercentage;
                return (int)(num1 / (100.0 * -GetWaterChange(party)));
            }
            return 0;
        }

        public static float GetWater(MobileParty party)
        {
            float waterItems = 0;
            for (int index = 0; index < party.ItemRoster.Count; index++)
            {
                ItemRosterElement itemRosterElement = party.ItemRoster[index];
                if (!itemRosterElement.IsEmpty)
                {
                    EquipmentElement equipmentElement = itemRosterElement.EquipmentElement;
                    ItemObject itemObject1 = equipmentElement.Item;
                    if (itemObject1.ItemCategory != null && itemObject1.ItemCategory.StringId == "water")
                    {
                        waterItems += (itemRosterElement.Amount);
                    }
                    else if (itemObject1.ItemCategory.StringId == "bf_mead" || itemObject1.ItemCategory.StringId == "mead" || itemObject1.ItemCategory.StringId == "beer" || itemObject1.ItemCategory.StringId == "wine")
                    {
                        waterItems += (itemRosterElement.Amount);
                    }
                }
            }
            return waterItems;
        }

        public static void PartyConsumeWater(MobileParty party)
        {
            if (ThirstManager.partyThirst.ContainsKey(party))
            {
                ThirstData partyThirstData = ThirstManager.partyThirst[party];
                bool isDehydrated = partyThirstData.IsDehydrated;
                float waterChange = GetWaterChange(party);
                double num1 = waterChange < 0.0 ? -waterChange : 0.0;
                float percentage = partyThirstData.RemainingWaterPercentage;
                float partyRemainingWaterPercentageOld = (percentage < 0.0f ? 0.0f : percentage) - MathF.Round((float)(num1 * 100.0f));
                float partyRemainingWaterPercentage = MakeWaterConsumption(party, partyRemainingWaterPercentageOld);
                float newPercentage = partyRemainingWaterPercentage < 0.0f ? 0.0f : partyRemainingWaterPercentage;
                if (newPercentage + GetWater(party) <= 0)
                {
                    isDehydrated = true;
                }
                else
                {
                    isDehydrated = false;
                }
                ThirstManager.partyThirst[party] = new ThirstData(newPercentage, isDehydrated);
                if (party == MobileParty.MainParty)
                {
                    ThirstManager.mainPartyRemainingWaterPercentage = newPercentage;
                    ThirstManager.mainPartyIsDehydrated = isDehydrated;
                }
                HandleDehydration(party, isDehydrated);
            }
        }

        public static float MakeWaterConsumption(MobileParty party, float partyRemainingWaterPercentage)
        {
            ItemRoster itemRoster = party.ItemRoster;
            int maxValueWater = 0;
            int maxValueAlcohol = 0;
            for (int index = 0; index < itemRoster.Count; index++)
            {
                if (itemRoster.GetItemAtIndex(index) != null && itemRoster.GetItemAtIndex(index).ItemCategory.StringId == "water")
                {
                    maxValueWater += itemRoster.GetElementNumber(index);
                }
                else if (itemRoster.GetItemAtIndex(index).ItemCategory.StringId == "bf_mead" ||
                    itemRoster.GetItemAtIndex(index).ItemCategory.StringId == "mead" ||
                    itemRoster.GetItemAtIndex(index).ItemCategory.StringId == "beer" ||
                    itemRoster.GetItemAtIndex(index).ItemCategory.StringId == "wine")
                {
                    maxValueAlcohol += itemRoster.GetElementNumber(index);
                }

            }
            int maxValue = maxValueWater + maxValueAlcohol;
            while (maxValue > 0 && partyRemainingWaterPercentage <= 0)
            {
                for (int index = itemRoster.Count - 1; index >= 0; --index)
                {
                    int elementNumber = itemRoster.GetElementNumber(index);
                    if (elementNumber > 0)
                    {
                        if (itemRoster.GetItemAtIndex(index).ItemCategory.StringId == "water")
                        {
                            partyRemainingWaterPercentage += 100;
                            if (elementNumber >= 1)
                            {
                                --maxValue;
                                --maxValueWater;
                            }
                            itemRoster.AddToCounts(itemRoster.GetItemAtIndex(index), -1);
                        }
                        else if (itemRoster.GetItemAtIndex(index).ItemCategory != null &&
                            ((itemRoster.GetItemAtIndex(index).ItemCategory.StringId == "beer") ||
                            itemRoster.GetItemAtIndex(index).ItemCategory.StringId == "wine") ||
                            itemRoster.GetItemAtIndex(index).ItemCategory.StringId == "mead")
                        {
                            partyRemainingWaterPercentage += 100;
                            if (elementNumber >= 1)
                            {
                                --maxValue;
                                --maxValueAlcohol;
                            }
                            itemRoster.AddToCounts(itemRoster.GetItemAtIndex(index), -1);
                        }
                    }
                }
            }
            return partyRemainingWaterPercentage;
        }

        public static void HandleDehydration(MobileParty party, bool isDehydrated)
        {
            CampaignTime campaignTime = CampaignData.CampaignStartTime;
            int toDays1 = (int)campaignTime.ToDays;
            campaignTime = CampaignTime.Now;
            int toDays2 = (int)campaignTime.ToDays;
            if (toDays1 != toDays2)
            {
                if (isDehydrated)
                {
                    int dehydrationMoralePenalty = Campaign.Current.Models.PartyMoraleModel.GetDailyStarvationMoralePenalty(party.Party);
                    if (party.IsMainParty)
                    {
                        MBTextManager.SetTextVariable("MORALE_PENALTY", -dehydrationMoralePenalty);
                        MBInformationManager.AddQuickInformation(new TextObject("Your party is dehydrated. You lose {MORALE_PENALTY} morale."));
                        campaignTime = CampaignTime.Now;
                        party.MoraleExplained.Add(dehydrationMoralePenalty, new TextObject("Dehydrated"));
                        if ((int)campaignTime.ToDays % 3 == 0 && party.MemberRoster.TotalManCount > 1)
                        {
                            TraitLevelingHelper.OnPartyStarved();
                        }
                    }
                    else
                    {
                        party.RecentEventsMorale += dehydrationMoralePenalty;
                    }
                }
                if (party.MemberRoster.TotalManCount > 1)
                {
                    SkillLevelingManager.OnFoodConsumed(party, isDehydrated);
                    if (!isDehydrated && party.IsMainParty && (double)party.Morale >= 90.0 && party.MemberRoster.TotalRegulars >= 20)
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
