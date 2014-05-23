using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diswerx.Common.Coordinates
{
    public enum CoordinateSystem
    {
        GCS,
        UTM,
    }

    public struct PointD
    {
        public double X;
        public double Y;

        public PointD(double x, double y)
        {
            X = x;
            Y = y;
        }
    }

    public struct GeoCoordinate
    {
        public double Easting;
        public double Northing;
        public CoordinateSystem CoordinateSystem;

        /// <summary>
        /// A general coordinate for representing Spherical (no height) or Flat Earth + projection.
        /// </summary>
        /// <param name="easting">Easting/Longitude</param>
        /// <param name="northing">Northing/Latitude</param>
        public GeoCoordinate(double easting, double northing)
        {
            this.Easting = easting;
            this.Northing = northing;
            CoordinateSystem = CoordinateSystem.GCS;
        }
    }
}
