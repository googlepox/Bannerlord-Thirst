using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.SaveSystem;
using Thirst.WaterItems;

namespace Thirst.Models
{
    public class PartyWaterConsumptionModel
    {
        public bool IsDehydrated = false;

        public float WaterChange;

        public float RemainingWaterPercentage;
        public static int NumberOfMenOnMapToDrinkOneWater => 20;

        [SaveableField(3)]
        public static bool IsWaterGiven = false;

        public PartyWaterConsumptionModel()
        {
            IsDehydrated = false;
            WaterChange = 0;
            RemainingWaterPercentage = 100;
        }

        public float GetWaterChange(MobileParty mainParty)
        {
            WaterChange = WaterChangeExplained(mainParty).ResultNumber;
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
        public static string GetDaysUntilNoWater(float totalWater, float waterChange)
        {
            if ((double)totalWater <= 1.4012984643248171E-45)
                return new TextObject("{=koX9okuG}None").ToString();
            return (double)waterChange >= -1.4012984643248171E-45 ? "Never" : MathF.Ceiling(MathF.Abs(totalWater / waterChange)).ToString();
        }

        public float GetWater(MobileParty party)
        {
            float waterItems = 0;
            for (int index = 0; index < party.ItemRoster.Count; ++index)
            {
                ItemRosterElement itemRosterElement = party.ItemRoster[index];
                if (!itemRosterElement.IsEmpty)
                {
                    EquipmentElement equipmentElement = itemRosterElement.EquipmentElement;
                    ItemObject itemObject1 = equipmentElement.Item;
                    if (itemObject1.ItemCategory != null && itemObject1.ItemCategory.StringId == "water")
                    {
                        if (itemObject1.Name.ToString().Contains("Pristine"))
                        {
                            waterItems += (itemRosterElement.Amount * 1.75f);
                        }
                        else if (itemObject1.Name.ToString().Contains("Clean"))
                        {
                            waterItems += (itemRosterElement.Amount * 1.50f);
                        }
                        else if (itemObject1.Name.ToString() == "Water")
                        {
                            waterItems += (itemRosterElement.Amount);
                        }
                        else if (itemObject1.Name.ToString().Contains("Dirty"))
                        {
                            waterItems += (itemRosterElement.Amount * 0.50f);
                        }
                        else if (itemObject1.Name.ToString().Contains("Filthy"))
                        {
                            waterItems += (itemRosterElement.Amount * 0.25f);
                        }
                    }
                    else if (itemObject1.StringId == "mead" || itemObject1.StringId == "wine" || itemObject1.StringId == "beer")
                    {
                        waterItems += (itemRosterElement.Amount);
                    }
                }
            }
            return waterItems;
        }

        public int GetNumDaysForWaterToLast(MobileParty party)
        {
            float num1 = GetWater(party) * 100;
            if (party == MobileParty.MainParty)
                num1 += (int) this.RemainingWaterPercentage;
            return (int)((double)num1 / (100.0 * -(double)this.WaterChange));
        }

        private int MakeWaterConsumption(MobileParty party, int partyRemainingWaterPercentage)
        {
            ItemRoster itemRoster = party.ItemRoster;
            int maxValueWater = 0;
            int maxValueWaterDirty = 0;
            int maxValueWaterFilthy = 0;
            int maxValueWaterClean = 0;
            int maxValueWaterPristine = 0;
            int maxValueAlcohol = 0;
            int dirtyConsumed = 0;
            int filthyConsumed = 0;
            for (int index = 0; index < itemRoster.Count; ++index)
            {
                if (itemRoster.GetItemAtIndex(index) != null && itemRoster.GetItemAtIndex(index).ItemCategory.StringId == "water")
                {
                    if (itemRoster.GetItemAtIndex(index) != null &&
                        itemRoster.GetItemAtIndex(index).Name.Contains("Pristine"))
                    {
                        ++maxValueWaterPristine;
                    }
                    else if (itemRoster.GetItemAtIndex(index) != null &&
                             itemRoster.GetItemAtIndex(index).Name.Contains("Clean"))
                    {
                        ++maxValueWaterClean;
                    }
                    else if (itemRoster.GetItemAtIndex(index) != null &&
                             itemRoster.GetItemAtIndex(index).Name.ToString() == "Water")
                    {
                        ++maxValueWater;
                    }
                    else if (itemRoster.GetItemAtIndex(index) != null &&
                             itemRoster.GetItemAtIndex(index).Name.Contains("Dirty"))
                    {
                        ++maxValueWaterDirty;
                    }
                    else if (itemRoster.GetItemAtIndex(index) != null &&
                             itemRoster.GetItemAtIndex(index).Name.Contains("Filthy"))
                    {
                        ++maxValueWaterFilthy; ;
                    }
                }
                else if (itemRoster.GetItemAtIndex(index).StringId == "mead" || 
                    itemRoster.GetItemAtIndex(index).StringId == "wine" || 
                    itemRoster.GetItemAtIndex(index).StringId == "beer")
                {
                    maxValueAlcohol++;
                }

            }
            int maxValue = maxValueWater + maxValueWaterDirty + maxValueWaterFilthy + maxValueWaterClean + maxValueWaterPristine + maxValueAlcohol;
            while (maxValue > 0 && partyRemainingWaterPercentage <= 0)
            {
                int num1 = MBRandom.RandomInt(maxValue);
                bool flag2 = false;
                int num2 = 0;
                for (int index = itemRoster.Count - 1; index >= 0 && !flag2; --index)
                {
                    int elementNumber = itemRoster.GetElementNumber(index);
                    if (elementNumber > 0)
                    {
                        ++num2;
                        if (num1 < num2)
                        {
                            if (itemRoster.GetItemAtIndex(index).ItemCategory != null && itemRoster.GetItemAtIndex(index).ItemCategory.StringId == "water")
                            {
                                if (itemRoster.GetItemAtIndex(index).Name.Contains("Pristine"))
                                {
                                    partyRemainingWaterPercentage += 175;
                                    if (elementNumber == 1)
                                    {
                                        --maxValue;
                                        --maxValueWaterPristine;
                                    }
                                    flag2 = true;
                                    itemRoster.AddToCounts(itemRoster.GetItemAtIndex(index), -1);
                                }
                                else if (itemRoster.GetItemAtIndex(index).Name.Contains("Clean") && maxValueWaterPristine <= 0)
                                {
                                    partyRemainingWaterPercentage += 150;
                                    if (elementNumber == 1)
                                    {
                                        --maxValue;
                                        --maxValueWaterClean;
                                    }
                                    flag2 = true;
                                    itemRoster.AddToCounts(itemRoster.GetItemAtIndex(index), -1);
                                }
                                else if (itemRoster.GetItemAtIndex(index).Name.ToString() == "Water" && maxValueWaterPristine <= 0 && maxValueWaterPristine <= 0)
                                {
                                    partyRemainingWaterPercentage += 100;
                                    if (elementNumber == 1)
                                    {
                                        --maxValue;
                                        --maxValueWater;
                                    }
                                    flag2 = true;
                                    itemRoster.AddToCounts(itemRoster.GetItemAtIndex(index), -1);
                                }
                                else if (itemRoster.GetItemAtIndex(index).Name.Contains("Dirty") && maxValueWaterPristine <= 0 && maxValueWaterPristine <= 0 && maxValueWater <= 0)
                                {
                                    partyRemainingWaterPercentage += 50;
                                    dirtyConsumed += 1;
                                    if (elementNumber == 1)
                                    {
                                        --maxValue;
                                        --maxValueWaterDirty;
                                    }
                                    flag2 = true;
                                    itemRoster.AddToCounts(itemRoster.GetItemAtIndex(index), -1);
                                }
                                else if (itemRoster.GetItemAtIndex(index).Name.Contains("Filthy") && maxValueWaterPristine <= 0 && maxValueWaterPristine <= 0 && maxValueWater <= 0 && maxValueWaterDirty <= 0)
                                {
                                    partyRemainingWaterPercentage += 25;
                                    filthyConsumed += 1;
                                    if (elementNumber == 1)
                                    {
                                        --maxValue;
                                        --maxValueWaterFilthy;
                                    }
                                    flag2 = true;
                                    itemRoster.AddToCounts(itemRoster.GetItemAtIndex(index), -1);
                                }
                            }
                            else if(itemRoster.GetItemAtIndex(index).ItemCategory != null && 
                                ((itemRoster.GetItemAtIndex(index).ItemCategory.StringId == "beer") || 
                                itemRoster.GetItemAtIndex(index).ItemCategory.StringId == "wine") ||
                                itemRoster.GetItemAtIndex(index).ItemCategory.StringId == "mead")
                            {
                                partyRemainingWaterPercentage += 100;
                                if (elementNumber == 1)
                                {
                                    --maxValue;
                                    --maxValueAlcohol;
                                }
                                itemRoster.AddToCounts(itemRoster.GetItemAtIndex(index), -1);
                            }

                        }
                    }
                }
            }
            CalculateInjuryWaterDirty(party, dirtyConsumed);
            CalculateInjuryWaterFilthy(party, filthyConsumed);
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
            bool isDehydrated1 = this.IsDehydrated;
            float waterChange = this.GetWaterChange(party);
            double num1 = (double)waterChange < 0.0 ? -(double)waterChange : 0.0;
            float percentage = (float)this.RemainingWaterPercentage;
            int partyRemainingWaterPercentageOld = (int)(percentage < 0 ? 0 : percentage) - MathF.Round((float)(num1 * 100));
            int partyRemainingWaterPercentage = this.MakeWaterConsumption(party, partyRemainingWaterPercentageOld);
            this.RemainingWaterPercentage = partyRemainingWaterPercentage < 0 ? 0 : partyRemainingWaterPercentage;
            if (this.RemainingWaterPercentage <= 0)
            {
                this.IsDehydrated = true;
            }
            else
            {
                this.IsDehydrated = false;
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
                        MBInformationManager.AddQuickInformation(new TextObject("Your party is dehydrated. You lose {MORALE_PENALTY} morale."));
                        campaignTime = CampaignTime.Now;
                        party.RecentEventsMorale -= 5;
                        party.MoraleExplained.Add(-5, new TextObject("Dehydrated"));
                        if ((int)campaignTime.ToDays % 3 == 0 && party.MemberRoster.TotalManCount > 1)
                            TraitLevelingHelper.OnPartyStarved();
                    }
                }
                if (party.MemberRoster.TotalManCount > 1)
                {
                    SkillLevelingManager.OnFoodConsumed(party, isDehydrated2);
                    if (!isDehydrated1 && !isDehydrated2 && party.IsMainParty && (double)party.Morale >= 90.0 && party.MemberRoster.TotalRegulars >= 20)
                    {
                        campaignTime = CampaignTime.Now;
                        if ((int)campaignTime.ToDays % 10 == 0)
                            TraitLevelingHelper.OnPartyTreatedWell();
                    }
                }
            }
        }
    }
}
