using Bannerlord.UIExtenderEx;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using Thirst.Behavior;
using Thirst.ItemCategories;
using Thirst.Managers;
using Thirst.Models;
using Thirst.Patches;
using Thirst.WaterItems;

namespace Thirst
{
    public class SubModule : MBSubModuleBase
    {
        public static readonly UIExtender uiExtender = new UIExtender(typeof(SubModule).Namespace);
        public static readonly Harmony harmony = new Harmony("Thirst");
        public static ThirstManager thirst;
        public static PartyWaterBuyingModel buyModel;
        public static WaterItemCategory waterItemCategory;
        public static WaterItem waterItem;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
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
        public override void OnGameInitializationFinished(Game game)
        {
            base.OnGameInitializationFinished(game);
            var speedModel = AccessTools.Method(typeof(DefaultPartySpeedCalculatingModel), "CalculateFinalSpeed");
            harmony.Patch(speedModel, postfix: new HarmonyMethod(AccessTools.Method(typeof(CalculateFinalSpeedPatch), "SpeedFinalizer")));
            var moraleModel = AccessTools.Method(typeof(DefaultPartyMoraleModel), "GetEffectivePartyMorale");
            harmony.Patch(moraleModel, postfix: new HarmonyMethod(AccessTools.Method(typeof(GetEffectivePartyMoralePatchDefault), "MoraleFinalizer")));
        }
    }
}
