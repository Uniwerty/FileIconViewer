using Avalonia.Controls;
using Avalonia.Interactivity;
using FileIconViewer.Models;
using FileIconViewer.ViewModels;

namespace FileIconViewer.Views;

public partial class MainWindow : Window
{
    private readonly IconResolver _resolver;

    public MainWindow()
    {
        InitializeComponent();
        _resolver = new IconResolver();
    }

    private void ExtensionSampleButton_OnClick(object? sender, RoutedEventArgs e)
    {
        FilePathBox.Text = ((sender as Button)?.DataContext as ExtensionSample)?.Extension;
    }


    private void ShowIconButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var viewModel = (sender as Button)?.DataContext as MainWindowViewModel;
        viewModel?.IconSamplesList.Clear();
        if (FilePathBox.Text == null) return;
        foreach (var sample in _resolver.GetIconsListByFilePath(FilePathBox.Text))
        {
            viewModel?.IconSamplesList.Add(sample);
        }
    }
}