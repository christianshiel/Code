using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using Assimp.Configs;
using Assimp;
using Paloma;
using Diswerx.Common;
using Diswerx.GeoSorting;

namespace Diswerx.SubControls
{
    public class Model : IDraw
    {
        Vector3 location; public Vector3 Location { get { return location; } }
        Scene scene;
        List<ModelMesh> meshes; public List<ModelMesh> Meshes { get { return meshes; } }
        string path;
        //float scale = 10f;
        float yRotation = 0f;

        public void Load(string path, string filename)
        {
            AssimpImporter importer = new AssimpImporter();

            this.path = path;
            meshes = new List<ModelMesh>();

            scene = importer.ImportFile(path + filename, PostProcessPreset.TargetRealTimeMaximumQuality);             
            
            importer.Dispose();
            
            for (int i = 0; i < scene.Meshes.Length; i++)
            {                
                ModelMesh mm = Build(scene, i);
                meshes.Add(mm);
            }            
        }

        public void Translate(float x, float y, float z)
        {
            location = new Vector3(x, y, z);

            if (meshes == null) return;

            foreach (ModelMesh mm in meshes)
            {
                //Matrix4 t = Matrix4.CreateTranslation(location);
                //mm.ModelView *= t;
                mm.Location = location;
            }
        }

        public void RotateY(float y)
        {
            yRotation = y;

            foreach (ModelMesh mm in meshes)
            {
                mm.Heading = y;
            }
        }

        public void Copy(Model model)
        {
            //meshes = new List<ModelMesh>(model.Meshes);
            meshes = new List<ModelMesh>();

            for (int i = 0; i < model.Meshes.Count; i++)
            {
                ModelMesh mm = Build(model.Meshes[i].VertexData, model.Meshes[i].NormalData,
                                        model.Meshes[i].TextureData, model.Meshes[i].IndicesData);

                mm.TextureID = model.Meshes[i].TextureID;
                mm.GenerateBuffers();

                meshes.Add(mm);
            }
        }

        public ModelMesh Build(Vector3[] verts, Vector3[] norms, Vector2[] texCoords, uint[] indices)
        {            
            ModelMesh mm = new ModelMesh();

            mm.VertexData = new Vector3[verts.Length];
            for (int i = 0; i < verts.Length; i++)
            {
                Vector3D v = new Vector3D(verts[i].X, verts[i].Y, verts[i].Z);
                mm.VertexData[i] = new Vector3(v.X, v.Y, v.Z);
            }

            mm.NormalData = new Vector3[norms.Length];
            for (int i = 0; i < norms.Length; i++)
            {
                Vector3D v = new Vector3D(norms[i].X, norms[i].Y, norms[i].Z);
                mm.NormalData[i] = new Vector3(v.X, v.Y, v.Z);
            }

            mm.TextureData = new Vector2[texCoords.Length];
            for (int i = 0; i < texCoords.Length; i++)
            {
                Vector3D v = new Vector3D(texCoords[i].X, texCoords[i].Y, 0);
                mm.TextureData[i] = new Vector2(v.X, v.Y); // nb
            }

            mm.IndicesData = new uint[indices.Length];
            for (int i = 0; i < indices.Length; i++)
            {
                uint v = indices[i];
                mm.IndicesData[i] = v;
            }

            return mm;
        }

        public ModelMesh Build(Scene scene, int meshIndex)                   
        {
            ModelMesh mm = new ModelMesh();
            Mesh m = scene.Meshes[meshIndex];

            Vector3D[] verts = m.Vertices;
            mm.VertexData = new Vector3[verts.Length];
            for (int i = 0; i < verts.Length; i++)
            {
                Vector3D v = verts[i];
                mm.VertexData[i] = new Vector3(v.X, v.Y, v.Z);
            }

            Vector3D[] norms = m.Normals;
            mm.NormalData = new Vector3[norms.Length];
            for (int i = 0; i < norms.Length; i++)
            {
                Vector3D v = norms[i];
                mm.NormalData[i] = new Vector3(v.X, v.Y, v.Z);
            }

            Vector3D[] texCoords = m.GetTextureCoords(0);
            mm.TextureData = new Vector2[texCoords.Length];
            for (int i = 0; i < texCoords.Length; i++)
            {
                Vector3D v = texCoords[i];
                mm.TextureData[i] = new Vector2(v.X, 1f - v.Y); // nb
            }

            uint[] indices = m.GetIndices();
            mm.IndicesData = new uint[indices.Length];
            for (int i = 0; i < indices.Length; i++)
            {
                uint v = indices[i];
                mm.IndicesData[i] = v;
            }

            //mm = Build(m.Vertices, m.Normals, m.GetTextureCoords(0), m.GetIndices());

            int matIndex = m.MaterialIndex;

            TextureSlot[] ts = scene.Materials[matIndex].GetAllTextures();

            try
            {
                if (ts.Length > 0)
                {
                    string imgPath = path + ts[0].FilePath;

                    if (imgPath.ToUpper().Contains(".TGA"))
                    {
                        Bitmap b = (Bitmap)Paloma.TargaImage.LoadTargaImage(imgPath);
                        mm.TextureID = Diswerx.SubControls.Helpers.LoadTexture(b, mm.TextureID);
                    }
                    else
                    {
                        Bitmap b = (Bitmap)Bitmap.FromFile(imgPath);
                        mm.TextureID = Diswerx.SubControls.Helpers.LoadTexture(b, mm.TextureID);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Texture not loaded.");
            }

            mm.GenerateBuffers();

            return mm;
        }

        public void Draw()
        {
            foreach (ModelMesh mm in meshes)
            {
                mm.Draw();
            }
        }

        public List<IQuadObject> GetQuads()
        {
            //throw new NotImplementedException();
            return null;
        }
    }
}
