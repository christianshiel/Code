using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenDis.Enumerations;
using OpenDis.Dis1998;
using System.Drawing;
using System.IO;
using DotNetCoords;
using OpenDis.Enumerations.EntityState;

namespace Diswerx
{
    public enum Domain
    {
        Air = 1,
        Land = 2,
        // todo
    }

    /// <summary>
    /// Convinient methods to make you life easier.
    /// </summary>
    public static class DisHelpers
    {
        #region Mapping

        private static Dictionary<EntityType, string> graphicsMappings =
            new Dictionary<EntityType, string>();

        /// <summary>
        /// Retrieve a bitmap for an entity enum
        /// </summary>
        /// <param name="entityMarking"></param>
        /// <returns></returns>
        public static string GetBitmapForEnum(EntityType entityMarking)
        {
            return graphicsMappings[entityMarking];
        }

        /// <summary>
        /// Associcate a bitmap with an entity enum.
        /// </summary>
        /// <param name="entityEnum"></param>
        /// <param name="bitmap"></param>
        public static void SetBitmapForEnum(EntityType entityEnum, string bitmap) // should be enum not marking?
        {
            if (graphicsMappings.ContainsKey(entityEnum))
            {
                graphicsMappings.Add(entityEnum, bitmap);
            }
            else
            {
                graphicsMappings[entityEnum] = bitmap;
            }
        }

        /// <summary>
        /// Associate some of your data with an enum (eg an icon path)
        /// </summary>
        /// <param name="config">File name</param>
        public static void SaveEnumMappings(string config)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(config, true))
                {
                    List<EntityType> keys = graphicsMappings.Keys.ToList<EntityType>();
                    
                    foreach (EntityType e in keys)
                    {
                        string bitmap = GetBitmapForEnum(e);

                        string code = e.Category + "." +
                                      e.Country + "." +
                                      e.Domain + "." + 
                                      e.EntityKind + "." +
                                      e.Extra + "." +
                                      e.Specific + "." +
                                      e.Subcategory;

                        writer.WriteLine(bitmap + "=" + code);
                    }                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be written:");
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Load data associated with an enum.
        /// </summary>
        /// <param name="config">File name</param>
        public static void LoadEnumMappings(string config)
        {

            return;

            try
            {
                using (StreamReader sr = new StreamReader(config))
                {
                    String line = sr.ReadToEnd();
                    
                    string[] strs = line.Split('=');

                    string bitmap = strs[0];
                    string code = strs[1];

                    EntityType et = ToEntityType(code);

                    SetBitmapForEnum(et, bitmap);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }

        #endregion

        /// <summary>
        /// Get Pdu type from byte
        /// </summary>
        /// <param name="type">Type as btye</param>
        /// <returns>Type as enum</returns>
        public static PduType GetPduType(byte type)
        {
            return (PduType)type;
        }

        /// <summary>
        /// See what type of Pdu is wrapped in the buffer
        /// </summary>
        /// <param name="buffer">Raw udp data</param>
        /// <returns></returns>
        public static byte GetPduType(byte[] buffer)
        {
            return buffer[2];
        }

        public static byte[] MarshalPdu(Pdu pdu)
        {
            OpenDis.Core.DataOutputStream dos = new OpenDis.Core.DataOutputStream(OpenDis.Core.Endian.Big);                
            pdu.Marshal(dos);
            return dos.ConvertToBytes();
        }

        /// <summary>
        /// Run a hueristic to test if this data is likely(!) DIS.
        /// </summary>
        /// <param name="buffer">Raw data</param>
        /// <returns>Whether or not this thinks its DIS</returns>
        public static bool IsDis(byte[] buffer)
        {
            // Do some hueristics to classify if this payload is likely a DIS packet
            bool isDisPacket = true;

            if (buffer.Length < 12)
            {
                return false;
            }

            // Check DIS version
            if (buffer[0] > 7 || buffer[0] < 1)
            {
                return false;
            }

            // Protocol family
            if (buffer[3] > 7 && buffer[3] != 129 || buffer[3] < 1)
            {
                return false;
            }

            // Check padding
            if (buffer[10] != '\0' && buffer[11] != '\0')
            {
                return false;
            }

            return isDisPacket;
        }

        public static string ToString(byte[] bytes)
        {
            return System.Text.Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// Return an EntityType from an enum.
        /// </summary>
        /// <param name="code">String should be in format (x.x.x.x.x.x.x), will fail otherwise</param>
        /// <returns>The EntityType</returns>
        public static EntityType ToEntityType(string code)
        {
            string[] codeTokens = code.Split('.');

            EntityType et = new EntityType();
            et.Category = byte.Parse(codeTokens[0]);
            et.Country = byte.Parse(codeTokens[1]);
            et.Domain = byte.Parse(codeTokens[2]);
            et.EntityKind = byte.Parse(codeTokens[3]);
            et.Extra = byte.Parse(codeTokens[4]);
            et.Specific = byte.Parse(codeTokens[5]);
            et.Subcategory = byte.Parse(codeTokens[6]);

            return et;
        }

        /// <summary>
        /// Set articulation parameters value (WARNING: big endian)
        /// </summary>
        /// <returns></returns>
        public static int SetArticulationParameters(ArticulatedPartIndex api, ArticulatedPartOffset apo)
        {
            byte[] ptb = new byte[4];
            ushort aph = (ushort)api;
            ushort apl = (ushort)apo;

            byte[] bh = BitConverter.GetBytes(aph);
            byte[] bl = BitConverter.GetBytes(apl);
            ptb[0] = bh[0]; ptb[1] = bh[1];
            ptb[2] = bl[0]; ptb[3] = bl[1];

            int pt = BitConverter.ToInt32(ptb, 0);

            return pt;
        }

        public static ArticulatedPartOffset GetArticulatedPartOffset(double ap)
        {
            byte[] raw = BitConverter.GetBytes(ap);
            byte[] upper = new byte[2];
            upper[0] = raw[0]; upper[1] = raw[1];
            ushort apo = BitConverter.ToUInt16(upper, 0);
            return (ArticulatedPartOffset)apo;
        }

        public static ArticulatedPartIndex GetArticulatedPartIndex(double ap)
        {
            byte[] raw = BitConverter.GetBytes(ap);
            byte[] lower = new byte[2];
            lower[0] = raw[2]; lower[1] = raw[3];
            ushort api = BitConverter.ToUInt16(lower, 0);
            return (ArticulatedPartIndex)api;
        }

        /// <summary>
        /// Convenience method to set the Marking on an Entity.
        /// </summary>
        /// <param name="marking">The string form of the marking</param>
        /// <returns>Marking in OpenDIS form</returns>
        public static Marking SetMarking(string marking)
        {
            Marking m = new Marking();
            m.Characters = new byte[marking.Length];
            for (int i = 0; i < marking.Length; i++)
            {
                m.Characters[i] = (byte)marking[i];
            }
            return m;
        }

        /// <summary>
        /// Convert coordinate to DIS standard.
        /// </summary>
        /// <param name="lat">Latitude</param>
        /// <param name="lng">Longitude</param>
        /// <returns>Earth fixed coordinate</returns>
        public static Vector3Double ToEarthFixed(double lat, double lng)
        {
            ECEFRef e = new ECEFRef(new LatLng(lat, lng));
            Vector3Double v = new Vector3Double();
            v.X = e.X; v.Y = e.Y; v.Z = e.Z;
            return v;
        }
    }
}
