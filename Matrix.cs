using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace Lab5
{
    class Matrix : ICloneable, IFormattable, IEnumerable
    {
        public int id { get; private set; }
        public int row { get; private set; }
        public int column { get; private set; }
        private double[] Ex;
        private static int num = 0;

        object ICloneable.Clone()
        {
            return new Matrix(this);
        }

        public Matrix Clone()
        {
            return new Matrix(this);
        }

        public IEnumerator GetEnumerator()
        {
            return Ex.GetEnumerator();
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            int k = 0;
            StringBuilder sb = new StringBuilder();

            if (String.IsNullOrEmpty(format)) format = "G";
            if (formatProvider == null) formatProvider = CultureInfo.CurrentCulture;

            string[] ff = format.Split(',');

            if (ff.Length == 2) format = String.Format("{{0,{0}:{1}}}", ff[1], ff[0]);
            else format = String.Format("{{0,2:{0}}} ", ff[0]);

            for (int h = 0; h < row; h++, sb.Append("\n"))
            {
                for (int j = 0; j < column; j++)
                    sb.Append(String.Format(formatProvider, format, this.Ex[k++]));
            }
            return sb.ToString();
        }

        public Matrix()
        {
            row = column = 0;
            Ex = null;
            id = ++num;
        }

        public delegate double Init(int i, int j);

        public Matrix(int r, int c, Init init) : this(r, c)
        {
            for (int i = 0; i < r; i++)
            {
                for (int j = 0; j < c; j++) this[i, j] = init(i, j);
            }
        }

        public Matrix(Matrix M): this (M.row, M.column, M.Ex) {}

        public Matrix(int r, int c, params double[] matr) : this()
        {
            if (r >= 0 && c >= 0)
            {
                row = r;
                column = c;
                if (row != 0 && column != 0)
                {
                    Ex = new double[row * column];
                    if (matr != null)
                        for (int k = 0; k < row * column; k++)
                            if (k < matr.Length)
                                Ex[k] = matr[k];
                }
            }
            else throw new ArgumentOutOfRangeException(String.Format("Invalid value for the size of the matrix {0}", id));
        }

        public Matrix(double[,] mass) : this(mass.GetLength(0), mass.GetLength(1)) 
        {
            int i = 0;
            for (int k = 0; k < row; k++)
                for (int j = 0; j < column; j++)
                    Ex[i++] = mass[k, j];
        }

        ~Matrix() {}

        public double Norm()
        {
            double n = 0.0;
            foreach (double x in Ex) n += x * x;
            return Math.Sqrt(n);
        }

        public static bool sumControl(Matrix M, Matrix N)
        {
            return ((M.column == N.column) && (M.row == N.row));
        }

        public static bool multControl(Matrix M, Matrix N)
        {
            return (M.column == N.row);
        }

        public double Maximum()
        {
            if (row == 0 || column == 0)  throw new InvalidOperationException(String.Format("Matrix {0} is empty", id));
            return Ex.Max();
        }

        public double Minimum()
        {
            if (row == 0 || column == 0)  throw new InvalidOperationException(String.Format("Matrix {0} is empty", id));
            return Ex.Min();
            
        }

        public static Matrix operator+ (Matrix M, Matrix N)
        {
            if (sumControl(M, N))
            {
                Matrix mtr = M.Clone();
                for (int k = 0; k < M.row * M.column; k++)
                    mtr.Ex[k] += N.Ex[k];
                return mtr;
            }
            else throw new ArgumentException(String.Format("Matrix sizes {0} and {1} don't equal. Addition can't be executed", M.id, N.id));
        }

        public static Matrix operator- (Matrix M, Matrix N)
        {
            if (sumControl(M, N))
            {
                Matrix mtr = M.Clone();
                for (int k = 0; k < M.row * M.column; k++)
                    mtr.Ex[k] -= N.Ex[k];
                return mtr;
            }
            else throw new ArgumentException(String.Format("Matrix sizes {0} and {1} don't equal. Difference can't be executed", M.id, N.id));
        }

        public static Matrix operator *(Matrix M, Matrix N)
        {
            if (multControl(M, N))
            {
                Matrix temp = new Matrix(M.row, N.column);
                double s;
                for (int i = 0; i < M.row; i++)
                    for (int j = 0; j < N.column; j++)
                    {
                        s = 0;
                        for (int l = 0; l < M.column; l++)
                            s += M.Ex[i * M.column + l] * N.Ex[j + l * N.column];
                        temp.Ex[i * N.column + j] = s;
                    }
                    return temp;
            }
            else throw new ArgumentException(String.Format("Matrix sizes {0} and {1} don't appropriate to multiply. Multiply can't be executed", M.id, N.id));
        }

        public static Matrix operator *(Matrix M, double k)
        {
            Matrix temp = M.Clone();
            for (int j = 0; j < M.row * M.column; j++)
                temp.Ex[j] *= k;
            return temp;
        }

        public static Matrix operator *(double k, Matrix M)
        {
            return (M * k);
        }

        public double this[int r, int c]
        {
            get
            {
                if((r < 0 || c < 0) || (r >= row || c >= column))
                    throw new IndexOutOfRangeException(String.Format("The index value of the matrix {0} is not valid!", id));
                return Ex[r * column + c];
            }
            set
            {
                if ((r < 0 || c < 0) || (r >= row || c >= column))
                    throw new IndexOutOfRangeException(String.Format("The index value of the matrix {0} is not valid!", id));
                Ex[r * column + c] = value;
            }
        }
    }

    class Vector : Matrix
    {
        public Vector() : base() { }
        public Vector(int n, params double[] el) : base(n, 1, el) { }
        public Vector(double[] el) : this(el.Length, el) { }
        public Vector(Vector v) : base(v) { }
        public Vector(Matrix m) : base(m) { if (column > 1) throw new RankException(String.Format("assign matrix to vector {0}", id)); }
        public new Vector Clone()
        {
            return new Vector(this);
        }
        public double this[int i]
        {
            get { return base[i, 0]; }
            set { base[i, 0] = value; }
        }
    }
}