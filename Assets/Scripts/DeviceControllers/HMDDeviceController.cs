using NormandErwan.MasterThesis.Experiment.Experiment.States;
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

    // Methods

    public override void ActivateTask()
    {
      base.ActivateTask();
      hmdDeviceHUD.ShowContent(false);
    }

    public override void SetParticipantIsRightHanded(bool value)
    {
      base.SetParticipantIsRightHanded(value);

      LeftLeapMotionHand.SetActive(!ParticipantIsRightHanded);
      LeftHandCursors.SetActive(!ParticipantIsRightHanded);
      RightLeapMotionHand.SetActive(ParticipantIsRightHanded);
      RightHandCursors.SetActive(ParticipantIsRightHanded);
    }

    protected override void Start()
    {
      base.Start();
      LeftLeapMotionHand.SetActive(false);
      LeftHandCursors.SetActive(false);
      RightLeapMotionHand.SetActive(false);
      RightHandCursors.SetActive(false);
    }

    protected override void StateController_CurrentStateUpdated(State currentState)
    {
      base.StateController_CurrentStateUpdated(currentState);

      hmdDeviceHUD.ShowContent(true);
      hmdDeviceHUD.UpdateInstructionsProgress(StateController);
    }
  }
}