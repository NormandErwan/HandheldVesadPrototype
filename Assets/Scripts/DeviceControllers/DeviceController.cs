using NormandErwan.MasterThesis.Experiment.Experiment.States;
using NormandErwan.MasterThesis.Experiment.Experiment.Variables;
using NormandErwan.MasterThesis.Experiment.Loggers;
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

    // Properties

    public StateController StateController { get { return stateController; } set { stateController = value; } }
    public Experiment.Task.Grid Grid { get { return grid; } set { grid = value; } }

    public int ParticipantId { get; protected set; }
    public int ConditionsOrdering { get; protected set; }
    public bool ParticipantIsRightHanded { get; protected set; }

    // Events

    public event Action ActivateTaskSync = delegate { };
    public event Action ConfigureExperimentSync = delegate { };
    public event Action<bool> ToogleZoomSync = delegate { };

    // Variables

    protected IVTechnique ivTechnique;
    protected IVClassificationDifficulty ivClassificationDifficulty;
    protected IVTextSize ivTextSize;

    // MonoBehaviour methods

    /// <summary>
    /// Deactivates <see cref="Grid"/>.
    /// </summary>
    protected virtual void Start()
    {
      ivTechnique = StateController.GetIndependentVariable<IVTechnique>();
      ivClassificationDifficulty = StateController.GetIndependentVariable<IVClassificationDifficulty>();
      ivTextSize = StateController.GetIndependentVariable<IVTextSize>();

      Grid.StateController = StateController;
      Grid.gameObject.SetActive(false);

      StateController.CurrentStateUpdated += StateController_CurrentStateUpdated;

      Grid.Completed += Grid_Completed;
      Grid.Configured += Grid_Configured;
    }

    protected virtual void OnDestroy()
    {
      StateController.CurrentStateUpdated -= StateController_CurrentStateUpdated;

      Grid.Completed -= Grid_Completed;
      Grid.Configured -= Grid_Configured;
    }

    // Methods

    public virtual void ConfigureExperiment(int participantId, int conditionsOrdering, bool participantIsRightHanded)
    {
      ParticipantId = participantId;
      ConditionsOrdering = conditionsOrdering;
      ParticipantIsRightHanded = participantIsRightHanded;
    }

    public virtual void ActivateTask()
    {
    }

    public virtual void ToggleZoom(bool activated)
    {
    }

    protected virtual void StateController_CurrentStateUpdated(State currentState)
    {
      Grid.gameObject.SetActive(false);
    }

    protected virtual void Grid_Configured()
    {
      Grid.gameObject.SetActive(true);
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
    /// Calls <see cref="ConfigureExperimentSync"/>.
    /// </summary>
    protected virtual void OnConfigureExperimentSync()
    {
      ConfigureExperimentSync();
    }

    /// <summary>
    /// Calls <see cref="ToogleZoomSync"/>.
    /// </summary>
    protected virtual void OnToogleZoomModeSync(bool zoomModeActivated)
    {
      ToogleZoomSync(zoomModeActivated);
    }
  }
}
