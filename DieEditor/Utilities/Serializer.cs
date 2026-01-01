using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DieEditor.Utilities
{
    public static class Serializer
    {
        public static void toFile<T>(T instance, string filePath)
        {
            try
            {
                using (var writer = new System.IO.StreamWriter(filePath))
                {
                    var serializer = new DataContractSerializer(typeof(T));
                    serializer.WriteObject(writer.BaseStream, instance);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error serializing to file: " + ex.Message);
                // TODO: Log
            }
        }

        internal static T FromFile<T>(string path)
        {
            try
            {
                using (var reader = new System.IO.StreamReader(path, true))
                {
                    var serializer = new DataContractSerializer(typeof(T));
                    T instance = (T) serializer.ReadObject(reader.BaseStream);
                    return instance;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error serializing to file: " + ex.Message);
                // TODO: Log
                return default(T);
            }
        }
    }
}
