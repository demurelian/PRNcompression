using PRNcompression.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace PRNcompression.ViewModels
{
    //public class ItemViewModel
    //{
    //    public string Name { get; set; }
    //}

    //internal class DiskViewModel : DirectoryViewModel
    //{
    //    string DiskName;

    //    public DiskViewModel(string name)
    //    {
    //        DiskName = name;
    //        _DirectoryInfo = new DirectoryInfo(name);
    //    }
    //}

    //public class DirectoryViewModel : ItemViewModel
    //{
    //    public ObservableCollection<ItemViewModel> DirectoryItems { get; set; }

    //    public DirectoryViewModel(string name)
    //    {
    //        Name = name;
    //        DirectoryItems = new ObservableCollection<ItemViewModel>();
    //    }
    //    public static IEnumerable<DirectoryViewModel> SubDirectories(DirectoryInfo directoryInfo)
    //    {
    //        try
    //        {
    //            return directoryInfo
    //                .EnumerateDirectories()
    //                .Select(dir => new DirectoryViewModel(dir.FullName));
    //        }
    //        catch (UnauthorizedAccessException e)
    //        {
    //            Debug.WriteLine(e);
    //        }
    //        return Enumerable.Empty<DirectoryViewModel>();
    //    }
    //}

    //public class FileViewModel : ItemViewModel
    //{
    //    public FileViewModel(string name)
    //    {
    //        Name = name;
    //    }
    //}

    class DirectoryViewModel : ViewModel
    {
        private readonly DirectoryInfo _DirectoryInfo;
        public IEnumerable<DirectoryViewModel> SubDirectories
        {
            get
            {
                try
                {
                    return _DirectoryInfo
                        .EnumerateDirectories()
                        .Select(dir => new DirectoryViewModel(dir.FullName));
                }
                catch (UnauthorizedAccessException e)
                {
                    Debug.WriteLine(e);
                }
                return Enumerable.Empty<DirectoryViewModel>();
            }
        }
        public IEnumerable<FileViewModel> Files
        {
            get
            {
                try
                {
                    var files = _DirectoryInfo
                        .EnumerateFiles()
                        .Select(file => new FileViewModel(file.FullName));
                    return files;
                }
                catch (UnauthorizedAccessException e)
                {
                    Debug.WriteLine(e);
                }
                return Enumerable.Empty<FileViewModel>();
            }
        }
        public IEnumerable<object> DirectoryItems
        {
            get
            {
                try
                {
                    return SubDirectories.Cast<object>().Concat(Files);
                }
                catch (UnauthorizedAccessException e)
                {
                    Debug.WriteLine(e);
                }
                return Enumerable.Empty<object>();
            }
        }
        public string Name => _DirectoryInfo.Name;
        public string Path => _DirectoryInfo.FullName;
        public DateTime CreationTime => _DirectoryInfo.CreationTime;
        public DirectoryViewModel(string path) => _DirectoryInfo = new DirectoryInfo(path);
        
    }

    class FileViewModel : ViewModel
    {
        private FileInfo _FileInfo;
        public string Name => _FileInfo.Name;
        public string Path => _FileInfo.FullName;
        public DateTime CreationTime => _FileInfo.CreationTime;
        public FileViewModel(string path) => _FileInfo = new FileInfo(path);
    }
}
