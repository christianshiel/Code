﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization.Formatters.Binary;

namespace Diswerx.GeoTiffLoader
{
    /// <summary>
    /// A class for handling reading of GeoTiffs.
    /// </summary>
    public class GeoTiff
    {
        // Generated data

        /// <summary>
        /// Origin east in coordinate system
        /// </summary>
        private double originEasting; public double OriginEasting { get { return originEasting; } }
        
        /// <summary>
        /// Origin north in coordinate system
        /// </summary>
        private double originNorthing; public double OriginNorthing { get { return originNorthing; } }

        /// <summary>
        /// Projection system
        /// </summary>
        private string projection; public string Projection { get { return projection; } }

        /// <summary>
        /// Bitmap size to world size mapping in x
        /// </summary>
        private double meters_to_pixels_x; public double Meters_To_Pixels_X { get { return meters_to_pixels_x; } }


        /// <summary>
        /// Bitmap size to world size mapping in y
        /// </summary>
        private double meters_to_pixels_y; public double Meters_To_Pixels_Y { get { return meters_to_pixels_y; } }
        //

        private double width_meters;
        private double height_meters;

        private Bitmap bitmap;

        /// <summary>
        /// Retreive the bitmap after loading
        /// </summary>
        public Bitmap Bitmap
        {
            get { return bitmap; }
        }

        /// <summary>
        /// Get the bitmap as a png byte stream
        /// </summary>
        /// <returns></returns>
        public byte[] GetPngSerialized()
        { 
            MemoryStream stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Png);
            return stream.ToArray();
        }

        public int GetWidth()
        {
            return bitmap.Width;
        }

        public int GetHeight()
        {
            return bitmap.Height;
        }

        // Header
        public enum ByteOrder
        {
            Big = 0,
            Little,
        }

        private ByteOrder byteOrder;
        private short version;
        uint ifdOffset;       
        //

        public struct Ifd
        {
            public ushort NumEntries;
            public TifTag[] TifTags;
            public uint NextIfdOffset; 
        }

        public struct TifTag
        {
            public TagId TagId;

            //1 BYTE 8-bit unsigned integer
            //2 ASCII 8-bit, NULL-terminated string
            //3 SHORT 16-bit unsigned integer
            //4 LONG 32-bit unsigned integer
            //5 RATIONAL Two 32-bit unsigned integers       
            //6 SBYTE 8-bit signed integer
            //7 UNDEFINE 8-bit byte
            //8 SSHORT 16-bit signed integer 
            //9 SLONG 32-bit signed integer 
            //10 SRATIONAL Two 32-bit signed integers 
            //11 FLOAT 4-byte single-precision IEEE floating-point value
            //12 DOUBLE 8-byte double-precision IEEE floating-point value
            public ushort DataType;
            
            public uint DataCount;
            public uint DataOffset;
        }

        public enum TagId
        {
            GeoKeyDirectoryTag = 34735, // This tag is also know as 'ProjectionInfoTag' and 'CoordSystemInfoTag' 
            GeoDoubleParamsTag = 34736,
            GeoAsciiParamsTag = 34737,
            ModelPixelScaleTag = 33550,
            ModelTiepointTag = 33922, // This tag is also known as 'GeoreferenceTag'. 
            ModelTransformationTag = 34264,
        }

        //private ConfigKeys configKey; public ConfigKeys ConfigKey { get; set; }
        //private GeographicParameterKeys geographicParameterKey; public GeographicParameterKeys GeographicParameterKey { get; set; }
        //private ProjectedParameterKeys projectedParameterKey; public ProjectedParameterKeys ProjectedParameterKey { get; set; }
        //private VerticalKeys verticalKey; public VerticalKeys VerticalKey { get; set; }
        //private ModelTypeCodes modelTypeCode; public ModelTypeCodes ModelTypeCode { get; set; }
        //private RasterTypeCodes rasterTypeCode; public RasterTypeCodes RasterTypeCode { get; set; }
        //private LinearUnitCodes linearUnitCode; public LinearUnitCodes LinearUnitCode { get; set; }
        //private AngularUnitsCodes angularUnitCode; public AngularUnitsCodes AngularUnitCode { get; set; }
        //private GeographicTypeCodes geographicTypeCode; public GeographicTypeCodes GeographicTypeCode { get; set; }
        //private GeodeticDatumCodes geodeticDatumCode; public GeodeticDatumCodes GeodeticDatumCode { get; set; }
        //private EllipsoidCodes ellipsoidCode; public EllipsoidCodes EllipsoidCode { get; set; }
        //private PrimeMeridianCodes primeMeridianCode; public PrimeMeridianCodes PrimeMeridianCode { get; set; }
        //private ProjectedTypeCodes projectedTypeCode; public ProjectedTypeCodes ProjectedTypeCode { get; set; }
        //private ProjectionCodes projectionCode; public ProjectionCodes ProjectionCode { get; set; }
        //private CoordinateTransformationCodes coordinateTransformCode; public CoordinateTransformationCodes CoordinateTransformationCode { get; set; }
        //private VertialTypeCodes verticalTypeCode; public VertialTypeCodes VerticalTypeCode { get; set; }

        private double[] modelPixelScaleTags; public double[] ModelPixelScaleTags { get; set; }
        private double[] modelTiepointTags; public double[] ModelTiepointTags { get; set; }

        public enum ModelTypeCodes
        {
            ModelTypeProjected = 1,   /* Projection Coordinate System         */
            ModelTypeGeographic = 2,   /* Geographic latitude-longitude System */
            ModelTypeGeocentric = 3,   /* Geocentric (X,Y,Z) Coordinate System */
        }

        public enum ConfigKeys
        {
            GTModelTypeGeoKey            = 1024, /*  Section 6.3.1.1 Codes       */
            GTRasterTypeGeoKey           = 1025, /*  Section 6.3.1.2 Codes       */
            GTCitationGeoKey             = 1026, /* documentation */
        }

        public enum GeographicParameterKeys
        {
            GeographicTypeGeoKey         = 2048, /*,  Section 6.3.2.1 Codes     */
            GeogCitationGeoKey           = 2049, /* documentation             */
            GeogGeodeticDatumGeoKey      = 2050, /*  Section 6.3.2.2 Codes     */
            GeogPrimeMeridianGeoKey      = 2051, /*  Section 6.3.2.4 codes     */
            GeogLinearUnitsGeoKey        = 2052, /*  Section 6.3.1.3 Codes     */
            GeogLinearUnitSizeGeoKey     = 2053, /* meters                    */
            GeogAngularUnitsGeoKey       = 2054, /*  Section 6.3.1.4 Codes     */
            GeogAngularUnitSizeGeoKey    = 2055, /* radians                   */
            GeogEllipsoidGeoKey          = 2056, /*  Section 6.3.2.3 Codes     */
            GeogSemiMajorAxisGeoKey      = 2057, /* GeogLinearUnits           */
            GeogSemiMinorAxisGeoKey      = 2058, /* GeogLinearUnits           */
            GeogInvFlatteningGeoKey      = 2059, /* ratio                     */
            GeogAzimuthUnitsGeoKey       = 2060, /*  Section 6.3.1.4 Codes     */
            GeogPrimeMeridianLongGeoKey  = 2061, /* GeogAngularUnit           */
        }

        public enum ProjectedParameterKeys
        {
            ProjectedCSTypeGeoKey          = 3072,  /*  Section 6.3.3.1 codes   */
            PCSCitationGeoKey              = 3073,  /* documentation           */
            ProjectionGeoKey               = 3074,  /*  Section 6.3.3.2 codes   */
            ProjCoordTransGeoKey           = 3075,  /*  Section 6.3.3.3 codes   */
            ProjLinearUnitsGeoKey          = 3076,  /*  Section 6.3.1.3 codes   */
            ProjLinearUnitSizeGeoKey       = 3077,  /* meters                  */
            ProjStdParallel1GeoKey         = 3078,  /* GeogAngularUnit */
            ProjStdParallel2GeoKey         = 3079,  /* GeogAngularUnit */
            ProjNatOriginLongGeoKey        = 3080,  /* GeogAngularUnit */
            ProjNatOriginLatGeoKey         = 3081,  /* GeogAngularUnit */
            ProjFalseEastingGeoKey         = 3082,  /* ProjLinearUnits */
            ProjFalseNorthingGeoKey        = 3083,  /* ProjLinearUnits */
            ProjFalseOriginLongGeoKey      = 3084,  /* GeogAngularUnit */
            ProjFalseOriginLatGeoKey       = 3085,  /* GeogAngularUnit */
            ProjFalseOriginEastingGeoKey   = 3086,  /* ProjLinearUnits */
            ProjFalseOriginNorthingGeoKey  = 3087,  /* ProjLinearUnits */
            ProjCenterLongGeoKey           = 3088,  /* GeogAngularUnit */
            ProjCenterLatGeoKey            = 3089,  /* GeogAngularUnit */
            ProjCenterEastingGeoKey        = 3090,  /* ProjLinearUnits */
            ProjCenterNorthingGeoKey       = 3091,  /* ProjLinearUnits */
            ProjScaleAtNatOriginGeoKey     = 3092,  /* ratio   */
            ProjScaleAtCenterGeoKey        = 3093,  /* ratio   */
            ProjAzimuthAngleGeoKey         = 3094,  /* GeogAzimuthUnit */
            ProjStraightVertPoleLongGeoKey = 3095,  /* GeogAngularUnit */
        }

        public enum VerticalKeys
        {
            VerticalCSTypeGeoKey           = 4096,   /*  Section 6.3.4.1 codes   */
            VerticalCitationGeoKey         = 4097,   /* documentation */
            VerticalDatumGeoKey            = 4098,   /*  Section 6.3.4.2 codes   */
            VerticalUnitsGeoKey            = 4099,   /*  Section 6.3.1.3 codes   */
        }

        public enum RasterTypeCodes
        {
            RasterPixelIsArea  = 1,
            RasterPixelIsPoint = 2,
        }

        public enum LinearUnitCodes
        {
            Linear_Meter =	9001,
            Linear_Foot =	9002,
            Linear_Foot_US_Survey =	9003,
            Linear_Foot_Modified_American =	9004,
            Linear_Foot_Clarke =	9005,
            Linear_Foot_Indian =	9006,
            Linear_Link =	9007,
            Linear_Link_Benoit =	9008,
            Linear_Link_Sears =	9009,
            Linear_Chain_Benoit =	9010,
            Linear_Chain_Sears =	9011,
            Linear_Yard_Sears =	9012,
            Linear_Yard_Indian =	9013,
            Linear_Fathom =	9014,
            Linear_Mile_International_Nautical = 9015,
        }

        public enum AngularUnitsCodes
        {   
            Angular_Radian =	9101,
            Angular_Degree =	9102,
            Angular_Arc_Minute =	9103,
            Angular_Arc_Second =	9104,
            Angular_Grad =	9105,
            Angular_Gon =	9106,
            Angular_DMS =	9107,
            Angular_DMS_Hemisphere =	9108,
        }

        public enum GeographicTypeCodes
        {
            GCS_Adindan =	4201,
            GCS_AGD66 =	4202,
            GCS_AGD84 =	4203,
            GCS_Ain_el_Abd =	4204,
            GCS_Afgooye =	4205,
            GCS_Agadez =	4206,
            GCS_Lisbon =	4207,
            GCS_Aratu =	4208,
            GCS_Arc_1950 =	4209,
            GCS_Arc_1960 =	4210,
            GCS_Batavia =	4211,
            GCS_Barbados =	4212,
            GCS_Beduaram =	4213,
            GCS_Beijing_1954 =	4214,
            GCS_Belge_1950 =	4215,
            GCS_Bermuda_1957 =	4216,
            GCS_Bern_1898 =	4217,
            GCS_Bogota =	4218,
            GCS_Bukit_Rimpah =	4219,
            GCS_Camacupa =	4220,
            GCS_Campo_Inchauspe =	4221,
            GCS_Cape =	4222,
            GCS_Carthage =	4223,
            GCS_Chua =	4224,
            GCS_Corrego_Alegre =	4225,
            GCS_Cote_d_Ivoire =	4226,
            GCS_Deir_ez_Zor =	4227,
            GCS_Douala =	4228,
            GCS_Egypt_1907 =	4229,
            GCS_ED50 =	4230,
            GCS_ED87 =	4231,
            GCS_Fahud =	4232,
            GCS_Gandajika_1970 =	4233,
            GCS_Garoua =	4234,
            GCS_Guyane_Francaise =	4235,
            GCS_Hu_Tzu_Shan =	4236,
            GCS_HD72 =	4237,
            GCS_ID74 =	4238,
            GCS_Indian_1954 =	4239,
            GCS_Indian_1975 =	4240,
            GCS_Jamaica_1875 =	4241,
            GCS_JAD69 =	4242,
            GCS_Kalianpur =	4243,
            GCS_Kandawala =	4244,
            GCS_Kertau =	4245,
            GCS_KOC =	4246,
            GCS_La_Canoa =	4247,
            GCS_PSAD56 =	4248,
            GCS_Lake =	4249,
            GCS_Leigon =	4250,
            GCS_Liberia_1964 =	4251,
            GCS_Lome =	4252,
            GCS_Luzon_1911 =	4253,
            GCS_Hito_XVIII_1963 =	4254,
            GCS_Herat_North =	4255,
            GCS_Mahe_1971 =	4256,
            GCS_Makassar =	4257,
            GCS_EUREF89 =	4258,
            GCS_Malongo_1987 =	4259,
            GCS_Manoca =	4260,
            GCS_Merchich =	4261,
            GCS_Massawa =	4262,
            GCS_Minna =	4263,
            GCS_Mhast =	4264,
            GCS_Monte_Mario =	4265,
            GCS_M_poraloko =	4266,
            GCS_NAD27 =	4267,
            GCS_NAD_Michigan =	4268,
            GCS_NAD83 =	4269,
            GCS_Nahrwan_1967 =	4270,
            GCS_Naparima_1972 =	4271,
            GCS_GD49 =	4272,
            GCS_NGO_1948 =	4273,
            GCS_Datum_73 =	4274,
            GCS_NTF =	4275,
            GCS_NSWC_9Z_2 =	4276,
            GCS_OSGB_1936 =	4277,
            GCS_OSGB70 =	4278,
            GCS_OS_SN80 =	4279,
            GCS_Padang =	4280,
            GCS_Palestine_1923 =	4281,
            GCS_Pointe_Noire =	4282,
            GCS_GDA94 =	4283,
            GCS_Pulkovo_1942 =	4284,
            GCS_Qatar =	4285,
            GCS_Qatar_1948 =	4286,
            GCS_Qornoq =	4287,
            GCS_Loma_Quintana =	4288,
            GCS_Amersfoort =	4289,
            GCS_RT38 =	4290,
            GCS_SAD69 =	4291,
            GCS_Sapper_Hill_1943 =	4292,
            GCS_Schwarzeck =	4293,
            GCS_Segora =	4294,
            GCS_Serindung =	4295,
            GCS_Sudan =	4296,
            GCS_Tananarive =	4297,
            GCS_Timbalai_1948 =	4298,
            GCS_TM65 =	4299,
            GCS_TM75 =	4300,
            GCS_Tokyo =	4301,
            GCS_Trinidad_1903 =	4302,
            GCS_TC_1948 =	4303,
            GCS_Voirol_1875 =	4304,
            GCS_Voirol_Unifie =	4305,
            GCS_Bern_1938 =	4306,
            GCS_Nord_Sahara_1959 =	4307,
            GCS_Stockholm_1938 =	4308,
            GCS_Yacare =	4309,
            GCS_Yoff =	4310,
            GCS_Zanderij =	4311,
            GCS_MGI =	4312,
            GCS_Belge_1972 =	4313,
            GCS_DHDN =	4314,
            GCS_Conakry_1905 =	4315,
            GCS_WGS_72 =	4322,
            GCS_WGS_72BE =	4324,
            GCS_WGS_84 =	4326,
            GCS_Bern_1898_Bern =	4801,
            GCS_Bogota_Bogota =	4802,
            GCS_Lisbon_Lisbon =	4803,
            GCS_Makassar_Jakarta =	4804,
            GCS_MGI_Ferro =	4805,
            GCS_Monte_Mario_Rome =	4806,
            GCS_NTF_Paris =	4807,
            GCS_Padang_Jakarta =	4808,
            GCS_Belge_1950_Brussels =	4809,
            GCS_Tananarive_Paris =	4810,
            GCS_Voirol_1875_Paris =	4811,
            GCS_Voirol_Unifie_Paris =	4812,
            GCS_Batavia_Jakarta =	4813,
            GCS_ATF_Paris =	4901,
            GCS_NDG_Paris =	4902,

