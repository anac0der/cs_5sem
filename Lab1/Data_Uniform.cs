using System.Numerics;


namespace Lab1
{
    delegate Complex FComplex(double x);
    [Serializable]
    struct DataItem
    {
        public double x { get; set; }
        public Complex value { get; set; }

        public DataItem(double x, Complex value)
        {
            this.x = x;
            this.value = value;
        }

        public string ToLongString(string format)
        {
            return "x = " + x.ToString(format) + " value = " + value.ToString(format);
        }

        public override string ToString()
        {
            return $"x = {x} value = {value}";
        }
    }

    [Serializable]
    struct UniformGrid
    {
        public double left;
        public double right;
        public int nodes;

        public UniformGrid(double left, double right, int nodes)
        {
            this.left = left;
            this.right = right;
            this.nodes = nodes;
        }

        public double MeshStep
        {
            get => (right - left) / (nodes - 1);
        }

        public override string ToString()
        {
            return $"Отрезок: [{left}:{right}], число узлов: {nodes}";
        }

        public string ToLongString(string format)
        {
            return $"Отрезок: [{right.ToString(format)}:{left.ToString(format)}], число узлов: {nodes}";
        }
    }    
}
