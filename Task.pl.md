# Laboratorium próbne — Obrazy i filtry

## Wprowadzenie

Zadaniem na dzisiejszych laboratoriach jest stworzenie prostej biblioteki odpowiedzialnej za przechowywanie obrazów i nakładanie na nie filtrów.

Obraz jest prostokątną tablicą pikseli. Każdy piksel ma trzy całkowitoliczbowe kanały koloru: czerwony, zielony i niebieski. W zadaniu nie korzystamy z plików graficznych ani okna do wyświetlania obrazu. Wyniki działania biblioteki będą wypisywane w konsoli przez dostarczony program.

Czas wykonania zadania: 90 minut. Łączna punktacja: 12 pkt.

### Uwaga

W celu przechodzenia do następnych etapów należy odkomentowywać poszczególne linijki typu `#define STAGE0X` znajdujące się na początku pliku `Program.cs`. Inna ingerencja w ten plik jest zakazana.

## Kod początkowy

> **Images**
>
> - [Images.csproj](Images/Images.csproj)
> - [Program.cs](Images/Program.cs)
> - Imaging/
>   - [Color.cs](Images/Imaging/Color.cs)
>   - [Image.cs](Images/Imaging/Image.cs)
>   - [ArrayImage.cs](Images/Imaging/ArrayImage.cs)
>   - [FlatImage.cs](Images/Imaging/FlatImage.cs)
> - Filters/
>   - [IFunctionalFilter.cs](Images/Filters/IFunctionalFilter.cs)
>   - [FunctionalFilter.cs](Images/Filters/FunctionalFilter.cs)
>   - [IMatrixFilter.cs](Images/Filters/IMatrixFilter.cs)
>   - [MatrixFilter.cs](Images/Filters/MatrixFilter.cs)
>
> [output.txt](output.txt)
>
> [Download as zip](../ObrazyFiltry90_Starter.zip)

Kod początkowy zawiera deklaracje typów do uzupełnienia. Typy związane z obrazem znajdują się w przestrzeni nazw `Images.Imaging`, a typy filtrów w `Images.Filters`. Należy zachować dostarczone nazwy i sygnatury. Można dodawać pola i metody pomocnicze.

## Etap 1 (4 pkt)

W folderze `Imaging` zaimplementuj cztery typy w osobnych plikach: `Color.cs`, `Image.cs`, `ArrayImage.cs` i `FlatImage.cs`.

Uzupełnij publiczną strukturę `Color`, która będzie posiadała:

- konstruktor przyjmujący wartości kanałów czerwonego, zielonego i niebieskiego jako `int`: `Color(int r, int g, int b)`;
- publiczne właściwości tylko do odczytu `R`, `G` i `B`, zwracające wartości poszczególnych kanałów.

Struktura powinna być niemutowalna i zadeklarowana jako `readonly`. Każdy kanał musi należeć do przedziału od 0 do 255 włącznie. W razie niepoprawnej wartości konstruktor powinien generować wyjątek typu `ArgumentOutOfRangeException`. Domyślna wartość struktury, `default(Color)`, powinna reprezentować kolor czarny, czyli `(0,0,0)`.

W tym samym folderze uzupełnij abstrakcyjną klasę `Image`, która będzie posiadała:

- chroniony konstruktor przyjmujący liczbę wierszy i kolumn obrazu jako `int`;
- publiczną właściwość tylko do odczytu `Rows`, zwracającą liczbę wierszy;
- publiczną właściwość tylko do odczytu `Columns`, zwracającą liczbę kolumn;
- abstrakcyjny dwuwymiarowy indeksator przyjmujący numer wiersza i kolumny, umożliwiający odczyt i zapis wartości typu `Color`.

Liczba wierszy i kolumn musi być dodatnia. W razie niepoprawności konstruktor powinien generować wyjątek typu `ArgumentOutOfRangeException`.

Uzupełnij następujące klasy dziedziczące po `Image`: `ArrayImage` i `FlatImage`.

`ArrayImage` powinna przechowywać piksele w prywatnej tablicy prostokątnej `Color[,]`.

`FlatImage` powinna przechowywać piksele w prywatnej tablicy jednowymiarowej `Color[]`, kolejno wierszami. Nie należy wykorzystywać w niej tablicy prostokątnej ani obiektu `ArrayImage` do przechowywania pikseli.

Obie klasy powinny posiadać publiczny konstruktor przyjmujący liczbę wierszy i kolumn. Nowo utworzone obrazy powinny być wypełnione kolorem czarnym. Każdy obraz powinien przechowywać własne piksele, niezależnie od innych obrazów. Obie implementacje powinny umożliwiać pracę przez zmienną typu `Image`.

