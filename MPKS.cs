using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ProgramLab
{
    public partial class MPKS : Form
    {
        private double PHI = (1 + Math.Sqrt(5)) / 2;
        private const double MAX_Y_VALUE = 10;
        private const double ZERO_TOLERANCE = 1e-10;
        private const double INFINITY_THRESHOLD = 1e10; // Порог для определения бесконечности

        // Для управления асинхронными операциями
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isSearchRunning = false;

        public MPKS()
        {
            InitializeComponent();
            SetupChart();
            SetupEventHandlers();
        }

        // Список функций без экстремумов
        private readonly List<string> NO_EXTREMUM_FUNCTIONS = new List<string>
        {
            "tan", "ctg", "exp", "log", "ln", "1/x", "x^-1", "e^x"
        };

        private void SetupChart()
        {
            chartFunc.Series.Clear();

            // Основной график функции
            Series functionSeries = new Series("Функция")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Blue,
                BorderWidth = 2
            };
            chartFunc.Series.Add(functionSeries);

            // Границы интервала (вертикальные линии)
            Series intervalSeries = new Series("Интервал")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Red,
                BorderWidth = 2,
                BorderDashStyle = ChartDashStyle.Dash
            };
            chartFunc.Series.Add(intervalSeries);

            // Точка минимума (результат)
            Series minSeries = new Series("Минимум")
            {
                ChartType = SeriesChartType.Point,
                Color = Color.Red,
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 10
            };
            chartFunc.Series.Add(minSeries);

            // Точка спуска (процесс поиска)
            Series descentSeries = new Series("Спуск")
            {
                ChartType = SeriesChartType.Point,
                Color = Color.Orange,
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 8
            };
            chartFunc.Series.Add(descentSeries);

            // Асимптоты (вертикальные линии в точках разрыва)
            Series asymptoteSeries = new Series("Асимптоты")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.DarkGray,
                BorderWidth = 1,
                BorderDashStyle = ChartDashStyle.DashDot
            };
            chartFunc.Series.Add(asymptoteSeries);

            // Ось X
            Series xAxisSeries = new Series("Ось X (y=0)")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.DarkGray,
                BorderWidth = 1,
                BorderDashStyle = ChartDashStyle.Dash
            };
            chartFunc.Series.Add(xAxisSeries);

            // Ось Y
            Series yAxisSeries = new Series("Ось Y (x=0)")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.DarkGray,
                BorderWidth = 1,
                BorderDashStyle = ChartDashStyle.Dash
            };
            chartFunc.Series.Add(yAxisSeries);

            // Настройка осей
            chartFunc.ChartAreas[0].AxisX.Title = "X";
            chartFunc.ChartAreas[0].AxisY.Title = "Y";
            chartFunc.ChartAreas[0].AxisX.Minimum = -10;
            chartFunc.ChartAreas[0].AxisX.Maximum = 10;
            chartFunc.ChartAreas[0].AxisY.Minimum = -MAX_Y_VALUE;
            chartFunc.ChartAreas[0].AxisY.Maximum = MAX_Y_VALUE;
        }

        private void SetupEventHandlers()
        {
            // Проверка ввода для числовых полей
            textBoxA.KeyPress += TextBoxNumber_KeyPress;
            textBoxB.KeyPress += TextBoxNumber_KeyPress;
            textBoxX.KeyPress += TextBoxNumber_KeyPress;
            textBoxH.KeyPress += TextBoxPositiveNumber_KeyPress;
            textBoxE.KeyPress += TextBoxEpsilon_KeyPress;
            textBoxF.KeyPress += TextBoxFunction_KeyPress;

            // Обработчики кнопок
            buttonStart.Click += ButtonStartCoordinateDescent_Click;
            buttonClear.Click += ButtonClear_Click;
            buttonStop.Click += ButtonStop_Click;
        }

        #region Обработчики ввода

        private void TextBoxNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            System.Windows.Forms.TextBox textBox = (System.Windows.Forms.TextBox)sender;
            string currentText = textBox.Text;
            int selectionStart = textBox.SelectionStart;

            // Разрешить управляющие символы (Backspace, Delete и т.д.)
            if (char.IsControl(e.KeyChar))
            {
                e.Handled = false;
                return;
            }

            // Разрешить знак минуса только в начале
            if (e.KeyChar == '-' && selectionStart == 0 && !currentText.Contains("-"))
            {
                e.Handled = false;
                return;
            }

            // Разрешить разделитель десятичных дробей (точка или запятая)
            if (e.KeyChar == '.' || e.KeyChar == ',')
            {
                if (!currentText.Contains('.') && !currentText.Contains(','))
                {
                    // Заменяем на разделитель текущей культуры
                    e.KeyChar = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
                    e.Handled = false;
                }
                else
                {
                    e.Handled = true;
                }
                return;
            }

            // Разрешить только цифры
            if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
                return;
            }

            e.Handled = false;
        }

        private void TextBoxPositiveNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            System.Windows.Forms.TextBox textBox = (System.Windows.Forms.TextBox)sender;
            string currentText = textBox.Text;

            // Разрешить управляющие символы
            if (char.IsControl(e.KeyChar))
            {
                e.Handled = false;
                return;
            }

            // НЕ разрешать знак минуса для положительных чисел
            if (e.KeyChar == '-')
            {
                e.Handled = true;
                return;
            }

            // Разрешить разделитель десятичных дробей
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

            // Разрешить только цифры
            if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
                return;
            }

            e.Handled = false;
        }

        private void TextBoxEpsilon_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Разрешить только цифры и управляющие символы
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void TextBoxFunction_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Запретить ввод кириллицы
            if ((e.KeyChar >= 'А' && e.KeyChar <= 'Я') ||
                (e.KeyChar >= 'а' && e.KeyChar <= 'я') ||
                e.KeyChar == 'ё' || e.KeyChar == 'Ё')
            {
                e.Handled = true;
                return;
            }

            e.Handled = false;
        }

        #endregion

        #region Вычисление функции

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

                // Обработка котангенса
                expression = Regex.Replace(expression, @"ctg\s*\(\s*(.*?)\s*\)", match =>
                {
                    string arg = match.Groups[1].Value;
                    return $"(1/tan({arg}))";
                });

                // Замена констант
                expression = expression
                    .Replace("pi", Math.PI.ToString(CultureInfo.InvariantCulture))
                    .Replace("e", Math.E.ToString(CultureInfo.InvariantCulture));

                // Замена переменной x
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

                // Обработка экспоненты
                expression = Regex.Replace(expression, @"exp\((.*?)\)", match =>
                {
                    double arg = EvaluateSimpleExpression(match.Groups[1].Value);
                    return Math.Exp(arg).ToString(CultureInfo.InvariantCulture);
                });

                // Обработка квадратного корня
                expression = Regex.Replace(expression, @"sqrt\((.*?)\)", match =>
                {
                    double arg = EvaluateSimpleExpression(match.Groups[1].Value);
                    if (arg < 0)
                        throw new ArgumentException("Корень из отрицательного числа");
                    return Math.Sqrt(arg).ToString(CultureInfo.InvariantCulture);
                });

                // Обработка логарифмов
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

                // Обработка степеней
                expression = ProcessPowers(expression);

                double result = EvaluateSimpleExpression(expression);

                // Проверка на бесконечность
                if (double.IsInfinity(result) || Math.Abs(result) > INFINITY_THRESHOLD)
                {
                    throw new OverflowException("Значение функции стремится к бесконечности");
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка вычисления функции в точке x={x}: {ex.Message}");
            }
        }

        private string ReplaceXInExpression(string expression, double x)
        {
            // Обработка выражений вида x^2, x^3 и т.д.
            expression = Regex.Replace(expression, @"x\^(\d+(?:\.\d+)?)", match =>
            {
                double exponent = double.Parse(match.Groups[1].Value);
                return Math.Pow(x, exponent).ToString(CultureInfo.InvariantCulture);
            });

            // Замена одиночного x
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

        #endregion

        #region Метод покоординатного спуска

        /// <summary>
        /// Метод покоординатного спуска для функции одной переменной
        /// Алгоритм:
        /// 1. Начинаем с начальной точки x0
        /// 2. Пока не достигнута точность или не вышли за границы интервала:
        ///    - Пробуем шаг влево: x1 = x - H
        ///    - Если f(x1) < f(x), двигаемся влево
        ///    - Иначе пробуем шаг вправо: x1 = x + H
        ///    - Если f(x1) < f(x), двигаемся вправо
        ///    - Иначе уменьшаем шаг H = H/2
        /// 3. Цикл продолжается, пока находится направление уменьшения функции
        /// </summary>
        private async Task CoordinateDescentAsync(string function, double a, double b,
            double startX, double step, double epsilon, CancellationToken cancellationToken)
        {
            double currentX = startX;
            double currentH = step;
            double previousValue = EvaluateFunction(function, currentX);
            int iteration = 0;
            const int maxIterations = 10000;

            // Очищаем серию точек спуска
            chartFunc.Series["Спуск"].Points.Clear();

            // Добавляем начальную точку
            chartFunc.Series["Спуск"].Points.AddXY(currentX, previousValue);
            chartFunc.Invalidate();

            // Основной цикл метода
            while (iteration < maxIterations && !cancellationToken.IsCancellationRequested)
            {
                iteration++;

                // Проверяем, не вышли ли за границы интервала
                if (currentX < a || currentX > b)
                {
                    AddStatusMessage($"Достигнута граница интервала [A, B] на итерации {iteration}");
                    break;
                }

                double leftX = currentX - currentH;
                double rightX = currentX + currentH;
                double leftValue = double.MaxValue;
                double rightValue = double.MaxValue;
                bool leftValid = false;
                bool rightValid = false;

                // Пробуем шаг влево (если не выходим за границу)
                if (leftX >= a)
                {
                    try
                    {
                        leftValue = EvaluateFunction(function, leftX);
                        leftValid = true;
                    }
                    catch
                    {
                        leftValid = false;
                    }
                }

                // Пробуем шаг вправо (если не выходим за границу)
                if (rightX <= b)
                {
                    try
                    {
                        rightValue = EvaluateFunction(function, rightX);
                        rightValid = true;
                    }
                    catch
                    {
                        rightValid = false;
                    }
                }

                double newX = currentX;
                double newValue = previousValue;
                bool foundBetter = false;

                // Выбираем направление с наименьшим значением функции
                if (leftValid && leftValue < previousValue)
                {
                    newX = leftX;
                    newValue = leftValue;
                    foundBetter = true;
                }

                if (rightValid && rightValue < previousValue && rightValue < leftValue)
                {
                    newX = rightX;
                    newValue = rightValue;
                    foundBetter = true;
                }

                // Если нашли лучшее значение, двигаемся
                if (foundBetter)
                {
                    // Обновляем точку
                    currentX = newX;

                    // Проверяем условие остановки по точности
                    if (Math.Abs(newValue - previousValue) < epsilon)
                    {
                        AddStatusMessage($"Достигнута заданная точность на итерации {iteration}");
                        break;
                    }

                    previousValue = newValue;

                    // Добавляем точку на график
                    chartFunc.Series["Спуск"].Points.AddXY(currentX, newValue);
                    chartFunc.Invalidate();

                    // Задержка для визуализации
                    await Task.Delay(500, cancellationToken);
                }
                else
                {
                    // Если не нашли лучшего направления, уменьшаем шаг
                    currentH /= 2.0;

                    // Проверяем условие остановки по шагу
                    if (currentH < epsilon)
                    {
                        AddStatusMessage($"Шаг стал меньше заданной точности на итерации {iteration}");
                        break;
                    }

                    AddStatusMessage($"Уменьшаем шаг до {currentH:F6} на итерации {iteration}");
                }
            }

            // Сохраняем результат
            if (!cancellationToken.IsCancellationRequested)
            {
                UpdateResult(currentX, previousValue);
            }
        }

        private void AddStatusMessage(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => AddStatusMessage(message)));
                return;
            }

            // Можно добавить статусное сообщение, если нужно
            // Например: statusLabel.Text = message;
        }

        private void UpdateResult(double x, double y)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateResult(x, y)));
                return;
            }

            // Записываем результат
            int precision = int.TryParse(textBoxE.Text, out int p) ? p : 6;
            textBoxXMin.Text = x.ToString($"F{precision}");
            textBoxYMin.Text = y.ToString($"F{precision}");

            // Отображаем найденный минимум на графике
            chartFunc.Series["Минимум"].Points.Clear();
            chartFunc.Series["Минимум"].Points.AddXY(x, y);
            chartFunc.Invalidate();
        }

        #endregion

        #region Поиск и отрисовка асимптот

        /// <summary>
        /// Находит точки разрыва функции (асимптоты) на заданном интервале
        /// </summary>
        private List<double> FindAsymptotes(string function, double a, double b, int samplePoints = 1000)
        {
            List<double> asymptotes = new List<double>();

            if (string.IsNullOrWhiteSpace(function))
                return asymptotes;

            double step = (b - a) / samplePoints;

            // Функции с известными асимптотами
            string funcLower = function.ToLower();

            // Для тангенса: асимптоты в точках pi/2 + pi*k
            if (funcLower.Contains("tan") || funcLower.Contains("ctg"))
            {
                // Находим точки вида pi/2 + pi*k в интервале [a, b]
                double pi = Math.PI;
                double startK = Math.Ceiling((a - pi / 2) / pi);
                double endK = Math.Floor((b - pi / 2) / pi);

                for (double k = startK; k <= endK; k++)
                {
                    double asymptotePoint = pi / 2 + pi * k;
                    if (asymptotePoint >= a && asymptotePoint <= b)
                    {
                        asymptotes.Add(asymptotePoint);
                    }
                }

                // Для котангенса: асимптоты в точках pi*k
                if (funcLower.Contains("ctg"))
                {
                    startK = Math.Ceiling(a / pi);
                    endK = Math.Floor(b / pi);

                    for (double k = startK; k <= endK; k++)
                    {
                        double asymptotePoint = pi * k;
                        if (asymptotePoint >= a && asymptotePoint <= b)
                        {
                            asymptotes.Add(asymptotePoint);
                        }
                    }
                }
            }

            // Для 1/x или дробей вида 1/(x-c)
            if (funcLower.Contains("1/x") || funcLower.Contains("1/") && funcLower.Contains("x"))
            {
                // Ищем точки, где знаменатель равен 0
                // Это упрощенная проверка - в реальности нужно парсить выражение
                if (a <= 0 && b >= 0)
                {
                    asymptotes.Add(0);
                }
            }

            // Для дробей общего вида - находим разрывы численным методом
            for (int i = 1; i < samplePoints; i++)
            {
                double x = a + i * step;
                double prevX = a + (i - 1) * step;
                double nextX = a + (i + 1) * step;

                try
                {
                    double y1 = EvaluateFunction(function, prevX);
                    double y2 = EvaluateFunction(function, nextX);

                    // Если значения очень большие и разных знаков - возможная асимптота
                    if ((Math.Abs(y1) > INFINITY_THRESHOLD / 10 || Math.Abs(y2) > INFINITY_THRESHOLD / 10) &&
                        Math.Sign(y1) != Math.Sign(y2))
                    {
                        // Уточняем положение асимптоты методом деления отрезка пополам
                        double left = prevX;
                        double right = nextX;

                        for (int j = 0; j < 20; j++) // 20 итераций для точности
                        {
                            double mid = (left + right) / 2;
                            try
                            {
                                double yMid = EvaluateFunction(function, mid);
                                if (Math.Abs(yMid) > INFINITY_THRESHOLD / 10)
                                {
                                    right = mid;
                                }
                                else
                                {
                                    left = mid;
                                }
                            }
                            catch
                            {
                                right = mid;
                            }
                        }

                        double asymptotePoint = (left + right) / 2;

                        // Проверяем, не добавили ли уже эту точку
                        if (!asymptotes.Any(p => Math.Abs(p - asymptotePoint) < step / 10))
                        {
                            asymptotes.Add(asymptotePoint);
                        }
                    }
                }
                catch
                {
                    // Пропускаем точки, где функция не определена
                }
            }

            return asymptotes.Distinct().OrderBy(x => x).ToList();
        }

        /// <summary>
        /// Рисует асимптоты на графике
        /// </summary>
        private void DrawAsymptotes(List<double> asymptotes, double yMin, double yMax)
        {
            chartFunc.Series["Асимптоты"].Points.Clear();

            foreach (double asymptoteX in asymptotes)
            {
                // Добавляем вертикальную линию асимптоты
                chartFunc.Series["Асимптоты"].Points.AddXY(asymptoteX, yMin);
                chartFunc.Series["Асимптоты"].Points.AddXY(asymptoteX, yMax);
                // Добавляем разрыв между линиями (для разделения асимптот)
                chartFunc.Series["Асимптоты"].Points.AddXY(double.NaN, double.NaN);
            }
        }

        #endregion

        #region Обработчики кнопок

        private async void ButtonStartCoordinateDescent_Click(object sender, EventArgs e)
        {
            try
            {
                // Проверка введенных данных
                if (string.IsNullOrWhiteSpace(textBoxF.Text))
                {
                    MessageBox.Show("Введите функцию для анализа", "Ошибка ввода",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!double.TryParse(textBoxA.Text.Replace(",", "."), NumberStyles.Any,
                    CultureInfo.InvariantCulture, out double a))
                {
                    MessageBox.Show("Некорректное значение A. Введите число", "Ошибка ввода",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!double.TryParse(textBoxB.Text.Replace(",", "."), NumberStyles.Any,
                    CultureInfo.InvariantCulture, out double b))
                {
                    MessageBox.Show("Некорректное значение B. Введите число", "Ошибка ввода",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (a >= b)
                {
                    MessageBox.Show("Значение A должно быть меньше B", "Ошибка интервала",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!double.TryParse(textBoxX.Text.Replace(",", "."), NumberStyles.Any,
                    CultureInfo.InvariantCulture, out double startX))
                {
                    MessageBox.Show("Некорректное значение начальной точки X. Введите число",
                        "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!double.TryParse(textBoxH.Text.Replace(",", "."), NumberStyles.Any,
                    CultureInfo.InvariantCulture, out double step))
                {
                    MessageBox.Show("Некорректное значение шага H. Введите положительное число",
                        "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (step <= 0)
                {
                    MessageBox.Show("Шаг H должен быть положительным числом", "Ошибка ввода",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!int.TryParse(textBoxE.Text, out int precision) || precision < 0)
                {
                    MessageBox.Show("Точность должна быть целым положительным числом",
                        "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Проверяем, что начальная точка в интервале
                if (startX < a || startX > b)
                {
                    MessageBox.Show($"Начальная точка X должна быть в интервале [{a}, {b}]",
                        "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Останавливаем предыдущий поиск, если он запущен
                if (_isSearchRunning)
                {
                    _cancellationTokenSource?.Cancel();
                }

                // Строим график функции с асимптотами
                PlotFunction(textBoxF.Text, a, b);

                // Запускаем метод покоординатного спуска
                _cancellationTokenSource = new CancellationTokenSource();
                _isSearchRunning = true;
                buttonStart.Enabled = false;
                buttonStop.Enabled = true;

                try
                {
                    double epsilon = Math.Pow(10, -precision);
                    await CoordinateDescentAsync(textBoxF.Text, a, b, startX, step,
                        epsilon, _cancellationTokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                    // Поиск был отменен пользователем
                    MessageBox.Show("Поиск минимума прерван пользователем", "Информация",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                finally
                {
                    _isSearchRunning = false;
                    buttonStart.Enabled = true;
                    buttonStop.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выполнении поиска: {ex.Message}",
                    "Ошибка выполнения", MessageBoxButtons.OK, MessageBoxIcon.Error);

                _isSearchRunning = false;
                buttonStart.Enabled = true;
                buttonStop.Enabled = false;
            }
        }

        private void ButtonStop_Click(object sender, EventArgs e)
        {
            if (_isSearchRunning && _cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                buttonStop.Enabled = false;
            }
        }

        private void ButtonChart_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(textBoxF.Text))
                {
                    MessageBox.Show("Введите функцию для построения графика", "Ошибка ввода",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!double.TryParse(textBoxA.Text.Replace(",", "."), NumberStyles.Any,
                    CultureInfo.InvariantCulture, out double a))
                {
                    MessageBox.Show("Некорректное значение A. Введите число", "Ошибка ввода",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!double.TryParse(textBoxB.Text.Replace(",", "."), NumberStyles.Any,
                    CultureInfo.InvariantCulture, out double b))
                {
                    MessageBox.Show("Некорректное значение B. Введите число", "Ошибка ввода",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (a >= b)
                {
                    MessageBox.Show("Значение A должно быть меньше B", "Ошибка интервала",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                PlotFunction(textBoxF.Text, a, b);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при построении графика: {ex.Message}",
                    "Ошибка выполнения", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ButtonClear_Click(object sender, EventArgs e)
        {
            // Останавливаем поиск, если он запущен
            if (_isSearchRunning)
            {
                _cancellationTokenSource?.Cancel();
                _isSearchRunning = false;
            }

            // Очищаем текстовые поля
            textBoxF.Clear();
            textBoxA.Clear();
            textBoxB.Clear();
            textBoxX.Clear();
            textBoxH.Clear();
            textBoxE.Clear();
            textBoxXMin.Clear();
            textBoxYMin.Clear();

            // Очищаем график
            foreach (Series series in chartFunc.Series)
            {
                series.Points.Clear();
            }

            // Сбрасываем настройки осей
            chartFunc.ChartAreas[0].AxisX.Minimum = -10;
            chartFunc.ChartAreas[0].AxisX.Maximum = 10;
            chartFunc.ChartAreas[0].AxisY.Minimum = -MAX_Y_VALUE;
            chartFunc.ChartAreas[0].AxisY.Maximum = MAX_Y_VALUE;

            chartFunc.Invalidate();
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            // Останавливаем поиск перед переходом
            if (_isSearchRunning)
            {
                _cancellationTokenSource?.Cancel();
            }

            Main main = new Main();
            main.Show();
            Close();
        }

        #endregion

        #region Визуализация

        private void PlotFunction(string function, double a, double b)
        {
            // Очищаем все серии
            chartFunc.Series["Функция"].Points.Clear();
            chartFunc.Series["Интервал"].Points.Clear();
            chartFunc.Series["Минимум"].Points.Clear();
            chartFunc.Series["Спуск"].Points.Clear();
            chartFunc.Series["Асимптоты"].Points.Clear();
            chartFunc.Series["Ось X (y=0)"].Points.Clear();
            chartFunc.Series["Ось Y (x=0)"].Points.Clear();

            // Настройка границ графика
            double xMin = Math.Min(a, b) - Math.Abs(b - a) * 0.2;
            double xMax = Math.Max(a, b) + Math.Abs(b - a) * 0.2;

            chartFunc.ChartAreas[0].AxisX.Minimum = xMin;
            chartFunc.ChartAreas[0].AxisX.Maximum = xMax;

            // Построение функции
            int pointsCount = 1000;
            double step = (b - a) / pointsCount;
            List<double> validYValues = new List<double>();

            // Находим асимптоты
            List<double> asymptotes = FindAsymptotes(function, a, b);

            for (int i = 0; i <= pointsCount; i++)
            {
                double x = a + i * step;

                // Проверяем, не находимся ли мы слишком близко к асимптоте
                bool nearAsymptote = asymptotes.Any(asymptote => Math.Abs(x - asymptote) < step / 100);

                if (nearAsymptote)
                {
                    // Пропускаем точки рядом с асимптотой
                    chartFunc.Series["Функция"].Points.AddXY(x, double.NaN);
                    continue;
                }

                try
                {
                    double y = EvaluateFunction(function, x);

                    if (double.IsInfinity(y) || double.IsNaN(y))
                    {
                        // Разрыв функции - добавляем NaN для разрыва линии
                        chartFunc.Series["Функция"].Points.AddXY(x, double.NaN);
                    }
                    else
                    {
                        chartFunc.Series["Функция"].Points.AddXY(x, y);
                        validYValues.Add(y);
                    }
                }
                catch (Exception ex) when (ex.Message.Contains("бесконечности") ||
                                          ex.Message.Contains("не определен") ||
                                          ex is DivideByZeroException)
                {
                    // Точка разрыва - добавляем NaN
                    chartFunc.Series["Функция"].Points.AddXY(x, double.NaN);
                }
                catch
                {
                    // Другие ошибки - пропускаем точку
                    chartFunc.Series["Функция"].Points.AddXY(x, double.NaN);
                }
            }

            // Настройка масштаба по оси Y
            double yMinChart, yMaxChart;
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

                yMinChart = yMin - yRange * 0.1;
                yMaxChart = yMax + yRange * 0.1;

                chartFunc.ChartAreas[0].AxisY.Minimum = yMinChart;
                chartFunc.ChartAreas[0].AxisY.Maximum = yMaxChart;
            }
            else
            {
                yMinChart = chartFunc.ChartAreas[0].AxisY.Minimum;
                yMaxChart = chartFunc.ChartAreas[0].AxisY.Maximum;
            }

            // Ограничиваем масштаб для функций с большими значениями
            if (IsFunctionWithoutExtremums(function))
            {
                chartFunc.ChartAreas[0].AxisY.Minimum = Math.Max(chartFunc.ChartAreas[0].AxisY.Minimum, -MAX_Y_VALUE);
                chartFunc.ChartAreas[0].AxisY.Maximum = Math.Min(chartFunc.ChartAreas[0].AxisY.Maximum, MAX_Y_VALUE);
            }

            // Отрисовка вертикальных линий интервала
            double lineY1 = chartFunc.ChartAreas[0].AxisY.Minimum;
            double lineY2 = chartFunc.ChartAreas[0].AxisY.Maximum;

            chartFunc.Series["Интервал"].Points.AddXY(a, lineY1);
            chartFunc.Series["Интервал"].Points.AddXY(a, lineY2);
            chartFunc.Series["Интервал"].Points.AddXY(double.NaN, double.NaN); // Разрыв

            chartFunc.Series["Интервал"].Points.AddXY(b, lineY1);
            chartFunc.Series["Интервал"].Points.AddXY(b, lineY2);

            // Отрисовка асимптот
            DrawAsymptotes(asymptotes, lineY1, lineY2);

            // Отрисовка осей координат
            if (chartFunc.ChartAreas[0].AxisY.Minimum <= 0 &&
                chartFunc.ChartAreas[0].AxisY.Maximum >= 0)
            {
                chartFunc.Series["Ось X (y=0)"].Points.AddXY(xMin, 0);
                chartFunc.Series["Ось X (y=0)"].Points.AddXY(xMax, 0);
            }

            if (chartFunc.ChartAreas[0].AxisX.Minimum <= 0 &&
                chartFunc.ChartAreas[0].AxisX.Maximum >= 0)
            {
                double yMinAxis = chartFunc.ChartAreas[0].AxisY.Minimum;
                double yMaxAxis = chartFunc.ChartAreas[0].AxisY.Maximum;
                chartFunc.Series["Ось Y (x=0)"].Points.AddXY(0, yMinAxis);
                chartFunc.Series["Ось Y (x=0)"].Points.AddXY(0, yMaxAxis);
            }

            chartFunc.Invalidate();
        }

        #endregion

        private void buttonBack_Click_1(object sender, EventArgs e)
        {
            Main main = new Main(); 
            main.Show();
            Close();
        }
    }
}