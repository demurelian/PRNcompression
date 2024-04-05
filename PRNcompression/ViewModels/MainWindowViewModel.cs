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

namespace PRNcompression.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        public ObservableCollection<DiskViewModel> Disks { get; set; }

        //public DirectoryViewModel DiskRootDir { get; } = new DirectoryViewModel("d:\\");
        //private DirectoryViewModel _SelectedDirectory;
        //public DirectoryViewModel SelectedDirectory
        //{
        //    get => _SelectedDirectory;
        //    set => Set(ref _SelectedDirectory, value);
        //}

        private IEnumerable<byte> _InitialBytes;
        public IEnumerable<byte> InitialBytes
        {
            get => _InitialBytes;
            private set => Set(ref _InitialBytes, value);
        }

        private string _ProgramStatus = "OK";
        public string ProgramStatus
        {
            get => _ProgramStatus;
            set => Set(ref _ProgramStatus, value);
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
            Disks = new ObservableCollection<DiskViewModel>();

            // Simulating some data (replace with your logic)
            foreach (var drive in DriveInfo.GetDrives())
            {
                var diskViewModel = new DiskViewModel(drive.Name);
                try
                {
                    foreach (var directory in drive.RootDirectory.GetDirectories())
                    {
                        var directoryViewModel = new DirectoryViewModel(directory.Name);
                        try
                        {
                            foreach (var file in directory.GetFiles())
                            {
                                directoryViewModel.DirectoryItems.Add(new FileViewModel(file.Name));
                            }
                        }
                        catch (UnauthorizedAccessException UAE)
                        {

                        }
                        diskViewModel.DirectoryItems.Add(directoryViewModel);
                    }
                }
                catch (UnauthorizedAccessException UAE)
                {

                }
                Disks.Add(diskViewModel);
            }
        }
    }
}
