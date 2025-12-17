using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ProgramLab
{
    public partial class Dichotomy : Form
    {
        private const double MAX_Y_VALUE = 10;
        private const double ZERO_TOLERANCE = 1e-10;

        public Dichotomy()
        {
            InitializeComponent();
            SetupChart();
            SetupEventHandlers();
        }

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

            // Красные точки для корней
            Series rootsSeries = new Series("Корни")
            {
                ChartType = SeriesChartType.Point,
                Color = Color.Red,
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 8
            };
            chartFunc.Series.Add(rootsSeries);

            // Серия для вертикальных асимптот
            Series asymptotesSeries = new Series("Асимптоты")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Gray,
                BorderWidth = 1,
                BorderDashStyle = ChartDashStyle.DashDotDot
            };
            chartFunc.Series.Add(asymptotesSeries);

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

        private double EvaluateFunction(string function, double x)
        {
            try
            {
                string expression = function.ToLower();

                // Обработка ctg
                expression = Regex.Replace(expression, @"ctg\s*\(\s*(.*?)\s*\)", match =>
                {
                    string arg = match.Groups[1].Value;
                    return $"(1/tan({arg}))";
                });

                // Замена констант
                expression = expression
                    .Replace("pi", Math.PI.ToString(CultureInfo.InvariantCulture))
                    .Replace("e", Math.E.ToString(CultureInfo.InvariantCulture));

                expression = ReplaceXInExpression(expression, x);

                // Обработка тригонометрических функций
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

        private List<(double x, double y)> FindRootsDichotomy(string function, double a, double b, double epsilon)
        {
            List<(double x, double y)> roots = new List<(double, double)>();

            // Количество интервалов для поиска смены знака
            int intervals = 100;
            double step = (b - a) / intervals;

            for (int i = 0; i < intervals; i++)
            {
                double x1 = a + i * step;
                double x2 = x1 + step;

                try
                {
                    double f1 = EvaluateFunction(function, x1);
                    double f2 = EvaluateFunction(function, x2);

                    // Проверяем смену знака функции
                    if (Math.Sign(f1) != Math.Sign(f2) && !double.IsNaN(f1) && !double.IsNaN(f2))
                    {
                        // Применяем метод дихотомии для уточнения корня
                        double root = DichotomyMethod(function, x1, x2, epsilon);
                        double y = EvaluateFunction(function, root);

                        // Проверяем, что найденный корень действительно близок к нулю
                        if (Math.Abs(y) < epsilon * 10)
                        {
                            // Проверяем на дубликаты
                            bool isDuplicate = false;
                            foreach (var existingRoot in roots)
                            {
                                if (Math.Abs(existingRoot.x - root) < epsilon * 10)
                                {
                                    isDuplicate = true;
                                    break;
                                }
                            }

                            if (!isDuplicate)
                            {
                                roots.Add((root, y));
                            }
                        }
                    }
                    // Проверяем, если функция точно равна нулю в одной из точек
                    else if (Math.Abs(f1) < epsilon)
                    {
                        bool isDuplicate = false;
                        foreach (var existingRoot in roots)
                        {
                            if (Math.Abs(existingRoot.x - x1) < epsilon * 10)
                            {
                                isDuplicate = true;
                                break;
                            }
                        }

                        if (!isDuplicate)
                        {
                            roots.Add((x1, f1));
                        }
                    }
                    else if (Math.Abs(f2) < epsilon)
                    {
                        bool isDuplicate = false;
                        foreach (var existingRoot in roots)
                        {
                            if (Math.Abs(existingRoot.x - x2) < epsilon * 10)
                            {
                                isDuplicate = true;
                                break;
                            }
                        }

                        if (!isDuplicate)
                        {
                            roots.Add((x2, f2));
                        }
                    }
                }
                catch
                {
                    // Пропускаем интервалы, где функция не определена
                    continue;
                }
            }

            return roots.OrderBy(r => r.x).ToList();
        }

        private double DichotomyMethod(string function, double a, double b, double epsilon)
        {
            double left = a;
            double right = b;
            int maxIterations = 1000;
            int iteration = 0;

            while (Math.Abs(right - left) > epsilon && iteration < maxIterations)
            {
                double mid = (left + right) / 2;

                try
                {
                    double fLeft = EvaluateFunction(function, left);
                    double fMid = EvaluateFunction(function, mid);

                    if (Math.Sign(fLeft) == Math.Sign(fMid))
                    {
                        left = mid;
                    }
                    else
                    {
                        right = mid;
                    }
                }
                catch
                {
                    // Если функция не определена в середине, смещаем границы
                    double delta = (right - left) / 4;
                    left += delta;
                    right -= delta;
                }

                iteration++;
            }

            return (left + right) / 2;
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
            List<double> discontinuityPoints = new List<double>();

            // Построение основного графика функции
            for (int i = 0; i <= pointsCount; i++)
            {
                double x = a + i * step;

                try
                {
                    double y = EvaluateFunction(function, x);

                    if (double.IsInfinity(y) || double.IsNaN(y))
                    {
                        chartFunc.Series["Функция"].Points.AddXY(x, double.NaN);

                        // Добавляем точку разрыва для проверки на асимптоты
                        discontinuityPoints.Add(x);
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
                    discontinuityPoints.Add(x);
                }
            }
            FindAsymptotes(function, a, b, discontinuityPoints, xMin, xMax);
            // Находим вертикальные асимптоты
        

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

                chartFunc.ChartAreas[0].AxisY.Minimum = Math.Max(chartFunc.ChartAreas[0].AxisY.Minimum, -MAX_Y_VALUE);
                chartFunc.ChartAreas[0].AxisY.Maximum = Math.Min(chartFunc.ChartAreas[0].AxisY.Maximum, MAX_Y_VALUE);

                double lineY1 = chartFunc.ChartAreas[0].AxisY.Minimum;
                double lineY2 = chartFunc.ChartAreas[0].AxisY.Maximum;

                chartFunc.Series["Интервал"].Points.AddXY(a, lineY1);
                chartFunc.Series["Интервал"].Points.AddXY(a, lineY2);
                chartFunc.Series["Интервал"].Points.AddXY(double.NaN, double.NaN);

                chartFunc.Series["Интервал"].Points.AddXY(b, lineY1);
                chartFunc.Series["Интервал"].Points.AddXY(b, lineY2);
            }

            // Рисуем оси координат
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

            // Добавляем точки корней на график, если они есть
            if (!string.IsNullOrWhiteSpace(textBoxX.Text) && textBoxX.Text != "нет")
            {
                string[] rootStrings = textBoxX.Text.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string rootStr in rootStrings)
                {
                    if (double.TryParse(rootStr.Replace(",", "."), NumberStyles.Any,
                        CultureInfo.InvariantCulture, out double root))
                    {
                        // Красная точка для корня
                        chartFunc.Series["Корни"].Points.AddXY(root, 0);
                    }
                }
            }

            chartFunc.Invalidate();
        }

        private void FindAsymptotes(string function, double a, double b, List<double> discontinuityPoints, double plotXMin, double plotXMax)
        {
            // Очищаем предыдущие асимптоты
            chartFunc.Series["Асимптоты"].Points.Clear();

            if (discontinuityPoints.Count == 0)
                return;

            // Для поиска асимптот анализируем точки разрыва
            // Асимптоты обычно в точках, где функция стремится к бесконечности
            double step = (b - a) / 1000;

            foreach (double discPoint in discontinuityPoints.Distinct())
            {
                // Проверяем окрестности точки разрыва
                double leftLimit = 0, rightLimit = 0;
                bool leftValid = false, rightValid = false;

                // Проверяем левый предел
                try
                {
                    for (double x = discPoint - step; x > discPoint - 10 * step; x -= step)
                    {
                        double y = EvaluateFunction(function, x);
                        if (!double.IsInfinity(y) && !double.IsNaN(y))
                        {
                            leftLimit = y;
                            leftValid = true;
                            break;
                        }
                    }
                }
                catch { }

                // Проверяем правый предел
                try
                {
                    for (double x = discPoint + step; x < discPoint + 10 * step; x += step)
                    {
                        double y = EvaluateFunction(function, x);
                        if (!double.IsInfinity(y) && !double.IsNaN(y))
                        {
                            rightLimit = y;
                            rightValid = true;
                            break;
                        }
                    }
                }
                catch { }

                // Если один из пределов стремится к бесконечности или имеет большой скачок,
                // считаем это асимптотой
                if (leftValid && rightValid)
                {
                    // Проверяем на большой скачок значений
                    if (Math.Abs(leftLimit - rightLimit) > MAX_Y_VALUE * 2)
                    {
                        // Рисуем вертикальную асимптоту
                        double yMin = chartFunc.ChartAreas[0].AxisY.Minimum;
                        double yMax = chartFunc.ChartAreas[0].AxisY.Maximum;

                        chartFunc.Series["Асимптоты"].Points.AddXY(discPoint, yMin);
                        chartFunc.Series["Асимптоты"].Points.AddXY(discPoint, yMax);
                        chartFunc.Series["Асимптоты"].Points.AddXY(double.NaN, double.NaN);
                    }
                }
                else if (leftValid && Math.Abs(leftLimit) > MAX_Y_VALUE / 2)
                {
                    // Левый предел большой - вероятно асимптота
                    double yMin = chartFunc.ChartAreas[0].AxisY.Minimum;
                    double yMax = chartFunc.ChartAreas[0].AxisY.Maximum;

                    chartFunc.Series["Асимптоты"].Points.AddXY(discPoint, yMin);
                    chartFunc.Series["Асимптоты"].Points.AddXY(discPoint, yMax);
                    chartFunc.Series["Асимптоты"].Points.AddXY(double.NaN, double.NaN);
                }
                else if (rightValid && Math.Abs(rightLimit) > MAX_Y_VALUE / 2)
                {
                    // Правый предел большой - вероятно асимптота
                    double yMin = chartFunc.ChartAreas[0].AxisY.Minimum;
                    double yMax = chartFunc.ChartAreas[0].AxisY.Maximum;

                    chartFunc.Series["Асимптоты"].Points.AddXY(discPoint, yMin);
                    chartFunc.Series["Асимптоты"].Points.AddXY(discPoint, yMax);
                    chartFunc.Series["Асимптоты"].Points.AddXY(double.NaN, double.NaN);
                }
            }

            // Проверяем специальные случаи для известных функций с асимптотами
            string funcLower = function.ToLower().Replace(" ", "");

            // Для тангенса - асимптоты в pi/2 + pi*n
            if (funcLower.Contains("tan") || funcLower.Contains("tg"))
            {
                double pi = Math.PI;
                double startX = Math.Floor(plotXMin / pi - 0.5) * pi + pi / 2;
                double endX = Math.Ceiling(plotXMax / pi - 0.5) * pi + pi / 2;

                for (double x = startX; x <= endX; x += pi)
                {
                    if (x >= a && x <= b)
                    {
                        double yMin = chartFunc.ChartAreas[0].AxisY.Minimum;
                        double yMax = chartFunc.ChartAreas[0].AxisY.Maximum;

                        chartFunc.Series["Асимптоты"].Points.AddXY(x, yMin);
                        chartFunc.Series["Асимптоты"].Points.AddXY(x, yMax);
                        chartFunc.Series["Асимптоты"].Points.AddXY(double.NaN, double.NaN);
                    }
                }
            }

            // Для 1/x - асимптота в x=0
            if (funcLower.Contains("1/x") || funcLower.Contains("x^-1") ||
                Regex.IsMatch(funcLower, @"1\s*/\s*x") || Regex.IsMatch(funcLower, @"1\s*/\s*\(x"))
            {
                if (0 >= a && 0 <= b)
                {
                    double yMin = chartFunc.ChartAreas[0].AxisY.Minimum;
                    double yMax = chartFunc.ChartAreas[0].AxisY.Maximum;

                    chartFunc.Series["Асимптоты"].Points.AddXY(0, yMin);
                    chartFunc.Series["Асимптоты"].Points.AddXY(0, yMax);
                    chartFunc.Series["Асимптоты"].Points.AddXY(double.NaN, double.NaN);
                }
            }

            // Для котангенса - асимптоты в pi*n
            if (funcLower.Contains("ctg") || funcLower.Contains("cot"))
            {
                double pi = Math.PI;
                double startX = Math.Floor(plotXMin / pi) * pi;
                double endX = Math.Ceiling(plotXMax / pi) * pi;

                for (double x = startX; x <= endX; x += pi)
                {
                    if (x >= a && x <= b)
                    {
                        double yMin = chartFunc.ChartAreas[0].AxisY.Minimum;
                        double yMax = chartFunc.ChartAreas[0].AxisY.Maximum;

                        chartFunc.Series["Асимптоты"].Points.AddXY(x, yMin);
                        chartFunc.Series["Асимптоты"].Points.AddXY(x, yMax);
                        chartFunc.Series["Асимптоты"].Points.AddXY(double.NaN, double.NaN);
                    }
                }
            }
        }

        private void TextBoxNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            System.Windows.Forms.TextBox textBox = (System.Windows.Forms.TextBox)sender;
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

                double epsilon = Math.Pow(10, -precision);

                // Ищем корни методом дихотомии
                List<(double x, double y)> roots = FindRootsDichotomy(textBoxF.Text, a, b, epsilon);

                // Выводим результаты
                if (roots.Count > 0)
                {
                    string format = $"F{precision}";
                    textBoxX.Text = string.Join(", ", roots.Select(r => r.x.ToString(format)));
                    textBoxY.Text = string.Join(", ", roots.Select(r => r.y.ToString(format)));
                }
                else
                {
                    textBoxX.Text = "нет";
                    textBoxY.Text = "нет";

                    MessageBox.Show("На заданном интервале корней не найдено",
                                  "Информация",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Information);
                }
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
            textBoxX.Clear();
            textBoxY.Clear();

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

        private void buttonBack_Click(object sender, EventArgs e)
        {
            Main main = new Main();
            main.Show();
            Close();
        }
    }
}