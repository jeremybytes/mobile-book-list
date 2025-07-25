using BookList.Library;
using System.ComponentModel;
using System.Windows.Input;

namespace MobileBookViewer;

public class BookViewModel : INotifyPropertyChanged
{
    private BookTitleComparer titleComparer = new();
    private int page = 1;
    private int pageSize = 11;

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
            //defaultBooks = allBooks.Skip(pageSize * page).Take(pageSize);
            Books = allBooks;
        }
    }

    public ICommand PerformSearch =>
        new Command<string>((string searchText) => SearchText = searchText);

    public ICommand NextPageCommand => new Command(NextPage);
    public ICommand PreviousPageCommand => new Command(PreviousPage);

    public void UpdateSearch()
    {
        if (string.IsNullOrEmpty(searchText) || string.IsNullOrWhiteSpace(searchText))
        {
            Books = allBooks;
            return;
        }

        Books = allBooks.Where(b => b.Author.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) ||
                                    b.Title.Contains(searchText, StringComparison.CurrentCultureIgnoreCase))
                        .Take(100);
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
