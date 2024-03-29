﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using Diswerx.Common.Coordinates;

namespace Diswerx.Shapefile
{
    public class Shapefile
    {
        int filecode, filelength, version, shapetype;
        double xMin, yMin, xMax, yMax, zMin, zMax, mMin, mMax;
        int x1, y1, x2, y2;
        int offsetX = 0;
        int offsetY = 0;
        bool down = false;

        public List<PointD> points;
        public struct Line
        {
            public double[] box;
            public int numParts;
            public int numPoints;
            public int[] parts;
            public PointD[] points;
        }
        public List<Line> lines;
        public struct Polygon
        {
            public double[] box;
            public int numParts;
            public int numPoints;
            public int[] parts;
            public PointD[] points;
        }
        public List<Polygon> polygons;

        public Shapefile()
        {
            points = new List<PointD>();
            lines = new List<Line>();
            polygons = new List<Polygon>();
        }

        public void ReadShapeFile(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Open);
            long fileLength = fs.Length;
            Byte[] data = new Byte[fileLength];
            fs.Read(data, 0, (int)fileLength);
            fs.Close();
            filecode = ReadIntBig(data, 0);
            filelength = ReadIntBig(data, 24);
            version = ReadIntLittle(data, 28);
            shapetype = ReadIntLittle(data, 32);
            xMin = ReadDoubleLittle(data, 36);
            yMin = ReadDoubleLittle(data, 44);
            yMin = 0 - yMin;
            xMax = ReadDoubleLittle(data, 52);
            yMax = ReadDoubleLittle(data, 60);
            yMax = 0 - yMax;
            zMin = ReadDoubleLittle(data, 68);
            zMax = ReadDoubleLittle(data, 76);
            mMin = ReadDoubleLittle(data, 84);
            mMax = ReadDoubleLittle(data, 92);
            int currentPosition = 100;
            while (currentPosition < fileLength)
            {
                int recordStart = currentPosition;
                int recordNumber = ReadIntBig(data, recordStart);
                int contentLength = ReadIntBig(data, recordStart + 4);
                int recordContentStart = recordStart + 8;
                if (shapetype == 1)
                {
                    PointD point = new PointD();
                    int recordShapeType = ReadIntLittle(data, recordContentStart);
                    point.X = (float)ReadDoubleLittle(data, recordContentStart + 4);
                    point.Y = 0 - (float)ReadDoubleLittle(data, recordContentStart + 12);
                    points.Add(point);
                }
                if (shapetype == 3)
                {
                    Line line = new Line();
                    int recordShapeType = ReadIntLittle(data, recordContentStart);
                    line.box = new Double[4];
                    line.box[0] = ReadDoubleLittle(data, recordContentStart + 4);
                    line.box[1] = ReadDoubleLittle(data, recordContentStart + 12);
                    line.box[2] = ReadDoubleLittle(data, recordContentStart + 20);
                    line.box[3] = ReadDoubleLittle(data, recordContentStart + 28);
                    line.numParts = ReadIntLittle(data, recordContentStart + 36);
                    line.parts = new int[line.numParts];
                    line.numPoints = ReadIntLittle(data, recordContentStart + 40);
                    line.points = new PointD[line.numPoints];
                    int partStart = recordContentStart + 44;
                    for (int i = 0; i < line.numParts; i++)
                    {
                        line.parts[i] = ReadIntLittle(data, partStart + i * 4);
                    }
                    int pointStart = recordContentStart + 44 + 4 * line.numParts;
                    for (int i = 0; i < line.numPoints; i++)
                    {
                        line.points[i].X = (float)ReadDoubleLittle(data, pointStart + (i * 16));
                        line.points[i].Y = (float)ReadDoubleLittle(data, pointStart + (i * 16) + 8);
                        line.points[i].Y = 0 - line.points[i].Y;
                    }
                    lines.Add(line);
                }
                if (shapetype == 5)
                {
                    Polygon polygon = new Polygon();
                    int recordShapeType = ReadIntLittle(data, recordContentStart);
                    polygon.box = new Double[4];
                    polygon.box[0] = ReadDoubleLittle(data, recordContentStart + 4);
                    polygon.box[1] = ReadDoubleLittle(data, recordContentStart + 12);
                    polygon.box[2] = ReadDoubleLittle(data, recordContentStart + 20);
                    polygon.box[3] = ReadDoubleLittle(data, recordContentStart + 28);
                    polygon.numParts = ReadIntLittle(data, recordContentStart + 36);
                    polygon.parts = new int[polygon.numParts];
                    polygon.numPoints = ReadIntLittle(data, recordContentStart + 40);
                    polygon.points = new PointD[polygon.numPoints];
                    int partStart = recordContentStart + 44;
                    for (int i = 0; i < polygon.numParts; i++)
                    {
                        polygon.parts[i] = ReadIntLittle(data, partStart + i * 4);
                    }
                    int pointStart = recordContentStart + 44 + 4 * polygon.numParts;
                    for (int i = 0; i < polygon.numPoints; i++)
                    {
                        polygon.points[i].X = (float)ReadDoubleLittle(data, pointStart + (i * 16));
                        polygon.points[i].Y = (float)ReadDoubleLittle(data, pointStart + (i * 16) + 8);
                        polygon.points[i].Y = 0 - polygon.points[i].Y;
                    }
                    polygons.Add(polygon);
                }
                currentPosition = recordStart + (4 + contentLength) * 2;
            }
        }

        public int ReadIntBig(byte[] data, int pos)
        {
            byte[] bytes = new byte[4];
            bytes[0] = data[pos];
            bytes[1] = data[pos + 1];
            bytes[2] = data[pos + 2];
            bytes[3] = data[pos + 3];
            Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        public int ReadIntLittle(byte[] data, int pos)
        {
            byte[] bytes = new byte[4];
            bytes[0] = data[pos];
            bytes[1] = data[pos + 1];
            bytes[2] = data[pos + 2];
            bytes[3] = data[pos + 3];
            return BitConverter.ToInt32(bytes, 0);
        }

        public double ReadDoubleLittle(byte[] data, int pos)
        {
            byte[] bytes = new byte[8];
            bytes[0] = data[pos];
            bytes[1] = data[pos + 1];
            bytes[2] = data[pos + 2];
            bytes[3] = data[pos + 3];
            bytes[4] = data[pos + 4];
            bytes[5] = data[pos + 5];
            bytes[6] = data[pos + 6];
            bytes[7] = data[pos + 7];
            return BitConverter.ToDouble(bytes, 0);
        }
    }
}
