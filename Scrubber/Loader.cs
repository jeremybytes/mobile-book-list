using CsvHelper;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Scrubber;

public static class Loader
{
    public static List<LaserBook> LoadLaserBooks(string filename)
    {
        using var reader = new StreamReader(filename);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var data = csv.GetRecords<LaserBook>();
        return data.ToList();
    }

    public static List<LaserBook> ScrubRecords(this List<LaserBook> books)
    {
        for (int i = 0; i < books.Count; i++)
        {
            books[i].Owned = books[i].Owned == "Y" ? "true" : "false";
        }
        return books.ToList();
    }

    public static List<FullBook> LoadFile(string filename)
    {
        FixHeader(filename);
        using var reader = new StreamReader(filename);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var data = csv.GetRecords<FullBook>();
        return data.ToList();
    }

    public static void FixHeader(string filename)
    {
        string[] lines = File.ReadAllLines(filename);
        lines[0] = """BookId,Title,Author1,Author,AdditionalAuthors,ISBN,ISBN13,MyRating,AverageRating,Publisher,Binding,NumberOfPages,YearPublished,OriginalPublicationYear,DateRead,DateAdded,Bookshelves,BookshelvesWithPositions,ExclusiveShelf,MyReview,Spoiler,PrivateNotes,ReadCount,OwnedCopies""";
        File.WriteAllLines(filename, lines);
    }

    public static List<FullBook> ScrubRecords(this List<FullBook> books)
    {
        for (int i = 0; i < books.Count; i++)
        {
            books[i] = ScrubRecord(books[i]);
        }
        return books;
    }

    public static FullBook ScrubRecord(FullBook book)
    {
        book.ISBN = FixISBN(book.ISBN);
        book.ISBN13 = FixISBN(book.ISBN13);
        book.Author1 = FixAuthor1(book.Author1);
        book.Author = FixName(book.Author);
        book.AdditionalAuthors = NullifyEmptyString(book.AdditionalAuthors);
        book.Title = FixTitle(book.Title);
        book.OriginalPublicationYear = FixPublicationYear(book);
        book.Publisher = NullifyEmptyString(book.Publisher);
        book.Binding = NullifyEmptyString(book.Binding);
        book.Bookshelves = NullifyEmptyString(book.Bookshelves);
        book.BookshelvesWithPositions = NullifyEmptyString(book.BookshelvesWithPositions);
        book.MyRating = NullifyZero(book.MyRating);
        book.MyReview = NullifyEmptyString(book.MyReview);
        book.Spoiler = NullifyEmptyString(book.Spoiler);
        book.PrivateNotes = NullifyEmptyString(book.PrivateNotes);
        return book;
    }

    private static string? FixISBN(string? isbn)
    {
        // =\u00220061056073\u0022
        if (isbn is null) return null;
        isbn = isbn[2..^1];
        if (isbn == "") return null;
        return isbn;
    }

    private static string? FixAuthor1(string? author1)
    {
        return author1 switch
        {
            "To be confirmed" => "Joe Zieja",
            "James                    White" => "James White",
            _ => author1,
        };
    }

    private static string? FixName(string? name)
    {
        return name switch
        {
            "confirmed, To be" => "Zieja, Joe",
            "Camp, L. Sprague de" => "de Camp, L. Sprague",
            "Rey, Lester del" => "del Rey, Lester",
            "Fred;Hoyle, Geoffrey Hoyle" => "Hoyle, Fred",
            "Clow, Martha deMey" => "deMey Clow, Martha",
            "Guin, Ursula K. Le" => "Le Guin, Ursula K.",
            "harry-harrison, harry-harrison" => "Harrison, Harry",
            "III, Arthur Herzog" => "Herzog III, Arthur",
            "III, Cecil Snyder" => "Snyder III, Cecil",
            "Jakes, Barrington J. Bayley / John W." => "Bayley, Barrington J.",
            "Jr., James Tiptree" => "Tiptree Jr., James",
            "Jr., Kevin O'Donnell" => "O'Donnell Jr., Kevin",
            "Jr., Lloyd Biggle" => "Biggle Jr., Lloyd",
            "Jr., Neal Barrett" => "Barrett Jr., Neal",
            "Jr., Sam Merwin" => "Merwin Jr., Sam",
            "Jr., Walter M. Miller" => "Miller Jr., Walter M.",
            "Jr., Kurt Vonnegut" => "Vonnegut Jr., Kurt",
            "silverberg-robert, silverberg-robert" => "Silverberg, Robert",
            "Vogt, A.E. van" => "van Vogt, A.E.",
            "Scyoc, Sydney J. Van" => "Van Scyoc, Sydney J.",
            "W., Mackelworth R." => "Mackelworth, R. W.",
            "XIV, Dalai Lama" => "Dalai Lama XIV",
            _ => name,
        };
    }

