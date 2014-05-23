using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using Diswerx.GeoSorting;
using Diswerx.Common;
using System.IO;

namespace Diswerx.SubControls
{
    public class TerrainMesh : IDraw, IQuadObject
    {
        private Vector3 location; public Vector3 Location { get { return location; } set { location = value; } }
        public Matrix4 translation;
        public Quaternion rotation;
        public Matrix4 scale;
        private int texture; public int Texture { get { return texture; } set { texture = value; } }

        //private List<IQuadObject> quads; public List<IQuadObject> Quads { get { return quads; } }

        private int quadCount; public int QuadCount { get { return quadCount; } }

        private int width = 0; public int Width { get { return width; } }
        private int height = 0; public int Height { get { return height; } }
        private float x_res = 1f;
        private float y_res = 1f;
        private int[,] heightData;

        private int tex_width;
        private int tex_height;
        private float tex_x_res;
        private float tex_y_res;

        private int xQuads = 0;
        private int yQuads = 0;
        private float[] quad = { 0, 0, 0, 0 };

        private Vector3 v1, v2, v3, v4;

        private Vector3[] vertexData = null; public Vector3[] VertexData { get { return vertexData; } set { vertexData = value; } }
        private Vector3[] normalData = null; public Vector3[] NormalData { get { return normalData; } set { normalData = value; } }
        private Vector2[] textureData = null; public Vector2[] TextureData { get { return textureData; } set { textureData = value; } }
        private uint[] indicesData = null; public uint[] IndicesData { get { return indicesData; } set { indicesData = value; } }

        // Variables for the graphics card handles
        int vertexBufferID;
        int normalBufferID;
        int indicesBufferID;
        int textureBufferID;

        public float GetExactHeightAt(float x, float y)
        {
            if (heightData == null) return 0f;

            float h = 0f;
            int max_x = heightData.GetLength(0);
            int max_y = heightData.GetLength(1);
           
            int x_lower = (int)x;
            int x_higher = x_lower + 1;
            int z_lower = (int)y;
            int z_higher = z_lower + 1;

            if (x_lower < 0 || x_higher < 0 || z_higher < 0 || z_lower < 0 ||
                x_lower > max_x || x_higher > max_x || z_higher > max_y || z_lower > max_y)
                return 10;

            int a = heightData[x_lower, z_lower];
            int b = heightData[x_lower, z_higher];
            int c = heightData[x_higher, z_lower];
            int d = heightData[x_higher, z_higher];

            //bool pointAboveLowerTriangle = (xRelative - zRelative < 1);

            // not accurate but I don't care if all I want is a sodding world coordinate
            h = (float)(a + b + c + d) / 4f;

            //if (h <= 0f)
             //   h = 10f;

            return h;
        }

        public TerrainMesh()
        {
        }

        public TerrainMesh(float x_res, float y_res, int[,] heightData, 
                           int texture, int tex_width, int tex_height, 
                           float tex_x_res, float tex_y_res, bool tile,
                           double location_x, double location_y)
        {
            //quads = new List<IQuadObject>();

            width = heightData.GetLength(0);
            height = heightData.GetLength(1);

            this.heightData = heightData;
            this.texture = texture;

            this.x_res = x_res;
            this.y_res = y_res;

            xQuads = width - 1;
            yQuads = height - 1;

            this.tex_width = tex_width;
            this.tex_height = tex_height;

            this.tex_x_res = tex_x_res;
            this.tex_y_res = tex_y_res;

            v1 = new Vector3(); v2 = new Vector3();
            v3 = new Vector3(); v4 = new Vector3();

            GenerateMesh(tile);
            //GenerateBuffers();

            location.X = (float)location_x;
            location.Y = (float)location_y;
            rect = new System.Windows.Rect(location_x, location_y, 
                                           x_res * width, y_res * height);
        }

        private float[] GetQuad(int x, int y)
        {
            quad[0] = (float)heightData[x, y];
            quad[1] = (float)heightData[x, y + 1];
            quad[2] = (float)heightData[x + 1, y];
            quad[3] = (float)heightData[x + 1, y + 1];

            return quad;
        }

