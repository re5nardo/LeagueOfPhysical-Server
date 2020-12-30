using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.IO.Compression;

public class Util
{
    public class Math
    {
		//	https://answers.unity.com/questions/1032673/how-to-get-0-360-degree-from-two-points.html
		public static float FindDegree(Vector2 vec2Position)
	    {
			float value = (float)((Mathf.Atan2(vec2Position.x, vec2Position.y) / Mathf.PI) * 180f);
	    	if(value < 0)
	    		value += 360f;

			return value;
	    }
    }

    public static void Parse(string text, char delim, List<string> output)
    {
        string[] arrText = text.Split(delim);

        output.Clear();
        foreach (string strText in arrText)
        {
            output.Add(strText);
        }
    }

    public static void Parse(string text, char delim, List<int> output)
    {
        if (string.IsNullOrEmpty(text))
            return;

        List<string> temp = new List<string>();
        Parse(text, delim, temp);

        output.Clear();
        foreach (string strText in temp)
        {
            int result = 0;
            if(Convert(strText, ref result))
            {
                output.Add(result);
            }
        }
    }

    public static void Parse(string text, char delim, List<float> output)
    {
        if (string.IsNullOrEmpty(text))
            return;

        List<string> temp = new List<string>();
        Parse(text, delim, temp);

        output.Clear();
        foreach (string strText in temp)
        {
            float result = 0;
            if(Convert(strText, ref result))
            {
                output.Add(result);
            }
        }
    }

    public static void Parse(string text, char delim, List<double> output)
    {
        if (string.IsNullOrEmpty(text))
            return;

        List<string> temp = new List<string>();
        Parse(text, delim, temp);

        output.Clear();
        foreach (string strText in temp)
        {
            double result = 0;
            if(Convert(strText, ref result))
            {
                output.Add(result);
            }
        }
    }

    public static List<List<string>> ReadCSV(string strFilePath)
    {
        return ReadCSV(Resources.Load<TextAsset>(strFilePath));
    }

    public static List<List<string>> ReadCSV(TextAsset textAsset)
    {
        List<List<string>> listData = new List<List<string>>();

        string text = System.Text.Encoding.UTF8.GetString(textAsset.bytes); //  unity 2017 can't read text with ansi encoding..
        string[] lines = text.Replace("\r", "").Split('\n');

        foreach(string line in lines)
        {
            if (line == "")
                continue;

            List<string> listWord = new List<string>();
            bool bInsideQuotes = false;
            int nWordStart = 0;

            for (int i = 0; i < line.Length; ++i)
            {
                bool bLast = i == line.Length - 1;

                if (line[i] == ',')
                {
                    if (bLast)
                    {
                        listWord.Add(line.Substring(nWordStart, i - nWordStart));
                        listWord.Add("");
                    }
                    else
                    {
                        if (!bInsideQuotes)
                        {
                            listWord.Add(line.Substring(nWordStart, i - nWordStart));
                            nWordStart = i + 1;
                        }
                    }
                }
                else if (line[i] == '"')
                {
                    if (bInsideQuotes)
                    {
                        if (bLast)
                        {
                            listWord.Add(line.Substring(nWordStart, i - nWordStart).Replace("\"\"", "\""));
                        }
                        else
                        {
                            if (line[i + 1] != '"')
                            {
                                listWord.Add(line.Substring(nWordStart, i - nWordStart).Replace("\"\"", "\""));
                                bInsideQuotes = false;

                                if (line[i + 1] == ',')
                                {
                                    if (i == line.Length - 2)
                                    {
                                        listWord.Add("");
                                    }
                                    else
                                    {
                                        ++i;
                                        nWordStart = i + 1;
                                    }
                                }
                            }
                            else
                            {
                                ++i;
                            }
                        }
                    }
                    else
                    {
                        nWordStart = i + 1;
                        bInsideQuotes = true;
                    }
                }
                else
                {
                    if (bLast)
                    {
                        listWord.Add(line.Substring(nWordStart, line.Length - nWordStart));
                    }
                }
            }

            listData.Add(listWord);
        }

        return listData;
    }

    public static bool Convert(string strText, ref int output)
    {
        if (string.IsNullOrEmpty(strText))
            return false;

        int result = 0;
        if (int.TryParse(strText.Replace(" ", ""), out result))
        {
            output = result;
            return true;
        }
        else
        {
            Debug.LogWarning("Can't convert to int type! strText : " + strText);
            return false;
        }
    }

    public static bool Convert(string strText, ref float output)
    {
        if (string.IsNullOrEmpty(strText))
            return false;
        
        float result = 0;
        if (float.TryParse(strText.Replace(" ", ""), out result))
        {
            output = result;
            return true;
        }
        else
        {
            Debug.LogWarning("Can't convert to float type! strText : " + strText);
            return false;
        }
    }

    public static bool Convert(string strText, ref double output)
    {
        if (string.IsNullOrEmpty(strText))
            return false;
        
        double result = 0;
        if (double.TryParse(strText.Replace(" ", ""), out result))
        {
            output = result;
            return true;
        }
        else
        {
            Debug.LogWarning("Can't convert to double type! strText : " + strText);
            return false;
        }
    }

    public static bool Convert<T>(string strText, ref T output) where T : struct, IConvertible
    {
        if (string.IsNullOrEmpty(strText))
            return false;

        try
        {
            output = (T)Enum.Parse(typeof(T), strText);
            return true;
        }
        catch(Exception e)
        {
            Debug.LogError(e.ToString());
            return false;
        }
    }

    public static bool Approximately(float a, float b, float fTolerance = 1E-05f)
    {
        return Mathf.Abs(a - b) < fTolerance;
    }

    public static bool Approximately(Vector3 a, Vector3 b, float fTolerance = 1E-05f)
    {
        if (!Approximately(a.x, b.x, fTolerance))
        {
            return false;
        }

        if (!Approximately(a.y, b.y, fTolerance))
        {
            return false;
        }

        if (!Approximately(a.z, b.z, fTolerance))
        {
            return false;
        }

        return true;
    }

    public static byte[] CommonDataSerialize(object customObject)
    {
        var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        using (var stream = new MemoryStream())
        {
            formatter.Serialize(stream, customObject);
            return stream.ToArray();
        }
    }

    public static object CommonDataDeserialize(byte[] serializedCustomObject)
    {
        var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        using (var stream = new MemoryStream(serializedCustomObject))
        {
            return formatter.Deserialize(stream);
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