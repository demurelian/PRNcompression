using PRNcompression.ViewModels.Base;

namespace PRNcompression.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        private string _ProgramStatus = "Status is OK";
        public string ProgramStatus
        {
            get => _ProgramStatus;
            set => Set(ref _ProgramStatus, value);
        }

        public MainWindowViewModel() 
        {
        
        }
    }
}
