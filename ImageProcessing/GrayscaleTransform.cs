namespace ImageProcessing;

public class GrayscaleTransform : IColorTransform
{
    public RGB Transform(RGB color)
    {
        float L = (float)(0.299f * color.R + 0.587 * color.G + 0.114f * color.B);
        return new RGB(L, L, L);
    }
}