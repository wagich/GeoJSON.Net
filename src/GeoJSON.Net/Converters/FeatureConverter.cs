using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace GeoJSON.Net.Converters
{
	public class FeatureConverter : JsonConverter
	{
		private static readonly GeometryConverter GeometryConverter = new GeometryConverter();

		public override bool CanConvert(Type objectType)
		{
			return objectType.IsInterface
				&& objectType.IsGenericType
				&& objectType.GetGenericTypeDefinition() == typeof(IFeature<,>);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var propsType = objectType.GetGenericArguments()[1];

			switch (reader.TokenType)
			{
				case JsonToken.Null:
					return null;
				case JsonToken.StartObject:
					var value = JObject.Load(reader);
					return ReadGeoJsonFeature(value, propsType, serializer);
			}

			throw new JsonReaderException("expected null, object or array token but received " + reader.TokenType);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			serializer.Serialize(writer, value);
		}

		private static object ReadGeoJsonFeature(JObject value, Type propsType, JsonSerializer serializer)
		{
			if (!value.TryGetValue("type", StringComparison.OrdinalIgnoreCase, out var token))
			{
				throw new JsonReaderException("json must contain a \"type\" property");
			}

			if (!Enum.TryParse(token.Value<string>(), true, out GeoJSONObjectType geoJsonType) || geoJsonType != GeoJSONObjectType.Feature)
			{
				throw new JsonReaderException("type must be \"feature\"");
			}

			var geometryReader = value["geometry"].CreateReader();
			geometryReader.Read();
			var geometry = GeometryConverter.ReadJson(geometryReader, typeof(IGeometryObject), null, serializer);

			var id = value.Value<string>("id");
			var properties = value["properties"].ToObject(propsType);

			var featureType = typeof(Feature<,>).MakeGenericType(geometry.GetType(), propsType);
			var ignorePropertyEquality = propsType == typeof(IDictionary<string, object>);

			return Activator.CreateInstance(
				featureType,
				BindingFlags.Instance | BindingFlags.NonPublic,
				args: new[] { geometry, properties, id, ignorePropertyEquality },
				binder: null,
				culture: null
			);
		}
	}
}
