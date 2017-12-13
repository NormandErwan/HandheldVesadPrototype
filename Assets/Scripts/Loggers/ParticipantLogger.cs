using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NormandErwan.MasterThesis.Experiment.Loggers
{
  public class ParticipantLogger : Logger
  {
    // Properties

    public int TrialId { get; protected set; }
    public int ParticipantId { get; set; }
    public DateTime startDateTime { get; protected set; }
    public string Technique { get; set; }
    public string TextSize { get; set; }
    public string ClassificationDistance { get; set; }
    public TimeSpan TotalTime { get; protected set; }
    public int TrialNumber { get; set; }

    // Variables

    protected Stopwatch stopWatch = new Stopwatch();

    // MonoBehaviour

    protected virtual void Awake()
    {
      TrialId = 0;
    }

    // Logger methods

    public override void StartLogger()
    {
      Filename = "participant-" + ParticipantId + ".csv";
      Columns = new List<string>() { "TrialId", "ParticipantId", "StartDateTime", "Technique", "TextSize",
        "ClassificationDistance", "TotalTime", "TrialNumber" };
      base.StartLogger();
    }

    public override void PrepareNextRow()
    {
      NextRow.Clear();
      startDateTime = DateTime.Now;
      stopWatch.Start();
    }

    public override void WriteRow()
    {
      stopWatch.Stop();
      TotalTime = stopWatch.Elapsed;

      NextRow.AddRange(new string[]
      {
        TrialId.ToString(),
        ParticipantId.ToString(),
        startDateTime.ToString("yyyy-MM-dd HH-mm-ss"),
        Technique,
        TextSize,
        ClassificationDistance,
        TotalTime.TotalSeconds.ToString(),
        TrialNumber.ToString(),
      });

      base.WriteRow();
      TrialId++;
    }
  }
}