using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Win32;
using TeacherPreferencesUI.Commands;
using TeacherPreferencesUI.UserControls;
using TeacherPreferences;
using static ExcelTimetableParser.TimetableParser;
using static LoadParser;
using MathNet.Numerics.LinearAlgebra.Solvers;
using TeacherPreferencesDataModel;
using System.Windows;

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

        public static RelayCommand LoadAcademicLoadCommand(ApplicationViewModel viewModel)
        {
            return new RelayCommand((o) =>
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "Excel Files (*.xlsx)|*.xlsx",
                    Title = "Выберите файл академической нагрузки"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    string filePath = openFileDialog.FileName;

                    try
                    {

                        var result = parseAcademicLoadTeachers(filePath);

                        foreach (var (teacherName, departmentName, courseName, activityType, groups) in result)
                        {
                            var department = viewModel.db.Departments.FirstOrDefault(d => d.Name == departmentName);
                            if (department == null)
                            {
                                department = new Department { Name = departmentName };
                                viewModel.db.Departments.Add(department);
                                viewModel.db.SaveChanges();
                                viewModel.Departments.Add(department);
                            }

                            var nameParts = teacherName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                            string surname = nameParts[0];
                            string name = nameParts.Length > 1 ? nameParts[1] : "";
                            string patronymic = nameParts.Length > 2 ? nameParts[2] : null;

                            var teacher = viewModel.db.Teachers.FirstOrDefault(t => t.Name == name && t.Surname == surname && t.Patronymic == patronymic);
                            if (teacher == null)
                            {
                                teacher = new Teacher
                                {
                                    Name = name,
                                    Surname = surname,
                                    Patronymic = patronymic,
                                    DepartmentId = department.DepartmentId,
                                    Department = department
                                };
                                viewModel.db.Teachers.Add(teacher);
                                viewModel.db.SaveChanges();

                                viewModel.Teachers.Add(TeacherViewModel.FromModel(teacher, viewModel));
                            }

                            foreach (var group in groups)
                            {
                                var course = new TeacherPreferencesDataModel.Course
                                {
                                    Group = group,
                                    SubjectName = courseName,
                                    ClassType = ClassType.Practical,
                                    TeacherId = teacher.Id,
                                    Teacher = teacher
                                };
                                viewModel.db.Courses.Add(course);
                                viewModel.db.SaveChanges();

                                viewModel.Courses.Add(CourseViewModel.FromModel(course));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Данный файл не является корректным файлом педагогической нагрузки.");
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
