using Bannerlord.UIExtenderEx;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using Thirst.Behavior;
using Thirst.Behaviors;
using Thirst.ItemCategories;
using Thirst.Managers;
using Thirst.Patches;
using Thirst.WaterItems;

namespace Thirst
{
    public class Main : MBSubModuleBase
    {
        private static readonly UIExtender uiExtender = new UIExtender(typeof(SubModule).Namespace);
        private static readonly Harmony harmony = new Harmony("Thirst");
        public static readonly ThirstManager thirst = new ThirstManager();
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            harmony.PatchAll();
            Main.uiExtender.Register(typeof(Main).Assembly);
            Main.uiExtender.Enable();
        }
        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);
            if (gameStarterObject.GetType() == typeof(CampaignGameStarter))
            {
                WaterItemCategory waterItemCategory = new WaterItemCategory();
                waterItemCategory.Initialize();
                WaterItem water = new WaterItem();
                water.Initialize();
                ((CampaignGameStarter)gameStarterObject).AddBehavior(new PartyThirstBehavior());
                ((CampaignGameStarter)gameStarterObject).AddBehavior(new SettlementThirstBehavior());
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
