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
    private ProjectedCursorsSync projectedCursorsSync;

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

      // TODO: remove, for debug testing only
      //ParticipantIsRightHanded = true;
      //StartCoroutine(StartTaskDebug());
    }

    // DeviceController methods

    public override void Configure(int participantId, int conditionsOrdering, bool participantIsRightHanded)
    {
      base.Configure(participantId, conditionsOrdering, participantIsRightHanded);

      experimentDetailsLogger.Index = (ParticipantIsRightHanded) ? rightIndexCursor : leftIndexCursor;
      if (ParticipantIsRightHanded)
      {
        leapFingerCursorsInput.Cursors.Remove(CursorType.LeftIndex);
        experimentDetailsLogger.ProjectedIndex = projectedCursorsSync.ProjectedCursors[CursorType.RightIndex];
        experimentDetailsLogger.ProjectedThumb = projectedCursorsSync.ProjectedCursors[CursorType.RightThumb];
      }
      else
      {
        leapFingerCursorsInput.Cursors.Remove(CursorType.RightIndex);
        experimentDetailsLogger.ProjectedIndex = projectedCursorsSync.ProjectedCursors[CursorType.LeftIndex];
        experimentDetailsLogger.ProjectedThumb = projectedCursorsSync.ProjectedCursors[CursorType.LeftThumb];
      }
    }

    public override void ActivateTask()
    {
      base.ActivateTask();

      if (technique.CurrentCondition.useLeapInput)
      {
        leapFingerCursorsInput.enabled = true;
        ActivateHand(ParticipantIsRightHanded, true);
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
      var cursor = (isRightHand) ? rightIndexCursor : leftIndexCursor;
      var leapHand = (isRightHand) ? rightLeapHand : leftLeapHand;

      cursor.SetActive(value);
      leapHand.gameObject.SetActive(activateLeapHands & value);
    }
  }
}