using System.Drawing;

namespace Model
{
    public class RawBorderData
    {
        public RawBorderData(Point startPoint, Point endPoint)
        {
            StartCoords = startPoint;
            EndCoords = endPoint;
        }

        public Point StartCoords { get; }

        public Point EndCoords { get; }
    }
}
