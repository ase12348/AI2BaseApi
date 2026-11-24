
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using System.Text.Json.Nodes;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace jcode.web.util
{
    public class GeoJSONSchemes
    {
        class DocumentFilter : IDocumentFilter
        {
            public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
            {

                swaggerDoc.Components.Schemas.Add(
                    "Point3D",
                    new OpenApiSchema
                    {
                        Type = JsonSchemaType.Array,
                        Description = "Point in 3D space",
                        ExternalDocs = new OpenApiExternalDocs
                        {
                            Url = new Uri("http://geojson.org/geojson-spec.html#id2")
                        },
                        MinItems = 2,
                        MaxItems = 3,
                        Items = new OpenApiSchema
                        {
                            Type = JsonSchemaType.Number
                        }
                    }
                );
     
                swaggerDoc.Components.Schemas.Add("Geometry",
                    new OpenApiSchema
                    {
                        ExternalDocs = new OpenApiExternalDocs
                        {
                            Url = new Uri("http://geojson.org/geojson-spec.html#geometry-objects"),
                        },
                        Type = JsonSchemaType.Object,
                        Extensions = new Dictionary<string, IOpenApiExtension>
                        {
                            ["clrType"] = new JsonNodeExtension(JsonValue.Create(typeof(Geometry).FullName))
                        },
                        Description = "GeoJSON geometry",
                        Discriminator = new OpenApiDiscriminator
                        {
                            PropertyName = "type",
                        },
                        Required = new HashSet<string> {"type"},
                        Properties = new Dictionary<string, IOpenApiSchema>
                        {
                            ["type"] = new OpenApiSchema
                            {
                                Type = JsonSchemaType.String,
                                Enum = new List<JsonNode>
                                {
                                    JsonValue.Create("Point"),
                                    JsonValue.Create("LineString"),
                                    JsonValue.Create("Polygon"),
                                    JsonValue.Create("MultiPoint"),
                                    JsonValue.Create("MultiLineString"),
                                    JsonValue.Create("MultiPolygon"),
                                },
                                Description = "the geometry type"
                            }
                        }
                    }
                );
            }
        }
     
        public static void ConfigureForNetTopologySuite(SwaggerGenOptions c)
        {
            c.DocumentFilter<DocumentFilter>();
            c.MapType<Geometry>(
                () => new OpenApiSchema
                {
                    ExternalDocs = new OpenApiExternalDocs
                    {
                        Url = new Uri("http://geojson.org/geojson-spec.html#geometry-objects"),
                    },
                    Type = JsonSchemaType.Object,
                    Extensions = new Dictionary<string, IOpenApiExtension>
                    {
                        ["clrType"] = new JsonNodeExtension(JsonValue.Create(typeof(Geometry).FullName))
                    },
                    Description = "GeoJSON geometry",
                    Discriminator = new OpenApiDiscriminator
                    {
                        PropertyName = "type",
                    },
                    Required = new HashSet<string> {"type"},
                    Properties = new Dictionary<string, IOpenApiSchema>
                    {
                        ["type"] = new OpenApiSchema
                        {
                            Type = JsonSchemaType.String,
                            Enum = new List<JsonNode>
                            {
                                JsonValue.Create("Point"),
                                JsonValue.Create("LineString"),
                                JsonValue.Create("Polygon"),
                                JsonValue.Create("MultiPoint"),
                                JsonValue.Create("MultiLineString"),
                                JsonValue.Create("MultiPolygon"),
                            },
                            Description = "the geometry type"
                        }
                    }
                }
            );
     
            c.MapType<Point>(
                () => new OpenApiSchema
                {
                    ExternalDocs = new OpenApiExternalDocs
                    {
                        Url = new Uri("http://geojson.org/geojson-spec.html#id2"),
                    },
                    Type = JsonSchemaType.Object,
                    Extensions = new Dictionary<string, IOpenApiExtension>
                    {
                        ["clrType"] = new JsonNodeExtension(JsonValue.Create(typeof(Point).FullName))
                    },
                    Description = "GeoJSON Point",
                    AllOf = new List<IOpenApiSchema>
                    {
                        new OpenApiSchemaReference("Geometry", null, null),
                        new OpenApiSchema
                        {
                            Properties = new Dictionary<string, IOpenApiSchema>
                            {
                                ["coordinates"] = new OpenApiSchemaReference("Point3D", null, null)
                            }
                        }
                    },
                }
            );
            c.MapType<LineString>(
                () => new OpenApiSchema
                {
                    ExternalDocs = new OpenApiExternalDocs
                    {
                        Url = new Uri("http://geojson.org/geojson-spec.html#id3"),
                    },
                    Type = JsonSchemaType.Object,
                    Extensions = new Dictionary<string, IOpenApiExtension>
                    {
                        ["clrType"] = new JsonNodeExtension(JsonValue.Create(typeof(LineString).FullName))
                    },
                    Description = "GeoJSON LineString",
                    AllOf = new List<IOpenApiSchema>
                    {
                        new OpenApiSchemaReference("Geometry", null, null),
                        new OpenApiSchema
                        {
                            Properties = new Dictionary<string, IOpenApiSchema>
                            {
                                ["type"] = new OpenApiSchema()
                                {
                                    Type = JsonSchemaType.String,
                                    Enum = new List<JsonNode>
                                    {
                                        JsonValue.Create("LineString"),
                                    },
                                },
                                ["coordinates"] = new OpenApiSchema
                                {
                                    Type = JsonSchemaType.Array,
                                    Items = new OpenApiSchemaReference("Point3D", null, null)
                                }
                            }
                        }
                    },
                }
            );
            c.MapType<Polygon>(
                () => new OpenApiSchema
                {
                    ExternalDocs = new OpenApiExternalDocs
                    {
                        Url = new Uri("http://geojson.org/geojson-spec.html#id4"),
                    },
                    Type = JsonSchemaType.Object,
                    Extensions = new Dictionary<string, IOpenApiExtension>
                    {
                        ["clrType"] = new JsonNodeExtension(JsonValue.Create(typeof(Polygon).FullName))
                    },
                    Description = "GeoJSON Polygon",
                    AllOf = new List<IOpenApiSchema>
                    {
                        new OpenApiSchemaReference("Geometry", null, null),
                        new OpenApiSchema
                        {
                            Properties = new Dictionary<string, IOpenApiSchema>
                            {
                                ["type"] = new OpenApiSchema()
                                {
                                    Type = JsonSchemaType.String,
                                    Enum = new List<JsonNode>
                                    {
                                        JsonValue.Create("Polygon"),
                                    },
                                },
                                ["coordinates"] = new OpenApiSchema
                                {
                                    Type = JsonSchemaType.Array,
                                    Items = new OpenApiSchema
                                    {
                                        Type = JsonSchemaType.Array,
                                        Items = new OpenApiSchemaReference("Point3D", null, null)
                                    }
                                }
                            }
                        }
                    },
                }
            );
            c.MapType<MultiPoint>(
                () => new OpenApiSchema
                {
                    ExternalDocs = new OpenApiExternalDocs
                    {
                        Url = new Uri("http://geojson.org/geojson-spec.html#id4"),
                    },
                    Type = JsonSchemaType.Object,
                    Extensions = new Dictionary<string, IOpenApiExtension>
                    {
                        ["clrType"] = new JsonNodeExtension(JsonValue.Create(typeof(MultiPoint).FullName))
                    },
                    Description = "GeoJSON MultiPolygon",
                    AllOf = new List<IOpenApiSchema>
                    {
                        new OpenApiSchemaReference("Geometry", null, null),
                        new OpenApiSchema
                        {
                            Properties = new Dictionary<string, IOpenApiSchema>
                            {
                                ["type"] = new OpenApiSchema()
                                {
                                    Type = JsonSchemaType.String,
                                    Enum = new List<JsonNode>
                                    {
                                        JsonValue.Create("MultiPoint"),
                                    },
                                },
                                ["coordinates"] = new OpenApiSchema
                                {
                                    Type = JsonSchemaType.Array,
                                    Items = new OpenApiSchemaReference("Point3D", null, null)
                                }
                            }
                        }
                    },
                }
            );
            c.MapType<MultiLineString>(
                () => new OpenApiSchema
                {
                    ExternalDocs = new OpenApiExternalDocs
                    {
                        Url = new Uri("http://geojson.org/geojson-spec.html#id5"),
                    },
                    Type = JsonSchemaType.Object,
                    Extensions = new Dictionary<string, IOpenApiExtension>
                    {
                        ["clrType"] = new JsonNodeExtension(JsonValue.Create(typeof(MultiLineString).FullName))
                    },
                    Description = "GeoJSON MultiLineString",
                    AllOf = new List<IOpenApiSchema>
                    {
                        new OpenApiSchemaReference("Geometry", null, null),
                        new OpenApiSchema
                        {
                            Properties = new Dictionary<string, IOpenApiSchema>
                            {
                                ["type"] = new OpenApiSchema()
                                {
                                    Type = JsonSchemaType.String,
                                    Enum = new List<JsonNode>
                                    {
                                        JsonValue.Create("MultiLineString"),
                                    },
                                },
                                ["coordinates"] = new OpenApiSchema
                                {
                                    Type = JsonSchemaType.Array,
                                    Items = new OpenApiSchema
                                    {
                                        Type = JsonSchemaType.Array,
                                        Items = new OpenApiSchemaReference("Point3D", null, null)
                                    }
                                }
                            }
                        }
                    },
                }
            );
            c.MapType<MultiPolygon>(
                () => new OpenApiSchema
                {
                    ExternalDocs = new OpenApiExternalDocs
                    {
                        Url = new Uri("http://geojson.org/geojson-spec.html#id6"),
                    },
                    Type = JsonSchemaType.Object,
                    Extensions = new Dictionary<string, IOpenApiExtension>
                    {
                        ["clrType"] = new JsonNodeExtension(JsonValue.Create(typeof(MultiPolygon).FullName))
                    },
                    Description = "GeoJSON MultiPolygon",
                    AllOf = new List<IOpenApiSchema>
                    {
                        new OpenApiSchemaReference("Geometry", null, null),
                        new OpenApiSchema
                        {
                            Properties = new Dictionary<string, IOpenApiSchema>
                            {
                                ["type"] = new OpenApiSchema()
                                {
                                    Type = JsonSchemaType.String,
                                    Enum = new List<JsonNode>
                                    {
                                        JsonValue.Create("MultiPolygon"),
                                    },
                                },
                                ["coordinates"] = new OpenApiSchema
                                {
                                    Type = JsonSchemaType.Array,
                                    Items = new OpenApiSchema
                                    {
                                        Type = JsonSchemaType.Array,
                                        Items = new OpenApiSchema
                                        {
                                            Type = JsonSchemaType.Array,
                                            Items = new OpenApiSchemaReference("Point3D", null, null)
                                        }
                                    }
                                }
                            }
                        }
                    },
                }
            );
            c.MapType<GeometryCollection>(
                () => new OpenApiSchema
                {
                    ExternalDocs = new OpenApiExternalDocs
                    {
                        Url = new Uri("http://geojson.org/geojson-spec.html#geometrycollection"),
                    },
                    Type = JsonSchemaType.Object,
                    Extensions = new Dictionary<string, IOpenApiExtension>
                    {
                        ["clrType"] = new JsonNodeExtension(JsonValue.Create(typeof(GeometryCollection).FullName))
                    },
                    Required = new HashSet<string> {"type", "geometries"},
                    Description = "GeoJSON geometry collection",
                    Properties = new Dictionary<string, IOpenApiSchema>
                    {
                        ["type"] = new OpenApiSchema
                        {
                            Type = JsonSchemaType.String,
                            Enum = new List<JsonNode> {JsonValue.Create("GeometryCollection")},
                        },
                        ["geometries"] = new OpenApiSchema
                        {
                            Type = JsonSchemaType.Array,
                            Items = new OpenApiSchemaReference("Geometry", null, null)
                        }
                    }
                }
            );
     
            c.MapType<Feature>(
                () => new OpenApiSchema
                {
                    Type = JsonSchemaType.Object,
                    Description = "GeoJSON Feature",
                    Required = new HashSet<string> {"type", "id", "geometry"},
                    ExternalDocs = new OpenApiExternalDocs
                    {
                        Url = new Uri("https://tools.ietf.org/html/rfc7946#section-3.2")
                    },
                    Properties = new Dictionary<string, IOpenApiSchema>
                    {
                        ["type"] = new OpenApiSchema
                        {
                            Type = JsonSchemaType.String,
                            Enum = new List<JsonNode> {JsonValue.Create("Feature")}
                        },
                        ["id"] = new OpenApiSchema
                        {
                            Type = JsonSchemaType.Integer,
                        },
                        ["geometry"] = new OpenApiSchemaReference("GeometryCollection", null, null),
                        ["properties"] = new OpenApiSchema
                        {
                            Type = JsonSchemaType.Object
                        }
                    }
                }
            );
     
            c.MapType<FeatureCollection>(
                () => new OpenApiSchema
                {
                    Type = JsonSchemaType.Object,
                    Description = "GeoJSON Feature collection",
                    Required = new HashSet<string> {"type", "features"},
                    ExternalDocs = new OpenApiExternalDocs
                    {
                        Url = new Uri("https://tools.ietf.org/html/rfc7946#section-3.2")
                    },
                    Properties = new Dictionary<string, IOpenApiSchema>
                    {
                        ["type"] = new OpenApiSchema
                        {
                            Type = JsonSchemaType.String,
                            Enum = new List<JsonNode> {JsonValue.Create("FeatureCollection")}
                        },
                        ["features"] = new OpenApiSchema
                        {
                            Type = JsonSchemaType.Array,
                            Items = new OpenApiSchemaReference("Feature", null, null)
                        }
                    }
                }
            );
        }
    }
}
 
