using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkinggi
{
    class ParkingSpace
    {
        public int number { get; set; }
        public Mat mat { get; set; }
        public Mat mat1 { get; set; }
        public int startX { get; set; }
        public int startY { get; set; }
        public int heigh { get; set; }
        public int width { get; set; }
        public bool isFree { get; set; }
        public double perChange { get; set; }
    }
}
