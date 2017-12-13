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
    [Range(0f, 0.05f)]
    private float maxSelectableDistance = 0.001f;

    [SerializeField]
    private ParticipantLogger participantLogger;

    // Properties

    public StateController StateController { get { return stateController; } set { stateController = value; } }
    public Experiment.Task.Grid Grid { get { return grid; } set { grid = value; } }
    public float MaxSelectableDistance { get { return maxSelectableDistance; } set { maxSelectableDistance = value; } }

    public int ParticipantId { get; protected set; }
    public int ConditionsOrdering { get; protected set; }
    public bool ParticipantIsRightHanded { get; protected set; }

    public ParticipantLogger ParticipantLogger { get { return participantLogger; } set { participantLogger = value; } }

    // Events

    public event Action ActivateTaskSync = delegate { };
    public event Action ConfigureExperimentSync = delegate { };

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
      print("activate task");
      ActivateTaskSync();
    }

    /// <summary>
    /// Calls <see cref="ConfigureExperimentSync"/>.
    /// </summary>
    protected virtual void OnConfigureExperimentSync()
    {
      ConfigureExperimentSync();
    }
  }
}
