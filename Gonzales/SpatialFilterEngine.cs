using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using OpenTK;

namespace Diswerx.GeoSorting
{
    public class SpatialFilterEngine
    {
        private QuadTree<IQuadObject> quadTree;

        /// <summary>
        /// Manage queries on large set of entities with this.
        /// </summary>
        public SpatialFilterEngine()
        {
            // Use larger min size, and higher min object values for better performance
            quadTree = new QuadTree<IQuadObject>(new Size(25, 25), 0, true);
        }

        /// <summary>
        /// Add an object you would like to observe.
        /// </summary>
        /// <param name="obj">Object must implement IQuadObject interface</param>
        public void AddObject(IQuadObject obj)
        {
            quadTree.Insert(obj);
        }

        /// <summary>
        /// Add a list of objects you would like to observe.
        /// </summary>
        /// <param name="objects"></param>
        public void AddObjects(List<IQuadObject> objects)
        {
            foreach (IQuadObject obj in objects)
            {
                quadTree.Insert(obj);
            }
        }

        /// <summary>
        /// Remove this object from observation.
        /// </summary>
        /// <param name="obj">The spatial object</param>
        public void RemoveObject(IQuadObject obj)
        {
            quadTree.Remove(obj);
        }

        /// <summary>
        /// Clear all objects (not implemented yet).
        /// </summary>
        public void ClearObjects()
        {
            // todo            
        }

        /// <summary>
        /// Do a spatial query.
        /// </summary>
        /// <param name="inArea">Specify the area as a Rectangle</param>
        /// <returns>All objects in the specified Rectangle</returns>
        public List<IQuadObject> FindObjects(Rect inArea)
        {
            return quadTree.Query(inArea);
        }

        /// <summary>
        /// Do a spatial query.
        /// </summary>
        /// <param name="polygon">Specify the area as a polygon</param>
        /// <returns>All objects in the specified polygon</returns>
        public List<IQuadObject> FindObjects(List<Vector2> polygon)
        {
            List<IQuadObject> inPoly = new List<IQuadObject>();

            List<IQuadObject> found;

            Rect rect = Helpers.GetBounds(polygon);

            found = quadTree.Query(rect);

            foreach (IQuadObject obj in found)
            {
                //if (obj is Vector2) // or...equiv
                {
                    Vector2 pt = new Vector2((float)obj.Bounds.X, (float)obj.Bounds.Y);

                    if (Helpers.IsPointInPolygon(polygon, pt))
                    {
                        inPoly.Add(obj);
                    }
                }
            }

            return inPoly;
        }

    }
}

