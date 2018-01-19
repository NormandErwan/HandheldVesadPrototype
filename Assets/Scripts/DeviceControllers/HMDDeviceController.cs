using NormandErwan.MasterThesis.Experiment.Experiment.States;
using NormandErwan.MasterThesis.Experiment.Experiment.Variables;
using NormandErwan.MasterThesis.Experiment.Inputs;
using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
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
    private ExperimentDetailsLogger experimentDetailsLogger;

    [Header("Cursors")]
    [SerializeField]
    private LeapFingerCursorsInput leapFingerCursorsInput;

    [SerializeField]
    private ProjectedCursorsController projectedCursorsController;

    [SerializeField]
    [Range(0f, 0.05f)]
    private float maxSelectableDistance = 0.001f;

    [SerializeField]
    private bool activateLeapHands;

    [SerializeField]
    private Renderer rightLeapHand;

    [SerializeField]
    private Renderer leftLeapHand;

    // Variables

    protected IVTechnique technique;
    protected Inputs.Cursor rightIndexCursor, leftIndexCursor;

    // Methods

    protected override void Start()
    {
      base.Start();

      technique = StateController.GetIndependentVariable<IVTechnique>();
      rightIndexCursor = leapFingerCursorsInput.Cursors[CursorType.RightIndex];
      leftIndexCursor = leapFingerCursorsInput.Cursors[CursorType.LeftIndex];

      leapFingerCursorsInput.Configure(maxSelectableDistance);
      leapFingerCursorsInput.enabled = false;
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
        leapFingerCursorsInput.enabled = true;
        ActivateHand(ParticipantIsRightHanded, true);

        experimentDetailsLogger.Index = (ParticipantIsRightHanded) ? rightIndexCursor : leftIndexCursor;
      }

      hmdDeviceHUD.ShowContent(false);
    }

    protected override void StateController_CurrentStateUpdated(State currentState)
    {
      base.StateController_CurrentStateUpdated(currentState);

      leapFingerCursorsInput.enabled = false;
      ActivateHand(ParticipantIsRightHanded, false);

      hmdDeviceHUD.ShowContent(true);
      hmdDeviceHUD.UpdateInstructionsProgress(StateController);
    }

    // Methods

    protected virtual void ActivateHand(bool isRightHand, bool value)
    {
      if (isRightHand)
      {
        rightIndexCursor.gameObject.SetActive(value);
        rightLeapHand.gameObject.SetActive(activateLeapHands & value);
      }
      else
      {
        leftIndexCursor.gameObject.SetActive(value);
        leftLeapHand.gameObject.SetActive(activateLeapHands & value);
      }
    }
  }
}