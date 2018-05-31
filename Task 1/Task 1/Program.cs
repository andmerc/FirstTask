using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_1
{
    class Program
    {
        static int[] GetRandomArray(int n)
        {
            var rand = new Random();
            var randArray = new int[n];
            for(int i = 0; i < randArray.Length; i++)
            {
                randArray[i] = rand.Next(25);
            }
            return randArray;
        }
        static double[] GetMovingAverages(int[] array, int period)
        {
            var averages = new double[array.Length - period + 1];
            for (int i = 0; i < averages.Length; i++)
            {
                averages[i] = SimpleMovingAverage(array, i + period - 1, period);
            }
            return averages;
        }
        static double SimpleMovingAverage(int[] array, int point, int period)
        {
            double sma = 0;
            if (point > array.Length)
            {
                throw new ArgumentOutOfRangeException();
            }
            for (int i = point; i >= point - period + 1; i--)
            {
                sma += (double)array[i];
            }
            sma /= (double)period;
            return sma;
        }
        static double[] GetRelationsForArray(int[] array)
        {
            var relations = new double[array.Length - 1];
            for (int i = 0; i < relations.Length; i++)
            {
                if(array[i] != 0)
                {
                    relations[i] = (double)array[i + 1] / (double)array[i];
                } else
                {
                    throw new DivideByZeroException();
                }
            }
            return relations;
        }
        static void Main()
        {
            int n = 0, p = 0;

            Console.Write("Enter array length: ");
            bool incorrectInput = true;
            while (incorrectInput)
            {
                try
                {
                    n = int.Parse(Console.ReadLine());
                    if (n <= 0)
                    {
                        throw new ArgumentException();
                    }
                    incorrectInput = false;
                }
                catch (Exception)
                {
                    Console.Write("Incorrect input! Enter array length again: ");
                }
            }
            var array = GetRandomArray(n);
            Console.WriteLine("Source array: {0}", string.Join(", ", array));

            Console.Write("Enter period for calculating SMA: ");
            incorrectInput = true;
            while (incorrectInput)
            {
                try
                {
                    p = int.Parse(Console.ReadLine());
                    if (p > n || p <= 0)
                    {
                        throw new ArgumentException();
                    }
                    incorrectInput = false;
                }
                catch (Exception)
                {
                    Console.Write("Incorrect input! Enter period again: ");
                }
            }
            var averages = GetMovingAverages(array, p);
            Console.WriteLine("Simple moving averages for that array: {0}", string.Join(", ", averages));

            try
            {
                var relations = GetRelationsForArray(array);
                Console.WriteLine("Relations of elements to previous one in that array: {0}", string.Join(", ", relations));
            }
            catch (DivideByZeroException)
            {
                Console.Write("There is a zero element in the middle of array, it's impossible to calculate the relations.");
            }
        }
    }
}
