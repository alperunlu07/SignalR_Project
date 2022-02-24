using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JsonModelHelper
{
    public string filename { get; set; }


    public JsonModelHelper(string _filename){
        filename = _filename;
    }


    public void writeModel<T>(T obj)
    {
        string s = JsonConvert.SerializeObject(obj);

        StreamWriter stream = new StreamWriter(Application.streamingAssetsPath + "/" + filename + ".json");
        stream.Write(s);
        stream.Close(); 

    }

    public static string ModelToString<T>(T obj)
    {
        return JsonConvert.SerializeObject(obj);
    }
    public static T StringToModel<T>(string s)
    {
        try
        {
            return JsonConvert.DeserializeObject<T>(s);
        }
        catch (Exception)
        {
            return default(T);
        }
       
    }

    public T readModel<T>()
    {
        try
        {
            string filePath = Application.streamingAssetsPath + "/" + filename + ".json"; 
            string jsonString = ""; 

            StreamReader reader1 = new StreamReader(filePath,System.Text.Encoding.UTF8);
            jsonString = reader1.ReadToEnd();
            reader1.Close(); 

            var _data = JsonConvert.DeserializeObject<T>(jsonString);
            return _data;
        }
        catch (Exception e)
        { 
            return default(T); 
        }


    }
}
