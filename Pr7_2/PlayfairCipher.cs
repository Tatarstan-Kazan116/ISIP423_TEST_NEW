using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pr7_2
{
    /// <summary>
    /// Класс для шифрования и дешифрования методом Плейфера.
    /// Использует матрицу 5×5 для обработки биграмм.
    /// </summary>
    public class PlayfairCipher
    {
        public const int MatrixSize = 5;
        private const char FillerChar = 'X';
        private readonly char[,] _matrix;
        private readonly Dictionary<char, (int row, int col)> _charPositions;

        /// <summary>
        /// Инициализирует новый экземпляр шифра Плейфера с указанным ключом.
        /// </summary>
        /// <param name="key">Ключевое слово для формирования матрицы</param>
        /// <exception cref="ArgumentNullException">Если ключ равен null</exception>
        /// <exception cref="ArgumentException">Если ключ пустой</exception>
        public PlayfairCipher(string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be empty", nameof(key));

            _matrix = new char[MatrixSize, MatrixSize];
            _charPositions = new Dictionary<char, (int, int)>();
            BuildMatrix(key);
        }

        /// <summary>
        /// Формирует матрицу 5×5 из ключевого слова.
        /// </summary>
        /// <param name="key">Ключевое слово</param>
        private void BuildMatrix(string key)
        {
            var usedChars = new HashSet<char>();
            var matrixChars = new List<char>();

            // Добавляем символы ключа (без дубликатов, с обработкой J→I)
            foreach (char c in key.ToUpper())
            {
                if (!char.IsLetter(c)) continue;

                char normalized = (c == 'J') ? 'I' : c;

                if (!usedChars.Contains(normalized))
                {
                    usedChars.Add(normalized);
                    matrixChars.Add(normalized);
                }
            }

            // Добавляем остальные буквы алфавита (кроме J)
            for (char c = 'A'; c <= 'Z'; c++)
            {
                if (c == 'J') continue; // Пропускаем J, объединяем с I
                if (!usedChars.Contains(c))
                {
                    usedChars.Add(c);
                    matrixChars.Add(c);
                }
            }

            // Заполняем матрицу
            int index = 0;
            for (int i = 0; i < MatrixSize; i++)
            {
                for (int j = 0; j < MatrixSize; j++)
                {
                    _matrix[i, j] = matrixChars[index];
                    _charPositions[matrixChars[index]] = (i, j);
                    index++;
                }
            }
        }

        /// <summary>
        /// Шифрует открытый текст методом Плейфера.
        /// </summary>
        /// <param name="text">Текст для шифрования</param>
        /// <returns>Зашифрованный текст</returns>
        /// <exception cref="ArgumentNullException">Если текст равен null</exception>
        public string Encrypt(string text)
        {

            if (text == null)
                throw new ArgumentNullException(nameof(text));

            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Текст не может быть пустым", nameof(text));
            // Подготовка текста: удаление неалфавитных, замена J→I, верхний регистр
            string prepared = PrepareText(text);

            // Формирование биграмм с обработкой дубликатов и нечётной длины
            var digraphs = CreateDigraphs(prepared);

            // Шифрование каждой биграммы
            var result = new StringBuilder();
            foreach (var (a, b) in digraphs)
            {
                result.Append(EncryptPair(a, b));
            }

            return result.ToString();
        }

        /// <summary>
        /// Дешифрует зашифрованный текст методом Плейфера.
        /// </summary>
        /// <param name="cipher">Зашифрованный текст</param>
        /// <returns>Расшифрованный текст</returns>
        /// <exception cref="ArgumentNullException">Если текст равен null</exception>
        public string Decrypt(string cipher)
        {
            if (cipher == null)
                throw new ArgumentNullException(nameof(cipher));

            if (string.IsNullOrWhiteSpace(cipher))
                throw new ArgumentException("Шифротекст не может быть пустым", nameof(cipher));
            // Подготовка: только буквы, верхний регистр, замена J→I
            string prepared = new string(cipher.ToUpper()
                .Where(char.IsLetter)
                .Select(c => c == 'J' ? 'I' : c)
                .ToArray());

            // Формирование биграмм (без дополнительной обработки, т.к. шифротекст уже корректный)
            var digraphs = new List<(char, char)>();
            for (int i = 0; i < prepared.Length - 1; i += 2)
            {
                digraphs.Add((prepared[i], prepared[i + 1]));
            }

            // Дешифрование каждой биграммы
            var result = new StringBuilder();
            foreach (var (a, b) in digraphs)
            {
                result.Append(DecryptPair(a, b));
            }

            return result.ToString();
        }

        /// <summary>
        /// Подготавливает текст: оставляет только буквы, приводит к верхнему регистру, 
        /// заменяет J на I.
        /// </summary>
        private string PrepareText(string text)
        {
            return new string(text.ToUpper()
                .Where(char.IsLetter)
                .Select(c => c == 'J' ? 'I' : c)
                .ToArray());
        }

        /// <summary>
        /// Формирует список биграмм из текста с обработкой:
        /// - Вставка разделителя между одинаковыми буквами в паре
        /// - Добавление заполнителя при нечётной длине
        /// </summary>
        private List<(char, char)> CreateDigraphs(string text)
        {
            var digraphs = new List<(char, char)>();
            int i = 0;

            while (i < text.Length)
            {
                char first = text[i];

                // Если это последний символ или следующий такой же — добавляем заполнитель
                if (i + 1 >= text.Length)
                {
                    digraphs.Add((first, FillerChar));
                    i++;
                }
                else if (text[i] == text[i + 1])
                {
                    digraphs.Add((first, FillerChar));
                    i++;
                }
                else
                {
                    digraphs.Add((first, text[i + 1]));
                    i += 2;
                }
            }

            return digraphs;
        }

        /// <summary>
        /// Шифрует пару символов согласно правилам Плейфера.
        /// </summary>
        private string EncryptPair(char a, char b)
        {
            var (rowA, colA) = _charPositions[a];
            var (rowB, colB) = _charPositions[b];

            if (rowA == rowB)
            {
                // Одна строка: сдвиг вправо
                return $"{_matrix[rowA, (colA + 1) % MatrixSize]}{_matrix[rowB, (colB + 1) % MatrixSize]}";
            }
            else if (colA == colB)
            {
                // Один столбец: сдвиг вниз
                return $"{_matrix[(rowA + 1) % MatrixSize, colA]}{_matrix[(rowB + 1) % MatrixSize, colB]}";
            }
            else
            {
                // Прямоугольник: замена по углам
                return $"{_matrix[rowA, colB]}{_matrix[rowB, colA]}";
            }
        }

        /// <summary>
        /// Дешифрует пару символов (обратные операции шифрования).
        /// </summary>
        private string DecryptPair(char a, char b)
        {
            var (rowA, colA) = _charPositions[a];
            var (rowB, colB) = _charPositions[b];

            if (rowA == rowB)
            {
                // Одна строка: сдвиг влево
                return $"{_matrix[rowA, (colA - 1 + MatrixSize) % MatrixSize]}{_matrix[rowB, (colB - 1 + MatrixSize) % MatrixSize]}";
            }
            else if (colA == colB)
            {
                // Один столбец: сдвиг вверх
                return $"{_matrix[(rowA - 1 + MatrixSize) % MatrixSize, colA]}{_matrix[(rowB - 1 + MatrixSize) % MatrixSize, colB]}";
            }
            else
            {
                // Прямоугольник: та же операция, что при шифровании
                return $"{_matrix[rowA, colB]}{_matrix[rowB, colA]}";
            }
        }

        /// <summary>
        /// Возвращает матрицу 5×5 в виде строки для отладки.
        /// </summary>
        public string GetMatrixAsString()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < MatrixSize; i++)
            {
                for (int j = 0; j < MatrixSize; j++)
                {
                    sb.Append(_matrix[i, j]).Append(' ');
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public char[,] GetMatrix()
        {
            return _matrix;
        }
    }
}