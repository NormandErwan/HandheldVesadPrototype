using NormandErwan.MasterThesis.Experiment.Experiment.Task;
using NormandErwan.MasterThesis.Experiment.Experiment.Variables;
using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Loggers
{
  public class ExperimentLogger : ExperimentBaseLogger
  {
    public class Variable
    {
      public string name;
      public int count;
      public Stopwatch time = new Stopwatch();
      public float distance;

      public Variable(string name)
      {
        this.name = name;
        Reset();
      }

      public void Reset()
      {
        count = 0;
        time.Reset();
        distance = 0;
      }

      public List<string> Colums()
      {
        return new List<string>() { name + "Count", name + "Time", name + "Distance" };
      }
    }

    // Properties

    public int HeadPhoneDistance { get; set; }

    // Variables

    protected DateTime startDateTime;

    public Variable selections = new Variable("Selections");
    protected int deselections = 0;
    protected int errors = 0;
    protected int success = 0;

    public Variable pan = new Variable("Pan");
    public Variable zoom = new Variable("Zoom");

    // Methods

    public override void Configure()
    {
      Filename = "participant-" + deviceController.ParticipantId + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".csv";

      Columns = new List<string>() {
        "ParticipantId", "Technique", "Distance", "TextSize", "TrialNumber",
        "StartDateTime", "TotalTime"
      };
      Columns.AddRange(selections.Colums());
      Columns.AddRange(new string[]{ "Deselections", "Errors", "Success" });
      Columns.AddRange(pan.Colums());
      Columns.AddRange(zoom.Colums());
      Columns.Add("HeadPhoneDistance");

      base.Configure();
    }

    protected override void Grid_Configured()
    {
      PrepareRow();

      startDateTime = DateTime.Now;

      selections.Reset();
      deselections = 0;
      errors = 0;

      pan.Reset();
      zoom.Reset();

      AddToRow(deviceController.ParticipantId);
      AddToRow(deviceController.StateController.GetIndependentVariable<IVTechnique>().CurrentCondition.id);
      AddToRow(deviceController.StateController.GetIndependentVariable<IVClassificationDifficulty>().CurrentCondition.id);
      AddToRow(deviceController.StateController.GetIndependentVariable<IVTextSize>().CurrentCondition.id);
      AddToRow(deviceController.StateController.CurrentTrial);
    }

    protected override void Grid_Completed()
    {
      AddToRow(startDateTime);
      AddToRow((DateTime.Now - startDateTime).TotalSeconds);

      AddToRow(selections);
      AddToRow(deselections);
      AddToRow(errors);
      AddToRow(success);

      AddToRow(pan);
      AddToRow(zoom);

      AddToRow(HeadPhoneDistance);

      WriteRow();
    }

    protected override void Grid_ItemSelected(Container container, Item item, bool selected)
    {
      if (selected)
      {
        selections.count++;
        selections.time.Start();
      }
      else
      {
        deselections++;
        selections.time.Stop();
      }
    }

    protected override void Grid_ItemClassed(Container oldContainer, Container newContainer, Item item, bool success)
    {
      if (success)
      {
        this.success++;
      }
      else
      {
        errors++;
      }
      selections.time.Stop();
    }

    protected override void Grid_DraggingStarted(IDraggable grid)
    {
      pan.count++;
      pan.time.Start();
    }

    protected override void Grid_Dragging(IDraggable grid, Vector3 translation)
    {
      var magnitude = translation.magnitude;
      pan.distance += magnitude;

      if (selections.time.IsRunning)
      {
        selections.distance += magnitude;
      }
    }

    protected override void Grid_DraggingStopped(IDraggable grid)
    {
      pan.time.Stop();
    }

    protected override void Grid_ZoomingStarted(IZoomable grid)
    {
      zoom.count++;
      zoom.time.Start();
    }

    protected override void Grid_Zooming(IZoomable grid, float scaleFactor, Vector3 translation, Vector3[] cursors)
    {
      var distance = cursors[0] - cursors[1];
      var previousDistance = cursors[2] - cursors[3];
      var magnitude = (distance - previousDistance).magnitude;

      zoom.distance += magnitude;

      if (selections.time.IsRunning)
      {
        selections.distance += magnitude;
      }
    }

    protected override void Grid_ZoomingStopped(IZoomable grid)
    {
      zoom.time.Stop();
    }

    protected virtual void AddToRow(Variable variable)
    {
      AddToRow(variable.count);
      AddToRow(variable.time.Elapsed.TotalSeconds);
      AddToRow(variable.distance);
    }
  }
}