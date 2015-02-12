using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using PolStrLib;

namespace Lab5
{
	class Program
	{
		static void Main(string[] args)
		{
			StreamReader FileIn = new StreamReader("Input.txt");
			int procedure = int.Parse(FileIn.ReadLine());
			int degree = int.Parse(FileIn.ReadLine());
			bool GridIsUniform = bool.Parse(FileIn.ReadLine());
			string function, PolFunc = "";

			//Считывание значений сетки Х или концов интервала
			string[] str = FileIn.ReadLine().Split(' ');
			double start = 0.0;
			double end = 0.0;
			double[] X = new double[str.Length];
			if (GridIsUniform)
			{
				start = double.Parse(str[0]);
				end = double.Parse(str[1]);
			}

			else for (int i = 0; i < str.Length; i++) X[i] = double.Parse(str[i]);

			//Считывание значений сетки Y
			str = FileIn.ReadLine().Split(' ');
			double[] Y = new double[str.Length];
			for (int i = 0; i < str.Length; i++) Y[i] = double.Parse(str[i]);

			//Считывание результирующей сетки
			int m = int.Parse(FileIn.ReadLine());
			str = FileIn.ReadLine().Split(' ');
			double [] resNodes = new double[m+1];
            for (int i = 0; i < m+1; i++)	 resNodes[i] = double.Parse(str[i]);

			//Считывание функции
			bool FuncIsDepend = bool.Parse(FileIn.ReadLine());
			if (FuncIsDepend)
			{
				function = FileIn.ReadLine();
				PolStr.StrToPolStr(function, out PolFunc, 0);
			}
			FileIn.Close();

			double[] Differences = FindDifferences(X, Y, GridIsUniform, degree);	//Используется как для разделенных, так и для конечных разностей
			if (GridIsUniform)				//Если сетка равномерна
			{
				double h = (end-start)/degree;
				switch (procedure)
				{
					case 0: NewtonUniform1(Differences, resNodes, h, start, FuncIsDepend, PolFunc); break;
					case 1: NewtonUniform2(Differences, resNodes, h, start, FuncIsDepend, PolFunc); break;
					case 2: NewtonUniform3(Differences, resNodes, h, start, FuncIsDepend, PolFunc); break;
					default: break;
				}
			}
			else
			{
				switch (procedure)
				{
					case 0: NewtonNotUniform1(Differences, X, resNodes, FuncIsDepend, PolFunc); break;
					case 1: NewtonNotUniform2(Differences, X, resNodes, FuncIsDepend, PolFunc); break;
					case 2: NewtonNotUniform3(Differences, X, resNodes, FuncIsDepend, PolFunc); break;
					default: break;
				}
			}
		}

		public static double[] FindDifferences(double [] Xvalues, double [] Yvalues, bool GridIsUniform, int n)
		{
			double [][] Difference = new double[n+1][];			//Хранит разделенные/конечные разности разности
			double[] Factor = new double[n+1];					//Хранит будущие коэффициенты полинома

			for (int i = 0; i < n+1; i++)						//Рассчитываем разности
			{
				Difference[i] = new double[n + 1 - i];			//Выделение места под хранение разделенных/конечных разностей i-го порядка
				for (int j = 0; j < n + 1 - i; j++)
				{
					if (i == 0) Difference[i][j] = Yvalues[j];
					else Difference[i][j] = (Difference[i - 1][j + 1] - Difference[i - 1][j]) / (GridIsUniform == false ? (Xvalues[j + i] - Xvalues[j]) : 1);
				}
			}
			for (int i = 0; i < n + 1; i++)	Factor[i] = Difference[i][0];
			return Factor;
		}

		public static int fact(int k)
		{
			int res = 1;
			for (int i = 1; i <= k; i++)
				res *= i;
			return res;
		}

		public static void NewtonUniform1(double[] Diff, double[] resGrid, double h, double start, bool FuncIsDepend, string PolFunc)
		{
			double [] result = new double[resGrid.Length];
			double q, temp;
			
			for (int col = 0; col < resGrid.Length; col++)
			{
				result[col] = 0.0;
				q = (resGrid[col] - start) / h;
				for(int i = 0; i < Diff.Length; i++)
				{
					temp = 1.0;
					for(int j = 0; j < i; j++)	temp *= (q - j);
					result[col] += (temp * Diff[i] / fact(i));
				}
			}
			save(result, resGrid, FuncIsDepend, PolFunc, 0); //цифра в конце обозначает порядок производной, рассчитываемой в текущей функции
		}

