using PRNcompression.Infrastructure.Commands;
using PRNcompression.Services;
using PRNcompression.Services.Interfaces;
using PRNcompression.ViewModels.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;

namespace PRNcompression.ViewModels
{
    internal class InitialDataViewModel : ViewModel
    {
        MainWindowViewModel MainModel;
        IBytesGenerationService _BytesGenerationService;

        private ObservableCollection<DirectoryViewModel> _Disks;
        public ObservableCollection<DirectoryViewModel> Disks
        {
            get => _Disks;
            set => Set(ref _Disks, value);
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

        public ICommand FileSelectedCommand { get; }
        private bool CanFileSelectedCommandExecute(object p) => true;
        private void OnFileSelectedCommandExecute(object p)
        {
            if (MainModel.FilePath != null)
            {
                var file = new FileInfo(MainModel.FilePath);

            }
        }

        public ICommand GenerateDataCommand { get; }
        private bool CanGenerateDataCommandExecute(object p) => true;
        private void OnGenerateDataCommandExecute(object p)
        {
            InitialBytes = _BytesGenerationService.GenerateBytes(_ByteNumberStr);
        }

        public InitialDataViewModel()
        {
            _BytesGenerationService = new BytesGenerationService();
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
