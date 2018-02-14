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

    // MonoBehaviour methods

    protected override void Start()
    {
      base.Start();

      serverHUD.BeginExperimentButtonPressed += ServerHUD_BeginExperimentButtonPressed;
      serverHUD.NextStateButtonPressed += ServerHUD_NextStateButtonPressed;

      // TODO: remove, for debug testing only
      //StartCoroutine(StartTaskDebug());
    }

    protected override void OnDestroy()
    {
      base.OnDestroy();

      serverHUD.BeginExperimentButtonPressed -= ServerHUD_BeginExperimentButtonPressed;
      serverHUD.NextStateButtonPressed -= ServerHUD_NextStateButtonPressed;
    }

    // DeviceController methods

    public override void ActivateTask()
    {
      base.ActivateTask();
      TaskGrid.Configure();
    }

    protected override void StateController_CurrentStateUpdated(State currentState)
    {
      base.StateController_CurrentStateUpdated(currentState);
      serverHUD.UpdateInstructionsProgress(StateController);
    }

    // Methods

    protected override void TaskGrid_Completed()
    {
      base.TaskGrid_Completed();
      if (StateController.CurrentState == StateController.taskTrialState)
      {
        StateController.NextState();
      }
    }

    protected virtual void ServerHUD_BeginExperimentButtonPressed()
    {
      serverHUD.DeactivateExperimentConfiguration();

      // Configure (sync) the experiment
      ParticipantId = serverHUD.ParticipantId;
      ConditionsOrdering = serverHUD.ConditionsOrdering;
      ParticipantIsRightHanded = serverHUD.ParticipantIsRightHanded;
      OnConfigureExperimentSync();

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

      // Begin (sync) the experiment
      StateController.BeginExperiment();
    }

    protected virtual void ServerHUD_NextStateButtonPressed()
    {
      StateController.NextState();
    }
  }
}