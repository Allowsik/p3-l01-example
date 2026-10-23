# Zadanie laboratoryjne — Podstawy C#: obrazy i filtry

**Autorska prognoza do ćwiczeń, a nie treść rzeczywistego laboratorium.**

**Czas:** 90 minut. **Suma punktów:** 12.

## Podstawa rekonstrukcji

Z przesłanego czatu wynika wskazówka o czterech plikach w pierwszym etapie i abstrakcyjnym indeksatorze dwuwymiarowym. Pierwsze zdjęcie zawiera podsumowanie o obrazie bazowym, dwóch reprezentacjach pikseli oraz filtrach funkcyjnych i macierzowych. Jest to relacja pośrednia, nie oryginalne polecenie. Drugie zdjęcie sugeruje około 12 krótkich plików rozwiązania. Nie traktujemy liczby linii dosłownie.

Przyjęta rekonstrukcja: cztery typy obrazu/koloru w etapie 1, pięć typów filtrów funkcyjnych w etapie 2, trzy typy filtrów macierzowych w etapie 3 i integracja w etapie 4. Daje to 12 plików implementacji, bez Program.cs. Nazwy, sygnatury, reprezentacje danych, konkretne filtry i punktacja poniżej są autorską prognozą.

Format nawiązuje do publicznych zadań P3: wprowadzenie, wymagania ogólne, etapy, sygnatury oraz punktacja. Materiał o raytracerze stanowi punkt wyjścia dla reprezentacji koloru, obrazu i dostępu przez indeksator. Filtry z tej prognozy są osobnym ćwiczeniem; publiczny etap 5 raytracera dotyczy Lambertiana i antyaliasingu.

Źródła formatu i zakresu:

- https://csharp.mini.pw.edu.pl/pl/labs/03.basics/
- https://csharp.mini.pw.edu.pl/pl/labs/03.basics/example1/
- https://csharp.mini.pw.edu.pl/pl/labs/lab03/

---

## Wprowadzenie

Zadaniem na dzisiejszych laboratoriach jest stworzenie prostej biblioteki do reprezentacji obrazów RGB oraz przetwarzania ich za pomocą filtrów. Obraz to prostokątna siatka pikseli. Każdy piksel zawiera trzy składowe: czerwoną, zieloną i niebieską.

Zaimplementujesz dwie reprezentacje obrazu udostępniające wspólny interfejs. Następnie przygotujesz filtry, których działanie nie będzie zależeć od sposobu przechowywania pikseli.

Filtr funkcyjny wyznacza nowy kolor wyłącznie z koloru pojedynczego piksela. Filtr macierzowy wykorzystuje również kolory sąsiednich pikseli oraz macierz współczynników, nazywaną maską filtra.

## Kod początkowy — założenie do symulacji

Przyjmij, że projekt konsolowy `ImageProcessing` zawiera `Program.cs` z przykładami uruchamianymi przez symbole `STAGE1`–`STAGE4`. W treningu możesz samodzielnie przygotować wywołania zgodne z przykładami poniżej. Ten dokument jest specyfikacją zadania, nie paczką gotowego kodu startowego.

W rzeczywistym laboratorium należy trzymać się nazw i sposobu uruchamiania podanych w otrzymanym projekcie.

## Wymagania ogólne

1. Wszystkie tworzone typy umieść w przestrzeni nazw `ImageProcessing`, każdy w osobnym pliku o nazwie zgodnej z typem.
2. Współrzędna `x` oznacza kolumnę, a `y` wiersz. Początek układu znajduje się w lewym górnym rogu.
3. Kanały koloru przechowuj jako `float`. Zakres wyświetlanego koloru to `[0, 1]`, ale obliczenia pośrednie mogą wykraczać poza ten zakres.
4. Filtry zwracają nowy obraz i nie modyfikują źródła. Wynik musi mieć ten sam rozmiar i tę samą konkretną reprezentację co źródło.
5. Algorytmy filtrów korzystają z klasy bazowej obrazu i jej indeksatora. Nie wolno uzależniać ich działania od konkretnej klasy obrazu przez rzutowanie lub rozpoznawanie typu.
6. Nie korzystaj z zewnętrznych bibliotek graficznych. Nie implementuj kamery, promieni ani materiałów.

## Etap 1: Reprezentacja koloru i obrazu (4 punkty)

W tym etapie utwórz dokładnie cztery pliki: `RGB.cs`, `Image.cs`, `ArrayImage.cs`, `FlatImage.cs`.

### 1.1. Struktura RGB (1 punkt)

Zdefiniuj publiczną strukturę `RGB`, która posiada:

