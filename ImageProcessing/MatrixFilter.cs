namespace ImageProcessing;

public abstract class MatrixFilter : IImageFilter
{
    private readonly float[,] _kernel;
    
    protected MatrixFilter(float[,] kernel)
    {
        // Maska musi być: niepusta, kwadratowa, nieparzysty wymiar
        //check if empty
        if (kernel is null) 
            throw new ArgumentNullException(nameof(kernel));
        if (kernel.GetLength(0) != kernel.GetLength(1)) 
            throw new ArgumentException("Macierz prostokątna");
        if (kernel.GetLength(0) % 2 == 0)
            throw new ArgumentException("Macierz parzysty wymiar");
        _kernel = kernel;
    }

    public Image Apply(Image source)
    {
        if (source is null) 
            throw new ArgumentNullException(nameof(source));

        Image result = source.CreateEmpty(source.Width, source.Height);

        int size = _kernel.GetLength(0);
        int radius = size / 2;

        // pixel by pixel
        for (int y = 0; y < source.Height; y++)
        {
            for (int x = 0; x < source.Width; x++)
            {
                RGB sum = new RGB(0,0,0);
                //Mamy pixel x,y teraz dwie petle przechodza po sasiadach
                for (int ky = 0; ky < _kernel.GetLength(0); ky++)
                {
                    for (int kx = 0; kx < _kernel.GetLength(0); kx++)
                    {
                        int sourceX = x + kx - radius;
                        int sourceY = y + ky - radius;
                        
                        // Poza obrazem biezemy najblizszy brzeg
                        sourceX = Math.Clamp(sourceX, 0, source.Width - 1);
                        sourceY = Math.Clamp(sourceY, 0, source.Height - 1);

                        RGB neighbor = source[sourceX, sourceY];
                        float weight = _kernel[ky, kx];

                        sum += neighbor * weight;
                    }
                }
                result[x, y] = sum.Clamp();
            }
        }
        
        return result;

    }
}