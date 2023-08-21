using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.ViewModels;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapBar;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using Thirst.Managers;

namespace Thirst.UI
{
    [ViewModelMixin("UpdatePlayerInfo")]
    internal class ThirstMapBarMixin : BaseViewModelMixin<MapInfoVM>
    {
        private float totalWater;
        private float water;
        private string waterAbbr;
        private bool _isDailyConsumptionTooltipWarningWater;
        private BasicTooltipViewModel thirstHint;
        private static MobileParty mainParty = MobileParty.MainParty;

        public ThirstMapBarMixin(MapInfoVM vm)
          : base(vm)
        {

        }

        [DataSourceProperty]
        public float TotalWater
        {
            get => this.totalWater;
            set
            {
                if (value == this.totalWater)
                {
                    return;
                }

                this.totalWater = value;
                this.ViewModel.OnPropertyChangedWithValue(value, nameof(TotalWater));
            }
        }

        [DataSourceProperty]
        public string WaterWithAbbrText
        {
            get => this.waterAbbr;

            set
            {
                if (!(value != this.waterAbbr))
                {
                    return;
                }

                this.waterAbbr = value;
                this.ViewModel.OnPropertyChangedWithValue<string>(value, nameof(WaterWithAbbrText));
            }
        }

        [DataSourceProperty]
        public float Water
        {
            get => this.water;
            set
            {
                if (value == this.water)
                {
                    return;
                }

                this.water = value;
                this.ViewModel.OnPropertyChangedWithValue(value, nameof(Water));
            }
        }

        [DataSourceProperty]
        public bool IsDailyConsumptionTooltipWarningWater
        {
            get => this._isDailyConsumptionTooltipWarningWater;
            set
            {
                if (value == this._isDailyConsumptionTooltipWarningWater)
                {
                    return;
                }

                this._isDailyConsumptionTooltipWarningWater = value;
                base.ViewModel.OnPropertyChangedWithValue(value, nameof(IsDailyConsumptionTooltipWarningWater));
            }
        }

        [DataSourceProperty]
        public BasicTooltipViewModel ThirstHint
        {
            get => this.thirstHint;
            set
            {
                if (value == this.thirstHint)
                {
                    return;
                }

                this.thirstHint = value;
                this.ViewModel.OnPropertyChangedWithValue((object)value, nameof(ThirstHint));
            }
        }

        public override void OnRefresh()
        {
            this.Water = (float)PartyThirstManager.GetWater(mainParty);
            this.Water = (this.Water < 0.0f ? 0.0f : this.Water);
            float preTotalWater = (((float)ThirstManager.mainPartyRemainingWaterPercentage + ((this.Water) * 100.0f))/100.0f) < 0.0f ? 0.0f : (float)((ThirstManager.mainPartyRemainingWaterPercentage + ((this.Water) * 100.0f))/100.0f);
            this.TotalWater = preTotalWater;
            this.WaterWithAbbrText = CampaignUIHelper.GetAbbreviatedValueTextFromValue((int)this.TotalWater);
            this.ThirstHint = new BasicTooltipViewModel(() => GetPartyWaterTooltip(mainParty));
            this._isDailyConsumptionTooltipWarningWater = PartyThirstManager.GetNumDaysForWaterToLast(mainParty) < 1;
            this.IsDailyConsumptionTooltipWarningWater = PartyThirstManager.GetNumDaysForWaterToLast(mainParty) < 1;
        }

