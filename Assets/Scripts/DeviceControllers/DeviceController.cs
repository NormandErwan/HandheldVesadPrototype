using NormandErwan.MasterThesis.Experiment.Experiment.States;
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
    private Experiment.Task.Grid grid;

    // Properties

    public StateController StateController { get { return stateController; } set { stateController = value; } }
    public Experiment.Task.Grid Grid { get { return grid; } set { grid = value; } }

    public int ParticipantId { get; protected set; }
    public int ConditionsOrdering { get; protected set; }
    public bool ParticipantIsRightHanded { get; protected set; }

    // Events

    public event Action ConfigureSync = delegate { };
    public event Action Configured = delegate { };
    public event Action ActivateTaskSync = delegate { };
    public event Action<bool> ToogleZoomSync = delegate { };

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
      Configured();
    }

    public virtual void ActivateTask()
    {
      Grid.Configure();
    }

    public virtual void ToggleZoom(bool activated)
    {
    }

    protected virtual void StateController_CurrentStateUpdated(State currentState)
    {
      Grid.Show(false);
    }

    protected virtual void Grid_Configured()
    {
      Grid.Show(true);
    }

    protected virtual void Grid_Completed()
    {
      StateController.NextState();
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
    /// Calls <see cref="ToogleZoomSync"/>.
    /// </summary>
    protected virtual void OnToogleZoomModeSync(bool zoomModeActivated)
    {
      ToogleZoomSync(zoomModeActivated);
    }

    protected virtual IEnumerator StartTaskDebug()
    {
      yield return null;
      OnConfigureExperimentSync();
      StateController.BeginExperiment();

      yield return null;
      StateController.NextState();

      yield return null;
      ActivateTask();
    }

    private void Update()
    {
      if (Input.GetKeyUp(KeyCode.C))
      {
        Grid.SetCompleted();
      }
    }
  }
}
