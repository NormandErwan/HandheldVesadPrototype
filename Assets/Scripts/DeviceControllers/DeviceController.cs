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

    [SerializeField]
    private ParticipantLogger participantLogger;

    // Properties

    public StateController StateController { get { return stateController; } set { stateController = value; } }
    public Experiment.Task.Grid Grid { get { return grid; } set { grid = value; } }

    public int ParticipantId { get; protected set; }
    public int ConditionsOrdering { get; protected set; }
    public bool ParticipantIsRightHanded { get; protected set; }

    public ParticipantLogger ParticipantLogger { get { return participantLogger; } set { participantLogger = value; } }

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
      // init vars
      ivTechnique = StateController.GetIndependentVariable<IVTechnique>();
      ivClassificationDifficulty = StateController.GetIndependentVariable<IVClassificationDifficulty>();
      ivTextSize = StateController.GetIndependentVariable<IVTextSize>();

      // subscribe
      StateController.CurrentStateUpdated += StateController_CurrentStateUpdated;
      Grid.Finished += Grid_Finished;

      // deactivate the grid
      Grid.gameObject.SetActive(false);
    }

    protected virtual void OnDestroy()
    {
      // unsubscribes
      StateController.CurrentStateUpdated -= StateController_CurrentStateUpdated;
      Grid.Finished -= Grid_Finished;
    }

    // Methods

    public virtual void ConfigureExperiment(int participantId, int conditionsOrdering, bool participantIsRightHanded)
    {
      ParticipantId = participantId;
      ConditionsOrdering = conditionsOrdering;
      ParticipantIsRightHanded = participantIsRightHanded;

      ParticipantLogger.ParticipantId = ParticipantId;
      ParticipantLogger.StartLogger();
    }

    /// <summary>
    /// Configures and activates <see cref="Grid"/>.
    /// </summary>
    public virtual void ActivateTask()
    {
      ParticipantLogger.Technique = ivTechnique.CurrentCondition.id;
      ParticipantLogger.TextSize = ivTextSize.CurrentCondition.id;
      ParticipantLogger.ClassificationDistance = ivClassificationDifficulty.CurrentCondition.id;
      ParticipantLogger.TrialNumber = StateController.CurrentTrial;
      ParticipantLogger.PrepareNextRow();

      Grid.Configure(StateController);
      Grid.gameObject.SetActive(true);
    }

    public virtual void ToggleZoom(bool activated)
    {
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
      ParticipantLogger.WriteRow();

      stateController.NextState();
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
