using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;
using System.Collections.Generic;

namespace Thirst.UI
{
    [PrefabExtension("MapBar", "descendant::HintWidget[@DataSource='{DailyConsumptionHint}']/Children/ListPanel/Children/MapBarTextWidget")]
    internal class FoodWidgetFix : PrefabExtensionSetAttributePatch
    {
        public override List<Attribute> Attributes => new List<Attribute> {
            new Attribute( "MinWidth", "20" )
        };
    }
}