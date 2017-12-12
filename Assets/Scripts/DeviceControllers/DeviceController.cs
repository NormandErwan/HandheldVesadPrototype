using NormandErwan.MasterThesis.Experiment.Experiment.States;
using System;
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

    [SerializeField]
    [Range(0f, 0.05f)]
    private float maxSelectableDistance = 0.001f;

    // Properties

    public StateController StateController { get { return stateController; } set { stateController = value; } }
    public Experiment.Task.Grid Grid { get { return grid; } set { grid = value; } }
    public float MaxSelectableDistance { get { return maxSelectableDistance; } set { maxSelectableDistance = value; } }

    public bool ParticipantIsRightHanded { get; internal set; }

    // Events

    public event Action RequestActivateTask = delegate { };
    public event Action ConfigureExperiment = delegate { };

    // Methods

    /// <summary>
    /// Configures and activates <see cref="Grid"/>.
    /// </summary>
    public virtual void ActivateTask()
    {
      Grid.Configure(StateController);
      Grid.gameObject.SetActive(true);
    }

    /// <summary>
    /// Deactivates <see cref="Grid"/>.
    /// </summary>
    protected virtual void Start()
    {
      Grid.gameObject.SetActive(false);
    }

    // subscribes
    protected virtual void OnEnable()
    {
      StateController.CurrentStateUpdated += StateController_CurrentStateUpdated;
      Grid.Finished += Grid_Finished;
    }

    // unsubscribes
    protected virtual void OnDisable()
    {
      StateController.CurrentStateUpdated -= StateController_CurrentStateUpdated;
      Grid.Finished -= Grid_Finished;
    }

    /// <summary>
    /// Deactivates <see cref="Grid"/> each new state.
    /// </summary>
    protected virtual void StateController_CurrentStateUpdated(State currentState)
    {
      Grid.gameObject.SetActive(false);
    }

    /// <summary>
    /// Go to the next state when the <see cref="Grid"/> is finished.
    /// </summary>
    protected virtual void Grid_Finished()
    {
      stateController.NextState();
    }

    /// <summary>
    /// Calls <see cref="RequestActivateTask"/>.
    /// </summary>
    protected virtual void OnRequestActivateTask()
    {
      RequestActivateTask();
    }

    /// <summary>
    /// Calls <see cref="ConfigureExperiment"/>.
    /// </summary>
    protected virtual void OnConfigureExperiment()
    {
      ConfigureExperiment();
    }
  }
}
