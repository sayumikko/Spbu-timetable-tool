using System.Windows;
using TeacherPreferencesUI.ViewModels;

namespace TeacherPreferencesUI.Windows
{
    public partial class AddTeacherWindow : Window
    {
        public ApplicationViewModel AppViewModel { get; private set; }

        public AddTeacherWindow(ApplicationViewModel appViewModel, TeacherViewModel teacher)
        {
            InitializeComponent();
            this.AppViewModel = appViewModel;

            // Устанавливаем редактируемого преподавателя
            AppViewModel.EditingTeacher = teacher;
            DataContext = AppViewModel;
        }

        void Accept_Click(object sender, RoutedEventArgs e)
        {
            var teacher = AppViewModel.EditingTeacher;

            if (teacher == null ||
                string.IsNullOrWhiteSpace(teacher.Surname) ||
                string.IsNullOrWhiteSpace(teacher.Name) ||
                teacher.Department == null)
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля (Фамилия, Имя и Кафедра).", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DialogResult = true;
        }
    }
}
