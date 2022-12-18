using TaleWorlds.SaveSystem;
using Thirst.Managers;
using Thirst.Models;

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
            this.AddClassDefinition(typeof(ThirstManager), 2);
            this.AddClassDefinition(typeof(PartyWaterConsumptionModel), 3);
        }

    }
}
