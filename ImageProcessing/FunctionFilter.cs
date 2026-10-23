namespace ImageProcessing;

public class FunctionFilter : IImageFilter
{
    private IColorTransform ColorTransform { get; }
    
    public FunctionFilter(IColorTransform transform)
    {
        if (transform == null) throw new ArgumentNullException(nameof(transform));
        ColorTransform = transform;
    }

    public Image Apply(Image source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        Image result = source.CreateEmpty(source.Width, source.Height);
        source.CreateEmpty(source.Width, source.Height);
        for (int y = 0; y < source.Height; y++)
        {
            for (int x = 0; x < source.Width; x++)
            {
                RGB original = source[x, y];
                RGB transformed = ColorTransform.Transform(original);

                result[x, y] = transformed.Clamp();
            }
        }
        return result;
    }
}