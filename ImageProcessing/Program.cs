#define STAGE1
// Odkomentuj kolejny etap po zaimplementowaniu jego klas.
#define STAGE2
#define STAGE3
#define STAGE4

// Włączenie wyższego etapu automatycznie włącza wcześniejsze.
#if STAGE4
#define STAGE3
#endif
#if STAGE3
#define STAGE2
#endif
#if STAGE2
#define STAGE1
#endif

using System;
using System.Text;

namespace ImageProcessing
{
    // Tester do autorskiego zadania „Obrazy i filtry”.
    // Dodaj do projektu konsolowego razem ze swoimi klasami i uruchom: dotnet run
    // Nie wymaga bibliotek testowych. Nie zawiera rozwiązań zadania.
    // Testy sprawdzają zachowanie; widoczność składowych i organizację kodu
    // należy dodatkowo porównać z poleceniem. Liczba testów nie oznacza punktacji.
    internal static class Program
    {
        private static int passed;
        private static int failed;

        private static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("P3 — obrazy i filtry: testy do próbnego laboratorium");

#if STAGE1
            Stage1();
#endif
#if STAGE2
            Stage2();
#endif
#if STAGE3
            Stage3();
#endif
#if STAGE4
            Stage4();
#endif

            Console.WriteLine($"\nWynik: {passed} OK, {failed} błędów.");
            Environment.ExitCode = failed == 0 ? 0 : 1;
        }

        private static void Test(string name, Action action)
        {
            try
            {
                action();
                passed++;
                Console.WriteLine($"[OK]   {name}");
            }
            catch (Exception e)
            {
                failed++;
                Console.WriteLine($"[BŁĄD] {name}: {e.GetType().Name}: {e.Message}");
            }
        }

        private static void Check(bool condition, string message)
        {
            if (!condition) throw new Exception(message);
        }

        private static void Throws<T>(Action action) where T : Exception
        {
            try { action(); }
            catch (T) { return; }
            // Inny typ wyjątku przejdzie do Test() i zostanie oznaczony jako błąd.
            throw new Exception($"Oczekiwano wyjątku {typeof(T).Name}, ale go nie zgłoszono.");
        }

#if STAGE1
        private static readonly Func<int, int, Image>[] Factories =
        {
            (w, h) => new ArrayImage(w, h),
            (w, h) => new FlatImage(w, h)
        };

        private static void Color(RGB actual, float r, float g, float b)
        {
            const float epsilon = 0.00001f;
            // Ta postać warunku odrzuca także NaN i nieskończoności.
            Check(Math.Abs(actual.R - r) <= epsilon &&
                  Math.Abs(actual.G - g) <= epsilon &&
                  Math.Abs(actual.B - b) <= epsilon,
                $"Oczekiwano ({r:G6}, {g:G6}, {b:G6}), " +
                $"otrzymano ({actual.R:G6}, {actual.G:G6}, {actual.B:G6}).");
        }

        private static void SamePixels(Image actual, Image expected)
        {
            Check(actual.Width == expected.Width && actual.Height == expected.Height,
                "Niepoprawne wymiary obrazu.");
            for (int y = 0; y < expected.Height; y++)
                for (int x = 0; x < expected.Width; x++)
                {
                    RGB c = expected[x, y];
                    try { Color(actual[x, y], c.R, c.G, c.B); }
                    catch (Exception e) { throw new Exception($"Piksel [{x}, {y}]: {e.Message}"); }
                }
        }

        private static void NewResult(Image result, Image source)
        {
            Check(!ReferenceEquals(result, source), "Wynik jest tym samym obiektem co źródło.");
            Check(result.GetType() == source.GetType(), "Zmieniono reprezentację obrazu.");
            Check(result.Width == source.Width && result.Height == source.Height,
                "Zmieniono wymiary obrazu.");
        }

        // Niesymetryczny obraz pozwala wykryć zamianę x i y.
        private static Image Pattern(Func<int, int, Image> create)
        {
            Image image = create(4, 3);
            for (int y = 0; y < image.Height; y++)
                for (int x = 0; x < image.Width; x++)
                    image[x, y] = new RGB(x / 4f, y / 3f, (x + y) / 6f);
            return image;
        }

