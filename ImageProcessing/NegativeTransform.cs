namespace ImageProcessing;

public class NegativeTransform : IColorTransform
{
    public RGB Transform(RGB color)
    {
        return new RGB(1 - color.R, 1 - color.G, 1 - color.B);
    }
}