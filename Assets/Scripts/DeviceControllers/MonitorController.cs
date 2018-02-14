using NormandErwan.MasterThesis.Experiment.Experiment.States;
using NormandErwan.MasterThesis.Experiment.UI.HUD;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.DeviceControllers
{
  public class MonitorController : DeviceController
  {
    // Editor fields

    [SerializeField]
    private MonitorHUD monitorHUD;

    // MonoBehaviour methods

    protected override void Start()
    {
      base.Start();

      monitorHUD.BeginExperimentButtonPressed += monitorHUD_BeginExperimentButtonPressed;
      monitorHUD.NextStateButtonPressed += monitorHUD_NextStateButtonPressed;

      // TODO: remove, for debug testing only
      //StartCoroutine(StartTaskDebug());
    }

    protected override void OnDestroy()
    {
      base.OnDestroy();

      monitorHUD.BeginExperimentButtonPressed -= monitorHUD_BeginExperimentButtonPressed;
      monitorHUD.NextStateButtonPressed -= monitorHUD_NextStateButtonPressed;
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
      monitorHUD.UpdateInstructionsProgress(StateController);
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

    protected virtual void monitorHUD_BeginExperimentButtonPressed()
    {
      monitorHUD.DeactivateExperimentConfiguration();

      // Configure (sync) the experiment
      ParticipantId = monitorHUD.ParticipantId;
      ConditionsOrdering = monitorHUD.ConditionsOrdering;
      ParticipantIsRightHanded = monitorHUD.ParticipantIsRightHanded;
      OnConfigureExperimentSync();

      // Set the ordering in conditions (sync)
      if (monitorHUD.ConditionsOrdering == 1)
      {
        var mainIndVar = StateController.independentVariables[0];
        mainIndVar.NextCondition();
      }
      else if (monitorHUD.ConditionsOrdering == 2)
      {
        var mainIndVar = StateController.independentVariables[0];
        mainIndVar.NextCondition();
        mainIndVar.NextCondition();
      }

      // Begin (sync) the experiment
      StateController.BeginExperiment();
    }

    protected virtual void monitorHUD_NextStateButtonPressed()
    {
      StateController.NextState();
    }
  }
}