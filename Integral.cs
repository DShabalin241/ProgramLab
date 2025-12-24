using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ProgramLab
{
    public partial class Integral : Form
    {
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isCalculationRunning = false;
        private const double INFINITY_THRESHOLD = 1e10;

        public Integral()
        {
            InitializeComponent();
            SetupChart();
            SetupEventHandlers();
        }

        private void SetupChart()
        {
            chartIntegration.Series.Clear();

            // График функции
            Series functionSeries = new Series("Функция")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Blue,
                BorderWidth = 2
            };
            chartIntegration.Series.Add(functionSeries);

            // Прямоугольники
            Series rectanglesSeries = new Series("Прямоугольники")
            {
                ChartType = SeriesChartType.Area,
                Color = Color.FromArgb(80, Color.Green),
                BorderColor = Color.Green,
                BorderWidth = 1
            };
            chartIntegration.Series.Add(rectanglesSeries);

            // Трапеции
            Series trapezoidsSeries = new Series("Трапеции")
            {
                ChartType = SeriesChartType.Area,
                Color = Color.FromArgb(80, Color.Orange),
                BorderColor = Color.Orange,
                BorderWidth = 1
            };
            chartIntegration.Series.Add(trapezoidsSeries);

            // Параболы (Симпсон)
            Series parabolasSeries = new Series("Параболы")
            {
                ChartType = SeriesChartType.Area,
                Color = Color.FromArgb(80, Color.Red),
                BorderColor = Color.Red,
                BorderWidth = 1
            };
            chartIntegration.Series.Add(parabolasSeries);

            // Границы интегрирования
            Series boundsSeries = new Series("Границы")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Black,
                BorderWidth = 2,
                BorderDashStyle = ChartDashStyle.Dash
            };
            chartIntegration.Series.Add(boundsSeries);

            // Настройка осей
            chartIntegration.ChartAreas[0].AxisX.Title = "X";
            chartIntegration.ChartAreas[0].AxisY.Title = "Y";
            chartIntegration.ChartAreas[0].AxisX.LabelStyle.Format = "F2";
            chartIntegration.ChartAreas[0].AxisY.LabelStyle.Format = "F2";
        }

        private void SetupEventHandlers()
        {
            textBoxA.KeyPress += TextBoxNumber_KeyPress;
            textBoxB.KeyPress += TextBoxNumber_KeyPress;
            textBoxN.KeyPress += TextBoxPositiveInteger_KeyPress;

            buttonCalculateIntegral.Click += ButtonCalculateIntegral_Click;
            buttonStopIntegral.Click += ButtonStopIntegral_Click;
            buttonClearIntegral.Click += ButtonClearIntegral_Click;
            checkBoxAutoN.CheckedChanged += CheckBoxAutoN_CheckedChanged;
        }

        #region Обработчики ввода
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

        private void TextBoxPositiveInteger_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        #endregion

        #region Улучшенная обработка функций
        private double EvaluateFunction(string function, double x)
        {
            try
            {
                string expression = function.ToLower();
                expression = expression.Replace(" ", ""); // Убираем пробелы

                // Обработка констант
                expression = expression
                    .Replace("pi", "(" + Math.PI.ToString(CultureInfo.InvariantCulture) + ")")
                    .Replace("e", "(" + Math.E.ToString(CultureInfo.InvariantCulture) + ")");

                // Обработка котангенса
                expression = expression.Replace("ctg", "cot");

                // Обработка степеней x^n
                expression = ProcessPowers(expression, x);

                // Замена переменной x
                expression = ReplaceXInExpression(expression, x);

                // Обработка тригонометрических функций
                expression = ProcessTrigonometricFunctions(expression);

                // Обработка логарифмов
                expression = ProcessLogarithms(expression);

                // Обработка экспоненты
                expression = ProcessExponential(expression);

                return EvaluateMathExpression(expression);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка вычисления функции в точке x={x}: {ex.Message}");
            }
        }

        private string ProcessPowers(string expression, double x)
        {
            // Обработка x^2, x^3 и т.д.
            expression = expression.Replace("x^2", "(x*x)");
            expression = expression.Replace("x^3", "(x*x*x)");
            expression = expression.Replace("x^4", "(x*x*x*x)");
            expression = expression.Replace("x^5", "(x*x*x*x*x)");

            // Общая обработка x^n
            var matches = System.Text.RegularExpressions.Regex.Matches(expression, @"x\^(\d+(\.\d+)?)");
            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                if (match.Success)
                {
                    string powerStr = match.Groups[1].Value;
                    if (double.TryParse(powerStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double power))
                    {
                        double result = Math.Pow(x, power);
                        expression = expression.Replace(match.Value, result.ToString(CultureInfo.InvariantCulture));
                    }
                }
            }

            return expression;
        }

        private string ReplaceXInExpression(string expression, double x)
        {
            // Сначала заменяем x в скобках, потом одиночный x
            expression = expression.Replace("(x)", "(" + x.ToString(CultureInfo.InvariantCulture) + ")");
            expression = expression.Replace("x", x.ToString(CultureInfo.InvariantCulture));
            return expression;
        }

        private string ProcessTrigonometricFunctions(string expression)
        {
            // Обработка sin, cos, tan, cot
            expression = System.Text.RegularExpressions.Regex.Replace(expression, @"sin\(([^)]+)\)", match =>
            {
                string arg = match.Groups[1].Value;
                try
                {
                    double value = EvaluateMathExpression(arg);
                    return Math.Sin(value).ToString(CultureInfo.InvariantCulture);
                }
                catch
                {
                    return match.Value;
                }
            });

            expression = System.Text.RegularExpressions.Regex.Replace(expression, @"cos\(([^)]+)\)", match =>
            {
                string arg = match.Groups[1].Value;
                try
                {
                    double value = EvaluateMathExpression(arg);
                    return Math.Cos(value).ToString(CultureInfo.InvariantCulture);
                }
                catch
                {
                    return match.Value;
                }
            });

            expression = System.Text.RegularExpressions.Regex.Replace(expression, @"tan\(([^)]+)\)", match =>
            {
                string arg = match.Groups[1].Value;
                try
                {
                    double value = EvaluateMathExpression(arg);
                    return Math.Tan(value).ToString(CultureInfo.InvariantCulture);
                }
                catch
                {
                    return match.Value;
                }
            });

            expression = System.Text.RegularExpressions.Regex.Replace(expression, @"cot\(([^)]+)\)", match =>
            {
                string arg = match.Groups[1].Value;
                try
                {
                    double value = EvaluateMathExpression(arg);
                    double tanValue = Math.Tan(value);
                    if (Math.Abs(tanValue) < 1e-15)
                        throw new DivideByZeroException("Котангенс не определен");
                    return (1.0 / tanValue).ToString(CultureInfo.InvariantCulture);
                }
                catch
                {
                    return match.Value;
                }
            });

            return expression;
        }

        private string ProcessLogarithms(string expression)
        {
            // Обработка ln (натуральный логарифм)
            expression = System.Text.RegularExpressions.Regex.Replace(expression, @"ln\(([^)]+)\)", match =>
            {
                string arg = match.Groups[1].Value;
                try
                {
                    double value = EvaluateMathExpression(arg);
                    if (value <= 0)
                        throw new ArgumentException("Логарифм от неположительного числа");
                    return Math.Log(value).ToString(CultureInfo.InvariantCulture);
                }
                catch
                {
                    return match.Value;
                }
            });

            // Обработка log (десятичный логарифм)
            expression = System.Text.RegularExpressions.Regex.Replace(expression, @"log\(([^)]+)\)", match =>
            {
                string arg = match.Groups[1].Value;
                try
                {
                    double value = EvaluateMathExpression(arg);
                    if (value <= 0)
                        throw new ArgumentException("Логарифм от неположительного числа");
                    return Math.Log10(value).ToString(CultureInfo.InvariantCulture);
                }
                catch
                {
                    return match.Value;
                }
            });

            return expression;
        }

        private string ProcessExponential(string expression)
        {
            // Обработка exp(x)
            expression = System.Text.RegularExpressions.Regex.Replace(expression, @"exp\(([^)]+)\)", match =>
            {
                string arg = match.Groups[1].Value;
                try
                {
                    double value = EvaluateMathExpression(arg);
                    return Math.Exp(value).ToString(CultureInfo.InvariantCulture);
                }
                catch
                {
                    return match.Value;
                }
            });

            return expression;
        }

        private double EvaluateMathExpression(string expression)
        {
            try
            {
                expression = expression.Replace(",", ".");

                // Обработка умножения без знака (например, 2x или x3)
                expression = System.Text.RegularExpressions.Regex.Replace(expression, @"(\d)([a-zA-Z\(])", "$1*$2");
                expression = System.Text.RegularExpressions.Regex.Replace(expression, @"([a-zA-Z\)])(\d)", "$1*$2");

                // Заменяем ^ на ** для вычисления степеней
                expression = expression.Replace("^", "**");

                System.Data.DataTable table = new System.Data.DataTable();
                table.Columns.Add("expression", typeof(string), expression);
                System.Data.DataRow row = table.NewRow();
                table.Rows.Add(row);
                return Convert.ToDouble(row["expression"]);
            }
            catch
            {
                throw new Exception($"Невозможно вычислить выражение: {expression}");
            }
        }
        #endregion

        #region Методы интегрирования с правильной площадью
        private string GetSelectedMethod()
        {
            if (radioButtonRectangles.Checked) return "Прямоугольники";
            if (radioButtonTrapezoids.Checked) return "Трапеции";
            if (radioButtonSimpson.Checked) return "Симпсон";
            return "Прямоугольники";
        }

        private double CalculateIntegral(string function, double a, double b, int n, string method)
        {
            switch (method)
            {
                case "Прямоугольники":
                    return CalculateRectanglesArea(function, a, b, n);
                case "Трапеции":
                    return CalculateTrapezoidsArea(function, a, b, n);
                case "Симпсон":
                    return CalculateSimpsonArea(function, a, b, n);
                default:
                    return CalculateRectanglesArea(function, a, b, n);
            }
        }

        private double CalculateRectanglesArea(string function, double a, double b, int n)
        {
            double h = (b - a) / n;
            double totalArea = 0;

            for (int i = 0; i < n; i++)
            {
                double xLeft = a + i * h;

                try
                {
                    double y = EvaluateFunction(function, xLeft);

                    if (!double.IsInfinity(y) && !double.IsNaN(y))
                    {
                        // Площадь прямоугольника = |y| * h
                        double rectangleArea = Math.Abs(y) * h;
                        totalArea += rectangleArea;
                    }
                }
                catch
                {
                    // Пропускаем точки разрыва
                }
            }

            return totalArea;
        }

        private double CalculateTrapezoidsArea(string function, double a, double b, int n)
        {
            double h = (b - a) / n;
            double totalArea = 0;

            for (int i = 0; i < n; i++)
            {
                double x1 = a + i * h;
                double x2 = x1 + h;

                try
                {
                    double y1 = EvaluateFunction(function, x1);
                    double y2 = EvaluateFunction(function, x2);

                    if (!double.IsInfinity(y1) && !double.IsNaN(y1) &&
                        !double.IsInfinity(y2) && !double.IsNaN(y2))
                    {
                        // Площадь трапеции = (|y1| + |y2|) * h / 2
                        double trapezoidArea = (Math.Abs(y1) + Math.Abs(y2)) * h / 2;
                        totalArea += trapezoidArea;
                    }
                }
                catch
                {
                    // Пропускаем точки разрыва
                }
            }

            return totalArea;
        }

        private double CalculateSimpsonArea(string function, double a, double b, int n)
        {
            if (n % 2 != 0) n++;

            double h = (b - a) / n;
            double totalArea = 0;

            for (int i = 0; i < n; i += 2)
            {
                double x0 = a + i * h;
                double x1 = x0 + h;
                double x2 = x1 + h;

                try
                {
                    double y0 = EvaluateFunction(function, x0);
                    double y1 = EvaluateFunction(function, x1);
                    double y2 = EvaluateFunction(function, x2);

                    if (!double.IsInfinity(y0) && !double.IsNaN(y0) &&
                        !double.IsInfinity(y1) && !double.IsNaN(y1) &&
                        !double.IsInfinity(y2) && !double.IsNaN(y2))
                    {
                        // Формула Симпсона для площади под параболой
                        double parabolaArea = (h / 3) * (Math.Abs(y0) + 4 * Math.Abs(y1) + Math.Abs(y2));
                        totalArea += parabolaArea;
                    }
                }
                catch
                {
                    // Пропускаем точки разрыва
                }
            }

            return totalArea;
        }
        #endregion

        #region Исправленная отрисовка - столбцы не выходят за функцию
        private void PlotFunctionAndAreas(string function, double a, double b, int n)
        {
            // Очищаем все графики
            chartIntegration.Series["Функция"].Points.Clear();
            chartIntegration.Series["Прямоугольники"].Points.Clear();
            chartIntegration.Series["Трапеции"].Points.Clear();
            chartIntegration.Series["Параболы"].Points.Clear();
            chartIntegration.Series["Границы"].Points.Clear();

            if (n <= 0) n = 1;
            string method = GetSelectedMethod();

            double h = (b - a) / n;

            // Собираем данные о функции
            int pointsCount = 1000;
            double step = (b - a) / pointsCount;
            List<double> validYValues = new List<double>();
            List<PointF> functionPoints = new List<PointF>();

            // График функции
            for (int i = 0; i <= pointsCount; i++)
            {
                double x = a + i * step;
                try
                {
                    double y = EvaluateFunction(function, x);

                    if (!double.IsInfinity(y) && !double.IsNaN(y) && Math.Abs(y) < INFINITY_THRESHOLD)
                    {
                        chartIntegration.Series["Функция"].Points.AddXY(x, y);
                        functionPoints.Add(new PointF((float)x, (float)y));
                        validYValues.Add(y);
                    }
                    else
                    {
                        chartIntegration.Series["Функция"].Points.AddXY(x, double.NaN);
                    }
                }
                catch
                {
                    chartIntegration.Series["Функция"].Points.AddXY(x, double.NaN);
                }
            }

            // Отрисовка в зависимости от метода
            switch (method)
            {
                case "Прямоугольники":
                    DrawRectanglesCorrect(function, a, b, n, h, functionPoints);
                    break;
                case "Трапеции":
                    DrawTrapezoidsCorrect(function, a, b, n, h, functionPoints);
                    break;
                case "Симпсон":
                    DrawParabolasCorrect(function, a, b, n, h, functionPoints);
                    break;
            }

            // Границы интегрирования
            double yMin, yMax;
            if (validYValues.Count > 0)
            {
                yMin = Math.Min(validYValues.Min(), 0) - 0.5;
                yMax = Math.Max(validYValues.Max(), 0) + 0.5;
            }
            else
            {
                yMin = -5;
                yMax = 5;
            }

            chartIntegration.Series["Границы"].Points.AddXY(a, yMin);
            chartIntegration.Series["Границы"].Points.AddXY(a, yMax);
            chartIntegration.Series["Границы"].Points.AddXY(double.NaN, double.NaN);
            chartIntegration.Series["Границы"].Points.AddXY(b, yMin);
            chartIntegration.Series["Границы"].Points.AddXY(b, yMax);

            // Настройка масштаба
            chartIntegration.ChartAreas[0].AxisX.Minimum = a - Math.Abs(b - a) * 0.05;
            chartIntegration.ChartAreas[0].AxisX.Maximum = b + Math.Abs(b - a) * 0.05;
            chartIntegration.ChartAreas[0].AxisY.Minimum = yMin;
            chartIntegration.ChartAreas[0].AxisY.Maximum = yMax;

            // Ось X
            if (chartIntegration.Series.IndexOf("AxisX") >= 0)
            {
                chartIntegration.Series.Remove(chartIntegration.Series["AxisX"]);
            }

            if (yMin < 0 && yMax > 0)
            {
                Series axisX = chartIntegration.Series.Add("AxisX");
                axisX.ChartType = SeriesChartType.Line;
                axisX.Color = Color.Gray;
                axisX.BorderWidth = 1;
                axisX.BorderDashStyle = ChartDashStyle.Dash;
                axisX.Points.AddXY(chartIntegration.ChartAreas[0].AxisX.Minimum, 0);
                axisX.Points.AddXY(chartIntegration.ChartAreas[0].AxisX.Maximum, 0);
            }

            chartIntegration.Invalidate();
        }

        // Правильная отрисовка прямоугольников (не выходят за функцию)
        private void DrawRectanglesCorrect(string function, double a, double b, int n, double h, List<PointF> functionPoints)
        {
            for (int i = 0; i < n; i++)
            {
                double x1 = a + i * h;
                double x2 = x1 + h;
                double xMid = x1 + h / 2; // Средняя точка для лучшего приближения

                try
                {
                    double y = EvaluateFunction(function, xMid);

                    if (!double.IsInfinity(y) && !double.IsNaN(y))
                    {
                        // Определяем, над или под осью X функция
                        double rectangleHeight = Math.Abs(y);
                        double rectangleBottom, rectangleTop;

                        if (y >= 0)
                        {
                            rectangleBottom = 0;
                            rectangleTop = rectangleHeight;
                        }
                        else
                        {
                            rectangleBottom = -rectangleHeight;
                            rectangleTop = 0;
                        }

                        // Рисуем прямоугольник
                        chartIntegration.Series["Прямоугольники"].Points.AddXY(x1, rectangleBottom);
                        chartIntegration.Series["Прямоугольники"].Points.AddXY(x1, rectangleTop);
                        chartIntegration.Series["Прямоугольники"].Points.AddXY(x2, rectangleTop);
                        chartIntegration.Series["Прямоугольники"].Points.AddXY(x2, rectangleBottom);
                        chartIntegration.Series["Прямоугольники"].Points.AddXY(x1, rectangleBottom);
                        chartIntegration.Series["Прямоугольники"].Points.AddXY(double.NaN, double.NaN);
                    }
                }
                catch
                {
                    // Пропускаем точки разрыва
                }
            }
        }

        // Правильная отрисовка трапеций
        private void DrawTrapezoidsCorrect(string function, double a, double b, int n, double h, List<PointF> functionPoints)
        {
            for (int i = 0; i < n; i++)
            {
                double x1 = a + i * h;
                double x2 = x1 + h;

                try
                {
                    double y1 = EvaluateFunction(function, x1);
                    double y2 = EvaluateFunction(function, x2);

                    if (!double.IsInfinity(y1) && !double.IsNaN(y1) &&
                        !double.IsInfinity(y2) && !double.IsNaN(y2))
                    {
                        // Определяем точки трапеции
                        double bottom1, top1, bottom2, top2;

                        if (y1 >= 0)
                        {
                            bottom1 = 0;
                            top1 = Math.Abs(y1);
                        }
                        else
                        {
                            bottom1 = -Math.Abs(y1);
                            top1 = 0;
                        }

                        if (y2 >= 0)
                        {
                            bottom2 = 0;
                            top2 = Math.Abs(y2);
                        }
                        else
                        {
                            bottom2 = -Math.Abs(y2);
                            top2 = 0;
                        }

                        // Рисуем трапецию
                        chartIntegration.Series["Трапеции"].Points.AddXY(x1, bottom1);
                        chartIntegration.Series["Трапеции"].Points.AddXY(x1, top1);
                        chartIntegration.Series["Трапеции"].Points.AddXY(x2, top2);
                        chartIntegration.Series["Трапеции"].Points.AddXY(x2, bottom2);
                        chartIntegration.Series["Трапеции"].Points.AddXY(x1, bottom1);
                        chartIntegration.Series["Трапеции"].Points.AddXY(double.NaN, double.NaN);
                    }
                }
                catch
                {
                    // Пропускаем точки разрыва
                }
            }
        }

        // Правильная отрисовка парабол (Симпсон)
        private void DrawParabolasCorrect(string function, double a, double b, int n, double h, List<PointF> functionPoints)
        {
            if (n % 2 != 0) n++;

            for (int i = 0; i < n; i += 2)
            {
                double x0 = a + i * h;
                double x1 = x0 + h;
                double x2 = x1 + h;

                try
                {
                    double y0 = EvaluateFunction(function, x0);
                    double y1 = EvaluateFunction(function, x1);
                    double y2 = EvaluateFunction(function, x2);

                    if (!double.IsInfinity(y0) && !double.IsNaN(y0) &&
                        !double.IsInfinity(y1) && !double.IsNaN(y1) &&
                        !double.IsInfinity(y2) && !double.IsNaN(y2))
                    {
                        int segments = 20;

                        // Верхняя кривая (парабола)
                        for (int j = 0; j <= segments; j++)
                        {
                            double t = (double)j / segments;
                            double x = x0 + (x2 - x0) * t;
                            double y = InterpolateQuadratic(x0, Math.Abs(y0), x1, Math.Abs(y1), x2, Math.Abs(y2), x);

                            // Учитываем знак
                            double finalY = (y0 >= 0 && y1 >= 0 && y2 >= 0) ? y : -y;
                            chartIntegration.Series["Параболы"].Points.AddXY(x, finalY);
                        }

                        // Нижняя линия (ось X)
                        for (int j = segments; j >= 0; j--)
                        {
                            double t = (double)j / segments;
                            double x = x0 + (x2 - x0) * t;
                            chartIntegration.Series["Параболы"].Points.AddXY(x, 0);
                        }

                        chartIntegration.Series["Параболы"].Points.AddXY(double.NaN, double.NaN);
                    }
                }
                catch
                {
                    // Пропускаем точки разрыва
                }
            }
        }

        private double InterpolateQuadratic(double x0, double y0, double x1, double y1, double x2, double y2, double x)
        {
            double l0 = ((x - x1) * (x - x2)) / ((x0 - x1) * (x0 - x2));
            double l1 = ((x - x0) * (x - x2)) / ((x1 - x0) * (x1 - x2));
            double l2 = ((x - x0) * (x - x1)) / ((x2 - x0) * (x2 - x1));

            return y0 * l0 + y1 * l1 + y2 * l2;
        }
        #endregion

        #region Автоматический подбор N с корректной отрисовкой
        private async Task<double> AdaptiveIntegrateAsync(
            string function,
            double a,
            double b,
            double precision,
            string method,
            CancellationToken cancellationToken)
        {
            int n = 4;
            if (method == "Симпсон" && n % 2 != 0) n += 2;

            double previousResult = CalculateIntegral(function, a, b, n, method);
            double currentResult;
            int iteration = 0;
            const int maxIterations = 15;

            while (iteration < maxIterations && !cancellationToken.IsCancellationRequested)
            {
                n *= 2;
                if (method == "Симпсон" && n % 2 != 0) n += 2;

                currentResult = CalculateIntegral(function, a, b, n, method);

                // Обновляем отрисовку с текущим N
                if (iteration % 2 == 0) // Обновляем каждую вторую итерацию для производительности
                {
                    this.Invoke((MethodInvoker)delegate {
                        PlotFunctionAndAreas(function, a, b, n);
                        labelInfo.Text = $"Итерация: {iteration + 1}, N={n}, Точность: {Math.Abs(currentResult - previousResult):E6}";
                    });
                }

                // Правило Рунге
                double power = (method == "Симпсон") ? 4.0 : 2.0;
                double errorEstimate = Math.Abs(currentResult - previousResult) / (Math.Pow(2, power) - 1);

                if (errorEstimate < precision || Math.Abs(currentResult - previousResult) < precision)
                {
                    // Финальная отрисовка с найденным N
                    this.Invoke((MethodInvoker)delegate {
                        PlotFunctionAndAreas(function, a, b, n);
                        labelInfo.Text = $"Сходимость достигнута за {iteration + 1} итераций, N={n}";
                        textBoxN.Text = n.ToString();
                    });
                    return currentResult;
                }

                previousResult = currentResult;
                iteration++;

                await Task.Delay(100, cancellationToken);
            }

            // Финальная отрисовка
            this.Invoke((MethodInvoker)delegate {
                PlotFunctionAndAreas(function, a, b, n);
                labelInfo.Text = $"Достигнут максимум итераций, N={n}";
                textBoxN.Text = n.ToString();
            });

            return previousResult;
        }
        #endregion

        #region Обработчики событий
        private void CheckBoxAutoN_CheckedChanged(object sender, EventArgs e)
        {
            textBoxN.Enabled = !checkBoxAutoN.Checked;

            if (checkBoxAutoN.Checked)
            {
                textBoxN.Text = "Авто";
            }
            else
            {
                textBoxN.Text = "10";
            }
        }

        private async void ButtonCalculateIntegral_Click(object sender, EventArgs e)
        {
            try
            {
                // Проверка входных данных
                if (string.IsNullOrWhiteSpace(textBoxFunction.Text))
                {
                    MessageBox.Show("Введите функцию для интегрирования", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!double.TryParse(textBoxA.Text.Replace(",", "."), NumberStyles.Any,
                    CultureInfo.InvariantCulture, out double a))
                {
                    MessageBox.Show("Некорректное значение A", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!double.TryParse(textBoxB.Text.Replace(",", "."), NumberStyles.Any,
                    CultureInfo.InvariantCulture, out double b))
                {
                    MessageBox.Show("Некорректное значение B", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (a >= b)
                {
                    MessageBox.Show("Значение A должно быть меньше B", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Проверка N
                int? n = null;
                if (!checkBoxAutoN.Checked)
                {
                    if (!int.TryParse(textBoxN.Text, out int nValue) || nValue <= 0)
                    {
                        MessageBox.Show("N должно быть положительным целым числом", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    n = Math.Min(nValue, 500); // Ограничиваем для производительности
                }
                else
                {
                    n = 4; // Начальное значение для авторежима
                }

                // Останавливаем предыдущее вычисление
                if (_isCalculationRunning)
                {
                    _cancellationTokenSource?.Cancel();
                }

                // Начальная визуализация
                PlotFunctionAndAreas(textBoxFunction.Text, a, b, n.Value);

                // Запуск вычисления
                _cancellationTokenSource = new CancellationTokenSource();
                _isCalculationRunning = true;
                buttonCalculateIntegral.Enabled = false;
                buttonStopIntegral.Enabled = true;

                try
                {
                    double result;
                    string method = GetSelectedMethod();

                    if (checkBoxAutoN.Checked)
                    {
                        double precision = 1e-6;
                        result = await AdaptiveIntegrateAsync(
                            textBoxFunction.Text, a, b, precision, method,
                            _cancellationTokenSource.Token);
                    }
                    else
                    {
                        result = CalculateIntegral(textBoxFunction.Text, a, b, n.Value, method);
                        labelInfo.Text = $"Метод: {method}, N={n.Value}";
                    }

                    // Вывод результата
                    textBoxResult.Text = result.ToString("F10");
                }
                catch (OperationCanceledException)
                {
                    textBoxResult.Text = "Прервано";
                    labelInfo.Text = "Вычисление прервано";
                }
                catch (Exception ex)
                {
                    textBoxResult.Text = "Ошибка";
                    labelInfo.Text = $"Ошибка: {ex.Message}";
                    MessageBox.Show($"Ошибка вычисления: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    _isCalculationRunning = false;
                    buttonCalculateIntegral.Enabled = true;
                    buttonStopIntegral.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка ввода: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

                _isCalculationRunning = false;
                buttonCalculateIntegral.Enabled = true;
                buttonStopIntegral.Enabled = false;
            }
        }

        private void ButtonStopIntegral_Click(object sender, EventArgs e)
        {
            if (_isCalculationRunning && _cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                buttonStopIntegral.Enabled = false;
            }
        }

        private void ButtonClearIntegral_Click(object sender, EventArgs e)
        {
            if (_isCalculationRunning)
            {
                _cancellationTokenSource?.Cancel();
                _isCalculationRunning = false;
            }

            // Очистка полей
            textBoxFunction.Clear();
            textBoxA.Clear();
            textBoxB.Clear();
            textBoxN.Text = "10";
            textBoxResult.Clear();
            checkBoxAutoN.Checked = false;
            labelInfo.Text = "";

            // Очистка графика
            foreach (Series series in chartIntegration.Series)
            {
                series.Points.Clear();
            }

            if (chartIntegration.Series.IndexOf("AxisX") >= 0)
            {
                chartIntegration.Series.Remove(chartIntegration.Series["AxisX"]);
            }

            SetupChart();
            chartIntegration.Invalidate();
        }
        #endregion

        private void buttonBack_Click(object sender, EventArgs e)
        {
            if (_isCalculationRunning)
            {
                _cancellationTokenSource?.Cancel();
            }

            Main main = new Main();
            main.Show();
            this.Close();
        }
    }
}