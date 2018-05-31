using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Task_2
{
    class Program
    {
        static object dollars250 = new object();
        static object dollars350 = new object();

        static void BuyingEarrings()
        {
            lock (dollars250)
            {
                Thread.Sleep(500);
                lock (dollars350)
                {
                    Console.WriteLine("Mary bought earrings");
                }
            }
        }

        static void BuyingWatches()
        {
            lock (dollars350)
            {
                Thread.Sleep(500);
                lock (dollars250)
                {
                    Console.WriteLine("James bought watches");
                }
            }
        }

        static void Main()
        {
            var James = new Thread(BuyingWatches);
            var Mary = new Thread(BuyingEarrings);

            James.Start();
            Mary.Start();

            Console.WriteLine("Main thread ended");
        }
    }
}