using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class RawRegionData
    {
        public RawRegionData(string id, RegionGeoType type, Point? cityCoords, Point? harborCoords, IList<RawBorderData> borders)
        {
            Id = id;
            Type = type;
            CityCoords = cityCoords;
            HarborCoords = harborCoords;
            Borders = borders.AsReadOnly();
        }

        public string Id { get; }

        public RegionGeoType Type { get; }

        public Point? CityCoords { get; }

        public Point? HarborCoords { get; }

        public IReadOnlyList<RawBorderData> Borders { get; }
    }
}
