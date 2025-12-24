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

namespace ProgramLab
{
    public partial class Newton : Form
    {
        private const double MAX_Y_VALUE = 10;
        private const double ZERO_TOLERANCE = 1e-10;
        private const double INFINITY_THRESHOLD = 1e10;

        // Список функций без экстремумов (минимумов/максимумов)
        private readonly List<string> NO_EXTREMUM_FUNCTIONS = new List<string>
        {
            "tan", "ctg", "log", "ln", "1/x", "x^-1"
        };

        // Для управления асинхронными операциями
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isSearchRunning = false;

        // Цвета для визуализации
        private readonly Color FUNCTION_COLOR = Color.Blue;
        private readonly Color TANGENT_COLOR = Color.Orange;
        private readonly Color MINIMUM_COLOR = Color.Red;
        private readonly Color AXIS_COLOR = Color.DarkGray;
        private readonly Color INTERVAL_COLOR = Color.Green;

        public Newton()
        {
            InitializeComponent();
            SetupChart();
            SetupEventHandlers();
        }

        private void SetupChart()
        {
            chartFunc.Series.Clear();

            // Основной график функции
            Series functionSeries = new Series("Функция")
            {
                ChartType = SeriesChartType.Line,
                Color = FUNCTION_COLOR,
                BorderWidth = 2
            };
            chartFunc.Series.Add(functionSeries);

            // Касательные к функции (для визуализации)
            Series tangentSeries = new Series("Касательные")
            {
                ChartType = SeriesChartType.Line,
                Color = TANGENT_COLOR,
                BorderWidth = 1,
                BorderDashStyle = ChartDashStyle.Dash
            };
            chartFunc.Series.Add(tangentSeries);

            // Точка минимума
            Series minimumSeries = new Series("Минимум")
            {
                ChartType = SeriesChartType.Point,
                Color = MINIMUM_COLOR,
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 10
            };
            chartFunc.Series.Add(minimumSeries);

            // Точки итераций
            Series iterationSeries = new Series("Итерации")
            {
                ChartType = SeriesChartType.Point,
                Color = Color.Purple,
                MarkerStyle = MarkerStyle.Triangle,
                MarkerSize = 8
            };
            chartFunc.Series.Add(iterationSeries);

            // Границы интервала
            Series intervalSeries = new Series("Интервал")
            {
                ChartType = SeriesChartType.Line,
                Color = INTERVAL_COLOR,
                BorderWidth = 2,
                BorderDashStyle = ChartDashStyle.Dash
            };
            chartFunc.Series.Add(intervalSeries);

            // Ось X
            Series xAxisSeries = new Series("Ось X (y=0)")
            {
                ChartType = SeriesChartType.Line,
                Color = AXIS_COLOR,
                BorderWidth = 1,
                BorderDashStyle = ChartDashStyle.Dash
            };
            chartFunc.Series.Add(xAxisSeries);

            // Ось Y
            Series yAxisSeries = new Series("Ось Y (x=0)")
            {
                ChartType = SeriesChartType.Line,
                Color = AXIS_COLOR,
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
            textBoxE.KeyPress += TextBoxEpsilon_KeyPress;
            textBoxF.KeyPress += TextBoxFunction_KeyPress;

            // Обработчики кнопок
            buttonStart.Click += ButtonStartNewton_Click;
            buttonClear.Click += ButtonClear_Click;
            buttonStop.Click += ButtonStop_Click;
        }

        #region Обработчики ввода

        private void TextBoxNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            System.Windows.Forms.TextBox textBox = (System.Windows.Forms.TextBox)sender;
            string currentText = textBox.Text;
            int selectionStart = textBox.SelectionStart;

            // Разрешить управляющие символы
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

        #endregion

        #region Вычисление функции и производных

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

        /// <summary>
        /// Вычисляет первую производную функции численным методом
        /// </summary>
        private double EvaluateFirstDerivative(string function, double x, double h = 1e-5)
        {
            // Центральная разностная схема (более точная)
            try
            {
                double f_plus = EvaluateFunction(function, x + h);
                double f_minus = EvaluateFunction(function, x - h);
                return (f_plus - f_minus) / (2 * h);
            }
            catch
            {
                // Правосторонняя разность как запасной вариант
                try
                {
                    double f_x = EvaluateFunction(function, x);
                    double f_x_plus_h = EvaluateFunction(function, x + h);
                    return (f_x_plus_h - f_x) / h;
                }
                catch
                {
                    throw new Exception($"Не удалось вычислить первую производную в точке x={x}");
                }
            }
        }

        /// <summary>
        /// Вычисляет вторую производную функции численным методом
        /// </summary>
        private double EvaluateSecondDerivative(string function, double x, double h = 1e-5)
        {
            try
            {
                double f_plus = EvaluateFunction(function, x + h);
                double f_x = EvaluateFunction(function, x);
                double f_minus = EvaluateFunction(function, x - h);
                return (f_plus - 2 * f_x + f_minus) / (h * h);
            }
            catch
            {
                throw new Exception($"Не удалось вычислить вторую производную в точке x={x}");
            }
        }

        /// <summary>
        /// Проверяет, имеет ли функция минимум на интервале
        /// </summary>
        private bool HasMinimumOnInterval(string function, double a, double b)
        {
            string funcLower = function.ToLower().Replace(" ", "");

            // Проверка по списку функций без экстремумов
            foreach (string pattern in NO_EXTREMUM_FUNCTIONS)
            {
                if (funcLower.Contains(pattern))
                {
                    return false;
                }
            }

            // Проверка дробей вида 1/x
            if (Regex.IsMatch(funcLower, @"1\s*/\s*x") || Regex.IsMatch(funcLower, @"x\^-\d"))
            {
                return false;
            }

            // Проверка линейных функций (нет экстремумов)
            if (IsLinearFunction(funcLower))
            {
                return false;
            }

            // Проверка экспоненты e^x (возрастающая функция)
            if (funcLower == "e^x" || funcLower == "exp(x)")
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Проверяет, является ли функция линейной
        /// </summary>
        private bool IsLinearFunction(string funcLower)
        {
            // Убираем пробелы
            funcLower = funcLower.Replace(" ", "");

            // Если есть тригонометрия, логарифмы, корни - не линейная
            if (funcLower.Contains("sin") || funcLower.Contains("cos") ||
                funcLower.Contains("tan") || funcLower.Contains("log") ||
                funcLower.Contains("ln") || funcLower.Contains("sqrt") ||
                funcLower.Contains("exp"))
            {
                return false;
            }

            // Если есть x^2, x^3 и т.д. - не линейная
            if (Regex.IsMatch(funcLower, @"x\^\d"))
            {
                return false;
            }

            // Проверяем наличие x без степени (кроме x^1)
            if (funcLower.Contains("x"))
            {
                // Проверяем, нет ли x в знаменателе
                if (funcLower.Contains("/x") || funcLower.Contains("1/x"))
                {
                    return false;
                }

                // Упрощенная проверка: если есть x и нет x^2, x^3 и т.д., считаем линейной
                return !funcLower.Contains("x^") || funcLower.Contains("x^1");
            }

            return false;
        }

        #endregion

        #region Метод Ньютона для оптимизации (поиск минимума)

        /// <summary>
        /// Метод Ньютона для поиска минимума функции
        /// Алгоритм:
        /// 1. Находим корень первой производной: f'(x) = 0
        /// 2. Проверяем знак второй производной: f''(x) > 0 для минимума
        /// 3. Формула итерации: x_{n+1} = x_n - f'(x_n) / f''(x_n)
        /// </summary>
        private async Task NewtonOptimizationAsync(string function, double a, double b,
            double startX, double epsilon, CancellationToken cancellationToken)
        {
            double currentX = startX;
            int iteration = 0;
            const int maxIterations = 100;

            // Очищаем серии
            chartFunc.Series["Касательные"].Points.Clear();
            chartFunc.Series["Итерации"].Points.Clear();
            chartFunc.Series["Минимум"].Points.Clear();

            // Добавляем начальную точку
            try
            {
                double currentY = EvaluateFunction(function, currentX);
                chartFunc.Series["Итерации"].Points.AddXY(currentX, currentY);
                chartFunc.Invalidate();
                await Task.Delay(500, cancellationToken);
            }
            catch
            {
                SetNoMinimum();
                return;
            }

            // Основной цикл метода Ньютона для оптимизации
            while (iteration < maxIterations && !cancellationToken.IsCancellationRequested)
            {
                iteration++;

                try
                {
                    // Вычисляем первую и вторую производные
                    double f_prime = EvaluateFirstDerivative(function, currentX);
                    double f_double_prime = EvaluateSecondDerivative(function, currentX);

                    // Проверяем, что вторая производная не равна нулю
                    if (Math.Abs(f_double_prime) < ZERO_TOLERANCE)
                    {
                        UpdateStatus($"Вторая производная близка к нулю на итерации {iteration}. Возможно точка перегиба.");
                        CheckForMinimumNearby(function, currentX, a, b);
                        break;
                    }

                    // Формула Ньютона для оптимизации: x_new = x - f'(x)/f''(x)
                    double nextX = currentX - f_prime / f_double_prime;

                    // Проверяем, не вышли ли за границы интервала
                    if (nextX < a || nextX > b)
                    {
                        UpdateStatus($"Решение вышло за границы интервала [{a}, {b}]");
                        // Проверяем границы на минимум
                        CheckBoundariesForMinimum(function, a, b);
                        break;
                    }

                    // Вычисляем функцию в новой точке
                    double nextY = EvaluateFunction(function, nextX);

                    // Отрисовываем касательную к функции в текущей точке
                    DrawFunctionTangent(function, currentX);

                    // Добавляем точку итерации
                    chartFunc.Series["Итерации"].Points.AddXY(nextX, nextY);
                    chartFunc.Invalidate();

                    // Проверяем условия остановки
                    double deltaX = Math.Abs(nextX - currentX);
                    double deltaPrime = Math.Abs(f_prime); // f'(x) должно стремиться к 0

                    UpdateStatus($"Итерация {iteration}: x = {nextX:F6}, f(x) = {nextY:F6}, f'(x) = {f_prime:F6}, Δx = {deltaX:F6}");

                    // Условия остановки: малое изменение x ИЛИ первая производная близка к 0
                    if (deltaX < epsilon || deltaPrime < epsilon)
                    {
                        // Проверяем, что это действительно минимум (f''(x) > 0)
                        double final_f_double_prime = EvaluateSecondDerivative(function, nextX);

                        if (final_f_double_prime > 0)
                        {
                            UpdateResult(nextX, nextY, iteration, "Минимум найден");
                        }
                        else if (final_f_double_prime < 0)
                        {
                            UpdateStatus($"Найден максимум в точке x={nextX:F6}. Ищем минимум...");
                            // Для периодических функций (sin, cos) ищем соседний минимум
                            FindMinimumForPeriodicFunction(function, nextX, a, b, iteration);
                        }
                        else
                        {
                            UpdateStatus($"Найден возможный перегиб в точке x={nextX:F6}");
                            CheckForMinimumNearby(function, nextX, a, b);
                        }
                        break;
                    }

                    // Обновляем текущую точку
                    currentX = nextX;

                    // Задержка для визуализации
                    await Task.Delay(800, cancellationToken);
                }
                catch (Exception ex)
                {
                    UpdateStatus($"Ошибка на итерации {iteration}: {ex.Message}");
                    SetNoMinimum();
                    break;
                }
            }

            if (iteration >= maxIterations)
            {
                UpdateStatus($"Достигнуто максимальное количество итераций ({maxIterations})");
                SetNoMinimum();
            }
        }

        /// <summary>
        /// Для периодических функций ищет минимум после нахождения максимума
        /// </summary>
        private void FindMinimumForPeriodicFunction(string function, double maxX, double a, double b, int iteration)
        {
            try
            {
                string funcLower = function.ToLower().Replace(" ", "");

                // Для sin(x): минимум в x = maxX - π
                // Для cos(x): минимум в x = maxX - π
                double period = Math.PI;

                // Проверяем кандидатов на минимум
                List<double> candidates = new List<double>();

                // Добавляем точки вокруг максимума
                candidates.Add(maxX - period);
                candidates.Add(maxX + period);
                candidates.Add(maxX - period / 2);
                candidates.Add(maxX + period / 2);

                double bestX = maxX;
                double bestY = EvaluateFunction(function, maxX);
                bool foundMinimum = false;

                foreach (double candidate in candidates)
                {
                    if (candidate >= a && candidate <= b)
                    {
                        try
                        {
                            double y = EvaluateFunction(function, candidate);
                            double f_double_prime = EvaluateSecondDerivative(function, candidate);

                            if (f_double_prime > 0 && y < bestY)
                            {
                                bestX = candidate;
                                bestY = y;
                                foundMinimum = true;
                            }
                        }
                        catch { }
                    }
                }

                if (foundMinimum)
                {
                    UpdateResult(bestX, bestY, iteration, "Минимум найден (после коррекции)");
                }
                else
                {
                    // Если не нашли минимум, проверяем границы
                    CheckBoundariesForMinimum(function, a, b);
                }
            }
            catch
            {
                SetNoMinimum();
            }
        }

        /// <summary>
        /// Проверяет точки рядом на наличие минимума
        /// </summary>
        private void CheckForMinimumNearby(string function, double x0, double a, double b)
        {
            try
            {
                // Проверяем несколько точек вокруг x0
                double step = (b - a) / 20;
                List<double> testPoints = new List<double>();

                for (int i = -5; i <= 5; i++)
                {
                    double point = x0 + i * step;
                    if (point >= a && point <= b)
                    {
                        testPoints.Add(point);
                    }
                }

                // Добавляем границы
                testPoints.Add(a);
                testPoints.Add(b);
                testPoints.Add((a + b) / 2);

                double bestX = x0;
                double bestY = EvaluateFunction(function, x0);
                bool foundBetter = false;

                foreach (double point in testPoints)
                {
                    try
                    {
                        double y = EvaluateFunction(function, point);
                        double f_double_prime = EvaluateSecondDerivative(function, point);

                        if (f_double_prime > 0 && y < bestY)
                        {
                            bestX = point;
                            bestY = y;
                            foundBetter = true;
                        }
                    }
                    catch { }
                }

                if (foundBetter)
                {
                    UpdateResult(bestX, bestY, 0, "Найден минимум в близкой точке");
                }
                else
                {
                    SetNoMinimum();
                }
            }
            catch
            {
                SetNoMinimum();
            }
        }

        /// <summary>
        /// Рисует касательную к функции в точке x0
        /// </summary>
        private void DrawFunctionTangent(string function, double x0)
        {
            try
            {
                double fx0 = EvaluateFunction(function, x0);
                double dfx0 = EvaluateFirstDerivative(function, x0);

                // Получаем текущие границы графика
                double xMin = chartFunc.ChartAreas[0].AxisX.Minimum;
                double xMax = chartFunc.ChartAreas[0].AxisX.Maximum;

                // Уравнение касательной: y = f'(x0)*(x - x0) + f(x0)
                double tangentX1 = xMin;
                double tangentX2 = xMax;
                double tangentY1 = dfx0 * (tangentX1 - x0) + fx0;
                double tangentY2 = dfx0 * (tangentX2 - x0) + fx0;

                // Добавляем касательную
                chartFunc.Series["Касательные"].Points.AddXY(tangentX1, tangentY1);
                chartFunc.Series["Касательные"].Points.AddXY(tangentX2, tangentY2);

                // Разрыв между касательными
                chartFunc.Series["Касательные"].Points.AddXY(double.NaN, double.NaN);
            }
            catch
            {
                // Пропускаем ошибки отрисовки касательной
            }
        }

        /// <summary>
        /// Проверяет границы интервала на наличие минимума
        /// </summary>
        private void CheckBoundariesForMinimum(string function, double a, double b)
        {
            try
            {
                double fa = EvaluateFunction(function, a);
                double fb = EvaluateFunction(function, b);

                // Проверяем производные на границах
                double f_prime_a = EvaluateFirstDerivative(function, a);
                double f_prime_b = EvaluateFirstDerivative(function, b);

                // Если на левой границе производная положительная - функция возрастает
                // Если на правой границе производная отрицательная - функция убывает
                // Значит минимум внутри
                if (f_prime_a > 0 && f_prime_b < 0)
                {
                    UpdateStatus("Минимум находится внутри интервала, но метод не сошелся");
                    SetNoMinimum();
                    return;
                }

                // Проверяем, какая граница дает меньшее значение
                if (fa < fb)
                {
                    // Проверяем, не является ли это локальным минимумом
                    double f_double_prime_a = EvaluateSecondDerivative(function, a);
                    if (f_double_prime_a > 0)
                    {
                        UpdateResult(a, fa, 0, "Минимум на левой границе");
                    }
                    else
                    {
                        UpdateResult(a, fa, 0, "Наименьшее значение на левой границе");
                    }
                }
                else
                {
                    double f_double_prime_b = EvaluateSecondDerivative(function, b);
                    if (f_double_prime_b > 0)
                    {
                        UpdateResult(b, fb, 0, "Минимум на правой границе");
                    }
                    else
                    {
                        UpdateResult(b, fb, 0, "Наименьшее значение на правой границе");
                    }
                }
            }
            catch
            {
                SetNoMinimum();
            }
        }

        private void SetNoMinimum()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(SetNoMinimum));
                return;
            }

