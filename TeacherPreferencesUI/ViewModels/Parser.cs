using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Win32;
using TeacherPreferencesUI.Commands;
using TeacherPreferencesUI.UserControls;
using TeacherPreferences;
using static ExcelTimetableParser.TimetableParser;
using System.Printing;


namespace TeacherPreferencesUI.ViewModels
{
    public class Parser : INotifyPropertyChanged
    {
        public static RelayCommand OpenFileCommand(ApplicationViewModel viewModel)
        {
            return new RelayCommand((o) =>
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "Excel Files (*.xlsx)|*.xlsx",
                    Title = "Выберите Excel файл"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    string filePath = openFileDialog.FileName;
                    var result = ExcelParser.processFile(filePath);

                    if (result.IsOk)
                    {
                        var parseResult = result.ResultValue;
                        PreferencesValidation.ValidationResult? validationResult = null;

                        if (parseResult.Timetable != null)
                        {
                            validationResult = PreferencesValidation.validate(viewModel.db, parseResult.Timetable);

                            if (validationResult is PreferencesValidation.ValidationResult.Invalid invalidResult)
                            {
                                viewModel.ValidationResult = new ObservableCollection<string>(invalidResult.Item);
                            }
                        }

                        if (parseResult?.Errors != null && parseResult.Errors.Any())
                        {
                            viewModel.Errors = new ObservableCollection<string>(parseResult.Errors);
                            viewModel.CurrentView = new ErrorView();
                        }
                        else
                        {
                            viewModel.ValidationResult = null;
                            viewModel.CurrentView = new ParsingResultView();
                        }
                    }
                    else
                    {
                        var errorMessage = result.ErrorValue;
                        viewModel.CurrentView = new ErrorView();
                    }
                }
            });
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
