using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProgramLab
{
    public partial class MNK : Form
    {
        private List<PointD> points = new List<PointD>();
        private double[] linearCoefficients;
        private double[] quadraticCoefficients;
        private double linearMSE;
        private double quadraticMSE;
        private bool useLinearModel = true;

        public MNK()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView.Rows.Add(10);
            UpdatePlot();

            // Установка значений по умолчанию
            numPoints.Value = 10;
            minXValue.Value = 0;
            maxXValue.Value = 10;
            minYValue.Value = -2;
            maxYValue.Value = 2;
            linearRadio.Checked = true;
        }

        private double[] LeastSquaresLinear(List<PointD> points)
        {
            int n = points.Count;
            if (n < 2) return new double[] { 0, 0 };

            double sumX = 0, sumY = 0, sumXY = 0, sumX2 = 0;

            foreach (var p in points)
            {
                sumX += p.X;
                sumY += p.Y;
                sumXY += p.X * p.Y;
                sumX2 += p.X * p.X;
            }

            double denominator = n * sumX2 - sumX * sumX;
            if (Math.Abs(denominator) < 1e-10) return new double[] { 0, 0 };

            double a = (n * sumXY - sumX * sumY) / denominator;
            double b = (sumY - a * sumX) / n;

            return new double[] { b, a };
        }

        private double[] LeastSquaresQuadratic(List<PointD> points)
        {
            int n = points.Count;
            if (n < 3) return new double[] { 0, 0, 0 };

            double sumX = 0, sumY = 0, sumX2 = 0, sumX3 = 0, sumX4 = 0, sumXY = 0, sumX2Y = 0;

            foreach (var p in points)
            {
                double x = p.X;
                double y = p.Y;
                double x2 = x * x;
                double x3 = x2 * x;
                double x4 = x3 * x;

                sumX += x;
                sumY += y;
                sumX2 += x2;
                sumX3 += x3;
                sumX4 += x4;
                sumXY += x * y;
                sumX2Y += x2 * y;
            }

            double[,] matrix = {
                { n, sumX, sumX2 },
                { sumX, sumX2, sumX3 },
                { sumX2, sumX3, sumX4 }
            };

            double[] constants = { sumY, sumXY, sumX2Y };

            return SolveLinearSystem(matrix, constants);
        }

        private double[] SolveLinearSystem(double[,] matrix, double[] constants)
        {
            double det = matrix[0, 0] * (matrix[1, 1] * matrix[2, 2] - matrix[1, 2] * matrix[2, 1])
                       - matrix[0, 1] * (matrix[1, 0] * matrix[2, 2] - matrix[1, 2] * matrix[2, 0])
                       + matrix[0, 2] * (matrix[1, 0] * matrix[2, 1] - matrix[1, 1] * matrix[2, 0]);

            if (Math.Abs(det) < 1e-10) return new double[] { 0, 0, 0 };

            double[] result = new double[3];

            for (int i = 0; i < 3; i++)
            {
                double[,] temp = (double[,])matrix.Clone();
                for (int j = 0; j < 3; j++)
                {
                    temp[j, i] = constants[j];
                }

                double tempDet = temp[0, 0] * (temp[1, 1] * temp[2, 2] - temp[1, 2] * temp[2, 1])
                               - temp[0, 1] * (temp[1, 0] * temp[2, 2] - temp[1, 2] * temp[2, 0])
                               + temp[0, 2] * (temp[1, 0] * temp[2, 1] - temp[1, 1] * temp[2, 0]);

                result[i] = tempDet / det;
            }

            return result;
        }

        private double CalculateMSE(List<PointD> points, double[] coefficients)
        {
            if (points.Count == 0 || coefficients == null) return 0;

            double sum = 0;
            foreach (var p in points)
            {
                double predicted = 0;
                for (int i = 0; i < coefficients.Length; i++)
                {
                    predicted += coefficients[i] * Math.Pow(p.X, i);
                }
                sum += Math.Pow(p.Y - predicted, 2);
            }
            return Math.Sqrt(sum / points.Count);
        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            try
            {
                CollectPointsFromGrid();

                if (points.Count < 2)
                {
                    MessageBox.Show("Недостаточно точек для расчета. Введите минимум 2 точки.",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                linearCoefficients = LeastSquaresLinear(points);
                linearMSE = CalculateMSE(points, linearCoefficients);

                if (points.Count >= 3)
                {
                    quadraticCoefficients = LeastSquaresQuadratic(points);
                    quadraticMSE = CalculateMSE(points, quadraticCoefficients);
                }
                else
                {
                    quadraticCoefficients = new double[] { 0, 0, 0 };
                    quadraticMSE = 0;
                }

                DisplayResults();
                UpdatePlot();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при расчете: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CollectPointsFromGrid()
        {
            points.Clear();

            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.IsNewRow) continue;

                if (row.Cells[0].Value != null && row.Cells[1].Value != null)
                {
                    if (double.TryParse(row.Cells[0].Value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out double x) &&
                        double.TryParse(row.Cells[1].Value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out double y))
                    {
                        points.Add(new PointD(x, y));
                    }
                }
            }
        }

        private void DisplayResults()
        {
            if (linearCoefficients != null && linearCoefficients.Length >= 2)
            {
                txtLinear.Text = $"f(x) = {linearCoefficients[0]:F4} + {linearCoefficients[1]:F4}x";
                txtLinearError.Text = $"{linearMSE:F6}";
            }

            if (quadraticCoefficients != null && quadraticCoefficients.Length >= 3)
            {
                txtQuadratic.Text = $"f(x) = {quadraticCoefficients[0]:F4} + {quadraticCoefficients[1]:F4}x + {quadraticCoefficients[2]:F4}x²";
                txtQuadraticError.Text = $"{quadraticMSE:F6}";
            }
        }

        private void UpdatePlot()
        {
            if (pictureBox.Width <= 0 || pictureBox.Height <= 0) return;

            Bitmap bmp = new Bitmap(pictureBox.Width, pictureBox.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
                g.SmoothingMode = SmoothingMode.AntiAlias;

                if (points.Count == 0)
                {
                    g.DrawString("Введите данные для построения графика",
                        new Font("Arial", 10), Brushes.Gray,
                        pictureBox.Width / 2 - 150, pictureBox.Height / 2 - 10);
                    pictureBox.Image = bmp;
                    return;
                }

                double minX = points.Min(p => p.X);
                double maxX = points.Max(p => p.X);
                double minY = points.Min(p => p.Y);
                double maxY = points.Max(p => p.Y);

                double rangeX = maxX - minX;
                double rangeY = maxY - minY;
                minX -= rangeX * 0.1;
                maxX += rangeX * 0.1;
                minY -= rangeY * 0.1;
                maxY += rangeY * 0.1;

                if (Math.Abs(rangeX) < 0.001) { minX -= 1; maxX += 1; }
                if (Math.Abs(rangeY) < 0.001) { minY -= 1; maxY += 1; }

                int padding = 40;
                int plotWidth = bmp.Width - 2 * padding;
                int plotHeight = bmp.Height - 2 * padding;

                Pen axisPen = new Pen(Color.Gray, 1);
                g.DrawLine(axisPen, padding, bmp.Height - padding, bmp.Width - padding, bmp.Height - padding);
                g.DrawLine(axisPen, padding, padding, padding, bmp.Height - padding);

                Func<double, double, PointF> transform = (x, y) =>
                {
                    float screenX = padding + (float)((x - minX) / (maxX - minX) * plotWidth);
                    float screenY = bmp.Height - padding - (float)((y - minY) / (maxY - minY) * plotHeight);
                    return new PointF(screenX, screenY);
                };

                if (linearCoefficients != null && points.Count > 0)
                {
                    PointF[] linearPoints = new PointF[plotWidth];
                    for (int i = 0; i < plotWidth; i++)
                    {
                        double x = minX + (maxX - minX) * i / (plotWidth - 1);
                        double y = linearCoefficients[0] + linearCoefficients[1] * x;
                        linearPoints[i] = transform(x, y);
                    }
                    g.DrawLines(new Pen(Color.Blue, 2), linearPoints);
                }

                if (quadraticCoefficients != null && points.Count >= 3)
                {
                    PointF[] quadraticPoints = new PointF[plotWidth];
                    for (int i = 0; i < plotWidth; i++)
                    {
                        double x = minX + (maxX - minX) * i / (plotWidth - 1);
                        double y = quadraticCoefficients[0] + quadraticCoefficients[1] * x + quadraticCoefficients[2] * x * x;
                        quadraticPoints[i] = transform(x, y);
                    }
                    g.DrawLines(new Pen(Color.Red, 2), quadraticPoints);
                }

                foreach (var point in points)
                {
                    PointF screenPoint = transform(point.X, point.Y);
                    g.FillEllipse(Brushes.Green, screenPoint.X - 3, screenPoint.Y - 3, 6, 6);
                    g.DrawEllipse(Pens.DarkGreen, screenPoint.X - 3, screenPoint.Y - 3, 6, 6);
                }
            }

            pictureBox.Image = bmp;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            dataGridView.Rows.Clear();
            dataGridView.Rows.Add(10);
            points.Clear();
            txtLinear.Clear();
            txtQuadratic.Clear();
            txtLinearError.Clear();
            txtQuadraticError.Clear();
            UpdatePlot();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            GenerateData((int)numPoints.Value,
                        (double)minXValue.Value, (double)maxXValue.Value,
                        (double)minYValue.Value, (double)maxYValue.Value,
                        linearRadio.Checked);
        }

        private void GenerateData(int count, double minX, double maxX, double minNoiseY, double maxNoiseY, bool isLinear)
        {
            Random rand = new Random();
            dataGridView.Rows.Clear();

            for (int i = 0; i < count; i++)
            {
                double x = minX + (maxX - minX) * i / Math.Max(1, count - 1);
                double noise = minNoiseY + (maxNoiseY - minNoiseY) * (rand.NextDouble() - 0.5) * 2;

                double y;
                if (isLinear)
                {
                    y = 2 * x + 3 + noise;
                }
                else
                {
                    y = 1.5 * x * x - 2 * x + 5 + noise;
                }

                dataGridView.Rows.Add(x.ToString("F2", CultureInfo.InvariantCulture),
                                      y.ToString("F2", CultureInfo.InvariantCulture));
            }

            CollectPointsFromGrid();
            UpdatePlot();
        }

        private void btnLoadExcel_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Filter = "CSV files (*.csv)|*.csv|Text files (*.txt)|*.txt|All files (*.*)|*.*";
                    ofd.Title = "Загрузить данные из файла";

                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        LoadDataFromTextFile(ofd.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadDataFromTextFile(string filePath)
        {
            dataGridView.Rows.Clear();
            int rowCount = 0;

            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split(new char[] { ',', ';', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length >= 2)
                    {
                        if (double.TryParse(parts[0].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out double x) &&
                            double.TryParse(parts[1].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out double y))
                        {
                            dataGridView.Rows.Add(
                                x.ToString(CultureInfo.InvariantCulture),
                                y.ToString(CultureInfo.InvariantCulture));
                            rowCount++;
                        }
                    }
                }
            }

            CollectPointsFromGrid();
            UpdatePlot();
            MessageBox.Show($"Загружено {rowCount} точек из файла.",
                "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void btnLoadGoogle_Click(object sender, EventArgs e)
        {
            try
            {
                string spreadsheetId = "1ctZQF6sIWvhUlb8gixyglYd9qtBOGdXSvK4ghFF0dq0";
                await LoadFromGoogleSheets(spreadsheetId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadFromGoogleSheets(string spreadsheetId)
        {
            // Имитация загрузки из Google Sheets
            await Task.Delay(1000);

            Random rand = new Random();
            dataGridView.Rows.Clear();

            // Создаем тестовые данные
            for (int i = 0; i < 10; i++)
            {
                double x = i + 1;
                double y = 2 * x + 3 + (rand.NextDouble() - 0.5) * 4;
                dataGridView.Rows.Add(x.ToString("F2", CultureInfo.InvariantCulture),
                                      y.ToString("F2", CultureInfo.InvariantCulture));
            }

            CollectPointsFromGrid();
            UpdatePlot();
            MessageBox.Show("Загружено тестовых данных из Google Sheets",
                "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void dataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                string value = e.FormattedValue.ToString();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    if (!double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                    {
                        dataGridView.Rows[e.RowIndex].ErrorText = "Введите число";
                        e.Cancel = true;
                    }
                }
            }
        }

        private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                dataGridView.Rows[e.RowIndex].ErrorText = string.Empty;
                CollectPointsFromGrid();
                UpdatePlot();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Main main = new Main();
            main.Show();
            Close();
        }
    }

    public class PointD
    {
        public double X { get; set; }
        public double Y { get; set; }

        public PointD(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}