using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NCalc;

namespace ProgramLab
{
    public partial class MPKS : Form
    {
        private string functionExpression;
        private double intervalA;
        private double intervalB;
        private int precision;
        private double initialX;
        private double stepH;
        private double minX;
        private double minY;

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

            // Устанавливаем формат отображения чисел на осях
            Chart.ChartAreas[0].AxisX.LabelStyle.Format = "0.###";
            Chart.ChartAreas[0].AxisY.LabelStyle.Format = "0.###";
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
                TextBoxXMin.Text = minX.ToString($"F{precision}", CultureInfo.InvariantCulture);
                TextBoxYMin.Text = minY.ToString($"F{precision}", CultureInfo.InvariantCulture);
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
            if (!double.TryParse(TextBoxA.Text.Replace('.', ','), NumberStyles.Any, CultureInfo.InvariantCulture, out a) ||
                !double.TryParse(TextBoxB.Text.Replace('.', ','), NumberStyles.Any, CultureInfo.InvariantCulture, out b))
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
            if (!double.TryParse(TextBoxX.Text.Replace('.', ','), NumberStyles.Any, CultureInfo.InvariantCulture, out x))
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
            // Преобразуем функцию для NCalc
            string input = TextBoxF.Text;

            // НЕ переводим в нижний регистр, чтобы сохранить регистр функций
            // NCalc чувствителен к регистру, стандартные функции: Sin, Cos, Tan, Exp, Log и т.д.

            // Обработка символа ^ - NCalc понимает его как возведение в степень
            // Оставляем ^ как есть

            // Обработка функций:
            // 1. sin -> Sin
            // 2. cos -> Cos
            // 3. tg -> Tan
            // 4. ctg -> (1/Tan)
            // 5. e(x) -> Exp(x)
            // 6. ln(x) -> Log(x) (натуральный логарифм)
            // 7. log(2,x) -> Log(2,x) - будет обработано в EvaluateFunction

