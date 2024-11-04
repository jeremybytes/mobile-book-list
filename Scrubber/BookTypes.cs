namespace Scrubber;

// BookId,Title,Author1,Author,AdditionalAuthors,
// ISBN,ISBN13,MyRating,AverageRating,Publisher,Binding,
// NumberOfPages,YearPublished,OriginalPublicationYear,
// DateRead,DateAdded,Bookshelves,BookshelvesWithPositions,
// ExclusiveShelf,MyReview,Spoiler,PrivateNotes,ReadCount,OwnedCopies

public class FullBook
{
    public int BookId { get; set; }
    public string? Title { get; set; }
    public string? Author1 { get; set; }
    public string? Author { get; set; }
    public string? AdditionalAuthors { get; set; }
    public string? ISBN { get; set; }
    public string? ISBN13 { get; set; }
    public int? MyRating { get; set; }
    public decimal? AverageRating { get; set; }
    public string? Publisher { get; set; }
    public string? Binding { get; set; }
    public int? NumberOfPages { get; set; }
    public int? YearPublished { get; set; }
    public int? OriginalPublicationYear { get; set; }
    public DateTime? DateRead { get; set; }
    public DateTime? DateAdded { get; set; }
    public string? Bookshelves { get; set; }
    public string? BookshelvesWithPositions { get; set; }
    public string? ExclusiveShelf { get; set; }
    public string? MyReview { get; set; }
    public string? Spoiler { get; set; }
    public string? PrivateNotes { get; set; }
    public int? ReadCount { get; set; }
    public int? OwnedCopies { get; set; }
}

public class LaserBook
{
    public int? Number { get; set; }
    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? Owned { get; set; }
}