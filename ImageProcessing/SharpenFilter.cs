namespace ImageProcessing;

public class SharpenFilter : MatrixFilter
{
    public SharpenFilter() : base(new float[,]
    {
        { 0, -1, 0},
        {-1, 5, -1},
        {0, -1, 0}
    }) {}
}