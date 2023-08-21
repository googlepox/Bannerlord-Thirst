using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.SaveSystem;
using Thirst.Managers;

namespace Thirst
{
    internal class ThirstSaveDefiner : SaveableTypeDefiner
    {

        public ThirstSaveDefiner() : base(685372151)
        {

        }

        protected override void DefineClassTypes()
        {
            this.AddClassDefinition(typeof(ThirstManager), 1);
            this.AddStructDefinition(typeof(ThirstData), 2);
        }

        protected override void DefineContainerDefinitions()
        {
            ConstructContainerDefinition(typeof(Dictionary<MobileParty, ThirstData>));
        }

    }
}
