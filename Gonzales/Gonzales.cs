using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Builders;

namespace GeoSorting
{
    public enum CoordinateSystem
    {
        GCS,
        UTM,
    }

    public struct GeoCoordinate
    {
        public double Easting;
        public double Northing;
        public CoordinateSystem CoordinateSystem;

        public GeoCoordinate(double easting, double northing)
        {
            this.Easting = easting;
            this.Northing = northing;
            CoordinateSystem = CoordinateSystem.GCS;
        }
    }

    public struct GeoArea
    {
        public GeoCoordinate UpperLeft;
        public GeoCoordinate LowerRight;
    }

    public class Gonzales
    {
        private MongoServer server;
        private MongoDatabase database;

        // store a local copy of keys
        public Gonzales()
        {
            //MongoServer[] servers = MongoServer.GetAllServers();
            server = MongoServer.Create();
        }

        public List<IGeoObject> GetNotIntersecting(GeoArea area, string table)
        {

            return null;
        }

        public List<IGeoObject> GetWithin(GeoArea area, string table)
        {
            List<IGeoObject> result = null;
            result = new List<IGeoObject>();

            MongoCollection<BsonDocument> places =
                database.GetCollection<BsonDocument>(table);   
         
            double lowerLeftX = area.UpperLeft.Easting;
            double lowerLeftY = area.LowerRight.Northing;
            double upperRightX = area.LowerRight.Easting;
            double upperRightY = area.UpperLeft.Northing;

            var query = Query.WithinRectangle("loc", lowerLeftX, lowerLeftY, upperRightX, upperRightY);
            var cursor = places.Find(query);

            foreach (var hit in cursor)
            {
                //Console.WriteLine("in circle");
                //foreach (var VARIABLE in hit)
                //{
                //    Console.WriteLine(VARIABLE.Value);

                //}

                //hit.

                //result.Add(
            }

            return result;
        }

        public List<IGeoObject> GetIntersecting(GeoArea area, string table)
        {
            return null;
        }

        public List<IGeoObject> Get(string name, string table)
        {
            return null;
        }

        public void Store(List<IGeoObject> objs, string table)
        {
            if (database != null)
            {
                if (!database.CollectionExists(table))
                {
                    database.CreateCollection(table);

                    MongoCollection<BsonDocument> places =
                                  database.GetCollection<BsonDocument>(table);

                    BsonDocument[] batch = {
                                       new BsonDocument { { "", "" }, { "loc", new BsonArray(new[] { 10, 10 }) } },
                                       new BsonDocument { { "name", "Ayla" }, { "loc", new BsonArray(new[] { 0, 0 }) } }
                    };

                    places.InsertBatch(batch);

                    places.EnsureIndex(IndexKeys.GeoSpatial("loc"));
                }
            }
        }

        public void Store(IGeoObject obj, string table)
        {
            if (database != null)
            {
                if (!database.CollectionExists(table))
                {
                    database.CreateCollection(table);

                    MongoCollection<BsonDocument> places =
                                  database.GetCollection<BsonDocument>(table);

                    BsonDocument[] batch = {
                        new BsonDocument { { "name", obj.Name }, 
                            { "loc", new BsonArray(new[] { obj.GeoCoordinate.Northing, obj.GeoCoordinate.Easting }) } }
                    };

                    places.InsertBatch(batch);

                    places.EnsureIndex(IndexKeys.GeoSpatial("loc"));
                }
            }
        }

        public void Load(string dbName)
        {
            database = server.GetDatabase(dbName);
        }
    }
}
