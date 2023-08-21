using TaleWorlds.SaveSystem;

namespace Thirst
{
    public struct ThirstData
    {
        [SaveableProperty(1)]
        public float RemainingWaterPercentage
        {
            get; set;
        }

        [SaveableProperty(2)]
        public bool IsDehydrated
        {
            get; set;
        }

        public ThirstData(float remainingWaterPercentage, bool isDehydrated)
        {
            this.RemainingWaterPercentage = remainingWaterPercentage;
            this.IsDehydrated = isDehydrated;
        }
    }
}