Indeksator powinien korzystać z kolejności `[wiersz, kolumna]`. Indeksy zaczynają się od zera. Nieprawidłowy wiersz lub kolumna, zarówno przy odczycie, jak i zapisie, powinny powodować wyjątek typu `ArgumentOutOfRangeException`. Zapis jednego piksela nie może zmieniać pozostałych pikseli. Należy sprawdzać zakres obu współrzędnych: dla obrazu o 2 wierszach i 3 kolumnach indeks `[0,3]` jest niepoprawny.

### Wskazówki

- Microsoft Learn: Properties
- Microsoft Learn: Abstract and Sealed Classes and Class Members
- Microsoft Learn: Indexers
- Microsoft Learn: Create and throw exceptions

### Punktacja

1 pkt za strukturę `Color` z walidacją, 1 pkt za klasę bazową `Image` z abstrakcyjnym indeksatorem oraz po 1 pkt za każdą klasę pochodną z poprawnym przechowywaniem pikseli i walidacją indeksów.

## Etap 2 (2 pkt)

Do klasy `Image` dodaj chronioną abstrakcyjną metodę `Image CreateEmpty(int rows, int columns)`. Zaimplementuj ją w klasach pochodnych w taki sposób, aby każda zwracała nowy, czarny obraz swojej klasy o podanych wymiarach. Metoda nie powinna być dostępna spoza tej klasy i klas pochodnych.

W klasie `Image` uzupełnij następujące publiczne metody:

- `void Fill(Color color)`, wypełniającą cały bieżący obraz podanym kolorem;
- `Image Copy()`, zwracającą kopię bieżącego obrazu.

Metoda `Copy` powinna wykorzystać `CreateEmpty`. Kopia powinna mieć takie same wymiary, reprezentację i kolory pikseli jak źródło, ale własną pamięć pikseli. Zmiana kopii nie może zmieniać źródła i odwrotnie.

Obie metody powinny korzystać z indeksatora. Wspólna implementacja powinna znajdować się w klasie bazowej, bez rozpoznawania konkretnych typów obrazów.

### Punktacja

0,5 pkt za metodę `CreateEmpty`, 0,5 pkt za `Fill` oraz 1 pkt za `Copy` z zachowaniem reprezentacji i niezależności pamięci.

## Etap 3 (3 pkt)

W folderze `Filters` wykorzystaj interfejs `IFunctionalFilter`, który deklaruje metodę `Color Transform(Color color)`. Metoda przekształca pojedynczy kolor w nowy kolor.

Uzupełnij klasę `FunctionalFilter` implementującą ten interfejs. Klasa powinna posiadać:

- publiczny konstruktor `FunctionalFilter(Func<Color, Color> transform)`, przyjmujący funkcję przekształcającą kolor;
- publiczną metodę `Color Transform(Color color)`, zwracającą wynik wywołania zapamiętanej funkcji dla podanego argumentu.

Przekazanie `null` do konstruktora powinno powodować wyjątek typu `ArgumentNullException`. Filtr ma obsługiwać dowolną przekazaną funkcję. Przykładowo funkcja `c => new Color(255 - c.R, c.G, c.B)` odwraca kanał czerwony, pozostawiając pozostałe kanały bez zmian.

W klasie `Image` uzupełnij publiczną metodę `Image Apply(IFunctionalFilter filter)`. Powinna ona zwracać nowy obraz, którego każdy piksel powstaje przez wywołanie metody `Transform` dla odpowiadającego piksela źródłowego. Funkcję należy wywołać dokładnie raz dla każdego piksela. Przekazanie `null` zamiast filtra powinno powodować wyjątek typu `ArgumentNullException`.

W implementacji wykorzystaj metodę `CreateEmpty`. Zwrócony obraz powinien mieć takie same wymiary i taką samą reprezentację jak obraz źródłowy. Obraz źródłowy powinien pozostać niezmieniony, a jego piksele nie mogą być współdzielone z wynikiem.

Wspólną implementację umieść w klasie bazowej. Korzystaj z indeksatora i interfejsu, bez rozpoznawania konkretnych typów obrazów lub filtrów. Metoda powinna współpracować również z innymi implementacjami `IFunctionalFilter`. Wyjątki zgłoszone przez przekazaną funkcję mogą być przekazywane dalej.

### Wskazówki

- Microsoft Learn: Interfaces
- Microsoft Learn: Func<T,TResult> Delegate
- Microsoft Learn: Lambda expressions

### Punktacja

