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

    protected bool itemSelected, itemDeselected, itemMoved, itemClassified;
    protected Container selectedContainer;
    protected Item selectedItem;
    protected bool zoomMode;

    // MonoBehaviour methods

    protected virtual void LateUpdate()
    {
      if (IsConfigured && stateController.CurrentState.id == stateController.taskTrialState.id)
      {
        if (!itemSelected && itemDeselected)
        {
          selectedItem = null;
          selectedContainer = null;
        }

        PrepareRow();

        AddToRow(Time.frameCount);
        AddToRow(deviceController.ParticipantId);
        AddToRow(DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss.fff"));

        AddToRow(grid.transform, false);
        AddToRow(grid.LossyScale);
        AddToRow(grid.IsConfigured);
        AddToRow(grid.IsCompleted);

        AddToRow(itemSelected);
        AddToRow(itemDeselected);
        AddToRow(itemMoved);
        AddToRow(itemClassified);
        AddToRow(selectedContainer);
        AddToRow(selectedItem);

        AddToRow(index.transform, false);
        AddToRow(zoomMode);

        AddToRow(head, false);
        AddToRow(mobileDevice, false);

        WriteRow();

        if (itemMoved || itemClassified)
        {
          selectedItem = null;
          selectedContainer = null;
        }
        itemSelected = itemDeselected = itemMoved = itemClassified = false;
      }
    }

    // Methods

    public override void Configure()
    {
      Filename = "participant-" + deviceController.ParticipantId + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + "_details.csv";

      Columns = new List<string>() { "frame_id", "participant_id", "date_time" };

      AddTransformToColumns("grid");
      Columns.AddRange(new string[] {
        "grid_is_configured", "grid_is_completed",
        "item_selected", "item_deselected", "item_moved", "item_classified",
        "selected_container", "selected_item"
      });

      AddTransformToColumns("index", false);
      Columns.Add("zoomMode");

      AddTransformToColumns("head", false);
      AddTransformToColumns("phone", false);

      base.Configure();
    }

    protected override void Grid_Configured()
    {
    }

    protected override void Grid_Completed()
    {
    }

    protected override void Grid_ItemSelected(Container container, Item item, bool selected)
    {
      if (selected)
      {
        itemSelected = true;
        selectedContainer = container;
        selectedItem = item;
      }
      else
      {
        itemDeselected = true;
      }
    }

    protected override void Grid_ItemMoved(Container oldContainer, Container newContainer, Item item, bool classified)
    {
      itemMoved = true;
      if (classified)
      {
        itemClassified = true;
      }
      selectedContainer = newContainer;
      selectedItem = item;
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
      AddToRow((item == null) ? "" : selectedContainer.Elements.IndexOf(item).ToString());
    }
  }
}