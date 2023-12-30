namespace Scrubber;

class Program
{
    static void Main()
    {
        Loader
            .LoadFile("raw_book_list.csv")
            .ScrubRecords()
            .SaveAsJson("book_list.json");

        Loader
            .LoadLaserBooks("laser_books.csv")
            .ScrubRecords()
            .SaveAsJson("laser_books.json");
    }
}
