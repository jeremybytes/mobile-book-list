using BookList.Library;
using System.ComponentModel;
using System.Windows.Input;

namespace MobileBookViewer;

public class BookViewModel : INotifyPropertyChanged
{
    private BookTitleComparer titleComparer = new();
    private int page = 0;
    private int pageSize = 10;

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

    private bool navVisible = true;
    public bool NavVisible
    {
        get { return navVisible; }
        set
        {
            navVisible = value;
            RaisePropertyChanged(nameof(NavVisible));
        }
    }


    public async Task Initialize()
    {
        allBooks = (await BookLoader.LoadJsonData("book_list.json"))?
                   .Where(b => b.Bookshelves?.Contains("owned-sci-fi") ?? false)
                   .OrderBy(b => b.Author).ThenBy(b => b.Title, titleComparer)
                   .ToList() ?? [];
        defaultBooks = allBooks.Take(pageSize);
        page = 1;
        Books = defaultBooks;
    }

    public ICommand PerformSearch =>
        new Command<string>((string searchText) => SearchText = searchText);

    public ICommand NextPageCommand => new Command(NextPage);
    public ICommand PreviousPageCommand => new Command(PreviousPage);

    public void UpdateSearch()
    {
        if (string.IsNullOrEmpty(searchText) || string.IsNullOrWhiteSpace(searchText))
        {
            page = 1;
            Books = defaultBooks;
            NavVisible = true;
            return;
        }

        Books = allBooks.Where(b => b.Author.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) ||
                                    b.Title.Contains(searchText, StringComparison.CurrentCultureIgnoreCase))
                        .Take(100);
        NavVisible = false;
    }

    public void NextPage()
    {
        page++;
        Books = allBooks.Skip(pageSize * page).Take(pageSize);
    }

    public void PreviousPage()
    {
        if (page > 1)
            page--;
        Books = allBooks.Skip(pageSize * page).Take(pageSize);
    }

    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler? PropertyChanged;
    private void RaisePropertyChanged(string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion
}