		public static void NewtonUniform2(double[] Diff, double[] resGrid, double h, double start, bool FuncIsDepend, string PolFunc)
		{
			double[] result = new double[resGrid.Length];
			double q, temp, buf;

			for (int col = 0; col < resGrid.Length; col++)
			{
				result[col] = 0.0;
				q = (resGrid[col] - start) / h;
				for (int i = 0; i < Diff.Length; i++)
				{
					buf = 0;
					for (int k = 0; k < i; k++)
					{
						temp = 1.0;
						for (int j = 0; j < i; j++) if(j != k)	temp *= (q - j);
						buf += temp;
					}
					result[col] += buf * Diff[i] / fact(i);
				}
				result[col] /= h;
			}
			save(result, resGrid, FuncIsDepend, PolFunc, 1); //цифра в конце обозначает порядок производной, рассчитываемой в текущей функции
		}

		public static void NewtonUniform3(double[] Diff, double[] resGrid, double h, double start, bool FuncIsDepend, string PolFunc)
		{
			double[] result = new double[resGrid.Length];
			double q, temp, buf, buf2;

			for (int col = 0; col < resGrid.Length; col++)
			{
				result[col] = 0.0;
				q = (resGrid[col] - start) / h;
				for (int i = 0; i < Diff.Length; i++)
				{
					buf2 = 0;
					for (int l = 0; l < i; l++)
					{
						buf = 0;
						for (int k = 0; k < i; k++)
						{
							if (k != l)
							{
								temp = 1.0;
								for (int j = 0; j < i; j++) if (j != k && j != l) temp *= (q - j);
								buf += temp;
							}
						}
						buf2 += buf;
					}
					result[col] += buf2 * Diff[i] / fact(i);
				}
				result[col] /= (h*h);
			}
			save(result, resGrid, FuncIsDepend, PolFunc, 2); //цифра в конце обозначает порядок производной, рассчитываемой в текущей функции
		}

		public static void NewtonNotUniform1(double[] Diff, double[] Xval, double[] resGrid, bool FuncIsDepend, string PolFunc)
		{
			double[] results = new double[resGrid.Length];
			for (int i = 0; i < resGrid.Length; i++)			//Пока в результирующей сетке есть узлы
			{
				for (int j = 0; j < Xval.Length; j++)
				{
					double temp = 1.0;
					for (int k = 0; k < j; k++) temp *= (resGrid[i] - Xval[k]);
					results[i] += Diff[j] * temp;
				}
			}
			save(results, resGrid, FuncIsDepend, PolFunc, 0); //цифра в конце обозначает порядок производной, рассчитываемой в текущей функции
		}

		public static void NewtonNotUniform2(double[] Diff, double[] Xval, double[] resGrid, bool FuncIsDepend, string PolFunc)
		{
			double[] results = new double[resGrid.Length];
			for (int i = 0; i < resGrid.Length; i++)			//Пока в результирующей сетке есть узлы
			{
				for (int j = 0; j < Xval.Length; j++)
				{
					double sum = 0.0;
					for(int l = 0; l < j; l++)
					{
						double temp = 1.0;
						for (int k = 0; k < j; k++)
							if(l != k) temp *= (resGrid[i] - Xval[k]);
						sum += temp;
					}
					results[i] += Diff[j] * sum;
				}
			}
			save(results, resGrid, FuncIsDepend, PolFunc, 1); //цифра в конце обозначает порядок производной, рассчитываемой в текущей функции
		}

		public static void NewtonNotUniform3(double[] Diff, double[] Xval, double[] resGrid, bool FuncIsDepend, string PolFunc)
		{
			double[] results = new double[resGrid.Length];
			for (int i = 0; i < resGrid.Length; i++)			//Пока в результирующей сетке есть узлы
			{
				for (int j = 0; j < Xval.Length; j++)
				{
					double buff = 0.0;
					for(int m = 0; m < j; m++)
					{
						double sum = 0.0;
						for (int l = 0; l < j; l++)
						{
							if (l != m)
							{
								double temp = 1.0;
								for (int k = 0; k < j; k++)
									if (k != l && k != m) temp *= (resGrid[i] - Xval[k]);
								sum += temp;
							}
						}
						buff += sum;
					}
					results[i] += Diff[j] * buff;
				}
			}
			save(results, resGrid, FuncIsDepend, PolFunc, 2); //цифра в конце обозначает порядок производной, рассчитываемой в текущей функции
		}

		public static void save(double[] resY, double[] resGrid, bool FuncIsDepend, string PolFunc, int k)
		{
			double trueResult, sum = 0.0;
			StreamWriter FOut = new StreamWriter("Output.txt");
			for (int i = 0; i < resGrid.Length; i++)
			{
				FOut.WriteLine("{0:f4}, {1:f4}", resGrid[i], resY[i]);
				if(FuncIsDepend)
				{
					trueResult = PolStr.EvalPolStr(PolFunc, resGrid[i], (uint)k);	//Истинный результат
					sum += Math.Pow((trueResult - resY[i]), 2);
				}
			}
			if(FuncIsDepend) FOut.WriteLine("{0:e}", Math.Sqrt(sum / resGrid.Length));
			FOut.Close();
		}
	}
}
