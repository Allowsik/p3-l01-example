namespace ImageProcessing;

public interface IColorTransform
{
    public RGB Transform(RGB color);
}