    private static string? FixTitle(string? title)
    {
        int openParen = title?.IndexOf('(') ?? 0;
        if (openParen > 0)
            title = title?.Substring(0, openParen - 1);

        return title switch
        {
            "Title: VIRILITY GENE" => "Virility Gene",
            "Avatar, The" => "The Avatar",
            "THE SHAPE CHANGER" => "The Shape Changer",
            "DAYS OF GLORY CONCERT BAND/HARMONIE/FANFARE" => "The Days of Glory",
            "The soft kill" => "The Soft Kill",
            "Mask of Chaos and The Star Virus" => "The Star Virus / Mask of Chaos",
            "Highwood/Annihilation Factor" => "Highwood / Annihilation Factor",
            "Yolanda:  The Girl From Erosphere" => "Yolanda: The Girl From Erosphere",
            "Five for infinity" => "Five for Infinity",
            _ => title,
        };
    }

    private static int? FixPublicationYear(FullBook book)
    {
        if (book.Title == "Alien" && book.Author!.Contains("Leonard"))
            return 1970;

        return book.Title switch
        {
            "Keith Laumer's Retief" => 1960,
            "System Failure" => 2019,
            "Pain Gain" => 1977,
            "Doom Of Three Planets" => 1978,
            "My Name is Vladimir Sloifoiski" => 1970,
            "The Gods of Xuma" => 1978,
            "The Shape Changer" => 1973,
            "Class G-Zero" => 1976,
            "Cloning" => 1972,
            "Love Is Forever- We Are For Tonight" => 1970,
            "The Commodore at Sea / Spartan Planet" => 1979,
            "The Days of Glory" => 1971,
            "Highwood / Annihilation Factor" => 1972,
            "The Soft Kill" => 1973,
            "Once Departed" => 1970,
            "Gold the Man" => 1971,
            "Star Rogue" => 1970,
            "Under a Calculating Star" => 1975,
            "The Star Virus / Mask of Chaos" => 1970,
            "The Flight of the Endeavor" => 1978,
            "Yolanda: The Girl From Erosphere" => 1975,
            "The Bromius Phenomenon"=> 1973,
            "Five for Infinity" => 1976,
            "The Last Gene" => 1976,
            "Lemmus 3 The Archives of Haven" => 1977,
            "Saucer Hill" => 1979,
            "The Wandering Worlds" => 1976,
            "When Harlie Was One: Release 1.0" => 1972,
            "The Star Beast" => 1954,
            "All the Myriad Ways" => 1971,
            "Crashlander" => 1994,
            "A Hole in Space" => 1974,
            "The Long Arm of Gil Hamilton" => 1976,
            "The Ringworld Engineers" => 1979,
            "The Ringworld Throne" => 1996,
            _ => book.OriginalPublicationYear,
        };
    }

    private static string? NullifyEmptyString(string? data)
    {
        if (data == string.Empty) return null;
        return data;
    }

    private static int? NullifyZero(int? data)
    {
        if (data == 0) return null;
        return data;
    }

    public static void SaveAsJson(this List<LaserBook> books, string filename)
    {
        var options = new JsonSerializerOptions()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        };
        var json = JsonSerializer.Serialize(books, options);
        File.WriteAllText(filename, json);
    }

    public static void SaveAsJson(this List<FullBook> books, string filename)
    {
        var options = new JsonSerializerOptions()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        };
        var json = JsonSerializer.Serialize(books, options);
        File.WriteAllText(filename, json);
    }
}