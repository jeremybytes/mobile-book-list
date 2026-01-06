using BookList.Library;
using System.ComponentModel;
using System.Windows.Input;

namespace MobileBookViewer;

public class BookViewModel : INotifyPropertyChanged
{
    private BookTitleComparer titleComparer = new();

    private string searchText = "";
    public string SearchText
    {
        get { return searchText; }
        set
        {
            searchText = value;
            UpdateSearch();
            RaisePropertyChanged(nameof(SearchText));
        }
    }

    private IEnumerable<Book> allBooks = [];
    private IEnumerable<Book> defaultBooks = [];

    private IEnumerable<Book> books = [];
    public IEnumerable<Book> Books
    {
        get { return books; }
        set
        {
            books = value;
            RaisePropertyChanged(nameof(Books));
        }
    }

    public async Task Initialize()
    {
        if (searchText.Trim() != string.Empty)
        {
            UpdateSearch();
        }
        else
        {
            allBooks = (await BookLoader.LoadJsonData("book_list.json"))?
                       .Where(b => b.Bookshelves?.Contains("owned-sci-fi") ?? false)
                       .OrderBy(b => b.Author).ThenBy(b => b.Title, titleComparer)
                       .ToList() ?? [];
            Books = allBooks;
        }
    }

    public ICommand PerformSearch =>
        new Command<string>((string searchText) => SearchText = searchText);

    public void UpdateSearch()
    {
        if (string.IsNullOrEmpty(searchText) || string.IsNullOrWhiteSpace(searchText))
        {
            Books = allBooks;
            return;
        }

        Books = allBooks.Where(b => b.Author.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) ||
                                    b.Title.Contains(searchText, StringComparison.CurrentCultureIgnoreCase));
    }

    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler? PropertyChanged;
    private void RaisePropertyChanged(string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion
}
