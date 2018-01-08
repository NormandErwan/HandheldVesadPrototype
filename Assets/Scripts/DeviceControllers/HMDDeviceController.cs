using NormandErwan.MasterThesis.Experiment.Experiment.States;
using NormandErwan.MasterThesis.Experiment.Inputs;
using NormandErwan.MasterThesis.Experiment.UI.HUD;
using UnityEngine;
using System.Collections;

namespace NormandErwan.MasterThesis.Experiment.DeviceControllers
{
  public class HMDDeviceController : DeviceController
  {
    // Editor fields

    [SerializeField]
    private HMDDeviceHUD hmdDeviceHUD;

    [SerializeField]
    private GameObject leftLeapMotionHand;

    [SerializeField]
    private GameObject leftHandCursors;

    [SerializeField]
    private GameObject rightLeapMotionHand;

    [SerializeField]
    private GameObject rightHandCursors;

    [SerializeField]
    private LeapFingerCursorsInput leapFingerCursorsInput;

    [SerializeField]
    [Range(0f, 0.05f)]
    private float maxSelectableDistance = 0.001f;

    // Properties

    public HMDDeviceHUD HMDDeviceHUD { get { return hmdDeviceHUD; } set { hmdDeviceHUD = value; } }
    public LeapFingerCursorsInput LeapFingerCursorsInput { get { return leapFingerCursorsInput; } set { leapFingerCursorsInput = value; } }

    // Methods

    protected override void Start()
    {
      base.Start();

      LeapFingerCursorsInput.Configure(maxSelectableDistance);
      LeapFingerCursorsInput.gameObject.SetActive(false);
      ActivateHand(true, false);
      ActivateHand(false, false);

      // TODO: remove, for debug testing only
      //StartCoroutine(StartTaskDebug());
    }

    // DeviceController methods

    public override void ActivateTask()
    {
      base.ActivateTask();
      HMDDeviceHUD.ShowContent(false);
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
      LeapFingerCursorsInput.gameObject.SetActive(currentState.ActivateTask && useLeapInput);
      ActivateHand(ParticipantIsRightHanded, currentState.ActivateTask && useLeapInput);

      // TODO: deactivate zoom mode

      HMDDeviceHUD.ShowContent(true);
      HMDDeviceHUD.UpdateInstructionsProgress(StateController);
    }

    // Methods

    protected virtual void ActivateHand(bool rightHand, bool value)
    {
      if (rightHand)
      {
        rightLeapMotionHand.SetActive(value);
        rightHandCursors.SetActive(value);
      }
      else
      {
        leftLeapMotionHand.SetActive(value);
        leftHandCursors.SetActive(value);
      }
    }
  }
}