        private static void Stage1()
        {
            Console.WriteLine("\nETAP 1 — RGB i reprezentacje obrazu");
            Test("RGB: konstruktor, właściwości i domyślna czerń", () =>
            {
                Color(default(RGB), 0, 0, 0);
                RGB c = new RGB(0.2f, 0.3f, 0.4f);
                Color(c, 0.2f, 0.3f, 0.4f);
                c.R = 0.8f; c.G = 0.7f; c.B = 0.6f;
                Color(c, 0.8f, 0.7f, 0.6f);
                RGB copy = c;
                copy.R = 0;
                Color(c, 0.8f, 0.7f, 0.6f);
            });
            Test("RGB: operatory nie ograniczają wartości", () =>
            {
                Color(new RGB(0.8f, 0.2f, -0.5f) + new RGB(0.7f, 0.3f, 0.2f),
                    1.5f, 0.5f, -0.3f);
                Color(new RGB(0.8f, 0.2f, -0.5f) * 2f, 1.6f, 0.4f, -1f);
                Color(new RGB(2, -1, 0.5f), 2, -1, 0.5f);
            });
            Test("RGB: Clamp zwraca ograniczoną kopię", () =>
            {
                RGB c = new RGB(-0.2f, 0.4f, 1.7f);
                Color(c.Clamp(), 0, 0.4f, 1);
                Color(c, -0.2f, 0.4f, 1.7f);
            });

            foreach (var create in Factories)
            {
                string name = create == Factories[0] ? "ArrayImage" : "FlatImage";
                Test(name + ": wymiary i początkowa czerń", () =>
                {
                    Image image = create(4, 3);
                    Check(image.Width == 4 && image.Height == 3, "Błędne wymiary.");
                    for (int y = 0; y < 3; y++)
                        for (int x = 0; x < 4; x++) Color(image[x, y], 0, 0, 0);
                });
                Test(name + ": niezależne piksele i współrzędne", () =>
                {
                    Image image = Pattern(create);
                    for (int y = 0; y < 3; y++)
                        for (int x = 0; x < 4; x++)
                            Color(image[x, y], x / 4f, y / 3f, (x + y) / 6f);
                    RGB copy = image[2, 1];
                    copy.R = 1;
                    Color(image[2, 1], 0.5f, 1f / 3, 0.5f);
                });
                Test(name + ": walidacja wymiarów", () =>
                {
                    Throws<ArgumentOutOfRangeException>(() => create(0, 2));
                    Throws<ArgumentOutOfRangeException>(() => create(2, 0));
                    Throws<ArgumentOutOfRangeException>(() => create(-1, 2));
                    Throws<ArgumentOutOfRangeException>(() => create(2, -1));
                });
                Test(name + ": granice indeksatora get i set", () =>
                {
                    Image image = create(4, 3);
                    int[,] invalid = { { -1, 1 }, { 1, -1 }, { 4, 0 }, { 0, 3 }, { -1, 1 }, { 4, 1 } };
                    for (int i = 0; i < invalid.GetLength(0); i++)
                    {
                        int x = invalid[i, 0], y = invalid[i, 1];
                        Throws<ArgumentOutOfRangeException>(() => { RGB ignored = image[x, y]; });
                        Throws<ArgumentOutOfRangeException>(() => image[x, y] = new RGB(1, 1, 1));
                    }
                });
                Test(name + ": CreateEmpty — typ, nowy rozmiar, czerń, niezależność", () =>
                {
                    Image source = Pattern(create);
                    Image empty = source.CreateEmpty(2, 5);
                    Check(empty.GetType() == source.GetType(), "Błędny typ CreateEmpty.");
                    Check(empty.Width == 2 && empty.Height == 5, "Zignorowano żądane wymiary.");
                    for (int y = 0; y < 5; y++)
                        for (int x = 0; x < 2; x++) Color(empty[x, y], 0, 0, 0);
                    empty[0, 0] = new RGB(1, 1, 1);
                    Color(source[0, 0], 0, 0, 0);
                    Throws<ArgumentOutOfRangeException>(() => source.CreateEmpty(0, 1));
                });
            }
            Test("Obie reprezentacje zwracają te same piksele", () =>
                SamePixels(Pattern(Factories[0]), Pattern(Factories[1])));
        }
#endif

#if STAGE2
        private sealed class ProbeTransform : IColorTransform
        {
            public int Calls { get; private set; }
            public RGB Transform(RGB color)
            {
                Calls++;
                return new RGB(color.G + 0.2f, color.B * 2, color.R - 0.3f);
            }
        }

