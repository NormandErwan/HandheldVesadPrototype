using NormandErwan.MasterThesis.Experiment.Experiment.States;
using NormandErwan.MasterThesis.Experiment.Experiment.Task;
using NormandErwan.MasterThesis.Experiment.Inputs;
using NormandErwan.MasterThesis.Experiment.Inputs.Cursors;
using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
using NormandErwan.MasterThesis.Experiment.UI.HUD;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.DeviceControllers
{
  public class HMDDeviceController : DeviceController
  {
    // Constants

    protected const float minTransformableDistance = FingerCursorTriggerITappable.defaultMaxCursorDistance;
    protected const float maxSelectableDistance = 0.01f;

    // Editor fields

    [SerializeField]
    private TaskGridMasks taskGridMasks;

    [SerializeField]
    private HMDDeviceHUD hmdDeviceHUD;

    [Header("Cursors")]
    [SerializeField]
    private LookCursorInput lookCursorsInput;

    [SerializeField]
    private LeapFingerCursorsInput leapFingerCursorsInput;

    [SerializeField]
    private Renderer rightLeapHand;

    [SerializeField]
    private Renderer leftLeapHand;

    // Properties

    public override FingerCursorsInput FingerCursorsInput { get { return leapFingerCursorsInput; } }

    // Methods

    protected override void Start()
    {
      base.Start();

      lookCursorsInput.enabled = false;

      leapFingerCursorsInput.Configure(minTransformableDistance, maxSelectableDistance);
      leapFingerCursorsInput.TaskGrid = TaskGrid.transform;
      leapFingerCursorsInput.enabled = false;

      hmdDeviceHUD.Configure(StateController);

      // TODO: remove, for debug testing only
      //ParticipantIsRightHanded = true;
      //StartCoroutine(StartTaskDebug());
    }

    // DeviceController methods

    public override void Configure(int participantId, int conditionsOrdering, bool participantIsRightHanded)
    {
      base.Configure(participantId, conditionsOrdering, participantIsRightHanded);
      lookCursorsInput.ParticipantIsRightHanded = ParticipantIsRightHanded;
    }

    public override void ActivateTask()
    {
      base.ActivateTask();

      lookCursorsInput.enabled = technique.CurrentCondition.useLookInput;
      foreach (var cursor in lookCursorsInput.Cursors)
      {
        cursor.Value.SetActive(cursor.Key == CursorType.Look && technique.CurrentCondition.useLookInput);
      }

      leapFingerCursorsInput.enabled = true;
      foreach (var cursor in leapFingerCursorsInput.Cursors)
      {
        cursor.Value.SetActive(cursor.Value.IsIndex 
          && technique.CurrentCondition.useLeapInput
          && (ParticipantIsRightHanded == cursor.Value.IsRightHanded));
      }

      hmdDeviceHUD.ShowContent(false);
    }

    protected override void StateController_CurrentStateUpdated(State currentState)
    {
      base.StateController_CurrentStateUpdated(currentState);

      hmdDeviceHUD.ShowContent(true);
      hmdDeviceHUD.UpdateInstructionsProgress();
    }

    protected override void TaskGrid_Configured()
    {
      base.TaskGrid_Configured();
      taskGridMasks.Configure();
      TaskGrid.Show(!technique.CurrentCondition.showTaskGridOnlyOnMobileDevice);
    }

    protected override void TaskGrid_Completed()
    {
      base.TaskGrid_Completed();
      taskGridMasks.Hide();
    }
  }
}