using NormandErwan.MasterThesis.Experiment.Experiment.States;
using NormandErwan.MasterThesis.Experiment.Experiment.Variables;
using NormandErwan.MasterThesis.Experiment.Inputs;
using NormandErwan.MasterThesis.Experiment.Loggers;
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
    private Inputs.Cursor rightIndexCursor;

    [SerializeField]
    private Inputs.Cursor leftIndexCursor;

    [SerializeField]
    private bool activateLeapHands;

    [SerializeField]
    private LeapFingerCursorsInput leapFingerCursorsInput;

    [SerializeField]
    private Renderer rightLeapHand;

    [SerializeField]
    private Renderer leftLeapHand;

    [SerializeField]
    [Range(0f, 0.05f)]
    private float maxSelectableDistance = 0.001f;

    [SerializeField]
    private ExperimentDetailsLogger experimentDetailsLogger;

    // Properties

    public HMDDeviceHUD HMDDeviceHUD { get { return hmdDeviceHUD; } set { hmdDeviceHUD = value; } }
    public LeapFingerCursorsInput LeapFingerCursorsInput { get { return leapFingerCursorsInput; } set { leapFingerCursorsInput = value; } }

    // Variables

    IVTechnique technique;

    // Methods

    protected override void Start()
    {
      base.Start();

      technique = StateController.GetIndependentVariable<IVTechnique>();

      LeapFingerCursorsInput.Configure(maxSelectableDistance);
      LeapFingerCursorsInput.enabled = false;
      ActivateHand(true, false);
      ActivateHand(false, false);
      ParticipantIsRightHanded = true;

      // TODO: remove, for debug testing only
      //StartCoroutine(StartTaskDebug());
    }

    // DeviceController methods

    public override void ActivateTask()
    {
      base.ActivateTask();

      if (technique.CurrentCondition.useLeapInput)
      {
        LeapFingerCursorsInput.enabled = true;
        ActivateHand(ParticipantIsRightHanded, true);

        experimentDetailsLogger.Index = (ParticipantIsRightHanded) ? rightIndexCursor : leftIndexCursor;
      }

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

      LeapFingerCursorsInput.enabled = false;
      ActivateHand(ParticipantIsRightHanded, false);

      // TODO: deactivate zoom mode

      HMDDeviceHUD.ShowContent(true);
      HMDDeviceHUD.UpdateInstructionsProgress(StateController);
    }

    // Methods

    protected virtual void ActivateHand(bool rightHand, bool value)
    {
      if (rightHand)
      {
        rightLeapHand.gameObject.SetActive(activateLeapHands & value);
        rightIndexCursor.gameObject.SetActive(value);
      }
      else
      {
        leftLeapHand.gameObject.SetActive(activateLeapHands & value);
        leftIndexCursor.gameObject.SetActive(value);
      }
    }
  }
}