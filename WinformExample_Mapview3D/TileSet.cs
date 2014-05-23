using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diswerx.DemRender;
using Diswerx.GeoTiffLoader;
using System.Drawing;
using Diswerx.SubControls;

namespace WinformExample_Mapview3D
{
    public class TileSet
    {
        //private List<TerrainMesh> tiles; public List<TerrainMesh> Tiles { get { return tiles; } }

        private void Setup_DEM_Params(ref Diswerx.SubControls.SubTerrain.DEM_Params dem_params, DemModel dem, float yExaggeration)
        {
            dem_params.EastingEast = dem.EastingNorthEast;
            dem_params.EastingWest = dem.EastingNorthWest;
            dem_params.NorthingNorth = dem.NorthingNorthEast;
            dem_params.NorthingSouth = dem.NorthingSouthEast;
            dem_params.Y_Exaggeration = yExaggeration;
        }

        public bool LoadTileSet(TileCache tileCache, ref Diswerx.SubControls.SubTerrain.DEM_Params dem_params, 
            string path, int xTiles, int yTiles, float meshResolution = 1000f, float yExaggeration = 200f)
        {
            //tiles = new List<TerrainMesh>();

            for (int y = 0; y < yTiles; y++)
            {
                for (int x = 0; x < xTiles; x++) 
                {
                    TerrainMesh terrain;

                    int xTile = x + 1;
                    int yTile = y + 1;

                    string tileName = "tile_" + xTile.ToString("D2") + "_" + yTile.ToString("D2");

                    string demName = tileName + ".DEM";
                    DemModel dem = new DemModel(path + demName);                    

                    string tiffName = tileName + ".tif";
                    GeoTiff geoTiff = new GeoTiff();
                    geoTiff.Load(path + tiffName);                    

                    //hrm
                    Setup_DEM_Params(ref dem_params, dem, yExaggeration);

                    // degrees conversion
                    double n = dem.NorthingNorthEast / 3600.0;
                    double e = dem.EastingNorthEast / 3600.0;
                    double s = dem.NorthingSouthEast / 3600.0;
                    double w = dem.EastingSouthWest / 3600.0;

                    //Bitmap b = geoTiff.GrabRegion(e, w, n, s);
                    Bitmap b = geoTiff.Bitmap;

                    int texture = 0;
                    texture = Helpers.LoadTexture(b, texture);                    

                    float xRes = dem.ResolutionEastWest * meshResolution;
                    float yRes = dem.ResolutionNorthSouth * meshResolution;

                    terrain = new TerrainMesh(xRes,
                                              yRes,
                                              dem.ElevationGrid,
                                              texture, 513, 513,
                                              1f, 1f, false,
                                              0, 0); // <- needs to be filled in

                    //terrain = new TerrainMesh(xRes, 
                    //                          yRes,
                    //                          dem.ElevationGrid, 
                    //                          texture, b.Width, b.Height,
                    //                          1f, 1f, false,
                    //                          0, 0); // <- needs to be filled in
                    
                    float yPos = (terrain.Height - 1f) * (y * yRes);
                    float xPos = (terrain.Width - 1f) * (x * xRes);

                    terrain.Location = new OpenTK.Vector3(yPos, 0, xPos);

                    tileCache.AddMesh(terrain, tileName);

                    //tiles.Add(terrain);

                    // important! release when finished with
                    geoTiff.Bitmap.Dispose();
                }
            }

            return true;
        }
    }
}