        private static void Stage2()
        {
            Console.WriteLine("\nETAP 2 — filtry funkcyjne");
            Test("Przekształcenia: negatyw i ważona skala szarości", () =>
            {
                IColorTransform negative = new NegativeTransform();
                IColorTransform gray = new GrayscaleTransform();
                Color(negative.Transform(new RGB(1, 0, 0)), 0, 1, 1);
                Color(negative.Transform(new RGB(0.2f, 0.4f, 0.8f)), 0.8f, 0.6f, 0.2f);
                Color(gray.Transform(new RGB(1, 0, 0)), 0.299f, 0.299f, 0.299f);
                Color(gray.Transform(new RGB(0, 1, 0)), 0.587f, 0.587f, 0.587f);
                Color(gray.Transform(new RGB(0, 0, 1)), 0.114f, 0.114f, 0.114f);
            });
            Test("FunctionFilter: null w konstruktorze i Apply", () =>
            {
                Throws<ArgumentNullException>(() => new FunctionFilter(null!));
                Throws<ArgumentNullException>(() => new FunctionFilter(new NegativeTransform()).Apply(null!));
            });
            foreach (var create in Factories)
            {
                string name = create == Factories[0] ? "ArrayImage" : "FlatImage";
                Test(name + ": dowolne przekształcenie, każdy piksel, Clamp", () =>
                {
                    Image source = Pattern(create);
                    ProbeTransform transform = new ProbeTransform();
                    IImageFilter filter = new FunctionFilter(transform);
                    Image result = filter.Apply(source);
                    NewResult(result, source);
                    Check(transform.Calls == 12, "Transform powinno być wywołane raz na piksel.");
                    for (int y = 0; y < 3; y++)
                        for (int x = 0; x < 4; x++)
                            Color(result[x, y], Math.Clamp(y / 3f + 0.2f, 0, 1),
                                Math.Clamp((x + y) / 3f, 0, 1), Math.Clamp(x / 4f - 0.3f, 0, 1));
                    SamePixels(source, Pattern(create));
                    result[0, 0] = new RGB(1, 1, 1);
                    Color(source[0, 0], 0, 0, 0);
                });
                Test(name + ": dwa negatywy odtwarzają obraz", () =>
                {
                    Image source = Pattern(create);
                    IImageFilter filter = new FunctionFilter(new NegativeTransform());
                    SamePixels(filter.Apply(filter.Apply(source)), source);
                });
            }
        }
#endif

#if STAGE3
        // Wyłącznie adapter do testowania chronionego konstruktora klasy bazowej.
        private sealed class ProbeMatrixFilter : MatrixFilter
        {
            public ProbeMatrixFilter(float[,] kernel) : base(kernel) { }
        }

