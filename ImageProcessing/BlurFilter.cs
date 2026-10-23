namespace ImageProcessing;

public class BlurFilter : MatrixFilter
{
    public BlurFilter() : base(new float[,]
    {
        { 1f / 9f, 1f / 9f, 1f / 9f },
        { 1f / 9f, 1f / 9f, 1f / 9f },
        { 1f / 9f, 1f / 9f, 1f / 9f }
    }) {}
}