using TeacherPreferencesDataModel;
using TeacherPreferencesUI.Windows;
using TeacherPreferencesUI.Commands;
using TeacherPreferencesUI.UserControls;
using Microsoft.EntityFrameworkCore;
using TeacherPreferencesUI.ViewModels.SpecificPreferenceViewModels;
using static TeacherPreferences.TeacherPreferences;
using System.Windows;

namespace TeacherPreferencesUI.ViewModels
{
    public static class Commands
    {
        public static RelayCommand AddCommand(ApplicationViewModel viewModel)
        {
            return new RelayCommand((o) =>
            {
                TeacherViewModel newTeacher = new TeacherViewModel();

                AddTeacherWindow addTeacherWindow = new AddTeacherWindow(viewModel, newTeacher);

                if (addTeacherWindow.ShowDialog() == true)
                {
                    TeacherViewModel teacherViewModel = viewModel.EditingTeacher;
                    if (teacherViewModel != null)
                    {
                        TeacherPreferencesDataModel.Teacher teacher = teacherViewModel.ToModel();
                        viewModel.db.Teachers.Add(teacher);
                        viewModel.db.SaveChanges();

                        viewModel.Teachers.Add(TeacherViewModel.FromModel(teacher, viewModel));
                    }
                }
            });
        }

        public static RelayCommand DeleteCommand(ApplicationViewModel viewModel)
        {
            return new RelayCommand((o) =>
            {
                if (viewModel.SelectedTeacher != null)
                {
                    var trackedTeacher = viewModel.db.Teachers.Find(viewModel.SelectedTeacher.Id);
                    if (trackedTeacher != null)
                    {
                        viewModel.db.Teachers.Remove(trackedTeacher);
                        viewModel.db.SaveChanges();

                        viewModel.Teachers.Remove(viewModel.SelectedTeacher);
                    }
                    else
                    {
                        viewModel.ErrorMessage = "Teacher not found in database.";
                    }
                }
            });
        }