        private static void Stage3()
        {
            Console.WriteLine("\nETAP 3 — filtry macierzowe");
            Test("MatrixFilter: walidacja maski i źródła", () =>
            {
                Throws<ArgumentNullException>(() => new ProbeMatrixFilter(null!));
                Throws<ArgumentException>(() => new ProbeMatrixFilter(new float[0, 0]));
                Throws<ArgumentException>(() => new ProbeMatrixFilter(new float[2, 2]));
                Throws<ArgumentException>(() => new ProbeMatrixFilter(new float[3, 5]));
                Throws<ArgumentNullException>(() => new BlurFilter().Apply(null!));
            });
            foreach (var create in Factories)
            {
                string name = create == Factories[0] ? "ArrayImage" : "FlatImage";
                Test(name + ": maska 1x1, brak automatycznej normalizacji", () =>
                {
                    Image source = create(1, 1);
                    source[0, 0] = new RGB(0.2f, 0.3f, 0.4f);
                    Image result = new ProbeMatrixFilter(new float[,] { { 2 } }).Apply(source);
                    NewResult(result, source);
                    Color(result[0, 0], 0.4f, 0.6f, 0.8f);
                });
                Test(name + ": maska asymetryczna — kierunek osi i brzeg", () =>
                {
                    Image source = Pattern(create);
                    // Jedynka w prawym sąsiedzie: bez obracania ani transpozycji maski.
                    float[,] right = { { 0, 0, 0 }, { 0, 0, 1 }, { 0, 0, 0 } };
                    Image result = new ProbeMatrixFilter(right).Apply(source);
                    NewResult(result, source);
                    for (int y = 0; y < 3; y++)
                        for (int x = 0; x < 4; x++)
                        {
                            RGB c = source[Math.Min(x + 1, 3), y];
                            Color(result[x, y], c.R, c.G, c.B);
                        }
                    SamePixels(source, Pattern(create));
                });
                Test(name + ": maska 5x5 — obsługa dowolnego nieparzystego rozmiaru", () =>
                {
                    float[,] mask = new float[5, 5];
                    mask[0, 2] = 1; // Sąsiad o dwa wiersze wyżej.
                    Image source = Pattern(create);
                    Image result = new ProbeMatrixFilter(mask).Apply(source);
                    for (int y = 0; y < 3; y++)
                        for (int x = 0; x < 4; x++)
                        {
                            RGB c = source[x, Math.Max(y - 2, 0)];
                            Color(result[x, y], c.R, c.G, c.B);
                        }
                });
                Test(name + ": rozmycie 2x1 — powielanie brzegu i osobny wynik", () =>
                {
                    Image source = create(2, 1);
                    source[0, 0] = new RGB(1, 0, 0);
                    Image result = new BlurFilter().Apply(source);
                    NewResult(result, source);
                    Color(result[0, 0], 2f / 3, 0, 0);
                    Color(result[1, 0], 1f / 3, 0, 0);
                    Color(source[0, 0], 1, 0, 0);
                    Color(source[1, 0], 0, 0, 0);
                    result[0, 0] = default(RGB);
                    Color(source[0, 0], 1, 0, 0);
                });
                Test(name + ": Blur i Sharpen zachowują stały kolor (także 1x1)", () =>
                {
                    foreach (int width in new[] { 1, 4 })
                    {
                        Image source = create(width, 1);
                        for (int x = 0; x < width; x++) source[x, 0] = new RGB(0.2f, 0.4f, 0.6f);
                        SamePixels(new BlurFilter().Apply(source), source);
                        SamePixels(new SharpenFilter().Apply(source), source);
                    }
                });
                Test(name + ": Sharpen — ujemne wagi i Clamp dopiero po sumie", () =>
                {
                    Image source = create(3, 3);
                    for (int y = 0; y < 3; y++)
                        for (int x = 0; x < 3; x++) source[x, y] = new RGB(0.4f, 0.4f, 0.4f);
                    source[1, 1] = new RGB(0.5f, 0.1f, 1);
                    Image result = new SharpenFilter().Apply(source);
                    NewResult(result, source);
                    // 5 * środek - czterech sąsiadów, potem Clamp.
                    Color(result[1, 1], 0.9f, 0, 1);
                    Color(source[1, 1], 0.5f, 0.1f, 1);
                });
            }
        }
#endif

#if STAGE4
        private static void Stage4()
        {
            Console.WriteLine("\nETAP 4 — kopie i sekwencje filtrów");
            foreach (var create in Factories)
            {
                string name = create == Factories[0] ? "ArrayImage" : "FlatImage";
                Test(name + ": niezależna kopia", () =>
                {
                    Image source = Pattern(create);
                    Image copy = source.Copy();
                    NewResult(copy, source);
                    SamePixels(copy, source);
                    copy[0, 0] = new RGB(1, 1, 1);
                    Color(source[0, 0], 0, 0, 0);
                    source[1, 0] = new RGB(1, 1, 1);
                    Color(copy[1, 0], 0.25f, 0, 1f / 6);
                });
                Test(name + ": pusta sekwencja zwraca kopię", () =>
                {
                    Image source = Pattern(create);
                    Image result = source.ApplyFilters();
                    NewResult(result, source);
                    SamePixels(result, source);
                    result[0, 0] = new RGB(1, 1, 1);
                    Color(source[0, 0], 0, 0, 0);
                });
                Test(name + ": kolejność filtrów i brak zmiany źródła", () =>
                {
                    Image source = create(1, 1);
                    source[0, 0] = new RGB(0.2f, 0.3f, 0.4f);
                    // ProbeTransform i negatyw nie są przemienne.
                    Image result = source.ApplyFilters(
                        new FunctionFilter(new ProbeTransform()),
                        new FunctionFilter(new NegativeTransform()));
                    NewResult(result, source);
                    Color(result[0, 0], 0.5f, 0.2f, 1);
                    Color(source[0, 0], 0.2f, 0.3f, 0.4f);
                });
                Test(name + ": wspólna sekwencja filtrów funkcyjnych i macierzowych", () =>
                {
                    Image source = Pattern(create);
                    IImageFilter negative = new FunctionFilter(new NegativeTransform());
                    IImageFilter blur = new BlurFilter();
                    IImageFilter sharpen = new SharpenFilter();
                    Image expected = sharpen.Apply(blur.Apply(negative.Apply(source)));
                    Image result = source.ApplyFilters(negative, blur, sharpen);
                    NewResult(result, source);
                    SamePixels(result, expected);
                    SamePixels(source, Pattern(create));
                });
                Test(name + ": null w sekwencji filtrów", () =>
                {
                    Image source = Pattern(create);
                    Throws<ArgumentNullException>(() => source.ApplyFilters((IImageFilter[])null!));
                    Throws<ArgumentNullException>(() => source.ApplyFilters(
                        new FunctionFilter(new NegativeTransform()), null!));
                    SamePixels(source, Pattern(create));
                });
            }
        }
#endif
    }
}