        //public void SaveMesh(Stream stream)
        //{
        //}

        //public void SaveMesh(string filename)
        //{
        //}

        //public void LoadMesh(Stream stream)
        //{
        //}

        private void GenerateMesh(bool tiledTexture = true)
        {            
            int numQuads = xQuads * yQuads;

            quadCount = numQuads;

            vertexData = new Vector3[numQuads * 4];
            normalData = new Vector3[numQuads * 4];
            textureData = new Vector2[numQuads * 4];
            indicesData = new uint[numQuads * 6];

            float u = 0;
            float v = 0;
            float uInc = (1f / (float)xQuads);
            float vInc = (1f / (float)yQuads);

            int vertIndex = 0;
            int texIndex = 0;
            uint indIndex = 0;
            int normIndex = 0;

            uint indInc = 0;

            // Loop thru the quads
            for (int k = 0; k < yQuads; k++)
            {
                for (int i = 0; i < xQuads; i++)
                {
                    float x = (i * x_res);
                    float y = (k * y_res);

                    float[] quad = GetQuad(i, k);

                    // Create vertices
                    {
                        v1.X = x; 
                        v1.Z = y; 
                        v1.Y = quad[0];
                    }
                    {
                        v2.X = x; 
                        v2.Z = y_res + y; 
                        v2.Y = quad[1];
                    }
                    {
                        v3.X = x_res + x; 
                        v3.Z = y; 
                        v3.Y = quad[2];
                    }
                    {
                        v4.X = x_res + x; 
                        v4.Z = y_res + y; 
                        v4.Y = quad[3];
                    }

                    vertexData[vertIndex + 0] = v1;
                    vertexData[vertIndex + 1] = v2;
                    vertexData[vertIndex + 2] = v3;
                    vertexData[vertIndex + 3] = v4;

                    vertIndex += 4;

                    // 1 3
                    // 2 4

                    // Create normals
                    Vector3 norm1 = Vector3.Cross(v2 - v1, v3 - v1);
                    norm1.Normalize();
                    Vector3 norm2 = Vector3.Cross(v4 - v2, v1 - v2);
                    norm2.Normalize();
                    Vector3 norm3 = Vector3.Cross(v1 - v3, v4 - v3);
                    norm3.Normalize();
                    Vector3 norm4 = Vector3.Cross(v3 - v4, v2 - v4);
                    norm4.Normalize();

                    normalData[normIndex + 0] = norm1;
                    normalData[normIndex + 1] = norm2;
                    normalData[normIndex + 2] = norm3;
                    normalData[normIndex + 3] = norm4;

                    normIndex += 4;

                    // Create texture coords

                    u = (float)(i) * uInc;
                    v = (float)(k) * vInc;

                    if (!tiledTexture)
                    {
                        textureData[texIndex + 0].X = u;
                        textureData[texIndex + 0].Y = v;

                        textureData[texIndex + 1].X = u;
                        textureData[texIndex + 1].Y = v + vInc;

                        textureData[texIndex + 2].X = u + uInc;
                        textureData[texIndex + 2].Y = v;

                        textureData[texIndex + 3].X = u + uInc;
                        textureData[texIndex + 3].Y = v + vInc;
                    }
                    else //if (tiledTexture)
                    {
                        textureData[texIndex + 0].X = 0;
                        textureData[texIndex + 0].Y = 0;

                        textureData[texIndex + 1].X = 0;
                        textureData[texIndex + 1].Y = 1;

                        textureData[texIndex + 2].X = 1;
                        textureData[texIndex + 2].Y = 0;

                        textureData[texIndex + 3].X = 1;
                        textureData[texIndex + 3].Y = 1;
                    }

                    texIndex += 4;

                    // Create indices
                    indicesData[indIndex + 0] = 0 + (indInc);
                    indicesData[indIndex + 1] = 1 + (indInc);
                    indicesData[indIndex + 2] = 2 + (indInc);

                    indicesData[indIndex + 3] = 2 + (indInc);
                    indicesData[indIndex + 4] = 1 + (indInc);
                    indicesData[indIndex + 5] = 3 + (indInc);

                    indIndex += 6;

                    indInc += 4;
                }
            }
        }

