using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using System.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ProgramLab
{
    public partial class Sort : Form
    {
        private List<int> numbers = new List<int>();
        private List<SortingAlgorithm> algorithms = new List<SortingAlgorithm>();
        private CancellationTokenSource cts;
        private System.Windows.Forms.Timer visualizationTimer;
        private List<SortResult> sortResults = new List<SortResult>();
        private const int BOGO_MAX_ITERATIONS = 1000000;
        private const int BOGO_MAX_ARRAY_SIZE = 10000;
        private const int MAX_GENERATED_ELEMENTS = 10000;

        public Sort()
        {
            InitializeComponent();
            InitializeAlgorithms();
            InitializeVisualizationTimer();
            SetupDataGridView();
        }

        private void InitializeAlgorithms()
        {
            algorithms.Add(new SortingAlgorithm("Пузырьковая", SortType.Bubble));
            algorithms.Add(new SortingAlgorithm("Вставками", SortType.Insertion));
            algorithms.Add(new SortingAlgorithm("Шейкерная", SortType.Shaker));
            algorithms.Add(new SortingAlgorithm("Быстрая", SortType.Quick));
            algorithms.Add(new SortingAlgorithm("BOGO", SortType.Bogo));
        }

        private void InitializeVisualizationTimer()
        {
            visualizationTimer = new System.Windows.Forms.Timer();
            visualizationTimer.Interval = 50;
            visualizationTimer.Tick += VisualizationTimer_Tick;
        }

        private void SetupDataGridView()
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.Add("Value", "Значение");
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.RowHeadersVisible = false;
        }

        private async void startSortingButton_Click(object sender, EventArgs e)
        {
            await StartSorting();
        }

        private async Task StartSorting()
        {
            sortResults.Clear();
            chart1.Series.Clear();
            resultsListBox.Items.Clear();
            visualizationTimer.Stop();

            numbers = GetNumbersFromGrid();
            if (numbers.Count == 0)
            {
                MessageBox.Show("Пожалуйста, введите данные для сортировки.", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedAlgorithms = GetSelectedAlgorithms();
            if (selectedAlgorithms.Count == 0)
            {
                MessageBox.Show("Пожалуйста, выберите хотя бы один алгоритм сортировки.", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (bogoSortCheckBox.Checked)
            {
                if (numbers.Count > BOGO_MAX_ARRAY_SIZE)
                {
                    var result = MessageBox.Show(
                        $"Bogo сортировка не рекомендуется для массивов размером более {BOGO_MAX_ARRAY_SIZE} элементов.\n" +
                        $"Текущий размер: {numbers.Count} элементов.\n\n" +
                        "Продолжить? (Это может занять очень много времени или не завершиться вовсе)",
                        "Предупреждение",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (result == DialogResult.No)
                    {
                        bogoSortCheckBox.Checked = false;
                        selectedAlgorithms = GetSelectedAlgorithms();
                        if (selectedAlgorithms.Count == 0)
                        {
                            MessageBox.Show("Вы отменили Bogo сортировку. Выберите другие алгоритмы.", "Информация",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }
                }
            }

            SetControlsState(false);
            cts = new CancellationTokenSource();

            try
            {
                var tasks = selectedAlgorithms.Select(alg =>
                    RunSortingAlgorithm(alg, numbers.ToList(), cts.Token)).ToList();

                await Task.WhenAll(tasks);

                DisplayResults();
                visualizationTimer.Start();
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Сортировка была отменена.", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (BogoSortTimeoutException ex)
            {
                MessageBox.Show($"Bogo сортировка превысила лимит итераций ({BOGO_MAX_ITERATIONS}).\n" +
                    $"Алгоритм был остановлен.", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                DisplayResults();
                visualizationTimer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetControlsState(true);
            }
        }

        private List<int> GetNumbersFromGrid()
        {
            var result = new List<int>();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells[0].Value != null && row.Cells[0].Value.ToString() != "")
                {
                    if (int.TryParse(row.Cells[0].Value.ToString(), out int value))
                    {
                        result.Add(value);
                    }
                    else
                    {
                        MessageBox.Show($"Некорректное значение: {row.Cells[0].Value}. Введите целое число.", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return new List<int>();
                    }
                }
            }
            return result;
        }

        private List<SortingAlgorithm> GetSelectedAlgorithms()
        {
            var selected = new List<SortingAlgorithm>();

            if (bubbleSortCheckBox.Checked)
                selected.Add(algorithms.Find(a => a.Type == SortType.Bubble));
            if (insertionSortCheckBox.Checked)
                selected.Add(algorithms.Find(a => a.Type == SortType.Insertion));
            if (shakerSortCheckBox.Checked)
                selected.Add(algorithms.Find(a => a.Type == SortType.Shaker));
            if (quickSortCheckBox.Checked)
                selected.Add(algorithms.Find(a => a.Type == SortType.Quick));
            if (bogoSortCheckBox.Checked)
                selected.Add(algorithms.Find(a => a.Type == SortType.Bogo));

            return selected;
        }

        private async Task<SortResult> RunSortingAlgorithm(SortingAlgorithm algorithm,
            List<int> data, CancellationToken token)
        {
            var sortOrder = ascendingRadioButton.Checked ? SortOrder.Ascending : SortOrder.Descending;
            var stopwatch = Stopwatch.StartNew();
            int iterations = 0;

            var dataCopy = new List<int>(data);
            List<int> sortedData = null;
            bool bogoTimeout = false;

            await Task.Run(() =>
            {
                try
                {
                    sortedData = algorithm.Sort(dataCopy, sortOrder, ref iterations, token, BOGO_MAX_ITERATIONS);
                }
                catch (BogoSortTimeoutException)
                {
                    bogoTimeout = true;
                    sortedData = dataCopy;
                }
            }, token);

            stopwatch.Stop();

            var result = new SortResult
            {
                AlgorithmName = algorithm.Name,
                ExecutionTime = stopwatch.Elapsed,
                Iterations = iterations,
                SortedData = sortedData,
                AlgorithmType = algorithm.Type,
                BogoTimeout = bogoTimeout
            };

            lock (sortResults)
            {
                sortResults.Add(result);
            }

            return result;
        }

        private void DisplayResults()
        {
            resultsListBox.Items.Clear();

            var validResults = sortResults.Where(r => !r.BogoTimeout).ToList();
            var fastest = validResults.OrderBy(r => r.ExecutionTime).FirstOrDefault();

            foreach (var result in sortResults.OrderBy(r => r.ExecutionTime))
            {
                string resultText = $"{result.AlgorithmName}: " +
                    $"{result.ExecutionTime.TotalMilliseconds:F2} мс, " +
                    $"{result.Iterations} итераций";

                if (result.BogoTimeout)
                {
                    resultText += " [ПРЕВЫШЕН ЛИМИТ]";
                }
                else if (result == fastest && validResults.Count > 1)
                {
                    resultText += " [САМЫЙ БЫСТРЫЙ]";
                }

                resultsListBox.Items.Add(resultText);
            }

            var successfulResult = sortResults.FirstOrDefault(r => !r.BogoTimeout);
            if (successfulResult != null)
            {
                sortedArrayTextBox.Text = string.Join(", ", successfulResult.SortedData);
            }
            else if (sortResults.Count > 0)
            {
                sortedArrayTextBox.Text = string.Join(", ", sortResults[0].SortedData) +
                    " (не отсортировано - превышен лимит)";
            }
        }

        private void VisualizationTimer_Tick(object sender, EventArgs e)
        {
            UpdateChart();
        }

        private void UpdateChart()
        {
            if (chart1.InvokeRequired)
            {
                chart1.Invoke(new Action(UpdateChart));
                return;
            }

            chart1.Series.Clear();
            chart1.ChartAreas[0].AxisY.Title = "Время (мс)";
            chart1.ChartAreas[0].AxisX.Title = "Алгоритмы";
            chart1.ChartAreas[0].AxisX.Interval = 1;

            foreach (var result in sortResults.Where(r => !r.BogoTimeout))
            {
                var series = new System.Windows.Forms.DataVisualization.Charting.Series
                {
                    Name = result.AlgorithmName,
                    ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column
                };

                series.Points.AddY(result.ExecutionTime.TotalMilliseconds);
                chart1.Series.Add(series);
            }

            chart1.Titles.Clear();
            chart1.Titles.Add("Время выполнения алгоритмов сортировки");
        }

        private void SetControlsState(bool enabled)
        {
            startToolStripMenuItem.Enabled = enabled;
            stopToolStripMenuItem.Enabled = !enabled;
            bubbleSortCheckBox.Enabled = enabled;
            insertionSortCheckBox.Enabled = enabled;
            shakerSortCheckBox.Enabled = enabled;
            quickSortCheckBox.Enabled = enabled;
            bogoSortCheckBox.Enabled = enabled;
            ascendingRadioButton.Enabled = enabled;
            descendingRadioButton.Enabled = enabled;
            dataGridView1.Enabled = enabled;
            generateDataButton.Enabled = enabled;
            clearDataButton.Enabled = enabled;
            countNumericUpDown.Enabled = enabled;
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cts?.Cancel();
            visualizationTimer.Stop();
            SetControlsState(true);
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearAll();
        }

        private void ClearAll()
        {
            ClearDataGridView();
            chart1.Series.Clear();
            resultsListBox.Items.Clear();
            sortedArrayTextBox.Clear();
            sortResults.Clear();
        }

        private void generateDataButton_Click(object sender, EventArgs e)
        {
            GenerateRandomData();
        }

        private void GenerateRandomData()
        {
            try
            {
                int count = (int)countNumericUpDown.Value;
                if (count <= 0 || count > MAX_GENERATED_ELEMENTS)
                {
                    MessageBox.Show($"Пожалуйста, введите количество чисел от 1 до {MAX_GENERATED_ELEMENTS}.", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                ClearDataGridView();

                Random rand = new Random();

                // Используем StringBuilder для больших массивов
                if (count > 1000)
                {
                    dataGridView1.SuspendLayout();
                    for (int i = 0; i < count; i++)
                    {
                        int rowIndex = dataGridView1.Rows.Add();
                        dataGridView1.Rows[rowIndex].Cells[0].Value = rand.Next(-10000, 10001);
                    }
                    dataGridView1.ResumeLayout();
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        int rowIndex = dataGridView1.Rows.Add();
                        dataGridView1.Rows[rowIndex].Cells[0].Value = rand.Next(-10000, 10001);
                    }
                }

                // Показываем информационное сообщение для больших массивов
                if (count > 1000)
                {
                    toolTip1.Show($"Сгенерировано {count} чисел", generateDataButton, 2000);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при генерации данных: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearDataGridView()
        {
            try
            {
                dataGridView1.SuspendLayout();

                // Создаем новый DataSource для очистки
                dataGridView1.DataSource = null;

                // Очищаем строки
                dataGridView1.Rows.Clear();

                // Пересоздаем колонку
                if (dataGridView1.Columns.Count == 0)
                {
                    dataGridView1.Columns.Add("Value", "Значение");
                }

                dataGridView1.ResumeLayout();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при очистке таблицы: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void clearDataButton_Click(object sender, EventArgs e)
        {
            ClearDataGridView();
        }

        private void loadFromExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadFromExcel();
        }

        private void LoadFromExcel()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Excel Files|*.xls;*.xlsx";
                openFileDialog.Title = "Выберите Excel файл";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var excelApp = new Excel.Application();
                        var workbook = excelApp.Workbooks.Open(openFileDialog.FileName);
                        var worksheet = (Excel.Worksheet)workbook.Sheets[1];
                        var range = worksheet.UsedRange;

                        ClearDataGridView();

                        int rowCount = range.Rows.Count;
                        if (rowCount > MAX_GENERATED_ELEMENTS)
                        {
                            var result = MessageBox.Show(
                                $"Файл содержит {rowCount} строк, что превышает лимит {MAX_GENERATED_ELEMENTS}.\n" +
                                $"Загрузить только первые {MAX_GENERATED_ELEMENTS} строк?",
                                "Предупреждение",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Warning);

                            if (result == DialogResult.No)
                            {
                                workbook.Close(false);
                                excelApp.Quit();
                                return;
                            }
                            rowCount = MAX_GENERATED_ELEMENTS;
                        }

                        dataGridView1.SuspendLayout();
                        for (int i = 1; i <= rowCount; i++)
                        {
                            if (range.Cells[i, 1].Value != null)
                            {
                                int rowIndex = dataGridView1.Rows.Add();
                                dataGridView1.Rows[rowIndex].Cells[0].Value = range.Cells[i, 1].Value.ToString();
                            }
                        }
                        dataGridView1.ResumeLayout();

                        workbook.Close(false);
                        excelApp.Quit();

                        System.Runtime.InteropServices.Marshal.ReleaseComObject(range);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка загрузки из Excel: {ex.Message}", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex == 0 && e.FormattedValue != null)
            {
                string value = e.FormattedValue.ToString();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    if (!int.TryParse(value, out _))
                    {
                        e.Cancel = true;
                        MessageBox.Show("Пожалуйста, введите целое число.", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void countNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            int value = (int)countNumericUpDown.Value;
            if (value > MAX_GENERATED_ELEMENTS)
            {
                countNumericUpDown.Value = MAX_GENERATED_ELEMENTS;
                value = MAX_GENERATED_ELEMENTS;
            }

            toolTip1.SetToolTip(countNumericUpDown,
                $"Будет сгенерировано {value} чисел\nМаксимум: {MAX_GENERATED_ELEMENTS}");
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                if (dataGridView1.CurrentCell != null)
                {
                    int rowIndex = dataGridView1.CurrentCell.RowIndex;
                    int colIndex = dataGridView1.CurrentCell.ColumnIndex;

                    if (rowIndex == dataGridView1.Rows.Count - 1 &&
                        colIndex == dataGridView1.Columns.Count - 1)
                    {
                        if (dataGridView1.Rows.Count < MAX_GENERATED_ELEMENTS)
                        {
                            dataGridView1.Rows.Add();
                        }
                        else
                        {
                            MessageBox.Show($"Достигнут максимальный лимит строк: {MAX_GENERATED_ELEMENTS}",
                                "Ограничение",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Устанавливаем максимальное значение
            countNumericUpDown.Maximum = MAX_GENERATED_ELEMENTS;
            countNumericUpDown.Minimum = 1;
            countNumericUpDown.Value = Math.Min(100, MAX_GENERATED_ELEMENTS);

            // Настраиваем tooltip
            toolTip1.SetToolTip(countNumericUpDown,
                $"Максимальное количество: {MAX_GENERATED_ELEMENTS}\nТекущее: {countNumericUpDown.Value}");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Main main = new Main();
            main.Show();
            Close();
        }
    }

    public class BogoSortTimeoutException : Exception
    {
        public BogoSortTimeoutException() : base("Превышен лимит итераций для Bogo сортировки") { }
    }

    public enum SortType { Bubble, Insertion, Shaker, Quick, Bogo }
    public enum SortOrder { Ascending, Descending }

    public class SortingAlgorithm
    {
        public string Name { get; }
        public SortType Type { get; }

        public SortingAlgorithm(string name, SortType type)
        {
            Name = name;
            Type = type;
        }

        public List<int> Sort(List<int> data, SortOrder order, ref int iterations,
            CancellationToken token, int maxIterations = int.MaxValue)
        {
            iterations = 0;
            var dataCopy = new List<int>(data);

            switch (Type)
            {
                case SortType.Bubble:
                    return BubbleSort(dataCopy, order, ref iterations, token);
                case SortType.Insertion:
                    return InsertionSort(dataCopy, order, ref iterations, token);
                case SortType.Shaker:
                    return ShakerSort(dataCopy, order, ref iterations, token);
                case SortType.Quick:
                    return QuickSort(dataCopy, order, ref iterations, token);
                case SortType.Bogo:
                    return BogoSort(dataCopy, order, ref iterations, token, maxIterations);
                default:
                    return dataCopy;
            }
        }

        private List<int> BubbleSort(List<int> array, SortOrder order, ref int iterations, CancellationToken token)
        {
            int n = array.Count;
            int comparisons = 0;

            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - i - 1; j++)
                {
                    token.ThrowIfCancellationRequested();
                    comparisons++;  // Каждое сравнение - итерация

                    bool shouldSwap = order == SortOrder.Ascending
                        ? array[j] > array[j + 1]
                        : array[j] < array[j + 1];

                    if (shouldSwap)
                    {
                        int temp = array[j];
                        array[j] = array[j + 1];
                        array[j + 1] = temp;
                    }
                }
            }

            iterations = comparisons;
            return array;
        }

        private List<int> InsertionSort(List<int> array, SortOrder order, ref int iterations, CancellationToken token)
        {
            int n = array.Count;
            int comparisons = 0;

            for (int i = 1; i < n; ++i)
            {
                int key = array[i];
                int j = i - 1;

                // Сравниваем key с элементами отсортированной части
                while (j >= 0)
                {
                    token.ThrowIfCancellationRequested();
                    comparisons++;  // Каждое сравнение - итерация

                    bool shouldMove = order == SortOrder.Ascending
                        ? array[j] > key
                        : array[j] < key;

                    if (!shouldMove)
                        break;

                    array[j + 1] = array[j];
                    j = j - 1;
                }
                array[j + 1] = key;
            }

            iterations = comparisons;
            return array;
        }

        private List<int> ShakerSort(List<int> array, SortOrder order, ref int iterations, CancellationToken token)
        {
            int left = 0;
            int right = array.Count - 1;
            bool swapped = true;
            int comparisons = 0;

            while (left < right && swapped)
            {
                swapped = false;

                // Проход слева направо
                for (int i = left; i < right; i++)
                {
                    token.ThrowIfCancellationRequested();
                    comparisons++;

                    bool shouldSwap = order == SortOrder.Ascending
                        ? array[i] > array[i + 1]
                        : array[i] < array[i + 1];

                    if (shouldSwap)
                    {
                        int temp = array[i];
                        array[i] = array[i + 1];
                        array[i + 1] = temp;
                        swapped = true;
                    }
                }
                right--;

                // Проход справа налево
                for (int i = right; i > left; i--)
                {
                    token.ThrowIfCancellationRequested();
                    comparisons++;

                    bool shouldSwap = order == SortOrder.Ascending
                        ? array[i - 1] > array[i]
                        : array[i - 1] < array[i];

                    if (shouldSwap)
                    {
                        int temp = array[i];
                        array[i] = array[i - 1];
                        array[i - 1] = temp;
                        swapped = true;
                    }
                }
                left++;
            }

            iterations = comparisons;
            return array;
        }

        private List<int> QuickSort(List<int> array, SortOrder order, ref int iterations, CancellationToken token)
        {
            QuickSortRecursive(array, 0, array.Count - 1, order, ref iterations, token);
            return array;
        }

        private void QuickSortRecursive(List<int> array, int low, int high, SortOrder order,
            ref int iterations, CancellationToken token)
        {
            if (low < high)
            {
                token.ThrowIfCancellationRequested();

                int pi = Partition(array, low, high, order, ref iterations, token);
                QuickSortRecursive(array, low, pi - 1, order, ref iterations, token);
                QuickSortRecursive(array, pi + 1, high, order, ref iterations, token);
            }
        }

        private int Partition(List<int> array, int low, int high, SortOrder order,
            ref int iterations, CancellationToken token)
        {
            int pivot = array[high];
            int i = low - 1;

            for (int j = low; j < high; j++)
            {
                token.ThrowIfCancellationRequested();
                iterations++;  // Сравнение с pivot

                bool shouldSwap = order == SortOrder.Ascending
                    ? array[j] < pivot
                    : array[j] > pivot;

                if (shouldSwap)
                {
                    i++;
                    int temp = array[i];
                    array[i] = array[j];
                    array[j] = temp;
                }
            }

            int temp1 = array[i + 1];
            array[i + 1] = array[high];
            array[high] = temp1;

            return i + 1;
        }

        private List<int> BogoSort(List<int> array, SortOrder order, ref int iterations,
            CancellationToken token, int maxIterations)
        {
            Random rand = new Random();
            int attempts = 0;

            while (!IsSorted(array, order))
            {
                token.ThrowIfCancellationRequested();
                attempts++;

                if (attempts > maxIterations)
                {
                    // Для Bogo сортировки итерациями считаем попытки перемешивания
                    iterations = attempts;
                    throw new BogoSortTimeoutException();
                }

                // Перемешиваем массив
                for (int i = 0; i < array.Count; i++)
                {
                    int randomIndex = rand.Next(i, array.Count);
                    int temp = array[i];
                    array[i] = array[randomIndex];
                    array[randomIndex] = temp;
                }
            }

            // Для Bogo сортировки итерациями считаем попытки перемешивания
            iterations = attempts;
            return array;
        }

        private bool IsSorted(List<int> array, SortOrder order)
        {
            for (int i = 0; i < array.Count - 1; i++)
            {
                if (order == SortOrder.Ascending && array[i] > array[i + 1])
                    return false;
                if (order == SortOrder.Descending && array[i] < array[i + 1])
                    return false;
            }
            return true;
        }
    }

    public class SortResult
    {
        public string AlgorithmName { get; set; }
        public TimeSpan ExecutionTime { get; set; }
        public int Iterations { get; set; }
        public List<int> SortedData { get; set; }
        public SortType AlgorithmType { get; set; }
        public bool BogoTimeout { get; set; } = false;
    }
}