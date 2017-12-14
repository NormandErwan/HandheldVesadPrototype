using DevicesSyncUnity;
using NormandErwan.MasterThesis.Experiment.Experiment.States;
using NormandErwan.MasterThesis.Experiment.Experiment.Variables;
using NormandErwan.MasterThesis.Experiment.Inputs;
using NormandErwan.MasterThesis.Experiment.UI.HUD;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.DeviceControllers
{
  public class HMDDeviceController : DeviceController
  {
    // Editor fields

    [SerializeField]
    private HMDDeviceHUD hmdDeviceHUD;

    [SerializeField]
    private GameObject LeftLeapMotionHand;

    [SerializeField]
    private GameObject LeftHandCursors;

    [SerializeField]
    private GameObject RightLeapMotionHand;

    [SerializeField]
    private GameObject RightHandCursors;

    [SerializeField]
    private LeapFingerCursorsInput leapFingerCursorsInput;

    [SerializeField]
    [Range(0f, 0.05f)]
    private float maxSelectableDistance = 0.001f;

    // Methods

    protected override void Start()
    {
      base.Start();

      ParticipantLogger.DeviceControllerName = "hmd";

      leapFingerCursorsInput.Configure(maxSelectableDistance);
      leapFingerCursorsInput.gameObject.SetActive(false);
      ActivateHand(true, false);
      ActivateHand(false, false);

      // TODO: remove, for debug testing only
      /*ActivateHand(true, true);
      StateController.BeginExperiment();
      ActivateTask();*/
    }

    public override void ActivateTask()
    {
      base.ActivateTask();
      hmdDeviceHUD.ShowContent(false);
    }

    public override void ToggleZoom(bool activated)
    {
      base.ToggleZoom(activated);
      // TODO
    }

    protected override void StateController_CurrentStateUpdated(State currentState)
    {
      base.StateController_CurrentStateUpdated(currentState);

      bool useLeapInput = ivTechnique.CurrentCondition.useLeapInput;
      leapFingerCursorsInput.gameObject.SetActive(currentState.ActivateTask && useLeapInput);
      ActivateHand(ParticipantIsRightHanded, currentState.ActivateTask && useLeapInput);

      // TODO: deactivate zoom mode

      hmdDeviceHUD.ShowContent(true);
      hmdDeviceHUD.UpdateInstructionsProgress(StateController);
    }

    protected virtual void ActivateHand(bool rightHand, bool value)
    {
      if (rightHand)
      {
        RightLeapMotionHand.SetActive(value);
        RightHandCursors.SetActive(value);
      }
      else
      {
        LeftLeapMotionHand.SetActive(value);
        LeftHandCursors.SetActive(value);
      }
    }
  }
}