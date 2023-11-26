namespace Model
{
    public class Border
    {
        public Border(Point startCoords, Point endCoords)
        {
            StartCoords = new VertexGeoData(startCoords);
            EndCoords = new VertexGeoData(endCoords);
        }

        public VertexGeoData StartCoords { get; }

        public VertexGeoData EndCoords { get; }

        /// <summary>
        /// Null means map edge
        /// </summary>
        public RegionGeoData? LeftSide { get; internal set; }

        /// <summary>
        /// Null means map edge
        /// </summary>
        public RegionGeoData? RightSide { get; internal set; }
    }
}