        public void LoadBuffers()
        {
            if (vertexData != null)
            {
                GL.GenBuffers(1, out vertexBufferID);
                GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferID);
                GL.BufferData<Vector3>(BufferTarget.ArrayBuffer,
                                       new IntPtr(vertexData.Length * Vector3.SizeInBytes),
                                       vertexData, BufferUsageHint.StaticDraw);
            }

            if (normalData != null)
            {
                GL.GenBuffers(1, out normalBufferID);
                GL.BindBuffer(BufferTarget.ArrayBuffer, normalBufferID);
                GL.BufferData<Vector3>(BufferTarget.ArrayBuffer,
                                       new IntPtr(normalData.Length * Vector3.SizeInBytes),
                                       normalData, BufferUsageHint.StaticDraw);
            }

            if (textureData != null)
            {
                GL.GenBuffers(1, out textureBufferID);
                GL.BindBuffer(BufferTarget.ArrayBuffer, textureBufferID);
                GL.BufferData<Vector2>(BufferTarget.ArrayBuffer,
                                    new IntPtr(textureData.Length * Vector2.SizeInBytes),
                                    textureData, BufferUsageHint.StaticDraw);
            } 

            if (indicesData != null)
            {
                GL.GenBuffers(1, out indicesBufferID);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, indicesBufferID);
                GL.BufferData<uint>(BufferTarget.ElementArrayBuffer,
                                    new IntPtr(indicesData.Length * sizeof(uint)),
                                    indicesData, BufferUsageHint.StaticDraw);
            }

            GL.InterleavedArrays(InterleavedArrayFormat.T2fN3fV3f, 0, IntPtr.Zero);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

        }

        public void Draw()
        {
            DrawBuffers();
        }

        private void DrawBuffers()
        {
            GL.PushMatrix();

            GL.Translate(location);

            GL.BindTexture(TextureTarget.Texture2D, texture);  

            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.NormalArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);            
            GL.EnableClientState(ArrayCap.IndexArray);

            if (textureBufferID != 0)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, textureBufferID);
                GL.TexCoordPointer(2, TexCoordPointerType.Float, Vector2.SizeInBytes, IntPtr.Zero);
            }
            
            if (normalBufferID != 0)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, normalBufferID);
                GL.NormalPointer(NormalPointerType.Float, Vector3.SizeInBytes, IntPtr.Zero);

                // Debug
                //GL.PushMatrix();

                //GL.LineWidth(1f);

                //GL.Begin(BeginMode.Lines);

                //for (int i = 0; i < vertexData.Length; i++)
                //{
                //    if (i % 4 == 0)
                //        GL.Color4(Color.Yellow);
                //    else if (i % 3 == 0)
                //        GL.Color4(Color.Green);
                //    else if (i % 2 == 0)
                //        GL.Color4(Color.Red);
                //    else
                //        GL.Color4(Color.Pink);

                //    GL.PushMatrix();
                //    Vector3 v = vertexData[i];
                //    Vector3 p1 = v;
                //    Vector3 p2 = v + (normalData[i] * 500f);
                //    GL.Vertex3(p1);
                //    GL.Vertex3(p2);
                //    GL.PopMatrix();
                //}

                //GL.Color3(Color.White);
                //GL.End();

                //GL.PopMatrix();
                //
            }

            if (vertexBufferID != 0)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferID);
                GL.VertexPointer(3, VertexPointerType.Float, Vector3.SizeInBytes, IntPtr.Zero);
            }

            if (indicesBufferID != 0)
            {
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, indicesBufferID);
                GL.DrawElements(BeginMode.Triangles, indicesData.Length, DrawElementsType.UnsignedInt, 0);
            }

            GL.PopMatrix();
        }

        #region IQuadObject

        private System.Windows.Rect rect;
  
        public System.Windows.Rect Bounds
        {
            get { return rect; }
        }

        public event EventHandler BoundsChanged;

        #endregion
    }
}
