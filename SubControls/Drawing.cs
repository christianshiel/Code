using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace Diswerx.SubControls
{
    public abstract class Drawing
    {
        public abstract void Draw(float scale);
        public abstract void Translate(int x, int y);
        public abstract void Rotate(float degrees);
        public abstract void Scale(float x, float y);
    }

    public class DrawingBitmap : Drawing
    {
        protected Bitmap bitmap;
        protected int texture = 0;
        protected int x, y;
        protected float size = 1f;
        protected float rotation = 0f;
        protected int height;
        protected int width;

        public DrawingBitmap()
        {
        }

        public DrawingBitmap(int texture, int width, int height)
        {
            this.texture = texture;
            this.width = width;
            this.height = height;            
        }

        public DrawingBitmap(Bitmap bitmap)
        {            
            texture = Helpers.LoadTexture(bitmap, texture);
            this.bitmap = bitmap;
            height = bitmap.Height;
            width = bitmap.Width;
        }

        public override void Scale(float x, float y)
        {
            this.size = (x + y) / 2f;
        }

        public override void Rotate(float degrees)
        {
            this.rotation = degrees;
        }

        public override void Translate(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override void Draw(float scale)
        {
            GL.PushMatrix();

            GL.Translate(x, y, 0);  

            GL.Scale(1f / scale, 1f / scale, 1f);

            GL.Rotate(rotation, 0, 0, 1);

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            GL.BindTexture(TextureTarget.Texture2D, texture);            

            GL.Begin(BeginMode.Quads);

            GL.TexCoord2(0.0f, 0.0f);
            GL.Vertex2(-(width / 2) * size, -(height / 2) * size);

            GL.TexCoord2(1.0f, 0.0f);
            GL.Vertex2((width / 2) * size, -(height / 2) * size);

            GL.TexCoord2(1.0f, 1.0f);
            GL.Vertex2((width / 2) * size, (height / 2) * size);

            GL.TexCoord2(0.0f, 1.0f);
            GL.Vertex2(-(width / 2) * size, (height / 2) * size);

            GL.End();

            GL.PopMatrix();
        }
    }

    public class DrawingVector : Drawing
    {
        protected int texture = 0;
        protected int x, y;
        protected float size = 1f;
        protected float rotation = 0f;
        protected List<Point> points;
        protected float thickness = 1f;
        protected Color colour;

        public DrawingVector(List<Point> points, Color colour, float thickness)
        {
            this.points = points;
            this.colour = colour;
            this.thickness = thickness;            
        }

        public override void Draw(float scale)
        {
            GL.PushMatrix();

            GL.Disable(EnableCap.Texture2D);
            GL.LineWidth(thickness);
            GL.Color3(colour);

            GL.Translate(x, y, 0);

            //GL.Scale(1f / scale, 1f / scale, 1f);

            GL.Rotate(rotation, 0, 0, 1);

            GL.Begin(BeginMode.LineStrip);

            foreach (Point p in points)
            {
                GL.Vertex2(p.X, p.Y);
            }

            GL.End();

            GL.PopMatrix();           
        }

        public override void Scale(float x, float y)
        {
            this.size = (x + y) / 2f;
        }

        public override void Rotate(float degrees)
        {
            this.rotation = degrees;
        }

        public override void Translate(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public class DrawingText :  DrawingBitmap
    {
        public DrawingText(string text, Font font, int width, int height, int x, int y) : base ()
        {
            bitmap = new Bitmap(width, height);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.Transparent);
                g.DrawString(text, font, Brushes.Black, new PointF(x, y));
            }

            texture = Helpers.LoadTexture(bitmap, texture);
        }

    }

}
