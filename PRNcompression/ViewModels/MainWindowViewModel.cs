using PRNcompression.Infrastructure.Commands;
using PRNcompression.ViewModels.Base;
using System.Windows.Input;

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

        private string _ByteNumberStr = "OK";
        public string ByteNumberStr
        {
            get => _ByteNumberStr;
            set
            {
                if (ValidateByteNumberStr(value))
                    Set(ref _ByteNumberStr, value);
                else

            }
        }

        public ICommand GenerateDataCommand { get; }
        private bool CanGenerateDataCommandExecute(object p) => true;
        private void OnGenerateDataCommandExecute(object p)
        {
            ProgramStatus = "GenerateData Executed";
        }


        public MainWindowViewModel() 
        {
            GenerateDataCommand = new LambdaCommand(OnGenerateDataCommandExecute, CanGenerateDataCommandExecute);

        }
    }
}
