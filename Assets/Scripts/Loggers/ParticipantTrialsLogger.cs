using NormandErwan.MasterThesis.Experiment.Experiment.Task;
using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
using NormandErwan.MasterThesis.Experiment.Loggers.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Loggers
{
  public class ParticipantTrialsLogger : ExperimentBaseLogger
  {
    // Variables

    protected DateTime startDateTime;

    protected InteractionLogger selections = new InteractionLogger("selections");
    protected int deselections = 0;
    protected int errors = 0;
    protected int classifications = 0;

    protected InteractionLogger pan = new InteractionLogger("pan");
    protected InteractionLogger zoom = new InteractionLogger("zoom");

    protected CursorDistance indexDistance, thumbDistance;
    protected ProjectedCursorDistance projectedIndexDistance, projectedThumbDistance;
    protected AbsoluteHeadPhoneDistance absoluteHeadPhoneDistance;
    protected SignedHeadPhoneDistance signedHeadPhoneDistance;

    // MonoBehaviour methods

    protected virtual void LateUpdate()
    {
      if (IsConfigured && stateController.CurrentState.Id == stateController.taskTrialState.Id)
      {
        indexDistance.Update();
        projectedIndexDistance.Update();

        if (selections.Time.IsRunning)
        {
          selections.Distance += indexDistance.Current;
          selections.ProjectedDistance += projectedIndexDistance.Current;
        }
        if (pan.Time.IsRunning)
        {
          pan.Distance += indexDistance.Current;
          pan.ProjectedDistance += projectedIndexDistance.Current;
        }
        if (zoom.Time.IsRunning)
        {
          if (technique.CurrentCondition.useTouchInput)
          {
            thumbDistance.Update();
            projectedThumbDistance.Update();
          }

          zoom.Distance += indexDistance.Current + thumbDistance.Current;
          zoom.ProjectedDistance += projectedIndexDistance.Current + projectedThumbDistance.Current;
        }

        absoluteHeadPhoneDistance.Update();
        signedHeadPhoneDistance.Update();
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
        "trial", "grid_config",
        "start_date_time", "total_time"
      };
      Columns.AddRange(selections.Columns());
      Columns.AddRange(new string[]{ "deselections", "errors", "classifications" });
      Columns.AddRange(pan.Columns());
      Columns.AddRange(zoom.Columns());
      Columns.Add("absolute_head_phone_distance");
      Columns.Add("signed_head_phone_distance");

      indexDistance = new CursorDistance(Index);
      thumbDistance = new CursorDistance(Thumb);
      projectedIndexDistance = new ProjectedCursorDistance(ProjectedIndex);
      projectedThumbDistance = new ProjectedCursorDistance(ProjectedThumb);

      absoluteHeadPhoneDistance = new AbsoluteHeadPhoneDistance(mobileDevice, head);
      signedHeadPhoneDistance = new SignedHeadPhoneDistance(mobileDevice, head);

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
        classifications = 0;

        pan.Reset();
        zoom.Reset();

        absoluteHeadPhoneDistance.Reset();
        signedHeadPhoneDistance.Reset();

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
      if (!CurrentRowCompleted)
      {
        // Complete the row
        AddToRow(startDateTime);
        AddToRow((DateTime.Now - startDateTime).TotalSeconds);

        AddToRow(selections);
        AddToRow(deselections);
        AddToRow(errors);
        AddToRow(classifications);

        AddToRow(pan);
        AddToRow(zoom);

        AddToRow(absoluteHeadPhoneDistance.Total);
        AddToRow(signedHeadPhoneDistance.Total.magnitude);

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

    protected virtual void AddToRow(InteractionLogger variable)
    {
      AddToRow(variable.Count);
      AddToRow(variable.Time.Elapsed.TotalSeconds);
      AddToRow(variable.Distance);
      AddToRow(variable.ProjectedDistance);
    }
  }
}