- publiczne właściwości `float R`, `float G`, `float B`, umożliwiające odczyt i zapis;
- publiczny konstruktor `RGB(float r, float g, float b)`;
- operator dodawania dwóch kolorów, działający osobno na każdym kanale;
- operator mnożenia koloru przez skalar w postaci `RGB * float`;
- metodę `RGB Clamp()`, zwracającą nowy kolor z każdą składową ograniczoną do `[0, 1]`.

Konstruktor i operatory nie ograniczają kanałów automatycznie. Wartość domyślna struktury reprezentuje kolor czarny.

Przykład: suma `(0.2, 0.3, 0.4)` i `(0.1, 0.2, 0.3)` wynosi w przybliżeniu `(0.3, 0.5, 0.7)`.

### 1.2. Abstrakcyjna klasa Image (1 punkt)

Zdefiniuj publiczną klasę abstrakcyjną `Image`, która posiada:

- chroniony konstruktor `Image(int width, int height)`;
- publiczne właściwości tylko do odczytu `int Width` i `int Height`;
- publiczny abstrakcyjny indeksator `RGB this[int x, int y]`, umożliwiający odczyt i zapis piksela;
- publiczną abstrakcyjną metodę `Image CreateEmpty(int width, int height)`.

Konstruktor zgłasza `ArgumentOutOfRangeException`, gdy dowolny wymiar jest niedodatni. `CreateEmpty` ma zwracać nowy, czarny obraz tej samej konkretnej klasy co obiekt, na którym wywołano metodę.

Nie przechowuj tablicy pikseli w klasie bazowej. O sposobie przechowywania decydują klasy pochodne.

### 1.3. Klasa ArrayImage (1 punkt)

Zdefiniuj publiczną klasę `ArrayImage` dziedziczącą po `Image`:

- konstruktor `ArrayImage(int width, int height)` wywołuje konstruktor bazowy;
- piksele są przechowywane w prywatnej tablicy `RGB[,]` o wymiarach `[width, height]`;
- nadpisany indeksator odczytuje i zapisuje odpowiedni element;
- `CreateEmpty` tworzy nowy obiekt `ArrayImage` o żądanych wymiarach.

### 1.4. Klasa FlatImage (1 punkt)

Zdefiniuj publiczną klasę `FlatImage` dziedziczącą po `Image`:

- konstruktor `FlatImage(int width, int height)` wywołuje konstruktor bazowy;
- piksele są przechowywane w prywatnej jednowymiarowej tablicy `RGB[]`;
- wiersze obrazu są umieszczone kolejno, jeden za drugim;
- pozycja piksela w tablicy wynosi `y * Width + x`;
- `CreateEmpty` tworzy nowy obiekt `FlatImage`.

W obu klasach indeksator zgłasza `ArgumentOutOfRangeException`, gdy `x` lub `y` są poza obrazem. Waliduj obie współrzędne przed dostępem do tablicy, również w akcesorze `set`.

**Przykład kontrolny:** w obrazie szerokości 4 piksel `(2, 1)` ma indeks 6 w tablicy płaskiej. Współrzędna `(4, 0)` jest błędna — nie może zostać potraktowana jako `(0, 1)`.

**Punktacja:** po 1 punkcie za każdą poprawnie zaimplementowaną część 1.1–1.4.

## Etap 2: Filtry funkcyjne (3 punkty)

Utwórz pliki `IImageFilter.cs`, `IColorTransform.cs`, `FunctionFilter.cs`, `NegativeTransform.cs` i `GrayscaleTransform.cs`.

### 2.1. Interfejsy (0.5 punktu)

Zdefiniuj publiczny interfejs `IImageFilter` deklarujący metodę:

```csharp
Image Apply(Image source);
```

Zdefiniuj publiczny interfejs `IColorTransform` deklarujący metodę:

```csharp
RGB Transform(RGB color);
```

Pierwszy interfejs opisuje operację na całym obrazie. Drugi opisuje przekształcenie pojedynczego koloru.

### 2.2. Klasa FunctionFilter (1.5 punktu)

Zdefiniuj publiczną klasę `FunctionFilter` implementującą `IImageFilter`.

- Konstruktor `FunctionFilter(IColorTransform transform)` zapamiętuje przekształcenie w prywatnym polu tylko do odczytu.
- Dla argumentu `null` konstruktor zgłasza `ArgumentNullException`.
- Metoda `Apply` tworzy pusty obraz za pomocą `source.CreateEmpty(...)`.
- Dla każdego piksela wywołuje `Transform` na jego kolorze i zapisuje wynik po `Clamp()` do odpowiedniej pozycji nowego obrazu.
- Zwraca nowy obraz. Dla źródła `null` zgłasza `ArgumentNullException`.