            // Заменяем sin на Sin (NCalc требует заглавную букву)
            input = System.Text.RegularExpressions.Regex.Replace(input, @"\bsin\b", "Sin", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            input = System.Text.RegularExpressions.Regex.Replace(input, @"\bcos\b", "Cos", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            input = System.Text.RegularExpressions.Regex.Replace(input, @"\btan\b", "Tan", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            // Заменяем tg на Tan
            input = System.Text.RegularExpressions.Regex.Replace(input, @"\btg\b", "Tan", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            // Заменяем ctg на (1/Tan)
            input = System.Text.RegularExpressions.Regex.Replace(input, @"\bctg\b", "(1/Tan)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            // Заменяем exp на Exp
            input = System.Text.RegularExpressions.Regex.Replace(input, @"\bexp\b", "Exp", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            // Заменяем e(x) на Exp(x)
            input = System.Text.RegularExpressions.Regex.Replace(input, @"\be\(", "Exp(", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            // Заменяем ln на Log
            input = System.Text.RegularExpressions.Regex.Replace(input, @"\bln\b", "Log", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            // Для log - оставляем как есть, будет обработано в EvaluateFunction

            functionExpression = input;

            // Парсим числа с учетом культуры
            intervalA = double.Parse(TextBoxA.Text.Replace('.', ','), CultureInfo.InvariantCulture);
            intervalB = double.Parse(TextBoxB.Text.Replace('.', ','), CultureInfo.InvariantCulture);
            precision = int.Parse(TextBoxE.Text);
            initialX = double.Parse(TextBoxX.Text.Replace('.', ','), CultureInfo.InvariantCulture);
            stepH = double.Parse(TextBoxH.Text.Replace('.', ','), CultureInfo.InvariantCulture);
        }

        private void PlotFunction()
        {
            // Очищаем график
            Chart.Series["Function"].Points.Clear();
            Chart.Series["Descent"].Points.Clear();
            Chart.Series["LeftBoundary"].Points.Clear();
            Chart.Series["RightBoundary"].Points.Clear();

            // Для определения диапазона Y
            double yMin = double.MaxValue;
            double yMax = double.MinValue;

            // Количество точек для построения графика
            int pointsCount = 500;
            double step = (intervalB - intervalA) / pointsCount;

            // Собираем значения функции
            List<PointF> validPoints = new List<PointF>();

            for (int i = 0; i <= pointsCount; i++)
            {
                double x = intervalA + i * step;
                double y = EvaluateFunction(x);

                // Добавляем точку в список, даже если NaN (для обработки разрывов)
                validPoints.Add(new PointF((float)x, (float)y));

                // Обновляем диапазон Y только для корректных значений
                if (!double.IsNaN(y) && !double.IsInfinity(y))
                {
                    yMin = Math.Min(yMin, y);
                    yMax = Math.Max(yMax, y);
                }
            }

            // Если нет корректных значений, устанавливаем диапазон по умолчанию
            if (yMin == double.MaxValue || yMax == double.MinValue)
            {
                yMin = -10;
                yMax = 10;
            }
            else
            {
                // Добавляем запас по вертикали
                double yRange = yMax - yMin;
                if (yRange < 0.1) yRange = Math.Max(Math.Abs(yMin), Math.Abs(yMax)) * 2;
                if (yRange < 1) yRange = 1;
                yMin -= yRange * 0.1;
                yMax += yRange * 0.1;
            }

            // Рисуем все точки
            foreach (var point in validPoints)
            {
                Chart.Series["Function"].Points.AddXY(point.X, point.Y);
            }

            // Рисуем границы интервала
            Chart.Series["LeftBoundary"].Points.AddXY(intervalA, yMin);
            Chart.Series["LeftBoundary"].Points.AddXY(intervalA, yMax);

            Chart.Series["RightBoundary"].Points.AddXY(intervalB, yMin);
            Chart.Series["RightBoundary"].Points.AddXY(intervalB, yMax);

            // Настраиваем видимый диапазон
            double xRange = Math.Abs(intervalB - intervalA);
            Chart.ChartAreas[0].AxisX.Minimum = intervalA - xRange * 0.05;
            Chart.ChartAreas[0].AxisX.Maximum = intervalB + xRange * 0.05;
            Chart.ChartAreas[0].AxisY.Minimum = yMin;
            Chart.ChartAreas[0].AxisY.Maximum = yMax;

            // Обновляем формат осей
            Chart.ChartAreas[0].AxisX.LabelStyle.Format = "0.###";
            Chart.ChartAreas[0].AxisY.LabelStyle.Format = "0.###";
            Chart.Refresh();
        }

        private double EvaluateFunction(double xValue)
        {
            try
            {
                // Создаем выражение NCalc
                Expression expression = new Expression(functionExpression);

                // Сохраняем значение x в локальную переменную для использования в делегатах
                double localX = xValue;

                // Устанавливаем параметры
                expression.Parameters["x"] = localX;
                expression.Parameters["X"] = localX;

                // Добавляем математические константы
                expression.Parameters["pi"] = Math.PI;
                expression.Parameters["e"] = Math.E;

                // Настраиваем перехват параметров
                expression.EvaluateParameter += (name, args) =>
                {
                    if (name == "pi" || name == "PI")
                    {
                        args.Result = Math.PI;
                    }
                    else if (name == "e" || name == "E")
                    {
                        args.Result = Math.E;
                    }
                    else if (name == "x" || name == "X")
                    {
                        args.Result = localX;
                    }
                };

                // Настраиваем функции для NCalc
                expression.EvaluateFunction += (name, args) =>
                {
                    try
                    {
                        if (args.Parameters == null || args.Parameters.Length == 0)
                        {
                            args.Result = 0;
                            return;
                        }

                        // Используем отдельную переменную для параметра
                        double paramValue = 0;
                        object paramObj = args.Parameters[0].Evaluate();

                        if (paramObj is double)
                            paramValue = (double)paramObj;
                        else if (paramObj is int)
                            paramValue = (int)paramObj;
                        else if (paramObj is decimal)
                            paramValue = (double)(decimal)paramObj;
                        else if (paramObj is bool)
                            paramValue = (bool)paramObj ? 1 : 0;
                        else if (paramObj != null)
                            double.TryParse(paramObj.ToString(), out paramValue);

                        switch (name.ToLower())
                        {
                            case "sin":
                                args.Result = Math.Sin(paramValue);
                                break;
                            case "cos":
                                args.Result = Math.Cos(paramValue);
                                break;
                            case "tan":
                                // Обработка тангенса с проверкой на разрыв
                                double cos = Math.Cos(paramValue);
                                if (Math.Abs(cos) < 1e-15)
                                {
                                    args.Result = double.NaN;
                                }
                                else
                                {
                                    args.Result = Math.Tan(paramValue);
                                }
                                break;
                            case "exp":
                                // Экспонента e^x
                                args.Result = Math.Exp(paramValue);
                                break;
                            case "log":
                                // Обработка логарифма: может быть один или два параметра
                                if (args.Parameters.Length == 1)
                                {
                                    // Натуральный логарифм ln(x) или просто log(x)
                                    if (paramValue <= 0)
                                        args.Result = double.NaN;
                                    else
                                        args.Result = Math.Log(paramValue);
                                }
                                else if (args.Parameters.Length == 2)
                                {
                                    // Логарифм с основанием: log(2,x) или log(10,x)
                                    double baseValue = 0;
                                    object baseObj = args.Parameters[1].Evaluate();

                                    if (baseObj is double)
                                        baseValue = (double)baseObj;
                                    else if (baseObj is int)
                                        baseValue = (int)baseObj;
                                    else if (baseObj is decimal)
                                        baseValue = (double)(decimal)baseObj;
                                    else if (baseObj != null)
                                        double.TryParse(baseObj.ToString(), out baseValue);

                                    // Логарифм по основанию: log_base(paramValue) = Math.Log(paramValue) / Math.Log(baseValue)
                                    if (paramValue <= 0 || baseValue <= 0 || baseValue == 1)
                                        args.Result = double.NaN;
                                    else
                                        args.Result = Math.Log(paramValue) / Math.Log(baseValue);
                                }
                                break;
                            case "sqrt":
                                if (paramValue < 0)
                                    args.Result = double.NaN;
                                else
                                    args.Result = Math.Sqrt(paramValue);
                                break;
                            case "abs":
                                args.Result = Math.Abs(paramValue);
                                break;
                            case "pow":
                                if (args.Parameters.Length >= 2)
                                {
                                    double exponent = 0;
                                    object expObj = args.Parameters[1].Evaluate();

                                    if (expObj is double)
                                        exponent = (double)expObj;
                                    else if (expObj is int)
                                        exponent = (int)expObj;
                                    else if (expObj is decimal)
                                        exponent = (double)(decimal)expObj;
                                    else if (expObj != null)
                                        double.TryParse(expObj.ToString(), out exponent);

                                    args.Result = Math.Pow(paramValue, exponent);
                                }
                                break;
                            default:
                                // Если функция не распознана, возвращаем NaN
                                args.Result = double.NaN;
                                break;
                        }
                    }
                    catch
                    {
                        args.Result = double.NaN;
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
                else if (result is bool)
                {
                    return (bool)result ? 1 : 0;
                }
                else
                {
                    // Пытаемся преобразовать строку
                    if (result != null && double.TryParse(result.ToString(), out double parsed))
                    {
                        return parsed;
                    }
                    return double.NaN;
                }
            }
            catch (Exception ex)
            {
                // Для отладки можно вывести сообщение в консоль
                System.Diagnostics.Debug.WriteLine($"Ошибка вычисления функции в точке x={xValue}: {ex.Message}");
                return double.NaN;
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
                    double currentFAtBoundary = EvaluateFunction(currentX);

                    if (!double.IsNaN(currentFAtBoundary) && currentFAtBoundary < minY)
                    {
                        minX = currentX;
                        minY = currentFAtBoundary;
                    }

                    AddDescentPoint(currentX);
                    await Task.Delay(500, cancellationToken);
                    break;
                }

                // Вычисляем текущее значение функции
                double currentF = EvaluateFunction(currentX);

                // Пропускаем итерацию, если значение невалидное
                if (double.IsNaN(currentF) || double.IsInfinity(currentF))
                {
                    // Пробуем сместиться немного вправо
                    currentX += stepH;
                    continue;
                }

                // Первый этап: определение направления движения
                double x1 = currentX - stepH;
                double x2 = currentX + stepH;

                double f1 = double.MaxValue;
                double f2 = double.MaxValue;

                double f1Val = EvaluateFunction(x1);
                double f2Val = EvaluateFunction(x2);

                if (!double.IsNaN(f1Val) && !double.IsInfinity(f1Val))
                    f1 = f1Val;

                if (!double.IsNaN(f2Val) && !double.IsInfinity(f2Val))
                    f2 = f2Val;

                double newX = currentX;
                bool foundDirection = false;

                if (f1 < currentF && x1 >= intervalA)
                {
                    newX = x1; // Двигаемся влево
                    foundDirection = true;
                }
                else if (f2 < currentF && x2 <= intervalB)
                {
                    newX = x2; // Двигаемся вправо
                    foundDirection = true;
                }

                if (!foundDirection)
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

                // Пропускаем, если значение невалидное
                if (double.IsNaN(newF) || double.IsInfinity(newF))
                {
                    continue;
                }

                // Обновляем точку минимума
                if (newF < minY)
                {
                    minX = newX;
                    minY = newF;
                }

                if (newF < currentF)
                {
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

            // Проверяем, что значение корректное
            if (!double.IsNaN(y) && !double.IsInfinity(y))
            {
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

                        // Помечаем последнюю точку зеленым цветом
                        if (descentPoints.Count > 0 && Chart.Series["Descent"].Points.Count > 0)
                        {
                            Chart.Series["Descent"].Points.Last().Color = Color.Green;
                            Chart.Series["Descent"].Points.Last().MarkerSize = 10;
                        }
                    }));
                }
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