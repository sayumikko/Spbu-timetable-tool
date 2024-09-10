using System.Windows;
using TeacherPreferencesUI.ViewModels;
using TeacherPreferencesDataModel;

namespace TeacherPreferencesUI.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddTeacherWindow.xaml
    /// </summary>
    public partial class AddTeacherWindow : Window
    {
        public TeacherViewModel teacher { get; private set; }
        public AddTeacherWindow(TeacherViewModel teacher)
        {
            InitializeComponent();
            this.teacher = teacher;
            DataContext = this.teacher;
        }

        void Accept_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (TeacherViewModel)DataContext;

            if (string.IsNullOrWhiteSpace(viewModel.Surname) ||
                string.IsNullOrWhiteSpace(viewModel.Name) ||
                viewModel.Department == default)
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля (Фамилия, Имя и Кафедра).", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DialogResult = true;
        }
    }
}