Kod przechodzenia po pikselach powinien występować w `FunctionFilter`, a nie osobno w każdym przekształceniu.

### 2.3. Konkretne przekształcenia (1 punkt)

Zdefiniuj dwie publiczne klasy implementujące `IColorTransform`:

1. `NegativeTransform` — zwraca kolor `(1 − R, 1 − G, 1 − B)`.
2. `GrayscaleTransform` — oblicza jasność `L = 0.299f * R + 0.587f * G + 0.114f * B` i zwraca `(L, L, L)`.

Obie klasy mają być możliwe do utworzenia bez argumentów. Nie dodawaj do nich pętli przechodzących po obrazie.

**Przykładowe użycie:**

```csharp
IImageFilter negative = new FunctionFilter(new NegativeTransform());
Image result = negative.Apply(source);
```

**Punktacja:** interfejsy 0.5 pkt, ogólny filtr funkcyjny 1.5 pkt, po 0.5 pkt za każde konkretne przekształcenie.

## Etap 3: Filtry macierzowe (3 punkty)

Utwórz pliki `MatrixFilter.cs`, `BlurFilter.cs` i `SharpenFilter.cs`.

### 3.1. Abstrakcyjna klasa MatrixFilter (2 punkty)

Zdefiniuj publiczną klasę abstrakcyjną `MatrixFilter` implementującą `IImageFilter`.

Klasa ma chroniony konstruktor `MatrixFilter(float[,] kernel)`. Maska musi być niepusta i kwadratowa, a jej wymiar musi być nieparzysty. Dla `null` zgłoś `ArgumentNullException`, a dla błędnego kształtu `ArgumentException`. W tej symulacji wystarczy zapamiętać referencję do maski; nie wymagamy jej kopiowania.

Zaimplementuj publiczną metodę `Apply(Image source)`:

1. Utwórz pusty obraz tego samego typu i rozmiaru co źródło.
2. Dla każdego piksela `(x, y)` wyzeruj akumulator koloru.
3. Przejdź po wszystkich pozycjach maski i dodawaj odpowiednio ważone kolory sąsiadów do akumulatora.
4. Ogranicz kanały dopiero po zsumowaniu całej maski i zapisz wynik do nowego obrazu.
5. Zwróć nowy obraz. Dla źródła `null` zgłoś `ArgumentNullException`.

**Konwencja indeksowania:** pierwszy indeks maski oznacza wiersz, drugi kolumnę: `kernel[ky, kx]`. Dla maski rozmiaru `n` jej środek ma indeks `r = n / 2`. Waga `kernel[ky, kx]` odpowiada pikselowi źródłowemu:

```csharp
sourceX = x + kx - r;
sourceY = y + ky - r;
```

Jeżeli sąsiad wypada poza obrazem, użyj najbliższego piksela brzegowego: ogranicz `sourceX` do `[0, Width - 1]`, a `sourceY` do `[0, Height - 1]`.

W tej specyfikacji stosujemy maskę bez odwracania jej osi. Nie wykonuj automatycznego dzielenia przez sumę wag — właściwe współczynniki są już zapisane w masce.

### 3.2. Klasa BlurFilter (0.5 punktu)

Zdefiniuj publiczną klasę `BlurFilter` dziedziczącą po `MatrixFilter`. Jej bezparametrowy konstruktor przekazuje do konstruktora bazowego maskę 3 × 3, której wszystkie dziewięć współczynników wynosi `1f / 9f`.

Nie nadpisuj ponownie algorytmu `Apply`. Filtr oblicza średnią z dziewięciu próbek.

### 3.3. Klasa SharpenFilter (0.5 punktu)

Zdefiniuj publiczną klasę `SharpenFilter` dziedziczącą po `MatrixFilter`. Jej bezparametrowy konstruktor przekazuje do konstruktora bazowego maskę:

| | | |
|---:|---:|---:|
| 0 | -1 | 0 |
| -1 | 5 | -1 |
| 0 | -1 | 0 |

Nie nadpisuj ponownie algorytmu `Apply`.

**Punktacja:** poprawne sumowanie i indeksowanie maski 1 pkt, obsługa brzegu i walidacja maski 0.5 pkt, osobny wynik właściwego typu i końcowe ograniczenie koloru 0.5 pkt, po 0.5 pkt za każdy filtr konkretny.

## Etap 4: Polimorficzne przetwarzanie obrazu (2 punkty)

Rozbuduj klasę `Image`, nie tworząc nowego pliku.

### 4.1. Kopia obrazu (1 punkt)

Zaimplementuj publiczną metodę `Image Copy()`. Ma zwracać niezależną kopię pikseli, zachowując rozmiar i konkretny typ obrazu. W implementacji wykorzystaj `CreateEmpty` i indeksator.

