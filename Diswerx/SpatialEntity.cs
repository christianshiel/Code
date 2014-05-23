using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenDis.Dis1998;
using Diswerx.GeoSorting;

namespace Diswerx
{
    /// <summary>
    /// A wrapper for DIS entities so you can run spatial queries on them.
    /// </summary>
    public class SpatialEntity: IQuadObject
    {
        private double easting; public double Easting { get { return easting; } set { easting = value; } }
        private double northing; public double Northing { get { return northing; } set { northing = value; } }
        private ushort id; public ushort Id { get { return id; } set { id = value; } }
        private float width = 0.1f;
        private float height = 0.1f;

        /// <summary>
        /// You must call this everytime the DIS entity updates.
        /// </summary>
        /// <param name="easting">X, Lng, Easting, etc</param>
        /// <param name="northing">Y, Lat, Northing, etc</param>
        /// <param name="width">Width of entity (unlikely to change)</param>
        /// <param name="height">Width of entity (unlikely to change)</param>
        public void Update(double easting, double northing, float width, float height)
        {
            Easting = easting;
            Northing = northing;
            this.width = width;
            this.height = height;
        }

        public System.Windows.Rect Bounds
        {
            get { return new System.Windows.Rect(easting, northing, width, height); }
        }

        public event EventHandler BoundsChanged;
    }
}
