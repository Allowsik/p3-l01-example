namespace ImageProcessing;

public interface IImageFilter
{
    public Image Apply(Image source);
}