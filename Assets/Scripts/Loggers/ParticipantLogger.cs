using System;
using System.Collections.Generic;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Loggers
{
  public class ParticipantLogger : Logger
  {
    // Properties

    public string DeviceControllerName { get; set; }
    public int TrialId { get; protected set; }
    public int ParticipantId { get; set; }
    public DateTime startDateTime { get; protected set; }
    public string Technique { get; set; }
    public string TextSize { get; set; }
    public string ClassificationDistance { get; set; }
    public float TotalTime { get; protected set; }
    public int TrialNumber { get; set; }

    // Variables

    protected float stopWatch;

    // MonoBehaviour

    protected override void Awake()
    {
      base.Awake();
      TrialId = 0;
    }

    // Logger methods

    public override void StartLogger()
    {
      Filename = "participant-" + ParticipantId + "-" + DeviceControllerName + ".csv";
      Columns = new List<string>() { "TrialId", "ParticipantId", "StartDateTime", "Technique", "TextSize",
        "ClassificationDistance", "TotalTime", "TrialNumber" };
      base.StartLogger();
    }

    public override void PrepareNextRow()
    {
      startDateTime = DateTime.Now;
      stopWatch = Time.unscaledDeltaTime;
    }

    public override void WriteRow()
    {
      TotalTime = Time.unscaledDeltaTime - stopWatch;

      AddToNextRow(TrialId);
      AddToNextRow(ParticipantId);
      AddToNextRow(startDateTime);
      AddToNextRow(Technique);
      AddToNextRow(TextSize);
      AddToNextRow(ClassificationDistance);
      AddToNextRow(TotalTime);
      AddToNextRow(TrialNumber);

      TrialId++;

      base.WriteRow();
    }
  }
}