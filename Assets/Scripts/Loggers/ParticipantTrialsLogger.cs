using NormandErwan.MasterThesis.Experiment.Experiment.Task;
using NormandErwan.MasterThesis.Experiment.Experiment.Variables;
using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Loggers
{
  public class ParticipantTrialsLogger : ExperimentBaseLogger
  {
    public class Variable
    {
      public string name;
      public int count;
      public Stopwatch time = new Stopwatch();
      public float distance, projectedDistance;

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
        projectedDistance = 0;
      }

      public List<string> Columns()
      {
        return new List<string>() { name + "_count", name + "_time", name + "_distance", name + "_projected_distance" };
      }
    }

    // Variables

    protected IVTechnique technique;
    protected IVTextSize textSize;
    protected IVClassificationDifficulty distance;

    protected DateTime startDateTime;

    protected Variable selections = new Variable("selections");
    protected int deselections = 0;
    protected int errors = 0;
    protected int classifications = 0;

    protected Variable pan = new Variable("pan");
    protected Variable zoom = new Variable("zoom");

    protected Vector3 oldIndexPosition, oldProjectedIndexPosition;
    protected Vector3 oldThumbPosition, oldProjectedThumbPosition;

    protected float headPhoneDistance = 0;
    protected float oldHeadPhoneDistance = 0;

    // MonoBehaviour methods

    protected virtual void Start()
    {
      oldHeadPhoneDistance = (head.position - mobileDevice.position).magnitude;

      technique = stateController.GetIndependentVariable<IVTechnique>();
      textSize = stateController.GetIndependentVariable<IVTextSize>();
      distance = stateController.GetIndependentVariable<IVClassificationDifficulty>();
    }

    protected virtual void LateUpdate()
    {
      if (IsConfigured && stateController.CurrentState.Id == stateController.taskTrialState.Id)
      {
        float distance = (Index.transform.position - oldIndexPosition).magnitude 
          + (Thumb.transform.position - oldThumbPosition).magnitude;
        oldIndexPosition = Index.transform.position;
        oldThumbPosition = Thumb.transform.position;

        float projectedDistance = (ProjectedIndex.transform.position - oldProjectedIndexPosition).magnitude 
          + (ProjectedThumb.transform.position - oldProjectedThumbPosition).magnitude;
        oldProjectedIndexPosition = ProjectedIndex.transform.position;
        oldProjectedThumbPosition = ProjectedThumb.transform.position;

        if (selections.time.IsRunning)
        {
          selections.distance += distance;
          selections.projectedDistance += projectedDistance;
        }
        if (pan.time.IsRunning)
        {
          pan.distance += distance;
          pan.projectedDistance += projectedDistance;
        }
        if (zoom.time.IsRunning)
        {
          zoom.distance += distance;
          zoom.projectedDistance += projectedDistance;
        }

        float headPhoneDistance = (head.position - mobileDevice.position).magnitude;
        this.headPhoneDistance += headPhoneDistance - oldHeadPhoneDistance;
        oldHeadPhoneDistance = headPhoneDistance;
      }
    }

    // Methods

    public override void Configure()
    {
      Filename = "participant-" + deviceController.ParticipantId + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + "_trials.csv";

      Columns = new List<string>() {
        "participant_id",
        "technique_id", "technique_name",
        "text_size_id", "text_size_name",
        "distance_id", "distance_name",
        "trial_number", "grid_config",
        "start_date_time", "total_time"
      };
      Columns.AddRange(selections.Columns());
      Columns.AddRange(new string[]{ "deselections", "errors", "classifications" });
      Columns.AddRange(pan.Columns());
      Columns.AddRange(zoom.Columns());
      Columns.Add("head_phone_distance");

      base.Configure();
    }

    protected override void TaskGrid_Configured()
    {
      if (stateController.CurrentState.Id == stateController.taskTrialState.Id)
      {
        PrepareRow();

        startDateTime = DateTime.Now;

        selections.Reset();
        deselections = 0;
        errors = 0;

        pan.Reset();
        zoom.Reset();

        AddToRow(deviceController.ParticipantId);
        AddToRow(technique.CurrentCondition.Id); AddToRow(technique.CurrentCondition.Name);
        AddToRow(textSize.CurrentCondition.Id); AddToRow(textSize.CurrentCondition.Name);
        AddToRow(distance.CurrentCondition.Id); AddToRow(distance.CurrentCondition.Name);
        AddToRow(stateController.CurrentTrial);
        AddToRow(taskGrid.GridGenerator.ToString());
      }
    }

    protected override void TaskGrid_Completed()
    {
      if (stateController.CurrentState.Id == stateController.taskTrialState.Id)
      {
        AddToRow(startDateTime);
        AddToRow((DateTime.Now - startDateTime).TotalSeconds);

        AddToRow(selections);
        AddToRow(deselections);
        AddToRow(errors);
        AddToRow(classifications);

        AddToRow(pan);
        AddToRow(zoom);

        AddToRow(headPhoneDistance);

        WriteRow();
      }
    }

    protected override void TaskGrid_ItemSelected(Container container, Item item)
    {
      if (item.IsSelected)
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

    protected override void TaskGrid_ItemMoved(Container oldContainer, Container newContainer, Item item, TaskGrid.ItemMovedType moveType)
    {
      if (moveType == TaskGrid.ItemMovedType.Classified)
      {
        classifications++;
      }
      else if (moveType == TaskGrid.ItemMovedType.Error)
      {
        errors++;
      }
      selections.time.Stop();
    }

    protected override void TaskGrid_DraggingStarted(IDraggable grid)
    {
      pan.count++;
      pan.time.Start();
    }

    protected override void TaskGrid_Dragging(IDraggable grid, Vector3 translation)
    {
    }

    protected override void TaskGrid_DraggingStopped(IDraggable grid)
    {
      pan.time.Stop();
    }

    protected override void TaskGrid_ZoomingStarted(IZoomable grid)
    {
      zoom.count++;
      zoom.time.Start();
    }

    protected override void TaskGrid_Zooming(IZoomable grid, Vector3 scaling, Vector3 translation)
    {
    }

    protected override void TaskGrid_ZoomingStopped(IZoomable grid)
    {
      zoom.time.Stop();
    }

    protected virtual void AddToRow(Variable variable)
    {
      AddToRow(variable.count);
      AddToRow(variable.time.Elapsed.TotalSeconds);
      AddToRow(variable.distance);
      AddToRow(variable.projectedDistance);
    }
  }
}