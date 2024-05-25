using System.Collections.Generic;
using System.Collections.ObjectModel;
using FileIconViewer.Models;

namespace FileIconViewer.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public List<ExtensionSample> ExtensionSamplesList { get; set; } =
    [
        new("txt"),
        new("rtf"),
        new("pdf"),
        new("docx"),
        new("pptx"),
        new("xlsx"),
        new("java"),
        new("cpp"),
        new("json"),
        new("xml"),
        new("svg"),
        new("png"),
        new("jpeg"),
        new("mp4"),
        new("avi"),
        new("mp3"),
        new("midi"),
        new("au"),
        new("zip"),
        new("apk"),
        new("deb")
    ];

    public ObservableCollection<IconSample> IconSamplesList { get; set; } = [];
}