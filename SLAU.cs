using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ProgramLab
{
    public partial class SLAU : Form
    {
        private Random random = new Random();
        private CancellationTokenSource cancellationTokenSource;
        private bool isCalculating = false;

        public SLAU()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeDataGridViews();
        }

        private void InitializeDataGridViews()
        {
            int n = (int)numericUpDownN.Value;

            // Очищаем существующие колонки
            dataGridViewInput.Columns.Clear();
            dataGridViewResults.Columns.Clear();

            // Создаем колонки для матрицы A и вектора B
            for (int i = 0; i < n; i++)
            {
                dataGridViewInput.Columns.Add($"A{i + 1}", $"A{i + 1}");
                dataGridViewInput.Columns[i].Width = 50;
            }

            // Добавляем колонку для вектора B
            dataGridViewInput.Columns.Add("B", "B");
            dataGridViewInput.Columns[n].Width = 50;

            // Добавляем строки
            dataGridViewInput.Rows.Add(n);

            // Инициализируем DataGridView для сравнения результатов
            InitializeResultsGrid();

            // Заполняем случайными значениями
            GenerateRandomData();
        }

        private void InitializeResultsGrid()
        {
            dataGridViewResults.Columns.Clear();

            // Добавляем колонки
            dataGridViewResults.Columns.Add("Method", "Метод");
            dataGridViewResults.Columns.Add("Time", "Время (мс)");
            dataGridViewResults.Columns.Add("Status", "Статус");
            dataGridViewResults.Columns.Add("VectorX", "Вектор X (первые 5 значений)");

            dataGridViewResults.Columns[0].Width = 150;
            dataGridViewResults.Columns[1].Width = 100;
            dataGridViewResults.Columns[2].Width = 150;
            dataGridViewResults.Columns[3].Width = 250;
        }

        private void GenerateRandomData()
        {
            int n = (int)numericUpDownN.Value;

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    dataGridViewInput.Rows[i].Cells[j].Value = random.Next(-10, 11);
                }
                dataGridViewInput.Rows[i].Cells[n].Value = random.Next(-10, 11);
            }
        }

        private void NumericUpDownN_ValueChanged(object sender, EventArgs e)
        {
            if (!isCalculating)
            {
                InitializeDataGridViews();
            }
        }

        private void GenerateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!isCalculating)
            {
                GenerateRandomData();
                MessageBox.Show("Данные сгенерированы случайным образом.", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private bool ValidateInputData()
        {
            int n = (int)numericUpDownN.Value;

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j <= n; j++)
                {
                    if (dataGridViewInput.Rows[i].Cells[j].Value == null ||
                        string.IsNullOrWhiteSpace(dataGridViewInput.Rows[i].Cells[j].Value.ToString()))
                    {
                        MessageBox.Show($"Заполните все ячейки матрицы A и вектора B.\nСтрока {i + 1}, колонка {(j < n ? $"A{j + 1}" : "B")}",
                            "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    if (!double.TryParse(dataGridViewInput.Rows[i].Cells[j].Value.ToString(), out double value))
                    {
                        MessageBox.Show($"Некорректное значение в ячейке: строка {i + 1}, колонка {(j < n ? $"A{j + 1}" : "B")}",
                            "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            }

            return true;
        }

        private double[,] GetMatrixA()
        {
            int n = (int)numericUpDownN.Value;
            double[,] A = new double[n, n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    A[i, j] = Convert.ToDouble(dataGridViewInput.Rows[i].Cells[j].Value);
                }
            }

            return A;
        }

        private double[] GetVectorB()
        {
            int n = (int)numericUpDownN.Value;
            double[] B = new double[n];

            for (int i = 0; i < n; i++)
            {
                B[i] = Convert.ToDouble(dataGridViewInput.Rows[i].Cells[n].Value);
            }

            return B;
        }

        private async void GaussToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!ValidateInputData() || isCalculating)
                return;

            await SolveWithMethodAsync("Гаусса", SolveGaussAsync);
        }

        private async void JordanGaussToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!ValidateInputData() || isCalculating)
                return;

            await SolveWithMethodAsync("Жордана-Гаусса", SolveJordanGaussAsync);
        }

        private async void CramerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!ValidateInputData() || isCalculating)
                return;

            await SolveWithMethodAsync("Крамера", SolveCramerAsync);
        }

        private async void AllMethodsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!ValidateInputData() || isCalculating)
                return;

            await CompareAllMethodsAsync();
        }

        private async Task CompareAllMethodsAsync()
        {
            StartCalculation();

            try
            {
                double[,] A = GetMatrixA();
                double[] B = GetVectorB();

                // Очищаем таблицу результатов
                dataGridViewResults.Rows.Clear();

                // Обновляем статус
                UpdateStatus("Запуск всех методов...");

                // Выполняем все методы параллельно
                var tasks = new[]
                {
                    RunMethodAsync("Метод Гаусса", SolveGaussAsync, A, B),
                    RunMethodAsync("Метод Жордана-Гаусса", SolveJordanGaussAsync, A, B),
                    RunMethodAsync("Метод Крамера", SolveCramerAsync, A, B)
                };

                await Task.WhenAll(tasks);

                // Находим самый быстрый метод
                double fastestTime = double.MaxValue;
                string fastestMethod = "";

                foreach (DataGridViewRow row in dataGridViewResults.Rows)
                {
                    if (row.Cells[1].Value != null && row.Cells[1].Value.ToString() != "Ошибка")
                    {
                        if (double.TryParse(row.Cells[1].Value.ToString(), out double time))
                        {
                            if (time < fastestTime)
                            {
                                fastestTime = time;
                                fastestMethod = row.Cells[0].Value.ToString();
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(fastestMethod))
                {
                    labelTime.Text = $"Самый быстрый метод: {fastestMethod} ({fastestTime:F2} мс)";
                }

                UpdateStatus("Все вычисления завершены");
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Вычисления отменены пользователем.", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                UpdateStatus("Вычисления отменены");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при решении СЛАУ: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateStatus("Ошибка вычислений");
            }
            finally
            {
                StopCalculation();
            }
        }

        private async Task RunMethodAsync(string methodName, Func<double[,], double[], CancellationToken, Task<double[]>> method, double[,] A, double[] B)
        {
            try
            {
                UpdateStatus($"Запуск {methodName}...");

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                // Выполняем метод асинхронно
                double[] result = await method(A, B, cancellationTokenSource.Token);

                stopwatch.Stop();

                // Обновляем UI в основном потоке
                this.Invoke((MethodInvoker)delegate
                {
                    int rowIndex = dataGridViewResults.Rows.Add();
                    dataGridViewResults.Rows[rowIndex].Cells[0].Value = methodName;
                    dataGridViewResults.Rows[rowIndex].Cells[1].Value = stopwatch.Elapsed.TotalMilliseconds.ToString("F2");
                    dataGridViewResults.Rows[rowIndex].Cells[2].Value = "Успешно";

                    // Формируем строку с результатом
                    string resultStr = "[";
                    for (int i = 0; i < Math.Min(5, result.Length); i++)
                    {
                        resultStr += result[i].ToString("F4");
                        if (i < Math.Min(5, result.Length) - 1)
                            resultStr += ", ";
                    }
                    if (result.Length > 5)
                        resultStr += ", ...";
                    resultStr += "]";

                    dataGridViewResults.Rows[rowIndex].Cells[3].Value = resultStr;

                    // Обновляем метки времени
                    switch (methodName)
                    {
                        case "Метод Гаусса":
                            labelGaussTime.Text = $"Гаусса: {stopwatch.Elapsed.TotalMilliseconds:F2} мс";
                            break;
                        case "Метод Жордана-Гаусса":
                            labelJordanTime.Text = $"Жордана-Гаусса: {stopwatch.Elapsed.TotalMilliseconds:F2} мс";
                            break;
                        case "Метод Крамера":
                            labelCramerTime.Text = $"Крамера: {stopwatch.Elapsed.TotalMilliseconds:F2} мс";
                            break;
                    }
                });
            }
            catch (OperationCanceledException)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    int rowIndex = dataGridViewResults.Rows.Add();
                    dataGridViewResults.Rows[rowIndex].Cells[0].Value = methodName;
                    dataGridViewResults.Rows[rowIndex].Cells[1].Value = "Отменено";
                    dataGridViewResults.Rows[rowIndex].Cells[2].Value = "Прервано пользователем";
                    dataGridViewResults.Rows[rowIndex].Cells[3].Value = "";
                });
                throw;
            }
            catch (Exception ex)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    int rowIndex = dataGridViewResults.Rows.Add();
                    dataGridViewResults.Rows[rowIndex].Cells[0].Value = methodName;
                    dataGridViewResults.Rows[rowIndex].Cells[1].Value = "Ошибка";
                    dataGridViewResults.Rows[rowIndex].Cells[2].Value = ex.Message;
                    dataGridViewResults.Rows[rowIndex].Cells[3].Value = "";
                });
            }
        }

        private async Task SolveWithMethodAsync(string methodName, Func<double[,], double[], CancellationToken, Task<double[]>> method)
        {
            StartCalculation();

            try
            {
                double[,] A = GetMatrixA();
                double[] B = GetVectorB();

                UpdateStatus($"Выполнение метода {methodName}...");

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                // Асинхронное выполнение вычислений
                double[] X = await method(A, B, cancellationTokenSource.Token);

                stopwatch.Stop();

                // Обновляем UI в основном потоке
                this.Invoke((MethodInvoker)delegate
                {
                    // Добавляем результат в таблицу
                    int rowIndex = dataGridViewResults.Rows.Add();
                    dataGridViewResults.Rows[rowIndex].Cells[0].Value = $"Метод {methodName}";
                    dataGridViewResults.Rows[rowIndex].Cells[1].Value = stopwatch.Elapsed.TotalMilliseconds.ToString("F2");
                    dataGridViewResults.Rows[rowIndex].Cells[2].Value = "Успешно";

                    // Формируем строку с результатом
                    string resultStr = "[";
                    for (int i = 0; i < Math.Min(5, X.Length); i++)
                    {
                        resultStr += X[i].ToString("F4");
                        if (i < Math.Min(5, X.Length) - 1)
                            resultStr += ", ";
                    }
                    if (X.Length > 5)
                        resultStr += ", ...";
                    resultStr += "]";

                    dataGridViewResults.Rows[rowIndex].Cells[3].Value = resultStr;

                    labelTime.Text = $"Время выполнения ({methodName}): {stopwatch.Elapsed.TotalMilliseconds:F2} мс";

                    // Обновляем метку времени для конкретного метода
                    switch (methodName)
                    {
                        case "Гаусса":
                            labelGaussTime.Text = $"Гаусса: {stopwatch.Elapsed.TotalMilliseconds:F2} мс";
                            break;
                        case "Жордана-Гаусса":
                            labelJordanTime.Text = $"Жордана-Гаусса: {stopwatch.Elapsed.TotalMilliseconds:F2} мс";
                            break;
                        case "Крамера":
                            labelCramerTime.Text = $"Крамера: {stopwatch.Elapsed.TotalMilliseconds:F2} мс";
                            break;
                    }
                });

                UpdateStatus($"Метод {methodName} выполнен");
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Вычисления отменены пользователем.", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                UpdateStatus("Вычисления отменены");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при решении СЛАУ: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateStatus("Ошибка вычислений");
            }
            finally
            {
                StopCalculation();
            }
        }

        private void StartCalculation()
        {
            isCalculating = true;
            btnStop.Enabled = true;
            progressBar.Visible = true;
            progressBar.Style = ProgressBarStyle.Marquee;
            cancellationTokenSource = new CancellationTokenSource();

            // Блокируем возможность изменения размера матрицы
            numericUpDownN.Enabled = false;
        }

        private void StopCalculation()
        {
            isCalculating = false;
            btnStop.Enabled = false;
            progressBar.Visible = false;
            numericUpDownN.Enabled = true;
            btnStop.Text = "Остановить";

            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Dispose();
                cancellationTokenSource = null;
            }
        }

        private void UpdateStatus(string status)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    labelStatus.Text = status;
                });
            }
            else
            {
                labelStatus.Text = status;
            }
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            if (cancellationTokenSource != null && !cancellationTokenSource.IsCancellationRequested)
            {
                cancellationTokenSource.Cancel();
                btnStop.Text = "Отмена...";
                UpdateStatus("Отмена вычислений...");
            }
        }

        // Асинхронная версия метода Гаусса
        private async Task<double[]> SolveGaussAsync(double[,] A, double[] B, CancellationToken cancellationToken)
        {
            return await Task.Run(() => SolveGaussInternal(A, B, cancellationToken));
        }

        private double[] SolveGaussInternal(double[,] A, double[] B, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            int n = B.Length;
            double[] X = new double[n];

            // Создаем расширенную матрицу
            double[,] AB = new double[n, n + 1];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    AB[i, j] = A[i, j];
                }
                AB[i, n] = -B[i];
            }

            // Прямой ход
            for (int i = 0; i < n; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Поиск ведущего элемента
                int maxRow = i;
                for (int k = i + 1; k < n; k++)
                {
                    if (Math.Abs(AB[k, i]) > Math.Abs(AB[maxRow, i]))
                    {
                        maxRow = k;
                    }
                }

                // Перестановка строк
                if (maxRow != i)
                {
                    for (int k = 0; k <= n; k++)
                    {
                        double temp = AB[i, k];
                        AB[i, k] = AB[maxRow, k];
                        AB[maxRow, k] = temp;
                    }
                }

                // Проверка на нулевой ведущий элемент
                if (Math.Abs(AB[i, i]) < 1e-10)
                {
                    throw new Exception("Матрица вырожденная или система не имеет единственного решения");
                }

                // Нормализация строки
                double divisor = AB[i, i];
                for (int k = i; k <= n; k++)
                {
                    AB[i, k] /= divisor;
                }

                // Исключение переменной
                for (int k = i + 1; k < n; k++)
                {
                    double factor = AB[k, i];
                    for (int j = i; j <= n; j++)
                    {
                        AB[k, j] -= factor * AB[i, j];
                    }
                }
            }

            // Обратный ход
            for (int i = n - 1; i >= 0; i--)
            {
                cancellationToken.ThrowIfCancellationRequested();

                X[i] = AB[i, n];
                for (int j = i + 1; j < n; j++)
                {
                    X[i] -= AB[i, j] * X[j];
                }
            }

            return X;
        }

        // Асинхронная версия метода Жордана-Гаусса
        private async Task<double[]> SolveJordanGaussAsync(double[,] A, double[] B, CancellationToken cancellationToken)
        {
            return await Task.Run(() => SolveJordanGaussInternal(A, B, cancellationToken));
        }

        private double[] SolveJordanGaussInternal(double[,] A, double[] B, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            int n = B.Length;
            double[] X = new double[n];

            // Создаем расширенную матрицу
            double[,] AB = new double[n, n + 1];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    AB[i, j] = A[i, j];
                }
                AB[i, n] = -B[i];
            }

            // Метод Жордана-Гаусса
            for (int i = 0; i < n; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Поиск ведущего элемента
                int maxRow = i;
                for (int k = i + 1; k < n; k++)
                {
                    if (Math.Abs(AB[k, i]) > Math.Abs(AB[maxRow, i]))
                    {
                        maxRow = k;
                    }
                }

                // Перестановка строк
                if (maxRow != i)
                {
                    for (int k = 0; k <= n; k++)
                    {
                        double temp = AB[i, k];
                        AB[i, k] = AB[maxRow, k];
                        AB[maxRow, k] = temp;
                    }
                }

                // Проверка на нулевой ведущий элемент
                if (Math.Abs(AB[i, i]) < 1e-10)
                {
                    throw new Exception("Матрица вырожденная или система не имеет единственного решения");
                }

                // Нормализация ведущей строки
                double divisor = AB[i, i];
                for (int k = 0; k <= n; k++)
                {
                    AB[i, k] /= divisor;
                }

                // Исключение переменной из всех строк кроме текущей
                for (int k = 0; k < n; k++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (k != i)
                    {
                        double factor = AB[k, i];
                        for (int j = 0; j <= n; j++)
                        {
                            AB[k, j] -= factor * AB[i, j];
                        }
                    }
                }
            }

            // Извлекаем решение
            for (int i = 0; i < n; i++)
            {
                X[i] = AB[i, n];
            }

            return X;
        }

        // Асинхронная версия метода Крамера
        private async Task<double[]> SolveCramerAsync(double[,] A, double[] B, CancellationToken cancellationToken)
        {
            return await Task.Run(() => SolveCramerInternal(A, B, cancellationToken));
        }

        private double[] SolveCramerInternal(double[,] A, double[] B, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            int n = B.Length;
            double[] X = new double[n];

            // Вычисляем определитель основной матрицы асинхронно
            double detA = DeterminantAsync(A, cancellationToken).GetAwaiter().GetResult();

            if (Math.Abs(detA) < 1e-10)
            {
                throw new Exception("Определитель матрицы A равен нулю. Метод Крамера неприменим.");
            }

            // Для каждого неизвестного
            for (int i = 0; i < n; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Создаем копию матрицы A
                double[,] Ai = (double[,])A.Clone();

                // Заменяем i-й столбец на вектор -B
                for (int j = 0; j < n; j++)
                {
                    Ai[j, i] = -B[j];
                }

                // Вычисляем определитель асинхронно
                double detAi = DeterminantAsync(Ai, cancellationToken).GetAwaiter().GetResult();

                // Находим решение
                X[i] = detAi / detA;
            }

            return X;
        }

        // Асинхронное вычисление определителя
        private async Task<double> DeterminantAsync(double[,] matrix, CancellationToken cancellationToken)
        {
            return await Task.Run(() => DeterminantInternal(matrix, cancellationToken));
        }

        private double DeterminantInternal(double[,] matrix, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            int n = matrix.GetLength(0);

            if (n == 1)
            {
                return matrix[0, 0];
            }
            else if (n == 2)
            {
                return matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0];
            }
            else
            {
                double det = 0;
                for (int j = 0; j < n; j++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    // Создаем минор
                    double[,] minor = new double[n - 1, n - 1];
                    for (int k = 1; k < n; k++)
                    {
                        int colIndex = 0;
                        for (int l = 0; l < n; l++)
                        {
                            if (l != j)
                            {
                                minor[k - 1, colIndex] = matrix[k, l];
                                colIndex++;
                            }
                        }
                    }

                    det += matrix[0, j] * Math.Pow(-1, j) * DeterminantInternal(minor, cancellationToken);
                }
                return det;
            }
        }

        private void ClearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isCalculating)
            {
                MessageBox.Show("Дождитесь окончания вычислений.", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            dataGridViewInput.Rows.Clear();
            dataGridViewResults.Rows.Clear();

            // Переинициализируем таблицы
            InitializeDataGridViews();

            labelTime.Text = "Время выполнения: ";
            labelGaussTime.Text = "Гаусса: не вычислено";
            labelJordanTime.Text = "Жордана-Гаусса: не вычислено";
            labelCramerTime.Text = "Крамера: не вычислено";
            labelStatus.Text = "Готов к вычислениям";

            MessageBox.Show("Все поля очищены.", "Информация",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void LoadFromExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Загрузка из Excel требует установки Microsoft.Office.Interop.Excel.\n" +
                          "В данном примере используется генерация случайных данных.",
                          "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Временно используем генерацию
            GenerateRandomData();
        }

        private void LoadFromGoogleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Загрузка из Google Tables требует настройки Google Sheets API.\n" +
                          "В данном примере используется генерация случайных данных.",
                          "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Временно используем генерацию
            GenerateRandomData();
        }
    }
}