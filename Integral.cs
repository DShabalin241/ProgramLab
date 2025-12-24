using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
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
            // Проверка ввода
            textBoxA.KeyPress += TextBoxNumber_KeyPress;
            textBoxB.KeyPress += TextBoxNumber_KeyPress;
            textBoxN.KeyPress += TextBoxPositiveInteger_KeyPress;

            // Обработчики
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

        #region Методы вычисления функции
        private double EvaluateFunction(string function, double x)
        {
            try
            {
                string expression = function.ToLower();

                // Обработка котангенса
                expression = expression.Replace("ctg", "(1/tan)");

                // Замена констант
                expression = expression
                    .Replace("pi", Math.PI.ToString(CultureInfo.InvariantCulture))
                    .Replace("e", Math.E.ToString(CultureInfo.InvariantCulture));

                // Замена переменной x
                expression = ReplaceXInExpression(expression, x);

                // Вычисление выражения
                return EvaluateSimpleExpression(expression);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка вычисления функции в точке x={x}: {ex.Message}");
            }
        }

        private string ReplaceXInExpression(string expression, double x)
        {
            expression = expression.Replace("x", $"({x.ToString(CultureInfo.InvariantCulture)})");
            expression = expression.Replace("X", $"({x.ToString(CultureInfo.InvariantCulture)})");
            return expression;
        }

        private double EvaluateSimpleExpression(string expression)
        {
            System.Data.DataTable table = new System.Data.DataTable();
            expression = expression.Replace(",", ".");
            expression = expression.Replace("^", "**");

            table.Columns.Add("expression", typeof(string), expression);
            System.Data.DataRow row = table.NewRow();
            table.Rows.Add(row);
            return double.Parse((string)row["expression"]);
        }
        #endregion

        #region Методы численного интегрирования

        // Получение выбранного метода
        private string GetSelectedMethod()
        {
            if (radioButtonRectangles.Checked) return "Прямоугольники";
            if (radioButtonTrapezoids.Checked) return "Трапеции";
            if (radioButtonSimpson.Checked) return "Симпсон";
            return "Прямоугольники";
        }

        // Основной метод расчета интеграла
        private double CalculateIntegral(string function, double a, double b, int n, string method)
        {
            switch (method)
            {
                case "Прямоугольники":
                    return RectanglesMethod(function, a, b, n);
                case "Трапеции":
                    return TrapezoidsMethod(function, a, b, n);
                case "Симпсон":
                    return SimpsonMethod(function, a, b, n);
                default:
                    return RectanglesMethod(function, a, b, n);
            }
        }

        // Метод прямоугольников (левые прямоугольники)
        private double RectanglesMethod(string function, double a, double b, int n)
        {
            double h = (b - a) / n;
            double sum = 0;

            for (int i = 0; i < n; i++)
            {
                double x = a + i * h;
                try
                {
                    double y = EvaluateFunction(function, x);
                    if (!double.IsInfinity(y) && !double.IsNaN(y))
                    {
                        sum += Math.Abs(y * h); // Площадь прямоугольника
                    }
                }
                catch
                {
                    // Пропускаем точки разрыва
                }
            }

            return sum;
        }

        // Метод трапеций
        private double TrapezoidsMethod(string function, double a, double b, int n)
        {
            double h = (b - a) / n;
            double sum = 0;

            try
            {
                double y0 = EvaluateFunction(function, a);
                double yn = EvaluateFunction(function, b);

                if (!double.IsInfinity(y0) && !double.IsNaN(y0) &&
                    !double.IsInfinity(yn) && !double.IsNaN(yn))
                {
                    sum = (Math.Abs(y0) + Math.Abs(yn)) / 2;
                }
            }
            catch
            {
                // Если на границах функция не определена
            }

            for (int i = 1; i < n; i++)
            {
                double x = a + i * h;
                try
                {
                    double y = EvaluateFunction(function, x);
                    if (!double.IsInfinity(y) && !double.IsNaN(y))
                    {
                        sum += Math.Abs(y); // Сумма средних линий
                    }
                }
                catch
                {
                    // Пропускаем точки разрыва
                }
            }

            return sum * h; // Умножаем на шаг в конце
        }

        // Метод Симпсона (парабол)
        private double SimpsonMethod(string function, double a, double b, int n)
        {
            if (n % 2 != 0) n++; // Методу Симпсона нужно четное n

            double h = (b - a) / n;
            double sum = 0;

            try
            {
                double y0 = EvaluateFunction(function, a);
                double yn = EvaluateFunction(function, b);

                if (!double.IsInfinity(y0) && !double.IsNaN(y0) &&
                    !double.IsInfinity(yn) && !double.IsNaN(yn))
                {
                    sum = Math.Abs(y0) + Math.Abs(yn);
                }
            }
            catch
            {
                // Если на границах функция не определена
            }

            for (int i = 1; i < n; i++)
            {
                double x = a + i * h;
                try
                {
                    double y = EvaluateFunction(function, x);
                    if (!double.IsInfinity(y) && !double.IsNaN(y))
                    {
                        double coefficient = (i % 2 == 0) ? 2 : 4;
                        sum += coefficient * Math.Abs(y);
                    }
                }
                catch
                {
                    // Пропускаем точки разрыва
                }
            }

            return sum * h / 3;
        }

        // Адаптивный метод с контролем точности
        private async Task<double> AdaptiveIntegrateAsync(
            string function,
            double a,
            double b,
            double precision,
            string method,
            CancellationToken cancellationToken)
        {
            int n = 4;
            double previousResult = CalculateIntegral(function, a, b, n, method);
            double currentResult;
            int iteration = 0;
            const int maxIterations = 20;

            while (iteration < maxIterations && !cancellationToken.IsCancellationRequested)
            {
                n *= 2;
                if (method == "Симпсон" && n % 2 != 0) n++;

                currentResult = CalculateIntegral(function, a, b, n, method);

                // Правило Рунге для оценки погрешности
                double errorEstimate = Math.Abs(currentResult - previousResult);

                if (errorEstimate < precision)
                {
                    return currentResult;
                }

                previousResult = currentResult;
                iteration++;

                await Task.Delay(50, cancellationToken);
            }

            return previousResult;
        }

        #endregion

        #region Визуализация

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

            // Вычисляем ширину разбиений (одинаковая для всех методов)
            double h = (b - a) / n;

            // Построение функции
            int pointsCount = 1000;
            double step = (b - a) / pointsCount;
            List<double> validYValues = new List<double>();

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

            // Отрисовка в зависимости от выбранного метода
            switch (method)
            {
                case "Прямоугольники":
                    DrawRectangles(function, a, b, n, h);
                    break;
                case "Трапеции":
                    DrawTrapezoids(function, a, b, n, h);
                    break;
                case "Симпсон":
                    DrawParabolas(function, a, b, n, h);
                    break;
            }

            // Границы интегрирования
            double yMin, yMax;
            if (validYValues.Count > 0)
            {
                yMin = validYValues.Min() - 1;
                yMax = validYValues.Max() + 1;
                yMin = Math.Min(yMin, -1);
                yMax = Math.Max(yMax, 1);
            }
            else
            {
                yMin = -10;
                yMax = 10;
            }

            // Вертикальные линии границ
            chartIntegration.Series["Границы"].Points.AddXY(a, yMin);
            chartIntegration.Series["Границы"].Points.AddXY(a, yMax);
            chartIntegration.Series["Границы"].Points.AddXY(double.NaN, double.NaN);
            chartIntegration.Series["Границы"].Points.AddXY(b, yMin);
            chartIntegration.Series["Границы"].Points.AddXY(b, yMax);

            // Настройка масштаба графика
            chartIntegration.ChartAreas[0].AxisX.Minimum = a - Math.Abs(b - a) * 0.1;
            chartIntegration.ChartAreas[0].AxisX.Maximum = b + Math.Abs(b - a) * 0.1;
            chartIntegration.ChartAreas[0].AxisY.Minimum = yMin;
            chartIntegration.ChartAreas[0].AxisY.Maximum = yMax;

            // Ось X (y=0)
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

        // Отрисовка прямоугольников
        private void DrawRectangles(string function, double a, double b, int n, double h)
        {
            for (int i = 0; i < n; i++)
            {
                double x1 = a + i * h;
                double x2 = x1 + h;
                double xLeft = x1; // Левая точка для прямоугольника

                try
                {
                    double y = EvaluateFunction(function, xLeft);

                    if (!double.IsInfinity(y) && !double.IsNaN(y))
                    {
                        // Определяем границы прямоугольника
                        double bottomY, topY;

                        if (y >= 0)
                        {
                            bottomY = 0;
                            topY = y;
                        }
                        else
                        {
                            bottomY = y;
                            topY = 0;
                        }

                        // Рисуем прямоугольник
                        chartIntegration.Series["Прямоугольники"].Points.AddXY(x1, bottomY);
                        chartIntegration.Series["Прямоугольники"].Points.AddXY(x1, topY);
                        chartIntegration.Series["Прямоугольники"].Points.AddXY(x2, topY);
                        chartIntegration.Series["Прямоугольники"].Points.AddXY(x2, bottomY);
                        chartIntegration.Series["Прямоугольники"].Points.AddXY(x1, bottomY);
                        chartIntegration.Series["Прямоугольники"].Points.AddXY(double.NaN, double.NaN);
                    }
                }
                catch
                {
                    // Пропускаем точки разрыва
                }
            }
        }

        // Отрисовка трапеций
        private void DrawTrapezoids(string function, double a, double b, int n, double h)
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
                        // Трапеция: точки (x1,0), (x1,y1), (x2,y2), (x2,0)
                        chartIntegration.Series["Трапеции"].Points.AddXY(x1, 0);
                        chartIntegration.Series["Трапеции"].Points.AddXY(x1, y1);
                        chartIntegration.Series["Трапеции"].Points.AddXY(x2, y2);
                        chartIntegration.Series["Трапеции"].Points.AddXY(x2, 0);
                        chartIntegration.Series["Трапеции"].Points.AddXY(x1, 0);
                        chartIntegration.Series["Трапеции"].Points.AddXY(double.NaN, double.NaN);
                    }
                }
                catch
                {
                    // Пропускаем точки разрыва
                }
            }
        }

        // Отрисовка парабол (Симпсон)
        private void DrawParabolas(string function, double a, double b, int n, double h)
        {
            if (n % 2 != 0) n++; // Нужно четное количество интервалов

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
                        // Аппроксимируем параболой через три точки
                        // Для простоты визуализации рисуем отрезки
                        chartIntegration.Series["Параболы"].Points.AddXY(x0, 0);
                        chartIntegration.Series["Параболы"].Points.AddXY(x0, y0);

                        // Точки на параболе (аппроксимация)
                        int segments = 10;
                        for (int j = 0; j <= segments; j++)
                        {
                            double t = (double)j / segments;
                            double x = x0 + 2 * h * t; // От x0 до x2
                            // Квадратичная интерполяция
                            double y = InterpolateQuadratic(x0, y0, x1, y1, x2, y2, x);
                            chartIntegration.Series["Параболы"].Points.AddXY(x, y);
                        }

                        chartIntegration.Series["Параболы"].Points.AddXY(x2, y2);
                        chartIntegration.Series["Параболы"].Points.AddXY(x2, 0);
                        chartIntegration.Series["Параболы"].Points.AddXY(x0, 0);
                        chartIntegration.Series["Параболы"].Points.AddXY(double.NaN, double.NaN);
                    }
                }
                catch
                {
                    // Пропускаем точки разрыва
                }
            }
        }

        // Квадратичная интерполяция
        private double InterpolateQuadratic(double x0, double y0, double x1, double y1, double x2, double y2, double x)
        {
            double l0 = ((x - x1) * (x - x2)) / ((x0 - x1) * (x0 - x2));
            double l1 = ((x - x0) * (x - x2)) / ((x1 - x0) * (x1 - x2));
            double l2 = ((x - x0) * (x - x1)) / ((x2 - x0) * (x2 - x1));

            return y0 * l0 + y1 * l1 + y2 * l2;
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
                    n = Math.Min(nValue, 1000);
                }
                else
                {
                    n = 10; // Для визуализации
                }

                // Останавливаем предыдущее вычисление
                if (_isCalculationRunning)
                {
                    _cancellationTokenSource?.Cancel();
                }

                // Визуализация
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

                        // Обновляем N
                        int estimatedN = (int)Math.Ceiling(Math.Abs(b - a) / Math.Sqrt(precision));
                        if (method == "Симпсон" && estimatedN % 2 != 0) estimatedN++;
                        textBoxN.Text = estimatedN.ToString();
                    }
                    else
                    {
                        result = CalculateIntegral(textBoxFunction.Text, a, b, n.Value, method);
                    }

                    // Вывод результата
                    textBoxResult.Text = result.ToString("F10");

                    // Вывод информации о методе
                    labelInfo.Text = $"Метод: {method}, N={n.Value}, Площадь={result:F6}";
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
                MessageBox.Show($"Ошибка: {ex.Message}",
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

            // Удаляем временные серии
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