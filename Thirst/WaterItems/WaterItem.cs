using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Thirst.WaterItems
{
    public class WaterItem
    {
        public ItemObject Water
        {
            get; set;
        }
        public static ItemObject WaterObj
        {
            get; private set;
        }

        public void Initialize()
        {
            this.Water = Game.Current.ObjectManager.RegisterPresumedObject<ItemObject>(new ItemObject("water"));
            ItemObject.InitializeTradeGood(this.Water, new TextObject("{=Thirst_Water}Water{@Plural}barrels of water{\\@}"), "bd_barrel_a", SubModule.thirst.WaterCat, 5, 10f, ItemObject.ItemTypeEnum.Goods, false);
        }
    }
}
