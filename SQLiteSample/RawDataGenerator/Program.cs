using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RawDataGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            new Generator("test.in").Generate(2014, 1, 2017, 8, 10000, 30000, 100, 1000);
        }

        public class Generator
        {
            private const int productsCount = 7;
            private string path;
            public Generator(string path)
            {
                this.path = path;
            }

            public void Generate(int startYear, int startMonth, int endYear, int endMonth, int minMonthOrder, int maxMonthOrder, float minAmount, float maxAmount)
            {
                var rnd = new Random();
                int id = 0;
                using (var sw = new StreamWriter(new FileStream(path, FileMode.Create)))
                {
                    sw.WriteLine("id\tdt\tamount\tproduct_id");
                    for (int i = startYear; i <= endYear; i++)
                    {
                        int endM = startYear == endYear ? endMonth : 12;
                        for (int j = startMonth; j <= endM; j++)
                        {

                            int rndCount = rnd.Next(minMonthOrder, maxMonthOrder);
                            for (int k = 0; k < rndCount; k++)
                            {
                                //a bad string
                                if (rnd.Next(100) < 5)
                                {
                                    sw.WriteLine("1\t2017-02-01T10string:02:12\t235.very\t1bad");
                                } else
                                {
                                    string month = j < 10 ? '0' + j.ToString() : j.ToString();
                                    sw.WriteLine($"{id++}\t{i}-{month}-01T10:02:12\t{(float)(rnd.NextDouble() * (maxAmount - minAmount) + minAmount)}\t{rnd.Next(productsCount) + 1}");
                                }
                                
                            }
                        }
                    }
                }
            }
        }
    }
}
