namespace ImageProcessing;

public struct RGB
{
    public float R { get; set; } = 0;
    public float G { get; set; } = 0;
    public float B { get; set; } = 0;

    public RGB(float r, float g, float b)
    {
        R = r;
        G = g;
        B = b;
    }

    public static RGB operator +(RGB a, RGB b)
    {
        return new RGB(a.R + b.R, a.G + b.G, a.B + b.B);
    }

    public static RGB operator *(RGB a, float b)
    {
        return new RGB(a.R * b, a.G * b, a.B * b);
    }
    
    public static RGB operator *(float b, RGB a)
    {    
        return new RGB(a.R * b, a.G * b, a.B * b);
    }

    public RGB Clamp()
    {
        return new RGB(Math.Clamp(R, 0, 1), Math.Clamp(G, 0, 1), Math.Clamp(B, 0, 1));
    }


}