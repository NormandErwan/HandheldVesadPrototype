using NormandErwan.MasterThesis.Experiment.Experiment.States;
using NormandErwan.MasterThesis.Experiment.Experiment.Task;
using NormandErwan.MasterThesis.Experiment.Experiment.Variables;
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
    public event Action<TaskGrid.InteractionMode> SetTaskGridModeSync = delegate { };

    // Variables

    protected IVTechnique technique;

    // MonoBehaviour methods

    protected virtual void Awake()
    {
      StateController.CurrentStateUpdated += StateController_CurrentStateUpdated;

      TaskGrid.Configured += TaskGrid_Configured;
      TaskGrid.Completed += TaskGrid_Completed;
    }

    protected virtual void Start()
    {
      technique = StateController.GetIndependentVariable<IVTechnique>();

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
        SetTaskGridModeSync(TaskGrid.InteractionMode.Zoom);
      }
      if (Input.GetKeyUp(KeyCode.X))
      {
        SetTaskGridModeSync(TaskGrid.InteractionMode.Pan);
      }
      if (Input.GetKeyUp(KeyCode.C))
      {
        SetTaskGridModeSync(TaskGrid.InteractionMode.Select);
      }
      if (Input.GetKeyUp(KeyCode.V))
      {
        SetTaskGridModeSync(TaskGrid.InteractionMode.Zoom | TaskGrid.InteractionMode.Pan | TaskGrid.InteractionMode.Select);
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
      if (technique.CurrentCondition.useTouchInput)
      {
        OnSetTaskGridModeSync(TaskGrid.InteractionMode.Select | TaskGrid.InteractionMode.Pan | TaskGrid.InteractionMode.Zoom);
      }
      else if (technique.CurrentCondition.useLeapInput)
      {
        OnSetTaskGridModeSync(TaskGrid.InteractionMode.Select);
      }
    }

    protected virtual void StateController_CurrentStateUpdated(State currentState)
    {
      // Clear the grid if it changed state without being completed
      if (TaskGrid.IsConfigured && !TaskGrid.IsCompleted)
      {
        TaskGrid.SetCompleted();
      }
    }

    protected virtual void TaskGrid_Configured()
    {
    }

    protected virtual void TaskGrid_Completed()
    {
      TaskGrid.Show(false);
      if (CursorsInput != null)
      {
        CursorsInput.DeactivateCursors();
        CursorsInput.enabled = false;
      }
    }

    protected virtual void OnActivateTaskSync()
    {
      ActivateTaskSync();
    }

    protected virtual void OnConfigureExperimentSync()
    {
      ConfigureSync();
    }

    protected virtual void OnSetTaskGridModeSync(TaskGrid.InteractionMode interactionMode)
    {
      SetTaskGridModeSync(interactionMode);
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
      TaskGrid.Configure();
    }
  }
}
