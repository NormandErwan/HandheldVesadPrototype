using NormandErwan.MasterThesis.Experiment.Experiment.States;
using NormandErwan.MasterThesis.Experiment.Experiment.Variables;
using NormandErwan.MasterThesis.Experiment.Loggers;
using NormandErwan.MasterThesis.Experiment.UI.HUD;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.DeviceControllers
{
  public class ServerController : DeviceController
  {
    // Editor fields

    [SerializeField]
    private ServerHUD serverHUD;

    [SerializeField]
    private ParticipantLogger participantLogger;

    // Variables

    protected IVTechnique ivTechnique;
    protected IVClassificationDifficulty ivClassificationDifficulty;
    protected IVTextSize ivTextSize;

    // MonoBehaviour methods

    protected override void OnEnable()
    {
      base.OnEnable();
      serverHUD.RequestBeginExperiment += ServerHUD_RequestBeginExperiment;
      serverHUD.RequestNextState += ServerHUD_RequestNextState;
    }

    protected override void OnDisable()
    {
      base.OnDisable();
      serverHUD.RequestBeginExperiment -= ServerHUD_RequestBeginExperiment;
      serverHUD.RequestNextState -= ServerHUD_RequestNextState;
    }

    protected override void Start()
    {
      base.Start();
      ivTechnique = StateController.GetIndependentVariable<IVTechnique>();
      ivClassificationDifficulty = StateController.GetIndependentVariable<IVClassificationDifficulty>();
      ivTextSize = StateController.GetIndependentVariable<IVTextSize>();
    }

    // DeviceController methods

    public override void ActivateTask()
    {
      participantLogger.Technique = ivTechnique.CurrentCondition.id;
      participantLogger.TextSize = ivTextSize.CurrentCondition.id;
      participantLogger.ClassificationDistance = ivClassificationDifficulty.CurrentCondition.id;
      participantLogger.TrialNumber = StateController.CurrentTrial;
      participantLogger.PrepareNextRow();

      base.ActivateTask();
    }

    protected override void Grid_Finished()
    {
      participantLogger.WriteRow();
      base.Grid_Finished();
    }

    protected override void StateController_CurrentStateUpdated(State currentState)
    {
      base.StateController_CurrentStateUpdated(currentState);
      serverHUD.UpdateInstructionsProgress(StateController);
    }

    protected virtual void ServerHUD_RequestBeginExperiment()
    {
      serverHUD.DeactivateExperimentConfiguration();

      // Configure (sync) the experiment
      ParticipantIsRightHanded = serverHUD.ParticipantIsRightHanded;
      OnConfigureExperiment();

      // Set the ordering in conditions (sync)
      if (serverHUD.ConditionsOrdering == 1)
      {
        var mainIndVar = StateController.independentVariables[0];
        mainIndVar.NextCondition();
      }
      else if (serverHUD.ConditionsOrdering == 2)
      {
        var mainIndVar = StateController.independentVariables[0];
        mainIndVar.NextCondition();
        mainIndVar.NextCondition();
      }

      // Start loggers
      participantLogger.ParticipantId = serverHUD.ParticipantId;
      participantLogger.StartLogger();

      // Begin (sync) the experiment
      StateController.BeginExperiment();
    }

    protected virtual void ServerHUD_RequestNextState()
    {
      StateController.NextState();
    }
  }
}