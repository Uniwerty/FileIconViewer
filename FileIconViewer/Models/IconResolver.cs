using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Avalonia.Media.Imaging;

namespace FileIconViewer.Models;

public class IconResolver
{
    private const string MimeTypesFilePath = "/etc/mime.types";
    private const string IconsDirectory = "../../../../../../../../usr/share/icons/Yaru/";
    private const string IconFileExtension = ".png";
    private static readonly Regex WhitespaceRegex = new("\\s+");
    private static readonly Regex StartingWithDigitRegex = new("^\\d");
    private const char CommentChar = '#';
    private const string DefaultMimeType = "application-octet-stream";

    private static readonly Dictionary<string, string> GenericMimeTypes = new([
        new("text", "text-plain"),
        new("image", "image-x-generic"),
        new("video", "video-x-generic"),
        new("audio", "audio-x-generic")
    ]);

    private readonly Dictionary<string, string> _mimeTypeByExtension;
    private readonly SortedDictionary<string, string> _directoryByResolution;

    public IconResolver()
    {
        _mimeTypeByExtension = new Dictionary<string, string>();
        GetMimeTypesDictionary();

        _directoryByResolution = new SortedDictionary<string, string>(
            Comparer<string>.Create((x, y) =>
                {
                    var result = int.Parse(x.Split('x')[0]).CompareTo(int.Parse(y.Split('x')[0]));
                    if (result == 0) return x.Length.CompareTo(y.Length);
                    return result;
                }
            )
        );
        GetResolutionDirectoriesDictionary();
    }

    private void GetMimeTypesDictionary()
    {
        using var stream = new StreamReader(MimeTypesFilePath);
        while (stream.ReadLine() is { } line)
        {
            if (line.StartsWith(CommentChar))
            {
                continue;
            }

            var ruleParts = WhitespaceRegex.Split(line);
            if (ruleParts.Length > 1)
            {
                for (int i = 1; i < ruleParts.Length; i++)
                {
                    _mimeTypeByExtension.TryAdd(ruleParts[i], ruleParts[0]);
                }
            }
        }
    }

    private void GetResolutionDirectoriesDictionary()
    {
        foreach (var directory in Directory.EnumerateDirectories(IconsDirectory))
        {
            var iconResolution = directory.Split('/').Last();
            if (StartingWithDigitRegex.IsMatch(iconResolution))
            {
                var mimeTypesDirectory = directory + "/mimetypes";
                if (Directory.Exists(mimeTypesDirectory))
                {
                    _directoryByResolution.TryAdd(iconResolution, mimeTypesDirectory);
                }
            }
        }
    }

    public List<IconSample> GetIconsListByFilePath(string filePath)
    {
        var fileExtension = filePath.Split('.').Last();
        if (_mimeTypeByExtension.TryGetValue(fileExtension, out var mimeType))
        {
            return GetIconsListByMimeType(mimeType.Replace('/', '-'));
        }

        return GetIconsListByMimeType(DefaultMimeType);
    }

    private List<IconSample> GetIconsListByMimeType(string mimeType)
    {
        List<IconSample> iconSamples = [];
        var iconFileName = '/' + mimeType + IconFileExtension;
        foreach (var directoryMapping in _directoryByResolution)
        {
            var iconPath = directoryMapping.Value + iconFileName;
            if (File.Exists(iconPath))
            {
                iconSamples.Add(new(directoryMapping.Key, new Bitmap(iconPath)));
            }
            else
            {
                var contentType = mimeType.Split('-').First();
                var genericMimeType = GenericMimeTypes
                    .Where(typeMapping => contentType.Equals(typeMapping.Key))
                    .Select(typeMapping => typeMapping.Value)
                    .FirstOrDefault(DefaultMimeType);
                iconFileName = '/' + genericMimeType + IconFileExtension;
                iconPath = directoryMapping.Value + iconFileName;
                iconSamples.Add(new(directoryMapping.Key, new Bitmap(iconPath)));
            }
        }

        return iconSamples;
    }
}