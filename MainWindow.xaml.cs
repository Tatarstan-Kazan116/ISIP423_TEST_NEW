using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace задание_тк
{
    public partial class MainWindow : Window
    {
        // Создаем экземпляр класса логики для расчетов
        private readonly WeightCalculator _calculator = new WeightCalculator();

        public MainWindow()
        {
            InitializeComponent();
        }

        // Разрешаем ввод только цифр для поля роста
        private void TxtHeight_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        // Разрешаем ввод только цифр для поля веса
        private void TxtWeight_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        // Проверка: только цифры
        private bool IsTextAllowed(string text)
        {
            return Regex.IsMatch(text, @"^[0-9]*$");
        }

        private void BtnCalculate_Click(object sender, RoutedEventArgs e)
        {
            // Скрываем предыдущие ошибки
            txtHeightError.Visibility = Visibility.Collapsed;
            txtWeightError.Visibility = Visibility.Collapsed;

            // 1. Проверяем ввод роста (парсинг)
            if (!double.TryParse(txtHeight.Text, out double height))
            {
                txtHeightError.Text = "Пожалуйста, введите числовое значение роста";
                txtHeightError.Visibility = Visibility.Visible;
                return;
            }

            // 2. Проверяем диапазон роста через класс логики (130-220 см)
            if (!_calculator.IsValidHeight(height))
            {
                txtHeightError.Text = $"Рост должен быть в диапазоне {WeightCalculator.MinHeight}-{WeightCalculator.MaxHeight} см";
                txtHeightError.Visibility = Visibility.Visible;
                return;
            }

            // 3. Проверяем ввод веса (парсинг)
            if (!double.TryParse(txtWeight.Text, out double weight))
            {
                txtWeightError.Text = "Пожалуйста, введите числовое значение веса";
                txtWeightError.Visibility = Visibility.Visible;
                return;
            }

            // 4. Проверяем диапазон веса через класс логики (40-170 кг)
            if (!_calculator.IsValidWeight(weight))
            {
                txtWeightError.Text = $"Вес должен быть в диапазоне {WeightCalculator.MinWeight}-{WeightCalculator.MaxWeight} кг";
                txtWeightError.Visibility = Visibility.Visible;
                return;
            }

            // 5. Определяем пол
            WeightCalculator.Gender gender = rbMale.IsChecked == true
                ? WeightCalculator.Gender.Male
                : WeightCalculator.Gender.Female;

            // 6. Вычисляем оптимальный вес
            double optimalWeight = _calculator.CalculateOptimalWeight(height, gender);

            // 7. Определяем характеристику (с учетом допуска +- 3кг)
            string assessment = _calculator.GetAssessment(weight, optimalWeight);

            // 8. Выводим результаты
            txtOptimalWeight.Text = $"Оптимальный вес: {optimalWeight:F1} кг";
            txtAssessment.Text = $"Характеристика: {assessment}";
        }
    }
}