            // Ellipsoid-Only GCS:
            // Note: the numeric code is equal to the code of the correspoding
            // EPSG ellipsoid, minus 3000.

            GCSE_Airy1830 =	4001,
            GCSE_AiryModified1849 =	4002,
            GCSE_AustralianNationalSpheroid =	4003,
            GCSE_Bessel1841 =	4004,
            GCSE_BesselModified =	4005,
            GCSE_BesselNamibia =	4006,
            GCSE_Clarke1858 =	4007,
            GCSE_Clarke1866 =	4008,
            GCSE_Clarke1866Michigan =	4009,
            GCSE_Clarke1880_Benoit =	4010,
            GCSE_Clarke1880_IGN =	4011,
            GCSE_Clarke1880_RGS =	4012,
            GCSE_Clarke1880_Arc =	4013,
            GCSE_Clarke1880_SGA1922 =	4014,
            GCSE_Everest1830_1937Adjustment =	4015,
            GCSE_Everest1830_1967Definition =	4016,
            GCSE_Everest1830_1975Definition =	4017,
            GCSE_Everest1830Modified =	4018,
            GCSE_GRS1980 =	4019,
            GCSE_Helmert1906 =	4020,
            GCSE_IndonesianNationalSpheroid =	4021,
            GCSE_International1924 =	4022,
            GCSE_International1967 =	4023,
            GCSE_Krassowsky1940 =	4024,
            GCSE_NWL9D =	4025,
            GCSE_NWL10D =	4026,
            GCSE_Plessis1817 =	4027,
            GCSE_Struve1860 =	4028,
            GCSE_WarOffice =	4029,
            GCSE_WGS84 =	4030,
            GCSE_GEM10C =	4031,
            GCSE_OSU86F =	4032,
            GCSE_OSU91A =	4033,
            GCSE_Clarke1880 =	4034,
            GCSE_Sphere =	4035,
        }

        public enum GeodeticDatumCodes
        {
            Datum_Adindan =	6201,
            Datum_Australian_Geodetic_Datum_1966 =	6202,
            Datum_Australian_Geodetic_Datum_1984 =	6203,
            Datum_Ain_el_Abd_1970 =	6204,
            Datum_Afgooye =	6205,
            Datum_Agadez =	6206,
            Datum_Lisbon =	6207,
            Datum_Aratu =	6208,
            Datum_Arc_1950 =	6209,
            Datum_Arc_1960 =	6210,
            Datum_Batavia =	6211,
            Datum_Barbados =	6212,
            Datum_Beduaram =	6213,
            Datum_Beijing_1954 =	6214,
            Datum_Reseau_National_Belge_1950 =	6215,
            Datum_Bermuda_1957 =	6216,
            Datum_Bern_1898 =	6217,
            Datum_Bogota =	6218,
            Datum_Bukit_Rimpah =	6219,
            Datum_Camacupa =	6220,
            Datum_Campo_Inchauspe =	6221,
            Datum_Cape =	6222,
            Datum_Carthage =	6223,
            Datum_Chua =	6224,
            Datum_Corrego_Alegre =	6225,
            Datum_Cote_d_Ivoire =	6226,
            Datum_Deir_ez_Zor =	6227,
            Datum_Douala =	6228,
            Datum_Egypt_1907 =	6229,
            Datum_European_Datum_1950 =	6230,
            Datum_European_Datum_1987 =	6231,
            Datum_Fahud =	6232,
            Datum_Gandajika_1970 =	6233,
            Datum_Garoua =	6234,
            Datum_Guyane_Francaise =	6235,
            Datum_Hu_Tzu_Shan =	6236,
            Datum_Hungarian_Datum_1972 =	6237,
            Datum_Indonesian_Datum_1974 =	6238,
            Datum_Indian_1954 =	6239,
            Datum_Indian_1975 =	6240,
            Datum_Jamaica_1875 =	6241,
            Datum_Jamaica_1969 =	6242,
            Datum_Kalianpur =	6243,
            Datum_Kandawala =	6244,
            Datum_Kertau =	6245,
            Datum_Kuwait_Oil_Company =	6246,
            Datum_La_Canoa =	6247,
            Datum_Provisional_S_American_Datum_1956 =	6248,
            Datum_Lake =	6249,
            Datum_Leigon =	6250,
            Datum_Liberia_1964 =	6251,
            Datum_Lome =	6252,
            Datum_Luzon_1911 =	6253,
            Datum_Hito_XVIII_1963 =	6254,
            Datum_Herat_North =	6255,
            Datum_Mahe_1971 =	6256,
            Datum_Makassar =	6257,
            Datum_European_Reference_System_1989 =	6258,
            Datum_Malongo_1987 =	6259,
            Datum_Manoca =	6260,
            Datum_Merchich =	6261,
            Datum_Massawa =	6262,
            Datum_Minna =	6263,
            Datum_Mhast =	6264,
            Datum_Monte_Mario =	6265,
            Datum_M_poraloko =	6266,
            Datum_North_American_Datum_1927 =	6267,
            Datum_NAD_Michigan =	6268,
            Datum_North_American_Datum_1983 =	6269,
            Datum_Nahrwan_1967 =	6270,
            Datum_Naparima_1972 =	6271,
            Datum_New_Zealand_Geodetic_Datum_1949 =	6272,
            Datum_NGO_1948 =	6273,
            Datum_Datum_73 =	6274,
            Datum_Nouvelle_Triangulation_Francaise =	6275,
            Datum_NSWC_9Z_2 =	6276,
            Datum_OSGB_1936 =	6277,
            Datum_OSGB_1970_SN =	6278,
            Datum_OS_SN_1980 =	6279,
            Datum_Padang_1884 =	6280,
            Datum_Palestine_1923 =	6281,
            Datum_Pointe_Noire =	6282,
            Datum_Geocentric_Datum_of_Australia_1994 =	6283,
            Datum_Pulkovo_1942 =	6284,
            Datum_Qatar =	6285,
            Datum_Qatar_1948 =	6286,
            Datum_Qornoq =	6287,
            Datum_Loma_Quintana =	6288,
            Datum_Amersfoort =	6289,
            Datum_RT38 =	6290,
            Datum_South_American_Datum_1969 =	6291,
            Datum_Sapper_Hill_1943 =	6292,
            Datum_Schwarzeck =	6293,
            Datum_Segora =	6294,
            Datum_Serindung =	6295,
            Datum_Sudan =	6296,
            Datum_Tananarive_1925 =	6297,
            Datum_Timbalai_1948 =	6298,
            Datum_TM65 =	6299,
            Datum_TM75 =	6300,
            Datum_Tokyo =	6301,
            Datum_Trinidad_1903 =	6302,
            Datum_Trucial_Coast_1948 =	6303,
            Datum_Voirol_1875 =	6304,
            Datum_Voirol_Unifie_1960 =	6305,
            Datum_Bern_1938 =	6306,
            Datum_Nord_Sahara_1959 =	6307,
            Datum_Stockholm_1938 =	6308,
            Datum_Yacare =	6309,
            Datum_Yoff =	6310,
            Datum_Zanderij =	6311,
            Datum_Militar_Geographische_Institut =	6312,
            Datum_Reseau_National_Belge_1972 =	6313,
            Datum_Deutsche_Hauptdreiecksnetz =	6314,
            Datum_Conakry_1905 =	6315,
            Datum_WGS72 =	6322,
            Datum_WGS72_Transit_Broadcast_Ephemeris =	6324,
            Datum_WGS84 =	6326,
            Datum_Ancienne_Triangulation_Francaise =	6901,
            Datum_Nord_de_Guerre =	6902,

            // Ellipsoid-Only Datum:
            //   Note: the numeric code is equal to the corresponding ellipsoid
            //   code, minus 1000.

            DatumE_Airy1830 =	6001,
            DatumE_AiryModified1849 =	6002,
            DatumE_AustralianNationalSpheroid =	6003,
            DatumE_Bessel1841 =	6004,
            DatumE_BesselModified =	6005,
            DatumE_BesselNamibia =	6006,
            DatumE_Clarke1858 =	6007,
            DatumE_Clarke1866 =	6008,
            DatumE_Clarke1866Michigan =	6009,
            DatumE_Clarke1880_Benoit =	6010,
            DatumE_Clarke1880_IGN =	6011,
            DatumE_Clarke1880_RGS =	6012,
            DatumE_Clarke1880_Arc =	6013,
            DatumE_Clarke1880_SGA1922 =	6014,
            DatumE_Everest1830_1937Adjustment =	6015,
            DatumE_Everest1830_1967Definition =	6016,
            DatumE_Everest1830_1975Definition =	6017,
            DatumE_Everest1830Modified =	6018,
            DatumE_GRS1980 =	6019,
            DatumE_Helmert1906 =	6020,
            DatumE_IndonesianNationalSpheroid =	6021,
            DatumE_International1924 =	6022,
            DatumE_International1967 =	6023,
            DatumE_Krassowsky1960 =	6024,
            DatumE_NWL9D =	6025,
            DatumE_NWL10D =	6026,
            DatumE_Plessis1817 =	6027,
            DatumE_Struve1860 =	6028,
            DatumE_WarOffice =	6029,
            DatumE_WGS84 =	6030,
            DatumE_GEM10C =	6031,
            DatumE_OSU86F =	6032,
            DatumE_OSU91A =	6033,
            DatumE_Clarke1880 =	6034,
            DatumE_Sphere =	6035,
        }

        public enum EllipsoidCodes
        {
            Ellipse_Airy_1830 =	7001,
            Ellipse_Airy_Modified_1849 =	7002,
            Ellipse_Australian_National_Spheroid =	7003,
            Ellipse_Bessel_1841 =	7004,
            Ellipse_Bessel_Modified =	7005,
            Ellipse_Bessel_Namibia =	7006,
            Ellipse_Clarke_1858 =	7007,
            Ellipse_Clarke_1866 =	7008,
            Ellipse_Clarke_1866_Michigan =	7009,
            Ellipse_Clarke_1880_Benoit =	7010,
            Ellipse_Clarke_1880_IGN =	7011,
            Ellipse_Clarke_1880_RGS =	7012,
            Ellipse_Clarke_1880_Arc =	7013,
            Ellipse_Clarke_1880_SGA_1922 =	7014,
            Ellipse_Everest_1830_1937_Adjustment =	7015,
            Ellipse_Everest_1830_1967_Definition =	7016,
            Ellipse_Everest_1830_1975_Definition =	7017,
            Ellipse_Everest_1830_Modified =	7018,
            Ellipse_GRS_1980 =	7019,
            Ellipse_Helmert_1906 =	7020,
            Ellipse_Indonesian_National_Spheroid =	7021,
            Ellipse_International_1924 =	7022,
            Ellipse_International_1967 =	7023,
            Ellipse_Krassowsky_1940 =	7024,
            Ellipse_NWL_9D =	7025,
            Ellipse_NWL_10D =	7026,
            Ellipse_Plessis_1817 =	7027,
            Ellipse_Struve_1860 =	7028,
            Ellipse_War_Office =	7029,
            Ellipse_WGS_84 =	7030,
            Ellipse_GEM_10C =	7031,
            Ellipse_OSU86F =	7032,
            Ellipse_OSU91A =	7033,
            Ellipse_Clarke_1880 =	7034,
            Ellipse_Sphere =	7035,
        }

        public enum PrimeMeridianCodes
        {
            PM_Greenwich =	8901,
            PM_Lisbon =	8902,
            PM_Paris =	8903,
            PM_Bogota =	8904,
            PM_Madrid =	8905,
            PM_Rome =	8906,
            PM_Bern =	8907,
            PM_Jakarta =	8908,
            PM_Ferro =	8909,
            PM_Brussels =	8910,
            PM_Stockholm =	8911,
        }

