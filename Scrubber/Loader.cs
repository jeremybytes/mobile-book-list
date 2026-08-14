using CsvHelper;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Scrubber;

public static class Loader
{
    public static OutputBook ToOutputBook(ImportBook import)
    {
        var book = new OutputBook()
        {
            BookId = import.BookId,
            Title = import.Title,
            Author1 = import.Author1,
            Author = import.Author,
            AdditionalAuthors = import.AdditionalAuthors,
            ISBN = import.ISBN,
            ISBN13 = import.ISBN13,
            MyRating = (int?)import.MyRating,
            Publisher = import.Publisher,
            Binding = import.Binding,
            NumberOfPages = import.NumberOfPages,
            YearPublished = import.YearPublished,
            OriginalPublicationYear = import.OriginalPublicationYear,
            DateRead = import.DateRead,
            DateAdded = import.DateAdded,
            Bookshelves = import.Bookshelves,
            BookshelvesWithPositions = import.BookshelvesWithPositions,
            ExclusiveShelf = import.ExclusiveShelf,
            MyReview = import.MyReview,
            Spoiler = import.Spoiler,
            PrivateNotes = import.PrivateNotes,
            ReadCount = import.ReadCount,
            OwnedCopies = import.OwnedCopies,
        };
        return book;
    }

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

    public static List<OutputBook> LoadFile(string filename)
    {
        FixHeader(filename);
        using var reader = new StreamReader(filename);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var data = csv.GetRecords<ImportBook>();
        var output = data.Select(i => ToOutputBook(i));
        return output.ToList();
    }

    public static void FixHeader(string filename)
    {
        string[] lines = File.ReadAllLines(filename);
        lines[0] = """BookId,Title,Author1,Author,AdditionalAuthors,ISBN,ISBN13,MyRating,Publisher,Binding,NumberOfPages,YearPublished,OriginalPublicationYear,DateRead,DateAdded,Bookshelves,BookshelvesWithPositions,ExclusiveShelf,MyReview,Spoiler,PrivateNotes,ReadCount,OwnedCopies""";
        File.WriteAllLines(filename, lines);
    }

    public static List<OutputBook> ScrubRecords(this List<OutputBook> books)
    {
        for (int i = 0; i < books.Count; i++)
        {
            books[i] = ScrubRecord(books[i]);
        }
        return books;
    }

    public static OutputBook ScrubRecord(OutputBook book)
    {
        book.ISBN = FixISBN(book.ISBN);
        book.ISBN13 = FixISBN(book.ISBN13);
        book.Author1 = FixAuthor1(book.Author1);
        book.Author = FixName(book.Author);
        book.AdditionalAuthors = NullifyEmptyString(book.AdditionalAuthors);
        book.Title = FixTitle(book);
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
            "Fred;Hoyle, Geoffrey Hoyle" => "Hoyle, Fred and Geoffrey",
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
            "james-blish, james-blish" => "Blish, James",
            _ => name,
        };
    }

    private static string? FixTitle(OutputBook book)
    {
        int openParen = book.Title?.IndexOf('(') ?? 0;
        if (openParen > 0)
            book.Title = book.Title?.Substring(0, openParen - 1);

        return book.BookId switch
        {
            4207005 => "Virility Gene",
            125567002 => "The Shape Changer",
            41459123 => "The Days of Glory",
            6317581 => "The Soft Kill",
            164333779 => "The Star Virus / Mask of Chaos",
            42605530 => "Highwood / Annihilation Factor",
            3250817 => "Yolanda: The Girl From Erosphere",
            8842358 => "Five for Infinity",
            8035077 => "Day After Tomorrow",
            4986352 => "The Third Body: A Novel",
            _ => book.Title,
        };
    }

    private static int? FixPublicationYear(OutputBook book)
    {
        return book.BookId switch
        {
            900188 => 1970,
            53503609 => 1960,
            42201415 => 2019,
            9721109 => 1977,
            11128145 => 1978,
            135655412 => 1970,
            60523876 => 1978,
            7137525 => 1973,
            125567002 => 1973,
            163798 => 1976,
            761379 => 1972,
            40678119 => 1970,
            3066418 => 1979,
            41459123 => 1971,
            42605530 => 1972,
            6317581 => 1973,
            8034776 => 1970,
            55261061 => 1971,
            4945490 => 1970,
            1975347 => 1975,
            164333779 => 1970,
            167060109 => 1978,
            3250817 => 1975,
            208634668 => 1973,
            8842358 => 1976,
            87871353 => 1976,
            166662384 => 1977,
            1262383 => 1979,
            1937468 => 1976,
            58527184 => 1972,
            175328 => 1954,
            218479 => 1971,
            100347 => 1994,
            218473 => 1974,
            116355 => 1976,
            760547 => 1979,
            64467 => 1996,
            211175894 => 2026,
            11153219 => 1972,
            134214322 => 1979,
            125930852 => 1970,
            232058387 => 1977,
            1550192 => 1971,
            1415143 => 1978,
            45706833 => 1975,
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

    private static decimal? NullifyZero(decimal? data)
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

    public static void SaveAsJson(this List<OutputBook> books, string filename)
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