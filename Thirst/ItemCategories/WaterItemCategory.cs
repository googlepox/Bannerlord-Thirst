﻿using TaleWorlds.Core;

namespace Thirst.ItemCategories
{
    public class WaterItemCategory
    {
        public ItemCategory WaterCategory
        {
            get; set;
        }

        public void Initialize()
        {
            this.WaterCategory = Game.Current.ObjectManager.RegisterPresumedObject(new ItemCategory("water"));
            this.WaterCategory.InitializeObject(true, 50, properties: ItemCategory.Property.BonusToFoodStores);
        }
    }
}
