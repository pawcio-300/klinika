public class Klinika
{
    private List<Pacjent> pacjenci;
    private const string SciezkaPliku = "dane_kliniki.txt";

    public Klinika()
    {
        pacjenci = new List<Pacjent>();
    }
    public void DodajPacjenta(Pacjent p)
    {
        pacjenci.Add(p);
        Console.WriteLine($"Pacjent {p.Imie} został dodany do bazy kliniki.");
    }
    public Pacjent ZnajdzPacjenta(string imie)
    {
        foreach (Pacjent p in pacjenci)
        {
            if (p.Imie.ToLower() == imie.ToLower())
            {
                return p;
            }
        }
        return null;
    }
    public void ZapiszDoPliku()
    {
        using (StreamWriter plik = new StreamWriter(SciezkaPliku))
        {
            foreach (Pacjent p in pacjenci)
            {
                plik.WriteLine($"P:{p.Imie},{p.Gatunek},{p.Wiek},{p.DaneOpiekuna}");

                foreach (Wizyta w in p.HistoriaWizyt)
                {
                    plik.WriteLine($"W:{w.Data},{w.Lekarz}");

                    foreach (ProceduraMedyczna m in w.Procedury)
                    {
                        plik.WriteLine($"M:{m.TypProcedury},{m.Opis},{m.Wynik}");
                    }
                }
            }
        }
        Console.WriteLine("Pomyślnie zapisano całą strukturę kliniki do pliku tekstowego.");
    }

    public void WczytajZPliku()
    {
        if (!File.Exists(SciezkaPliku))
        {
            Console.WriteLine("Plik bazy danych nie istnieje!");
            return;
        }

        pacjenci.Clear();
        Pacjent aktualnyPacjent = null;
        Wizyta aktualnaWizyta = null;

        using (StreamReader plik = new StreamReader(SciezkaPliku))
        {
            string linia;
            while ((linia = plik.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(linia)) continue;
                string typ = linia.Substring(0, 2);
                string[] dane = linia.Substring(2).Split(',');

                switch (typ)
                {
                    case "P:":
                        string imie = dane[0];
                        string gatunek = dane[1];
                        int wiek = int.Parse(dane[2]);
                        string opiekun = dane[3];

                        aktualnyPacjent = new Pacjent(imie, gatunek, wiek, opiekun);
                        pacjenci.Add(aktualnyPacjent);
                        aktualnaWizyta = null;
                        break;

                    case "W:":
                        if (aktualnyPacjent != null)
                        {
                            string data = dane[0];
                            string lekarz = dane[1];

                            aktualnaWizyta = new Wizyta(data, lekarz);
                            aktualnyPacjent.DodajWizyte(aktualnaWizyta);
                        }
                        break;

                    case "M:":
                        if (aktualnaWizyta != null)
                        {
                            string typProc = dane[0];
                            string opis = dane[1];
                            string wynik = dane[2];

                            ProceduraMedyczna nowaProc = new ProceduraMedyczna(typProc, opis, wynik);
                            aktualnaWizyta.DodajProcedure(nowaProc);
                        }
                        break;
                }
            }
        }
        Console.WriteLine("Pomyślnie odtworzono pacjentów wraz z historią wizyt i zabiegów!");
    }
}

public class Pacjent
{
    private string imie;
    private string gatunek;
    private int wiek;
    private string daneOpiekuna;
    private List<Wizyta> historiaWizyt;

    public Pacjent(string imie, string gatunek, int wiek, string daneOpiekuna)
    {
        this.imie = imie;
        this.gatunek = gatunek;
        this.wiek = wiek;
        this.daneOpiekuna = daneOpiekuna;
        this.historiaWizyt = new List<Wizyta>();
    }
    public void DodajWizyte(Wizyta w)
    {
        historiaWizyt.Add(w);
        Console.WriteLine($"Dodano wizytę z dnia {w.Data} dla pacjenta {imie}.");
    }
    public void WyswietlHistorie()
    {
        Console.WriteLine($"\n--- Historia wizyt pacjenta: {imie} ({gatunek}) ---");
        foreach (var wizyta in historiaWizyt)
        {
            Console.WriteLine($"Data: {wizyta.Data}, Lekarz: {wizyta.Lekarz}");
            Console.WriteLine($"Liczba procedur: {wizyta.Procedury.Count}");
        }
        Console.WriteLine("---------------------------------------------------");
    }

    public string Imie => imie;
    public string Gatunek => gatunek;
    public int Wiek => wiek;
    public string DaneOpiekuna => daneOpiekuna;
    public List<Wizyta> HistoriaWizyt => historiaWizyt;
}

public class Wizyta
{
    private string data;
    private string lekarz;
    private List<ProceduraMedyczna> procedury;

    public Wizyta(string data, string lekarz)
    {
        this.data = data;
        this.lekarz = lekarz;
        this.procedury = new List<ProceduraMedyczna>();
    }

    public void DodajProcedure(ProceduraMedyczna p)
    {
        procedury.Add(p);
        Console.WriteLine($"Dodano procedurę {p.TypProcedury} do wizyty.");
    }

    public string Data => data;
    public string Lekarz => lekarz;
    public List<ProceduraMedyczna> Procedury => procedury;
}

