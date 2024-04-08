using PRNcompression.ViewModels.Base;

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

        private string _FilePath = null;
        public string FilePath
        {
            get => _FilePath;
            set => Set(ref _FilePath, value);
        }

        public MainWindowViewModel() 
        {
            
        }
    }
}
