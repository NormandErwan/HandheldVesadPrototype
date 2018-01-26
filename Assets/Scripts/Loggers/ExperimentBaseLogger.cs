using NormandErwan.MasterThesis.Experiment.DeviceControllers;
using NormandErwan.MasterThesis.Experiment.Experiment.States;
using NormandErwan.MasterThesis.Experiment.Experiment.Task;
using NormandErwan.MasterThesis.Experiment.Inputs;
using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Loggers
{
  public abstract class ExperimentBaseLogger : Logger
  {
    // Editor fields

    [SerializeField]
    protected Transform head;

    [SerializeField]
    protected Transform mobileDevice;

    [SerializeField]
    protected DeviceController deviceController;

    [SerializeField]
    protected BaseCursor projectedRightIndex;

    [SerializeField]
    protected BaseCursor projectedLeftIndex;

    [SerializeField]
    protected BaseCursor projectedRightThumb;

    [SerializeField]
    protected BaseCursor projectedLeftThumb;

    // Properties

    protected BaseCursor Index { get; set; }
    protected BaseCursor ProjectedIndex { get; set; }

    protected BaseCursor Thumb { get; set; }
    protected BaseCursor ProjectedThumb { get; set; }

    // Variables

    protected StateController stateController;
    protected TaskGrid taskGrid;

    // MonoBehaviour methods

    protected override void Awake()
    {
      base.Awake();

      dataPath += "/Logs";

      stateController = deviceController.StateController;
      taskGrid = deviceController.TaskGrid;

      deviceController.Configured += DeviceController_Configured;

      taskGrid.Configured += TaskGrid_Configured;
      taskGrid.Completed += TaskGrid_Completed;
      taskGrid.ItemSelected += TaskGrid_ItemSelected;
      taskGrid.ItemMoved += TaskGrid_ItemMoved;

      taskGrid.DraggingStarted += TaskGrid_DraggingStarted;
      taskGrid.Dragging += TaskGrid_Dragging;
      taskGrid.DraggingStopped += TaskGrid_DraggingStopped;

      taskGrid.ZoomingStarted += TaskGrid_ZoomingStarted;
      taskGrid.Zooming += TaskGrid_Zooming;
      taskGrid.ZoomingStopped += TaskGrid_ZoomingStopped;
    }

    protected override void OnDestroy()
    {
      base.OnDestroy();

      deviceController.Configured -= DeviceController_Configured;

      taskGrid.Configured -= TaskGrid_Configured;
      taskGrid.Completed -= TaskGrid_Completed;
      taskGrid.ItemSelected -= TaskGrid_ItemSelected;
      taskGrid.ItemMoved -= TaskGrid_ItemMoved;

      taskGrid.DraggingStarted -= TaskGrid_DraggingStarted;
      taskGrid.Dragging -= TaskGrid_Dragging;
      taskGrid.DraggingStopped -= TaskGrid_DraggingStopped;

      taskGrid.ZoomingStarted -= TaskGrid_ZoomingStarted;
      taskGrid.Zooming -= TaskGrid_Zooming;
      taskGrid.ZoomingStopped -= TaskGrid_ZoomingStopped;
    }

    // Methods

    protected virtual void DeviceController_Configured()
    {
      if (deviceController.ParticipantIsRightHanded)
      {
        Index = deviceController.CursorsInput.Cursors[CursorType.RightIndex];
        Thumb = deviceController.CursorsInput.Cursors[CursorType.RightThumb];

        ProjectedIndex = projectedRightIndex;
        ProjectedThumb = projectedRightThumb;
      }
      else
      {
        Index = deviceController.CursorsInput.Cursors[CursorType.LeftIndex];
        Thumb = deviceController.CursorsInput.Cursors[CursorType.LeftThumb];

        ProjectedIndex = projectedLeftIndex;
        ProjectedThumb = projectedLeftThumb;
      }

      Configure();
    }

    protected abstract void TaskGrid_Configured();
    protected abstract void TaskGrid_Completed();

    protected abstract void TaskGrid_ItemSelected(Container container, Item item);
    protected abstract void TaskGrid_ItemMoved(Container oldContainer, Container newContainer, Item item, TaskGrid.ItemMovedType moveType);

    protected abstract void TaskGrid_DraggingStarted(IDraggable grid);
    protected abstract void TaskGrid_Dragging(IDraggable grid, Vector3 translation);
    protected abstract void TaskGrid_DraggingStopped(IDraggable grid);

    protected abstract void TaskGrid_ZoomingStarted(IZoomable grid);
    protected abstract void TaskGrid_Zooming(IZoomable grid, Vector3 scaling, Vector3 translation);
    protected abstract void TaskGrid_ZoomingStopped(IZoomable grid);
  }
}