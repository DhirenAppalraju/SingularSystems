using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingularSystems
{
    class Program
    {
        static void Main(string[] args)
        {
            List<ObjectInSpace> objectsInSpaceList = new List<ObjectInSpace>();
            Random random = new Random();
            using (var writer = new StreamWriter("..\\..\\..\\Universe.txt"))
            {
                for (int loop = 0; loop < 15000; loop++)
                {
                    objectsInSpaceList.Add(new ObjectInSpace()
                    {
                        X = random.Next(0, 999999999),
                        Y = random.Next(0, 999999999),
                        Z = random.Next(0, 999999999),

                        Habbitable = random.Next(2) == 0,
                        Monster = random.Next(2) == 0,

                        Space = random.Next(1, 100)
                    });

                    writer.WriteLine(
                        string.Format("{0},{1},{2},{3},{4},{5}",
                        objectsInSpaceList[loop].X,
                        objectsInSpaceList[loop].Y,
                        objectsInSpaceList[loop].Z,
                        objectsInSpaceList[loop].Habbitable,
                        objectsInSpaceList[loop].Monster,
                        objectsInSpaceList[loop].Space
                        ));
                }
            }
        }

        public class ObjectInSpace
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Z { get; set; }
            
            public bool Habbitable { get; set; }
            public bool Monster { get; set; }

            public int Space { get; set; }
        }
    }
}
