using NormandErwan.MasterThesis.Experiment.Experiment.States;
using NormandErwan.MasterThesis.Experiment.Inputs;
using NormandErwan.MasterThesis.Experiment.UI.HUD;
using UnityEngine;
using System.Collections;
using NormandErwan.MasterThesis.Experiment.Experiment.Task;
using NormandErwan.MasterThesis.Experiment.Inputs.Cursors;

namespace NormandErwan.MasterThesis.Experiment.DeviceControllers
{
  public class MobileDeviceController : DeviceController
  {
    // Editor fields

    [SerializeField]
    private MobileDeviceHUD mobileDeviceHUD;

    [SerializeField]
    private TouchFingerCursorsInput touchFingerCursorsInput;

    [SerializeField]
    private new Camera camera;

    // Properties

    public override FingerCursorsInput FingerCursorsInput { get { return touchFingerCursorsInput; } }

    // MonoBehaviour methods

    protected override void Start()
    {
      base.Start();

      camera.orthographic = true;
      camera.orthographicSize = 0.5f * TaskGrid.transform.localScale.y * (TaskGrid.ElementScale.y + TaskGrid.ElementMargin.y);

      touchFingerCursorsInput.Configure(FingerCursorTriggerITappable.defaultMaxCursorDistance, FingerCursorTriggerITappable.defaultMaxCursorDistance);
      touchFingerCursorsInput.enabled = false;

      mobileDeviceHUD.ActivateTaskButtonPressed += MobileDeviceHUD_ActivateTaskButtonPressed;
      mobileDeviceHUD.NextStateButtonPressed += MobileDeviceHUD_NextStateButtonPressed;
      mobileDeviceHUD.TaskGridModeButtonPressed += MobileDeviceHUD_TaskGridModeButtonPressed;

      // TODO: remove, for debug testing only
      //StartCoroutine(StartTaskDebug());
    }

    protected override IEnumerator StartTaskDebug()
    {
      yield return base.StartTaskDebug();
      mobileDeviceHUD.HideAllButtons();
    }

    protected override void OnDestroy()
    {
      base.OnDestroy();

      mobileDeviceHUD.ActivateTaskButtonPressed -= MobileDeviceHUD_ActivateTaskButtonPressed;
      mobileDeviceHUD.NextStateButtonPressed -= MobileDeviceHUD_NextStateButtonPressed;
      mobileDeviceHUD.TaskGridModeButtonPressed -= MobileDeviceHUD_TaskGridModeButtonPressed;
    }

    // Methods

    public override void Configure(int participantId, int conditionsOrdering, bool participantIsRightHanded)
    {
      base.Configure(participantId, conditionsOrdering, participantIsRightHanded);

      touchFingerCursorsInput.ParticipantIsRightHanded = ParticipantIsRightHanded;
    }

    public override void ActivateTask()
    {
      base.ActivateTask();

      if (technique.CurrentCondition.useLeapInput)
      {
        mobileDeviceHUD.ToggleButtons(mobileDeviceHUD.TaskGridButtonParent);
        mobileDeviceHUD.SetActiveTaskModeButton(TaskGrid.Mode);
      }
      if (technique.CurrentCondition.useTouchInput)
      {
        touchFingerCursorsInput.enabled = true;
      }

      foreach (var cursor in touchFingerCursorsInput.Cursors)
      {
        cursor.Value.SetActive(technique.CurrentCondition.useTouchInput);
      }
    }

    protected override void StateController_CurrentStateUpdated(State currentState)
    {
      base.StateController_CurrentStateUpdated(currentState);

      if (currentState.ActivateTask)
      {
        mobileDeviceHUD.ToggleButtons(mobileDeviceHUD.ActivateTaskButton.gameObject);
      }
      else if (currentState != StateController.experimentEndState)
      {
        mobileDeviceHUD.ToggleButtons(mobileDeviceHUD.NextStateButton.gameObject);
      }
    }

    protected override void TaskGrid_Configured()
    {
      base.TaskGrid_Configured();
      TaskGrid.Show(true);
    }

    protected virtual void MobileDeviceHUD_ActivateTaskButtonPressed()
    {
      mobileDeviceHUD.HideAllButtons();
      OnActivateTaskSync();
    }

    protected virtual void MobileDeviceHUD_NextStateButtonPressed()
    {
      mobileDeviceHUD.HideAllButtons();
      StateController.NextState();
    }

    protected virtual void MobileDeviceHUD_TaskGridModeButtonPressed(TaskGrid.InteractionMode interactionMode)
    {
      OnSetTaskGridModeSync(interactionMode);
    }
  }
}