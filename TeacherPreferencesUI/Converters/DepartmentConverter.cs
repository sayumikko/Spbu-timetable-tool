using System.Globalization;
using System.Windows.Data;
using TeacherPreferencesDataModel;

namespace TeacherPreferencesUI.Converters
{
    public class DepartmentConverter : IValueConverter
    {
        public static string ConvertEnumToString(Department department)
        {
            return department switch
            {
                Department.None => "Не выбрано",
                Department.Astronomy => "Кафедра астрономии",
                Department.Astrophysics => "Кафедра астрофизики",
                Department.Algebra => "Кафедра высшей алгебры и теории чисел",
                Department.Geometry => "Кафедра высшей геометрии",
                Department.ComputationalMathematics => "Кафедра вычислительной математики",
                Department.DifferentialEquations => "Кафедра дифференциальных уравнений",
                Department.ForeignLanguages => "Кафедра иностранных языков",
                Department.Informatics => "Кафедра информатики",
                Department.InformationAndAnalyticalSystems => "Кафедра информационно-аналитических систем",
                Department.Hydroaeromechanics => "Кафедра гидроаэромеханики",
                Department.OperationsResearch => "Кафедра исследования операций",
                Department.MathematicalAnalysis => "Кафедра математического анализа",
                Department.MathematicalPhysics => "Кафедра математической физики",
                Department.CelestialMechanics => "Кафедра небесной механики",
                Department.ParallelAlgorithms => "Кафедра параллельных алгоритмов",
                Department.AppliedCybernetics => "Кафедра прикладной кибернетики",
                Department.SystemProgramming => "Кафедра системного программирования",
                Department.StatisticalModeling => "Кафедра статистического моделирования",
                Department.TheoreticalAndAppliedMechanics => "Кафедра теоретической и прикладной механики",
                Department.TheoreticalCybernetics => "Кафедра теоретической кибернетики",
                Department.ProbabilityTheoryAndMathematicalStatistics => "Кафедра теории вероятностей и математической статистики",
                Department.PhysicalMechanics => "Кафедра физической механики",
                Department.TheoryOfElasticity => "Кафедра теории упругости",
                _ => department.ToString()
            };
        }

        private static Department ConvertStringToEnum(string str)
        {
            return str switch
            {
                "Не выбрано" => Department.None,
                "Кафедра астрономии" => Department.Astronomy,
                "Кафедра астрофизики" => Department.Astrophysics,
                "Кафедра высшей алгебры и теории чисел" => Department.Algebra,
                "Кафедра высшей геометрии" => Department.Geometry,
                "Кафедра вычислительной математики" => Department.ComputationalMathematics,
                "Кафедра дифференциальных уравнений" => Department.DifferentialEquations,
                "Кафедра иностранных языков" => Department.ForeignLanguages,
                "Кафедра информатики" => Department.Informatics,
                "Кафедра информационно-аналитических систем" => Department.InformationAndAnalyticalSystems,
                "Кафедра гидроаэромеханики" => Department.Hydroaeromechanics,
                "Кафедра исследования операций" => Department.OperationsResearch,
                "Кафедра математического анализа" => Department.MathematicalAnalysis,
                "Кафедра математической физики" => Department.MathematicalPhysics,
                "Кафедра небесной механики" => Department.CelestialMechanics,
                "Кафедра параллельных алгоритмов" => Department.ParallelAlgorithms,
                "Кафедра прикладной кибернетики" => Department.AppliedCybernetics,
                "Кафедра системного программирования" => Department.SystemProgramming,
                "Кафедра статистического моделирования" => Department.StatisticalModeling,
                "Кафедра теоретической и прикладной механики" => Department.TheoreticalAndAppliedMechanics,
                "Кафедра теоретической кибернетики" => Department.TheoreticalCybernetics,
                "Кафедра теории вероятностей и математической статистики" => Department.ProbabilityTheoryAndMathematicalStatistics,
                "Кафедра физической механики" => Department.PhysicalMechanics,
                "Кафедра теории упругости" => Department.TheoryOfElasticity,
                _ => throw new ArgumentException("Unknown Department string")
            };
        }


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Department department)
            {
                return ConvertEnumToString(department);
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                return ConvertStringToEnum(str);
            }
            return Department.None;
        }
    }
}
