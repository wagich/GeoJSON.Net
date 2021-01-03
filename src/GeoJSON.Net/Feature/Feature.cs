// Copyright © Joerg Battermann 2014, Matt Hunt 2017

using System.Collections.Generic;
using GeoJSON.Net.Converters;
using GeoJSON.Net.Geometry;
using Newtonsoft.Json;
using System;
using System.Reflection;
using System.Linq;

namespace GeoJSON.Net.Feature
{
	/// <summary>
	/// A GeoJSON Feature Object; generic version for strongly typed <see cref="Geometry"/>
	/// and <see cref="Properties"/>
	/// </summary>
	/// <remarks>
	/// See https://tools.ietf.org/html/rfc7946#section-3.2
	/// </remarks>
	public interface IFeature<out TGeometry, TProps> : IGeoJSONObject where TGeometry : IGeometryObject
	{
		string Id { get; }
		TGeometry Geometry { get; }
		TProps Properties { get; }
	}

	/// <summary>
	/// A GeoJSON Feature Object; generic version for strongly typed <see cref="Geometry"/>
	/// and <see cref="Properties"/>
	/// </summary>
	/// <remarks>
	/// See https://tools.ietf.org/html/rfc7946#section-3.2
	/// </remarks>
	public class Feature<TGeometry, TProps> : GeoJSONObject, IFeature<TGeometry, TProps>, IEquatable<Feature<TGeometry, TProps>>
		where TGeometry : IGeometryObject
	{
		private readonly bool _ignorePropertyEquality;

		[JsonConstructor]
		public Feature(TGeometry geometry, TProps properties, string id = null)
		{
			Geometry = geometry;
			Properties = properties;
			Id = id;
		}

		protected Feature(TGeometry geometry, TProps properties, string id, bool ignorePropertyEquality)
		{
			Geometry = geometry;
			Properties = properties;
			Id = id;
			_ignorePropertyEquality = ignorePropertyEquality;
		}

		public override GeoJSONObjectType Type => GeoJSONObjectType.Feature;

		[JsonProperty(PropertyName = "id", NullValueHandling = NullValueHandling.Ignore)]
		public string Id { get; }

		[JsonProperty(PropertyName = "geometry", Required = Required.AllowNull)]
		[JsonConverter(typeof(GeometryConverter))]
		public TGeometry Geometry { get; }

		[JsonProperty(PropertyName = "properties", Required = Required.AllowNull)]
		public TProps Properties { get; }

		/// <summary>
		/// Equality comparer.
		/// </summary>
		/// <remarks>
		/// In contrast to <see cref="Feature.Equals(Feature)"/>, this implementation returns true only
		/// if <see cref="Id"/> and <see cref="Properties"/> are also equal. See
		/// <a href="https://github.com/GeoJSON-Net/GeoJSON.Net/issues/80">#80</a> for discussion. The rationale
		/// here is that a user explicitly specifying the property type most probably cares about the properties
		/// equality.
		/// </remarks>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(Feature<TGeometry, TProps> other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return base.Equals(other)
				   && string.Equals(Id, other.Id)
				   && EqualityComparer<TGeometry>.Default.Equals(Geometry, other.Geometry)
				   && (_ignorePropertyEquality || EqualityComparer<TProps>.Default.Equals(Properties, other.Properties));
		}

		public override bool Equals(object obj)
		{
			if (obj is null) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((Feature<TGeometry, TProps>)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = base.GetHashCode();
				hashCode = (hashCode * 397) ^ (Id != null ? Id.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ EqualityComparer<TGeometry>.Default.GetHashCode(Geometry);
				if (!_ignorePropertyEquality)
				{
					hashCode = (hashCode * 397) ^ EqualityComparer<TProps>.Default.GetHashCode(Properties);
				}
				return hashCode;
			}
		}

		public static bool operator ==(Feature<TGeometry, TProps> left, Feature<TGeometry, TProps> right)
		{
			return object.Equals(left, right);
		}

		public static bool operator !=(Feature<TGeometry, TProps> left, Feature<TGeometry, TProps> right)
		{
			return !object.Equals(left, right);
		}
	}

	/// <summary>
	/// A GeoJSON Feature Object.
	/// </summary>
	/// <remarks>
	/// See https://tools.ietf.org/html/rfc7946#section-3.2
	/// </remarks>
	public class Feature<TGeometry> : Feature<TGeometry, IDictionary<string, object>>, IEquatable<Feature<TGeometry, IDictionary<string, object>>>
		where TGeometry : IGeometryObject
	{
		[JsonConstructor]
		public Feature(TGeometry geometry, IDictionary<string, object> properties = null, string id = null)
			: base(geometry, properties, id, true) { }

		protected Feature(TGeometry geometry, IDictionary<string, object> properties, string id, bool ignorePropertyEquality)
			: base(geometry, properties, id, ignorePropertyEquality) { }

		public bool Equals(Feature<TGeometry> other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;

			if (Geometry == null && other.Geometry == null)
			{
				return true;
			}

			if (Geometry == null && other.Geometry != null)
			{
				return false;
			}

			if (Geometry == null)
			{
				return false;
			}

			return EqualityComparer<TGeometry>.Default.Equals(Geometry, other.Geometry);
		}

		public override bool Equals(object obj)
		{
			if (obj is null) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj.GetType() == GetType() && Equals((Feature<TGeometry>)obj);
		}

		public override int GetHashCode()
		{
			return Geometry.GetHashCode();
		}

		public static bool operator ==(Feature<TGeometry> left, Feature<TGeometry> right)
		{
			return left?.Equals(right) ?? right is null;
		}

		public static bool operator !=(Feature<TGeometry> left, Feature<TGeometry> right)
		{
			return !(left?.Equals(right) ?? right is null);
		}
	}

	/// <summary>
	/// A GeoJSON Feature Object.
	/// </summary>
	/// <remarks>
	/// See https://tools.ietf.org/html/rfc7946#section-3.2
	/// </remarks>
	public class Feature : Feature<IGeometryObject>
	{
		public static Feature<TGeometry, TProps> Create<TGeometry, TProps>(TGeometry geometry, TProps properties, string id = null)
			where TGeometry : IGeometryObject
			=> new Feature<TGeometry, TProps>(geometry, properties, id);

		public static Feature<TGeometry> Create<TGeometry>(TGeometry geometry, IDictionary<string, object> properties, string id = null)
			where TGeometry : IGeometryObject
			=> new Feature<TGeometry>(geometry, properties, id);

		[JsonConstructor]
		public Feature(IGeometryObject geometry, IDictionary<string, object> properties = null, string id = null)
			: base(geometry, properties ?? new Dictionary<string, object>(), id, true) { }
	}
}
