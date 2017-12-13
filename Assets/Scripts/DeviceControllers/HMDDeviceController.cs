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

    // Methods

    public override void ActivateTask()
    {
      base.ActivateTask();
      hmdDeviceHUD.ShowContent(false);
    }

    protected override void Start()
    {
      base.Start();

      leapFingerCursorsInput.gameObject.SetActive(false);
      ActivateHand(true, false);
      ActivateHand(false, false);

      // TODO: remove, for debug testing only
      ActivateHand(true, true);
      StateController.BeginExperiment();
      ActivateTask();
    }

    protected override void StateController_CurrentStateUpdated(State currentState)
    {
      base.StateController_CurrentStateUpdated(currentState);

      var indVarTechnique = StateController.GetIndependentVariable<IVTechnique>();
      bool useLeapInput = indVarTechnique.CurrentCondition.useLeapInput;

      leapFingerCursorsInput.gameObject.SetActive(useLeapInput);
      ActivateHand(ParticipantIsRightHanded, currentState.ActivateTask && useLeapInput);

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