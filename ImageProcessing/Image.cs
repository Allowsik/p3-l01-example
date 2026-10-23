namespace ImageProcessing;

public abstract class Image
{
    public int Width { get; } = 0;
    public int Height { get; } = 0;
    
    protected Image(int width, int height)
    {
        if (height <= 0 || width <= 0) throw new ArgumentOutOfRangeException();
        Height = height;
        Width = width;
    }

    public abstract RGB this[int x, int y] { get; set; }

    public abstract Image CreateEmpty(int width, int height);

    public Image Copy()
    {
        Image result = CreateEmpty(Width, Height);
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                result[x,y] = this[x,y];
            }
        }
        return result;
    }

    public Image ApplyFilters(params IImageFilter[] filters)
    {
        if (filters is null) throw new ArgumentNullException(nameof(filters));
        Image result = Copy();
        foreach (var imageFilter in filters)
        {
            if (imageFilter is null) throw new ArgumentNullException(nameof(imageFilter));
            result = imageFilter.Apply(result);
        }

        return result;
    }
}