        public enum ProjectedTypeCodes
        {
            PCS_Adindan_UTM_zone_37N =	20137,
            PCS_Adindan_UTM_zone_38N =	20138,
            PCS_AGD66_AMG_zone_48 =	20248,
            PCS_AGD66_AMG_zone_49 =	20249,
            PCS_AGD66_AMG_zone_50 =	20250,
            PCS_AGD66_AMG_zone_51 =	20251,
            PCS_AGD66_AMG_zone_52 =	20252,
            PCS_AGD66_AMG_zone_53 =	20253,
            PCS_AGD66_AMG_zone_54 =	20254,
            PCS_AGD66_AMG_zone_55 =	20255,
            PCS_AGD66_AMG_zone_56 =	20256,
            PCS_AGD66_AMG_zone_57 =	20257,
            PCS_AGD66_AMG_zone_58 =	20258,
            PCS_AGD84_AMG_zone_48 =	20348,
            PCS_AGD84_AMG_zone_49 =	20349,
            PCS_AGD84_AMG_zone_50 =	20350,
            PCS_AGD84_AMG_zone_51 =	20351,
            PCS_AGD84_AMG_zone_52 =	20352,
            PCS_AGD84_AMG_zone_53 =	20353,
            PCS_AGD84_AMG_zone_54 =	20354,
            PCS_AGD84_AMG_zone_55 =	20355,
            PCS_AGD84_AMG_zone_56 =	20356,
            PCS_AGD84_AMG_zone_57 =	20357,
            PCS_AGD84_AMG_zone_58 =	20358,
            PCS_Ain_el_Abd_UTM_zone_37N =	20437,
            PCS_Ain_el_Abd_UTM_zone_38N =	20438,
            PCS_Ain_el_Abd_UTM_zone_39N =	20439,
            PCS_Ain_el_Abd_Bahrain_Grid =	20499,
            PCS_Afgooye_UTM_zone_38N =	20538,
            PCS_Afgooye_UTM_zone_39N =	20539,
            PCS_Lisbon_Portugese_Grid =	20700,
            PCS_Aratu_UTM_zone_22S =	20822,
            PCS_Aratu_UTM_zone_23S =	20823,
            PCS_Aratu_UTM_zone_24S =	20824,
            PCS_Arc_1950_Lo13 =	20973,
            PCS_Arc_1950_Lo15 =	20975,
            PCS_Arc_1950_Lo17 =	20977,
            PCS_Arc_1950_Lo19 =	20979,
            PCS_Arc_1950_Lo21 =	20981,
            PCS_Arc_1950_Lo23 =	20983,
            PCS_Arc_1950_Lo25 =	20985,
            PCS_Arc_1950_Lo27 =	20987,
            PCS_Arc_1950_Lo29 =	20989,
            PCS_Arc_1950_Lo31 =	20991,
            PCS_Arc_1950_Lo33 =	20993,
            PCS_Arc_1950_Lo35 =	20995,
            PCS_Batavia_NEIEZ =	21100,
            PCS_Batavia_UTM_zone_48S =	21148,
            PCS_Batavia_UTM_zone_49S =	21149,
            PCS_Batavia_UTM_zone_50S =	21150,
            PCS_Beijing_Gauss_zone_13 =	21413,
            PCS_Beijing_Gauss_zone_14 =	21414,
            PCS_Beijing_Gauss_zone_15 =	21415,
            PCS_Beijing_Gauss_zone_16 =	21416,
            PCS_Beijing_Gauss_zone_17 =	21417,
            PCS_Beijing_Gauss_zone_18 =	21418,
            PCS_Beijing_Gauss_zone_19 =	21419,
            PCS_Beijing_Gauss_zone_20 =	21420,
            PCS_Beijing_Gauss_zone_21 =	21421,
            PCS_Beijing_Gauss_zone_22 =	21422,
            PCS_Beijing_Gauss_zone_23 =	21423,
            PCS_Beijing_Gauss_13N =	21473,
            PCS_Beijing_Gauss_14N =	21474,
            PCS_Beijing_Gauss_15N =	21475,
            PCS_Beijing_Gauss_16N =	21476,
            PCS_Beijing_Gauss_17N =	21477,
            PCS_Beijing_Gauss_18N =	21478,
            PCS_Beijing_Gauss_19N =	21479,
            PCS_Beijing_Gauss_20N =	21480,
            PCS_Beijing_Gauss_21N =	21481,
            PCS_Beijing_Gauss_22N =	21482,
            PCS_Beijing_Gauss_23N =	21483,
            PCS_Belge_Lambert_50 =	21500,
            PCS_Bern_1898_Swiss_Old =	21790,
            PCS_Bogota_UTM_zone_17N =	21817,
            PCS_Bogota_UTM_zone_18N =	21818,
            PCS_Bogota_Colombia_3W =	21891,
            PCS_Bogota_Colombia_Bogota =	21892,
            PCS_Bogota_Colombia_3E =	21893,
            PCS_Bogota_Colombia_6E =	21894,
            PCS_Camacupa_UTM_32S =	22032,
            PCS_Camacupa_UTM_33S =	22033,
            PCS_C_Inchauspe_Argentina_1 =	22191,
            PCS_C_Inchauspe_Argentina_2 =	22192,
            PCS_C_Inchauspe_Argentina_3 =	22193,
            PCS_C_Inchauspe_Argentina_4 =	22194,
            PCS_C_Inchauspe_Argentina_5 =	22195,
            PCS_C_Inchauspe_Argentina_6 =	22196,
            PCS_C_Inchauspe_Argentina_7 =	22197,
            PCS_Carthage_UTM_zone_32N =	22332,
            PCS_Carthage_Nord_Tunisie =	22391,
            PCS_Carthage_Sud_Tunisie =	22392,
            PCS_Corrego_Alegre_UTM_23S =	22523,
            PCS_Corrego_Alegre_UTM_24S =	22524,
            PCS_Douala_UTM_zone_32N =	22832,
            PCS_Egypt_1907_Red_Belt =	22992,
            PCS_Egypt_1907_Purple_Belt =	22993,
            PCS_Egypt_1907_Ext_Purple =	22994,
            PCS_ED50_UTM_zone_28N =	23028,
            PCS_ED50_UTM_zone_29N =	23029,
            PCS_ED50_UTM_zone_30N =	23030,
            PCS_ED50_UTM_zone_31N =	23031,
            PCS_ED50_UTM_zone_32N =	23032,
            PCS_ED50_UTM_zone_33N =	23033,
            PCS_ED50_UTM_zone_34N =	23034,
            PCS_ED50_UTM_zone_35N =	23035,
            PCS_ED50_UTM_zone_36N =	23036,
            PCS_ED50_UTM_zone_37N =	23037,
            PCS_ED50_UTM_zone_38N =	23038,
            PCS_Fahud_UTM_zone_39N =	23239,
            PCS_Fahud_UTM_zone_40N =	23240,
            PCS_Garoua_UTM_zone_33N =	23433,
            PCS_ID74_UTM_zone_46N =	23846,
            PCS_ID74_UTM_zone_47N =	23847,
            PCS_ID74_UTM_zone_48N =	23848,
            PCS_ID74_UTM_zone_49N =	23849,
            PCS_ID74_UTM_zone_50N =	23850,
            PCS_ID74_UTM_zone_51N =	23851,
            PCS_ID74_UTM_zone_52N =	23852,
            PCS_ID74_UTM_zone_53N =	23853,
            PCS_ID74_UTM_zone_46S =	23886,
            PCS_ID74_UTM_zone_47S =	23887,
            PCS_ID74_UTM_zone_48S =	23888,
            PCS_ID74_UTM_zone_49S =	23889,
            PCS_ID74_UTM_zone_50S =	23890,
            PCS_ID74_UTM_zone_51S =	23891,
            PCS_ID74_UTM_zone_52S =	23892,
            PCS_ID74_UTM_zone_53S =	23893,
            PCS_ID74_UTM_zone_54S =	23894,
            PCS_Indian_1954_UTM_47N =	23947,
            PCS_Indian_1954_UTM_48N =	23948,
            PCS_Indian_1975_UTM_47N =	24047,
            PCS_Indian_1975_UTM_48N =	24048,
            PCS_Jamaica_1875_Old_Grid =	24100,
            PCS_JAD69_Jamaica_Grid =	24200,
            PCS_Kalianpur_India_0 =	24370,
            PCS_Kalianpur_India_I =	24371,
            PCS_Kalianpur_India_IIa =	24372,
            PCS_Kalianpur_India_IIIa =	24373,
            PCS_Kalianpur_India_IVa =	24374,
            PCS_Kalianpur_India_IIb =	24382,
            PCS_Kalianpur_India_IIIb =	24383,
            PCS_Kalianpur_India_IVb =	24384,
            PCS_Kertau_Singapore_Grid =	24500,
            PCS_Kertau_UTM_zone_47N =	24547,
            PCS_Kertau_UTM_zone_48N =	24548,
            PCS_La_Canoa_UTM_zone_20N =	24720,
            PCS_La_Canoa_UTM_zone_21N =	24721,
            PCS_PSAD56_UTM_zone_18N =	24818,
            PCS_PSAD56_UTM_zone_19N =	24819,
            PCS_PSAD56_UTM_zone_20N =	24820,
            PCS_PSAD56_UTM_zone_21N =	24821,
            PCS_PSAD56_UTM_zone_17S =	24877,
            PCS_PSAD56_UTM_zone_18S =	24878,
            PCS_PSAD56_UTM_zone_19S =	24879,
            PCS_PSAD56_UTM_zone_20S =	24880,
            PCS_PSAD56_Peru_west_zone =	24891,
            PCS_PSAD56_Peru_central =	24892,
            PCS_PSAD56_Peru_east_zone =	24893,
            PCS_Leigon_Ghana_Grid =	25000,
            PCS_Lome_UTM_zone_31N =	25231,
            PCS_Luzon_Philippines_I =	25391,
            PCS_Luzon_Philippines_II =	25392,
            PCS_Luzon_Philippines_III =	25393,
            PCS_Luzon_Philippines_IV =	25394,
            PCS_Luzon_Philippines_V =	25395,
            PCS_Makassar_NEIEZ =	25700,
            PCS_Malongo_1987_UTM_32S =	25932,
            PCS_Merchich_Nord_Maroc =	26191,
            PCS_Merchich_Sud_Maroc =	26192,
            PCS_Merchich_Sahara =	26193,
            PCS_Massawa_UTM_zone_37N =	26237,
            PCS_Minna_UTM_zone_31N =	26331,
            PCS_Minna_UTM_zone_32N =	26332,
            PCS_Minna_Nigeria_West =	26391,
            PCS_Minna_Nigeria_Mid_Belt =	26392,
            PCS_Minna_Nigeria_East =	26393,
            PCS_Mhast_UTM_zone_32S =	26432,
            PCS_Monte_Mario_Italy_1 =	26591,
            PCS_Monte_Mario_Italy_2 =	26592,
            PCS_M_poraloko_UTM_32N =	26632,
            PCS_M_poraloko_UTM_32S =	26692,
            PCS_NAD27_UTM_zone_3N =	26703,
            PCS_NAD27_UTM_zone_4N =	26704,
            PCS_NAD27_UTM_zone_5N =	26705,
            PCS_NAD27_UTM_zone_6N =	26706,
            PCS_NAD27_UTM_zone_7N =	26707,
            PCS_NAD27_UTM_zone_8N =	26708,
            PCS_NAD27_UTM_zone_9N =	26709,
            PCS_NAD27_UTM_zone_10N =	26710,
            PCS_NAD27_UTM_zone_11N =	26711,
            PCS_NAD27_UTM_zone_12N =	26712,
            PCS_NAD27_UTM_zone_13N =	26713,
            PCS_NAD27_UTM_zone_14N =	26714,
            PCS_NAD27_UTM_zone_15N =	26715,
            PCS_NAD27_UTM_zone_16N =	26716,
            PCS_NAD27_UTM_zone_17N =	26717,
            PCS_NAD27_UTM_zone_18N =	26718,
            PCS_NAD27_UTM_zone_19N =	26719,
            PCS_NAD27_UTM_zone_20N =	26720,
            PCS_NAD27_UTM_zone_21N =	26721,
            PCS_NAD27_UTM_zone_22N =	26722,
            PCS_NAD27_Alabama_East =	26729,
            PCS_NAD27_Alabama_West =	26730,
            PCS_NAD27_Alaska_zone_1 =	26731,
            PCS_NAD27_Alaska_zone_2 =	26732,
            PCS_NAD27_Alaska_zone_3 =	26733,
            PCS_NAD27_Alaska_zone_4 =	26734,
            PCS_NAD27_Alaska_zone_5 =	26735,
            PCS_NAD27_Alaska_zone_6 =	26736,
            PCS_NAD27_Alaska_zone_7 =	26737,
            PCS_NAD27_Alaska_zone_8 =	26738,
            PCS_NAD27_Alaska_zone_9 =	26739,
            PCS_NAD27_Alaska_zone_10 =	26740,
            PCS_NAD27_California_I =	26741,
            PCS_NAD27_California_II =	26742,
            PCS_NAD27_California_III =	26743,
            PCS_NAD27_California_IV =	26744,
            PCS_NAD27_California_V =	26745,
            PCS_NAD27_California_VI =	26746,
            PCS_NAD27_California_VII =	26747,
            PCS_NAD27_Arizona_East =	26748,
            PCS_NAD27_Arizona_Central =	26749,
            PCS_NAD27_Arizona_West =	26750,
            PCS_NAD27_Arkansas_North =	26751,
            PCS_NAD27_Arkansas_South =	26752,
            PCS_NAD27_Colorado_North =	26753,
            PCS_NAD27_Colorado_Central =	26754,
            PCS_NAD27_Colorado_South =	26755,
            PCS_NAD27_Connecticut =	26756,
            PCS_NAD27_Delaware =	26757,
            PCS_NAD27_Florida_East =	26758,
            PCS_NAD27_Florida_West =	26759,
            PCS_NAD27_Florida_North =	26760,
            PCS_NAD27_Hawaii_zone_1 =	26761,
            PCS_NAD27_Hawaii_zone_2 =	26762,
            PCS_NAD27_Hawaii_zone_3 =	26763,
            PCS_NAD27_Hawaii_zone_4 =	26764,
            PCS_NAD27_Hawaii_zone_5 =	26765,
            PCS_NAD27_Georgia_East =	26766,
            PCS_NAD27_Georgia_West =	26767,
            PCS_NAD27_Idaho_East =	26768,
            PCS_NAD27_Idaho_Central =	26769,
            PCS_NAD27_Idaho_West =	26770,
            PCS_NAD27_Illinois_East =	26771,
            PCS_NAD27_Illinois_West =	26772,
            PCS_NAD27_Indiana_East =	26773,
            PCS_NAD27_BLM_14N_feet =	26774,
            PCS_NAD27_Indiana_West =	26774,
            PCS_NAD27_BLM_15N_feet =	26775,
            PCS_NAD27_Iowa_North =	26775,
            PCS_NAD27_BLM_16N_feet =	26776,
            PCS_NAD27_Iowa_South =	26776,
            PCS_NAD27_BLM_17N_feet =	26777,
            PCS_NAD27_Kansas_North =	26777,
            PCS_NAD27_Kansas_South =	26778,
            PCS_NAD27_Kentucky_North =	26779,
            PCS_NAD27_Kentucky_South =	26780,
            PCS_NAD27_Louisiana_North =	26781,
            PCS_NAD27_Louisiana_South =	26782,
            PCS_NAD27_Maine_East =	26783,
            PCS_NAD27_Maine_West =	26784,
            PCS_NAD27_Maryland =	26785,
            PCS_NAD27_Massachusetts =	26786,
            PCS_NAD27_Massachusetts_Is =	26787,
            PCS_NAD27_Michigan_North =	26788,
            PCS_NAD27_Michigan_Central =	26789,
            PCS_NAD27_Michigan_South =	26790,
            PCS_NAD27_Minnesota_North =	26791,
            PCS_NAD27_Minnesota_Cent =	26792,
            PCS_NAD27_Minnesota_South =	26793,
            PCS_NAD27_Mississippi_East =	26794,
            PCS_NAD27_Mississippi_West =	26795,
            PCS_NAD27_Missouri_East =	26796,
            PCS_NAD27_Missouri_Central =	26797,
            PCS_NAD27_Missouri_West =	26798,
            PCS_NAD_Michigan_Michigan_East =	26801,
            PCS_NAD_Michigan_Michigan_Old_Central =	26802,
            PCS_NAD_Michigan_Michigan_West =	26803,
            PCS_NAD83_UTM_zone_3N =	26903,
            PCS_NAD83_UTM_zone_4N =	26904,
            PCS_NAD83_UTM_zone_5N =	26905,
            PCS_NAD83_UTM_zone_6N =	26906,
            PCS_NAD83_UTM_zone_7N =	26907,
            PCS_NAD83_UTM_zone_8N =	26908,
            PCS_NAD83_UTM_zone_9N =	26909,
            PCS_NAD83_UTM_zone_10N =	26910,
            PCS_NAD83_UTM_zone_11N =	26911,
            PCS_NAD83_UTM_zone_12N =	26912,
            PCS_NAD83_UTM_zone_13N =	26913,
            PCS_NAD83_UTM_zone_14N =	26914,
            PCS_NAD83_UTM_zone_15N =	26915,
            PCS_NAD83_UTM_zone_16N =	26916,
            PCS_NAD83_UTM_zone_17N =	26917,
            PCS_NAD83_UTM_zone_18N =	26918,
            PCS_NAD83_UTM_zone_19N =	26919,
            PCS_NAD83_UTM_zone_20N =	26920,
            PCS_NAD83_UTM_zone_21N =	26921,
            PCS_NAD83_UTM_zone_22N =	26922,
            PCS_NAD83_UTM_zone_23N =	26923,
            PCS_NAD83_Alabama_East =	26929,
            PCS_NAD83_Alabama_West =	26930,
            PCS_NAD83_Alaska_zone_1 =	26931,
            PCS_NAD83_Alaska_zone_2 =	26932,
            PCS_NAD83_Alaska_zone_3 =	26933,
            PCS_NAD83_Alaska_zone_4 =	26934,
            PCS_NAD83_Alaska_zone_5 =	26935,
            PCS_NAD83_Alaska_zone_6 =	26936,
            PCS_NAD83_Alaska_zone_7 =	26937,
            PCS_NAD83_Alaska_zone_8 =	26938,
            PCS_NAD83_Alaska_zone_9 =	26939,
            PCS_NAD83_Alaska_zone_10 =	26940,
            PCS_NAD83_California_1 =	26941,
            PCS_NAD83_California_2 =	26942,
            PCS_NAD83_California_3 =	26943,
            PCS_NAD83_California_4 =	26944,
            PCS_NAD83_California_5 =	26945,
            PCS_NAD83_California_6 =	26946,
            PCS_NAD83_Arizona_East =	26948,
            PCS_NAD83_Arizona_Central =	26949,
            PCS_NAD83_Arizona_West =	26950,
            PCS_NAD83_Arkansas_North =	26951,
            PCS_NAD83_Arkansas_South =	26952,
            PCS_NAD83_Colorado_North =	26953,
            PCS_NAD83_Colorado_Central =	26954,
            PCS_NAD83_Colorado_South =	26955,
            PCS_NAD83_Connecticut =	26956,
            PCS_NAD83_Delaware =	26957,
            PCS_NAD83_Florida_East =	26958,
            PCS_NAD83_Florida_West =	26959,
            PCS_NAD83_Florida_North =	26960,
            PCS_NAD83_Hawaii_zone_1 =	26961,
            PCS_NAD83_Hawaii_zone_2 =	26962,
            PCS_NAD83_Hawaii_zone_3 =	26963,
            PCS_NAD83_Hawaii_zone_4 =	26964,
            PCS_NAD83_Hawaii_zone_5 =	26965,
            PCS_NAD83_Georgia_East =	26966,
            PCS_NAD83_Georgia_West =	26967,
            PCS_NAD83_Idaho_East =	26968,
            PCS_NAD83_Idaho_Central =	26969,
            PCS_NAD83_Idaho_West =	26970,
            PCS_NAD83_Illinois_East =	26971,
            PCS_NAD83_Illinois_West =	26972,
            PCS_NAD83_Indiana_East =	26973,
            PCS_NAD83_Indiana_West =	26974,
            PCS_NAD83_Iowa_North =	26975,
            PCS_NAD83_Iowa_South =	26976,
            PCS_NAD83_Kansas_North =	26977,
            PCS_NAD83_Kansas_South =	26978,
            PCS_NAD83_Kentucky_North =	26979,
            PCS_NAD83_Kentucky_South =	26980,
            PCS_NAD83_Louisiana_North =	26981,
            PCS_NAD83_Louisiana_South =	26982,
            PCS_NAD83_Maine_East =	26983,
            PCS_NAD83_Maine_West =	26984,
            PCS_NAD83_Maryland =	26985,
            PCS_NAD83_Massachusetts =	26986,
            PCS_NAD83_Massachusetts_Is =	26987,
            PCS_NAD83_Michigan_North =	26988,
            PCS_NAD83_Michigan_Central =	26989,
            PCS_NAD83_Michigan_South =	26990,
            PCS_NAD83_Minnesota_North =	26991,
            PCS_NAD83_Minnesota_Cent =	26992,
            PCS_NAD83_Minnesota_South =	26993,
            PCS_NAD83_Mississippi_East =	26994,
            PCS_NAD83_Mississippi_West =	26995,
            PCS_NAD83_Missouri_East =	26996,
            PCS_NAD83_Missouri_Central =	26997,
            PCS_NAD83_Missouri_West =	26998,
            PCS_Nahrwan_1967_UTM_38N =	27038,
            PCS_Nahrwan_1967_UTM_39N =	27039,
            PCS_Nahrwan_1967_UTM_40N =	27040,
            PCS_Naparima_UTM_20N =	27120,
            PCS_GD49_NZ_Map_Grid =	27200,
            PCS_GD49_North_Island_Grid =	27291,
            PCS_GD49_South_Island_Grid =	27292,
            PCS_Datum_73_UTM_zone_29N =	27429,
            PCS_ATF_Nord_de_Guerre =	27500,
            PCS_NTF_France_I =	27581,
            PCS_NTF_France_II =	27582,
            PCS_NTF_France_III =	27583,
            PCS_NTF_Nord_France =	27591,
            PCS_NTF_Centre_France =	27592,
            PCS_NTF_Sud_France =	27593,
            PCS_British_National_Grid =	27700,
            PCS_Point_Noire_UTM_32S =	28232,
            PCS_GDA94_MGA_zone_48 =	28348,
            PCS_GDA94_MGA_zone_49 =	28349,
            PCS_GDA94_MGA_zone_50 =	28350,
            PCS_GDA94_MGA_zone_51 =	28351,
            PCS_GDA94_MGA_zone_52 =	28352,
            PCS_GDA94_MGA_zone_53 =	28353,
            PCS_GDA94_MGA_zone_54 =	28354,
            PCS_GDA94_MGA_zone_55 =	28355,
            PCS_GDA94_MGA_zone_56 =	28356,
            PCS_GDA94_MGA_zone_57 =	28357,
            PCS_GDA94_MGA_zone_58 =	28358,
            PCS_Pulkovo_Gauss_zone_4 =	28404,
            PCS_Pulkovo_Gauss_zone_5 =	28405,
            PCS_Pulkovo_Gauss_zone_6 =	28406,
            PCS_Pulkovo_Gauss_zone_7 =	28407,
            PCS_Pulkovo_Gauss_zone_8 =	28408,
            PCS_Pulkovo_Gauss_zone_9 =	28409,
            PCS_Pulkovo_Gauss_zone_10 =	28410,
            PCS_Pulkovo_Gauss_zone_11 =	28411,
            PCS_Pulkovo_Gauss_zone_12 =	28412,
            PCS_Pulkovo_Gauss_zone_13 =	28413,
            PCS_Pulkovo_Gauss_zone_14 =	28414,
            PCS_Pulkovo_Gauss_zone_15 =	28415,
            PCS_Pulkovo_Gauss_zone_16 =	28416,
            PCS_Pulkovo_Gauss_zone_17 =	28417,
            PCS_Pulkovo_Gauss_zone_18 =	28418,
            PCS_Pulkovo_Gauss_zone_19 =	28419,
            PCS_Pulkovo_Gauss_zone_20 =	28420,
            PCS_Pulkovo_Gauss_zone_21 =	28421,
            PCS_Pulkovo_Gauss_zone_22 =	28422,
            PCS_Pulkovo_Gauss_zone_23 =	28423,
            PCS_Pulkovo_Gauss_zone_24 =	28424,
            PCS_Pulkovo_Gauss_zone_25 =	28425,
            PCS_Pulkovo_Gauss_zone_26 =	28426,
            PCS_Pulkovo_Gauss_zone_27 =	28427,
            PCS_Pulkovo_Gauss_zone_28 =	28428,
            PCS_Pulkovo_Gauss_zone_29 =	28429,
            PCS_Pulkovo_Gauss_zone_30 =	28430,
            PCS_Pulkovo_Gauss_zone_31 =	28431,
            PCS_Pulkovo_Gauss_zone_32 =	28432,
            PCS_Pulkovo_Gauss_4N =	28464,
            PCS_Pulkovo_Gauss_5N =	28465,
            PCS_Pulkovo_Gauss_6N =	28466,
            PCS_Pulkovo_Gauss_7N =	28467,
            PCS_Pulkovo_Gauss_8N =	28468,
            PCS_Pulkovo_Gauss_9N =	28469,
            PCS_Pulkovo_Gauss_10N =	28470,
            PCS_Pulkovo_Gauss_11N =	28471,
            PCS_Pulkovo_Gauss_12N =	28472,
            PCS_Pulkovo_Gauss_13N =	28473,
            PCS_Pulkovo_Gauss_14N =	28474,
            PCS_Pulkovo_Gauss_15N =	28475,
            PCS_Pulkovo_Gauss_16N =	28476,
            PCS_Pulkovo_Gauss_17N =	28477,
            PCS_Pulkovo_Gauss_18N =	28478,
            PCS_Pulkovo_Gauss_19N =	28479,
            PCS_Pulkovo_Gauss_20N =	28480,
            PCS_Pulkovo_Gauss_21N =	28481,
            PCS_Pulkovo_Gauss_22N =	28482,
            PCS_Pulkovo_Gauss_23N =	28483,
            PCS_Pulkovo_Gauss_24N =	28484,
            PCS_Pulkovo_Gauss_25N =	28485,
            PCS_Pulkovo_Gauss_26N =	28486,
            PCS_Pulkovo_Gauss_27N =	28487,
            PCS_Pulkovo_Gauss_28N =	28488,
            PCS_Pulkovo_Gauss_29N =	28489,
            PCS_Pulkovo_Gauss_30N =	28490,
            PCS_Pulkovo_Gauss_31N =	28491,
            PCS_Pulkovo_Gauss_32N =	28492,
            PCS_Qatar_National_Grid =	28600,
            PCS_RD_Netherlands_Old =	28991,
            PCS_RD_Netherlands_New =	28992,
            PCS_SAD69_UTM_zone_18N =	29118,
            PCS_SAD69_UTM_zone_19N =	29119,
            PCS_SAD69_UTM_zone_20N =	29120,
            PCS_SAD69_UTM_zone_21N =	29121,
            PCS_SAD69_UTM_zone_22N =	29122,
            PCS_SAD69_UTM_zone_17S =	29177,
            PCS_SAD69_UTM_zone_18S =	29178,
            PCS_SAD69_UTM_zone_19S =	29179,
            PCS_SAD69_UTM_zone_20S =	29180,
            PCS_SAD69_UTM_zone_21S =	29181,
            PCS_SAD69_UTM_zone_22S =	29182,
            PCS_SAD69_UTM_zone_23S =	29183,
            PCS_SAD69_UTM_zone_24S =	29184,
            PCS_SAD69_UTM_zone_25S =	29185,
            PCS_Sapper_Hill_UTM_20S =	29220,
            PCS_Sapper_Hill_UTM_21S =	29221,
            PCS_Schwarzeck_UTM_33S =	29333,
            PCS_Sudan_UTM_zone_35N =	29635,
            PCS_Sudan_UTM_zone_36N =	29636,
            PCS_Tananarive_Laborde =	29700,
            PCS_Tananarive_UTM_38S =	29738,
            PCS_Tananarive_UTM_39S =	29739,
            PCS_Timbalai_1948_Borneo =	29800,
            PCS_Timbalai_1948_UTM_49N =	29849,
            PCS_Timbalai_1948_UTM_50N =	29850,
            PCS_TM65_Irish_Nat_Grid =	29900,
            PCS_Trinidad_1903_Trinidad =	30200,
            PCS_TC_1948_UTM_zone_39N =	30339,
            PCS_TC_1948_UTM_zone_40N =	30340,
            PCS_Voirol_N_Algerie_ancien =	30491,
            PCS_Voirol_S_Algerie_ancien =	30492,
            PCS_Voirol_Unifie_N_Algerie =	30591,
            PCS_Voirol_Unifie_S_Algerie =	30592,
            PCS_Bern_1938_Swiss_New =	30600,
            PCS_Nord_Sahara_UTM_29N =	30729,
            PCS_Nord_Sahara_UTM_30N =	30730,
            PCS_Nord_Sahara_UTM_31N =	30731,
            PCS_Nord_Sahara_UTM_32N =	30732,
            PCS_Yoff_UTM_zone_28N =	31028,
            PCS_Zanderij_UTM_zone_21N =	31121,
            PCS_MGI_Austria_West =	31291,
            PCS_MGI_Austria_Central =	31292,
            PCS_MGI_Austria_East =	31293,
            PCS_Belge_Lambert_72 =	31300,
            PCS_DHDN_Germany_zone_1 =	31491,
            PCS_DHDN_Germany_zone_2 =	31492,
            PCS_DHDN_Germany_zone_3 =	31493,
            PCS_DHDN_Germany_zone_4 =	31494,
            PCS_DHDN_Germany_zone_5 =	31495,
            PCS_NAD27_Montana_North =	32001,
            PCS_NAD27_Montana_Central =	32002,
            PCS_NAD27_Montana_South =	32003,
            PCS_NAD27_Nebraska_North =	32005,
            PCS_NAD27_Nebraska_South =	32006,
            PCS_NAD27_Nevada_East =	32007,
            PCS_NAD27_Nevada_Central =	32008,
            PCS_NAD27_Nevada_West =	32009,
            PCS_NAD27_New_Hampshire =	32010,
            PCS_NAD27_New_Jersey =	32011,
            PCS_NAD27_New_Mexico_East =	32012,
            PCS_NAD27_New_Mexico_Cent =	32013,
            PCS_NAD27_New_Mexico_West =	32014,
            PCS_NAD27_New_York_East =	32015,
            PCS_NAD27_New_York_Central =	32016,
            PCS_NAD27_New_York_West =	32017,
            PCS_NAD27_New_York_Long_Is =	32018,
            PCS_NAD27_North_Carolina =	32019,
            PCS_NAD27_North_Dakota_N =	32020,
            PCS_NAD27_North_Dakota_S =	32021,
            PCS_NAD27_Ohio_North =	32022,
            PCS_NAD27_Ohio_South =	32023,
            PCS_NAD27_Oklahoma_North =	32024,
            PCS_NAD27_Oklahoma_South =	32025,
            PCS_NAD27_Oregon_North =	32026,
            PCS_NAD27_Oregon_South =	32027,
            PCS_NAD27_Pennsylvania_N =	32028,
            PCS_NAD27_Pennsylvania_S =	32029,
            PCS_NAD27_Rhode_Island =	32030,
            PCS_NAD27_South_Carolina_N =	32031,
            PCS_NAD27_South_Carolina_S =	32033,
            PCS_NAD27_South_Dakota_N =	32034,
            PCS_NAD27_South_Dakota_S =	32035,
            PCS_NAD27_Tennessee =	32036,
            PCS_NAD27_Texas_North =	32037,
            PCS_NAD27_Texas_North_Cen =	32038,
            PCS_NAD27_Texas_Central =	32039,
            PCS_NAD27_Texas_South_Cen =	32040,
            PCS_NAD27_Texas_South =	32041,
            PCS_NAD27_Utah_North =	32042,
            PCS_NAD27_Utah_Central =	32043,
            PCS_NAD27_Utah_South =	32044,
            PCS_NAD27_Vermont =	32045,
            PCS_NAD27_Virginia_North =	32046,
            PCS_NAD27_Virginia_South =	32047,
            PCS_NAD27_Washington_North =	32048,
            PCS_NAD27_Washington_South =	32049,
            PCS_NAD27_West_Virginia_N =	32050,
            PCS_NAD27_West_Virginia_S =	32051,
            PCS_NAD27_Wisconsin_North =	32052,
            PCS_NAD27_Wisconsin_Cen =	32053,
            PCS_NAD27_Wisconsin_South =	32054,
            PCS_NAD27_Wyoming_East =	32055,
            PCS_NAD27_Wyoming_E_Cen =	32056,
            PCS_NAD27_Wyoming_W_Cen =	32057,
            PCS_NAD27_Wyoming_West =	32058,
            PCS_NAD27_Puerto_Rico =	32059,
            PCS_NAD27_St_Croix =	32060,
            PCS_NAD83_Montana =	32100,
            PCS_NAD83_Nebraska =	32104,
            PCS_NAD83_Nevada_East =	32107,
            PCS_NAD83_Nevada_Central =	32108,
            PCS_NAD83_Nevada_West =	32109,
            PCS_NAD83_New_Hampshire =	32110,
            PCS_NAD83_New_Jersey =	32111,
            PCS_NAD83_New_Mexico_East =	32112,
            PCS_NAD83_New_Mexico_Cent =	32113,
            PCS_NAD83_New_Mexico_West =	32114,
            PCS_NAD83_New_York_East =	32115,
            PCS_NAD83_New_York_Central =	32116,
            PCS_NAD83_New_York_West =	32117,
            PCS_NAD83_New_York_Long_Is =	32118,
            PCS_NAD83_North_Carolina =	32119,
            PCS_NAD83_North_Dakota_N =	32120,
            PCS_NAD83_North_Dakota_S =	32121,
            PCS_NAD83_Ohio_North =	32122,
            PCS_NAD83_Ohio_South =	32123,
            PCS_NAD83_Oklahoma_North =	32124,
            PCS_NAD83_Oklahoma_South =	32125,
            PCS_NAD83_Oregon_North =	32126,
            PCS_NAD83_Oregon_South =	32127,
            PCS_NAD83_Pennsylvania_N =	32128,
            PCS_NAD83_Pennsylvania_S =	32129,
            PCS_NAD83_Rhode_Island =	32130,
            PCS_NAD83_South_Carolina =	32133,
            PCS_NAD83_South_Dakota_N =	32134,
            PCS_NAD83_South_Dakota_S =	32135,
            PCS_NAD83_Tennessee =	32136,
            PCS_NAD83_Texas_North =	32137,
            PCS_NAD83_Texas_North_Cen =	32138,
            PCS_NAD83_Texas_Central =	32139,
            PCS_NAD83_Texas_South_Cen =	32140,
            PCS_NAD83_Texas_South =	32141,
            PCS_NAD83_Utah_North =	32142,
            PCS_NAD83_Utah_Central =	32143,
            PCS_NAD83_Utah_South =	32144,
            PCS_NAD83_Vermont =	32145,
            PCS_NAD83_Virginia_North =	32146,
            PCS_NAD83_Virginia_South =	32147,
            PCS_NAD83_Washington_North =	32148,
            PCS_NAD83_Washington_South =	32149,
            PCS_NAD83_West_Virginia_N =	32150,
            PCS_NAD83_West_Virginia_S =	32151,
            PCS_NAD83_Wisconsin_North =	32152,
            PCS_NAD83_Wisconsin_Cen =	32153,
            PCS_NAD83_Wisconsin_South =	32154,
            PCS_NAD83_Wyoming_East =	32155,
            PCS_NAD83_Wyoming_E_Cen =	32156,
            PCS_NAD83_Wyoming_W_Cen =	32157,
            PCS_NAD83_Wyoming_West =	32158,
            PCS_NAD83_Puerto_Rico_Virgin_Is =	32161,
            PCS_WGS72_UTM_zone_1N =	32201,
            PCS_WGS72_UTM_zone_2N =	32202,
            PCS_WGS72_UTM_zone_3N =	32203,
            PCS_WGS72_UTM_zone_4N =	32204,
            PCS_WGS72_UTM_zone_5N =	32205,
            PCS_WGS72_UTM_zone_6N =	32206,
            PCS_WGS72_UTM_zone_7N =	32207,
            PCS_WGS72_UTM_zone_8N =	32208,
            PCS_WGS72_UTM_zone_9N =	32209,
            PCS_WGS72_UTM_zone_10N =	32210,
            PCS_WGS72_UTM_zone_11N =	32211,
            PCS_WGS72_UTM_zone_12N =	32212,
            PCS_WGS72_UTM_zone_13N =	32213,
            PCS_WGS72_UTM_zone_14N =	32214,
            PCS_WGS72_UTM_zone_15N =	32215,
            PCS_WGS72_UTM_zone_16N =	32216,
            PCS_WGS72_UTM_zone_17N =	32217,
            PCS_WGS72_UTM_zone_18N =	32218,
            PCS_WGS72_UTM_zone_19N =	32219,
            PCS_WGS72_UTM_zone_20N =	32220,
            PCS_WGS72_UTM_zone_21N =	32221,
            PCS_WGS72_UTM_zone_22N =	32222,
            PCS_WGS72_UTM_zone_23N =	32223,
            PCS_WGS72_UTM_zone_24N =	32224,
            PCS_WGS72_UTM_zone_25N =	32225,
            PCS_WGS72_UTM_zone_26N =	32226,
            PCS_WGS72_UTM_zone_27N =	32227,
            PCS_WGS72_UTM_zone_28N =	32228,
            PCS_WGS72_UTM_zone_29N =	32229,
            PCS_WGS72_UTM_zone_30N =	32230,
            PCS_WGS72_UTM_zone_31N =	32231,
            PCS_WGS72_UTM_zone_32N =	32232,
            PCS_WGS72_UTM_zone_33N =	32233,
            PCS_WGS72_UTM_zone_34N =	32234,
            PCS_WGS72_UTM_zone_35N =	32235,
            PCS_WGS72_UTM_zone_36N =	32236,
            PCS_WGS72_UTM_zone_37N =	32237,
            PCS_WGS72_UTM_zone_38N =	32238,
            PCS_WGS72_UTM_zone_39N =	32239,
            PCS_WGS72_UTM_zone_40N =	32240,
            PCS_WGS72_UTM_zone_41N =	32241,
            PCS_WGS72_UTM_zone_42N =	32242,
            PCS_WGS72_UTM_zone_43N =	32243,
            PCS_WGS72_UTM_zone_44N =	32244,
            PCS_WGS72_UTM_zone_45N =	32245,
            PCS_WGS72_UTM_zone_46N =	32246,
            PCS_WGS72_UTM_zone_47N =	32247,
            PCS_WGS72_UTM_zone_48N =	32248,
            PCS_WGS72_UTM_zone_49N =	32249,
            PCS_WGS72_UTM_zone_50N =	32250,
            PCS_WGS72_UTM_zone_51N =	32251,
            PCS_WGS72_UTM_zone_52N =	32252,
            PCS_WGS72_UTM_zone_53N =	32253,
            PCS_WGS72_UTM_zone_54N =	32254,
            PCS_WGS72_UTM_zone_55N =	32255,
            PCS_WGS72_UTM_zone_56N =	32256,
            PCS_WGS72_UTM_zone_57N =	32257,
            PCS_WGS72_UTM_zone_58N =	32258,
            PCS_WGS72_UTM_zone_59N =	32259,
            PCS_WGS72_UTM_zone_60N =	32260,
            PCS_WGS72_UTM_zone_1S =	32301,
            PCS_WGS72_UTM_zone_2S =	32302,
            PCS_WGS72_UTM_zone_3S =	32303,
            PCS_WGS72_UTM_zone_4S =	32304,
            PCS_WGS72_UTM_zone_5S =	32305,
            PCS_WGS72_UTM_zone_6S =	32306,
            PCS_WGS72_UTM_zone_7S =	32307,
            PCS_WGS72_UTM_zone_8S =	32308,
            PCS_WGS72_UTM_zone_9S =	32309,
            PCS_WGS72_UTM_zone_10S =	32310,
            PCS_WGS72_UTM_zone_11S =	32311,
            PCS_WGS72_UTM_zone_12S =	32312,
            PCS_WGS72_UTM_zone_13S =	32313,
            PCS_WGS72_UTM_zone_14S =	32314,
            PCS_WGS72_UTM_zone_15S =	32315,
            PCS_WGS72_UTM_zone_16S =	32316,
            PCS_WGS72_UTM_zone_17S =	32317,
            PCS_WGS72_UTM_zone_18S =	32318,
            PCS_WGS72_UTM_zone_19S =	32319,
            PCS_WGS72_UTM_zone_20S =	32320,
            PCS_WGS72_UTM_zone_21S =	32321,
            PCS_WGS72_UTM_zone_22S =	32322,
            PCS_WGS72_UTM_zone_23S =	32323,
            PCS_WGS72_UTM_zone_24S =	32324,
            PCS_WGS72_UTM_zone_25S =	32325,
            PCS_WGS72_UTM_zone_26S =	32326,
            PCS_WGS72_UTM_zone_27S =	32327,
            PCS_WGS72_UTM_zone_28S =	32328,
            PCS_WGS72_UTM_zone_29S =	32329,
            PCS_WGS72_UTM_zone_30S =	32330,
            PCS_WGS72_UTM_zone_31S =	32331,
            PCS_WGS72_UTM_zone_32S =	32332,
            PCS_WGS72_UTM_zone_33S =	32333,
            PCS_WGS72_UTM_zone_34S =	32334,
            PCS_WGS72_UTM_zone_35S =	32335,
            PCS_WGS72_UTM_zone_36S =	32336,
            PCS_WGS72_UTM_zone_37S =	32337,
            PCS_WGS72_UTM_zone_38S =	32338,
            PCS_WGS72_UTM_zone_39S =	32339,
            PCS_WGS72_UTM_zone_40S =	32340,
            PCS_WGS72_UTM_zone_41S =	32341,
            PCS_WGS72_UTM_zone_42S =	32342,
            PCS_WGS72_UTM_zone_43S =	32343,
            PCS_WGS72_UTM_zone_44S =	32344,
            PCS_WGS72_UTM_zone_45S =	32345,
            PCS_WGS72_UTM_zone_46S =	32346,
            PCS_WGS72_UTM_zone_47S =	32347,
            PCS_WGS72_UTM_zone_48S =	32348,
            PCS_WGS72_UTM_zone_49S =	32349,
            PCS_WGS72_UTM_zone_50S =	32350,
            PCS_WGS72_UTM_zone_51S =	32351,
            PCS_WGS72_UTM_zone_52S =	32352,
            PCS_WGS72_UTM_zone_53S =	32353,
            PCS_WGS72_UTM_zone_54S =	32354,
            PCS_WGS72_UTM_zone_55S =	32355,
            PCS_WGS72_UTM_zone_56S =	32356,
            PCS_WGS72_UTM_zone_57S =	32357,
            PCS_WGS72_UTM_zone_58S =	32358,
            PCS_WGS72_UTM_zone_59S =	32359,
            PCS_WGS72_UTM_zone_60S =	32360,
            PCS_WGS72BE_UTM_zone_1N =	32401,
            PCS_WGS72BE_UTM_zone_2N =	32402,
            PCS_WGS72BE_UTM_zone_3N =	32403,
            PCS_WGS72BE_UTM_zone_4N =	32404,
            PCS_WGS72BE_UTM_zone_5N =	32405,
            PCS_WGS72BE_UTM_zone_6N =	32406,
            PCS_WGS72BE_UTM_zone_7N =	32407,
            PCS_WGS72BE_UTM_zone_8N =	32408,
            PCS_WGS72BE_UTM_zone_9N =	32409,
            PCS_WGS72BE_UTM_zone_10N =	32410,
            PCS_WGS72BE_UTM_zone_11N =	32411,
            PCS_WGS72BE_UTM_zone_12N =	32412,
            PCS_WGS72BE_UTM_zone_13N =	32413,
            PCS_WGS72BE_UTM_zone_14N =	32414,
            PCS_WGS72BE_UTM_zone_15N =	32415,
            PCS_WGS72BE_UTM_zone_16N =	32416,
            PCS_WGS72BE_UTM_zone_17N =	32417,
            PCS_WGS72BE_UTM_zone_18N =	32418,
            PCS_WGS72BE_UTM_zone_19N =	32419,
            PCS_WGS72BE_UTM_zone_20N =	32420,
            PCS_WGS72BE_UTM_zone_21N =	32421,
            PCS_WGS72BE_UTM_zone_22N =	32422,
            PCS_WGS72BE_UTM_zone_23N =	32423,
            PCS_WGS72BE_UTM_zone_24N =	32424,
            PCS_WGS72BE_UTM_zone_25N =	32425,
            PCS_WGS72BE_UTM_zone_26N =	32426,
            PCS_WGS72BE_UTM_zone_27N =	32427,
            PCS_WGS72BE_UTM_zone_28N =	32428,
            PCS_WGS72BE_UTM_zone_29N =	32429,
            PCS_WGS72BE_UTM_zone_30N =	32430,
            PCS_WGS72BE_UTM_zone_31N =	32431,
            PCS_WGS72BE_UTM_zone_32N =	32432,
            PCS_WGS72BE_UTM_zone_33N =	32433,
            PCS_WGS72BE_UTM_zone_34N =	32434,
            PCS_WGS72BE_UTM_zone_35N =	32435,
            PCS_WGS72BE_UTM_zone_36N =	32436,
            PCS_WGS72BE_UTM_zone_37N =	32437,
            PCS_WGS72BE_UTM_zone_38N =	32438,
            PCS_WGS72BE_UTM_zone_39N =	32439,
            PCS_WGS72BE_UTM_zone_40N =	32440,
            PCS_WGS72BE_UTM_zone_41N =	32441,
            PCS_WGS72BE_UTM_zone_42N =	32442,
            PCS_WGS72BE_UTM_zone_43N =	32443,
            PCS_WGS72BE_UTM_zone_44N =	32444,
            PCS_WGS72BE_UTM_zone_45N =	32445,
            PCS_WGS72BE_UTM_zone_46N =	32446,
            PCS_WGS72BE_UTM_zone_47N =	32447,
            PCS_WGS72BE_UTM_zone_48N =	32448,
            PCS_WGS72BE_UTM_zone_49N =	32449,
            PCS_WGS72BE_UTM_zone_50N =	32450,
            PCS_WGS72BE_UTM_zone_51N =	32451,
            PCS_WGS72BE_UTM_zone_52N =	32452,
            PCS_WGS72BE_UTM_zone_53N =	32453,
            PCS_WGS72BE_UTM_zone_54N =	32454,
            PCS_WGS72BE_UTM_zone_55N =	32455,
            PCS_WGS72BE_UTM_zone_56N =	32456,
            PCS_WGS72BE_UTM_zone_57N =	32457,
            PCS_WGS72BE_UTM_zone_58N =	32458,
            PCS_WGS72BE_UTM_zone_59N =	32459,
            PCS_WGS72BE_UTM_zone_60N =	32460,
            PCS_WGS72BE_UTM_zone_1S =	32501,
            PCS_WGS72BE_UTM_zone_2S =	32502,
            PCS_WGS72BE_UTM_zone_3S =	32503,
            PCS_WGS72BE_UTM_zone_4S =	32504,
            PCS_WGS72BE_UTM_zone_5S =	32505,
            PCS_WGS72BE_UTM_zone_6S =	32506,
            PCS_WGS72BE_UTM_zone_7S =	32507,
            PCS_WGS72BE_UTM_zone_8S =	32508,
            PCS_WGS72BE_UTM_zone_9S =	32509,
            PCS_WGS72BE_UTM_zone_10S =	32510,
            PCS_WGS72BE_UTM_zone_11S =	32511,
            PCS_WGS72BE_UTM_zone_12S =	32512,
            PCS_WGS72BE_UTM_zone_13S =	32513,
            PCS_WGS72BE_UTM_zone_14S =	32514,
            PCS_WGS72BE_UTM_zone_15S =	32515,
            PCS_WGS72BE_UTM_zone_16S =	32516,
            PCS_WGS72BE_UTM_zone_17S =	32517,
            PCS_WGS72BE_UTM_zone_18S =	32518,
            PCS_WGS72BE_UTM_zone_19S =	32519,
            PCS_WGS72BE_UTM_zone_20S =	32520,
            PCS_WGS72BE_UTM_zone_21S =	32521,
            PCS_WGS72BE_UTM_zone_22S =	32522,
            PCS_WGS72BE_UTM_zone_23S =	32523,
            PCS_WGS72BE_UTM_zone_24S =	32524,
            PCS_WGS72BE_UTM_zone_25S =	32525,
            PCS_WGS72BE_UTM_zone_26S =	32526,
            PCS_WGS72BE_UTM_zone_27S =	32527,
            PCS_WGS72BE_UTM_zone_28S =	32528,
            PCS_WGS72BE_UTM_zone_29S =	32529,
            PCS_WGS72BE_UTM_zone_30S =	32530,
            PCS_WGS72BE_UTM_zone_31S =	32531,
            PCS_WGS72BE_UTM_zone_32S =	32532,
            PCS_WGS72BE_UTM_zone_33S =	32533,
            PCS_WGS72BE_UTM_zone_34S =	32534,
            PCS_WGS72BE_UTM_zone_35S =	32535,
            PCS_WGS72BE_UTM_zone_36S =	32536,
            PCS_WGS72BE_UTM_zone_37S =	32537,
            PCS_WGS72BE_UTM_zone_38S =	32538,
            PCS_WGS72BE_UTM_zone_39S =	32539,
            PCS_WGS72BE_UTM_zone_40S =	32540,
            PCS_WGS72BE_UTM_zone_41S =	32541,
            PCS_WGS72BE_UTM_zone_42S =	32542,
            PCS_WGS72BE_UTM_zone_43S =	32543,
            PCS_WGS72BE_UTM_zone_44S =	32544,
            PCS_WGS72BE_UTM_zone_45S =	32545,
            PCS_WGS72BE_UTM_zone_46S =	32546,
            PCS_WGS72BE_UTM_zone_47S =	32547,
            PCS_WGS72BE_UTM_zone_48S =	32548,
            PCS_WGS72BE_UTM_zone_49S =	32549,
            PCS_WGS72BE_UTM_zone_50S =	32550,
            PCS_WGS72BE_UTM_zone_51S =	32551,
            PCS_WGS72BE_UTM_zone_52S =	32552,
            PCS_WGS72BE_UTM_zone_53S =	32553,
            PCS_WGS72BE_UTM_zone_54S =	32554,
            PCS_WGS72BE_UTM_zone_55S =	32555,
            PCS_WGS72BE_UTM_zone_56S =	32556,
            PCS_WGS72BE_UTM_zone_57S =	32557,
            PCS_WGS72BE_UTM_zone_58S =	32558,
            PCS_WGS72BE_UTM_zone_59S =	32559,
            PCS_WGS72BE_UTM_zone_60S =	32560,
            PCS_WGS84_UTM_zone_1N =	32601,
            PCS_WGS84_UTM_zone_2N =	32602,
            PCS_WGS84_UTM_zone_3N =	32603,
            PCS_WGS84_UTM_zone_4N =	32604,
            PCS_WGS84_UTM_zone_5N =	32605,
            PCS_WGS84_UTM_zone_6N =	32606,
            PCS_WGS84_UTM_zone_7N =	32607,
            PCS_WGS84_UTM_zone_8N =	32608,
            PCS_WGS84_UTM_zone_9N =	32609,
            PCS_WGS84_UTM_zone_10N =	32610,
            PCS_WGS84_UTM_zone_11N =	32611,
            PCS_WGS84_UTM_zone_12N =	32612,
            PCS_WGS84_UTM_zone_13N =	32613,
            PCS_WGS84_UTM_zone_14N =	32614,
            PCS_WGS84_UTM_zone_15N =	32615,
            PCS_WGS84_UTM_zone_16N =	32616,
            PCS_WGS84_UTM_zone_17N =	32617,
            PCS_WGS84_UTM_zone_18N =	32618,
            PCS_WGS84_UTM_zone_19N =	32619,
            PCS_WGS84_UTM_zone_20N =	32620,
            PCS_WGS84_UTM_zone_21N =	32621,
            PCS_WGS84_UTM_zone_22N =	32622,
            PCS_WGS84_UTM_zone_23N =	32623,
            PCS_WGS84_UTM_zone_24N =	32624,
            PCS_WGS84_UTM_zone_25N =	32625,
            PCS_WGS84_UTM_zone_26N =	32626,
            PCS_WGS84_UTM_zone_27N =	32627,
            PCS_WGS84_UTM_zone_28N =	32628,
            PCS_WGS84_UTM_zone_29N =	32629,
            PCS_WGS84_UTM_zone_30N =	32630,
            PCS_WGS84_UTM_zone_31N =	32631,
            PCS_WGS84_UTM_zone_32N =	32632,
            PCS_WGS84_UTM_zone_33N =	32633,
            PCS_WGS84_UTM_zone_34N =	32634,
            PCS_WGS84_UTM_zone_35N =	32635,
            PCS_WGS84_UTM_zone_36N =	32636,
            PCS_WGS84_UTM_zone_37N =	32637,
            PCS_WGS84_UTM_zone_38N =	32638,
            PCS_WGS84_UTM_zone_39N =	32639,
            PCS_WGS84_UTM_zone_40N =	32640,
            PCS_WGS84_UTM_zone_41N =	32641,
            PCS_WGS84_UTM_zone_42N =	32642,
            PCS_WGS84_UTM_zone_43N =	32643,
            PCS_WGS84_UTM_zone_44N =	32644,
            PCS_WGS84_UTM_zone_45N =	32645,
            PCS_WGS84_UTM_zone_46N =	32646,
            PCS_WGS84_UTM_zone_47N =	32647,
            PCS_WGS84_UTM_zone_48N =	32648,
            PCS_WGS84_UTM_zone_49N =	32649,
            PCS_WGS84_UTM_zone_50N =	32650,
            PCS_WGS84_UTM_zone_51N =	32651,
            PCS_WGS84_UTM_zone_52N =	32652,
            PCS_WGS84_UTM_zone_53N =	32653,
            PCS_WGS84_UTM_zone_54N =	32654,
            PCS_WGS84_UTM_zone_55N =	32655,
            PCS_WGS84_UTM_zone_56N =	32656,
            PCS_WGS84_UTM_zone_57N =	32657,
            PCS_WGS84_UTM_zone_58N =	32658,
            PCS_WGS84_UTM_zone_59N =	32659,
            PCS_WGS84_UTM_zone_60N =	32660,
            PCS_WGS84_UTM_zone_1S =	32701,
            PCS_WGS84_UTM_zone_2S =	32702,
            PCS_WGS84_UTM_zone_3S =	32703,
            PCS_WGS84_UTM_zone_4S =	32704,
            PCS_WGS84_UTM_zone_5S =	32705,
            PCS_WGS84_UTM_zone_6S =	32706,
            PCS_WGS84_UTM_zone_7S =	32707,
            PCS_WGS84_UTM_zone_8S =	32708,
            PCS_WGS84_UTM_zone_9S =	32709,
            PCS_WGS84_UTM_zone_10S =	32710,
            PCS_WGS84_UTM_zone_11S =	32711,
            PCS_WGS84_UTM_zone_12S =	32712,
            PCS_WGS84_UTM_zone_13S =	32713,
            PCS_WGS84_UTM_zone_14S =	32714,
            PCS_WGS84_UTM_zone_15S =	32715,
            PCS_WGS84_UTM_zone_16S =	32716,
            PCS_WGS84_UTM_zone_17S =	32717,
            PCS_WGS84_UTM_zone_18S =	32718,
            PCS_WGS84_UTM_zone_19S =	32719,
            PCS_WGS84_UTM_zone_20S =	32720,
            PCS_WGS84_UTM_zone_21S =	32721,
            PCS_WGS84_UTM_zone_22S =	32722,
            PCS_WGS84_UTM_zone_23S =	32723,
            PCS_WGS84_UTM_zone_24S =	32724,
            PCS_WGS84_UTM_zone_25S =	32725,
            PCS_WGS84_UTM_zone_26S =	32726,
            PCS_WGS84_UTM_zone_27S =	32727,
            PCS_WGS84_UTM_zone_28S =	32728,
            PCS_WGS84_UTM_zone_29S =	32729,
            PCS_WGS84_UTM_zone_30S =	32730,
            PCS_WGS84_UTM_zone_31S =	32731,
           PCS_WGS84_UTM_zone_32S =	32732,
            PCS_WGS84_UTM_zone_33S =	32733,
            PCS_WGS84_UTM_zone_34S =	32734,
            PCS_WGS84_UTM_zone_35S =	32735,
            PCS_WGS84_UTM_zone_36S =	32736,
            PCS_WGS84_UTM_zone_37S =	32737,
            PCS_WGS84_UTM_zone_38S =	32738,
            PCS_WGS84_UTM_zone_39S =	32739,
            PCS_WGS84_UTM_zone_40S =	32740,
            PCS_WGS84_UTM_zone_41S =	32741,
            PCS_WGS84_UTM_zone_42S =	32742,
            PCS_WGS84_UTM_zone_43S =	32743,
            PCS_WGS84_UTM_zone_44S =	32744,
            PCS_WGS84_UTM_zone_45S =	32745,
            PCS_WGS84_UTM_zone_46S =	32746,
            PCS_WGS84_UTM_zone_47S =	32747,
            PCS_WGS84_UTM_zone_48S =	32748,
            PCS_WGS84_UTM_zone_49S =	32749,
            PCS_WGS84_UTM_zone_50S =	32750,
            PCS_WGS84_UTM_zone_51S =	32751,
            PCS_WGS84_UTM_zone_52S =	32752,
            PCS_WGS84_UTM_zone_53S =	32753,
            PCS_WGS84_UTM_zone_54S =	32754,
            PCS_WGS84_UTM_zone_55S =	32755,
            PCS_WGS84_UTM_zone_56S =	32756,
            PCS_WGS84_UTM_zone_57S =	32757,
            PCS_WGS84_UTM_zone_58S =	32758,
            PCS_WGS84_UTM_zone_59S =	32759,
            PCS_WGS84_UTM_zone_60S =	32760,
        }

