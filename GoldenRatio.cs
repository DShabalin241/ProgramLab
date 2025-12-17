using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace ProgramLab
{
    public partial class GoldenRatio : Form
    {
        private double PHI = (1 + Math.Sqrt(5)) / 2;
        private const double MAX_Y_VALUE = 10;
        private const double ZERO_TOLERANCE = 1e-10;
        public GoldenRatio()
        {
            InitializeComponent();
            SetupChart();      // Настраиваем график при запуске
            SetupEventHandlers(); // Подключаем обработчики событий
        }
        private readonly List<string> NO_EXTREMUM_FUNCTIONS = new List<string>
        {
            "tan", "ctg", "exp", "log", "ln", "1/x", "x^-1", "e^x"
        };

        private void SetupChart()
        {
            chartFunc.Series.Clear();

            Series functionSeries = new Series("Функция")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Blue,
                BorderWidth = 2
            };
            chartFunc.Series.Add(functionSeries);

            Series intervalSeries = new Series("Интервал")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Red,
                BorderWidth = 2,
                BorderDashStyle = ChartDashStyle.Dash
            };
            chartFunc.Series.Add(intervalSeries);

            Series minSeries = new Series("Минимум")
            {
                ChartType = SeriesChartType.Point,
                Color = Color.Red,
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 10
            };
            chartFunc.Series.Add(minSeries);

            Series maxSeries = new Series("Максимум")
            {
                ChartType = SeriesChartType.Point,
                Color = Color.Green,
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 10
            };
            chartFunc.Series.Add(maxSeries);

            Series xAxisSeries = new Series("Ось X (y=0)")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.DarkGray,
                BorderWidth = 1,
                BorderDashStyle = ChartDashStyle.Dash
            };
            chartFunc.Series.Add(xAxisSeries);

            Series yAxisSeries = new Series("Ось Y (x=0)")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.DarkGray,
                BorderWidth = 1,
                BorderDashStyle = ChartDashStyle.Dash
            };
            chartFunc.Series.Add(yAxisSeries);

            chartFunc.ChartAreas[0].AxisX.Title = "X";
            chartFunc.ChartAreas[0].AxisY.Title = "Y";
            chartFunc.ChartAreas[0].AxisX.Minimum = -10;
            chartFunc.ChartAreas[0].AxisX.Maximum = 10;
            chartFunc.ChartAreas[0].AxisY.Minimum = -MAX_Y_VALUE;
            chartFunc.ChartAreas[0].AxisY.Maximum = MAX_Y_VALUE;
        }

        private void SetupEventHandlers()
        {
            textBoxA.KeyPress += TextBoxNumber_KeyPress;
            textBoxB.KeyPress += TextBoxNumber_KeyPress;
            textBoxE.KeyPress += TextBoxEpsilon_KeyPress;
            textBoxF.KeyPress += TextBoxFunction_KeyPress;

            buttonStart.Click += ButtonStart_Click;
            buttonChart.Click += ButtonChart_Click;
            buttonClear.Click += ButtonClear_Click;
        }

        // Метод для проверки и отрисовки точки экстремума в координатах (0,0)
        private void DrawZeroZeroExtremumPoint()
        {
            // Очищаем серии точек экстремумов перед добавлением новых точек
            chartFunc.Series["Минимум"].Points.Clear();
            chartFunc.Series["Максимум"].Points.Clear();

            // Проверяем точку МИНИМУМА
            if (!string.IsNullOrWhiteSpace(textBoxXMin.Text) &&
                !string.IsNullOrWhiteSpace(textBoxYMin.Text) &&
                textBoxXMin.Text != "нет" && textBoxYMin.Text != "нет")
            {
                // Парсим координаты из текстовых полей
                if (double.TryParse(textBoxXMin.Text.Replace(",", "."), NumberStyles.Any,
                    CultureInfo.InvariantCulture, out double minX) &&
                    double.TryParse(textBoxYMin.Text.Replace(",", "."), NumberStyles.Any,
                    CultureInfo.InvariantCulture, out double minY))
                {
                    // Проверяем, являются ли координаты (0,0) с учетом погрешности
                    if (Math.Abs(minX) < ZERO_TOLERANCE && Math.Abs(minY) < ZERO_TOLERANCE)
                    {
                        // Рисуем точку минимума точно в (0,0)
                        chartFunc.Series["Минимум"].Points.AddXY(0.0, 0.0);

                        // Устанавливаем настройки точки
                        chartFunc.Series["Минимум"].MarkerStyle = MarkerStyle.Circle;
                        chartFunc.Series["Минимум"].MarkerSize = 10;
                        chartFunc.Series["Минимум"].Color = Color.Red;
                    }
                    else
                    {
                        // Для не-нулевых координат рисуем точку с обычными настройками
                        chartFunc.Series["Минимум"].Points.AddXY(minX, minY);
                        chartFunc.Series["Минимум"].MarkerStyle = MarkerStyle.Circle;
                        chartFunc.Series["Минимум"].MarkerSize = 10;
                        chartFunc.Series["Минимум"].Color = Color.Red;
                    }
                }
            }

            // Проверяем точку МАКСИМУМА
            if (!string.IsNullOrWhiteSpace(textBoxXMax.Text) &&
                !string.IsNullOrWhiteSpace(textBoxYMax.Text) &&
                textBoxXMax.Text != "нет" && textBoxYMax.Text != "нет")
            {
                // Парсим координаты из текстовых полей
                if (double.TryParse(textBoxXMax.Text.Replace(",", "."), NumberStyles.Any,
                    CultureInfo.InvariantCulture, out double maxX) &&
                    double.TryParse(textBoxYMax.Text.Replace(",", "."), NumberStyles.Any,
                    CultureInfo.InvariantCulture, out double maxY))
                {
                    // Проверяем, являются ли координаты (0,0) с учетом погрешности
                    if (Math.Abs(maxX) < ZERO_TOLERANCE && Math.Abs(maxY) < ZERO_TOLERANCE)
                    {
                        // Рисуем точку максимума точно в (0,0)
                        chartFunc.Series["Максимум"].Points.AddXY(0.0, 0.0);

                        // Устанавливаем настройки точки
                        chartFunc.Series["Максимум"].MarkerStyle = MarkerStyle.Circle;
                        chartFunc.Series["Максимум"].MarkerSize = 10;
                        chartFunc.Series["Максимум"].Color = Color.Green;
                    }
                    else
                    {
                        // Для не-нулевых координат рисуем точку с обычными настройками
                        chartFunc.Series["Максимум"].Points.AddXY(maxX, maxY);
                        chartFunc.Series["Максимум"].MarkerStyle = MarkerStyle.Circle;
                        chartFunc.Series["Максимум"].MarkerSize = 10;
                        chartFunc.Series["Максимум"].Color = Color.Green;
                    }
                }
            }
        }

        private bool IsFunctionWithoutExtremums(string function)
        {
            string funcLower = function.ToLower().Replace(" ", "");

            foreach (string pattern in NO_EXTREMUM_FUNCTIONS)
            {
                if (funcLower.Contains(pattern))
                {
                    return true;
                }
            }

            if (Regex.IsMatch(funcLower, @"1\s*/\s*x") || Regex.IsMatch(funcLower, @"1\s*/\s*\(x"))
            {
                return true;
            }

            if (funcLower.Contains("e^") && funcLower.Contains("x"))
            {
                return true;
            }

            return false;
        }

        private double EvaluateFunction(string function, double x)
        {
            try
            {
                string expression = function.ToLower();

                expression = Regex.Replace(expression, @"ctg\s*\(\s*(.*?)\s*\)", match =>
                {
                    string arg = match.Groups[1].Value;
                    return $"(1/tan({arg}))";
                });

                expression = expression
                    .Replace("pi", Math.PI.ToString(CultureInfo.InvariantCulture))
                    .Replace("e", Math.E.ToString(CultureInfo.InvariantCulture));

                expression = ReplaceXInExpression(expression, x);

                expression = Regex.Replace(expression, @"sin\((.*?)\)", match =>
                {
                    double arg = EvaluateSimpleExpression(match.Groups[1].Value);
                    return Math.Sin(arg).ToString(CultureInfo.InvariantCulture);
                });

                expression = Regex.Replace(expression, @"cos\((.*?)\)", match =>
                {
                    double arg = EvaluateSimpleExpression(match.Groups[1].Value);
                    return Math.Cos(arg).ToString(CultureInfo.InvariantCulture);
                });

                expression = Regex.Replace(expression, @"tan\((.*?)\)", match =>
                {
                    double arg = EvaluateSimpleExpression(match.Groups[1].Value);
                    double cos = Math.Cos(arg);
                    if (Math.Abs(cos) < 1e-15)
                        throw new DivideByZeroException("Тангенс не определен");
                    return Math.Tan(arg).ToString(CultureInfo.InvariantCulture);
                });

                expression = Regex.Replace(expression, @"exp\((.*?)\)", match =>
                {
                    double arg = EvaluateSimpleExpression(match.Groups[1].Value);
                    return Math.Exp(arg).ToString(CultureInfo.InvariantCulture);
                });

                expression = Regex.Replace(expression, @"sqrt\((.*?)\)", match =>
                {
                    double arg = EvaluateSimpleExpression(match.Groups[1].Value);
                    if (arg < 0)
                        throw new ArgumentException("Корень из отрицательного числа");
                    return Math.Sqrt(arg).ToString(CultureInfo.InvariantCulture);
                });

                expression = Regex.Replace(expression, @"log\((.*?),(.*?)\)", match =>
                {
                    double num = EvaluateSimpleExpression(match.Groups[1].Value);
                    double baseVal = EvaluateSimpleExpression(match.Groups[2].Value);
                    if (num <= 0 || baseVal <= 0 || baseVal == 1)
                        throw new ArgumentException("Логарифм не определен");
                    return Math.Log(num, baseVal).ToString(CultureInfo.InvariantCulture);
                });

                expression = Regex.Replace(expression, @"ln\((.*?)\)", match =>
                {
                    double arg = EvaluateSimpleExpression(match.Groups[1].Value);
                    if (arg <= 0)
                        throw new ArgumentException("Натуральный логарифм не определен");
                    return Math.Log(arg).ToString(CultureInfo.InvariantCulture);
                });

                expression = ProcessPowers(expression);

                return EvaluateSimpleExpression(expression);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка вычисления функции: {ex.Message}");
            }
        }

        private string ReplaceXInExpression(string expression, double x)
        {
            expression = Regex.Replace(expression, @"x\^(\d+(?:\.\d+)?)", match =>
            {
                double exponent = double.Parse(match.Groups[1].Value);
                return Math.Pow(x, exponent).ToString(CultureInfo.InvariantCulture);
            });

            expression = expression.Replace("x", x.ToString(CultureInfo.InvariantCulture));

            return expression;
        }

        private string ProcessPowers(string expression)
        {
            var matches = Regex.Matches(expression, @"(\d+(?:\.\d+)?|\([^)]+\))\^(\d+(?:\.\d+)?|\([^)]+\))");

            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    string baseStr = match.Groups[1].Value;
                    string expStr = match.Groups[2].Value;

                    if (baseStr.StartsWith("(") && baseStr.EndsWith(")"))
                        baseStr = baseStr.Substring(1, baseStr.Length - 2);
                    if (expStr.StartsWith("(") && expStr.EndsWith(")"))
                        expStr = expStr.Substring(1, expStr.Length - 2);

                    double baseVal = EvaluateSimpleExpression(baseStr);
                    double exponent = EvaluateSimpleExpression(expStr);

                    expression = expression.Replace(match.Value,
                        Math.Pow(baseVal, exponent).ToString(CultureInfo.InvariantCulture));
                }
            }

            return expression;
        }

        private double EvaluateSimpleExpression(string expression)
        {
            DataTable table = new DataTable();
            expression = expression.Replace(",", ".");
            table.Columns.Add("expression", typeof(string), expression);
            DataRow row = table.NewRow();
            table.Rows.Add(row);
            return double.Parse((string)row["expression"]);
        }

        private (double x, double y, bool found) GoldenSectionSearchExtremum(
            string function,
            double a,
            double b,
            double epsilon,
            bool findMin)
        {
            try
            {
                int maxIterations = 1000;
                int iteration = 0;
                double left = a;
                double right = b;

                while (Math.Abs(right - left) > epsilon && iteration < maxIterations)
                {
                    double x1 = right - (right - left) / PHI;
                    double x2 = left + (right - left) / PHI;

                    double f1 = EvaluateFunction(function, x1);
                    double f2 = EvaluateFunction(function, x2);

                    if (findMin)
                    {
                        if (f1 <= f2)
                            right = x2;
                        else
                            left = x1;
                    }
                    else
                    {
                        if (f1 >= f2)
                            right = x2;
                        else
                            left = x1;
                    }

                    iteration++;
                }

                double x = (left + right) / 2;
                double y = EvaluateFunction(function, x);

                return (x, y, true);
            }
            catch
            {
                return (0, 0, false);
            }
        }

        private void FindExtremums(string function, double a, double b, int precision)
        {
            if (IsFunctionWithoutExtremums(function))
            {
                textBoxXMin.Text = "нет";
                textBoxYMin.Text = "нет";
                textBoxXMax.Text = "нет";
                textBoxYMax.Text = "нет";

                MessageBox.Show("Для данной функции (тангенс, котангенс, гипербола, экспонента, логарифм) экстремумов не существует.",
                              "Информация",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Information);
                return;
            }

            double epsilon = Math.Pow(10, -precision);

            double minX = double.NaN, minY = double.NaN;
            double maxX = double.NaN, maxY = double.NaN;
            bool minFound = false;
            bool maxFound = false;

            try
            {
                double fa = EvaluateFunction(function, a);
                double fb = EvaluateFunction(function, b);

                var minResult = GoldenSectionSearchExtremum(function, a, b, epsilon, true);
                if (minResult.found)
                {
                    double testDelta = (b - a) / 1000;
                    double leftVal = EvaluateFunction(function, minResult.x - testDelta);
                    double rightVal = EvaluateFunction(function, minResult.x + testDelta);

                    if (minResult.y < fa && minResult.y < fb &&
                        minResult.y < leftVal && minResult.y < rightVal)
                    {
                        minX = minResult.x;
                        minY = minResult.y;
                        minFound = true;
                    }
                }

                var maxResult = GoldenSectionSearchExtremum(function, a, b, epsilon, false);
                if (maxResult.found)
                {
                    double testDelta = (b - a) / 1000;
                    double leftVal = EvaluateFunction(function, maxResult.x - testDelta);
                    double rightVal = EvaluateFunction(function, maxResult.x + testDelta);

                    if (maxResult.y > fa && maxResult.y > fb &&
                        maxResult.y > leftVal && maxResult.y > rightVal)
                    {
                        maxX = maxResult.x;
                        maxY = maxResult.y;
                        maxFound = true;
                    }
                }

                if (!minFound && !maxFound)
                {
                    double mid = (a + b) / 2;
                    double fMid = EvaluateFunction(function, mid);

                    if (fa < fMid && fMid < fb)
                    {
                        minX = a; minY = fa; minFound = true;
                        maxX = b; maxY = fb; maxFound = true;
                    }
                    else if (fa > fMid && fMid > fb)
                    {
                        minX = b; minY = fb; minFound = true;
                        maxX = a; maxY = fa; maxFound = true;
                    }
                }

                if (minFound)
                {
                    textBoxXMin.Text = minX.ToString($"F{precision}");
                    textBoxYMin.Text = minY.ToString($"F{precision}");
                }
                else
                {
                    textBoxXMin.Text = "нет";
                    textBoxYMin.Text = "нет";
                }

                if (maxFound)
                {
                    textBoxXMax.Text = maxX.ToString($"F{precision}");
                    textBoxYMax.Text = maxY.ToString($"F{precision}");
                }
                else
                {
                    textBoxXMax.Text = "нет";
                    textBoxYMax.Text = "нет";
                }

                if (!minFound && !maxFound)
                {
                    MessageBox.Show("На заданном интервале не найдено экстремумов функции",
                                  "Информация",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                textBoxXMin.Text = "нет";
                textBoxYMin.Text = "нет";
                textBoxXMax.Text = "нет";
                textBoxYMax.Text = "нет";

                MessageBox.Show($"Ошибка при поиске экстремумов: {ex.Message}",
                              "Ошибка",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
        }

        private void PlotFunction(string function, double a, double b)
        {
            foreach (Series series in chartFunc.Series)
            {
                series.Points.Clear();
            }

            double xMin = Math.Min(a, b) - Math.Abs(b - a) * 0.2;
            double xMax = Math.Max(a, b) + Math.Abs(b - a) * 0.2;

            chartFunc.ChartAreas[0].AxisX.Minimum = xMin;
            chartFunc.ChartAreas[0].AxisX.Maximum = xMax;

            int pointsCount = 1000;
            double step = (b - a) / pointsCount;

            List<double> validYValues = new List<double>();

            bool isUnboundedFunction = IsFunctionWithoutExtremums(function);

            for (int i = 0; i <= pointsCount; i++)
            {
                double x = a + i * step;

                try
                {
                    double y = EvaluateFunction(function, x);

                    if (double.IsInfinity(y) || double.IsNaN(y))
                    {
                        chartFunc.Series["Функция"].Points.AddXY(x, double.NaN);
                    }
                    else if (isUnboundedFunction && Math.Abs(y) > MAX_Y_VALUE)
                    {
                        chartFunc.Series["Функция"].Points.AddXY(x, double.NaN);
                    }
                    else
                    {
                        chartFunc.Series["Функция"].Points.AddXY(x, y);
                        validYValues.Add(y);
                    }
                }
                catch
                {
                    chartFunc.Series["Функция"].Points.AddXY(x, double.NaN);
                }
            }

            if (validYValues.Count > 0)
            {
                double yMin = validYValues.Min();
                double yMax = validYValues.Max();
                double yRange = yMax - yMin;

                if (yRange < 1e-10)
                {
                    yMin -= 1;
                    yMax += 1;
                    yRange = 2;
                }

                chartFunc.ChartAreas[0].AxisY.Minimum = yMin - yRange * 0.1;
                chartFunc.ChartAreas[0].AxisY.Maximum = yMax + yRange * 0.1;

                if (isUnboundedFunction)
                {
                    chartFunc.ChartAreas[0].AxisY.Minimum = Math.Max(chartFunc.ChartAreas[0].AxisY.Minimum, -MAX_Y_VALUE);
                    chartFunc.ChartAreas[0].AxisY.Maximum = Math.Min(chartFunc.ChartAreas[0].AxisY.Maximum, MAX_Y_VALUE);
                }

                double lineY1 = chartFunc.ChartAreas[0].AxisY.Minimum;
                double lineY2 = chartFunc.ChartAreas[0].AxisY.Maximum;

                chartFunc.Series["Интервал"].Points.AddXY(a, lineY1);
                chartFunc.Series["Интервал"].Points.AddXY(a, lineY2);
                chartFunc.Series["Интервал"].Points.AddXY(double.NaN, double.NaN);

                chartFunc.Series["Интервал"].Points.AddXY(b, lineY1);
                chartFunc.Series["Интервал"].Points.AddXY(b, lineY2);
            }

            // Рисуем оси координат (0,0)
            if (chartFunc.ChartAreas[0].AxisY.Minimum <= 0 &&
                chartFunc.ChartAreas[0].AxisY.Maximum >= 0)
            {
                chartFunc.Series["Ось X (y=0)"].Points.AddXY(xMin, 0);
                chartFunc.Series["Ось X (y=0)"].Points.AddXY(xMax, 0);
            }

            if (chartFunc.ChartAreas[0].AxisX.Minimum <= 0 &&
                chartFunc.ChartAreas[0].AxisX.Maximum >= 0)
            {
                double yMin = chartFunc.ChartAreas[0].AxisY.Minimum;
                double yMax = chartFunc.ChartAreas[0].AxisY.Maximum;
                chartFunc.Series["Ось Y (x=0)"].Points.AddXY(0, yMin);
                chartFunc.Series["Ось Y (x=0)"].Points.AddXY(0, yMax);
            }

            // ========== ВЫЗОВ МЕТОДА ДЛЯ ОТРИСОВКИ ТОЧЕК ЭКСТРЕМУМОВ ==========
            // Этот метод проверяет текстовые поля и рисует точки минимума и максимума
            // Включая специальную обработку для точки (0,0)
            DrawZeroZeroExtremumPoint();

            chartFunc.Invalidate();
        }

        private void TextBoxNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string currentText = textBox.Text;
            int selectionStart = textBox.SelectionStart;

            if (char.IsControl(e.KeyChar))
            {
                e.Handled = false;
                return;
            }

            if (e.KeyChar == '-' && selectionStart == 0 && !currentText.Contains("-"))
            {
                e.Handled = false;
                return;
            }

            if (e.KeyChar == '.' || e.KeyChar == ',')
            {
                if (!currentText.Contains('.') && !currentText.Contains(','))
                {
                    e.KeyChar = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
                    e.Handled = false;
                }
                else
                {
                    e.Handled = true;
                }
                return;
            }

            if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
                return;
            }

            e.Handled = false;
        }

        private void TextBoxEpsilon_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void TextBoxFunction_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= 'А' && e.KeyChar <= 'Я') ||
                (e.KeyChar >= 'а' && e.KeyChar <= 'я') ||
                e.KeyChar == 'ё' || e.KeyChar == 'Ё')
            {
                e.Handled = true;
                return;
            }

            e.Handled = false;
        }

        private void ButtonStart_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(textBoxF.Text))
                {
                    MessageBox.Show("Введите функцию", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!double.TryParse(textBoxA.Text.Replace(",", "."), NumberStyles.Any,
                    CultureInfo.InvariantCulture, out double a))
                {
                    MessageBox.Show("Некорректное значение A", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!double.TryParse(textBoxB.Text.Replace(",", "."), NumberStyles.Any,
                    CultureInfo.InvariantCulture, out double b))
                {
                    MessageBox.Show("Некорректное значение B", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!int.TryParse(textBoxE.Text, out int precision) || precision < 0)
                {
                    MessageBox.Show("Точность должна быть целым положительным числом",
                                  "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (a >= b)
                {
                    MessageBox.Show("A должно быть меньше B", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                FindExtremums(textBoxF.Text, a, b, precision);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ButtonChart_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(textBoxF.Text))
                {
                    MessageBox.Show("Введите функцию", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!double.TryParse(textBoxA.Text.Replace(",", "."), NumberStyles.Any,
                    CultureInfo.InvariantCulture, out double a))
                {
                    MessageBox.Show("Некорректное значение A", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!double.TryParse(textBoxB.Text.Replace(",", "."), NumberStyles.Any,
                    CultureInfo.InvariantCulture, out double b))
                {
                    MessageBox.Show("Некорректное значение B", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (a >= b)
                {
                    MessageBox.Show("A должно быть меньше B", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                PlotFunction(textBoxF.Text, a, b);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при построении графика: {ex.Message}",
                              "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ButtonClear_Click(object sender, EventArgs e)
        {
            textBoxF.Clear();
            textBoxA.Clear();
            textBoxB.Clear();
            textBoxE.Clear();
            textBoxXMin.Clear();
            textBoxYMin.Clear();
            textBoxXMax.Clear();
            textBoxYMax.Clear();

            foreach (Series series in chartFunc.Series)
            {
                series.Points.Clear();
            }

            chartFunc.ChartAreas[0].AxisX.Minimum = -10;
            chartFunc.ChartAreas[0].AxisX.Maximum = 10;
            chartFunc.ChartAreas[0].AxisY.Minimum = -MAX_Y_VALUE;
            chartFunc.ChartAreas[0].AxisY.Maximum = MAX_Y_VALUE;

            chartFunc.Invalidate();
        }

      
    }
}