public class ProceduraMedyczna
{
    private string typProcedury;
    private string opis;
    private string wynik;

    public ProceduraMedyczna(string typProcedury, string opis, string wynik)
    {
        this.typProcedury = typProcedury;
        this.opis = opis;
        this.wynik = wynik;
    }

    public string TypProcedury => typProcedury;
    public string Opis => opis;
    public string Wynik => wynik;
}

class Program
{
    static void Main(string[] args)
    {
        Klinika klinika = new Klinika();

        while (true)
        {
            Console.WriteLine("=== MENU KLINIKI WETERYNARYJNEJ ===");
            Console.WriteLine("1. Dodaj nowego pacjenta (zwierzę)");
            Console.WriteLine("2. Zarejestruj wizytę dla pacjenta");
            Console.WriteLine("3. Wyświetl historię wizyt pacjenta");
            Console.WriteLine("4. Zapisz dane do pliku");
            Console.WriteLine("5. Wczytaj dane z pliku");
            Console.WriteLine("6. Wyjście z programu");
            Console.Write("Wybierz opcję (1-6): ");

            string wybor = Console.ReadLine();
            Console.WriteLine();

            switch (wybor)
            {
                case "1":
                    Console.Write("Podaj imię zwierzęcia: ");
                    string imie = Console.ReadLine();
                    Console.Write("Podaj gatunek (np. Pies, Kot): ");
                    string gatunek = Console.ReadLine();
                    Console.Write("Podaj wiek: ");
                    int wiek;
                    while (int.TryParse(Console.ReadLine(), out wiek) == false || wiek < 0)
                    {
                        Console.Write("To nie jest poprawny wiek! Podaj liczbę całkowitą: ");
                    }
                    Console.Write("Podaj imię i nazwisko opiekuna: ");
                    string opiekun = Console.ReadLine();

                    Pacjent nowyPacjent = new Pacjent(imie, gatunek, wiek, opiekun);
                    klinika.DodajPacjenta(nowyPacjent);
                    break;

                case "2":
                    Console.Write("Podaj imię pacjenta, dla którego chcesz dodać wizytę: ");
                    string szukaneImie = Console.ReadLine();
                    Pacjent pacjentDoWizyty = klinika.ZnajdzPacjenta(szukaneImie);

                    if (pacjentDoWizyty != null)
                    {
                        Console.Write("Podaj nazwisko lekarza prowadzącego: ");
                        string lekarz = Console.ReadLine();

                        Console.Write("Podaj datę wykonania procedury: ");
                        string data = Console.ReadLine();

                        // Wizytę tworzymy na samym początku, raz dla całej operacji
                        Wizyta nowaWizyta = new Wizyta(data, lekarz);

                        // Zmienna sterująca naszą nową pętlą
                        bool dodajKolejna = true;

                        while (dodajKolejna)
                        {
                            Console.WriteLine("\n--- Wprowadzanie danych procedury ---");
                            Console.Write("Podaj nazwę procedury medycznej (np. Szczepienie, Kontrola): ");
                            string typProc = Console.ReadLine();

                            Console.Write("Podaj krótki opis procedury: ");
                            string opisProc = Console.ReadLine();

                            ProceduraMedyczna nowaProcedura = new ProceduraMedyczna(typProc, opisProc, "Do wykonania");
                            nowaWizyta.DodajProcedure(nowaProcedura);

                            Console.Write("\nCzy chcesz dodać kolejną procedurę do tej wizyty? (t/n): ");
                            string odpowiedz = Console.ReadLine().ToLower();

                            if (odpowiedz != "t" && odpowiedz != "tak")
                            {
                                dodajKolejna = false;
                            }
                        }

                        pacjentDoWizyty.DodajWizyte(nowaWizyta);

                        Console.WriteLine("\nWizyta oraz wszystkie wprowadzone procedury zostały pomyślnie dodane!\n");
                    }
                    else
                    {
                        Console.WriteLine("\nBłąd: Nie znaleziono pacjenta o takim imieniu.\n");
                    }
                    break;

                case "3":
                    Console.Write("Podaj imię pacjenta, by zobaczyć historię: ");
                    string imieDoHistorii = Console.ReadLine();
                    Pacjent pacjentZHistoria = klinika.ZnajdzPacjenta(imieDoHistorii);

                    if (pacjentZHistoria != null)
                    {
                        pacjentZHistoria.WyswietlHistorie();
                    }
                    else
                    {
                        Console.WriteLine("\nBłąd: Nie znaleziono pacjenta o takim imieniu.\n");
                    }
                    break;

                case "4":
                    klinika.ZapiszDoPliku();
                    Console.WriteLine();
                    break;

                case "5":
                    klinika.WczytajZPliku();
                    Console.WriteLine();
                    break;

                case "6":
                    Console.WriteLine("Zamykanie programu. Do widzenia!");
                    return;

                default:
                    Console.WriteLine("Niepoprawny wybór. Wybierz cyfrę od 1 do 5.\n");
                    break;
            }
            Console.WriteLine("Naciśnij dowolny klawisz, aby wrócić do menu...");
            Console.ReadKey();
            Console.Clear();
        }
    }
}