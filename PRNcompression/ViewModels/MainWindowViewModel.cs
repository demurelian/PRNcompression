using PRNcompression.Infrastructure.Commands;
using PRNcompression.ViewModels.Base;
using PRNcompression.Services;
using System.Windows.Input;
using PRNcompression.Model;

namespace PRNcompression.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        private DataModel _DataModel;

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
                _DataModel.GenerateBytes(generate_size);
                //ProgramStatus = "GenerateData Executed";
            } else
            {
                ProgramStatus = "Wrong Number";
            }
        }

        public MainWindowViewModel() 
        {
            GenerateDataCommand = new LambdaCommand(OnGenerateDataCommandExecute, CanGenerateDataCommandExecute);
            _DataModel = new DataModel();
        }
    }
}