        public enum ProjectionCodes
        {
            Proj_Alabama_CS27_East =	10101,
            Proj_Alabama_CS27_West =	10102,
            Proj_Alabama_CS83_East =	10131,
            Proj_Alabama_CS83_West =	10132,
            Proj_Arizona_Coordinate_System_east =	10201,
            Proj_Arizona_Coordinate_System_Central =	10202,
            Proj_Arizona_Coordinate_System_west =	10203,
            Proj_Arizona_CS83_east =	10231,
            Proj_Arizona_CS83_Central =	10232,
            Proj_Arizona_CS83_west =	10233,
            Proj_Arkansas_CS27_North =	10301,
            Proj_Arkansas_CS27_South =	10302,
            Proj_Arkansas_CS83_North =	10331,
            Proj_Arkansas_CS83_South =	10332,
            Proj_California_CS27_I =	10401,
            Proj_California_CS27_II =	10402,
            Proj_California_CS27_III =	10403,
            Proj_California_CS27_IV =	10404,
            Proj_California_CS27_V =	10405,
            Proj_California_CS27_VI =	10406,
            Proj_California_CS27_VII =	10407,
            Proj_California_CS83_1 =	10431,
            Proj_California_CS83_2 =	10432,
            Proj_California_CS83_3 =	10433,
            Proj_California_CS83_4 =	10434,
            Proj_California_CS83_5 =	10435,
            Proj_California_CS83_6 =	10436,
            Proj_Colorado_CS27_North =	10501,
            Proj_Colorado_CS27_Central =	10502,
            Proj_Colorado_CS27_South =	10503,
            Proj_Colorado_CS83_North =	10531,
            Proj_Colorado_CS83_Central =	10532,
            Proj_Colorado_CS83_South =	10533,
            Proj_Connecticut_CS27 =	10600,
            Proj_Connecticut_CS83 =	10630,
            Proj_Delaware_CS27 =	10700,
            Proj_Delaware_CS83 =	10730,
            Proj_Florida_CS27_East =	10901,
            Proj_Florida_CS27_West =	10902,
            Proj_Florida_CS27_North =	10903,
            Proj_Florida_CS83_East =	10931,
            Proj_Florida_CS83_West =	10932,
            Proj_Florida_CS83_North =	10933,
            Proj_Georgia_CS27_East =	11001,
            Proj_Georgia_CS27_West =	11002,
            Proj_Georgia_CS83_East =	11031,
            Proj_Georgia_CS83_West =	11032,
            Proj_Idaho_CS27_East =	11101,
            Proj_Idaho_CS27_Central =	11102,
            Proj_Idaho_CS27_West =	11103,
            Proj_Idaho_CS83_East =	11131,
            Proj_Idaho_CS83_Central =	11132,
            Proj_Idaho_CS83_West =	11133,
            Proj_Illinois_CS27_East =	11201,
            Proj_Illinois_CS27_West =	11202,
            Proj_Illinois_CS83_East =	11231,
            Proj_Illinois_CS83_West =	11232,
            Proj_Indiana_CS27_East =	11301,
            Proj_Indiana_CS27_West =	11302,
            Proj_Indiana_CS83_East =	11331,
            Proj_Indiana_CS83_West =	11332,
            Proj_Iowa_CS27_North =	11401,
            Proj_Iowa_CS27_South =	11402,
            Proj_Iowa_CS83_North =	11431,
            Proj_Iowa_CS83_South =	11432,
            Proj_Kansas_CS27_North =	11501,
            Proj_Kansas_CS27_South =	11502,
            Proj_Kansas_CS83_North =	11531,
            Proj_Kansas_CS83_South =	11532,
            Proj_Kentucky_CS27_North =	11601,
            Proj_Kentucky_CS27_South =	11602,
            Proj_Kentucky_CS83_North =	11631,
            Proj_Kentucky_CS83_South =	11632,
            Proj_Louisiana_CS27_North =	11701,
            Proj_Louisiana_CS27_South =	11702,
            Proj_Louisiana_CS83_North =	11731,
            Proj_Louisiana_CS83_South =	11732,
            Proj_Maine_CS27_East =	11801,
            Proj_Maine_CS27_West =	11802,
            Proj_Maine_CS83_East =	11831,
            Proj_Maine_CS83_West =	11832,
            Proj_Maryland_CS27 =	11900,
            Proj_Maryland_CS83 =	11930,
            Proj_Massachusetts_CS27_Mainland =	12001,
            Proj_Massachusetts_CS27_Island =	12002,
            Proj_Massachusetts_CS83_Mainland =	12031,
            Proj_Massachusetts_CS83_Island =	12032,
            Proj_Michigan_State_Plane_East =	12101,
            Proj_Michigan_State_Plane_Old_Central =	12102,
            Proj_Michigan_State_Plane_West =	12103,
            Proj_Michigan_CS27_North =	12111,
            Proj_Michigan_CS27_Central =	12112,
            Proj_Michigan_CS27_South =	12113,
            Proj_Michigan_CS83_North =	12141,
            Proj_Michigan_CS83_Central =	12142,
            Proj_Michigan_CS83_South =	12143,
            Proj_Minnesota_CS27_North =	12201,
            Proj_Minnesota_CS27_Central =	12202,
            Proj_Minnesota_CS27_South =	12203,
            Proj_Minnesota_CS83_North =	12231,
            Proj_Minnesota_CS83_Central =	12232,
            Proj_Minnesota_CS83_South =	12233,
            Proj_Mississippi_CS27_East =	12301,
            Proj_Mississippi_CS27_West =	12302,
            Proj_Mississippi_CS83_East =	12331,
            Proj_Mississippi_CS83_West =	12332,
            Proj_Missouri_CS27_East =	12401,
            Proj_Missouri_CS27_Central =	12402,
            Proj_Missouri_CS27_West =	12403,
            Proj_Missouri_CS83_East =	12431,
            Proj_Missouri_CS83_Central =	12432,
            Proj_Missouri_CS83_West =	12433,
            Proj_Montana_CS27_North =	12501,
            Proj_Montana_CS27_Central =	12502,
            Proj_Montana_CS27_South =	12503,
            Proj_Montana_CS83 =	12530,
            Proj_Nebraska_CS27_North =	12601,
            Proj_Nebraska_CS27_South =	12602,
            Proj_Nebraska_CS83 =	12630,
            Proj_Nevada_CS27_East =	12701,
            Proj_Nevada_CS27_Central =	12702,
            Proj_Nevada_CS27_West =	12703,
            Proj_Nevada_CS83_East =	12731,
            Proj_Nevada_CS83_Central =	12732,
            Proj_Nevada_CS83_West =	12733,
            Proj_New_Hampshire_CS27 =	12800,
            Proj_New_Hampshire_CS83 =	12830,
            Proj_New_Jersey_CS27 =	12900,
            Proj_New_Jersey_CS83 =	12930,
            Proj_New_Mexico_CS27_East =	13001,
            Proj_New_Mexico_CS27_Central =	13002,
            Proj_New_Mexico_CS27_West =	13003,
            Proj_New_Mexico_CS83_East =	13031,
            Proj_New_Mexico_CS83_Central =	13032,
            Proj_New_Mexico_CS83_West =	13033,
            Proj_New_York_CS27_East =	13101,
            Proj_New_York_CS27_Central =	13102,
            Proj_New_York_CS27_West =	13103,
            Proj_New_York_CS27_Long_Island =	13104,
            Proj_New_York_CS83_East =	13131,
            Proj_New_York_CS83_Central =	13132,
            Proj_New_York_CS83_West =	13133,
            Proj_New_York_CS83_Long_Island =	13134,
            Proj_North_Carolina_CS27 =	13200,
            Proj_North_Carolina_CS83 =	13230,
            Proj_North_Dakota_CS27_North =	13301,
            Proj_North_Dakota_CS27_South =	13302,
            Proj_North_Dakota_CS83_North =	13331,
            Proj_North_Dakota_CS83_South =	13332,
            Proj_Ohio_CS27_North =	13401,
            Proj_Ohio_CS27_South =	13402,
            Proj_Ohio_CS83_North =	13431,
            Proj_Ohio_CS83_South =	13432,
            Proj_Oklahoma_CS27_North =	13501,
            Proj_Oklahoma_CS27_South =	13502,
            Proj_Oklahoma_CS83_North =	13531,
            Proj_Oklahoma_CS83_South =	13532,
            Proj_Oregon_CS27_North =	13601,
            Proj_Oregon_CS27_South =	13602,
            Proj_Oregon_CS83_North =	13631,
            Proj_Oregon_CS83_South =	13632,
            Proj_Pennsylvania_CS27_North =	13701,
            Proj_Pennsylvania_CS27_South =	13702,
            Proj_Pennsylvania_CS83_North =	13731,
            Proj_Pennsylvania_CS83_South =	13732,
            Proj_Rhode_Island_CS27 =	13800,
            Proj_Rhode_Island_CS83 =	13830,
            Proj_South_Carolina_CS27_North =	13901,
            Proj_South_Carolina_CS27_South =	13902,
            Proj_South_Carolina_CS83 =	13930,
            Proj_South_Dakota_CS27_North =	14001,
            Proj_South_Dakota_CS27_South =	14002,
            Proj_South_Dakota_CS83_North =	14031,
            Proj_South_Dakota_CS83_South =	14032,
            Proj_Tennessee_CS27 =	14100,
            Proj_Tennessee_CS83 =	14130,
            Proj_Texas_CS27_North =	14201,
            Proj_Texas_CS27_North_Central =	14202,
            Proj_Texas_CS27_Central =	14203,
            Proj_Texas_CS27_South_Central =	14204,
            Proj_Texas_CS27_South =	14205,
            Proj_Texas_CS83_North =	14231,
            Proj_Texas_CS83_North_Central =	14232,
            Proj_Texas_CS83_Central =	14233,
            Proj_Texas_CS83_South_Central =	14234,
            Proj_Texas_CS83_South =	14235,
            Proj_Utah_CS27_North =	14301,
            Proj_Utah_CS27_Central =	14302,
            Proj_Utah_CS27_South =	14303,
            Proj_Utah_CS83_North =	14331,
            Proj_Utah_CS83_Central =	14332,
            Proj_Utah_CS83_South =	14333,
            Proj_Vermont_CS27 =	14400,
            Proj_Vermont_CS83 =	14430,
            Proj_Virginia_CS27_North =	14501,
            Proj_Virginia_CS27_South =	14502,
            Proj_Virginia_CS83_North =	14531,
            Proj_Virginia_CS83_South =	14532,
            Proj_Washington_CS27_North =	14601,
            Proj_Washington_CS27_South =	14602,
            Proj_Washington_CS83_North =	14631,
            Proj_Washington_CS83_South =	14632,
            Proj_West_Virginia_CS27_North =	14701,
            Proj_West_Virginia_CS27_South =	14702,
            Proj_West_Virginia_CS83_North =	14731,
            Proj_West_Virginia_CS83_South =	14732,
            Proj_Wisconsin_CS27_North =	14801,
            Proj_Wisconsin_CS27_Central =	14802,
            Proj_Wisconsin_CS27_South =	14803,
            Proj_Wisconsin_CS83_North =	14831,
            Proj_Wisconsin_CS83_Central =	14832,
            Proj_Wisconsin_CS83_South =	14833,
            Proj_Wyoming_CS27_East =	14901,
            Proj_Wyoming_CS27_East_Central =	14902,
            Proj_Wyoming_CS27_West_Central =	14903,
            Proj_Wyoming_CS27_West =	14904,
            Proj_Wyoming_CS83_East =	14931,
            Proj_Wyoming_CS83_East_Central =	14932,
            Proj_Wyoming_CS83_West_Central =	14933,
            Proj_Wyoming_CS83_West =	14934,
            Proj_Alaska_CS27_1 =	15001,
            Proj_Alaska_CS27_2 =	15002,
            Proj_Alaska_CS27_3 =	15003,
            Proj_Alaska_CS27_4 =	15004,
            Proj_Alaska_CS27_5 =	15005,
            Proj_Alaska_CS27_6 =	15006,
            Proj_Alaska_CS27_7 =	15007,
            Proj_Alaska_CS27_8 =	15008,
            Proj_Alaska_CS27_9 =	15009,
            Proj_Alaska_CS27_10 =	15010,
            Proj_Alaska_CS83_1 =	15031,
            Proj_Alaska_CS83_2 =	15032,
            Proj_Alaska_CS83_3 =	15033,
            Proj_Alaska_CS83_4 =	15034,
            Proj_Alaska_CS83_5 =	15035,
            Proj_Alaska_CS83_6 =	15036,
            Proj_Alaska_CS83_7 =	15037,
            Proj_Alaska_CS83_8 =	15038,
            Proj_Alaska_CS83_9 =	15039,
            Proj_Alaska_CS83_10 =	15040,
            Proj_Hawaii_CS27_1 =	15101,
            Proj_Hawaii_CS27_2 =	15102,
            Proj_Hawaii_CS27_3 =	15103,
            Proj_Hawaii_CS27_4 =	15104,
            Proj_Hawaii_CS27_5 =	15105,
            Proj_Hawaii_CS83_1 =	15131,
            Proj_Hawaii_CS83_2 =	15132,
            Proj_Hawaii_CS83_3 =	15133,
            Proj_Hawaii_CS83_4 =	15134,
            Proj_Hawaii_CS83_5 =	15135,
            Proj_Puerto_Rico_CS27 =	15201,
            Proj_St_Croix =	15202,
            Proj_Puerto_Rico_Virgin_Is =	15230,
            Proj_BLM_14N_feet =	15914,
            Proj_BLM_15N_feet =	15915,
            Proj_BLM_16N_feet =	15916,
            Proj_BLM_17N_feet =	15917,
            Proj_Map_Grid_of_Australia_48 =	17348,
            Proj_Map_Grid_of_Australia_49 =	17349,
            Proj_Map_Grid_of_Australia_50 =	17350,
            Proj_Map_Grid_of_Australia_51 =	17351,
            Proj_Map_Grid_of_Australia_52 =	17352,
            Proj_Map_Grid_of_Australia_53 =	17353,
            Proj_Map_Grid_of_Australia_54 =	17354,
            Proj_Map_Grid_of_Australia_55 =	17355,
            Proj_Map_Grid_of_Australia_56 =	17356,
            Proj_Map_Grid_of_Australia_57 =	17357,
            Proj_Map_Grid_of_Australia_58 =	17358,
            Proj_Australian_Map_Grid_48 =	17448,
            Proj_Australian_Map_Grid_49 =	17449,
            Proj_Australian_Map_Grid_50 =	17450,
            Proj_Australian_Map_Grid_51 =	17451,
            Proj_Australian_Map_Grid_52 =	17452,
            Proj_Australian_Map_Grid_53 =	17453,
            Proj_Australian_Map_Grid_54 =	17454,
            Proj_Australian_Map_Grid_55 =	17455,
            Proj_Australian_Map_Grid_56 =	17456,
            Proj_Australian_Map_Grid_57 =	17457,
            Proj_Australian_Map_Grid_58 =	17458,
            Proj_Argentina_1 =	18031,
            Proj_Argentina_2 =	18032,
            Proj_Argentina_3 =	18033,
            Proj_Argentina_4 =	18034,
            Proj_Argentina_5 =	18035,
            Proj_Argentina_6 =	18036,
            Proj_Argentina_7 =	18037,
            Proj_Colombia_3W =	18051,
            Proj_Colombia_Bogota =	18052,
            Proj_Colombia_3E =	18053,
            Proj_Colombia_6E =	18054,
            Proj_Egypt_Red_Belt =	18072,
            Proj_Egypt_Purple_Belt =	18073,
            Proj_Extended_Purple_Belt =	18074,
            Proj_New_Zealand_North_Island_Nat_Grid =	18141,
            Proj_New_Zealand_South_Island_Nat_Grid =	18142,
            Proj_Bahrain_Grid =	19900,
            Proj_Netherlands_E_Indies_Equatorial =	19905,
            Proj_RSO_Borneo =	19912,
        }

