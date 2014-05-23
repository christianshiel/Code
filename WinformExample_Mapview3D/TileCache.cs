using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diswerx.SubControls;
using Diswerx.GeoSorting;
using System.IO;
using OpenTK;

namespace WinformExample_Mapview3D
{
    public class TileCache
    {
        //private List<TerrainMesh> meshes; public List<TerrainMesh> Meshes { get { return meshes; } set { meshes = value; } }

        SpatialFilterEngine sqe;

        public TileCache()
        {
            //meshes = new List<TerrainMesh>();
            sqe = new SpatialFilterEngine();            
        }

        public void AddMesh(TerrainMesh mesh, string meshId)
        {
            FileStream fs = new FileStream(@"C:\Diswerx\Data\Cache\" + meshId + ".msh", FileMode.Create);

            BinaryWriter w = new BinaryWriter(fs);

            //w.Write("QuadCount=" + mesh.QuadCount.ToString());
            w.Write(mesh.QuadCount);

            foreach (Vector3 v in mesh.VertexData)
            {
                w.Write(v.X);
                w.Write(v.Y);
                w.Write(v.Z);
            }

            foreach (Vector3 v in mesh.NormalData)
            {
                w.Write(v.X);
                w.Write(v.Y);
                w.Write(v.Z);
            }

            foreach (Vector2 v in mesh.TextureData)
            {
                w.Write(v.X);
                w.Write(v.Y);                
            }

            foreach (uint i in mesh.IndicesData)
            {
                w.Write(i);
            }

            w.Close();

            fs.Close();
        }

        //public Texture

        public TerrainMesh LoadMesh(string meshId)
        {
            TerrainMesh mesh = new TerrainMesh();

            FileStream fs = new FileStream(@"C:\Diswerx\Data\Cache\" + meshId + ".msh", 
                                            FileMode.Open, FileAccess.Read, FileShare.Read, 1);
            
            BinaryReader r = new BinaryReader(fs);

            //string quadCount = r.ReadString();
            //int count = Int32.Parse(quadCount.Split('=')[1]);
            int count = r.ReadInt32();

            int numVerts = count * 4;
            int numNorms = count * 4;
            int numUVs = count * 4;
            int numInds = count * 6;

            mesh.VertexData = new Vector3[numVerts];
            mesh.NormalData = new Vector3[numNorms];
            mesh.TextureData = new Vector2[numUVs];
            mesh.IndicesData = new uint[numInds];

            for (int i = 0; i < numVerts; i++)
            {
                float x = r.ReadSingle();
                float y = r.ReadSingle();
                float z = r.ReadSingle();
                Vector3 v = new Vector3(x, y, z);
                mesh.VertexData[i] = v;
            }

            for (int i = 0; i < numNorms; i++)
            {
                float x = r.ReadSingle();
                float y = r.ReadSingle();
                float z = r.ReadSingle();
                Vector3 v = new Vector3(x, y, z);
                mesh.NormalData[i] = v;
            }

            for (int i = 0; i < numUVs; i++)
            {
                float x = r.ReadSingle();
                float y = r.ReadSingle();
                Vector2 v = new Vector2(x, y);
                mesh.TextureData[i] = v;
            }

            for (int i = 0; i < numInds; i++)
            {
                uint ind = r.ReadUInt32();
                mesh.IndicesData[i] = ind;
            }

            r.Close();

            fs.Close();

            return mesh;

        }

        public TerrainMesh GetMesh(double x, double y)
        {
            return null;
        }

        public void OptimizeCache()
        {
            //foreach (TerrainMesh m in meshes)
            //{
            //    sqe.AddObject(m);
            //}
        }
        

        
    }
}
