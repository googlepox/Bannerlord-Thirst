using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;
using System.Collections.Generic;
using System.Xml;

namespace BannerKings.UI.Extensions
{
    [PrefabExtension("MapBar", "descendant::HintWidget[@DataSource='{DailyConsumptionHint}']", "MapBar")]
    [System.Obsolete]
    internal class ThirstMapBarExtension : PrefabExtensionInsertPatch
    {
        private readonly List<XmlNode> _nodes;

        public override InsertType Type => InsertType.Append;
        public ThirstMapBarExtension()
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml("<HintWidget DataSource=\"{ThirstHint}\" WidthSizePolicy=\"CoverChildren\" HeightSizePolicy=\"CoverChildren\" VerticalAlignment=\"Center\" Command.HoverBegin=\"ExecuteBeginHint\" Command.HoverEnd=\"ExecuteEndHint\"><Children><ListPanel WidthSizePolicy=\"CoverChildren\" HeightSizePolicy=\"CoverChildren\" IsEnabled=\"false\"><Children><Widget WidthSizePolicy=\"Fixed\" HeightSizePolicy=\"Fixed\" SuggestedWidth=\"18\" SuggestedHeight=\"23\" VerticalAlignment=\"Center\"><Children><Widget WidthSizePolicy=\"Fixed\" HeightSizePolicy=\"Fixed\" SuggestedWidth=\"18\" SuggestedHeight=\"23\" HorizontalAlignment=\"Center\" VerticalAlignment=\"Center\"  Sprite=\"water_drop\" /></Children></Widget><MapBarTextWidget DataSource=\"{..}\" WidthSizePolicy=\"CoverChildren\" HeightSizePolicy=\"Fixed\" MinWidth=\"20\" MaxWidth=\"55\" SuggestedHeight=\"50\" VerticalAlignment=\"Center\" PositionYOffset=\"2\" MarginLeft=\"8\" MarginRight=\"4\" Brush=\"MapTextBrushWithAnim\" Brush.FontSize=\"20\" IsWarning=\"@IsDailyConsumptionTooltipWarningWater\" NormalColor=\"!NormalMapBarTextColor\" Text=\"@WaterWithAbbrText\" ValueAsInt=\"@Water\" WarningColor=\"!WarningMapBarTextColor\" /></Children></ListPanel></Children></HintWidget>");
            this._nodes = new List<XmlNode>()
      {
        (XmlNode) xmlDocument
      };
        }

        public override int Index => 1;

        [PrefabExtensionInsertPatch.PrefabExtensionXmlNodes]
        public IEnumerable<XmlNode> Nodes => (IEnumerable<XmlNode>)this._nodes;
    }
}
