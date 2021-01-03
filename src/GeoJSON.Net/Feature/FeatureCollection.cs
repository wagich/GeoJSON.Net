using GeoJSON.Net.Converters;
using GeoJSON.Net.Geometry;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoJSON.Net.Feature
{
    /// <summary>
    /// Defines the FeatureCollection type.
    /// </summary>
    public class FeatureCollection<TProps> : GeoJSONObject, IEqualityComparer<FeatureCollection<TProps>>, IEquatable<FeatureCollection<TProps>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureCollection" /> class.
        /// </summary>
        public FeatureCollection() : this(new List<IFeature<IGeometryObject, TProps>>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureCollection" /> class.
        /// </summary>
        /// <param name="features">The features.</param>
        public FeatureCollection(IEnumerable<IFeature<IGeometryObject, TProps>> features)
        {
            if (features == null)
            {
                throw new ArgumentNullException(nameof(features));
            }

            Features = features.ToList();
        }

        public override GeoJSONObjectType Type => GeoJSONObjectType.FeatureCollection;

        /// <summary>
        /// Gets the features.
        /// </summary>
        /// <value>The features.</value>
        [JsonProperty(PropertyName = "features", Required = Required.Always, ItemConverterType = typeof(FeatureConverter))]
        public List<IFeature<IGeometryObject, TProps>> Features { get; private set; }

        #region IEqualityComparer, IEquatable

        /// <summary>
        /// Determines whether the specified object is equal to the current object
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(this, obj as FeatureCollection<TProps>);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object
        /// </summary>
        public bool Equals(FeatureCollection<TProps> other)
        {
            return Equals(this, other);
        }

        /// <summary>
        /// Determines whether the specified object instances are considered equal
        /// </summary>
        public bool Equals(FeatureCollection<TProps> left, FeatureCollection<TProps> right)
        {
            if (base.Equals(left, right))
            {
                return left.Features.SequenceEqual(right.Features);
            }
            return false;
        }

        /// <summary>
        /// Determines whether the specified object instances are considered equal
        /// </summary>
        public static bool operator ==(FeatureCollection<TProps> left, FeatureCollection<TProps> right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }
            if (ReferenceEquals(null, right))
            {
                return false;
            }
            return left != null && left.Equals(right);
        }

        /// <summary>
        /// Determines whether the specified object instances are not considered equal
        /// </summary>
        public static bool operator !=(FeatureCollection<TProps> left, FeatureCollection<TProps> right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Returns the hash code for this instance
        /// </summary>
        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            foreach (var feature in Features)
            {
                hash = (hash * 397) ^ feature.GetHashCode();
            }
            return hash;
        }

        /// <summary>
        /// Returns the hash code for the specified object
        /// </summary>
        public int GetHashCode(FeatureCollection<TProps> other)
        {
            return other.GetHashCode();
        }

        #endregion
    }

    /// <summary>
    /// Defines the FeatureCollection type.
    /// </summary>
    public class FeatureCollection : FeatureCollection<IDictionary<string, object>>
	{
        public static FeatureCollection<TProps> Create<TProps>(IEnumerable<IFeature<IGeometryObject, TProps>> features)
            => new FeatureCollection<TProps>(features);
        public static FeatureCollection<TProps> Create<TProps>(params IFeature<IGeometryObject, TProps>[] features)
            => new FeatureCollection<TProps>(features);

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureCollection" /> class.
        /// </summary>
        public FeatureCollection() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureCollection" /> class.
        /// </summary>
        /// <param name="features">The features.</param>
        public FeatureCollection(IEnumerable<IFeature<IGeometryObject, IDictionary<string, object>>> features) : base(features) { }
    }
}
