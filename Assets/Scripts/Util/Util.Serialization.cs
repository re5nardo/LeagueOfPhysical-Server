using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

public partial class Util
{
    private static BinaryFormatter formatter;
    private static BinaryFormatter Formatter
    {
        get
        {
            if (formatter == null)
            {
                SurrogateSelector surrogateSelector = new SurrogateSelector();
                surrogateSelector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), new Vector3SerializationSurrogate());

                formatter = new BinaryFormatter();
                formatter.SurrogateSelector = surrogateSelector;
            }
            return formatter;
        }
    }

    public static byte[] CommonDataSerialize(object customObject)
    {
        using (var stream = new MemoryStream())
        {
            Formatter.Serialize(stream, customObject);
            return stream.ToArray();
        }
    }

    public static object CommonDataDeserialize(byte[] serializedCustomObject)
    {
        using (var stream = new MemoryStream(serializedCustomObject))
        {
            return Formatter.Deserialize(stream);
        }
    }

    public static byte[] CommonDataCompressionSerialize(object customObject)
    {
        return ZipByteArray(CommonDataSerialize(customObject));
    }

    public static object CommonDataCompressionDeserialize(byte[] serializedCustomObject)
    {
        return CommonDataDeserialize(UnzipByteArray(serializedCustomObject));
    }

    public static byte[] ZipByteArray(byte[] sourceByteArray)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            using (GZipStream gs = new GZipStream(ms, CompressionMode.Compress))
            {
                gs.Write(sourceByteArray, 0, sourceByteArray.Length);
            }
            return ms.ToArray();
        }
    }

    public static byte[] UnzipByteArray(byte[] sourceByteArray)
    {
        using (MemoryStream resultStream = new MemoryStream())
        {
            using (MemoryStream sourceStream = new MemoryStream(sourceByteArray))
            {
                using (GZipStream gs = new GZipStream(sourceStream, CompressionMode.Decompress))
                {
                    gs.CopyTo(resultStream);
                }
            }

            return resultStream.ToArray();
        }
    }
}