        public enum CoordinateTransformationCodes
        {
            CT_TransverseMercator =	1,
            CT_TransvMercator_Modified_Alaska = 2,
            CT_ObliqueMercator =	3,
            CT_ObliqueMercator_Laborde =	4,
            CT_ObliqueMercator_Rosenmund =	5,
            CT_ObliqueMercator_Spherical =	6,
            CT_Mercator =	7,
            CT_LambertConfConic_2SP =	8,
            CT_LambertConfConic_Helmert =	9,
            CT_LambertAzimEqualArea =	10,
            CT_AlbersEqualArea =	11,
            CT_AzimuthalEquidistant =	12,
            CT_EquidistantConic =	13,
            CT_Stereographic =	14,
            CT_PolarStereographic =	15,
            CT_ObliqueStereographic =	16,
            CT_Equirectangular =	17,
            CT_CassiniSoldner =	18,
            CT_Gnomonic =	19,
            CT_MillerCylindrical =	20,
            CT_Orthographic =	21,
            CT_Polyconic =	22,
            CT_Robinson =	23,
            CT_Sinusoidal =	24,
            CT_VanDerGrinten =	25,
            CT_NewZealandMapGrid =	26,
            CT_TransvMercator_SouthOriented=	27,
        }

        public enum VertialTypeCodes
        {
            VertCS_Airy_Modified_1849_ellipsoid =	5002,
            VertCS_ANS_ellipsoid =	5003,
            VertCS_Bessel_1841_ellipsoid =	5004,
            VertCS_Bessel_Modified_ellipsoid =	5005,
            VertCS_Bessel_Namibia_ellipsoid =	5006,
            VertCS_Clarke_1858_ellipsoid =	5007,
            VertCS_Clarke_1866_ellipsoid =	5008,
            VertCS_Clarke_1880_Benoit_ellipsoid =	5010,
            VertCS_Clarke_1880_IGN_ellipsoid =	5011,
            VertCS_Clarke_1880_RGS_ellipsoid =	5012,
            VertCS_Clarke_1880_Arc_ellipsoid =	5013,
            VertCS_Clarke_1880_SGA_1922_ellipsoid =	5014,
            VertCS_Everest_1830_1937_Adjustment_ellipsoid =	5015,
            VertCS_Everest_1830_1967_Definition_ellipsoid =	5016,
            VertCS_Everest_1830_1975_Definition_ellipsoid =	5017,
            VertCS_Everest_1830_Modified_ellipsoid =	5018,
            VertCS_GRS_1980_ellipsoid =	5019,
            VertCS_Helmert_1906_ellipsoid =	5020,
            VertCS_INS_ellipsoid =	5021,
            VertCS_International_1924_ellipsoid =	5022,
            VertCS_International_1967_ellipsoid =	5023,
            VertCS_Krassowsky_1940_ellipsoid =	5024,
            VertCS_NWL_9D_ellipsoid =	5025,
            VertCS_NWL_10D_ellipsoid =	5026,
            VertCS_Plessis_1817_ellipsoid =	5027,
            VertCS_Struve_1860_ellipsoid =	5028,
            VertCS_War_Office_ellipsoid =	5029,
            VertCS_WGS_84_ellipsoid =	5030,
            VertCS_GEM_10C_ellipsoid =	5031,
            VertCS_OSU86F_ellipsoid =	5032,
            VertCS_OSU91A_ellipsoid =	5033,
            // Orthometric Vertical CS;
            VertCS_Newlyn =	5101,
            VertCS_North_American_Vertical_Datum_1929 =	5102,
            VertCS_North_American_Vertical_Datum_1988 =	5103,
            VertCS_Yellow_Sea_1956 =	5104,
            VertCS_Baltic_Sea =	5105,
            VertCS_Caspian_Sea =	5106,
        }

