using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using NCalc;
using ProgramLab;

namespace ProgramLab
{
    public partial class Dichotomy : Form
    {
        private List<double> foundRoots = new List<double>();

        public Dichotomy()
        {
            InitializeComponent();
            buttonStart.Click += ButtonStart_Click;
            buttonChart.Click += ButtonChart_Click;
            buttonClear.Click += ButtonClear_Click;
            SetupInputValidation();
        }

        private void SetupInputValidation()
        {
            var numberTextBoxes = new[] { textBoxA, textBoxB, textBoxE };
            foreach (var textBox in numberTextBoxes)
            {
                textBox.KeyPress += (sender, e) =>
                {
                    var tb = sender as TextBox;
                    char decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
                    char altSeparator = (decimalSeparator == ',') ? '.' : ',';

                    bool isDigit = char.IsDigit(e.KeyChar);
                    bool isBackspace = e.KeyChar == '\b';
                    bool isMinus = e.KeyChar == '-' && (tb.SelectionStart == 0 || tb.Text.Length == 0);
                    bool isSeparator = (e.KeyChar == decimalSeparator || e.KeyChar == altSeparator) &&
                                       !tb.Text.Contains(decimalSeparator) &&
                                       !tb.Text.Contains(altSeparator);

                    if (!isDigit && !isBackspace && !isMinus && !isSeparator)
                        e.Handled = true;
                };
            }

            textBoxE.KeyPress += (sender, e) =>
            {
                if (!char.IsDigit(e.KeyChar) && e.KeyChar != '\b')
                    e.Handled = true;
            };
        }

        private void ButtonClear_Click(object sender, EventArgs e)
        {
            // Очищаем все текстовые поля
            textBoxF.Clear();
            textBoxA.Clear();
            textBoxB.Clear();
            textBoxE.Clear();
            textBoxX.Clear();
            textBoxY.Clear();

            // Очищаем график
            ClearChartData();
            foundRoots.Clear();
        }

        private double EvaluateSimpleFunction(string expression, double x)
        {
            try
            {
                expression = expression.Trim().ToLower();

                // Убираем лишние пробелы
                expression = Regex.Replace(expression, @"\s+", "");

                // Проверяем, является ли выражение параболой вида ax^2 + bx + c
                if (IsQuadraticFunction(expression, out double a, out double b, out double c))
                {
                    return a * x * x + b * x + c;
                }

                // Обработка функций sin(x)
                if (expression.StartsWith("sin(") && expression.EndsWith(")"))
                {
                    return EvaluateTrigonometricArgument(expression, x, Math.Sin);
                }
                // Обработка функций cos(x)
                else if (expression.StartsWith("cos(") && expression.EndsWith(")"))
                {
                    return EvaluateTrigonometricArgument(expression, x, Math.Cos);
                }
                // Обработка функций tan(x)
                else if (expression.StartsWith("tan(") && expression.EndsWith(")"))
                {
                    return EvaluateTrigonometricArgument(expression, x, Math.Tan);
                }
                // Обработка функций ctg(x) - котангенс
                else if (expression.StartsWith("ctg(") && expression.EndsWith(")"))
                {
                    return EvaluateCtgArgument(expression, x);
                }
                // Обработка функций ln(x) - натуральный логарифм
                else if (expression.StartsWith("ln(") && expression.EndsWith(")"))
                {
                    return EvaluateLogArgument(expression, x, Math.Log);
                }
                // Обработка функций exp(x) - экспонента
                else if (expression.StartsWith("exp(") && expression.EndsWith(")"))
                {
                    return EvaluateExpArgument(expression, x);
                }

                // Если это не простая функция, используем NCalc
                return EvaluateFunctionWithNCalc(expression, x);
            }
            catch (Exception ex)
            {
                // В случае ошибки возвращаем NaN
                return double.NaN;
            }
        }

