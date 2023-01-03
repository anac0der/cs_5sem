using System.Collections;
using System.Collections.ObjectModel;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace Lab1
{
    [Serializable]
    abstract class V1Data: IEnumerable<DataItem>
    {
        public static Complex Field(double x)
        {
            return new Complex(x * x * x, x * x * x);
        }
        public string ObjectID { get; set; }
        public DateTime date { get; set; }

        public V1Data(string ObjectID, DateTime date)
        {
            this.ObjectID = ObjectID;
            this.date = date;
        }

        public abstract double MaxMagnitude { get; }

        public abstract string ToLongString(string format);
        public override string ToString()
        {
            return $"ObjectID = {ObjectID}, date = {date.ToString()}";
        }

        public abstract IEnumerator<DataItem> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    class V1DataList: V1Data
    {
        public List<DataItem> data { get; set; }

        public V1DataList(string ObjectID, DateTime date): base(ObjectID, date)
        {
            this.data = new List<DataItem>();
        }

        public bool Add(double x, Complex field)
        {
            bool isInList = false;
            for(int i = 0; i < data.Count; ++i)
            {
                if (data[i].x == x)
                {
                    isInList = true;
                    break;
                }
            }
            if (isInList)
            {
                return false;
            }
            data.Add(new DataItem(x, field));
            return true;
        }

        public int AddDefaults(int a, int b, int nItems, FComplex F)
        {
            Random rnd = new Random();
            int cnt = 0;
            for (int i = 0; i < nItems; ++i)
            {
                double x = (b - a) * rnd.NextDouble() + a;
                if (this.Add(x, F(x)))
                {
                    ++cnt;
                }
            }
            return cnt;
        }

        public override double MaxMagnitude {
            get
            {
                double maxMagnitude = 0;
                for (int i = 0; i < data.Count; ++i)
                {
                    if (data[i].value.Magnitude > maxMagnitude)
                    {
                        maxMagnitude = data[i].value.Magnitude; 
                    }
                }
                return maxMagnitude;
            } 
        }

        public override string ToString()
        {
            return $"Тип объекта: V1DataList, ObjectID = {ObjectID}, date = {date}.\n";
        }

        public override string ToLongString(string format)
        {
            string pattern = $"Тип объекта: V1DataList, ObjectID = {ObjectID}, date = {date}.\n";

            for(int i = 0; i < data.Count; ++i)
            {
                pattern += $"x = {data[i].x.ToString(format)} value = {data[i].value.ToString(format)}\n";
            }
            return pattern;            
        }

        public static explicit operator V1DataNUGrid(V1DataList source)
        {
            double[] NuGridNodes = new double[source.data.Count];
            Complex[] NuGridValues = new Complex[source.data.Count];
            for (int i = 0; i < source.data.Count; ++i){
                NuGridNodes[i] = source.data[i].x;
                NuGridValues[i] = source.data[i].value;
            }
            return new V1DataNUGrid(source.ObjectID, source.date, NuGridNodes, NuGridValues);
        }

        public override IEnumerator<DataItem> GetEnumerator()
        {
            return data.GetEnumerator();
        }
    }

    class V1DataNUGrid: V1Data
    {
        public double[] NuGridNodes { get; set; }
        public Complex[] NuGridValues { get; set; }

        public V1DataNUGrid(string ObjectID, DateTime date): base(ObjectID, date)
        {
            this.NuGridNodes = new double[0];
            this.NuGridValues = new Complex[0];
        }

        public V1DataNUGrid(string ObjectID, DateTime date, double[] NuGridNodes, FComplex Field): base(ObjectID, date)
        {
            this.NuGridNodes = (double[])NuGridNodes.Clone();
            this.NuGridValues = new Complex[NuGridNodes.Length];
            for(int i = 0; i < NuGridNodes.Length; ++i)
            {
                NuGridValues[i] = Field(NuGridNodes[i]);
            }
        }

        public V1DataNUGrid(string ObjectID, DateTime date, double[] NuGridNodes, Complex[] FieldValues): base(ObjectID, date)
        {
            this.NuGridNodes = (double[])NuGridNodes.Clone();
            this.NuGridValues = new Complex[NuGridNodes.Length];
            for (int i = 0; i < NuGridNodes.Length; ++i)
            {
                NuGridValues[i] = FieldValues[i];
            }
        }

        public override double MaxMagnitude
        {
            get
            {
                double maxMagnitude = 0;
                for (int i = 0; i < NuGridValues.Length; ++i)
                {
                    if (NuGridValues[i].Magnitude > maxMagnitude)
                    {
                        maxMagnitude = NuGridValues[i].Magnitude;
                    }
                }
                return maxMagnitude;
            }
        }

        public override string ToString()
        {
            return $"Тип объекта: V1DataNUGrid, ObjectID = {ObjectID}, date = {date}.";
        }

        public override string ToLongString(string format)
        {
            string pattern = $"Тип объекта: V1DataNUGrid, ObjectID = {ObjectID}, date = {date}.\nИзмерения поля:\n";

            for (int i = 0; i < NuGridValues.Length; ++i)
            {
                pattern += $"x = {NuGridNodes[i].ToString(format)} value = {NuGridValues[i].ToString(format)}\n";
            }
            return pattern;
        }

        public override IEnumerator<DataItem> GetEnumerator()
        {
            List<DataItem> dataItems = new List<DataItem>();
            for (int i = 0; i < NuGridNodes.Length; ++i)
            {
                DataItem data = new DataItem(NuGridNodes[i], NuGridValues[i]);
                dataItems.Add(data);
            }
            return dataItems.GetEnumerator();
        }
    }

    [Serializable]
    class V1DataUGrid: V1Data
    {
        public UniformGrid grid { get; set; }
        public Complex[] UGridValues { get; set; }

        public V1DataUGrid(string ObjectID, DateTime date) : base(ObjectID, date)
        {
            this.UGridValues = new Complex[0];
        }

        public V1DataUGrid(string ObjectID, DateTime date, UniformGrid grid, FComplex Field) : base(ObjectID, date)
        {
            this.grid = grid;
            this.UGridValues = new Complex[grid.nodes];
            double node = grid.left;
            for (int i = 0; i < grid.nodes; ++i)
            {
                UGridValues[i] = Field(node);
                node += grid.MeshStep;
            }
        }

        public override double MaxMagnitude
        {
            get
            {
                double maxMagnitude = 0;
                for (int i = 0; i < UGridValues.Length; ++i)
                {
                    if (UGridValues[i].Magnitude > maxMagnitude)
                    {
                        maxMagnitude = UGridValues[i].Magnitude;
                    }
                }
                return maxMagnitude;
            }
        }

        public bool Save(string filename)
        {
            StreamWriter? file = null;
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                IncludeFields = true
            };
            try 
            {
                file = new StreamWriter(filename, false);
                string grid = JsonSerializer.Serialize<UniformGrid>(this.grid, options);
                double[] doubleArr = new double[this.UGridValues.Length * 2];
                for(int i = 0; i < this.UGridValues.Length ; ++i)
                {
                    doubleArr[2 * i] = this.UGridValues[i].Real;
                    doubleArr[2 * i + 1] = this.UGridValues[i].Imaginary;
                }
                string values = JsonSerializer.Serialize<double[]>(doubleArr, options);
                string id = JsonSerializer.Serialize<string>(this.ObjectID, options);
                string date = JsonSerializer.Serialize<DateTime>(this.date, options);
                if(grid == null || values == null || id == null || date == null)
                {
                    throw new Exception("Ошибка при сериализации полей объекта!");
                }
                file.WriteLine(id);
                file.WriteLine(date);
                file.WriteLine(grid);
                file.WriteLine(values);
            }
            catch (Exception ex) {
                Console.WriteLine($"Исключение: {ex.Message}");
                if (file != null) file.Close();
                return false;
            }
            if (file != null) file.Close();    
            return true;
        }

        public static bool Load(string filename, ref V1DataUGrid v1)
        {
            StreamReader? file = null;            
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                IncludeFields = true
            };
            try
            {
                file = new StreamReader(filename);
                string? jsonId = file.ReadLine();
                string? jsonDate = file.ReadLine();
                string? jsonGrid = file.ReadLine();
                string? jsonValues = file.ReadLine();
                if(jsonId == null || jsonDate == null || jsonGrid == null || jsonValues == null)
                {
                    throw new Exception("Ошибка чтения из файла при десериализации объекта!");
                }
                v1.ObjectID = JsonSerializer.Deserialize<string>(jsonId, options);
                v1.date = JsonSerializer.Deserialize<DateTime>(jsonDate, options);
                v1.grid = JsonSerializer.Deserialize<UniformGrid>(jsonGrid, options);
                double[] doubleArr = JsonSerializer.Deserialize<double[]>(jsonValues, options);
                Complex[] UGValues = new Complex[doubleArr.Length / 2];
                for(int i = 0; i < doubleArr.Length / 2; ++i)
                {
                    UGValues[i] = new Complex(doubleArr[2 * i], doubleArr[2 * i + 1]);
                }
                v1.UGridValues = UGValues;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Исключение: {ex.Message}");
                if (file != null) file.Close();
                return false;
            }
            if (file != null) file.Close();
            return true;
        }

        public override string ToString()
        {
            return $"Тип объекта: V1DataUGrid, ObjectID = {ObjectID}, date = {date}.";
        }

        public override string ToLongString(string format)
        {
            string pattern = $"Тип объекта: V1DataUGrid, ObjectID = {ObjectID}, date = {date}.\nИзмерения поля:\n";
            double node = grid.left;
            for(int i = 0; i < grid.nodes; ++i)
            {
                pattern += $"x = {node.ToString(format)} value = {UGridValues[i].ToString(format)}\n";
                node += grid.MeshStep;
            }
            return pattern;
        }
        public override IEnumerator<DataItem> GetEnumerator()
        {
            List<DataItem> dataItems = new List<DataItem>();
            for (int i = 0; i < grid.nodes; ++i)
            {
                DataItem data = new DataItem(grid.left + i * grid.MeshStep, UGridValues[i]);
                dataItems.Add(data);
            }
            return dataItems.GetEnumerator();
        }
    }

    class V1DataCollection: ObservableCollection<V1Data>
    {

        public double MeanValue
        {
            get
            {
                IEnumerable<double> dataItems = from item in this 
                                                from items in item 
                                                select items.value.Magnitude;
                if(dataItems != null)
                {
                    return dataItems.Average();
                }
                return double.NaN;                
            }
        }
        public DataItem MaxDeviationFromMean
        {
            get
            {
                double mean = this.MeanValue;
                IEnumerable<double> abs = from item in this 
                                          from items in item 
                                          select Math.Abs(items.value.Magnitude - mean);
                double maxDeviation = abs.Max();
                IEnumerable<DataItem> Items = from item in this 
                                              from items in item 
                                              where Math.Abs(items.value.Magnitude - mean) == maxDeviation 
                                              select items;
                return Items.FirstOrDefault();                             
            }
        }

        public IEnumerable <double> NonUniquePoints
        {
            get
            {
                IEnumerable<DataItem> allx = from item in this 
                                             from items in item 
                                             select items;
                allx = allx.Distinct();
                IEnumerable<double> allxFinal = from item1 in this 
                                                from item2 in this 
                                                from elem in allx 
                                                where item1.Contains(elem) && item2.Contains(elem) && item1 != item2 
                                                select elem.x;
                return allxFinal.Distinct();
            }
        }

        public new V1Data this[int index]
        {
            get => this[index];
        } 

        public bool Contains(string ID)
        {
            foreach(V1Data elem in this)
            {
                if(elem.ObjectID == ID)
                {
                    return true;
                }
            }
            return false;
        }

        public new bool Add(V1Data v1Data)
        {
            if (this.Contains(v1Data.ObjectID))
            {
                return false;
            }
            base.Add(v1Data);
            return true;
        }

        public void AddDefaults(int left = 0, int right = 1)
        {
            int objectsToAdd = 1;
            int minNodes = 3;
            int maxNodes = 5;
            int maxID = 100000;
            FComplex Field = V1Data.Field;
            for (int i = 0; i < objectsToAdd; ++i) {
                Random rnd = new Random();
                int id1 = rnd.Next(1, maxID);
                int id2 = rnd.Next(1, maxID);
                int id3 = rnd.Next(1, maxID);
                V1DataList v1DataList = new V1DataList(id1.ToString(), DateTime.Now);
                v1DataList.AddDefaults(left, right, rnd.Next(minNodes, maxNodes), Field);
                int nodes = rnd.Next(minNodes, maxNodes);
                V1DataUGrid v1DataUGrid = new V1DataUGrid(id2.ToString(), DateTime.Now, new UniformGrid(left, right, nodes), Field);
                double[] NuGridValues = new double[rnd.Next(minNodes, maxNodes)];
                for(int j = 0; j < NuGridValues.Length; ++j)
                {
                    NuGridValues[j] = (right - left) * rnd.NextDouble() + left;
                }
                //Array.Sort(NuGridValues);
                V1DataNUGrid v1DataNUGrid = new V1DataNUGrid(id3.ToString(), DateTime.Now, NuGridValues, Field);
                this.Add(v1DataList);
                this.Add(v1DataUGrid);
                this.Add(v1DataNUGrid);
            }
        }

        public string ToLongString(string format)
        {
            string pattern = $"Информация о коллекции:\n";
            foreach(V1Data elem in this)
            {
                pattern += elem.ToLongString(format);
            }
            return pattern;
        }

        public override string ToString()
        {
            string pattern = $"Информация о коллекции:\n";
            foreach (V1Data elem in this)
            {
                pattern += elem.ToString();
            }
            return pattern;
        }
    }

    class V1DataNUGridSpline
    {
        public V1DataNUGrid? Grid{ get; private set; }
        public double LeftDer { get; private set; }
        public double RightDer { get; private set; }
        public double[]? NewGrid { get; private set; }
        public Complex[]? NewGridValues { get; private set; }
        public Complex[]? NewGridDerValues { get; private set; }
        public double LeftIntegralBound { get; private set; }
        public double RightIntegralBound { get; private set; }
        public Complex Integral { get; private set; }
        public int isError = 0;

        public V1DataNUGridSpline(V1DataNUGrid grid, double[] newGrid, double leftDer, double rightDer, double leftIntBound, double rightIntBound)
        {
            Grid = grid;
            NewGrid = newGrid;
            LeftDer = leftDer;
            RightDer = rightDer;
            LeftIntegralBound = leftIntBound;
            RightIntegralBound = rightIntBound;
            NewGridValues = new Complex[newGrid.Length];
            NewGridDerValues = new Complex[newGrid.Length];
        }

        public string ToLongString(string format)
        {
            string pattern = $"Информация об объекте V1DataNUGridSpline:\n";
            pattern += Grid.ToLongString(format);
            pattern += $"Первая производная на левом конце отрезка: {LeftDer}\n";
            pattern += $"Первая производная на правом конце отрезка: {RightDer}\n";
            pattern += $"Отрезок интегрирования: [{LeftIntegralBound}, {RightIntegralBound}]\n";
            pattern += $"Значение интеграла: {Integral.ToString(format)}\n";
            for(int i = 0; i < NewGridValues.Length; ++i) 
            {
                pattern += $"x = {NewGrid[i].ToString(format)} value = {NewGridValues[i].ToString(format)} derivative = {NewGridDerValues[i].ToString(format)}\n";
            }
            return pattern;
        }

        public void Save(string filename, string format)
        {
            StreamWriter file = new StreamWriter(filename, false);
            file.Write(ToLongString(format));
            file.Close();
        }

        public void InterpolateGrid()
        {
            double[] values = new double[2 * Grid.NuGridValues.Length];
            for (int i = 0; i < Grid.NuGridValues.Length; ++i)
            {
                values[i] = Grid.NuGridValues[i].Real;
                values[i + Grid.NuGridValues.Length] = Grid.NuGridValues[i].Imaginary;
            }
            double[] derivatives = { LeftDer, RightDer };
            double[] result = new double[4 * NewGrid.Length + 2];
            int status = spline_interpolation(Grid.NuGridNodes.Length, result, Grid.NuGridNodes, values, derivatives, NewGrid.Length, NewGrid, LeftIntegralBound, RightIntegralBound);
            if (status == 0)
            {
                Integral = new Complex(result[4 * NewGrid.Length], result[4 * NewGrid.Length + 1]);
                for (int i = 0; i < NewGrid.Length; ++i)
                {
                    NewGridValues[i] = new Complex(result[2 * i], result[2 * i + 2 * NewGrid.Length]);
                    NewGridDerValues[i] = new Complex(result[2 * i + 1], result[2 * i + 1 + 2 * NewGrid.Length]);
                }
            }
            else
            {
                isError = status;
            }     
        }

        [DllImport(@"Dll1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int spline_interpolation(int length, double[] res_arr, double[] x, double[] y, double[] derivatives, int new_length, double[] new_x, double a, double b);
    }
}