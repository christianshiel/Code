using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using DotNetCoords;
using OpenDis.Dis1998;
using Diswerx.Common;
using Diswerx.Common.Coordinates;

namespace Diswerx
{
    /// <summary>
    /// Coordinate conversion utils
    /// </summary>
    public static class GeoHelpers
    {
        public static bool IsPointInPolygon(PointD[] polygon, PointD point)
        {
            bool isInside = false;
            for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
            {
                if (((polygon[i].Y > point.Y) != (polygon[j].Y > point.Y)) &&
                (point.X < (polygon[j].X - polygon[i].X) *
                (point.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) + polygon[i].X))
                {
                    isInside = !isInside;
                }
            }
            return isInside;
        }

        public static bool IsPointInPolygon(Vector3Double[] polygon, Vector3Double point)
        {
            throw new Exception("Not implemented yet.");
        }

        public static bool IsPointInRect(Vector3Double upperLeft, Vector3Double lowerRight, Vector3Double point)
        {
            throw new Exception("Not implemented yet.");
        }

        public static Vector3Double ToVecECEFRef(LatLng ll)
        {
            Vector3Double v = new Vector3Double();
            ECEFRef ef = ToECEFRef(ll);
            v.X = ef.X;
            v.Y = ef.Y;
            v.Z = ef.Z;
            return v;
        }

        public static ECEFRef ToECEFRef(LatLng ll)
        {            
            ECEFRef ec = new ECEFRef(ll);
            return ec;
        }

        public static LatLng ToLatLng(double x, double y, double z)
        {
            ECEFRef ec = new ECEFRef(x, y, z);
            return ec.ToLatLng();
        }

        public static LatLng ToLatLng(Vector3Double pos)
        {
            ECEFRef ec = new ECEFRef(pos.X,
                                     pos.Y,
                                     pos.Z);
            return ec.ToLatLng();
        }

        public static PointD ToPointDLatLng(double x, double y, double z)
        {
            LatLng latLng = ToLatLng(x, y, z);
            return new PointD(latLng.Latitude, latLng.Longitude);
        }

        public static PointD ToPointDLatLng(Vector3Double pos)
        {
            if (pos.CalculateLength() <= 0f)
                return new PointD(0, 0); // error

            LatLng latLng = ToLatLng(pos);
            return new PointD(latLng.Latitude, latLng.Longitude);
        }
    }
}
