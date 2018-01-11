using NormandErwan.MasterThesis.Experiment.Experiment.Task;
using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Loggers
{
  public class ExperimentDetailsLogger : ExperimentBaseLogger
  {
    // Editor fields

    [SerializeField]
    private Inputs.Cursor index;

    // Variables

    protected bool gridConfigured, gridCompleted;
    protected bool itemSelected, itemDeselected, itemMoved, itemClassified;
    protected Container container, oldContainer;
    protected Item item;
    protected bool zoomMode;

    // MonoBehaviour methods

    protected virtual void LateUpdate()
    {
      if (IsConfigured && stateController.CurrentState.id == stateController.taskTrialState.id)
      {
        PrepareRow();

        AddToRow(Time.frameCount);
        AddToRow(deviceController.ParticipantId);
        AddToRow(DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss.fff"));

        AddToRow(head, false);
        AddToRow(mobileDevice, false);

        AddToRow(grid.transform, false);
        AddToRow(grid.LossyScale);
        AddToRow(gridConfigured);
        AddToRow(gridCompleted);
        AddToRow(itemSelected);
        AddToRow(itemDeselected);
        AddToRow(itemMoved);
        AddToRow(itemClassified);
        AddToRow(container);
        AddToRow(item);
        AddToRow(oldContainer);

        AddToRow(index.transform, false);
        AddToRow(zoomMode);

        WriteRow();

        gridConfigured = gridCompleted = false;
        itemSelected = itemDeselected = itemMoved = itemClassified = false;
        container = oldContainer = null;
        item = null;
        zoomMode = false;
      }
    }

    // Methods

    public override void Configure()
    {
      Filename = "participant-" + deviceController.ParticipantId + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + "_details.csv";

      Columns = new List<string>() { "frame_id", "participant_id", "date_time" };

      AddTransformToColumns("head", false);
      AddTransformToColumns("phone", false);

      AddTransformToColumns("grid");
      Columns.AddRange(new string[] {
        "grid_configured", "grid_completed",
        "item_selected", "item_deselected", "item_moved", "item_classified",
        "container", "item", "old_container"
      });

      AddTransformToColumns("index", false);
      Columns.Add("zoomMode");

      base.Configure();
    }

    protected override void Grid_Configured()
    {
      gridConfigured = true;
    }

    protected override void Grid_Completed()
    {
      gridCompleted = true;
    }

    protected override void Grid_ItemSelected(Container container, Item item, bool selected)
    {
      if (selected)
      {
        itemSelected = true;
      }
      else
      {
        itemDeselected = true;
      }
      this.container = container;
      this.item = item;
    }

    protected override void Grid_ItemMoved(Container oldContainer, Container newContainer, Item item, bool classified)
    {
      itemMoved = true;
      if (classified)
      {
        itemClassified = true;
      }
      container = newContainer;
      this.item = item;
      this.oldContainer = oldContainer;
    }

    protected override void Grid_DraggingStarted(IDraggable grid)
    {
    }

    protected override void Grid_Dragging(IDraggable grid, Vector3 translation)
    {
    }

    protected override void Grid_DraggingStopped(IDraggable grid)
    {
    }

    protected override void Grid_ZoomingStarted(IZoomable grid)
    {
    }

    protected override void Grid_Zooming(IZoomable grid, float scaleFactor, Vector3 translation, Vector3[] cursors)
    {
    }

    protected override void Grid_ZoomingStopped(IZoomable grid)
    {
    }

    protected virtual void AddToRow(Container container)
    {
      if (container == null)
      {
        AddToRow("");
      }
      else
      {
        var position = grid.GetPosition(container);
        AddToRow("(" + position.x + ", " + position.y + ")");
      }
    }

    protected virtual void AddToRow(Item item)
    {
      AddToRow((item == null) ? "" : container.Elements.IndexOf(item).ToString());
    }
  }
}