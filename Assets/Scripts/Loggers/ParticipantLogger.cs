using NormandErwan.MasterThesis.Experiment.DeviceControllers;
using NormandErwan.MasterThesis.Experiment.Experiment.Variables;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Loggers
{
  public class ParticipantLogger : Logger
  {
    // Editor fields

    [SerializeField]
    private DeviceController deviceController;

    // Properties

    public int Selections { get; set; }
    public int Deselections { get; set; }
    public int Errors { get; set; }

    public int HeadPhoneDistance { get; set; }

    // Variables

    protected DateTime startDateTime;
    protected float stopWatch;

    protected bool panCount;
    protected float panTime;
    protected int panDistance;

    protected bool zoomCount;
    protected float zoomTime;
    protected int zoomDistance;

    // MonoBehaviour methods

    protected override void Awake()
    {
      base.Awake();

      deviceController.Configured += DeviceController_Configured;

      deviceController.Grid.Configured += Grid_Configured;
      deviceController.Grid.Completed += Grid_Completed;
    }

    protected override void OnDestroy()
    {
      base.OnDestroy();

      deviceController.Configured -= DeviceController_Configured;

      deviceController.Grid.Configured -= Grid_Configured;
      deviceController.Grid.Completed -= Grid_Completed;
    }

    // Methods

    public override void StartLogger()
    {
      Filename = "participant-" + deviceController.ParticipantId + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".csv";
      Columns = new List<string>() {
        "ParticipantId", "Technique", "Distance", "TextSize", "TrialNumber",
        "StartDateTime", "TotalTime",
        "Selections", "Deselections", "Errors",
        "PanCount", "PanTime", "PanDistance",
        "ZoomCount", "ZoomTime", "ZoomDistance",
        "HeadDeviceDistance"
      };
      base.StartLogger();
    }

    protected virtual void DeviceController_Configured()
    {
      StartLogger();
    }

    protected virtual void Grid_Configured()
    {
      PrepareNextRow();

      startDateTime = DateTime.Now;
      stopWatch = Time.unscaledDeltaTime;

      AddToNextRow(deviceController.ParticipantId);
      AddToNextRow(deviceController.StateController.GetIndependentVariable<IVTechnique>().CurrentCondition.id);
      AddToNextRow(deviceController.StateController.GetIndependentVariable<IVClassificationDifficulty>().CurrentCondition.id);
      AddToNextRow(deviceController.StateController.GetIndependentVariable<IVTextSize>().CurrentCondition.id);
      AddToNextRow(deviceController.StateController.CurrentTrial);
    }

    protected virtual void Grid_Completed()
    {
      AddToNextRow(startDateTime);
      AddToNextRow((startDateTime - DateTime.Now).TotalSeconds);

      WriteRow();
    }
  }
}