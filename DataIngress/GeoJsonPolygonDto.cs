using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataIngress
{
    // Root myDeserializedClass = JsonSerializer.Deserialize<GeoJsonPolygonDto>(myJsonResponse);
    public record GeoJsonPolygonDto(
        [property: JsonPropertyName("type")] string Type,
        [property: JsonPropertyName("features")] IReadOnlyList<GeoJsonPolygonFeatureDto> Features
    );

    public record GeoJsonPolygonFeatureDto(
        [property: JsonPropertyName("type")] string Type,
        [property: JsonPropertyName("properties")] GeoJsonPolygonPropertiesDto Properties,
        [property: JsonPropertyName("geometry")] GeoJsonPolygonGeometryDto Geometry
    );

    public record GeoJsonPolygonGeometryDto(
        [property: JsonPropertyName("type")] string Type,
        [property: JsonPropertyName("coordinates")] IReadOnlyList<IReadOnlyList<IReadOnlyList<double>>> Coordinates
    );

    public record GeoJsonPolygonPropertiesDto(
        [property: JsonPropertyName("Type")] string Type
    );
}
