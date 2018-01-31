using NormandErwan.MasterThesis.Experiment.Experiment.Task;
using NormandErwan.MasterThesis.Experiment.Experiment.Variables;
using NormandErwan.MasterThesis.Experiment.Inputs;
using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Loggers
{
  public class ParticipantTrialsLogger : ExperimentBaseLogger
  {
    protected class Variable
    {
      public string Name { get; protected set; }
      public Stopwatch Time { get; protected set; }

      public int Count { get; set; }
      public float Distance { get; set; }
      public float ProjectedDistance { get; set; }

      public Variable(string name)
      {
        Name = name;
        Time = new Stopwatch();
        Reset();
      }

      public void Reset()
      {
        Time.Reset();

        Count = 0;
        Distance = 0;
        ProjectedDistance = 0;
      }

      public List<string> Columns()
      {
        return new List<string>() { Name + "_count", Name + "_time", Name + "_distance", Name + "_projected_distance" };
      }
    }

    protected class Distance
    {
      public bool Started { get; protected set; }
      public float TotalDistance { get; protected set; }
      public float CurrentDistance { get; protected set; }
      public float PreviousDistance { get; protected set; }

      protected Vector3 previousPosition;
      protected Func<bool> updateThisFrame;
      protected Func<float> getCurrentDistance;

      public Distance(Func<bool> updateThisFrame, Func<Vector3> getTrackedPosition)
      {
        this.updateThisFrame = updateThisFrame;
        getCurrentDistance = () =>
        {
          var position = getTrackedPosition();
          var distance = (position - previousPosition).magnitude;
          previousPosition = position;
          return distance;
        };
        Reset();
      }

      public Distance(Func<bool> updateThisFrame, Func<float> getCurrentDistance)
      {
        this.updateThisFrame = updateThisFrame;
        this.getCurrentDistance = getCurrentDistance;
        Reset();
      }

      public void Reset()
      {
        Started = false;
        TotalDistance = 0;
        CurrentDistance = 0;
        PreviousDistance = 0;
      }

      public void Update()
      {
        if (updateThisFrame())
        {
          CurrentDistance = getCurrentDistance();
          if (!Started)
          {
            Started = true;
            TotalDistance = 0;
          }
          else
          {
            TotalDistance += Mathf.Abs(CurrentDistance - PreviousDistance);
          }
          PreviousDistance = CurrentDistance;
        }
      }
    }
    protected class CursorDistance : Distance
    {
      public CursorDistance(BaseCursor cursor) 
        : base(() => { return cursor.IsVisible; }, () => { return cursor.transform.position; })
      {
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

    protected Distance indexDistance, thumbDistance;
    protected Distance projectedIndexDistance, projectedThumbDistance;
    protected Distance headPhoneDistance;

    // MonoBehaviour methods

    protected virtual void Start()
    {
      technique = stateController.GetIndependentVariable<IVTechnique>();
      textSize = stateController.GetIndependentVariable<IVTextSize>();
      distance = stateController.GetIndependentVariable<IVClassificationDifficulty>();
    }

    protected virtual void LateUpdate()
    {
      if (IsConfigured && stateController.CurrentState.Id == stateController.taskTrialState.Id)
      {
        indexDistance.Update();
        thumbDistance.Update();
        projectedIndexDistance.Update();
        projectedThumbDistance.Update();

        if (selections.Time.IsRunning)
        {
          selections.Distance += indexDistance.CurrentDistance + thumbDistance.CurrentDistance;
          selections.ProjectedDistance += projectedIndexDistance.CurrentDistance + projectedThumbDistance.CurrentDistance;
        }
        if (pan.Time.IsRunning)
        {
          pan.Distance += indexDistance.CurrentDistance + thumbDistance.CurrentDistance;
          pan.ProjectedDistance += projectedIndexDistance.CurrentDistance + projectedThumbDistance.CurrentDistance;
        }
        if (zoom.Time.IsRunning)
        {
          zoom.Distance += indexDistance.CurrentDistance + thumbDistance.CurrentDistance;
          zoom.ProjectedDistance += projectedIndexDistance.CurrentDistance + projectedThumbDistance.CurrentDistance;
        }

        headPhoneDistance.Update();
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

      indexDistance = new CursorDistance(Index);
      thumbDistance = new CursorDistance(Thumb);
      projectedIndexDistance = new CursorDistance(ProjectedIndex);
      projectedThumbDistance = new CursorDistance(ProjectedThumb);
      headPhoneDistance = new Distance(() => { return mobileDevice.IsTracking; }, () => { return (head.position - mobileDevice.transform.position).magnitude; });

      base.Configure();
    }

    protected override void TaskGrid_Configured()
    {
      if (stateController.CurrentState.Id == stateController.taskTrialState.Id)
      {
        PrepareRow();

        // Reset fields
        startDateTime = DateTime.Now;

        indexDistance.Reset();
        thumbDistance.Reset();
        projectedIndexDistance.Reset();
        projectedThumbDistance.Reset();

        selections.Reset();
        deselections = 0;
        errors = 0;

        pan.Reset();
        zoom.Reset();

        headPhoneDistance.Reset();

        // Add the first columns to the current row
        AddToRow(deviceController.ParticipantId);

        AddToRow(technique.CurrentCondition.Id);
        AddToRow(technique.CurrentCondition.Name);

        AddToRow(textSize.CurrentCondition.Id);
        AddToRow(textSize.CurrentCondition.Name);

        AddToRow(distance.CurrentCondition.Id);
        AddToRow(distance.CurrentCondition.Name);

        AddToRow(stateController.CurrentTrial);
        AddToRow(taskGrid.GridGenerator.ToString());
      }
    }

    protected override void TaskGrid_Completed()
    {
      // Complete the row
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

        AddToRow(headPhoneDistance.TotalDistance);

        WriteRow();
      }
    }

    protected override void TaskGrid_ItemSelected(Container container, Item item)
    {
      if (item.IsSelected)
      {
        selections.Count++;
        selections.Time.Start();
      }
      else
      {
        deselections++;
        selections.Time.Stop();
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
      selections.Time.Stop();
    }

    protected override void TaskGrid_DraggingStarted(IDraggable grid)
    {
      pan.Count++;
      pan.Time.Start();
    }

    protected override void TaskGrid_Dragging(IDraggable grid, Vector3 translation)
    {
    }

    protected override void TaskGrid_DraggingStopped(IDraggable grid)
    {
      pan.Time.Stop();
    }

    protected override void TaskGrid_ZoomingStarted(IZoomable grid)
    {
      zoom.Count++;
      zoom.Time.Start();
    }

    protected override void TaskGrid_Zooming(IZoomable grid, Vector3 scaling, Vector3 translation)
    {
    }

    protected override void TaskGrid_ZoomingStopped(IZoomable grid)
    {
      zoom.Time.Stop();
    }

    protected virtual void AddToRow(Variable variable)
    {
      AddToRow(variable.Count);
      AddToRow(variable.Time.Elapsed.TotalSeconds);
      AddToRow(variable.Distance);
      AddToRow(variable.ProjectedDistance);
    }
  }
}