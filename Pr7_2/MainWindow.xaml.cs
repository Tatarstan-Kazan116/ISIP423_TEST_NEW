using Pr7_2;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Pr7_2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// Основное окно приложения шифра Плейфера
    /// </summary>
    public partial class MainWindow : Window
    {
        private PlayfairCipher _cipher;

        public MainWindow()
        {
            InitializeComponent();
            UpdateStatus("Введите ключ и текст для начала работы");
        }

        /// <summary>
        /// Обработчик изменения ключа: валидация ввода
        /// </summary>
        private void KeyTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string key = KeyTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(key))
            {
                ShowError(KeyError, "Ключ не может быть пустым");
                _cipher = null;
                return;
            }

            if (!IsValidKey(key))
            {
                ShowError(KeyError, "Ключ должен содержать только буквы (A-Z)");
                _cipher = null;
                return;
            }

            HideError(KeyError);

            try
            {
                _cipher = new PlayfairCipher(key);
                UpdateStatus($"Ключ принят. Матрица {PlayfairCipher.MatrixSize}×{PlayfairCipher.MatrixSize} создана");
            }
            catch (Exception ex)
            {
                ShowError(KeyError, $"Ошибка: {ex.Message}");
                _cipher = null;
                UpdateStatus("Ошибка создания шифра");
            }
        }

        /// <summary>
        /// Валидация ключа: только латинские буквы
        /// </summary>
        private bool IsValidKey(string key)
        {
            foreach (char c in key.ToUpper())
            {
                if (!char.IsLetter(c) || c > 'Z') return false;
            }
            return true;
        }

        /// <summary>
        /// Шифрование текста
        /// </summary>
        private void EncryptButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            try
            {
                string result = _cipher.Encrypt(TextTextBox.Text);
                ResultTextBox.Text = result;
                UpdateStatus("Текст успешно зашифрован");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка шифрования: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                UpdateStatus("Ошибка при шифровании");
            }
        }

        /// <summary>
        /// Дешифрование текста
        /// </summary>
        private void DecryptButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            try
            {
                string result = _cipher.Decrypt(TextTextBox.Text);
                ResultTextBox.Text = result;
                UpdateStatus("Текст успешно расшифрован");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка дешифрования: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                UpdateStatus("Ошибка при дешифровании");
            }
        }

        /// <summary>
        /// Очистка всех полей
        /// </summary>
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            KeyTextBox.Clear();
            TextTextBox.Clear();
            ResultTextBox.Clear();
            HideError(KeyError);
            HideError(TextError);
            UpdateStatus("Поля очищены");
        }

        /// <summary>
        /// Валидация входных данных перед операцией
        /// </summary>
        private bool ValidateInput()
        {
            if (_cipher == null)
            {
                ShowError(KeyError, "Сначала введите корректный ключ");
                UpdateStatus("Ошибка: ключ не задан");
                return false;
            }

            if (string.IsNullOrWhiteSpace(TextTextBox.Text))
            {
                ShowError(TextError, "Введите текст для обработки");
                UpdateStatus("Ошибка: текст пустой");
                return false;
            }

            HideError(TextError);
            return true;
        }

        /// <summary>
        /// Отображение ошибки в указанном элементе
        /// </summary>
        private void ShowError(TextBlock errorBlock, string message)
        {
            errorBlock.Text = message;
            errorBlock.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Скрытие сообщения об ошибке
        /// </summary>
        private void HideError(TextBlock errorBlock)
        {
            errorBlock.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Обновление статуса в строке состояния
        /// </summary>
        private void UpdateStatus(string message)
        {
            StatusText.Text = message;
        }
    }
}