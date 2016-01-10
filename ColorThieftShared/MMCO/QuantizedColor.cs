namespace ColorThief.MMCO
{
    public class QuantizedColor
    {
        public QuantizedColor(Color color, int population)
        {
            Color = color;
            Population = population;
            IsDark = ColorUtility.CalculateYiqLuma(color) < 128;
        }

        public Color Color { get; }
        public int Population { get; }
        public bool IsDark { get; }
    }
}