        private bool IsQuadraticFunction(string expression, out double a, out double b, out double c)
        {
            a = 0; b = 0; c = 0;

            // Упрощаем выражение
            expression = expression.Replace(" ", "");

            // Паттерны для параболы
            // 1. ax^2 + bx + c
            // 2. x^2 + bx + c
            // 3. ax^2 + c
            // 4. x^2 + c
            // 5. ax^2
            // 6. x^2

            // Проверяем наличие x^2
            if (!expression.Contains("x^2") && !expression.Contains("x*x") && !expression.Contains("pow(x,2)"))
                return false;

            try
            {
                // Заменяем x^2 на x*x для упрощения парсинга
                expression = expression.Replace("x^2", "x*x");

                // Если выражение содержит сложные операции кроме +, -, *, /, то это не простая парабола
                if (expression.Contains("sin(") || expression.Contains("cos(") ||
                    expression.Contains("tan(") || expression.Contains("ln(") ||
                    expression.Contains("exp(") || expression.Contains("sqrt(") ||
                    expression.Contains("/x") || expression.Contains("1/"))
                    return false;

                // Разбираем выражение на части
                string pattern = @"([+-]?\d*\.?\d*)\*?x\*x|([+-]?\d*\.?\d*)\*?x|([+-]?\d+\.?\d*)";
                var matches = Regex.Matches(expression, pattern);

                foreach (Match match in matches)
                {
                    if (match.Groups[1].Success) // Коэффициент для x^2
                    {
                        string coeff = match.Groups[1].Value;
                        if (string.IsNullOrEmpty(coeff) || coeff == "+")
                            a += 1;
                        else if (coeff == "-")
                            a -= 1;
                        else
                            a += double.Parse(coeff, CultureInfo.InvariantCulture);
                    }
                    else if (match.Groups[2].Success) // Коэффициент для x
                    {
                        string coeff = match.Groups[2].Value;
                        if (string.IsNullOrEmpty(coeff) || coeff == "+")
                            b += 1;
                        else if (coeff == "-")
                            b -= 1;
                        else
                            b += double.Parse(coeff, CultureInfo.InvariantCulture);
                    }
                    else if (match.Groups[3].Success) // Свободный член
                    {
                        c += double.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private double EvaluateTrigonometricArgument(string expression, double x, Func<double, double> trigFunction)
        {
            // Извлекаем аргумент из скобок
            string argStr = expression.Substring(4, expression.Length - 5);

            // Если аргумент просто x
            if (argStr == "x")
            {
                return trigFunction(x);
            }
            // Если аргумент содержит x с коэффициентом (например, sin(2x), sin(3*x))
            else if (Regex.IsMatch(argStr, @"^(\d+\.?\d*)?\*?x$"))
            {
                double coefficient = 1.0;
                string coeffStr = argStr.Replace("*x", "").Replace("x", "");

                if (!string.IsNullOrEmpty(coeffStr))
                {
                    coefficient = double.Parse(coeffStr, CultureInfo.InvariantCulture);
                }

                return trigFunction(coefficient * x);
            }
            // Если аргумент сложный, используем NCalc
            else
            {
                Expression e = new Expression(argStr);
                e.Parameters["x"] = x;
                e.Parameters["pi"] = Math.PI;
                e.Parameters["e"] = Math.E;

                var result = e.Evaluate();
                return trigFunction(Convert.ToDouble(result));
            }
        }

        private double EvaluateCtgArgument(string expression, double x)
        {
            // ctg(x) = cos(x)/sin(x) = 1/tan(x)
            // Извлекаем аргумент из скобок
            string argStr = expression.Substring(4, expression.Length - 5);

            double argumentValue;

            // Если аргумент просто x
            if (argStr == "x")
            {
                argumentValue = x;
            }
            // Если аргумент содержит x с коэффициентом
            else if (Regex.IsMatch(argStr, @"^(\d+\.?\d*)?\*?x$"))
            {
                double coefficient = 1.0;
                string coeffStr = argStr.Replace("*x", "").Replace("x", "");

                if (!string.IsNullOrEmpty(coeffStr))
                {
                    coefficient = double.Parse(coeffStr, CultureInfo.InvariantCulture);
                }

                argumentValue = coefficient * x;
            }
            // Если аргумент сложный, используем NCalc
            else
            {
                Expression e = new Expression(argStr);
                e.Parameters["x"] = x;
                e.Parameters["pi"] = Math.PI;
                e.Parameters["e"] = Math.E;

                var result = e.Evaluate();
                argumentValue = Convert.ToDouble(result);
            }

            // Вычисляем ctg(x) = cos(x)/sin(x)
            double sinValue = Math.Sin(argumentValue);

            // Проверяем, чтобы sin(x) не был равен 0 (деление на ноль)
            if (Math.Abs(sinValue) < 1e-15)
            {
                return double.NaN;
            }

            return Math.Cos(argumentValue) / sinValue;
        }

        private double EvaluateLogArgument(string expression, double x, Func<double, double> logFunction)
        {
            // ln(x) - натуральный логарифм
            // Извлекаем аргумент из скобок
            string argStr = expression.Substring(3, expression.Length - 4);

            double argumentValue;

            // Если аргумент просто x
            if (argStr == "x")
            {
                argumentValue = x;
            }
            // Если аргумент содержит x с коэффициентом
            else if (Regex.IsMatch(argStr, @"^(\d+\.?\d*)?\*?x$"))
            {
                double coefficient = 1.0;
                string coeffStr = argStr.Replace("*x", "").Replace("x", "");

                if (!string.IsNullOrEmpty(coeffStr))
                {
                    coefficient = double.Parse(coeffStr, CultureInfo.InvariantCulture);
                }

                argumentValue = coefficient * x;
            }
            // Если аргумент сложный, используем NCalc
            else
            {
                Expression e = new Expression(argStr);
                e.Parameters["x"] = x;
                e.Parameters["pi"] = Math.PI;
                e.Parameters["e"] = Math.E;

                var result = e.Evaluate();
                argumentValue = Convert.ToDouble(result);
            }

            // Проверяем, чтобы аргумент логарифма был положительным
            if (argumentValue <= 0)
            {
                return double.NaN;
            }

            return logFunction(argumentValue);
        }

        private double EvaluateExpArgument(string expression, double x)
        {
            // exp(x) - экспонента
            // Извлекаем аргумент из скобок
            string argStr = expression.Substring(4, expression.Length - 5);

            double argumentValue;

            // Если аргумент просто x
            if (argStr == "x")
            {
                argumentValue = x;
            }
            // Если аргумент содержит x с коэффициентом
            else if (Regex.IsMatch(argStr, @"^(\d+\.?\d*)?\*?x$"))
            {
                double coefficient = 1.0;
                string coeffStr = argStr.Replace("*x", "").Replace("x", "");

                if (!string.IsNullOrEmpty(coeffStr))
                {
                    coefficient = double.Parse(coeffStr, CultureInfo.InvariantCulture);
                }

                argumentValue = coefficient * x;
            }
            // Если аргумент сложный, используем NCalc
            else
            {
                Expression e = new Expression(argStr);
                e.Parameters["x"] = x;
                e.Parameters["pi"] = Math.PI;
                e.Parameters["e"] = Math.E;

                var result = e.Evaluate();
                argumentValue = Convert.ToDouble(result);
            }

            return Math.Exp(argumentValue);
        }

        private double EvaluateFunctionWithNCalc(string expression, double x)
        {
            try
            {
                expression = expression.Replace(',', '.');

                // Преобразуем оператор ^ в функцию Pow
                expression = ConvertPowerOperator(expression);

                // Заменяем ctg на 1/tan для NCalc
                expression = expression.Replace("ctg(", "1/tan(");
                expression = expression.Replace("Ctg(", "1/tan(");
                expression = expression.Replace("CTG(", "1/tan(");

                Expression e = new Expression(expression);
                e.Parameters["x"] = x;
                e.Parameters["pi"] = Math.PI;
                e.Parameters["e"] = Math.E;

                e.Parameters["sin"] = new Func<double, double>(Math.Sin);
                e.Parameters["cos"] = new Func<double, double>(Math.Cos);
                e.Parameters["tan"] = new Func<double, double>(Math.Tan);
                e.Parameters["ctg"] = new Func<double, double>((arg) =>
                {
                    double tanVal = Math.Tan(arg);
                    return Math.Abs(tanVal) < 1e-15 ? double.NaN : 1.0 / tanVal;
                });
                e.Parameters["log"] = new Func<double, double>(Math.Log10);
                e.Parameters["ln"] = new Func<double, double>(Math.Log);
                e.Parameters["exp"] = new Func<double, double>(Math.Exp);
                e.Parameters["sqrt"] = new Func<double, double>(Math.Sqrt);
                e.Parameters["abs"] = new Func<double, double>(Math.Abs);
                e.Parameters["Pow"] = new Func<double, double, double>(Math.Pow);

                var result = e.Evaluate();
                return Convert.ToDouble(result);
            }
            catch (Exception ex)
            {
                // Возвращаем NaN для неопределенных точек
                return double.NaN;
            }
        }

        private double EvaluateFunction(string expression, double x)
        {
            // Сначала проверяем, является ли выражение простой функцией
            string lowerExpr = expression.ToLower();

            // Проверяем простые функции, включая параболы
            if (lowerExpr.Contains("sin(") || lowerExpr.Contains("cos(") ||
                lowerExpr.Contains("tan(") || lowerExpr.Contains("tg(") ||
                lowerExpr.Contains("ctg(") || lowerExpr.Contains("ln(") ||
                lowerExpr.Contains("exp(") || lowerExpr.Contains("x^2") ||
                lowerExpr.Contains("x*x") || IsQuadraticExpression(lowerExpr))
            {
                // Используем оптимизированный метод для простых функций
                return EvaluateSimpleFunction(expression, x);
            }

            // Для всех других функций используем NCalc
            return EvaluateFunctionWithNCalc(expression, x);
        }

        private bool IsQuadraticExpression(string expression)
        {
            // Проверяем, является ли выражение квадратичной функцией
            expression = expression.Replace(" ", "");
            return expression.Contains("x^2") || expression.Contains("x*x") || expression.Contains("pow(x,2)");
        }

        private bool IsHyperbola(string expression)
        {
            // Проверяем, содержит ли выражение гиперболу
            expression = expression.ToLower();
            return expression.Contains("1/x") || expression.Contains("1/(x)") ||
                   expression.Contains("x^(-1)") || expression.Contains("x**-1") ||
                   Regex.IsMatch(expression, @"x\s*\/\s*[^+\-*/()]+") ||
                   Regex.IsMatch(expression, @"[^+\-*/()]+\s*\/\s*x");
        }

        private bool IsTrigonometricFunction(string expression)
        {
            // Проверяем, является ли функция тригонометрической
            expression = expression.ToLower();
            return expression.Contains("sin(") || expression.Contains("cos(") ||
                   expression.Contains("tan(") || expression.Contains("tg(") ||
                   expression.Contains("ctg(");
        }

        private bool IsLogarithmicFunction(string expression)
        {
            // Проверяем, является ли функция логарифмической
            expression = expression.ToLower();
            return expression.Contains("ln(") || expression.Contains("log(");
        }

        private bool IsExponentialFunction(string expression)
        {
            // Проверяем, является ли функция экспоненциальной
            expression = expression.ToLower();
            return expression.Contains("exp(") || expression.Contains("e^");
        }

        private bool IsQuadraticFunction(string expression)
        {
            // Проверяем, является ли функция квадратичной
            expression = expression.ToLower();
            return expression.Contains("x^2") || expression.Contains("x*x") || expression.Contains("pow(x,2)");
        }

        private bool IsContinuousFunction(string expression)
        {
            // Определяем, является ли функция непрерывной на всей области определения
            expression = expression.ToLower();

            // Непрерывные функции: sin, cos, exp, полиномы, включая параболы
            // Разрывные: tan, ctg, ln (на отрицательных и нуле), 1/x
            if (expression.Contains("tan(") || expression.Contains("tg(") ||
                expression.Contains("ctg(") || expression.Contains("1/x") ||
                expression.Contains("ln(") || expression.Contains("log("))
            {
                return false;
            }

            return true;
        }

        private string ConvertPowerOperator(string expression)
        {
            // Преобразуем оператор ^ в Pow для NCalc
            return Regex.Replace(expression, @"([a-zA-Z0-9\.\(\)]+)\s*\^\s*([a-zA-Z0-9\.\(\)]+)", "Pow($1, $2)");
        }

        private List<(double x, double y)> FindAllRoots(string function, double a, double b, int precision)
        {
            List<(double x, double y)> roots = new List<(double, double)>();

            if (precision < 0 || precision > 15)
                return roots;

            double epsilon = Math.Pow(10, -precision);

            // Проверяем, является ли функция гиперболой
            if (IsHyperbola(function))
            {
                // Для гиперболы нет корней (кроме x=0, что невозможно)
                return roots;
            }

            // Проверяем, является ли функция ln(x) - у нее только один корень в x=1
            if (function.ToLower().Contains("ln(x)"))
            {
                // Проверяем, попадает ли x=1 в интервал [a, b]
                if (a <= 1 && 1 <= b)
                {
                    double y = EvaluateFunction(function, 1);
                    if (Math.Abs(y) < epsilon)
                    {
                        roots.Add((1, y));
                    }
                }
            }

            // Проверяем, является ли функция квадратичной
            bool isQuadratic = IsQuadraticFunction(function);

            // Для квадратичных функций используем аналитическое решение
            if (isQuadratic && IsQuadraticFunction(function, out double A, out double B, out double C))
            {
                double discriminant = B * B - 4 * A * C;

                if (discriminant < 0)
                {
                    // Нет действительных корней
                    return roots;
                }
                else if (Math.Abs(discriminant) < epsilon)
                {
                    // Один корень (кратности 2)
                    double x = -B / (2 * A);
                    if (x >= a && x <= b)
                    {
                        double y = EvaluateFunction(function, x);
                        if (Math.Abs(y) < epsilon * 10)
                        {
                            roots.Add((x, y));
                        }
                    }
                }
                else
                {
                    // Два корня
                    double sqrtD = Math.Sqrt(discriminant);
                    double x1 = (-B - sqrtD) / (2 * A);
                    double x2 = (-B + sqrtD) / (2 * A);

                    // Добавляем корни, попадающие в интервал
                    if (x1 >= a && x1 <= b)
                    {
                        double y1 = EvaluateFunction(function, x1);
                        if (Math.Abs(y1) < epsilon * 10)
                        {
                            roots.Add((x1, y1));
                        }
                    }

                    if (x2 >= a && x2 <= b)
                    {
                        double y2 = EvaluateFunction(function, x2);
                        if (Math.Abs(y2) < epsilon * 10)
                        {
                            roots.Add((x2, y2));
                        }
                    }
                }

                return roots;
            }

            // Определяем количество шагов в зависимости от типа функции
            int steps;
            if (IsTrigonometricFunction(function))
            {
                // Для тригонометрических функций используем больше шагов
                steps = Math.Min(5000, (int)((b - a) * 100));
                steps = Math.Max(steps, 100);
            }
            else if (IsExponentialFunction(function) || IsLogarithmicFunction(function))
            {
                // Для экспоненциальных и логарифмических функций используем среднее количество шагов
                steps = Math.Min(3000, (int)((b - a) * 75));
                steps = Math.Max(steps, 100);
            }
            else
            {
                steps = Math.Min(2000, (int)((b - a) * 50));
                steps = Math.Max(steps, 100);
            }

            double step = (b - a) / steps;

            double prevX = a;
            double prevY = EvaluateFunction(function, a);

            for (int i = 1; i <= steps; i++)
            {
                double currentX = a + i * step;
                double currentY = EvaluateFunction(function, currentX);

                // Проверяем наличие корня на отрезке [prevX, currentX]
                if (!double.IsNaN(prevY) && !double.IsNaN(currentY))
                {
                    // Проверяем изменение знака (только для корней нечётной кратности)
                    if (prevY * currentY < 0)
                    {
                        // Уточняем корень методом дихотомии на этом отрезке
                        var root = FindRootOnSegment(function, prevX, currentX, precision);
                        if (root.HasValue)
                        {
                            double rootX = root.Value.x;
                            double rootY = root.Value.y;

                            // Проверяем, не добавлен ли уже этот корень (для касаний)
                            bool isDuplicate = false;
                            foreach (var existingRoot in roots)
                            {
                                if (Math.Abs(existingRoot.x - rootX) < epsilon)
                                {
                                    isDuplicate = true;
                                    break;
                                }
                            }

                            if (!isDuplicate && !double.IsNaN(rootY))
                            {
                                roots.Add((rootX, rootY));
                            }
                        }
                    }
                    // Проверяем близость к нулю
                    else if (Math.Abs(prevY) < epsilon || Math.Abs(currentY) < epsilon)
                    {
                        double rootX = Math.Abs(prevY) < epsilon ? prevX : currentX;
                        double rootY = Math.Abs(prevY) < epsilon ? prevY : currentY;

                        // Проверяем, не добавлен ли уже этот корень
                        bool isDuplicate = false;
                        foreach (var existingRoot in roots)
                        {
                            if (Math.Abs(existingRoot.x - rootX) < epsilon)
                            {
                                isDuplicate = true;
                                break;
                            }
                        }

                        if (!isDuplicate && !double.IsNaN(rootY))
                        {
                            roots.Add((rootX, rootY));
                        }
                    }
                }

                prevX = currentX;
                prevY = currentY;
            }

            // Сортируем корни по возрастанию X
            roots = roots.OrderBy(r => r.x).ToList();

            // Объединяем близкие корни (для случаев касаний и точности вычислений)
            return MergeCloseRoots(roots, epsilon);
        }

        private List<(double x, double y)> MergeCloseRoots(List<(double x, double y)> roots, double epsilon)
        {
            if (roots.Count == 0)
                return roots;

            List<(double x, double y)> merged = new List<(double, double)>();
            merged.Add(roots[0]);

            for (int i = 1; i < roots.Count; i++)
            {
                var lastRoot = merged[merged.Count - 1];
                var currentRoot = roots[i];

                // Если корни слишком близки (в пределах точности), пропускаем
                if (Math.Abs(currentRoot.x - lastRoot.x) > epsilon * 10)
                {
                    merged.Add(currentRoot);
                }
                else
                {
                    // Для близких корней выбираем среднее значение
                    double avgX = (lastRoot.x + currentRoot.x) / 2;
                    double avgY = (lastRoot.y + currentRoot.y) / 2;
                    merged[merged.Count - 1] = (avgX, avgY);
                }
            }

            return merged;
        }

        private (double x, double y)? FindRootOnSegment(string function, double a, double b, int precision)
        {
            double epsilon = Math.Pow(10, -precision);

            double fa = EvaluateFunction(function, a);
            double fb = EvaluateFunction(function, b);

            if (double.IsNaN(fa) || double.IsNaN(fb))
                return null;

            // Уже проверено, что fa * fb < 0
            double left = a;
            double right = b;
            double fLeft = fa;

            int iterations = 0;
            double x = 0;

            while (Math.Abs(right - left) > epsilon && iterations < 1000)
            {
                iterations++;
                x = (left + right) / 2;
                double fx = EvaluateFunction(function, x);

                if (double.IsNaN(fx))
                    break;

                if (Math.Abs(fx) < epsilon)
                    break;

                if (fLeft * fx < 0)
                    right = x;
                else
                {
                    left = x;
                    fLeft = fx;
                }
            }

            x = (left + right) / 2;
            double y = EvaluateFunction(function, x);

            if (double.IsNaN(y))
                return null;

            return (x, y);
        }

        private void ButtonStart_Click(object sender, EventArgs e)
        {
            try
            {
                string function = textBoxF.Text.Trim();
                if (string.IsNullOrEmpty(function))
                    throw new ArgumentException("Введите функцию");

                if (!double.TryParse(textBoxA.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double a))
                    throw new ArgumentException("Некорректное значение A");

                if (!double.TryParse(textBoxB.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double b))
                    throw new ArgumentException("Некорректное значение B");

                if (!int.TryParse(textBoxE.Text, out int precision) || precision < 0)
                    throw new ArgumentException("Точность должна быть целым неотрицательным числом");

                if (a >= b)
                    throw new ArgumentException("A должно быть меньше B");

                // Проверяем особые случаи
                string lowerFunction = function.ToLower();

                // Для ln(x) на интервале, не содержащем положительных чисел
                if (lowerFunction.Contains("ln(x)") && b <= 0)
                {
                    textBoxX.Text = "корней нет";
                    textBoxY.Text = "корней нет";
                    MessageBox.Show("Функция ln(x) определена только для x > 0", "Информация",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Для exp(x) - экспонента никогда не равна 0
                if (lowerFunction.Contains("exp(") && !lowerFunction.Contains("exp(x)-") &&
                    !lowerFunction.Contains("exp(x)+") && !lowerFunction.Contains("-exp("))
                {
                    textBoxX.Text = "корней нет";
                    textBoxY.Text = "корней нет";
                    MessageBox.Show("Функция exp(x) всегда положительна и не имеет корней", "Информация",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Для квадратичных функций проверяем особые случаи
                if (IsQuadraticFunction(function) && IsQuadraticFunction(function, out double A, out double B, out double C))
                {
                    double discriminant = B * B - 4 * A * C;
                    if (discriminant < 0)
                    {
                        textBoxX.Text = "корней нет";
                        textBoxY.Text = "корней нет";
                        MessageBox.Show("Дискриминант отрицательный, действительных корней нет", "Информация",
                                      MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }

                // Ищем все корни на интервале
                var roots = FindAllRoots(function, a, b, precision);
                foundRoots = roots.Select(r => r.x).ToList();

                if (roots.Count == 0)
                {
                    // Проверяем, является ли функция гиперболой
                    if (IsHyperbola(function))
                    {
                        textBoxX.Text = "корней нет";
                        textBoxY.Text = "корней нет";
                        MessageBox.Show("Для гиперболы корней нет", "Информация",
                                      MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        textBoxX.Text = "корней нет";
                        textBoxY.Text = "корней нет";
                    }
                }
                else if (roots.Count == 1)
                {
                    var root = roots[0];
                    textBoxX.Text = root.x.ToString($"F{precision}");
                    textBoxY.Text = root.y.ToString($"F{precision}");
                }
                else
                {
                    // Выводим несколько корней через запятую
                    string xValues = string.Join(", ", roots.Select(r => r.x.ToString($"F{precision}")));
                    string yValues = string.Join(", ", roots.Select(r => r.y.ToString($"F{precision}")));

                    textBoxX.Text = xValues;
                    textBoxY.Text = yValues;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ButtonChart_Click(object sender, EventArgs e)
        {
            try
            {
                EnsureChartSeries();
                ClearChartData();

                string function = textBoxF.Text.Trim();
                if (string.IsNullOrEmpty(function))
                    throw new ArgumentException("Введите функцию");

                if (!double.TryParse(textBoxA.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double a))
                    throw new ArgumentException("Некорректное значение A");

                if (!double.TryParse(textBoxB.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double b))
                    throw new ArgumentException("Некорректное значение B");

                if (a >= b)
                    throw new ArgumentException("A должно быть меньше B");

                int precision = 6;
                if (!int.TryParse(textBoxE.Text, out precision))
                    precision = 6;

                // Ищем все корни для отображения
                var roots = FindAllRoots(function, a, b, precision);
                foundRoots = roots.Select(r => r.x).ToList();

                double range = b - a;
                double xMin = a - range * 0.1;
                double xMax = b + range * 0.1;

                // Для ln(x) ограничиваем минимальное значение X
                if (function.ToLower().Contains("ln("))
                {
                    xMin = Math.Max(xMin, 0.001); // Нельзя брать 0 или отрицательные значения
                }

                if (chart.ChartAreas.Count == 0)
                {
                    chart.ChartAreas.Add(new ChartArea("MainArea"));
                }

                ChartArea area = chart.ChartAreas[0];
                area.AxisX.Minimum = xMin;
                area.AxisX.Maximum = xMax;
                area.AxisX.Interval = Math.Max(0.1, range / 10);
                area.AxisX.Title = "X";
                area.AxisY.Title = "F(X)";

                area.AxisX.StripLines.Clear();

                // Добавляем полосу для интервала [A, B] на оси X
                StripLine intervalStrip = new StripLine();
                intervalStrip.Interval = 0;
                intervalStrip.IntervalOffset = a;
                intervalStrip.StripWidth = b - a;
                intervalStrip.BackColor = Color.FromArgb(30, Color.LightBlue);
                intervalStrip.BackSecondaryColor = Color.FromArgb(30, Color.LightBlue);
                intervalStrip.BackGradientStyle = GradientStyle.TopBottom;
                area.AxisX.StripLines.Add(intervalStrip);

                // Добавляем вертикальные линии для границ интервала
                AddVerticalLine(area, a, Color.Red, "A");
                AddVerticalLine(area, b, Color.Red, "B");

                // Проверяем тип функции
                bool isHyperbola = IsHyperbola(function);
                bool isTrigonometric = IsTrigonometricFunction(function);
                bool isLogarithmic = IsLogarithmicFunction(function);
                bool isExponential = IsExponentialFunction(function);
                bool isQuadratic = IsQuadraticFunction(function);
                bool isContinuous = IsContinuousFunction(function);

                // Определяем количество точек для построения графика
                int pointsCount;
                if (isTrigonometric)
                {
                    pointsCount = Math.Min(5000, (int)((xMax - xMin) * 100));
                    pointsCount = Math.Max(pointsCount, 1000);
                }
                else if (isExponential || isLogarithmic)
                {
                    pointsCount = Math.Min(3000, (int)((xMax - xMin) * 75));
                    pointsCount = Math.Max(pointsCount, 1000);
                }
                else
                {
                    pointsCount = Math.Min(2000, (int)((xMax - xMin) * 50));
                    pointsCount = Math.Max(pointsCount, 1000);
                }

                // Построение графика функции
                bool hasValidPoints = false;
                bool previousWasValid = false;

                for (int i = 0; i <= pointsCount; i++)
                {
                    double x = xMin + (xMax - xMin) * i / pointsCount;
                    double y = EvaluateFunction(function, x);

                    if (!double.IsInfinity(y) && !double.IsNaN(y))
                    {
                        chart.Series["Функция"].Points.AddXY(x, y);
                        hasValidPoints = true;
                        previousWasValid = true;
                    }
                    else
                    {
                        // Для разрывных функций добавляем разрыв
                        if (!isContinuous && previousWasValid)
                        {
                            chart.Series["Функция"].Points.AddXY(double.NaN, double.NaN);
                            previousWasValid = false;
                        }
                    }
                }

                if (!hasValidPoints)
                {
                    MessageBox.Show("Не удалось построить график функции. Проверьте корректность ввода функции.\n" +
                                  "Примеры корректных функций: sin(x), cos(x), tan(x), ctg(x), ln(x), exp(x), x^2, x^2+2x+1, x+1, 1/x",
                                  "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                // Отметка всех найденных корней (только точки, без надписей)
                foreach (var root in roots)
                {
                    try
                    {
                        double rootY = EvaluateFunction(function, root.x);
                        if (!double.IsNaN(rootY) && !double.IsInfinity(rootY))
                        {
                            chart.Series["Корень"].Points.AddXY(root.x, rootY);
                        }
                    }
                    catch { }
                }

                // Автомасштабирование по оси Y
                area.AxisY.Minimum = double.NaN;
                area.AxisY.Maximum = double.NaN;

                // Для некоторых функций устанавливаем специальные пределы
                if (isTrigonometric)
                {
                    // Для тригонометрических функций
                    area.AxisY.Minimum = -2;
                    area.AxisY.Maximum = 2;
                }
                else if (isExponential)
                {
                    // Для экспоненциальных функций
                    area.AxisY.Minimum = 0;
                    area.AxisY.Maximum = double.NaN; // Автоматически
                }
                else if (isLogarithmic)
                {
                    // Для логарифмических функций
                    area.AxisY.Minimum = double.NaN; // Автоматически
                    area.AxisY.Maximum = double.NaN; // Автоматически
                }

                // Добавляем сетку
                area.AxisX.MajorGrid.Enabled = true;
                area.AxisY.MajorGrid.Enabled = true;
                area.AxisX.MajorGrid.LineColor = Color.LightGray;
                area.AxisY.MajorGrid.LineColor = Color.LightGray;

                chart.Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при построении графика: {ex.Message}",
                              "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddVerticalLine(ChartArea area, double xValue, Color color, string text)
        {
            // Создаем вертикальную линию
            StripLine verticalLine = new StripLine();
            verticalLine.Interval = 0;
            verticalLine.IntervalOffset = xValue;
            verticalLine.StripWidth = 0.01 * (area.AxisX.Maximum - area.AxisX.Minimum);
            verticalLine.BackColor = color;
            verticalLine.BorderColor = color;
            verticalLine.BorderWidth = 2;
            area.AxisX.StripLines.Add(verticalLine);

            // Добавляем подпись A или B
            var annotation = new TextAnnotation
            {
                Text = text,
                X = xValue,
                Y = area.AxisY.Minimum,
                ForeColor = color,
                Font = new Font("Arial", 10, FontStyle.Bold),
                Visible = true,
                Alignment = ContentAlignment.TopCenter
            };
            chart.Annotations.Add(annotation);
        }

        private void EnsureChartSeries()
        {
            chart.Annotations.Clear();

            if (chart.Series.IndexOf("Функция") == -1)
            {
                Series functionSeries = new Series("Функция")
                {
                    ChartType = SeriesChartType.Line,
                    Color = Color.Blue,
                    BorderWidth = 2
                };
                chart.Series.Add(functionSeries);
            }

            if (chart.Series.IndexOf("Корень") == -1)
            {
                Series rootSeries = new Series("Корень")
                {
                    ChartType = SeriesChartType.Point,
                    Color = Color.DarkGreen,
                    MarkerSize = 8,
                    MarkerStyle = MarkerStyle.Circle
                };
                chart.Series.Add(rootSeries);
            }
        }

        private void ClearChartData()
        {
            // Очищаем точки всех серий
            foreach (Series series in chart.Series)
            {
                series.Points.Clear();
            }

            // Очищаем все аннотации
            chart.Annotations.Clear();

            // Очищаем StripLines
            if (chart.ChartAreas.Count > 0)
            {
                chart.ChartAreas[0].AxisX.StripLines.Clear();
            }

            // Сбрасываем масштаб графика
            if (chart.ChartAreas.Count > 0)
            {
                chart.ChartAreas[0].AxisX.Minimum = double.NaN;
                chart.ChartAreas[0].AxisX.Maximum = double.NaN;
                chart.ChartAreas[0].AxisY.Minimum = double.NaN;
                chart.ChartAreas[0].AxisY.Maximum = double.NaN;
            }
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            Main main = new Main();
            main.Show();
        }

        private void textBoxA_TextChanged(object sender, EventArgs e)
        {

        }
    }

    class ProgramLab
    {
        [STAThread]
        static void Dichotomy()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Dichotomy());
        }
    }
}