        public List<TooltipProperty> GetPartyWaterTooltip(MobileParty mainParty)
        {
            List<TooltipProperty> properties = new List<TooltipProperty>();
            float num1 = this.TotalWater;
            string str = num1.ToString("0.##");
            properties.Add(new TooltipProperty("Water", str, 0, modifier: TooltipProperty.TooltipPropertyFlags.Title));
            ExplainedNumber waterChangeExplained = PartyThirstManager.WaterChangeExplained(mainParty);
            List<(string, float)> lines = waterChangeExplained.GetLines();
            if (lines.Count > 0)
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    (string, float) tuple = lines[i];
                    string item = tuple.Item1;
                    string changeValueString1 = GetChangeValueString(tuple.Item2);
                    properties.Add(new TooltipProperty(item, changeValueString1, 0));
                }
            }
            properties.Add(new TooltipProperty("", string.Empty, 0, false, TooltipProperty.TooltipPropertyFlags.RundownSeperator));
            string changeValueString2 = GetChangeValueString(waterChangeExplained.ResultNumber);
            properties.Add(new TooltipProperty("Expected Change", changeValueString2, 0, modifier: TooltipProperty.TooltipPropertyFlags.RundownResult));
            properties.Add(new TooltipProperty(string.Empty, string.Empty, -1, false));
            List<TooltipProperty> collection1 = new List<TooltipProperty>();
            int num2 = 0;
            List<TooltipProperty> collection2 = new List<TooltipProperty>();
            for (int index = 0; index < mainParty.ItemRoster.Count; ++index)
            {
                ItemRosterElement itemRosterElement = mainParty.ItemRoster[index];
                if (!itemRosterElement.IsEmpty && itemRosterElement.EquipmentElement.Item.ItemCategory.StringId == "water")
                {
                    EquipmentElement equipmentElement = itemRosterElement.EquipmentElement;
                    ItemObject itemObject1 = equipmentElement.Item;
                    float modifiedAmount;
                    List<TooltipProperty> tooltipPropertyList = collection1;
                    equipmentElement = itemRosterElement.EquipmentElement;
                    modifiedAmount = itemRosterElement.Amount * 1.00f;
                    TooltipProperty tooltipProperty = new TooltipProperty(equipmentElement.GetModifiedItemName().ToString(), modifiedAmount.ToString(format: "0.##"), 0);
                    tooltipPropertyList.Add(tooltipProperty);
                    num2 += itemRosterElement.Amount;
                }
                else if (!itemRosterElement.IsEmpty &&
                         itemRosterElement.EquipmentElement.Item.ItemCategory.StringId == "mead" ||
                         itemRosterElement.EquipmentElement.Item.ItemCategory.StringId == "beer" ||
                         itemRosterElement.EquipmentElement.Item.ItemCategory.StringId == "bf_mead" ||
                         itemRosterElement.EquipmentElement.Item.ItemCategory.StringId == "wine")
                {
                    EquipmentElement equipmentElement = itemRosterElement.EquipmentElement;
                    ItemObject itemObject1 = equipmentElement.Item;
                    float modifiedAmount;
                    List<TooltipProperty> tooltipPropertyList = collection1;
                    equipmentElement = itemRosterElement.EquipmentElement;
                    modifiedAmount = itemRosterElement.Amount * 1.00f;
                    TooltipProperty tooltipProperty = new TooltipProperty(equipmentElement.GetModifiedItemName().ToString(), modifiedAmount.ToString(format: "0.##"), 0);
                    tooltipPropertyList.Add(tooltipProperty);
                    num2 += itemRosterElement.Amount;
                }
            }
            if (num2 > 0)
            {
                properties.Add(new TooltipProperty("Water", num2.ToString(), 0));
                properties.Add(new TooltipProperty("", string.Empty, 0, false, TooltipProperty.TooltipPropertyFlags.RundownSeperator));
                properties.AddRange(collection1);
                properties.Add(new TooltipProperty(string.Empty, string.Empty, -1, false));
            }
            int daysForWaterToLast = PartyThirstManager.GetNumDaysForWaterToLast(mainParty);
            properties.Add(new TooltipProperty(new TextObject("Days until no water").ToString(), PartyThirstManager.GetDaysUntilNoWater(daysForWaterToLast, waterChangeExplained.ResultNumber), 0));
            return properties;
        }

        private string GetChangeValueString(float value)
        {
            string text = value.ToString("0.##");
            if ((double)value <= 1.0 / 1000.0)
            {
                return text;
            }

            MBTextManager.SetTextVariable("NUMBER", text, false);
            return GameTexts.FindText("str_plus_with_number").ToString();
        }

    }
}
