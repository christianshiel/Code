using System;
using System.Collections.Generic;
using System.IO;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace Diswerx.DemRender
{
    public static class FileChecker
    {

        public static bool CheckSignature(string filepath, int signatureSize, string expectedSignature)
        {
            if (String.IsNullOrEmpty(filepath))
                throw new ArgumentException("Must specify a filepath");
            if (String.IsNullOrEmpty(expectedSignature))
                throw new ArgumentException("Must specify a value for the expected file signature");

            using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {

                if (fs.Length < signatureSize)
                    return false;

                byte[] signature = new byte[signatureSize];
                int bytesRequired = signatureSize;
                int index = 0;

                while (bytesRequired > 0)
                {
                    int bytesRead = fs.Read(signature, index, bytesRequired);
                    bytesRequired -= bytesRead;
                    index += bytesRead;
                }

                string actualSignature = BitConverter.ToString(signature);

                if (actualSignature == expectedSignature)
                    return true;
                else
                    return false;
            }

        }
    }

}

