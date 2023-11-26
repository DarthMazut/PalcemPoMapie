using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataIngress
{
    // Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
    public record GeoJsonPointDto(
        [property: JsonPropertyName("type")] string Type,
        [property: JsonPropertyName("features")] IReadOnlyList<GeoJsonPointFeatureDto> Features
    );

    public record GeoJsonPointFeatureDto(
        [property: JsonPropertyName("type")] string Type,
        [property: JsonPropertyName("properties")] GeoJsonPointPropertiesDto Properties,
        [property: JsonPropertyName("geometry")] GeoJsonPointGeometryDto Geometry
    );

    public record GeoJsonPointGeometryDto(
        [property: JsonPropertyName("type")] string Type,
        [property: JsonPropertyName("coordinates")] IReadOnlyList<double> Coordinates
    );

    public record GeoJsonPointPropertiesDto();




}
