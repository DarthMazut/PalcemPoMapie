namespace Model
{
    public class VertexGeoData
    {
        private readonly List<RegionGeoData> _neighbors = new();

        public VertexGeoData(Point coords)
        {
            Coords = coords;
        }

        public Point Coords { get; }

        public IReadOnlyList<RegionGeoData> Neighbors => _neighbors.AsReadOnly();

        internal bool AddNeighbor(RegionGeoData region)
        {
            if (!_neighbors.Contains(region))
            {
                _neighbors.Add(region);
                return true;
            }

            return false;
        }
    }
}