        //private void StoreValue(ushort code)
        //{
        //    // stores the value in the model 

        //    //[    0,  1023]       Reserved
        //    //[ 1024,  2047]       GeoTIFF Configuration Keys
        //    //[ 2048,  3071]       Geographic/Geocentric CS Parameter Keys
        //    //[ 3072,  4095]       Projected CS Parameter Keys
        //    //[ 4096,  5119]       Vertical CS Parameter Keys
        //    //[ 5120, 32767]       Reserved
        //    //[32768, 65535]       Private use

        //    if (InRange(code, 0, 1023))
        //    {
        //        if (InRange(code, 1, 3)) // careful, duplicate
        //        {
        //            modelTypeCode = (ModelTypeCodes)code;
        //        }
        //    }
        //    else if (InRange(code, 1024, 2046))
        //    {
        //        configKey = (ConfigKeys)code;              
        //    }
        //    else if (InRange(code, 2048, 3071))
        //    {
        //        geographicParameterKey = (GeographicParameterKeys)code;          
        //    }
        //    else if (InRange(code, 3072, 4095))
        //    {
        //        projectedParameterKey = (ProjectedParameterKeys)code;      
        
        //        //4001 4035
        //    }
        //    else if (InRange(code, 4096, 5119))
        //    {
        //        if (InRange(code, 4201, 4902))
        //        {
        //            geographicTypeCode = (GeographicTypeCodes)code;
        //        }
        //        else if (InRange(code, 5002, 5106))
        //        {
        //            verticalTypeCode = (VertialTypeCodes)code;
        //        }                
        //    }
        //    else if (InRange(code, 5120, 32767))
        //    {
        //        if (InRange(code, 9001, 9015))
        //        {
        //            linearUnitCode = (LinearUnitCodes)code;
        //        }
        //        else if (InRange(code, 9101, 9108))
        //        {
        //            angularUnitCode = (AngularUnitsCodes)code;
        //        }
        //        else if (InRange(code, 10101, 19912))
        //        {
        //            projectionCode = (ProjectionCodes)code;
        //        }
        //        else if (InRange(code, 20137, 32760))
        //        {
        //            projectedTypeCode = (ProjectedTypeCodes)code;
        //        }
        //    }
        //    else if (InRange(code, 32768, 65535))
        //    {
        //    }

