using System.Linq.Expressions;
using System.Xml.Linq;

namespace Lab1
{
    class Program
    { 
        static void ObjectSavingAndLoading()
        {
            string filename = "file.txt";
            UniformGrid grid = new UniformGrid(1.2, 2.15, 7);
            V1DataUGrid v = new V1DataUGrid("1", DateTime.Now, grid, V1Data.Field);
            v.Save(filename);
            Console.WriteLine("Сохраненный объект:\n" + v.ToLongString("F3"));
            V1DataUGrid v1 = new V1DataUGrid("2", DateTime.Now);
            V1DataUGrid.Load(filename, ref v1);
            Console.WriteLine("Загруженный объект:\n" + v1.ToLongString("F3"));
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
            V1DataUGrid v3 = new V1DataUGrid("6", DateTime.Now);
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
            Console.WriteLine(Directory.GetCurrentDirectory());
            ObjectSavingAndLoading();
            LinqQueriesChecking();
        }
    }
}

