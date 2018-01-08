using NormandErwan.MasterThesis.Experiment.Experiment.States;
using NormandErwan.MasterThesis.Experiment.Experiment.Variables;
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
    private Experiment.Task.Grid grid;

    [SerializeField]
    private ParticipantLogger participantLogger;

    // Properties

    public StateController StateController { get { return stateController; } set { stateController = value; } }
    public Experiment.Task.Grid Grid { get { return grid; } set { grid = value; } }
    public ParticipantLogger ParticipantLogger { get { return participantLogger; } set { participantLogger = value; } }

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
      Grid.Show(false);

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

      ParticipantLogger.ParticipantId = ParticipantId;
      ParticipantLogger.StartLogger();
    }

    public virtual void ActivateTask()
    {
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

      ParticipantLogger.PrepareNextRow();
      ParticipantLogger.Technique = ivTechnique.CurrentCondition.id;
      ParticipantLogger.TextSize = ivTextSize.CurrentCondition.id;
      ParticipantLogger.ClassificationDistance = ivClassificationDifficulty.CurrentCondition.id;
      ParticipantLogger.TrialNumber = StateController.CurrentTrial;
    }

    protected virtual void Grid_Completed()
    {
      ParticipantLogger.WriteRow();
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

    protected virtual IEnumerator StartTaskDebug()
    {
      yield return null;
      OnConfigureExperimentSync();
      StateController.BeginExperiment();

      yield return null;
      StateController.NextState();
      ActivateTask();
      Grid.Configure();
    }

    private void Update()
    {
      if (Input.GetKeyUp(KeyCode.C))
      {
        Grid_Completed();
        StateController.NextState();
        ActivateTask();
        Grid.Configure();
      }
    }
  }
}
