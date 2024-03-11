using System;
using System.Threading;

namespace ThreadMinElementSharp
{
    class Program
    {
        private static readonly int dim = 5000;
        private static readonly int threadNum = 6;

        private readonly Thread[] threads = new Thread[threadNum];

        static void Main(string[] args)
        {
            Program main = new Program();
            main.InitArr();

            var result = main.ParallelMin();

            Console.WriteLine($"Minimum element: {result.MinValue} at index {result.MinIndex}");
            Console.ReadKey();
        }

        private int threadCount = 0;

        private (int MinValue, int MinIndex) ParallelMin()
        {
            for (int i = 0; i < threadNum; i++)
            {
                threads[i] = new Thread(StarterThread);
                threads[i].Start(new Bound(i * dim / threadNum, (i + 1) * dim / threadNum));
            }

            lock (lockerForCount)
            {
                while (threadCount < threadNum)
                {
                    Monitor.Wait(lockerForCount);
                }
            }
            return (minElement, minIndex);
        }

        private readonly int[] arr = new int[dim];
        private int minElement = int.MaxValue;
        private int minIndex = -1;

        private void InitArr()
        {
            // Random random = new Random();

            for (int i = 0; i < dim; i++)
            {
                arr[25] = -7;
            }
        }

        class Bound
        {
            public Bound(int startIndex, int finishIndex)
            {
                StartIndex = startIndex;
                FinishIndex = finishIndex;
            }

            public int StartIndex { get; set; }
            public int FinishIndex { get; set; }
        }

        private readonly object lockerForMin = new object();
        private void StarterThread(object param)
        {
            if (param is Bound)
            {
                (int minValue, int minIndex) = PartMin((param as Bound).StartIndex, (param as Bound).FinishIndex);

                lock (lockerForMin)
                {
                    UpdateMin(minValue, minIndex);
                }
                IncThreadCount();
            }
        }

        private readonly object lockerForCount = new object();
        private void IncThreadCount()
        {
            lock (lockerForCount)
            {
                threadCount++;
                Monitor.Pulse(lockerForCount);
            }
        }

        public void UpdateMin(int minValue, int minIndex)
        {
            if (minValue < minElement)
            {
                minElement = minValue;
                this.minIndex = minIndex;
            }
        }

        public (int MinValue, int MinIndex) PartMin(int startIndex, int finishIndex)
        {
            int min = int.MaxValue;
            int index = -1;

            for (int i = startIndex; i < finishIndex; i++)
            {
                if (arr[i] < min)
                {
                    min = arr[i];
                    index = i;
                }
            }
            return (min, index);
        }
    }
}
