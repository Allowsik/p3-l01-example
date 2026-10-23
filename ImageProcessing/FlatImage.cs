namespace ImageProcessing;

public class FlatImage : Image
{
    private RGB[] imageData;
    
    public FlatImage(int width, int height) : base(width, height)
    {
        if (width <= 0 || height <= 0) throw new ArgumentOutOfRangeException();
        imageData = new RGB[width * height];
    }

    public override RGB this[int x, int y]
    {
        get
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height) throw new ArgumentOutOfRangeException();
            return imageData[y * Width + x];
        }
        set
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height) throw new ArgumentOutOfRangeException();
            imageData[y * Width + x] = value;
        }
    }

    public override Image CreateEmpty(int width, int height)
    {
        return new FlatImage(width, height);
    }
    
}