1 pkt za klasę `FunctionalFilter` i sprawdzanie argumentu. 1 pkt za zastosowanie filtra do wszystkich pikseli przez interfejs. 1 pkt za wykorzystanie `CreateEmpty`, zachowanie typu obrazu i niezależność wyniku od źródła.

## Etap 4 (3 pkt)

W folderze `Filters` wykorzystaj interfejs `IMatrixFilter`, który deklaruje indeksator tylko do odczytu `double this[int row, int column]`. Indeksator udostępnia współczynniki maski o wymiarach 3×3.

Uzupełnij klasę `MatrixFilter` implementującą ten interfejs. Klasa powinna posiadać publiczny konstruktor `MatrixFilter(double[,] kernel)`. Należy zapamiętać własną kopię tablicy współczynników, aby późniejsze zmiany przekazanej tablicy nie zmieniały filtra.

Konstruktor powinien generować wyjątek typu `ArgumentNullException` dla `null` oraz `ArgumentException`, jeżeli tablica nie ma wymiarów 3×3 lub zawiera współczynnik `NaN` albo nieskończony. Indeksator powinien przyjmować indeksy od 0 do 2 włącznie. W razie niepoprawnego indeksu powinien generować wyjątek typu `ArgumentOutOfRangeException`.

W klasie `Image` uzupełnij publiczną metodę `Image Apply(IMatrixFilter filter)`. Powinna ona nakładać maskę na sąsiedztwo każdego piksela. Metoda ma zwracać nowy obraz o takich samych wymiarach i takiej samej reprezentacji jak źródło, wykorzystując `CreateEmpty`. Źródło powinno pozostać niezmienione, a wynik powinien mieć własną pamięć pikseli. Dla `null` należy zgłosić `ArgumentNullException`. Implementacja powinna korzystać z interfejsu, bez rzutowania na `MatrixFilter`.

Algorytm:

> Dla piksela wynikowego `[r,c]` oblicz osobno kanały `R`, `G` i `B`.
>
> Dla każdego kanału zsumuj dziewięć iloczynów `filter[i,j]` i wartości tego kanału w pikselu źródłowym `[r+i-1,c+j-1]`, gdzie `i` oraz `j` przyjmują wartości 0, 1, 2.
>
> Jeżeli współrzędna sąsiada wypada poza obraz, ogranicz ją do najbliższej poprawnej wartości. Powoduje to powielenie najbliższego piksela brzegowego. Ta zasada obowiązuje również dla obrazów 1×1 i 1×N.
>
> Po obliczeniu całej sumy ogranicz ją do przedziału od 0 do 255, zaokrąglij do najbliższej liczby całkowitej za pomocą `MidpointRounding.AwayFromZero` i zapisz jako kanał nowego piksela.

Sumowanie wykonuj na wartościach typu `double`, zawsze korzystając z oryginalnego obrazu. Nie obracaj ani nie transponuj maski: współczynnik `[1,1]` odpowiada bieżącemu pikselowi, `[0,0]` lewemu górnemu sąsiadowi, a `[1,2]` prawemu sąsiadowi. Nie zaokrąglaj pojedynczych iloczynów i nie dziel automatycznie przez sumę współczynników.

Maska zawierająca dziewięć wartości `1.0/9` realizuje rozmycie. Maska z wartością 1 na środku i zerami w pozostałych miejscach tworzy kopię obrazu. Współczynniki ujemne oraz sumujące się do wartości innej niż 1, w tym do 0, są dozwolone. Dla współczynnika 0.5 na środku maski kolor `(1,3,5)` powinien dać wynik `(1,2,3)`.

Na potrzeby laboratorium można przyjąć, że poprawne rozmiary obrazów mieszczą się w pamięci, ich iloczyn mieści się w `int`, a obliczenia nie powodują przepełnienia `double`. Inne implementacje interfejsów spełniają opisane kontrakty. Testy nie wymagają określonej kolejności sprawdzania kilku jednocześnie błędnych argumentów. Typy wyjątków muszą być zgodne z poleceniem; ich komunikaty i `ParamName` nie są oceniane.

Wynik programu po włączeniu wszystkich etapów powinien być zgodny z plikiem `output.txt`. Wypisywanie obrazu jest już zaimplementowane w dostarczonym `Program.cs`.

### Punktacja

1 pkt za klasę `MatrixFilter`, kopię maski i sprawdzanie argumentów. 1 pkt za sumowanie kanałów z oryginalnych pikseli, poprawną orientację maski i niezależny wynik właściwego typu. 1 pkt za obsługę brzegów, ograniczenie kanałów i zaokrąglanie.
