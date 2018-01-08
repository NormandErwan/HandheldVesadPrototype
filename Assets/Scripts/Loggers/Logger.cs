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
    public List<string> NextRow { get; protected set; }

    // Variables

    protected CsvFileWriter csvWriter;
    protected string dataPath;
    protected int columnIndex = 0;

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
      NextRow = new List<string>(new string[Columns.Count]);

      FilePath = Path.Combine(dataPath, Filename);
      csvWriter = new CsvFileWriter(FilePath);
      csvWriter.WriteRow(Columns);
    }

    public virtual void PrepareNextRow()
    {
      columnIndex = 0;
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
      NextRow[columnIndex] = text;
      columnIndex++;
    }

    protected virtual void AddToNextRow(int number)
    {
      AddToNextRow(number.ToString());
    }

    protected virtual void AddToNextRow(float number)
    {
      AddToNextRow(number.ToString());
    }

    protected virtual void AddToNextRow(double number)
    {
      AddToNextRow(number.ToString());
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