        //}

        private static bool InRange(ushort val, ushort min, ushort max)
        {
            return (val >= min && val <= max);
        }

        private List<Ifd> ifds;

        private void FindGeoData(FileStream file)
        {
            foreach (Ifd ifd in ifds)
            {
                foreach (TifTag tag in ifd.TifTags)
                {
                    file.Position = tag.DataOffset;

                    switch (tag.TagId)
                    {                       
                        case TagId.GeoKeyDirectoryTag:

                            // Decode all the geo info

                            //Header={KeyDirectoryVersion, KeyRevision, MinorRevision, NumberOfKeys}
                            ushort keyDirectoryVersion = ReadUShort(file);
                            ushort keyRevision = ReadUShort(file);
                            ushort minorRevision = ReadUShort(file);
                            ushort numberOfKeys = ReadUShort(file);

                            for (int i = 0; i < numberOfKeys; i++)
                            {
                                // KeyID, TIFFTagLocation, Count, Value_Offset
                                ushort keyId = ReadUShort(file);
                                ushort tiffTagLocation = ReadUShort(file);
                                ushort count = ReadUShort(file);
                                ushort valueOffset = ReadUShort(file);

                                if (InRange(keyId, 3072, 3095))
                                {
                                    // Set the projection
                                    ProjectedParameterKeys projectionParameter = (ProjectedParameterKeys)keyId;

                                    if (projectionParameter == ProjectedParameterKeys.ProjectedCSTypeGeoKey)
                                    {
                                        ProjectedTypeCodes projectedTypeCode = (ProjectedTypeCodes)valueOffset;
                                        projection = projectedTypeCode.ToString();
                                    }
                                    else if (projectionParameter == ProjectedParameterKeys.ProjLinearUnitsGeoKey)
                                    {
                                        LinearUnitCodes linearUnitCode = (LinearUnitCodes)valueOffset;
                                        //unitSize = linearUnitCode.ToString();
                                    }

                                }
                                else if (InRange(keyId, 2048, 2061))
                                {

                                }
                                else if (InRange(keyId, 1024, 1026))
                                {
                                    ConfigKeys configKey = (ConfigKeys)keyId;

                                    if (configKey == ConfigKeys.GTModelTypeGeoKey)
                                    {
                                    }
                                    else if (configKey == ConfigKeys.GTRasterTypeGeoKey)
                                    {
                                        RasterTypeCodes rasterTypeCode = (RasterTypeCodes)valueOffset; 
                                    }
                                    else if (configKey == ConfigKeys.GTCitationGeoKey)
                                    {
                                    }
                                }                               

                            }

                            break;
                        case TagId.GeoDoubleParamsTag: 
                            // todo
                            double todo = ReadDouble(file);
                            break;
                        case TagId.GeoAsciiParamsTag: 
                            // todo
                            break;
                        case TagId.ModelPixelScaleTag:
                              //Tag = 33550
                              //Type = DOUBLE (IEEE Double precision)
                              //N = 3
                            modelPixelScaleTags = new double[3];
                            for (int i = 0; i < 3; i++)
                            {
                                modelPixelScaleTags.SetValue(ReadDouble(file), i);
                            }

                            meters_to_pixels_x = modelPixelScaleTags[0];
                            meters_to_pixels_y = modelPixelScaleTags[1];

                            break;
                        case TagId.ModelTiepointTag:
                            //Tag = 33922 (8482.H) 
                            //Type = DOUBLE (IEEE Double precision)
                            //N = 6*K,  K = number of tiepoints
                            modelTiepointTags = new double[6];
                            for (int i = 0; i < 6; i++)
                            {
                                modelTiepointTags.SetValue(ReadDouble(file), i);
                            }
                            
                            // Set origin here
                            originEasting = modelTiepointTags[3];
                            originNorthing = modelTiepointTags[4];

                            break;
                        case TagId.ModelTransformationTag:
                              //Tag  =  34264  (85D8.H) 
                              //Type =  DOUBLE    
                              //N    =  16
                            double[] modelTransformTags = new double[16];
                            for (int i = 0; i < 16; i++)
                            {
                                modelTransformTags.SetValue(ReadDouble(file), i);
                            }
                            break;
                        default: break;
                    }
                }
            }            
        }

