using System.Linq.Expressions;
using System.Xml.Linq;
using System.Runtime.InteropServices;

namespace Lab1
{
    class Program
    { 
        static void ObjectSavingAndLoading()
        {
            string filename = "file.txt";
            UniformGrid grid = new UniformGrid(1.2, 2.15, 7);
            V1DataUGrid v = new V1DataUGrid("1", DateTime.Now, grid, V1Data.Field);
            if (v.Save(filename) == true)
            {
                Console.WriteLine("Сохраненный объект:\n" + v.ToLongString("F3"));
                V1DataUGrid v1 = new V1DataUGrid("2", DateTime.Now);
                if (V1DataUGrid.Load(filename, ref v1) == true)
                {
                    Console.WriteLine("Загруженный объект:\n" + v1.ToLongString("F3"));
                }
                else
                {
                    Console.WriteLine("Ошибка при загрузке объекта!");
            }
            }
            else
            {
                Console.WriteLine("Ошибка при сохранении объекта!");
            }
        }
        static void LinqQueriesChecking()
        {
            UniformGrid grid = new UniformGrid(0, 1.4, 6);
            UniformGrid grid1 = new UniformGrid(0, 1.4, 5);
            V1DataUGrid v = new V1DataUGrid("1", DateTime.Now, grid, V1Data.Field);
            V1DataUGrid v1 = new V1DataUGrid("2", DateTime.Now, grid1, V1Data.Field);
            V1DataList list = new V1DataList("3", DateTime.Now);
            V1DataList list1 = new V1DataList("4", DateTime.Now);
            V1DataUGrid v2 = new V1DataUGrid("5", DateTime.Now);
            V1DataNUGrid v3 = new V1DataNUGrid("6", DateTime.Now);
            list.AddDefaults(0, 1, 5, V1Data.Field);
            V1DataCollection collection = new V1DataCollection() {v, v1, list, list1, v2, v3};
            Console.WriteLine(collection.ToLongString("F3"));
            Console.WriteLine($"Среднее значение модулей поля всех элементов коллекции: {collection.MeanValue}\n");
            Console.WriteLine($"Максимально отклоняющаяся от среднего значения точка: {collection.MaxDeviationFromMean}\n");
            Console.WriteLine($"Координаты х точек, содержащихся хотя бы в двух элементах коллекции:");
            IEnumerable<double> nonUniquePoints = collection.NonUniquePoints;
            foreach(double elem in nonUniquePoints)
            {
                Console.WriteLine(elem);
            }
        }
        static void Main(string[] args)
        {     
            double[] nodes = { 1, 1.4, 2};
            double[] new_nodes = { 1, 1.1, 1.4, 1.6, 2};
            V1DataNUGrid grid = new V1DataNUGrid("1", DateTime.Now, nodes, V1Data.Field);
            double left_der = 3;
            double right_der = 12;
            V1DataNUGridSpline InterpolatingGrid = new V1DataNUGridSpline(grid, new_nodes, left_der, right_der, 1, 2);
            InterpolatingGrid.InterpolateGrid();
            Console.WriteLine(InterpolatingGrid.ToLongString("F3"));
            InterpolatingGrid.Save("file1.txt", "F4");

            double[] new_nodes1 = { 1, 1.4, 1.5, 2};
            V1DataNUGridSpline InterpolatingGrid1 = new V1DataNUGridSpline(grid, new_nodes1, left_der, right_der, 1, 2);
            V1DataNUGridSpline InterpolatingGrid2 = new V1DataNUGridSpline(grid, new_nodes1, left_der + 1, right_der, 1, 2);
            InterpolatingGrid1.InterpolateGrid();
            InterpolatingGrid2.InterpolateGrid();
            for(int i = 0; i < new_nodes1.Length; ++i)
            {
                Console.WriteLine($"x = {new_nodes1[i]} diff = {(InterpolatingGrid1.NewGridValues[i] - InterpolatingGrid2.NewGridValues[i]).ToString("F3")} " +
                    $"derivative diff = {(InterpolatingGrid1.NewGridDerValues[i] - InterpolatingGrid2.NewGridDerValues[i]).ToString("F3")}");
            }
            Console.WriteLine($"Integral diff = {(InterpolatingGrid1.Integral - InterpolatingGrid2.Integral).ToString("F3")}");
        }
    }
}