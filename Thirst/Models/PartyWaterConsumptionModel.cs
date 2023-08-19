using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;
using Thirst.Managers;

namespace Thirst.Models
{
    public class PartyWaterConsumptionModel
    {
        [SaveableField(3)]
        public bool IsDehydrated;

        public float WaterChange;

        [SaveableField(1)]
        public float RemainingWaterPercentage;
        public static int NumberOfMenOnMapToDrinkOneWater => 15;

        [SaveableField(2)]
        public static bool IsWaterGiven = false;

        public PartyWaterConsumptionModel()
        {
            this.IsDehydrated = false;
        }

        public float GetWaterChange(MobileParty mainParty)
        {
            this.WaterChange = WaterChangeExplained(mainParty).ResultNumber;
            return this.WaterChange;
        }

        public float GetRemainingWaterPercentage(MobileParty party)
        {
            if (ThirstManager.partyThirst.ContainsKey(party))
            {
                PartyWaterConsumptionModel partyModel = ThirstManager.partyThirst[party];
                return partyModel.RemainingWaterPercentage;
            }
            else
            {
                return 0;
            }
        }

        public void SetRemainingWaterPercentage(MobileParty party, float percentage)
        {
            if (ThirstManager.partyThirst.ContainsKey(party))
            {
                PartyWaterConsumptionModel partyModel = ThirstManager.partyThirst[party];
                partyModel.RemainingWaterPercentage = percentage;
            }
        }

        public bool GetIsDehydrated(MobileParty party)
        {
            if (ThirstManager.partyThirst.ContainsKey(party))
            {
                PartyWaterConsumptionModel partyModel = ThirstManager.partyThirst[party];
                return partyModel.IsDehydrated;
            }
            else
            {
                return false;
            }
        }

        public void SetIsDehydrated(MobileParty party, bool dehydrated)
        {
            if (ThirstManager.partyThirst.ContainsKey(party))
            {
                PartyWaterConsumptionModel partyModel = ThirstManager.partyThirst[party];
                partyModel.IsDehydrated = dehydrated;
            }
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
        public static string GetDaysUntilNoWater(int daysForWaterToLast, float waterChange)
        {
            if ((double)daysForWaterToLast <= 1.4012984643248171E-45)
            {
                return new TextObject("{=koX9okuG}None").ToString();
            }

            return (double)waterChange >= -1.4012984643248171E-45 ? "Never" : daysForWaterToLast.ToString();
        }

        public float GetWater(MobileParty party)
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

        public int GetNumDaysForWaterToLast(MobileParty party)
        {
            float num1 = (GetWater(party)) * 100f;
            num1 += this.GetRemainingWaterPercentage(party);
            return (int)((double)num1 / (100.0 * -(double)this.WaterChange));
        }

        private int MakeWaterConsumption(MobileParty party, int partyRemainingWaterPercentage)
        {
            ItemRoster itemRoster = party.ItemRoster;
            int maxValueWater = 0;
            int maxValueAlcohol = 0;
            for (int index = 0; index < itemRoster.Count; index++)
            {
                if (itemRoster.GetItemAtIndex(index) != null && itemRoster.GetItemAtIndex(index).ItemCategory.StringId == "water")
                {
                    maxValueWater++;
                }
                else if (itemRoster.GetItemAtIndex(index).ItemCategory.StringId == "bf_mead" ||
                    itemRoster.GetItemAtIndex(index).ItemCategory.StringId == "mead" ||
                    itemRoster.GetItemAtIndex(index).ItemCategory.StringId == "beer" ||
                    itemRoster.GetItemAtIndex(index).ItemCategory.StringId == "wine")
                {
                    maxValueAlcohol++;
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
            //this.SetRemainingWaterPercentage(party, partyRemainingWaterPercentage);
            return partyRemainingWaterPercentage;
        }

        public void CalculateInjuryWaterDirty(MobileParty party, int dirty)
        {
            Random rand = new Random();
            int chance = rand.Next(0, 100);
            int woundedCount = dirty * 10;
            while (woundedCount > 0)
            {
                int randElement = MBRandom.RandomInt(0, woundedCount);
                if (chance >= 25)
                {
                    CharacterObject randTroop = party.MemberRoster.GetCharacterAtIndex(randElement);
                    party.Party.WoundMemberRosterElements(randTroop, 1);
                }
                woundedCount -= 1;
            }
        }

        public void CalculateInjuryWaterFilthy(MobileParty party, int filthy)
        {
            Random rand = new Random();
            int chance = rand.Next(0, 100);
            int woundedCount = filthy * 10;
            while (woundedCount > 0)
            {
                int randElement = MBRandom.RandomInt(0, woundedCount);
                if (chance >= 50)
                {
                    CharacterObject randTroop = party.MemberRoster.GetCharacterAtIndex(randElement);
                    party.Party.WoundMemberRosterElements(randTroop, 1);
                }
                woundedCount -= 1;
            }
        }

        public void PartyConsumeWater(MobileParty party)
        {
            float waterChange = this.GetWaterChange(party);
            double num1 = (double)waterChange < 0.0 ? -(double)waterChange : 0.0;
            float percentage = (float)SubModule.thirst.mainModel.GetRemainingWaterPercentage(party);
            int partyRemainingWaterPercentageOld = (int)(percentage < 0.0f ? 0.0f : percentage) - MathF.Round((float)(num1 * 100.0f));
            int partyRemainingWaterPercentage = this.MakeWaterConsumption(party, partyRemainingWaterPercentageOld);
            float newPercentage = partyRemainingWaterPercentage < 0.0f ? 0.0f : partyRemainingWaterPercentage;
            this.SetRemainingWaterPercentage(party, newPercentage);
            if (newPercentage + SubModule.thirst.mainModel.GetWater(party) <= 0)
            {
                this.IsDehydrated = true;
            }
            else
            {
                this.IsDehydrated = false;
            }
            this.SetIsDehydrated(party, this.IsDehydrated);
            CampaignTime campaignTime = CampaignData.CampaignStartTime;
            int toDays1 = (int)campaignTime.ToDays;
            campaignTime = CampaignTime.Now;
            int toDays2 = (int)campaignTime.ToDays;
            if (toDays1 != toDays2)
            {
                if (this.IsDehydrated)
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
                    SkillLevelingManager.OnFoodConsumed(party, this.IsDehydrated);
                    if (!this.IsDehydrated && party.IsMainParty && (double)party.Morale >= 90.0 && party.MemberRoster.TotalRegulars >= 20)
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
