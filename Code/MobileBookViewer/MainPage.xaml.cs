using BookList.Library;
using System.ComponentModel;

namespace MobileBookViewer;

public partial class MainPage : ContentPage, INotifyPropertyChanged
{
    BookViewModel viewModel = new();

    public MainPage()
    {
        InitializeComponent();
        this.BindingContext = viewModel;
        Loaded += async (_, _) => await viewModel.Initialize();
    }

    private async void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        SearchBar searchBar = (SearchBar)sender;
        if (searchBar.Text.Length == 0)
        {
            viewModel.SearchText = searchBar.Text;
            await searchBar.HideSoftInputAsync(CancellationToken.None);
        }
    }

    private async void LaserButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new LaserBooksPage());
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        var book = e.Parameter as Book;
        if (book is null) return;
        await Navigation.PushAsync(new BookDetailPage(book));
    }

    private async void searchBar_SearchButtonPressed(object sender, EventArgs e)
    {
        SearchBar searchBar = (SearchBar)sender;
        await searchBar.HideSoftInputAsync(CancellationToken.None);
    }
}
