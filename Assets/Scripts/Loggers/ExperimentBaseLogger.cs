using NormandErwan.MasterThesis.Experiment.DeviceControllers;
using NormandErwan.MasterThesis.Experiment.Experiment;
using NormandErwan.MasterThesis.Experiment.Experiment.States;
using NormandErwan.MasterThesis.Experiment.Experiment.Task;
using NormandErwan.MasterThesis.Experiment.Experiment.Variables;
using NormandErwan.MasterThesis.Experiment.Inputs.Cursors;
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
    protected MobileDeviceTracking mobileDevice;

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

    protected FingerCursor Index { get; set; }
    protected ProjectedCursor ProjectedIndex { get; set; }

    protected FingerCursor Thumb { get; set; }
    protected ProjectedCursor ProjectedThumb { get; set; }

    // Variables

    protected StateController stateController;
    protected TaskGrid taskGrid;

    protected IVTechnique technique;
    protected IVTextSize textSize;
    protected IVClassificationDifficulty distance;

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

    protected virtual void Start()
    {
      technique = stateController.GetIndependentVariable<IVTechnique>();
      textSize = stateController.GetIndependentVariable<IVTextSize>();
      distance = stateController.GetIndependentVariable<IVClassificationDifficulty>();
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
        Index = deviceController.FingerCursorsInput.Cursors[CursorType.RightIndex];
        Thumb = deviceController.FingerCursorsInput.Cursors[CursorType.RightThumb];

        ProjectedIndex = projectedRightIndex;
        ProjectedThumb = projectedRightThumb;
      }
      else
      {
        Index = deviceController.FingerCursorsInput.Cursors[CursorType.LeftIndex];
        Thumb = deviceController.FingerCursorsInput.Cursors[CursorType.LeftThumb];

        ProjectedIndex = projectedLeftIndex;
        ProjectedThumb = projectedLeftThumb;
      }

      Configure();
    }

    protected virtual void TaskGrid_Configured() { }
    protected virtual void TaskGrid_Completed() { }

    protected virtual void TaskGrid_ItemSelected(Container container, Item item) { }
    protected virtual void TaskGrid_ItemMoved(Container oldContainer, Container newContainer, Item item, TaskGrid.ItemMovedType moveType) { }

    protected virtual void TaskGrid_DraggingStarted(IDraggable grid) { }
    protected virtual void TaskGrid_Dragging(IDraggable grid, Vector3 translation) { }
    protected virtual void TaskGrid_DraggingStopped(IDraggable grid) { }

    protected virtual void TaskGrid_ZoomingStarted(IZoomable grid) { }
    protected virtual void TaskGrid_Zooming(IZoomable grid, Vector3 scaling, Vector3 translation) { }
    protected virtual void TaskGrid_ZoomingStopped(IZoomable grid) { }
  }
}