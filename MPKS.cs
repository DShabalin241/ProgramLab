using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NCalc;

namespace ProgramLab
{
    public partial class MPKS : Form
    {
        // Приватные поля для хранения данных
        private string functionExpression;
        private double intervalA;
        private double intervalB;
        private int precision;
        private double initialX;
        private double stepH;
        private double minX;
        private double minY;

        // Для управления выполнением
        private CancellationTokenSource cancellationTokenSource;
        private bool isRunning = false;

        // Для хранения точек спуска
        private List<PointF> descentPoints = new List<PointF>();

        public MPKS()
        {
            InitializeComponent();
            SetupEventHandlers();
        }

        private void SetupEventHandlers()
        {
            // Запрет ввода кириллицы в поле функции
            TextBoxF.KeyPress += (s, e) =>
            {
                // Разрешаем только латинские буквы, цифры и математические символы
                if ((e.KeyChar >= 'А' && e.KeyChar <= 'я') || e.KeyChar == 'ё' || e.KeyChar == 'Ё')
                {
                    e.Handled = true;
                    MessageBox.Show("Ввод кириллицы запрещен. Используйте только латинские символы для функции.",
                        "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            // Валидация числовых полей
            SetupNumericTextBox(TextBoxA);
            SetupNumericTextBox(TextBoxB);
            SetupNumericTextBox(TextBoxX);
            SetupPositiveNumericTextBox(TextBoxH);
            SetupPositiveIntegerTextBox(TextBoxE);

            // Обработчики кнопок
            ButtonStart.Click += async (s, e) => await StartDescentAsync();
            ButtonStop.Click += StopDescent;
            ButtonClear.Click += ClearAll;

            // Инициализация графика
            InitializeChart();
        }

        private void SetupNumericTextBox(TextBox textBox)
        {
            textBox.KeyPress += (s, e) =>
            {
                // Разрешаем цифры, минус, запятую, точку и Backspace
                bool isDigit = char.IsDigit(e.KeyChar);
                bool isControl = char.IsControl(e.KeyChar);
                bool isDecimalSeparator = e.KeyChar == ',' || e.KeyChar == '.';
                bool isMinus = e.KeyChar == '-';

                // Разрешаем минус только в начале
                if (isMinus && textBox.SelectionStart != 0)
                {
                    e.Handled = true;
                    return;
                }

                // Преобразуем точку в запятую
                if (e.KeyChar == '.')
                {
                    e.KeyChar = ',';
                }

                // Если символ не цифра, не управляющий и не разделитель - запрещаем
                if (!(isDigit || isControl || isDecimalSeparator || isMinus))
                {
                    e.Handled = true;
                }

                // Проверяем, чтобы разделитель был только один
                if (isDecimalSeparator && textBox.Text.Contains(','))
                {
                    e.Handled = true;
                }
            };
        }

        private void SetupPositiveNumericTextBox(TextBox textBox)
        {
            textBox.KeyPress += (s, e) =>
            {
                // Разрешаем цифры, запятую, точку и Backspace
                bool isDigit = char.IsDigit(e.KeyChar);
                bool isControl = char.IsControl(e.KeyChar);
                bool isDecimalSeparator = e.KeyChar == ',' || e.KeyChar == '.';

                // Преобразуем точку в запятую
                if (e.KeyChar == '.')
                {
                    e.KeyChar = ',';
                }

                if (!(isDigit || isControl || isDecimalSeparator))
                {
                    e.Handled = true;
                }

                // Проверяем, чтобы разделитель был только один
                if (isDecimalSeparator && textBox.Text.Contains(','))
                {
                    e.Handled = true;
                }
            };
        }

        private void SetupPositiveIntegerTextBox(TextBox textBox)
        {
            textBox.KeyPress += (s, e) =>
            {
                // Разрешаем только цифры и Backspace
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            };
        }

        private void InitializeChart()
        {
            Chart.Series.Clear();

            // Основной график функции
            var functionSeries = Chart.Series.Add("Function");
            functionSeries.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            functionSeries.Color = Color.Blue;
            functionSeries.BorderWidth = 2;

            // Точки спуска
            var descentSeries = Chart.Series.Add("Descent");
            descentSeries.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            descentSeries.Color = Color.Red;
            descentSeries.MarkerSize = 8;
            descentSeries.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;

            // Вертикальные линии интервала
            var leftBoundarySeries = Chart.Series.Add("LeftBoundary");
            leftBoundarySeries.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            leftBoundarySeries.Color = Color.Green;
            leftBoundarySeries.BorderWidth = 2;

            var rightBoundarySeries = Chart.Series.Add("RightBoundary");
            rightBoundarySeries.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            rightBoundarySeries.Color = Color.Green;
            rightBoundarySeries.BorderWidth = 2;

            // Настройка осей
            Chart.ChartAreas[0].AxisX.Crossing = 0;
            Chart.ChartAreas[0].AxisY.Crossing = 0;
            Chart.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.LightGray;
            Chart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightGray;
        }

        private async Task StartDescentAsync()
        {
            if (isRunning)
            {
                MessageBox.Show("Вычисление уже выполняется", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!ValidateInput())
            {
                return;
            }

            // Инициализация отмены
            cancellationTokenSource = new CancellationTokenSource();
            isRunning = true;
            ButtonStart.Enabled = false;
            ButtonStop.Enabled = true;

            try
            {
                // Получаем данные из полей
                ParseInput();

                // Очищаем предыдущие точки спуска
                descentPoints.Clear();

                // Строим график функции
                PlotFunction();

                // Запускаем метод покоординатного спуска
                await Task.Run(() => PerformCoordinateDescentAsync(cancellationTokenSource.Token));
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Вычисление прервано пользователем", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выполнении: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                isRunning = false;
                ButtonStart.Enabled = true;
                ButtonStop.Enabled = false;

                // Выводим результат
                TextBoxXMin.Text = minX.ToString($"F{precision}");
                TextBoxYMin.Text = minY.ToString($"F{precision}");
            }
        }

        private bool ValidateInput()
        {
            // Проверка заполнения всех полей
            if (string.IsNullOrWhiteSpace(TextBoxF.Text))
            {
                MessageBox.Show("Функция не задана. Введите математическое выражение.",
                    "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(TextBoxA.Text) || string.IsNullOrWhiteSpace(TextBoxB.Text))
            {
                MessageBox.Show("Интервал не задан полностью.",
                    "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(TextBoxE.Text))
            {
                MessageBox.Show("Точность не задана.",
                    "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(TextBoxX.Text))
            {
                MessageBox.Show("Начальная точка не задана.",
                    "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(TextBoxH.Text))
            {
                MessageBox.Show("Шаг не задан.",
                    "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Проверка интервала
            double a, b;
            if (!double.TryParse(TextBoxA.Text.Replace('.', ','), out a) ||
                !double.TryParse(TextBoxB.Text.Replace('.', ','), out b))
            {
                MessageBox.Show("Некорректный формат чисел в интервале.",
                    "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (a >= b)
            {
                MessageBox.Show("Левая граница интервала (A) должна быть меньше правой (B).",
                    "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Проверка начальной точки
            double x;
            if (!double.TryParse(TextBoxX.Text.Replace('.', ','), out x))
            {
                MessageBox.Show("Некорректный формат начальной точки.",
                    "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (x < a || x > b)
            {
                MessageBox.Show("Начальная точка должна находиться внутри интервала [A, B].",
                    "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void ParseInput()
        {
            // Преобразуем функцию: оставляем ^ как есть - NCalc понимает его как возведение в степень
            functionExpression = TextBoxF.Text
                .Replace("sin", "Sin")  // NCalc требует заглавные буквы
                .Replace("cos", "Cos")
                .Replace("tan", "Tan")
                .Replace("tg", "Tan")
                .Replace("ctg", "Cot")
                .Replace("log", "Log")
                .Replace("ln", "Log")
                .Replace("exp", "Exp")
                .Replace("sqrt", "Sqrt");

            // Парсим числа
            intervalA = double.Parse(TextBoxA.Text.Replace('.', ','));
            intervalB = double.Parse(TextBoxB.Text.Replace('.', ','));
            precision = int.Parse(TextBoxE.Text);
            initialX = double.Parse(TextBoxX.Text.Replace('.', ','));
            stepH = double.Parse(TextBoxH.Text.Replace('.', ','));
        }

        private void PlotFunction()
        {
            // Очищаем график
            Chart.Series["Function"].Points.Clear();
            Chart.Series["Descent"].Points.Clear();
            Chart.Series["LeftBoundary"].Points.Clear();
            Chart.Series["RightBoundary"].Points.Clear();

            // Для хранения точек графика
            List<PointF> functionPoints = new List<PointF>();

            // Рисуем вертикальные линии границ
            double yMin = double.MaxValue;
            double yMax = double.MinValue;

            // Собираем значения функции для определения диапазона Y
            int pointsCount = 500;
            double step = (intervalB - intervalA) / pointsCount;

            for (int i = 0; i <= pointsCount; i++)
            {
                double x = intervalA + i * step;
                try
                {
                    double y = EvaluateFunction(x);

                    if (!double.IsInfinity(y) && !double.IsNaN(y))
                    {
                        // Добавляем точку в список
                        functionPoints.Add(new PointF((float)x, (float)y));

                        // Обновляем диапазон Y
                        yMin = Math.Min(yMin, y);
                        yMax = Math.Max(yMax, y);
                    }
                    else
                    {
                        // Для разрывов добавляем точку с NaN
                        functionPoints.Add(new PointF((float)x, float.NaN));
                    }
                }
                catch
                {
                    // Ошибка вычисления - добавляем точку с NaN
                    functionPoints.Add(new PointF((float)x, float.NaN));
                }
            }

            // Если нет корректных значений, устанавливаем диапазон по умолчанию
            if (yMin == double.MaxValue || yMax == double.MinValue)
            {
                yMin = -10;
                yMax = 10;
            }

            // Добавляем запас по вертикали
            double yRange = yMax - yMin;
            yMin -= yRange * 0.1;
            yMax += yRange * 0.1;

            // Рисуем границы интервала
            Chart.Series["LeftBoundary"].Points.AddXY(intervalA, yMin);
            Chart.Series["LeftBoundary"].Points.AddXY(intervalA, yMax);

            Chart.Series["RightBoundary"].Points.AddXY(intervalB, yMin);
            Chart.Series["RightBoundary"].Points.AddXY(intervalB, yMax);

            // Рисуем саму функцию - ВАЖНО: точки должны быть отсортированы по X
            // Сортируем точки по координате X
            functionPoints.Sort((p1, p2) => p1.X.CompareTo(p2.X));

            // Добавляем точки на график в правильном порядке
            foreach (var point in functionPoints)
            {
                if (float.IsNaN(point.Y))
                {
                    // Для разрывов добавляем пустую точку
                    Chart.Series["Function"].Points.AddXY(point.X, double.NaN);
                }
                else
                {
                    Chart.Series["Function"].Points.AddXY(point.X, point.Y);
                }
            }

            // Настраиваем видимый диапазон
            Chart.ChartAreas[0].AxisX.Minimum = intervalA - Math.Abs(intervalB - intervalA) * 0.1;
            Chart.ChartAreas[0].AxisX.Maximum = intervalB + Math.Abs(intervalB - intervalA) * 0.1;
            Chart.ChartAreas[0].AxisY.Minimum = yMin;
            Chart.ChartAreas[0].AxisY.Maximum = yMax;

            // Обновляем график
            Chart.Refresh();
        }

        private double EvaluateFunction(double x)
        {
            try
            {
                // Создаем выражение NCalc
                Expression expression = new Expression(functionExpression);

                // Устанавливаем параметры
                expression.Parameters["x"] = x;
                expression.Parameters["X"] = x;

                // Добавляем математические функции
                expression.EvaluateParameter += (name, args) =>
                {
                    if (name == "pi" || name == "PI") args.Result = Math.PI;
                    if (name == "e" || name == "E") args.Result = Math.E;
                };

                // Добавляем обработку функции pow для совместимости
                expression.EvaluateFunction += (name, args) =>
                {
                    if (name.ToLower() == "pow" && args.Parameters.Length == 2)
                    {
                        try
                        {
                            double baseValue = Convert.ToDouble(args.Parameters[0].Evaluate());
                            double exponent = Convert.ToDouble(args.Parameters[1].Evaluate());
                            args.Result = Math.Pow(baseValue, exponent);
                        }
                        catch
                        {
                            args.Result = double.NaN;
                        }
                    }
                };

                // Вычисляем результат
                object result = expression.Evaluate();

                if (result is double)
                {
                    return (double)result;
                }
                else if (result is int)
                {
                    return (int)result;
                }
                else if (result is decimal)
                {
                    return (double)(decimal)result;
                }
                else
                {
                    throw new InvalidOperationException("Неверный тип результата функции");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка вычисления функции в точке x={x}: {ex.Message}");
            }
        }

        private async Task PerformCoordinateDescentAsync(CancellationToken cancellationToken)
        {
            double currentX = initialX;
            double epsilon = Math.Pow(10, -precision);
            double previousMin = double.MaxValue;

            // Инициализируем минимум
            minX = currentX;
            minY = EvaluateFunction(currentX);

            // Основной цикл метода покоординатного спуска
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }

                // Проверяем границы интервала
                if (currentX < intervalA || currentX > intervalB)
                {
                    // Если вышли за границы, возвращаемся к ближайшей границе
                    currentX = Math.Max(intervalA, Math.Min(currentX, intervalB));
                    AddDescentPoint(currentX);
                    await Task.Delay(500);
                    break;
                }

                // Вычисляем текущее значение функции
                double currentF = EvaluateFunction(currentX);

                // Первый этап: определение направления движения
                double x1 = currentX - stepH;
                double x2 = currentX + stepH;

                double f1 = EvaluateFunction(x1);
                double f2 = EvaluateFunction(x2);

                double newX;
                if (f1 < currentF && x1 >= intervalA)
                {
                    newX = x1; // Двигаемся влево
                }
                else if (f2 < currentF && x2 <= intervalB)
                {
                    newX = x2; // Двигаемся вправо
                }
                else
                {
                    // Если оба направления не улучшают значение, уменьшаем шаг
                    if (stepH > epsilon * 10)
                    {
                        stepH /= 2;
                        continue;
                    }
                    else
                    {
                        // Достигли минимума
                        break;
                    }
                }

                // Второй этап: проверка условия остановки
                double newF = EvaluateFunction(newX);

                // Обновляем точку минимума
                if (newF < currentF)
                {
                    minX = newX;
                    minY = newF;
                    currentX = newX;

                    // Добавляем точку на график
                    AddDescentPoint(currentX);

                    // Задержка для визуализации
                    await Task.Delay(500, cancellationToken);
                }
                else
                {
                    // Если улучшение незначительное, останавливаемся
                    if (Math.Abs(newF - currentF) < epsilon)
                    {
                        break;
                    }
                }

                // Проверяем сходимость
                if (Math.Abs(previousMin - newF) < epsilon)
                {
                    break;
                }

                previousMin = newF;

                // Проверяем, достигли ли границы интервала
                if (Math.Abs(currentX - intervalA) < epsilon ||
                    Math.Abs(currentX - intervalB) < epsilon)
                {
                    break;
                }
            }
        }

        private void AddDescentPoint(double x)
        {
            // Вычисляем значение функции в точке
            double y = EvaluateFunction(x);

            // Добавляем в список точек спуска
            descentPoints.Add(new PointF((float)x, (float)y));

            // Обновляем график в UI потоке
            if (Chart.InvokeRequired)
            {
                Chart.Invoke(new Action(() =>
                {
                    Chart.Series["Descent"].Points.Clear();
                    foreach (var point in descentPoints)
                    {
                        Chart.Series["Descent"].Points.AddXY(point.X, point.Y);
                    }

                    // Помечаем последнюю точку
                    if (descentPoints.Count > 0)
                    {
                        var lastPoint = descentPoints.Last();
                        Chart.Series["Descent"].Points.Last().Color = Color.Green;
                        Chart.Series["Descent"].Points.Last().MarkerSize = 10;
                    }
                }));
            }
        }

        private void StopDescent(object sender, EventArgs e)
        {
            if (cancellationTokenSource != null && isRunning)
            {
                cancellationTokenSource.Cancel();
                isRunning = false;
                ButtonStart.Enabled = true;
                ButtonStop.Enabled = false;
            }
        }

        private void ClearAll(object sender, EventArgs e)
        {
            // Очищаем все поля ввода
            TextBoxF.Clear();
            TextBoxA.Clear();
            TextBoxB.Clear();
            TextBoxE.Clear();
            TextBoxX.Clear();
            TextBoxH.Clear();
            TextBoxXMin.Clear();
            TextBoxYMin.Clear();

            // Очищаем график
            Chart.Series.Clear();
            InitializeChart();

            // Сбрасываем состояние
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
                cancellationTokenSource = null;
            }

            isRunning = false;
            ButtonStart.Enabled = true;
            ButtonStop.Enabled = false;
            descentPoints.Clear();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Останавливаем выполнение при закрытии формы
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
            }
            base.OnFormClosing(e);
        }
    }
}