        public static RelayCommand ClearAllTeachersCommand(ApplicationViewModel viewModel)
        {
            return new RelayCommand((o) =>
            {
                if (MessageBox.Show("Вы уверены, что хотите очистить список преподавателей?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    viewModel.Teachers.Clear();
                    viewModel.db.Teachers.RemoveRange(viewModel.db.Teachers);
                    viewModel.db.SaveChanges();
                }
            });
        }


        public static RelayCommand SwitchToParsingResultCommand(ApplicationViewModel viewModel)
        {
            return new RelayCommand((o) =>
            {
                if (viewModel.ValidationResult != null)
                {
                    viewModel.CurrentView = new ParsingResultView();
                }
            });
        }

        public static RelayCommand ErrorBackCommand(ApplicationViewModel viewModel)
        {
            return new RelayCommand((o) =>
            {
                viewModel.CurrentView = new InitialView();
            });
        }

        public static RelayCommand BackCommand(ApplicationViewModel viewModel)
        {
            return new RelayCommand((o) =>
            {
                viewModel.SelectedTeacher = null;
            });
        }

        public static RelayCommand SavePreferencesCommand(ApplicationViewModel viewModel)
        {
            return new RelayCommand((o) =>
            {
                if (viewModel.SelectedTeacher != null)
                {
                    var trackedTeacher = viewModel.db.Teachers
                        .Include(t => t.GeneralPreferences)
                        .Include(t => t.Courses)
                        .Include(t => t.TimeSlots)
                        .FirstOrDefault(t => t.Id == viewModel.SelectedTeacher.Id);


                    if (trackedTeacher != null)
                    {
                        trackedTeacher.GeneralPreferences.Clear();
                        foreach (var generalPreferenceViewModel in viewModel.SelectedTeacher.GeneralPreferences)
                        {
                            trackedTeacher.GeneralPreferences.Add(generalPreferenceViewModel.ToModel());
                        }

                        trackedTeacher.Courses.Clear();
                        foreach (var course in viewModel.SelectedTeacher.Courses)
                        {
                            trackedTeacher.Courses.Add(course.ToModel());
                        }

                        trackedTeacher.TimeSlots.Clear();
                        foreach (var timeSlotViewModel in viewModel.SelectedTeacher.TimeSlots)
                        {
                            trackedTeacher.TimeSlots.Add(timeSlotViewModel.ToModel());
                        }

                        viewModel.db.SaveChanges();
                    }
                    else
                    {
                        viewModel.ErrorMessage = "Преподаватель не найден в базе.";
                    }
                }
                else
                {
                    viewModel.ErrorMessage = "Преподаватель не выбран.";
                }
            });
        }

        public static RelayCommand DeletePreferenceCommand(ApplicationViewModel viewModel)
        {
            return new RelayCommand((selectedPreference) =>
            {
                if (selectedPreference is GeneralPreferenceViewModel preferenceToDelete)
                {
                    viewModel.SelectedTeacher.GeneralPreferences.Remove(preferenceToDelete);

                    var modelPreference = viewModel.db.GeneralPreferences.Find(preferenceToDelete.Id);
                    if (modelPreference != null)
                    {
                        viewModel.db.GeneralPreferences.Remove(modelPreference);
                        viewModel.db.SaveChanges();
                    }
                }
            });
        }

        public static RelayCommand DeleteCourseCommand(ApplicationViewModel viewModel)
        {
            return new RelayCommand((selectedPreference) =>
            {
                if (selectedPreference is CourseViewModel preferenceToDelete)
                {
                    viewModel.SelectedTeacher.Courses.Remove(preferenceToDelete);

                    var modelPreference = viewModel.db.Courses.Find(preferenceToDelete.Id);
                    if (modelPreference != null)
                    {
                        viewModel.db.Courses.Remove(modelPreference);
                        viewModel.db.SaveChanges();
                    }
                }
            });
        }

        public static RelayCommand AddDefaultPreferenceCommand(ApplicationViewModel viewModel)
        {
            return new RelayCommand((o) =>
            {
                if (viewModel.SelectedTeacher != null)
                {
                    var defaultPreference = new GeneralPreferenceViewModel
                    {
                        TeacherId = viewModel.SelectedTeacher.Id,
                        PreferenceType = GeneralPreferenceType.MaxDaysPerWeek,
                        IntValue = 0,
                        Priority = TeacherPreferencesDataModel.Priority.Neutral
                    };

                    viewModel.SelectedTeacher.GeneralPreferences.Add(defaultPreference);

                    var modelPreference = defaultPreference.ToModel();

                    viewModel.db.GeneralPreferences.Add(modelPreference);
                    viewModel.db.SaveChanges();

                    viewModel.GeneralPreferences.Add(GeneralPreferenceViewModel.FromModel(modelPreference));
                }
                else
                {
                    viewModel.ErrorMessage = "Преподаватель не выбран.";
                }
            });
        }

        public static RelayCommand AddDefaultSpecificPreference(ApplicationViewModel viewModel)
        {
            return new RelayCommand((selectedCourse) =>
            {
                if (selectedCourse is CourseViewModel courseViewModel)
                {
                    var defaultSpecificPreference = new SpecificPreferenceViewModel
                    {
                        CourseId = courseViewModel.Id,
                        SpecificPreferenceType = SpecificPreferenceType.InOneDay,
                        Priority = TeacherPreferencesDataModel.Priority.Neutral
                    };

                    courseViewModel.SpecificPreferences.Add(defaultSpecificPreference);

                    var modelPreference = defaultSpecificPreference.ToModel();
                    viewModel.db.SpecificPreferences.Add(modelPreference);
                    viewModel.db.SaveChanges();

                    defaultSpecificPreference.Id = modelPreference.Id;
                }
                else
                {
                    viewModel.ErrorMessage = "Курс не выбран.";
                }
            });
        }

        public static RelayCommand AddCourseCommand(ApplicationViewModel viewModel)
        {
            return new RelayCommand((o) =>
            {
                if (viewModel.SelectedTeacher != null)
                {
                    var newCourse = new CourseViewModel()
                    {
                        TeacherId = viewModel.SelectedTeacher.Id,
                        SubjectName = "Новый курс",
                        Group = "Новая группа",
                        ClassType = TeacherPreferencesDataModel.ClassType.Lecture,
                    };

                    viewModel.Courses.Add(newCourse);
                    viewModel.SelectedTeacher.Courses.Add(newCourse);

                    var modelCourse = newCourse.ToModel();
                    viewModel.db.Courses.Add(modelCourse);
                    viewModel.db.SaveChanges();

                    newCourse.Id = modelCourse.Id;
                }
                else
                {
                    viewModel.ErrorMessage = "Преподаватель не выбран.";
                }
            });
        }

        public static RelayCommand AddAudienceEquipCommand(ApplicationViewModel viewModel)
        {
            return new RelayCommand((o) =>
            {
                if (o is CourseViewModel course)
                {
                    var newAudienceEquip = new AudienceEquipmentViewModel
                    {
                        IntValue = null,
                        EquipmentType = AudienceEquipmentType.Projector,
                        Priority = TeacherPreferencesDataModel.Priority.Neutral,
                        ParentCourse = course,
                        CourseId = course.Id
                    };

                    course.AudienceEquipments.Add(newAudienceEquip);

                    var modelEquip = newAudienceEquip.ToModel();
                    viewModel.db.AudienceEquipments.Add(modelEquip);
                    viewModel.db.SaveChanges();
                }
            });
        }

        public static RelayCommand AddAudienceCommand(ApplicationViewModel viewModel)
        {
            return new RelayCommand((o) =>
            {
                if (o is CourseViewModel course)
                {
                    var newAudience = new AudienceViewModel
                    {
                        Number = 0,
                        Priority = TeacherPreferencesDataModel.Priority.Neutral,
                        CourseId = course.Id,
                        ParentCourse = course
                    };

                    course.Audiences.Add(newAudience);

                    var modelAudience = newAudience.ToModel();
                    viewModel.db.Audiences.Add(modelAudience);
                    viewModel.db.SaveChanges();
                }
            });
        }

        public static RelayCommand DeleteAudienceCommand(ApplicationViewModel viewModel)
        {
            return new RelayCommand((o) =>
            {
                if (o is AudienceViewModel audienceToDelete)
                {
                    var specificPreference = audienceToDelete.ParentCourse;

                    specificPreference.Audiences.Remove(audienceToDelete);

                    var modelAudience = viewModel.db.Audiences.Find(audienceToDelete.Id);
                    if (modelAudience != null)
                    {
                        viewModel.db.Audiences.Remove(modelAudience);
                        viewModel.db.SaveChanges();
                    }
                }
            });
        }

        public static RelayCommand DeleteAudienceEquipCommand(ApplicationViewModel viewModel)
        {
            return new RelayCommand((o) =>
            {
                if (o is AudienceEquipmentViewModel audienceEquipmentToDelete)
                {
                    var specificPreference = audienceEquipmentToDelete.ParentCourse;

                    specificPreference.AudienceEquipments.Remove(audienceEquipmentToDelete);

                    var modelEquip = viewModel.db.AudienceEquipments.Find(audienceEquipmentToDelete.Id);
                    if (modelEquip != null)
                    {
                        viewModel.db.AudienceEquipments.Remove(modelEquip);
                        viewModel.db.SaveChanges();
                    }
                }
            });
        }

        public static RelayCommand DeleteSpecificPreferenceCommand(ApplicationViewModel viewModel)
        {
            return new RelayCommand((o) =>
            {
                if (o is SpecificPreferenceViewModel preferenceToDelete)
                {
                    var course = viewModel.SelectedTeacher.Courses.FirstOrDefault(c => c.SpecificPreferences.Contains(preferenceToDelete));
                    if (course != null)
                    {
                        course.SpecificPreferences.Remove(preferenceToDelete);

                        var modelPreference = viewModel.db.SpecificPreferences.Find(preferenceToDelete.Id);
                        if (modelPreference != null)
                        {
                            viewModel.db.SpecificPreferences.Remove(modelPreference);
                            viewModel.db.SaveChanges();
                        }
                    }
                }
                else
                {
                    viewModel.ErrorMessage = "Предпочтение не выбрано.";
                }
            });
        }

        public static RelayCommand AddTimeSlotCommand(ApplicationViewModel viewModel)
        {
            return new RelayCommand((o) =>
            {
                if (viewModel.SelectedTeacher != null)
                {
                    var newTimeSlot = new TimeSlotViewModel
                    {
                        DayOfWeek = TeacherPreferencesDataModel.DayOfWeek.AllWeek,
                        StartTime = new TimeSpan(9, 30, 0),
                        EndTime = new TimeSpan(18, 45, 0),
                        Priority = TeacherPreferencesDataModel.Priority.Neutral,
                        TeacherId = viewModel.SelectedTeacher.Id
                    };

                    viewModel.SelectedTeacher.TimeSlots.Add(newTimeSlot);

                    var modelTimeSlot = newTimeSlot.ToModel();
                    viewModel.db.TimeSlots.Add(modelTimeSlot);
                    viewModel.db.SaveChanges();

                    newTimeSlot.Id = modelTimeSlot.Id;
                }
                else
                {
                    viewModel.ErrorMessage = "Преподаватель не выбран.";
                }
            });
        }

        public static RelayCommand DeleteTimeSlotCommand(ApplicationViewModel viewModel)
        {
            return new RelayCommand((o) =>
            {
                if (o is TimeSlotViewModel timeSlotToDelete)
                {
                    if (viewModel.SelectedTeacher != null)
                    {
                        viewModel.SelectedTeacher.TimeSlots.Remove(timeSlotToDelete);

                        var modelTimeSlot = viewModel.db.TimeSlots.Find(timeSlotToDelete.Id);
                        if (modelTimeSlot != null)
                        {
                            viewModel.db.TimeSlots.Remove(modelTimeSlot);
                            viewModel.db.SaveChanges();
                        }
                    }
                    else
                    {
                        viewModel.ErrorMessage = "Предпочтение не найдено.";
                    }
                }
                else
                {
                    viewModel.ErrorMessage = "Не выбран преподаватель.";
                }
            });
        }
    }
}