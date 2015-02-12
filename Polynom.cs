using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace Lab5
{
	class Polynom : ICloneable
	{
		private double[] Factor;
		public int size {get; private set;}

		object ICloneable.Clone()
		{
			return new Polynom(this);
		}

		public Polynom(int s, params double[] arr)
		{
			size = s;
			Factor = new double[size];
			if (arr.Length != 0)	for (int i = 0; i < size; i++) Factor[i] = arr[i];
		}

		public Polynom(Matrix M) : this(M.column + 1)
		{
			int factor = (M.column % 2 == 0 ? 1 : -1);
			for (int i = size-2; i >= 0; i--) Factor[i] = -M[0, M.column - i - 1] * factor;
			Factor[size - 1] = factor;
		}

		public Polynom(Polynom P) : this(P.size, P.Factor) { }

		~Polynom() { }

		public double this[int k]
		{
			get
			{
				return Factor[k];
			}
		}

		public double RectornValue(double x)
		{
			double result = 0;
			for(int i = 0; i < size; i++)
				result += (Factor[i]*Math.Pow(x, i));
			return result;
		}

		public double Derivative(double value)
		{
			int i = 0;
			for (; i < size - 1; i++) Factor[i] = Factor[i + 1] * (i + 1);
			Factor[i] = 0.0;
			return this.RectornValue(value);
		}
	}
}
