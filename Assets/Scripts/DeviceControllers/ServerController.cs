using NormandErwan.MasterThesis.Experiment.Experiment.States;
using NormandErwan.MasterThesis.Experiment.UI.HUD;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.DeviceControllers
{
  public class ServerController : DeviceController
  {
    // Editor fields

    [SerializeField]
    private ServerHUD serverHUD;

    // Methods

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

    protected override void StateController_CurrentStateUpdated(State currentState)
    {
      base.StateController_CurrentStateUpdated(currentState);
      serverHUD.UpdateInstructionsProgress(StateController);
    }

    protected virtual void ServerHUD_RequestBeginExperiment()
    {
      serverHUD.DeactivateExperimentConfiguration();

      ParticipantIsRightHanded = serverHUD.ParticipantIsRightHanded;
      OnConfigureExperiment();

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

      StateController.BeginExperiment();
    }

    protected virtual void ServerHUD_RequestNextState()
    {
      StateController.NextState();
    }
  }
}