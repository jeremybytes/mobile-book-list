namespace MobileBookViewer;

public partial class LaserBooksPage : ContentPage
{
    LaserViewModel viewModel = new();

    public LaserBooksPage()
	{
		InitializeComponent();
        this.BindingContext = viewModel;
        Loaded += async (_, _) => await viewModel.Initialize();

        //LoadAfterConstruction();
    }

    private async void LoadAfterConstruction()
    {
        await viewModel.Initialize();
        this.BindingContext = viewModel;
    }
}