namespace ImageProcessing;

public class ArrayImage : Image
{
    private RGB[,] array;
    
    public ArrayImage(int width, int height) : base(width, height)
    {
        if (width <= 0 || height <= 0) throw new ArgumentOutOfRangeException();
        array = new RGB[width, height];
    }

    public override RGB this[int x, int y]
    {
        get
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height) throw new ArgumentOutOfRangeException();
            return array[x, y];
        }
        set
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height) throw new ArgumentOutOfRangeException();
            array[x, y] = value;
        }
    }
    public override Image CreateEmpty(int width, int height)
    {
        return new ArrayImage(width, height);
    }
}