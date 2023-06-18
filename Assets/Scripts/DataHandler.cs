using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class DataHandler
{
    private string dataDirPath = "";

    private string dataFileName = "";

    public DataHandler(string dataDirPath, string dataFileName){
        this.dataDirPath=dataDirPath;
        this.dataFileName=dataFileName;
    }

    public void Save(GameMetrics data){
        string  fullPath= Path.Combine(dataDirPath,dataFileName);
        try
        {
          Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

          string dataToStore = JsonUtility.ToJson(data, true);

          using(FileStream stream = new FileStream(fullPath, FileMode.Create)){
            using (StreamWriter writer  = new StreamWriter(stream))
            {
                writer.Write(dataToStore);
            }
          }  
        }
        catch (Exception e)
        {
            Debug.LogError("error when trying to save file");
        }
    }
}