            textBoxXMin.Text = "нет";
            textBoxYMin.Text = "нет";
        }

        private void UpdateStatus(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateStatus(message)));
                return;
            }
            // Можно добавить вывод в статусную строку, если есть
        }

        private void UpdateResult(double x, double y, int iterations, string message = "")
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateResult(x, y, iterations, message)));
                return;
            }

            // Записываем результат
            int precision = int.TryParse(textBoxE.Text, out int p) ? p : 6;
            textBoxXMin.Text = x.ToString($"F{precision}");
            textBoxYMin.Text = y.ToString($"F{precision}");

            // Отображаем найденный минимум на графике
            chartFunc.Series["Минимум"].Points.Clear();
            chartFunc.Series["Минимум"].Points.AddXY(x, y);

            // Добавляем подпись к точке минимума
            chartFunc.Series["Минимум"].Points.Last().Label = $"Минимум\nx={x:F3}\ny={y:F3}";
            chartFunc.Invalidate();

            UpdateStatus($"{message} за {iterations} итераций");
        }

        #endregion

        #region Обработчики кнопок

        private async void ButtonStartNewton_Click(object sender, EventArgs e)
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

                // Проверяем, имеет ли функция минимум на интервале
                if (!HasMinimumOnInterval(textBoxF.Text, a, b))
                {
                    textBoxXMin.Text = "нет";
                    textBoxYMin.Text = "нет";
                    MessageBox.Show("Данная функция не имеет минимума на заданном интервале",
                        "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Все равно строим график
                    PlotFunction(textBoxF.Text, a, b);
                    return;
                }

                // Останавливаем предыдущий поиск, если он запущен
                if (_isSearchRunning)
                {
                    _cancellationTokenSource?.Cancel();
                }

                // Строим график функции
                PlotFunction(textBoxF.Text, a, b);

                // Запускаем метод Ньютона для оптимизации
                _cancellationTokenSource = new CancellationTokenSource();
                _isSearchRunning = true;
                buttonStart.Enabled = false;
                buttonStop.Enabled = true;

                try
                {
                    double epsilon = Math.Pow(10, -precision);
                    await NewtonOptimizationAsync(textBoxF.Text, a, b, startX, epsilon,
                        _cancellationTokenSource.Token);
                }
                catch (OperationCanceledException)
                {
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

        #endregion

        #region Визуализация

        private void PlotFunction(string function, double a, double b)
        {
            // Очищаем все серии
            chartFunc.Series["Функция"].Points.Clear();
            chartFunc.Series["Касательные"].Points.Clear();
            chartFunc.Series["Минимум"].Points.Clear();
            chartFunc.Series["Итерации"].Points.Clear();
            chartFunc.Series["Интервал"].Points.Clear();
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

            // Настройка масштаба по оси Y
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
            }

            // Ограничиваем масштаб для функций с большими значениями
            if (chartFunc.ChartAreas[0].AxisY.Minimum < -MAX_Y_VALUE)
                chartFunc.ChartAreas[0].AxisY.Minimum = -MAX_Y_VALUE;
            if (chartFunc.ChartAreas[0].AxisY.Maximum > MAX_Y_VALUE)
                chartFunc.ChartAreas[0].AxisY.Maximum = MAX_Y_VALUE;

            // Отрисовка вертикальных линий интервала
            double lineY1 = chartFunc.ChartAreas[0].AxisY.Minimum;
            double lineY2 = chartFunc.ChartAreas[0].AxisY.Maximum;

            chartFunc.Series["Интервал"].Points.AddXY(a, lineY1);
            chartFunc.Series["Интервал"].Points.AddXY(a, lineY2);
            chartFunc.Series["Интервал"].Points.AddXY(double.NaN, double.NaN);

            chartFunc.Series["Интервал"].Points.AddXY(b, lineY1);
            chartFunc.Series["Интервал"].Points.AddXY(b, lineY2);

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
            // Останавливаем поиск перед переходом
            if (_isSearchRunning)
            {
                _cancellationTokenSource?.Cancel();
            }

            Main main = new Main();
            main.Show();
            Close();
        }
    }
}