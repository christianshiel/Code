using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Diswerx.SubControls
{
    public class ModelMesh
    {
        Vector3 location; public Vector3 Location { get { return location; } set { location = value; } }
        //Vector3 orientation;
        float heading; public float Heading { get { return heading; } set { heading = value; } }
        Matrix4 modelView; public Matrix4 ModelView { get { return modelView; } set { modelView = value; } }

        Vector3[] vertexData = null; public Vector3[] VertexData { get { return vertexData; } set { vertexData = value; } }
        Vector3[] normalData = null; public Vector3[] NormalData { get { return normalData; } set { normalData = value; } }
        Vector2[] textureData = null; public Vector2[] TextureData { get { return textureData; } set { textureData = value; } }
        uint[] indicesData = null; public uint[] IndicesData { get { return indicesData; } set { indicesData = value; } }

        int textureID; public int TextureID { get { return textureID; } set { textureID = value; } }
        int vertexBufferID;
        int normalBufferID;
        int indicesBufferID;
        int textureBufferID;

        public void GenerateBuffers()
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

            GL.BindTexture(TextureTarget.Texture2D, textureID);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.NormalArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);
            GL.EnableClientState(ArrayCap.IndexArray);

            //GL.MatrixMode(MatrixMode.Modelview);
            //GL.LoadMatrix(ref modelView);
        
            GL.Translate(location);
            GL.Scale(1000f, 1000f, 1000f);
            //GL.Rotate(-180f, Vector3.UnitX);
            //GL.Rotate(heading+180f, Vector3.UnitZ);

            GL.Rotate(heading, Vector3.UnitY);

            if (textureBufferID != 0)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, textureBufferID);
                GL.TexCoordPointer(2, TexCoordPointerType.Float, Vector2.SizeInBytes, IntPtr.Zero);
            }

            if (normalBufferID != 0)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, normalBufferID);
                GL.NormalPointer(NormalPointerType.Float, Vector3.SizeInBytes, IntPtr.Zero);
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

    }
}
