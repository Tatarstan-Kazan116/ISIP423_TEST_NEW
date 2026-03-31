using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace задание_тк
{
    public class WeightCalculator
    {
        // Константы диапазонов
        public const double MinHeight = 130;
        public const double MaxHeight = 220;
        public const double MinWeight = 40;
        public const double MaxWeight = 170;
        public const double Tolerance = 3.0; // Допуск +- 3 кг

        public enum Gender { Male, Female }

        /// <summary>
        /// Проверка валидности роста
        /// </summary>
        public bool IsValidHeight(double height)
        {
            return height >= MinHeight && height <= MaxHeight;
        }

        /// <summary>
        /// Проверка валидности веса
        /// </summary>
        public bool IsValidWeight(double weight)
        {
            return weight >= MinWeight && weight <= MaxWeight;
        }

        /// <summary>
        /// Расчет оптимального веса
        /// </summary>
        public double CalculateOptimalWeight(double height, Gender gender)
        {
            double baseWeight = height - 100;

            if (gender == Gender.Male)
            {
                // Мужчины: + 13%
                return baseWeight + (baseWeight * 0.13);
            }
            else
            {
                // Женщины: - 10%
                return baseWeight - (baseWeight * 0.10);
            }
        }

        /// <summary>
        /// Оценка текущего веса относительно оптимального
        /// </summary>
        public string GetAssessment(double actualWeight, double optimalWeight)
        {
            double difference = actualWeight - optimalWeight;

            if (Math.Abs(difference) <= Tolerance)
            {
                return "Норма";
            }
            else if (difference < -Tolerance)
            {
                return "Ниже нормы";
            }
            else
            {
                return "Выше нормы";
            }
        }
    }
}
