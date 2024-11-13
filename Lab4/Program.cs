using ScottPlot;
using Lab4;
using System.Diagnostics;
using System.Numerics;
using System;

var kdTree = new KDTree(2);
while (true)
{
    Console.WriteLine("1 - Add point , 2 - Delete point, 3 - Find the nearest neighbour");
    int option;
    if (int.TryParse(Console.ReadLine(), out option))
    {
        double x, y;
        var plt = new ScottPlot.Plot();
        switch (option)
        {
            case 1:
                Console.WriteLine("Write x: ");
                x = double.Parse(Console.ReadLine());
                Console.WriteLine("Write y: ");
                y = double.Parse(Console.ReadLine());
                kdTree.Insert(new double[] { x, y });
                break;
            case 2:
                Console.WriteLine("Write x: ");
                x = double.Parse(Console.ReadLine());
                Console.WriteLine("Write y: ");
                y = double.Parse(Console.ReadLine());
                kdTree.Delete(new double[] { x, y });
                break;
            case 3:
                Console.WriteLine("Write x: ");
                x = double.Parse(Console.ReadLine());
                Console.WriteLine("Write y: ");
                y = double.Parse(Console.ReadLine());
                double[] target = new double[] { 9, 2 };
                var nearest = kdTree.FindNearest(target);
                plt.Add.Marker(target[0], target[1], shape: MarkerShape.FilledCircle, size: 10, ScottPlot.Color.FromColor(System.Drawing.Color.Red));
                plt.Add.Marker(nearest.Point[0], nearest.Point[1], shape: MarkerShape.FilledCircle, size: 10, ScottPlot.Color.FromColor(System.Drawing.Color.Green));
                break;
        }
        var points = kdTree.GetAllPoints();
        Visualize(kdTree, points);
        //double[] xs = new double[points.Count];
        //double[] ys = new double[points.Count];
        //for (int i = 0; i < points.Count; i++)
        //{
        //    plt.Add.Marker(points[i][0], points[i][1]);
        //}

        
        //plt.SavePng(@"D:\\source\\repos\\CSharp\\University\\DiscreteStructures\\KDTree.png", 600, 400);
        OpenImage(@"D:\\source\\repos\\CSharp\\University\\DiscreteStructures\\KDTree.png");
    }

}
static void OpenImage(string filePath)
{
    try
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = filePath,
            UseShellExecute = true
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error opening image: " + ex.Message);
    }
}

static void Visualize(KDTree kdTree, List<double[]> points)
{
    var plt = new ScottPlot.Plot();
    List<double> xPoints = new List<double>();
    List<double> yPoints = new List<double>();

    foreach (var point in points)
    {
        xPoints.Add(point[0]);
        yPoints.Add(point[1]);
    }

    var line = plt.Add.Scatter(xPoints.ToArray(), yPoints.ToArray());
    line.LineWidth = 0;
    line.MarkerSize = 5;
    DrawMedians(kdTree.root, plt, 0, 0, 0, 10, 10);
    // Налаштування осей та відображення
    plt.XLabel("X");
    plt.YLabel("Y");
    plt.Title("2D KdTree Visualization");

    // Відображення графіку
    plt.SavePng(@"D:\\source\\repos\\CSharp\\University\\DiscreteStructures\\KDTree.png", 600, 400);

}
static void DrawMedians(KDTree.Node node, Plot plt, int depth, double minX, double minY, double maxX, double maxY)
{
    if (node == null || (node.Left == null && node.Right == null))
        return;

    int cd = depth % 2;  // Ось для поділу (X або Y)

    if (cd == 0) // Розподіл по осі X
    {
        double median = node.Point[0];
        var line = plt.Add.VerticalLine(median, color: ScottPlot.Color.FromColor(System.Drawing.Color.Red));
        line.LineWidth = 2;// Малюємо вертикальну лінію
        DrawMedians(node.Left, plt, depth + 1, minX, minY, median, maxY);  // Ліва частина
        DrawMedians(node.Right, plt, depth + 1, median, minY, maxX, maxY); // Права частина
    }
    else // Розподіл по осі Y
    {
        double median = node.Point[1];
        var line = plt.Add.HorizontalLine(median, color: ScottPlot.Color.FromColor(System.Drawing.Color.Blue)); // Малюємо горизонтальну лінію
        line.LineWidth = 2;
        DrawMedians(node.Left, plt, depth + 1, minX, minY, maxX, median);  // Ліва частина
        DrawMedians(node.Right, plt, depth + 1, minX, median, maxX, maxY); // Права частина
    }
}
static void VisualizePartitionLines(KDTree kdTree, Plot plt, double xMin, double xMax, double yMin, double yMax, int depth)
{
    VisualizePartitionLinesRec(kdTree.root, depth, plt, xMin, xMax, yMin, yMax);
}
// Рекурсивна функція для побудови ліній
static void VisualizePartitionLinesRec(KDTree.Node node, int depth, Plot plt, double xMin, double xMax, double yMin, double yMax)
{
    if (node == null)
        return;

    int cd = depth % 2; // Визначення осі для поділу (0 - вертикальна, 1 - горизонтальна)

    if (cd == 0) // Якщо вертикальна лінія
    {
        // Малюємо вертикальну лінію
        plt.Add.VerticalLine(node.Point[0], color: ScottPlot.Color.FromColor(System.Drawing.Color.Red));
        // Рекурсивно ділимо простір на ліву та праву частину
        VisualizePartitionLinesRec(node.Left, depth + 1, plt, xMin, node.Point[0], yMin, yMax);
        VisualizePartitionLinesRec(node.Right, depth + 1, plt, node.Point[0], xMax, yMin, yMax);
    }
    else // Якщо горизонтальна лінія
    {
        // Малюємо горизонтальну лінію
        plt.Add.HorizontalLine(node.Point[1], color: ScottPlot.Color.FromColor(System.Drawing.Color.Blue));
        // Рекурсивно ділимо простір на верхню та нижню частину
        VisualizePartitionLinesRec(node.Left, depth + 1, plt, xMin, xMax, yMin, node.Point[1]);
        VisualizePartitionLinesRec(node.Right, depth + 1, plt, xMin, xMax, node.Point[1], yMax);
    }
}