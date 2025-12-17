using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ProgramLab
{
    public partial class Integral : Form
    {
        // Приватные поля для хранения данных
        private string _functionText = "";
        private double _a = 0;
        private double _b = 0;
        private double _e = 0.001;
        private int _decimalPlaces = 3;
        private IntegrationMethod _currentMethod = IntegrationMethod.Rectangle;

        // Перечисление для методов интегрирования
        private enum IntegrationMethod
        {
            Rectangle,
            Trapezoidal,
            Simpson
        }

        public Integral()
        {
            InitializeComponent();
            SetupChart();
            SetupComboBox();
        }

        private void SetupChart()
        {
            chart1.Series.Clear();

            // Настройка области графика
            ChartArea chartArea = chart1.ChartAreas[0];
            chartArea.AxisX.Crossing = 0;
            chartArea.AxisY.Crossing = 0;
            chartArea.AxisX.MajorGrid.LineColor = Color.LightGray;
            chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;
            chartArea.AxisX.Title = "X";
            chartArea.AxisY.Title = "Y";

            // Устанавливаем фиксированный масштаб
            chartArea.AxisX.Minimum = -10;
            chartArea.AxisX.Maximum = 10;
            chartArea.AxisY.Minimum = -10;
            chartArea.AxisY.Maximum = 10;

            AddAxes();
        }

        private void SetupComboBox()
        {
            comboBoxMethods.Items.Add("Метод прямоугольников");
            comboBoxMethods.Items.Add("Метод трапеций");
            comboBoxMethods.Items.Add("Метод Симпсона");
            comboBoxMethods.SelectedIndex = 0;
        }

        private void AddAxes()
        {
            // Удаляем старые оси если есть
            var seriesToRemove = chart1.Series
                .Where(s => s.Name == "XAxis" || s.Name == "YAxis")
                .ToList();
            foreach (var series in seriesToRemove)
            {
                chart1.Series.Remove(series);
            }

            // Ось X
            var xAxis = new Series("XAxis")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Black,
                BorderWidth = 1
            };
            xAxis.Points.AddXY(-1000, 0);
            xAxis.Points.AddXY(1000, 0);
            chart1.Series.Add(xAxis);

            // Ось Y
            var yAxis = new Series("YAxis")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Black,
                BorderWidth = 1
            };
            yAxis.Points.AddXY(0, -1000);
            yAxis.Points.AddXY(0, 1000);
            chart1.Series.Add(yAxis);
        }

        // Кнопка запуска вычислений
        private void buttonStart_Click(object sender, EventArgs e)
        {
            try
            {
                if (!GetInputData())
                    return;

                ClearChart();
                DrawFunction();
                DrawBoundaries();

                double result = CalculateIntegral();
                textBoxRes.Text = result.ToString($"F{_decimalPlaces}");

                // Автоматически устанавливаем масштаб для отображения функции
                AutoScaleChart();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AutoScaleChart()
        {
            try
            {
                var chartArea = chart1.ChartAreas[0];

                // Находим min и max значения функции на отрезке [A, B]
                double minY = double.MaxValue;
                double maxY = double.MinValue;
                int samples = 100;

                for (int i = 0; i <= samples; i++)
                {
                    double x = _a + i * (_b - _a) / samples;
                    try
                    {
                        double y = CalculateFunction(x);
                        if (!double.IsNaN(y) && !double.IsInfinity(y))
                        {
                            minY = Math.Min(minY, y);
                            maxY = Math.Max(maxY, y);
                        }
                    }
                    catch { }
                }

                // Если не нашли значений, используем стандартный диапазон
                if (minY > maxY)
                {
                    minY = -10;
                    maxY = 10;
                }

                // Добавляем отступы
                double xPadding = (_b - _a) * 0.1;
                double yPadding = Math.Max(Math.Abs(maxY - minY) * 0.1, 1);

                double xMin = Math.Min(_a - xPadding, -10);
                double xMax = Math.Max(_b + xPadding, 10);
                double yMin = Math.Min(minY - yPadding, -10);
                double yMax = Math.Max(maxY + yPadding, 10);

                // Устанавливаем масштаб
                chartArea.AxisX.Minimum = xMin;
                chartArea.AxisX.Maximum = xMax;
                chartArea.AxisY.Minimum = yMin;
                chartArea.AxisY.Maximum = yMax;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при авто-масштабировании: {ex.Message}");
            }
        }

        private bool GetInputData()
        {
            // Проверка функции
            if (string.IsNullOrWhiteSpace(textBoxF.Text))
            {
                MessageBox.Show("Введите функцию!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxF.Focus();
                return false;
            }

            _functionText = textBoxF.Text.Trim();

            // Проверка чисел
            if (!TryParseDouble(textBoxA.Text, out _a))
            {
                MessageBox.Show("Неверный формат числа A!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxA.Focus();
                return false;
            }

            if (!TryParseDouble(textBoxB.Text, out _b))
            {
                MessageBox.Show("Неверный формат числа B!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxB.Focus();
                return false;
            }

            // Проверка A < B
            if (_a >= _b)
            {
                MessageBox.Show("A должно быть меньше B!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Проверка точности
            if (!int.TryParse(textBoxE.Text, out _decimalPlaces) || _decimalPlaces <= 0)
            {
                MessageBox.Show("Точность должна быть положительным целым числом!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxE.Focus();
                return false;
            }

            _e = Math.Pow(10, -_decimalPlaces);

            // Получаем выбранный метод
            switch (comboBoxMethods.SelectedIndex)
            {
                case 0: _currentMethod = IntegrationMethod.Rectangle; break;
                case 1: _currentMethod = IntegrationMethod.Trapezoidal; break;
                case 2: _currentMethod = IntegrationMethod.Simpson; break;
            }

            return true;
        }

        private bool TryParseDouble(string text, out double result)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                result = 0;
                return false;
            }

            text = text.Replace(',', '.');
            return double.TryParse(text, NumberStyles.Any,
                CultureInfo.InvariantCulture, out result);
        }

        private double CalculateFunction(double x)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_functionText))
                    throw new Exception("Функция не задана");

                // Преобразуем функцию для вычисления
                string expression = _functionText.ToLower().Trim();

                // Заменяем константы
                expression = expression.Replace("pi", Math.PI.ToString(CultureInfo.InvariantCulture));
                expression = expression.Replace("e", Math.E.ToString(CultureInfo.InvariantCulture));

                // Заменяем x на значение, добавляя скобки для отрицательных чисел
                expression = ReplaceXWithValue(expression, x);

                // Заменяем ^ на ** для DataTable.Compute
                expression = expression.Replace("^", "**");

                // Заменяем математические функции
                expression = ReplaceMathFunctions(expression);

                // Вычисляем выражение
                return EvaluateExpressionWithDataTable(expression);
            }
            catch (DivideByZeroException)
            {
                return double.NaN;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка вычисления функции при x = {x}: {ex.Message}");
            }
        }

        // Замена переменной x с учетом отрицательных чисел
        private string ReplaceXWithValue(string expression, double x)
        {
            string xValue = x.ToString(CultureInfo.InvariantCulture);

            // Список операторов, перед которыми может стоять x
            char[] operators = { '+', '-', '*', '/', '^', '(', ')', ',', '=' };

            // Строим результат посимвольно
            string result = "";

            for (int i = 0; i < expression.Length; i++)
            {
                if (expression[i] == 'x' || expression[i] == 'X')
                {
                    // Проверяем, стоит ли перед x оператор
                    bool needsParentheses = x < 0;

                    if (i > 0)
                    {
                        char prevChar = expression[i - 1];
                        // Если перед x стоит цифра, буква или закрывающая скобка, нужно добавить *
                        if (char.IsDigit(prevChar) || char.IsLetter(prevChar) || prevChar == ')')
                        {
                            result += "*";
                        }
                    }

                    if (needsParentheses)
                    {
                        result += "(" + xValue + ")";
                    }
                    else
                    {
                        result += xValue;
                    }

                    // Проверяем, нужно ли добавить * после x
                    if (i < expression.Length - 1)
                    {
                        char nextChar = expression[i + 1];
                        if (char.IsDigit(nextChar) || char.IsLetter(nextChar) || nextChar == '(')
                        {
                            result += "*";
                        }
                    }
                }
                else
                {
                    result += expression[i];
                }
            }

            return result;
        }

        // Замена математических функций
        private string ReplaceMathFunctions(string expression)
        {
            var replacements = new Dictionary<string, string>
    {
        { "sin", "Sin" },
        { "cos", "Cos" },
        { "tan", "Tan" },
        { "tg", "Tan" },
        { "asin", "Asin" },
        { "acos", "Acos" },
        { "atan", "Atan" },
        { "sinh", "Sinh" },
        { "cosh", "Cosh" },
        { "tanh", "Tanh" },
        { "log", "Log" },
        { "ln", "Log" },
        { "lg", "Log10" },
        { "exp", "Exp" },
        { "sqrt", "Sqrt" },
        { "abs", "Abs" }
    };

            foreach (var replacement in replacements)
            {
                expression = Regex.Replace(expression, $@"\b{replacement.Key}\b", replacement.Value, RegexOptions.IgnoreCase);
            }

            return expression;
        }

        // Вычисление выражения с помощью DataTable
        private double EvaluateExpressionWithDataTable(string expression)
        {
            try
            {
                // Удаляем лишние пробелы
                expression = expression.Replace(" ", "");

                // Проверяем баланс скобок
                int balance = 0;
                foreach (char c in expression)
                {
                    if (c == '(') balance++;
                    if (c == ')') balance--;
                    if (balance < 0) throw new Exception("Несбалансированные скобки");
                }
                if (balance != 0) throw new Exception("Несбалансированные скобки");

                var dataTable = new System.Data.DataTable();
                object result = dataTable.Compute(expression, "");

                if (result == DBNull.Value)
                    throw new Exception("Результат вычисления не определен");

                return Convert.ToDouble(result);
            }
            catch (Exception ex)
            {
                // Альтернативный метод для сложных случаев
                return EvaluateWithCustomMethod(expression);
            }
        }

        // Альтернативный метод вычисления
        private double EvaluateWithCustomMethod(string expression)
        {
            // Пытаемся разобрать выражение вручную для простых случаев
            if (expression.Contains("**"))
            {
                // Обработка степеней
                var parts = expression.Split(new[] { "**" }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2)
                {
                    double baseVal = EvaluateSimpleNumber(parts[0]);
                    double expVal = EvaluateSimpleNumber(parts[1]);
                    return Math.Pow(baseVal, expVal);
                }
            }

            // Для простых арифметических выражений
            if (!expression.Contains("(") && !expression.Contains(")"))
            {
                try
                {
                    var dataTable = new System.Data.DataTable();
                    return Convert.ToDouble(dataTable.Compute(expression, ""));
                }
                catch
                {
                    throw new Exception($"Не удалось вычислить: {expression}");
                }
            }

            throw new Exception($"Слишком сложное выражение: {expression}");
        }

        // Вычисление простого числа или выражения без скобок
        private double EvaluateSimpleNumber(string expr)
        {
            expr = expr.Trim();

            if (double.TryParse(expr, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
            {
                return result;
            }

            try
            {
                var dataTable = new System.Data.DataTable();
                return Convert.ToDouble(dataTable.Compute(expr, ""));
            }
            catch
            {
                throw new Exception($"Не удалось вычислить: {expr}");
            }
        }

        // Препроцессинг выражения - заменяем все функции и операторы
        private string PreprocessExpression(string expression, double x)
        {
            expression = expression.Trim().ToLower();

            // Заменяем x на значение в скобках всегда
            string xValue = x.ToString(CultureInfo.InvariantCulture);
            if (x < 0)
            {
                xValue = "(" + xValue + ")";
            }

            // Простая замена x, но аккуратно
            expression = expression.Replace("x", xValue);

            // Заменяем константы
            expression = expression.Replace("pi", Math.PI.ToString(CultureInfo.InvariantCulture));
            expression = expression.Replace("e", Math.E.ToString(CultureInfo.InvariantCulture));

            // Заменяем ^ на ** для DataTable
            expression = expression.Replace("^", "**");

            // Простые замены функций
            expression = expression.Replace("sin(", "Sin(");
            expression = expression.Replace("cos(", "Cos(");
            expression = expression.Replace("tan(", "Tan(");
            expression = expression.Replace("exp(", "Exp(");
            expression = expression.Replace("log(", "Log(");
            expression = expression.Replace("ln(", "Log(");
            expression = expression.Replace("sqrt(", "Sqrt(");

            return expression;
        }

        // Замена переменной x на ее значение
        private string ReplaceVariableWithValue(string expression, string variable, string value)
        {
            // Регулярное выражение для поиска переменной x
            string pattern = @"(?<![a-zA-Z0-9_])" + variable + @"(?![a-zA-Z0-9_])";
            return Regex.Replace(expression, pattern, value);
        }

        // Обработка оператора возведения в степень ^
        private string ProcessPowerOperator(string expression)
        {
            // Заменяем ^ на ** для DataTable.Compute
            return expression.Replace("^", "**");
        }

        // Обработка математических функций
        private string ProcessMathFunctions(string expression)
        {
            // Список всех поддерживаемых функций и их замены для DataTable.Compute
            var functionReplacements = new Dictionary<string, string>
            {
                // Тригонометрические функции
                { @"sin\(", "Sin(" },
                { @"cos\(", "Cos(" },
                { @"tan\(", "Tan(" },
                { @"tg\(", "Tan(" },
                { @"ctg\(", "Cotan(" },
                
                // Обратные тригонометрические функции
                { @"asin\(", "Asin(" },
                { @"acos\(", "Acos(" },
                { @"atan\(", "Atan(" },
                
                // Гиперболические функции
                { @"sinh\(", "Sinh(" },
                { @"cosh\(", "Cosh(" },
                { @"tanh\(", "Tanh(" },
                
                // Логарифмические функции
                { @"log\(", "Log(" },      // натуральный логарифм
                { @"ln\(", "Log(" },       // натуральный логарифм
                { @"lg\(", "Log10(" },     // десятичный логарифм
                
                // Экспонента
                { @"exp\(", "Exp(" },
                
                // Квадратный корень
                { @"sqrt\(", "Sqrt(" },
                
                // Модуль
                { @"abs\(", "Abs(" }
            };

            // Применяем все замены
            foreach (var replacement in functionReplacements)
            {
                expression = Regex.Replace(expression, replacement.Key, replacement.Value, RegexOptions.IgnoreCase);
            }

            return expression;
        }

        // Вычисление математического выражения с использованием DataTable.Compute
        private double EvaluateMathExpression(string expression)
        {
            try
            {
                // Проверяем скобки
                if (!CheckParentheses(expression))
                {
                    throw new Exception("Несбалансированные скобки в выражении");
                }

                // Убираем лишние пробелы
                expression = expression.Replace(" ", "");

                // Используем DataTable.Compute для вычисления выражения
                var dataTable = new System.Data.DataTable();

                try
                {
                    var result = dataTable.Compute(expression, "");

                    if (result == DBNull.Value)
                        throw new Exception("Не удалось вычислить выражение");

                    return Convert.ToDouble(result);
                }
                catch (System.Data.EvaluateException ex)
                {
                    throw new Exception($"Ошибка вычисления: {ex.Message}");
                }
                catch (System.Data.SyntaxErrorException ex)
                {
                    throw new Exception($"Синтаксическая ошибка: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при вычислении выражения '{expression}': {ex.Message}");
            }
        }

        // Проверка баланса скобок
        private bool CheckParentheses(string expression)
        {
            int balance = 0;

            foreach (char c in expression)
            {
                if (c == '(')
                    balance++;
                else if (c == ')')
                    balance--;

                if (balance < 0)
                    return false;
            }

            return balance == 0;
        }

        private void DrawFunction()
        {
            var functionSeries = new Series("Function")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Blue,
                BorderWidth = 2
            };

            // Используем диапазон функции с небольшими отступами
            double displayMin = Math.Min(_a, _b) - 2;
            double displayMax = Math.Max(_a, _b) + 2;

            int pointsCount = 1000; // Фиксированное количество точек для гладкого графика
            double step = (displayMax - displayMin) / pointsCount;

            bool lastPointValid = false;
            double lastY = 0;

            for (int i = 0; i <= pointsCount; i++)
            {
                double x = displayMin + i * step;

                try
                {
                    double y = CalculateFunction(x);

                    if (!double.IsNaN(y) && !double.IsInfinity(y))
                    {
                        // Ограничиваем очень большие значения для отображения
                        if (y > 10000) y = 10000;
                        if (y < -10000) y = -10000;

                        functionSeries.Points.AddXY(x, y);

                        if (!lastPointValid)
                        {
                            functionSeries.Points.AddXY(x, y);
                        }

                        lastPointValid = true;
                        lastY = y;
                    }
                    else
                    {
                        if (lastPointValid)
                        {
                            functionSeries.Points.AddXY(x, lastY);
                            functionSeries.Points.AddXY(x, double.NaN);
                        }
                        lastPointValid = false;
                    }
                }
                catch
                {
                    if (lastPointValid)
                    {
                        functionSeries.Points.AddXY(x, lastY);
                        functionSeries.Points.AddXY(x, double.NaN);
                    }
                    lastPointValid = false;
                }
            }

            chart1.Series.Add(functionSeries);
        }

        private void DrawBoundaries()
        {
            // Левая граница
            var leftBoundary = new Series("LeftBoundary")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Red,
                BorderWidth = 2,
                BorderDashStyle = ChartDashStyle.Dash
            };

            // Находим min и max значение функции на отрезке
            double minY = double.MaxValue;
            double maxY = double.MinValue;
            int samples = 100;

            for (int i = 0; i <= samples; i++)
            {
                double x = _a + i * (_b - _a) / samples;
                try
                {
                    double y = CalculateFunction(x);
                    if (!double.IsNaN(y) && !double.IsInfinity(y))
                    {
                        minY = Math.Min(minY, y);
                        maxY = Math.Max(maxY, y);
                    }
                }
                catch { }
            }

            if (minY > maxY)
            {
                minY = -10;
                maxY = 10;
            }

            minY -= 1;
            maxY += 1;

            leftBoundary.Points.AddXY(_a, minY);
            leftBoundary.Points.AddXY(_a, maxY);
            chart1.Series.Add(leftBoundary);

            // Правая граница
            var rightBoundary = new Series("RightBoundary")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Green,
                BorderWidth = 2,
                BorderDashStyle = ChartDashStyle.Dash
            };

            rightBoundary.Points.AddXY(_b, minY);
            rightBoundary.Points.AddXY(_b, maxY);
            chart1.Series.Add(rightBoundary);
        }

        private double CalculateIntegral()
        {
            switch (_currentMethod)
            {
                case IntegrationMethod.Rectangle:
                    return RectangleMethod();
                case IntegrationMethod.Trapezoidal:
                    return TrapezoidalMethod();
                case IntegrationMethod.Simpson:
                    return SimpsonMethod();
                default:
                    return RectangleMethod();
            }
        }

        // Уменьшил начальное количество разбиений с 1000 до 100
        private double RectangleMethod()
        {
            int n = 100; // УМЕНЬШЕНО С 1000 ДО 100
            double previousResult = 0;
            double currentResult = 0;

            do
            {
                previousResult = currentResult;
                n *= 2;
                currentResult = 0;
                double h = (_b - _a) / n;

                for (int i = 0; i < n; i++)
                {
                    double x = _a + i * h + h / 2;
                    double y = CalculateFunction(x);

                    if (!double.IsNaN(y) && !double.IsInfinity(y))
                    {
                        currentResult += y * h;
                    }
                }
            } while (Math.Abs(currentResult - previousResult) > _e && n < 1000000);

            return currentResult;
        }

        private double TrapezoidalMethod()
        {
            int n = 100; // УМЕНЬШЕНО С 1000 ДО 100
            double previousResult = 0;
            double currentResult = 0;

            do
            {
                previousResult = currentResult;
                n *= 2;
                currentResult = 0;
                double h = (_b - _a) / n;

                for (int i = 0; i < n; i++)
                {
                    double x1 = _a + i * h;
                    double x2 = _a + (i + 1) * h;
                    double y1 = CalculateFunction(x1);
                    double y2 = CalculateFunction(x2);

                    if (!double.IsNaN(y1) && !double.IsInfinity(y1) &&
                        !double.IsNaN(y2) && !double.IsInfinity(y2))
                    {
                        currentResult += (y1 + y2) * h / 2;
                    }
                }
            } while (Math.Abs(currentResult - previousResult) > _e && n < 1000000);

            return currentResult;
        }

        private double SimpsonMethod()
        {
            // УМЕНЬШЕНО С 1000 ДО 100
            int n = 100;
            if (n % 2 != 0) n++;

            double previousResult = 0;
            double currentResult = 0;

            do
            {
                previousResult = currentResult;
                n += 2;
                currentResult = 0;
                double h = (_b - _a) / n;

                for (int i = 0; i <= n; i++)
                {
                    double x = _a + i * h;
                    double y = CalculateFunction(x);

                    if (double.IsNaN(y) || double.IsInfinity(y))
                    {
                        return double.NaN;
                    }

                    double coefficient = (i == 0 || i == n) ? 1 : (i % 2 == 0) ? 2 : 4;
                    currentResult += coefficient * y;
                }

                currentResult *= h / 3;
            } while (Math.Abs(currentResult - previousResult) > _e && n < 1000000);

            return currentResult;
        }

        private void ClearChart()
        {
            var seriesToRemove = chart1.Series
                .Where(s => s.Name != "XAxis" && s.Name != "YAxis")
                .ToList();

            foreach (var series in seriesToRemove)
            {
                chart1.Series.Remove(series);
            }
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            textBoxF.Clear();
            textBoxA.Clear();
            textBoxB.Clear();
            textBoxE.Clear();
            textBoxRes.Clear();

            ClearChart();
            AddAxes();

            // Сброс масштаба
            ResetChartScale();

            textBoxF.Focus();
        }

        private void ResetChartScale()
        {
            var chartArea = chart1.ChartAreas[0];
            chartArea.AxisX.Minimum = -10;
            chartArea.AxisX.Maximum = 10;
            chartArea.AxisY.Minimum = -10;
            chartArea.AxisY.Maximum = 10;
        }

        // Валидация ввода функции
        private void textBoxF_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;

            // Проверяем на кириллицу
            if ((c >= 'А' && c <= 'я') || c == 'ё' || c == 'Ё')
            {
                e.Handled = true;
                return;
            }

            // Разрешаем управляющие символы
            if (char.IsControl(c))
            {
                return;
            }

            // Разрешаемые символы для математических выражений
            string allowedSymbols = "xyzabcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789+-*/^()., ";

            if (!allowedSymbols.Contains(c))
            {
                e.Handled = true;
            }
        }

        // Валидация ввода чисел (A и B)
        private void textBoxNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            System.Windows.Forms.TextBox textBox = sender as System.Windows.Forms.TextBox;
            if (textBox == null) return;

            if (char.IsControl(e.KeyChar))
            {
                return;
            }

            string allowedChars = "0123456789-.,";

            if (textBox.SelectionStart == 0 && (e.KeyChar == '.' || e.KeyChar == ','))
            {
                e.Handled = true;
                return;
            }

            if (!allowedChars.Contains(e.KeyChar))
            {
                e.Handled = true;
                return;
            }

            if (e.KeyChar == '-' && textBox.SelectionStart != 0)
            {
                e.Handled = true;
                return;
            }

            if ((e.KeyChar == '.' || e.KeyChar == ',') &&
                (textBox.Text.Contains('.') || textBox.Text.Contains(',')))
            {
                if (textBox.SelectedText.Contains('.') || textBox.SelectedText.Contains(','))
                {
                    return;
                }
                e.Handled = true;
            }
        }

        // Валидация точности (только целые положительные)
        private void textBoxE_KeyPress(object sender, KeyPressEventArgs e)
        {
            System.Windows.Forms.TextBox textBox = sender as System.Windows.Forms.TextBox;

            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        // Автоматическая замена запятой на точку
        private void textBoxNumber_Leave(object sender, EventArgs e)
        {
            System.Windows.Forms.TextBox textBox = sender as System.Windows.Forms.TextBox;
            if (textBox != null && !string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = textBox.Text.Replace(',', '.');
            }
        }
    }
}