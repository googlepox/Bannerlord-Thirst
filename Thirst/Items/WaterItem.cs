using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace Thirst.WaterItems
{
    public class WaterItem
    {
        public ItemObject Water
        {
            get; set;
        }

        public void Initialize()
        {
            this.Water = Game.Current.ObjectManager.RegisterPresumedObject(new ItemObject("water"));
            ItemObject.InitializeTradeGood(this.Water, new TextObject("{=Thirst_Water}Water{@Plural}barrels of water{\\@}"), "bd_barrel_a", MBObjectManager.Instance.GetObject<ItemCategory>(x => x.StringId == "water"), 5, 10f, ItemObject.ItemTypeEnum.Goods, false);
        }
    }
}
