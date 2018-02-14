using Mono.Csv;
using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Loggers
{
  public abstract class Logger : MonoBehaviour
  {
    // Properties

    public string Filename { get; protected set; }
    public string FilePath { get; protected set; }

    public List<string> Columns { get; protected set; }
    public List<string> Row { get; protected set; }

    public bool IsConfigured { get; private set; }
    public bool CurrentRowCompleted { get; protected set; }

    // Variables

    protected CsvFileWriter csvWriter;
    protected string dataPath;
    protected int columnIndex = 0;

    // MonoBehaviour methods

    protected virtual void Awake()
    {
      IsConfigured = false;
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

    public virtual void Configure()
    {
      Row = new List<string>(new string[Columns.Count]);
      CurrentRowCompleted = true;

      Directory.CreateDirectory(dataPath);
      FilePath = Path.Combine(dataPath, Filename);

      csvWriter = new CsvFileWriter(FilePath);
      csvWriter.WriteRow(Columns);

      IsConfigured = true;
    }

    public virtual void PrepareRow()
    {
      CurrentRowCompleted = false;
      columnIndex = 0;
    }

    public virtual void WriteRow()
    {
      CurrentRowCompleted = true;
      csvWriter.WriteRow(Row);
    }

    public virtual void StopLogger()
    {
      csvWriter.Dispose();
    }

    protected virtual void AddTransformToColumns(string transform, bool addScale = true)
    {
      AddVector3ToColumns(transform + "_position");
      AddVector3ToColumns(transform + "_rotation");
      if (addScale)
      {
        AddVector3ToColumns(transform + "_scale");
      }
    }

    protected virtual void AddVector3ToColumns(string vector3)
    {
      Columns.AddRange(new string[] { vector3 + "_x", vector3 + "_y", vector3 + "_z" });
    }

    protected virtual void AddToRow(string text)
    {
      Row[columnIndex] = text;
      columnIndex++;
    }

    protected virtual void AddToRow(bool boolean)
    {
      AddToRow((boolean) ? "true" : "false");
    }

    protected virtual void AddToRow(DateTime dateTime)
    {
      AddToRow(dateTime.ToString("yyyy-MM-dd HH-mm-ss"));
    }

    protected virtual void AddToRow(int number)
    {
      AddToRow(number.ToString(CultureInfo.InvariantCulture));
    }

    protected virtual void AddToRow(float number)
    {
      AddToRow(number.ToString(CultureInfo.InvariantCulture));
    }

    protected virtual void AddToRow(double number)
    {
      AddToRow(number.ToString(CultureInfo.InvariantCulture));
    }

    protected virtual void AddToRow(Transform transform, bool addScale = true)
    {
      AddToRow(transform.position);
      AddToRow(transform.rotation.eulerAngles);
      if (addScale)
      {
        AddToRow(transform.lossyScale);
      }
    }

    protected virtual void AddToRow(Vector3 vector3)
    {
      AddToRow(vector3.x);
      AddToRow(vector3.y);
      AddToRow(vector3.z);
    }
  }
}