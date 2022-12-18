using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.ViewModels;
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapBar;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using Thirst.Models;

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

        [DataSourceProperty]
        public float TotalWater
        {
            get
            {
                return this.totalWater;
            }
            set
            {
                bool flag = value != this.totalWater;
                if (flag)
                {
                    this.totalWater = value;
                    base.ViewModel.OnPropertyChangedWithValue(value, "TotalWater");
                }
            }
        }

        [DataSourceProperty]
        public string WaterWithAbbrText
        {
            get
            {
                return this.waterAbbr;
            }
            set
            {
                bool flag = value != this.waterAbbr;
                if (flag)
                {

                    this.waterAbbr = value;
                    base.ViewModel.OnPropertyChangedWithValue(value, "WaterWithAbbrText");
                }
            }
        }

        [DataSourceProperty]
        public float Water
        {
            get
            {
                return this.water;
            }
            set
            {
                bool flag = value != this.water;
                if (flag)
                {
                    this.water = value;
                    base.ViewModel.OnPropertyChangedWithValue(value, "Water");
                }
            }
        }

        [DataSourceProperty]
        public bool IsDailyConsumptionTooltipWarningWater
        {
            get => this._isDailyConsumptionTooltipWarningWater;
            set
            {
                if (value == this._isDailyConsumptionTooltipWarningWater)
                    return;
                this._isDailyConsumptionTooltipWarningWater = value;
                base.ViewModel.OnPropertyChangedWithValue((object)value, nameof(IsDailyConsumptionTooltipWarningWater));
            }
        }

        public ThirstMapBarMixin(MapInfoVM vm)
          : base(vm)
        {
            
        }

        [DataSourceProperty]
        public BasicTooltipViewModel ThirstHint
        {
            get => this.thirstHint;
            set
            {
                if (value == this.thirstHint)
                    return;
                this.thirstHint = value;
                this.ViewModel.OnPropertyChangedWithValue((object)value, nameof(ThirstHint));
            }
        }

        public override void OnRefresh()
        {
            this.water = SubModule.thirst.mainModel.GetWater(mainParty);
            this.totalWater = (float) (SubModule.thirst.mainModel.RemainingWaterPercentage + ((this.water - 1) * 100))/100;
            this.waterAbbr = CampaignUIHelper.GetAbbreviatedValueTextFromValue((int) this.water);
            this.ThirstHint = new BasicTooltipViewModel((Func<List<TooltipProperty>>)(() => GetPartyWaterTooltip(mainParty)));
            this._isDailyConsumptionTooltipWarningWater = SubModule.thirst.mainModel.GetNumDaysForWaterToLast(mainParty) < 1;
        }

        public List<TooltipProperty> GetPartyWaterTooltip(MobileParty mainParty)
        {
            List<TooltipProperty> properties = new List<TooltipProperty>();
            float num1 = this.totalWater;
            string str = num1.ToString("0.##");
            properties.Add(new TooltipProperty("Water", str, 0, modifier: TooltipProperty.TooltipPropertyFlags.Title));
            ExplainedNumber waterChangeExplained = SubModule.thirst.mainModel.WaterChangeExplained(mainParty);
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
            SubModule.thirst.mainModel.WaterChange = waterChangeExplained.ResultNumber;
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
                    if ((itemObject1 != null ? (itemObject1.Name.Contains("Pristine") ? 1 : 0) : 0) != 0)
                    {
                        List<TooltipProperty> tooltipPropertyList = collection1;
                        equipmentElement = itemRosterElement.EquipmentElement;
                        modifiedAmount = itemRosterElement.Amount * 1.75f;
                        TooltipProperty tooltipProperty = new TooltipProperty(equipmentElement.GetModifiedItemName().ToString(), modifiedAmount.ToString(format: "0.##"), 0);
                        tooltipPropertyList.Add(tooltipProperty);
                        num2 += itemRosterElement.Amount;
                    }
                    else if ((itemObject1 != null ? (itemObject1.Name.Contains("Clean") ? 1 : 0) : 0) != 0)
                    {
                        List<TooltipProperty> tooltipPropertyList = collection1;
                        equipmentElement = itemRosterElement.EquipmentElement;
                        modifiedAmount = itemRosterElement.Amount * 1.50f;
                        TooltipProperty tooltipProperty = new TooltipProperty(equipmentElement.GetModifiedItemName().ToString(), modifiedAmount.ToString(format: "0.##"), 0);
                        tooltipPropertyList.Add(tooltipProperty);
                        num2 += itemRosterElement.Amount;
                    }
                    else if ((itemObject1 != null ? (itemObject1.Name.Contains("Dirty") ? 1 : 0) : 0) != 0)
                    {
                        List<TooltipProperty> tooltipPropertyList = collection1;
                        equipmentElement = itemRosterElement.EquipmentElement;
                        modifiedAmount = itemRosterElement.Amount * 0.50f;
                        TooltipProperty tooltipProperty = new TooltipProperty(equipmentElement.GetModifiedItemName().ToString(), modifiedAmount.ToString(format: "0.##"), 0);
                        tooltipPropertyList.Add(tooltipProperty);
                        num2 += itemRosterElement.Amount;
                    }
                    else if ((itemObject1 != null ? (itemObject1.Name.Contains("Filthy") ? 1 : 0) : 0) != 0)
                    {
                        List<TooltipProperty> tooltipPropertyList = collection1;
                        equipmentElement = itemRosterElement.EquipmentElement;
                        modifiedAmount = itemRosterElement.Amount * 0.25f;
                        TooltipProperty tooltipProperty = new TooltipProperty(equipmentElement.GetModifiedItemName().ToString(), modifiedAmount.ToString(format:"0.##"), 0);
                        tooltipPropertyList.Add(tooltipProperty);
                        num2 += itemRosterElement.Amount;
                    }
                    else if ((itemObject1 != null ? (itemObject1.Name.ToString() == ("Water") ? 1 : 0) : 0) != 0)
                    {
                        List<TooltipProperty> tooltipPropertyList = collection1;
                        equipmentElement = itemRosterElement.EquipmentElement;
                        modifiedAmount = itemRosterElement.Amount * 1.00f;
                        TooltipProperty tooltipProperty = new TooltipProperty(equipmentElement.GetModifiedItemName().ToString(), modifiedAmount.ToString(format: "0.##"), 0);
                        tooltipPropertyList.Add(tooltipProperty);
                        num2 += itemRosterElement.Amount;
                    }
                }
            }
            if (num2 > 0)
            {
                properties.Add(new TooltipProperty("Water", num2.ToString(), 0));
                properties.Add(new TooltipProperty("", string.Empty, 0, false, TooltipProperty.TooltipPropertyFlags.RundownSeperator));
                properties.AddRange((IEnumerable<TooltipProperty>)collection1);
                properties.Add(new TooltipProperty(string.Empty, string.Empty, -1, false));
            }
            properties.Add(new TooltipProperty(new TextObject("Days until no water").ToString(), PartyWaterConsumptionModel.GetDaysUntilNoWater(num1, waterChangeExplained.ResultNumber), 0));
            return properties;
        }

        private string GetChangeValueString(float value)
        {
            string text = value.ToString("0.##");
            if ((double)value <= 1.0 / 1000.0)
                return text;
            MBTextManager.SetTextVariable("NUMBER", text, false);
            return GameTexts.FindText("str_plus_with_number").ToString();
        }

    }
}
