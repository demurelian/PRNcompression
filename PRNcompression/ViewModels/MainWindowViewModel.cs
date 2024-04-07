using PRNcompression.Infrastructure.Commands;
using PRNcompression.ViewModels.Base;
using PRNcompression.Services;
using System.Windows.Input;
using System.Collections.ObjectModel;
using PRNcompression.Model;
using System.Windows.Data;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using static PRNcompression.ViewModels.DirectoryViewModel;
using System.IO;
using System;
using System.Windows;
using PRNcompression.Infrastructure;

namespace PRNcompression.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        private string _ProgramStatus = "OK";
        public string ProgramStatus
        {
            get => _ProgramStatus;
            set => Set(ref _ProgramStatus, value);
        }
        private int _Progress;
        public int Progress
        {
            get => _Progress;
            set => Set(ref _Progress, value);
        }
        private string _FilePath = null;
        public string FilePath
        {
            get => _FilePath;
            set => Set(ref _FilePath, value);
        }

        private ObservableCollection<DirectoryViewModel> _Disks;
        public ObservableCollection<DirectoryViewModel> Disks
        {
            get => _Disks;
            set => Set(ref _Disks, value);
        }
        //public DirectoryViewModel DiskRootDir { get; } = new DirectoryViewModel("d:\\");
        
        private FileViewModel _SelectedFile;
        public FileViewModel SelectedFile
        {
            get => SelectedFile;
            set
            {
                Set(ref _SelectedFile, value);
                ProgramStatus = _SelectedFile.Name;
            }
        }

        public ICommand FileSelectedCommand { get; }
        private bool CanFileSelectedCommandExecute(object p) => true;
        private void OnFileSelectedCommandExecute(object p)
        {
            if (FilePath != null)
            {
                var file = new FileInfo(FilePath);
                
            }
        }

        private IEnumerable<byte> _InitialBytes;
        public IEnumerable<byte> InitialBytes
        {
            get => _InitialBytes;
            private set => Set(ref _InitialBytes, value);
        }

        private string _ByteNumberStr;
        public string ByteNumberStr
        {
            get => _ByteNumberStr;
            set => Set(ref _ByteNumberStr, value);
        }

        public ICommand GenerateDataCommand { get; }
        private bool CanGenerateDataCommandExecute(object p) => true;
        private void OnGenerateDataCommandExecute(object p)
        {
            var generate_size = ValidationHelper.ValidateByteNumberString(_ByteNumberStr);
            if (generate_size > 0)
            {
                InitialBytes = DataModel.GenerateBytes(generate_size);
                ProgramStatus = "Data Generated";
            } else
            {
                ProgramStatus = "Wrong Number";
            }
        }

        public MainWindowViewModel() 
        {
            GenerateDataCommand = new LambdaCommand(OnGenerateDataCommandExecute, CanGenerateDataCommandExecute);
            FileSelectedCommand = new LambdaCommand(OnFileSelectedCommandExecute, CanFileSelectedCommandExecute);
            Disks = new ObservableCollection<DirectoryViewModel>();

            // Simulating some data (replace with your logic)
            foreach (var drive in DriveInfo.GetDrives())
            {
                var dir = new DirectoryViewModel(drive.Name);
                Disks.Add(dir);
            }
        }
    }
}
