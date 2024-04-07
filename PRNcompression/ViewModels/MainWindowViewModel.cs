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
