using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class RegionGeoData
    {
        private readonly List<Border> _borders = new();
        private readonly List<VertexGeoData> _vertices = new();
        private readonly List<RegionGeoData> _neighbors = new();

        private RegionGeoData(string id, RegionGeoType type, Point? cityCoords, Point? harborCoords)
        {
            Id = id;
            Type = type;
            CityCoords = cityCoords;
            HarborCoords = harborCoords;
        }

        public string Id { get; }

        public RegionGeoType Type { get; }

        public Point? CityCoords { get; }

        public Point? HarborCoords { get; }

        public IReadOnlyList<Border> Borders => _borders.AsReadOnly();

        public IReadOnlyList<VertexGeoData> Vertices => _vertices.AsReadOnly();

        public IReadOnlyList<RegionGeoData> Neighbors => _neighbors.AsReadOnly();

        public static IList<RegionGeoData> FromRawRegionData(IList<RawRegionData> rawRegions)
        {
            List<RegionGeoData> regions = new();
            List<VertexGeoData> commonVertices = new();
            List<Border> commonBorders = new();

            foreach (RawRegionData rawRegion in rawRegions)
            {
                RegionGeoData region = new(rawRegion.Id, rawRegion.Type, rawRegion.CityCoords, rawRegion.HarborCoords);
                foreach (RawBorderData rawBorder in rawRegion.Borders)
                {
                    HandleBorder(commonBorders, region, rawBorder);
                    HandleVertices(commonVertices, region, rawBorder);
                }

                regions.Add(region);
            }

            AssignNeighbors(regions);

            return regions;
        }

        private static void HandleBorder(List<Border> commonBorders, RegionGeoData region, RawBorderData rawBorder)
        {
            Border? border = commonBorders.FirstOrDefault(b =>
            b.StartCoords.Coords == rawBorder.StartCoords && b.EndCoords.Coords == rawBorder.EndCoords ||
            b.EndCoords.Coords == rawBorder.StartCoords && b.StartCoords.Coords == rawBorder.EndCoords);
            if (border is null)
            {
                border = new Border(rawBorder.StartCoords, rawBorder.EndCoords)
                {
                    LeftSide = region
                };
                commonBorders.Add(border);
            }
            else
            {
                border.RightSide = region;
            }

            region.AddBorder(border);
        }

        private static void HandleVertices(List<VertexGeoData> commonVertices, RegionGeoData region, RawBorderData rawBorder)
        {
            VertexGeoData? startVertex = commonVertices.FirstOrDefault(v => v.Coords == rawBorder.StartCoords);
            VertexGeoData? endVertex = commonVertices.FirstOrDefault(v => v.Coords == rawBorder.EndCoords);

            if (startVertex is null)
            {
                startVertex = new VertexGeoData(rawBorder.StartCoords);
                commonVertices.Add(startVertex);
            }

            if (endVertex is null)
            {
                endVertex = new VertexGeoData(rawBorder.EndCoords);
                commonVertices.Add(endVertex);
            }

            startVertex.AddNeighbor(region);
            endVertex.AddNeighbor(region);

            region.AddVertex(startVertex);
            region.AddVertex(endVertex);
        }

        private static void AssignNeighbors(List<RegionGeoData> regions)
        {
            foreach (RegionGeoData region in regions)
            {
                foreach (Border border in region.Borders)
                {
                    if (border.LeftSide is { })
                    {
                        region.AddNeighbor(border.LeftSide);
                    }

                    if (border.RightSide is { })
                    {
                        region.AddNeighbor(border.RightSide);
                    }
                }
            }
        }

        private void AddBorder(Border border)
        {
            if (!_borders.Contains(border))
            {
                _borders.Add(border);
            }
        }

        private void AddVertex(VertexGeoData vertex)
        {
            if (!Vertices.Contains(vertex))
            {
                _vertices.Add(vertex);
            }
        }

        private void AddNeighbor(RegionGeoData neighbor)
        {
            if (neighbor != this && !_neighbors.Contains(neighbor))
            {
                _neighbors.Add(neighbor);
            }
        }

        
    }
}
