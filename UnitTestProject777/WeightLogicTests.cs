using Microsoft.VisualStudio.TestTools.UnitTesting;
using задание_тк;

namespace задание_тк
{
    [TestClass]
    public class WeightLogicTests
    {
        private WeightCalculator _calculator;

        [TestInitialize]
        public void Setup()
        {
            _calculator = new WeightCalculator();
        }

        #region Тесты валидации роста

        [TestMethod]
        public void IsValidHeight_ValidRange_ReturnsTrue()
        {
            // Arrange
            double validHeight = 175;

            // Act
            bool result = _calculator.IsValidHeight(validHeight);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsValidHeight_TooLow_ReturnsFalse()
        {
            Assert.IsFalse(_calculator.IsValidHeight(129));
            Assert.IsFalse(_calculator.IsValidHeight(100));
        }

        [TestMethod]
        public void IsValidHeight_TooHigh_ReturnsFalse()
        {
            Assert.IsFalse(_calculator.IsValidHeight(221));
            Assert.IsFalse(_calculator.IsValidHeight(250));
        }

        [TestMethod]
        public void IsValidHeight_BoundaryValues_WorksCorrectly()
        {
            Assert.IsTrue(_calculator.IsValidHeight(130)); // нижняя граница
            Assert.IsTrue(_calculator.IsValidHeight(220)); // верхняя граница
        }

        #endregion

        #region Тесты валидации веса

        [TestMethod]
        public void IsValidWeight_ValidRange_ReturnsTrue()
        {
            Assert.IsTrue(_calculator.IsValidWeight(70));
            Assert.IsTrue(_calculator.IsValidWeight(40));
            Assert.IsTrue(_calculator.IsValidWeight(170));
        }

        [TestMethod]
        public void IsValidWeight_InvalidRange_ReturnsFalse()
        {
            Assert.IsFalse(_calculator.IsValidWeight(39));
            Assert.IsFalse(_calculator.IsValidWeight(171));
        }

        [TestMethod]
        public void IsValidWeight_BoundaryValues_WorksCorrectly()
        {
            Assert.IsTrue(_calculator.IsValidWeight(40));  // нижняя граница
            Assert.IsTrue(_calculator.IsValidWeight(170)); // верхняя граница
        }

        #endregion

        #region Тесты расчета оптимального веса

        [TestMethod]
        public void CalculateOptimalWeight_Male_CorrectFormula()
        {
            // Arrange
            double height = 180;
            // Формула: (180 - 100) + 13% = 80 + 10.4 = 90.4
            double expected = 90.4;

            // Act
            double result = _calculator.CalculateOptimalWeight(height, WeightCalculator.Gender.Male);

            // Assert
            Assert.AreEqual(expected, result, 0.1);
        }

        [TestMethod]
        public void CalculateOptimalWeight_Female_CorrectFormula()
        {
            // Arrange
            double height = 170;
            // Формула: (170 - 100) - 10% = 70 - 7 = 63
            double expected = 63.0;

            // Act
            double result = _calculator.CalculateOptimalWeight(height, WeightCalculator.Gender.Female);

            // Assert
            Assert.AreEqual(expected, result, 0.1);
        }

        [TestMethod]
        public void CalculateOptimalWeight_Male_AnotherExample()
        {
            // Arrange
            double height = 200;
            // (200 - 100) + 13% = 100 + 13 = 113
            double expected = 113.0;

            // Act
            double result = _calculator.CalculateOptimalWeight(height, WeightCalculator.Gender.Male);

            // Assert
            Assert.AreEqual(expected, result, 0.1);
        }

        [TestMethod]
        public void CalculateOptimalWeight_Female_AnotherExample()
        {
            // Arrange
            double height = 160;
            // (160 - 100) - 10% = 60 - 6 = 54
            double expected = 54.0;

            // Act
            double result = _calculator.CalculateOptimalWeight(height, WeightCalculator.Gender.Female);

            // Assert
            Assert.AreEqual(expected, result, 0.1);
        }

        #endregion

        #region Тесты оценки веса

        [TestMethod]
        public void GetAssessment_ExactOptimal_ReturnsNorm()
        {
            // Arrange
            double optimal = 70.0;
            double actual = 70.0;

            // Act
            string result = _calculator.GetAssessment(actual, optimal);

            // Assert
            Assert.AreEqual("Норма", result);
        }

        [TestMethod]
        public void GetAssessment_WithinTolerance_ReturnsNorm()
        {
            // +2 кг (в пределах допуска 3 кг)
            Assert.AreEqual("Норма", _calculator.GetAssessment(72.0, 70.0));

            // -3 кг (ровно на границе допуска)
            Assert.AreEqual("Норма", _calculator.GetAssessment(67.0, 70.0));

            // 0 кг разница
            Assert.AreEqual("Норма", _calculator.GetAssessment(70.0, 70.0));
        }

        [TestMethod]
        public void GetAssessment_AboveTolerance_ReturnsAboveNorm()
        {
            // +4 кг (больше допуска 3 кг)
            string result = _calculator.GetAssessment(74.0, 70.0);
            Assert.AreEqual("Выше нормы", result);

            // +10 кг
            Assert.AreEqual("Выше нормы", _calculator.GetAssessment(80.0, 70.0));
        }

        [TestMethod]
        public void GetAssessment_BelowTolerance_ReturnsBelowNorm()
        {
            // -4 кг (больше допуска 3 кг в минус)
            string result = _calculator.GetAssessment(66.0, 70.0);
            Assert.AreEqual("Ниже нормы", result);

            // -10 кг
            Assert.AreEqual("Ниже нормы", _calculator.GetAssessment(60.0, 70.0));
        }

        [TestMethod]
        public void GetAssessment_EdgeCases_WorksCorrectly()
        {
            // Ровно +3 кг (граница нормы)
            Assert.AreEqual("Норма", _calculator.GetAssessment(73.0, 70.0));

            // Ровно -3 кг (граница нормы)
            Assert.AreEqual("Норма", _calculator.GetAssessment(67.0, 70.0));

            // +3.1 кг (уже выше нормы)
            Assert.AreEqual("Выше нормы", _calculator.GetAssessment(73.1, 70.0));

            // -3.1 кг (уже ниже нормы)
            Assert.AreEqual("Ниже нормы", _calculator.GetAssessment(66.9, 70.0));
        }

        #endregion

        #region Интеграционные тесты

        [TestMethod]
        public void FullCalculation_Male_NormalWeight()
        {
            // Arrange
            double height = 180;
            double actualWeight = 90; // близко к оптимальному 90.4

            // Act
            double optimal = _calculator.CalculateOptimalWeight(height, WeightCalculator.Gender.Male);
            string assessment = _calculator.GetAssessment(actualWeight, optimal);

            // Assert
            Assert.AreEqual(90.4, optimal, 0.1);
            Assert.AreEqual("Норма", assessment);
        }

        [TestMethod]
        public void FullCalculation_Female_Underweight()
        {
            // Arrange
            double height = 170;
            double actualWeight = 55; // меньше оптимального 63

            // Act
            double optimal = _calculator.CalculateOptimalWeight(height, WeightCalculator.Gender.Female);
            string assessment = _calculator.GetAssessment(actualWeight, optimal);

            // Assert
            Assert.AreEqual(63.0, optimal, 0.1);
            Assert.AreEqual("Ниже нормы", assessment);
        }

        [TestMethod]
        public void FullCalculation_Male_Overweight()
        {
            // Arrange
            double height = 175;
            double actualWeight = 100; // больше оптимального (75 + 13% = 84.75)

            // Act
            double optimal = _calculator.CalculateOptimalWeight(height, WeightCalculator.Gender.Male);
            string assessment = _calculator.GetAssessment(actualWeight, optimal);

            // Assert
            Assert.AreEqual(84.75, optimal, 0.1);
            Assert.AreEqual("Выше нормы", assessment);
        }

        #endregion
    }
}