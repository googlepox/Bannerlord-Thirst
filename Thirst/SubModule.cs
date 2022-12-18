using Thirst.Behavior;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using HarmonyLib;
using Bannerlord.UIExtenderEx;
using MCM.Internal.Extensions;
using Thirst.ItemCategories;
using Thirst.Managers;
using Thirst.Models;
using Thirst.WaterItems;
using MCM.Implementation;
using System;
using System.Linq;
using TaleWorlds.CampaignSystem.Settlements;

namespace Thirst
{
	public class SubModule : MBSubModuleBase
	{
        private static readonly UIExtender uiExtender = new UIExtender(typeof(SubModule).Namespace);
        private static readonly Harmony harmony = new Harmony("Thirst");
        public static ThirstManager thirst;
        public static PartyWaterBuyingModel buyModel;
        public static WaterItemCategory waterItemCategory;
        public static WaterItem waterItem;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            uiExtender.Register(typeof(SubModule).Assembly);
            uiExtender.Enable();
        }
        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
		{
			if (gameStarterObject.GetType() == typeof(CampaignGameStarter))
			{
				base.OnCampaignStart(game, gameStarterObject);
                harmony.PatchAll();
                thirst = new ThirstManager();
                buyModel = new PartyWaterBuyingModel();
                waterItemCategory = new WaterItemCategory();
                waterItemCategory.Initialize();
                thirst.InitializeItemCategories();
                waterItem = new WaterItem();
                waterItem.Initialize();
                ((CampaignGameStarter)gameStarterObject).AddBehavior(new ThirstBehavior());
                ((CampaignGameStarter)gameStarterObject).AddBehavior(new PartiesBuyWaterCampaignBehavior());
            }
		}
	}
}