Zmiana piksela kopii nie może zmieniać oryginału.

### 4.2. Sekwencja filtrów (1 punkt)

Zaimplementuj publiczną metodę:

```csharp
Image ApplyFilters(params IImageFilter[] filters);
```

Metoda rozpoczyna od kopii bieżącego obrazu, a następnie stosuje kolejne filtry w podanej kolejności. Wynik jednego filtra jest źródłem następnego. Nie zmienia obrazu, na którym została wywołana. Dla pustej tablicy zwraca niezależną kopię. Dla tablicy `null` lub elementu `null` zgłasza `ArgumentNullException`.

Wywołanie ma działać identycznie dla obu reprezentacji:

```csharp
Image source = new FlatImage(3, 2);
source[0, 0] = new RGB(1, 0, 0);

Image result = source.ApplyFilters(
    new FunctionFilter(new NegativeTransform()),
    new BlurFilter());
```

Nie dodawaj do metody instrukcji rozpoznających typ filtra.

## Warunki kontrolne przed oddaniem

- Obie reprezentacje mają identyczną treść po zapisaniu tych samych pikseli.
- Kopia jest niezależna, a wynik filtra ma tę samą reprezentację co źródło.
- Negatyw czerwieni `(1, 0, 0)` to cyjan `(0, 1, 1)`.
- Skala szarości czerwieni daje około `(0.299, 0.299, 0.299)`.
- Dwa negatywy odtwarzają wejście o kanałach z zakresu `[0, 1]`, z tolerancją błędu `float`.
- Rozmycie obrazu o stałym kolorze nie zmienia tego koloru, również na brzegach.
- Wyostrzenie obrazu o stałym kolorze z zakresu `[0, 1]` również go nie zmienia.
- Obraz 1 × 1 nie powoduje wyjścia poza tablicę podczas filtrowania.
- Błędna współrzędna w tablicy płaskiej jest odrzucana przed przeliczeniem indeksu.
- Filtry nie odczytują pikseli z tworzonego obrazu wynikowego.

Nie trzeba implementować zapisu ani odczytu PPM. Do sprawdzania efektów można użyć pomocnika z raytracera, ale nie jest on częścią punktowanego zadania.

---

## Komentarz do prognozy — poza treścią zadania

Najsilniej uzasadnione elementy to: kolor RGB, abstrakcyjna klasa obrazu, abstrakcyjny indeksator z dwoma argumentami, dwie reprezentacje pikseli i dwa rodzaje filtrów. Cztery pierwsze pliki naturalnie wynikają z tego układu.

Słabiej uzasadnione są dokładne reprezentacje. Zamiast tablicy płaskiej może pojawić się tablica poszarpana, osobne tablice kanałów albo reprezentacja rzadka. Warto umieć przełożyć współrzędne na każdy z tych układów.

Nie wiadomo, czy kanały będą typu float z zakresem 0–1, czy typu byte/int z zakresem 0–255. W drugim wariancie negatyw ma wzór 255 − kanał; akumulację filtrów nadal trzeba wykonywać w typie, który mieści wartości pośrednie, i ograniczać dopiero na końcu.

Niewiadomy jest również model filtra funkcyjnego: może chodzić o interfejs z metodą przekształcenia, klasę bazową z metodą abstrakcyjną albo delegat Func<RGB, RGB>. Wersja z interfejsem celowo ćwiczy zakres wspomniany na zdjęciu, bez wymagania lambd.

Metoda CreateEmpty służy w tej wersji do zachowania reprezentacji wyniku. Rzeczywiste polecenie może jej nie wymagać, zwracać zawsze jedną klasę obrazu albo modyfikować obraz w miejscu. Podobnie obsługa brzegów może polegać na pomijaniu sąsiadów, używaniu czerni albo pozostawianiu brzegu bez zmian. To są decyzje specyfikacji, nie jedna uniwersalnie poprawna odpowiedź.

Etap 4 jest najbardziej spekulatywny. Może zostać zastąpiony zapisem PPM, dodatkowymi filtrami, operatorami na obrazach lub składaniem filtrów. Priorytetem treningowym powinny pozostać etapy 1–3.

Proponowany podział treningowych 90 minut: etap 1 — 25 minut, etap 2 — 20 minut, etap 3 — 25 minut, etap 4 — 10 minut, sprawdzenie — 10 minut. Jeśli zabraknie czasu, doprowadź jeden filtr funkcyjny i jeden macierzowy do działania dla obu reprezentacji, zamiast dopisywać kolejne nazwy bez wspólnego algorytmu.
