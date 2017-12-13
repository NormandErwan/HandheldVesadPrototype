using Mono.Csv;
using System.Collections.Generic;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Loggers
{
  public abstract class Logger : MonoBehaviour
  {
    // Properties

    public virtual string Filename { get; protected set; }
    public virtual List<string> Columns { get; protected set; }

    protected virtual List<string> NextRow { get; private set; }

    // Variables

    protected CsvFileWriter csvWriter;

    // MonoBehaviour methods

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
      csvWriter = new CsvFileWriter(Filename);
      csvWriter.WriteRow(Columns);

      NextRow = new List<string>(Columns.Count);
      for (int i = 0; i < Columns.Count; i++)
      {
        NextRow.Add("");
      }
    }

    public virtual void PrepareNextRow()
    {
    }

    public virtual void WriteRow()
    {
      csvWriter.WriteRow(NextRow);
    }

    public virtual void StopLogger()
    {
      csvWriter.Dispose();
    }

    protected virtual string ToString(bool boolean)
    {
      return (boolean) ? "true" : "false";
    }
  }
}