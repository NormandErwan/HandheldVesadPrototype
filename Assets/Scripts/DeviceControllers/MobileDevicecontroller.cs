using NormandErwan.MasterThesis.Experiment.Experiment.States;
using NormandErwan.MasterThesis.Experiment.UI.HUD;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.DeviceControllers
{
  public class MobileDeviceController : DeviceController
  {
    // Editor fields

    [SerializeField]
    private MobileDeviceHUD mobileDeviceHUD;

    // Methods

    protected override void OnEnable()
    {
      base.OnEnable();
      mobileDeviceHUD.ValidateButtonPressed += MobileDeviceHUD_ValidateButtonPressed;
    }

    protected override void OnDisable()
    {
      base.OnDisable();
      mobileDeviceHUD.ValidateButtonPressed -= MobileDeviceHUD_ValidateButtonPressed;
    }

    protected override void Start()
    {
      base.Start();

      // TODO: remove, for debug testing only
      /*StateController.BeginExperiment();
      ActivateTask();*/
    }

    protected override void StateController_CurrentStateUpdated(State currentState)
    {
      base.StateController_CurrentStateUpdated(currentState);
      mobileDeviceHUD.ShowValidateButton(true);
    }

    protected virtual void MobileDeviceHUD_ValidateButtonPressed()
    {
      mobileDeviceHUD.ShowValidateButton(false);
      if (StateController.CurrentState.ActivateTask)
      {
        OnRequestActivateTask();
      }
      else
      {
        StateController.NextState();
      }
    }
  }
}