using Avalonia.Media.Imaging;

namespace FileIconViewer.Models;

public class IconSample(string name, Bitmap icon)
{
    public string Name { get; } = name;
    public Bitmap Icon { get; } = icon;
}