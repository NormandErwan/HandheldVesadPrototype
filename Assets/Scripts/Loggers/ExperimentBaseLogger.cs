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
    protected ProjectedCursor projectedRightIndex;

    [SerializeField]
    protected ProjectedCursor projectedLeftIndex;

    [SerializeField]
    protected ProjectedCursor projectedRightThumb;

    [SerializeField]
    protected ProjectedCursor projectedLeftThumb;

    // Properties

    protected Inputs.Cursor Index { get; set; }
    protected ProjectedCursor ProjectedIndex { get; set; }

    protected Inputs.Cursor Thumb { get; set; }
    protected ProjectedCursor ProjectedThumb { get; set; }

    // Variables

    protected StateController stateController;
    protected Experiment.Task.Grid grid;

    // MonoBehaviour methods

    protected override void Awake()
    {
      base.Awake();

      dataPath += "/Logs";

      stateController = deviceController.StateController;
      grid = deviceController.Grid;

      deviceController.Configured += DeviceController_Configured;

      grid.Configured += Grid_Configured;
      grid.Completed += Grid_Completed;
      grid.ItemSelected += Grid_ItemSelected;
      grid.ItemMoved += Grid_ItemMoved;

      grid.DraggingStarted += Grid_DraggingStarted;
      grid.Dragging += Grid_Dragging;
      grid.DraggingStopped += Grid_DraggingStopped;

      grid.ZoomingStarted += Grid_ZoomingStarted;
      grid.Zooming += Grid_Zooming;
      grid.ZoomingStopped += Grid_ZoomingStopped;
    }

    protected override void OnDestroy()
    {
      base.OnDestroy();

      deviceController.Configured -= DeviceController_Configured;

      grid.Configured -= Grid_Configured;
      grid.Completed -= Grid_Completed;
      grid.ItemSelected -= Grid_ItemSelected;
      grid.ItemMoved -= Grid_ItemMoved;

      grid.DraggingStarted -= Grid_DraggingStarted;
      grid.Dragging -= Grid_Dragging;
      grid.DraggingStopped -= Grid_DraggingStopped;

      grid.ZoomingStarted -= Grid_ZoomingStarted;
      grid.Zooming -= Grid_Zooming;
      grid.ZoomingStopped -= Grid_ZoomingStopped;
    }

    // Methods

    protected virtual void DeviceController_Configured()
    {
      if (deviceController.ParticipantIsRightHanded)
      {
        ProjectedIndex = projectedRightIndex;
        ProjectedThumb = projectedRightThumb;
      }
      else
      {
        ProjectedIndex = projectedLeftIndex;
        ProjectedThumb = projectedLeftThumb;
      }

      Index = ProjectedIndex.Cursor;
      Thumb = ProjectedThumb.Cursor;

      Configure();
    }

    protected abstract void Grid_Configured();
    protected abstract void Grid_Completed();

    protected abstract void Grid_ItemSelected(Container container, Item item);
    protected abstract void Grid_ItemMoved(Container oldContainer, Container newContainer, Item item, Experiment.Task.Grid.ItemMovedType moveType);

    protected abstract void Grid_DraggingStarted(IDraggable grid);
    protected abstract void Grid_Dragging(IDraggable grid, Vector3 translation);
    protected abstract void Grid_DraggingStopped(IDraggable grid);

    protected abstract void Grid_ZoomingStarted(IZoomable grid);
    protected abstract void Grid_Zooming(IZoomable grid, Vector3 scaling, Vector3 translation);
    protected abstract void Grid_ZoomingStopped(IZoomable grid);
  }
}