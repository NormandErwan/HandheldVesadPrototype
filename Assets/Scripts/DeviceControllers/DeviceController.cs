using NormandErwan.MasterThesis.Experiment.Experiment.States;
using NormandErwan.MasterThesis.Experiment.Experiment.Task;
using NormandErwan.MasterThesis.Experiment.Inputs;
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
    private TaskGrid taskGrid;

    // Properties

    public StateController StateController { get { return stateController; } set { stateController = value; } }
    public TaskGrid TaskGrid { get { return taskGrid; } set { taskGrid = value; } }
    public virtual CursorsInput CursorsInput { get { return null; } }

    public int ParticipantId { get; protected set; }
    public int ConditionsOrdering { get; protected set; }
    public bool ParticipantIsRightHanded { get; protected set; }

    // Events

    public event Action ConfigureSync = delegate { };
    public event Action Configured = delegate { };
    public event Action ActivateTaskSync = delegate { };
    public event Action<bool> SetDragToZoomSync = delegate { };

    // MonoBehaviour methods

    protected virtual void Awake()
    {
      StateController.CurrentStateUpdated += StateController_CurrentStateUpdated;

      TaskGrid.Configured += TaskGrid_Configured;
      TaskGrid.Completed += TaskGrid_Completed;
    }

    protected virtual void Start()
    {
      TaskGrid.Show(false);
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
        if (TaskGrid.IsConfigured)
        {
          TaskGrid.SetCompleted();
        }
        StateController.NextState();
      }
      if (Input.GetKeyUp(KeyCode.A))
      {
        OnActivateTaskSync();
      }
      if (Input.GetKeyUp(KeyCode.C))
      {
        TaskGrid.Configure();
      }
      if (Input.GetKeyUp(KeyCode.Space))
      {
        if (TaskGrid.IsConfigured)
        {
          TaskGrid.SetCompleted();
        }
        StateController.NextState();
        OnActivateTaskSync();
        TaskGrid.Configure();
      }

      if (Input.GetKeyUp(KeyCode.Z))
      {
        OnSetDragToZoomSync(!TaskGrid.DragToZoom);
      }
    }

    protected virtual void OnDestroy()
    {
      StateController.CurrentStateUpdated -= StateController_CurrentStateUpdated;

      TaskGrid.Configured -= TaskGrid_Configured;
      TaskGrid.Completed -= TaskGrid_Completed;
    }

    // Methods

    public virtual void Configure(int participantId, int conditionsOrdering, bool participantIsRightHanded)
    {
      ParticipantId = participantId;
      ConditionsOrdering = conditionsOrdering;
      ParticipantIsRightHanded = participantIsRightHanded;
      Configured();
    }

    public virtual void ActivateTask()
    {
    }

    public virtual void SetDragToZoom(bool activated)
    {
      TaskGrid.DragToZoom = activated;
    }

    protected virtual void StateController_CurrentStateUpdated(State currentState)
    {
      TaskGrid.Show(false);
      SetDragToZoom(false);
    }

    protected virtual void TaskGrid_Configured()
    {
      TaskGrid.Show(true);
    }

    protected virtual void TaskGrid_Completed()
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
