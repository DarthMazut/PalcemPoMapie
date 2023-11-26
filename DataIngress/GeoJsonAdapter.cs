using Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DataIngress
{
    public static class GeoJsonAdapter
    {
        private static readonly string _regionType = "Region";
        private static readonly string _cityType = "City";
        private static readonly string _harborType = "Harbor";
        private static readonly string _seaType = "Sea";

        public static async Task<IReadOnlyList<RawRegionData>> ReadRegionDataFromFolder(string folderPath)
        {
            // Get only *.geojson files
            IEnumerable<string> geoJsonFiles = Directory.EnumerateFiles(folderPath)
                .Where(name => Path.GetExtension(name) == ".geojson");

            // Group files by region id
            var regionSets = geoJsonFiles.GroupBy(f => Path.GetFileNameWithoutExtension(f).Split('_')[0]);

            List<RawRegionData> resultList = new();
            foreach (var regionSet in regionSets)
            {
                string id = regionSet.Key;
                Dictionary<string, string> typePathDictionary = GetDictionaryFromFilesGroup(regionSet.ToList());
                RegionGeoType regionType = ResolveRegionType(typePathDictionary);
                Point? cityPoint = await GetCityPoint(typePathDictionary);
                Point? harborPoint = await GetHarborPoint(typePathDictionary);
                IList<RawBorderData> borderData = await GetBorderData(regionType, typePathDictionary);

                resultList.Add(new RawRegionData(id, regionType, cityPoint, harborPoint, borderData));
            }

            return resultList;
        }

        private static Dictionary<string, string> GetDictionaryFromFilesGroup(List<string> list)
            => list.ToDictionary(f => Path.GetFileNameWithoutExtension(f).Split('_')[1], f => f);

        private static RegionGeoType ResolveRegionType(Dictionary<string, string> typePathDictionary)
            => typePathDictionary.ContainsKey(_seaType) ? RegionGeoType.Sea : RegionGeoType.Land;

        private static Task<Point?> GetCityPoint(Dictionary<string, string> typePathDictionary)
            => GetPointByType(_cityType, typePathDictionary);

        private static Task<Point?> GetHarborPoint(Dictionary<string, string> typePathDictionary)
            => GetPointByType(_harborType, typePathDictionary);

        private static async Task<Point?> GetPointByType(string type, Dictionary<string, string> typePathDictionary)
        {
            if (typePathDictionary.TryGetValue(type, out string? file))
            {
                string fileContent = await File.ReadAllTextAsync(file);
                GeoJsonPointDto? pointDto = JsonSerializer.Deserialize<GeoJsonPointDto>(fileContent);
                if (pointDto is { })
                {
                    GeoJsonPointGeometryDto geometry = pointDto.Features[0].Geometry;
                    return new Point(geometry.Coordinates[0], geometry.Coordinates[1]);
                }
            }

            return null;
        }

        private static async Task<IList<RawBorderData>> GetBorderData(RegionGeoType type, Dictionary<string, string> typePathDictionary)
        {
            string selectedFile = type == RegionGeoType.Land ? typePathDictionary[_regionType] : typePathDictionary[_seaType];
            string fileContent = await File.ReadAllTextAsync(selectedFile);
            GeoJsonPolygonDto? dto = JsonSerializer.Deserialize<GeoJsonPolygonDto>(fileContent);
            if (dto is { })
            {
                List<RawBorderData> borders = new();
                List<Point> points = dto.Features[0].Geometry.Coordinates[0].Select(c => new Point(c[0], c[1])).ToList();
                for (int i = 1; i < points.Count; i++)
                {
                    Point startPoint = points[i-1];
                    Point endPoint = points[i];

                    borders.Add(new RawBorderData(startPoint, endPoint));
                }

                return borders;
            }

            return null!;
        }
    }
}
