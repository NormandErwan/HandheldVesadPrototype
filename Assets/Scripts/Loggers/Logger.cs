using Mono.Csv;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Loggers
{
  public abstract class Logger : MonoBehaviour
  {
    // Properties

    public string Filename { get; protected set; }
    public string FilePath { get; protected set; }

    public List<string> Columns { get; protected set; }

    protected List<string> NextRow { get; private set; }

    // Variables

    protected CsvFileWriter csvWriter;
    protected string dataPath;

    // MonoBehaviour methods

    protected virtual void Awake()
    {
      dataPath = (Application.isEditor) ? Application.dataPath : Application.persistentDataPath;
    }

    protected virtual void OnDestroy()
    {
      if (csvWriter != null)
      {
        StopLogger();
      }
    }

    // Methods

    public virtual void StartLogger()
    {
      FilePath = Path.Combine(dataPath, Filename);

      csvWriter = new CsvFileWriter(FilePath);
      csvWriter.WriteRow(Columns);

      NextRow = new List<string>(Columns.Count);
      for (int i = 0; i < Columns.Count; i++)
      {
        NextRow.Add("");
      }
    }

    public virtual void PrepareNextRow()
    {
      NextRow.Clear();
    }

    public virtual void WriteRow()
    {
      csvWriter.WriteRow(NextRow);
    }

    public virtual void StopLogger()
    {
      csvWriter.Dispose();
    }

    protected virtual void AddToNextRow(string text)
    {
      NextRow.Add(text);
    }

    protected virtual void AddToNextRow(int number)
    {
      NextRow.Add(number.ToString());
    }

    protected virtual void AddToNextRow(float number)
    {
      NextRow.Add(number.ToString());
    }

    protected virtual void AddToNextRow(DateTime dateTime)
    {
      AddToNextRow(dateTime.ToString("yyyy-MM-dd HH-mm-ss"));
    }

    protected virtual void AddToNextRow(bool boolean)
    {
      AddToNextRow((boolean) ? "true" : "false");
    }
  }
}