using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace Diswerx.DemRender
{
    public struct DataPoint
    {
        public int Elevation; // int?
        //public double Easting;
        //public double Northing;
        //public double Latitude;
        //public double Longitude;
    }

    public struct DataKey
    {
        public int Col;
        public int Row;
    }

    public class DemModel
    {
        private string filename;
        private Dictionary<DataKey, List<DataPoint>> elevations;
        private int[,] elevationGrid;
        private bool makeSquare = true;
        private double xSampling = 0; public double XSampling { get { return xSampling; } }
        private double ySampling = 0; public double YSampling { get { return ySampling; } }

        public Dictionary<DataKey, List<DataPoint>> Elevations
        {
            get { return elevations; }
        }

        public List<DataKey> GetKeys()
        {
            return elevations.Keys.ToList<DataKey>();
        }

        public int GetGridWidth()
        {
            return ElevationGrid.GetLength(0);
        }

        public int GetGridHeight()
        {
            return ElevationGrid.GetLength(1);
        }

        public int[,] ElevationGrid
        {
            get { return elevationGrid; }
        }

        // Only cater to downsizing for now
        public void PadElevations(int left, int right, int top, int bottom)
        {
            ResizeArray(ref elevationGrid, left, right, top, bottom);

            // recalculate origins
            int xdim = GetGridWidth();
            int ydim = GetGridHeight();

            eastingNW -= left * xSampling;
            eastingSW -= left * xSampling;
            eastingNE += right * xSampling;
            eastingSE += right * xSampling;

            northingNE += top * ySampling;
            northingNW += top * ySampling;
            northingSE -= bottom * ySampling;
            northingSW -= bottom * ySampling;
        }

        private static void ResizeArray<T>(
            ref T[,] array, int padLeft, int padRight, int padTop, int padBottom)
        {
            int ow = array.GetLength(0);
            int oh = array.GetLength(1);
            int nw = ow + padLeft + padRight;
            int nh = oh + padTop + padBottom;

            int x0 = padLeft;
            int y0 = padTop;
            int x1 = x0 + ow - 1;
            int y1 = y0 + oh - 1;
            int u0 = -x0;
            int v0 = -y0;

            if (x0 < 0) x0 = 0;
            if (y0 < 0) y0 = 0;
            if (x1 >= nw) x1 = nw - 1;
            if (y1 >= nh) y1 = nh - 1;

            T[,] nArr = new T[nw, nh];
            for (int y = y0; y <= y1; y++)
            {
                for (int x = x0; x <= x1; x++)
                {
                    nArr[x, y] = array[u0 + x, v0 + y];
                }
            }
            array = nArr;
        }

        public float[,] ConvertGridToFloats(int res = 1)
        {
            int max = elevationGrid.Cast<int>().Max();
            int min = elevationGrid.Cast<int>().Min();

            float[,] elevs = new float[GetGridHeight(), GetGridWidth()];
            float range = (float)(max - min);

            for (int y = 0; y < GetGridHeight(); y++)
            {
                for (int x = 0; x < GetGridWidth(); x++)
                {
                    float norm = ((float)ElevationGrid[x, y] - (float)min) / range;

                    //elevs[GetGridHeight()-1 - y, x] = norm;
                    elevs[y, x] = norm;
                }
            }

            return elevs;
        }

        public DemModel(string filename, bool makeSquare = true)
        {
            this.filename = filename;
            this.elevations = new Dictionary<DataKey, List<DataPoint>>();
            this.makeSquare = makeSquare;

            LoadDem();
            CalculateInitSampling();
            //CalculateNewOrigin();
            CreateElevationGrid();
            //CalculateSampling();
        }

        private void CalculateInitSampling()
        {
            List<DataKey> keys = Elevations.Keys.ToList<DataKey>();
            int xdim = keys.Count;
            int ydim = Elevations[keys[0]].Count;

            xSampling = Math.Abs(EastingNorthEast - EastingNorthWest) / xdim;
            ySampling = Math.Abs(NorthingNorthEast - NorthingSouthEast) / ydim;
        }

        //private void CalculateSampling()
        //{
        //    xSampling = (EastingNorthEast - EastingNorthWest) / GetGridWidth();
        //    ySampling = (NorthingNorthEast - NorthingSouthEast) / GetGridHeight();
        //}

        private void CreateElevationGrid()
        {
            List<DataKey> keys = Elevations.Keys.ToList<DataKey>();
            
            if (makeSquare)
            {
                int xdim = keys.Count;
                int ydim = Elevations[keys[0]].Count;
                int dim = Math.Min(xdim, ydim);
                elevationGrid = new int[dim, dim];

                for (int x = 0; x < dim; x++)
                {
                    List<DataPoint> dataPoints = Elevations[keys[x]];
                    //dataPoints.Reverse();
                    for (int y = 0; y < dim; y++)
                    {
                        elevationGrid[x, y] = dataPoints[y].Elevation;
                    }
                }

                // re-calculate origins to square fit
                if (xdim < ydim)
                {
                    int diff = Math.Abs(dim - xdim);
                    eastingSE -= (diff * xSampling);
                    eastingNE -= (diff * xSampling);
                }
                else
                {
                    int diff = Math.Abs(dim - ydim);
                    northingSE -= (diff * ySampling);
                    northingSW -= (diff * ySampling);
                }                
            }
            else
            {
                elevationGrid = new int[keys.Count, Elevations[keys[0]].Count];
                int xCount = keys.Count;
                for (int x = 0; x < xCount; x++)
                {
                    List<DataPoint> dataPoints = Elevations[keys[x]];
                    dataPoints.Reverse();

                    int yCount = dataPoints.Count;

                    for (int y = 0; y < yCount; y++)
                    {
                        elevationGrid[x, y] = dataPoints[y].Elevation;
                    }
                }
            }
        }

        //0 	134 	Descriptive Name of the represented area
        //162 	167 	UTM Zone number
        //529 	534 	Unit of resolution of ground grid (0=radian;1=feet;2=metre;3=arc-second)
        //535 	540 	Unit of resolution Elevation (1=feet;2=metre)
        //546 	569 	Easting of the South West corner
        //570 	593 	Northing of the South West corner
        //594 	617 	Easting of the North West corner
        //618 	641 	Northing of the North West corner
        //642 	665 	Easting of the North East corner
        //666 	689 	Northing of the North East corner
        //690 	713 	Easting of the South East corner
        //714 	737 	Northing of the South East corner
        //738 	761 	Minimum elevation found in this file
        //762 	786 	Maximum elevation found in this file
        //816 	827 	Resolution per grid cell East – West
        //828 	839 	Resolution per grid cell North – South
        //858 	863 	Number of columns

        public enum GroundGridUnit
        {
            Radian = 0,
            Feet = 1,
            Metre = 2,
            ArcSecond = 3,
        }

        public enum ElevationUnit
        {
            Feet = 1,
            Metre = 2,
        }

        string areaDescription;
        int utmZoneNumber;
        int utmCode;
        int unitOfResolutionGroundGrid;
        int unitOfResolutionElevation;
        double eastingSW;
        double northingSW;
        double eastingNW;
        double northingNW;
        double eastingNE;
        double northingNE;
        double eastingSE;
        double northingSE;
        double minElevation;
        double maxElevation;
        float resolutionEW;
        float resolutionNS;
        int numColums;

        public string AreaDescription
        {
            get { return areaDescription; }
        }

        public int UtmZone
        {
            get { return utmZoneNumber; }
        }

        public GroundGridUnit GroundGridResolution
        {
            get { return (GroundGridUnit)unitOfResolutionGroundGrid; }
        }

        public ElevationUnit ElevationResolution
        {
            get { return (ElevationUnit)unitOfResolutionElevation; }
        }

        public double EastingSouthWest
        {
            get { return eastingSW; }
        }

        public double NorthingSouthWest
        {
            get { return northingSW; }
        }

        public double EastingNorthWest
        {
            get { return eastingNW; }
        }

        public double NorthingNorthWest
        {
            get { return northingNW; }
        }

        public double EastingNorthEast
        {
            get { return eastingNE; }
        }

        public double NorthingNorthEast
        {
            get { return northingNE; }
        }

        public double EastingSouthEast
        {
            get { return eastingSE; }
        }

        public double NorthingSouthEast
        {
            get { return northingSE; }
        }

        public double MinElevation
        {
            get { return minElevation; }
        }

        public double MaxElevation
        {
            get { return maxElevation; }
        }

        public float ResolutionEastWest
        {
            get { return resolutionEW; }
        }

        public float ResolutionNorthSouth
        {
            get { return resolutionNS; }
        }

        public int NumColumns
        {
            get { return numColums; }
        }

        private string Decompress()
        {
            string newFilename = null;

            bool isGzip = FileChecker.CheckSignature(filename, 3, "1F-8B-08");
            bool isPKZip = FileChecker.CheckSignature(filename, 4, "50-4B-03-04");

            if (!isGzip)
                return null;

            return newFilename;
        }

        private void LoadDem()
        {
            FileStream reader;

            // Check if file is compressed
            //Decompress();

            using (reader = new FileStream(filename, FileMode.Open))
            {
                // read header
                //SE geographic corner (110-135!)

                areaDescription = ReadString(reader, 0, 134);

                // 163-168 is code for 
                utmCode = ReadInt(reader, 157);
                utmZoneNumber = ReadInt(reader, 162);

                unitOfResolutionGroundGrid = ReadInt(reader, 529);
                unitOfResolutionElevation = ReadInt(reader, 535);
                eastingSW = ReadDouble(reader, 546);
                northingSW = ReadDouble(reader, 570);
                eastingNW = ReadDouble(reader, 594);
                northingNW = ReadDouble(reader, 618);
                eastingNE = ReadDouble(reader, 642);
                northingNE = ReadDouble(reader, 666);
                eastingSE = ReadDouble(reader, 690);
                northingSE = ReadDouble(reader, 714);
                minElevation = ReadDouble(reader, 738);
                maxElevation = ReadDouble(reader, 762);
                resolutionEW = (float)ReadFloat(reader, 816); //? ReadDouble
                resolutionNS = (float)ReadFloat(reader, 828); //? ReadDouble
                numColums = ReadInt(reader, 858);

                int index = 1024;

                for (int column = 0; column < numColums; column++)
                {
                    // read a column
                    index = ReadProfile(reader, index);

                    // move index to next profile
                    // which is the next chunk at mod 1024
                    while (index % 1024 != 0 && index <= int.MaxValue)
                    {
                        index++;
                    }
                }

                // could reader footer?
            }

        }

        private int ReadProfile(FileStream reader, int index, int length = 1024)
        {
            DataKey key = new DataKey();
            List<DataPoint> dataPoints = new List<DataPoint>();
            key.Row = ReadInt(reader, index);
            index += 6;
            key.Col = ReadInt(reader, index);
            index += 6;

            int blockCount = ReadInt(reader, index);
            index += 6;

            int colCount = ReadInt(reader, index);
            index += 6; // col count            

            double startNorthing = ReadDouble(reader, index);
            index += 24;
            double startEasting = ReadDouble(reader, index);
            index += 24;

            index += 24; // wont bother reading these
            index += 24;
            index += 24;

            // finished 144 bytes read of profile header

            // read a block of height data
            for (int i = 0; i < blockCount; i++)
            {
                int newBlock = index + 4;

                if (index > 0 && newBlock % 1024 == 0)
                {
                    index += 4;
                }

                int elevation = ReadInt(reader, index);

                index += 6;

                DataPoint dataPoint = new DataPoint();
                dataPoint.Elevation = elevation;

                // calculate
                //dataPoint.Easting = 0;
                //dataPoint.Northing = 0;
                //dataPoint.Latitude = 0;
                //dataPoint.Longitude = 0;

                dataPoints.Add(dataPoint);
            }

            elevations.Add(key, dataPoints);

            return index;
        }

        byte[] dataInt = new byte[6];

        private int ReadInt(FileStream reader, int nonZeroIndex, int length = 6)
        {            
            reader.Position = nonZeroIndex;
            reader.Read(dataInt, 0, length);
            string str = Encoding.ASCII.GetString(dataInt);
            return Int32.Parse(str);
        }

        private float ReadFloat(FileStream reader, int nonZeroIndex, int length = 10)
        {
            reader.Position = nonZeroIndex;
            
            byte[] data = new byte[length];
            reader.Read(data, 0, length);
            string str = Encoding.ASCII.GetString(data);
            return float.Parse(str);
        }

        private double ReadDouble(FileStream reader, int nonZeroIndex, int length = 24)
        {
            reader.Position = nonZeroIndex;
            byte[] data = new byte[length];
            reader.Read(data, 0, length);
            string str = Encoding.ASCII.GetString(data);
            return double.Parse(str.Replace('D', 'E'));
        }

        private string ReadString(FileStream reader, int nonZeroIndex, int length)
        {
            reader.Position = nonZeroIndex;
            byte[] data = new byte[length];
            reader.Read(data, 0, length);
            return Encoding.ASCII.GetString(data);
        }

    }
}