        private int ReadHeader(FileStream file)
        {
            byte[] header = new byte[8];
            file.Read(header, 0, 8);

            // Byte order
            if (header[0] == 0x49 && header[1] == 0x49)
                byteOrder = ByteOrder.Little;
            else
                byteOrder = ByteOrder.Big;

            // Version
            version = BitConverter.ToInt16(header, 2);

            // Start tag/field address
            ifdOffset = BitConverter.ToUInt32(header, 4);

            return 7;
        }

        private int ReadIfdField(FileStream file, int index)
        {
            Ifd ifd = new Ifd();

            file.Position = index;

            ifd.NumEntries = ReadUShort(file);

            ifd.TifTags = new TifTag[ifd.NumEntries];

            for (int i = 0; i < ifd.NumEntries; i++)
            {
                TifTag tag = new TifTag();

                tag.TagId = (TagId)ReadUShort(file);
                tag.DataType = ReadUShort(file);
                tag.DataCount = ReadUInt32(file);
                tag.DataOffset = ReadUInt32(file);

                ifd.TifTags.SetValue(tag, i);
            }

            ifds.Add(ifd);

            return index;
        }

        private void ReadIfdFields(FileStream file, int index)
        {
            while (true)
            {
                ReadIfdField(file, index);

                if (ifds[ifds.Count - 1].NextIfdOffset == 0)
                {
                    return;
                }
            }
        }

        private static void CopyRegionIntoImage(Bitmap srcBitmap, Rectangle srcRegion, ref Bitmap destBitmap, Rectangle destRegion)
        {
            using (Graphics g = Graphics.FromImage(destBitmap))
            {
                g.DrawImage(srcBitmap, destRegion, srcRegion, GraphicsUnit.Pixel);
            }
        }

        private static Image FixedSize(Image imgPhoto, int Width, int Height)
        {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)Width / (float)sourceWidth);
            nPercentH = ((float)Height / (float)sourceHeight);
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = System.Convert.ToInt16((Width -
                              (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = System.Convert.ToInt16((Height -
                              (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = new Bitmap(Width, Height,
                              PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
                             imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(Color.Red);
            grPhoto.InterpolationMode =
                    InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }

        // todo make sure UTM is calculated correctly

        /// <summary>
        /// Given a point on the bitmap get the coordinate as string
        /// </summary>
        /// <param name="x">x location</param>
        /// <param name="y">y location</param>
        /// <returns>World coordinate in coordinate system of tif</returns>
        public string GetCoordinate(int x, int y)
        {
            double n = (Meters_To_Pixels_Y * y) + OriginNorthing;
            double e = (Meters_To_Pixels_X * x) + OriginEasting;

            return Projection + " N " + n.ToString() + " E" + e.ToString();
        }

        /// <summary>
        /// Get a point on the bitmap
        /// </summary>
        /// <param name="north">North in coordinate system of tif</param>
        /// <param name="east">East in coordinate system of tif</param>
        /// <returns>Point(x,y) on bitmap</returns>
        public Point FindPointOnBitmap(double north, double east)
        {
            double n_pixels = north / Meters_To_Pixels_Y;
            double e_pixels = east / Meters_To_Pixels_X;

            double origin_n_pixels = originNorthing / Meters_To_Pixels_Y;
            double origin_e_pixels = originEasting / Meters_To_Pixels_X;

            int x = (int)(e_pixels - origin_e_pixels);
            int y = (int)(origin_n_pixels - n_pixels);

            return new Point(x, y);
        }

        /// <summary>
        /// Grab a region
        /// </summary>
        /// <param name="east"></param>
        /// <param name="west"></param>
        /// <param name="north"></param>
        /// <param name="south"></param>
        /// <returns></returns>
        public Bitmap GrabRegion(double east, double west, double north, double south)
        {
            Point a = FindPointOnBitmap(north, west);
            Point b = FindPointOnBitmap(south, east);

            a.X = Math.Max(a.X, 0);
            a.Y = Math.Max(a.Y, 0);
            b.X = Math.Min(b.X, Bitmap.Width);
            b.Y = Math.Min(b.Y, Bitmap.Height);

            int w = Math.Abs(b.X - a.X);
            int h = Math.Abs(b.Y - a.Y);

            Point c = FindPointOnBitmap(north, west);
            Point d = FindPointOnBitmap(south, east);

            //int sx = Math.Abs(c.X);
            //int sy = Math.Abs(c.Y);

            // NB todo if source bitmap not big enough then pad it 

            int desiredWidth = d.X - c.X;
            int desiredHeight = d.Y - c.Y;

            Bitmap target = new System.Drawing.Bitmap(desiredWidth, desiredHeight);
            
            Rectangle srcRect = new Rectangle(a.X, a.Y, w, h);
            Rectangle dstRect = new Rectangle(0, 0, desiredWidth, desiredHeight);

            CopyRegionIntoImage(Bitmap, srcRect, ref target, dstRect);

            return target;
        }

        /// <summary>
        /// Scale the tif image
        /// </summary>
        /// <param name="width">new width</param>
        /// <param name="height">new height</param>
        public void ScaleImage(int width, int height)
        {
            var brush = new SolidBrush(Color.Black);

            float w_s = (float)width / (float)bitmap.Width;
            float h_s = (float)height / (float)bitmap.Height;
            
            float scale = Math.Min(w_s, h_s);
            Bitmap newBitmap = new Bitmap((int)width, (int)height);
            Graphics g = Graphics.FromImage(newBitmap);

            // uncomment for higher quality output
            //graph.InterpolationMode = InterpolationMode.High;
            //graph.CompositingQuality = CompositingQuality.HighQuality;
            //graph.SmoothingMode = SmoothingMode.AntiAlias;

            var scaleWidth = (int)(bitmap.Width * scale);
            var scaleHeight = (int)(bitmap.Height * scale);

            g.FillRectangle(brush, new RectangleF(0, 0, width, height));

            g.DrawImage(bitmap, new Rectangle(((int)width - scaleWidth) / 2, 
                                               ((int)height - scaleHeight) / 2, 
                                                scaleWidth, scaleHeight));

            bitmap = newBitmap;

            SetNewWidthHeight();
        }

        /// <summary>
        /// Add padding to the edges of the tif image
        /// </summary>
        /// <param name="width">new total width</param>
        /// <param name="height">new total height</param>
        public void Pad(int width, int height)
        {
            this.bitmap = (Bitmap)PadImage(bitmap, width, height);
        }

        private Image PadImage(Image originalImage, int newWidth, int newHeight)
        {
            int largestDimension = Math.Max(originalImage.Height, originalImage.Width);
            Size squareSize = new Size(newWidth, newHeight);
            Bitmap squareImage = new Bitmap(squareSize.Width, squareSize.Height);
            using (Graphics graphics = Graphics.FromImage(squareImage))
            {
                graphics.FillRectangle(Brushes.White, 0, 0, squareSize.Width, squareSize.Height);
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                graphics.DrawImage(originalImage, (squareSize.Width / 2) - (originalImage.Width / 2), (squareSize.Height / 2) - (originalImage.Height / 2), originalImage.Width, originalImage.Height);
            }
            return squareImage;
        }

        /// <summary>
        /// Load a geotiff.
        /// </summary>
        /// <param name="filename">filename + path</param>
        /// <returns>Succeed or fail</returns>
        public bool Load(string filename)
        {
            int index = 0;
            FileStream file;
            bitmap = new Bitmap(1, 1);

            ifds = new List<Ifd>();

            // Read Header

            try
            {
                using (file = new FileStream(filename, FileMode.Open))
                {
                    index = ReadHeader(file);

                    index = (int)ifdOffset;

                    ReadIfdFields(file, index);

                    // Read geo tags
                    FindGeoData(file);
                }

                bitmap = (Bitmap)Image.FromFile(filename);
            }
            catch (Exception ex)
            {
                // throw here
                return false;
            }

            SetNewWidthHeight();

            return true;
        }

        private void SetNewWidthHeight()
        {
            width_meters = Meters_To_Pixels_X * bitmap.Width;
            height_meters = Meters_To_Pixels_Y * bitmap.Height;

            meters_to_pixels_x = width_meters * bitmap.Width;
            meters_to_pixels_y = height_meters * bitmap.Height;
        }

        private UInt32 ReadUInt32(FileStream file)
        {
            byte[] data = new byte[4];
            file.Read(data, 0, 4);
            return BitConverter.ToUInt32(data, 0);
        }

        private ushort ReadUShort(FileStream file)
        {
            byte[] data = new byte[2];
            file.Read(data, 0, 2);
            return BitConverter.ToUInt16(data, 0);
        }

        private double ReadDouble(FileStream file)
        {
            byte[] data = new byte[8];
            file.Read(data, 0, 8);
            return BitConverter.ToDouble(data, 0);
        }
    }
}
