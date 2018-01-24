using NormandErwan.MasterThesis.Experiment.Experiment.States;
using NormandErwan.MasterThesis.Experiment.Inputs;
using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
using NormandErwan.MasterThesis.Experiment.Loggers;
using System;
using System.Collections;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.DeviceControllers
{
  public abstract class DeviceController : MonoBehaviour
  {
    // Editor fields

    [SerializeField]
    private StateController stateController;

    [SerializeField]
    private ExperimentDetailsLogger experimentDetailsLogger;

    [SerializeField]
    private ProjectedCursorsSync projectedCursorsSync;

    [SerializeField]
    private Experiment.Task.Grid grid;

    // Properties

    public StateController StateController { get { return stateController; } set { stateController = value; } }
    public Experiment.Task.Grid Grid { get { return grid; } set { grid = value; } }

    public int ParticipantId { get; protected set; }
    public int ConditionsOrdering { get; protected set; }
    public bool ParticipantIsRightHanded { get; protected set; }

    protected virtual CursorsInput CursorsInput { get; }

    // Events

    public event Action ConfigureSync = delegate { };
    public event Action Configured = delegate { };
    public event Action ActivateTaskSync = delegate { };
    public event Action<bool> SetDragToZoomSync = delegate { };

    // MonoBehaviour methods

    protected virtual void Awake()
    {
      StateController.CurrentStateUpdated += StateController_CurrentStateUpdated;

      Grid.Configured += Grid_Configured;
      Grid.Completed += Grid_Completed;
    }

    protected virtual void Start()
    {
      Grid.Show(false);
    }

    // TODO: remove, for debug testing only
    protected virtual void Update()
    {
      if (Input.GetKeyUp(KeyCode.B))
      {
        OnConfigureExperimentSync();
        StateController.BeginExperiment();
      }
      if (Input.GetKeyUp(KeyCode.N))
      {
        if (Grid.IsConfigured)
        {
          Grid.SetCompleted();
        }
        StateController.NextState();
      }
      if (Input.GetKeyUp(KeyCode.A))
      {
        OnActivateTaskSync();
      }
      if (Input.GetKeyUp(KeyCode.C))
      {
        Grid.Configure();
      }
      if (Input.GetKeyUp(KeyCode.Space))
      {
        if (Grid.IsConfigured)
        {
          Grid.SetCompleted();
        }
        StateController.NextState();
        OnActivateTaskSync();
        Grid.Configure();
      }

      if (Input.GetKeyUp(KeyCode.Z))
      {
        OnSetDragToZoomSync(!Grid.DragToZoom);
      }
    }

    protected virtual void OnDestroy()
    {
      StateController.CurrentStateUpdated -= StateController_CurrentStateUpdated;

      Grid.Configured -= Grid_Configured;
      Grid.Completed -= Grid_Completed;
    }

    // Methods

    public virtual void Configure(int participantId, int conditionsOrdering, bool participantIsRightHanded)
    {
      ParticipantId = participantId;
      ConditionsOrdering = conditionsOrdering;
      ParticipantIsRightHanded = participantIsRightHanded;

      if (CursorsInput != null)
      {
        experimentDetailsLogger.Index = (ParticipantIsRightHanded) ? CursorsInput.Cursors[CursorType.RightIndex] : CursorsInput.Cursors[CursorType.LeftIndex];
        if (ParticipantIsRightHanded)
        {
          experimentDetailsLogger.ProjectedIndex = projectedCursorsSync.ProjectedCursors[CursorType.RightIndex];
          experimentDetailsLogger.ProjectedThumb = projectedCursorsSync.ProjectedCursors[CursorType.RightThumb];
        }
        else
        {
          experimentDetailsLogger.ProjectedIndex = projectedCursorsSync.ProjectedCursors[CursorType.LeftIndex];
          experimentDetailsLogger.ProjectedThumb = projectedCursorsSync.ProjectedCursors[CursorType.LeftThumb];
        }
      }

      Configured();
    }

    public virtual void ActivateTask()
    {
    }

    public virtual void SetDragToZoom(bool activated)
    {
      Grid.DragToZoom = activated;
    }

    protected virtual void StateController_CurrentStateUpdated(State currentState)
    {
      Grid.Show(false);
      SetDragToZoom(false);
    }

    protected virtual void Grid_Configured()
    {
      Grid.Show(true);
    }

    protected virtual void Grid_Completed()
    {
    }

    /// <summary>
    /// Calls <see cref="ActivateTaskSync"/>.
    /// </summary>
    protected virtual void OnActivateTaskSync()
    {
      ActivateTaskSync();
    }

    /// <summary>
    /// Calls <see cref="ConfigureSync"/>.
    /// </summary>
    protected virtual void OnConfigureExperimentSync()
    {
      ConfigureSync();
    }

    /// <summary>
    /// Calls <see cref="SetDragToZoomSync"/>.
    /// </summary>
    protected virtual void OnSetDragToZoomSync(bool activated)
    {
      SetDragToZoomSync(activated);
    }

    // TODO: remove, for debug testing only
    protected virtual IEnumerator StartTaskDebug()
    {
      yield return null;
      OnConfigureExperimentSync();
      StateController.BeginExperiment();

      yield return null;
      StateController.NextState();

      yield return null;
      OnActivateTaskSync();
    }
  }
}
