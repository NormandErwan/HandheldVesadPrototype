using NormandErwan.MasterThesis.Experiment.Experiment.States;
using NormandErwan.MasterThesis.Experiment.Inputs;
using NormandErwan.MasterThesis.Experiment.UI.HUD;
using UnityEngine;
using System.Collections;
using NormandErwan.MasterThesis.Experiment.Experiment.Variables;

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
    [Range(0f, 0.05f)]
    private float maxSelectableDistance = 0.001f;

    [SerializeField]
    private new Camera camera;

    // Properties

    public override CursorsInput CursorsInput { get { return touchFingerCursorsInput; } }

    // Variables

    protected IVTechnique technique;

    // MonoBehaviour methods

    protected override void Start()
    {
      base.Start();

      camera.orthographic = true;
      camera.orthographicSize = 0.5f * TaskGrid.transform.localScale.y * (TaskGrid.ElementScale.y + TaskGrid.ElementMargin.y);

      technique = StateController.GetIndependentVariable<IVTechnique>();

      touchFingerCursorsInput.Configure(maxSelectableDistance);
      touchFingerCursorsInput.enabled = false;

      mobileDeviceHUD.ActivateTaskButtonPressed += MobileDeviceHUD_ActivateTaskButtonPressed;
      mobileDeviceHUD.NextStateButtonPressed += MobileDeviceHUD_NextStateButtonPressed;
      mobileDeviceHUD.DragToZoomButtonPressed += MobileDeviceHUD_DragToZoomButtonPressed;

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
      mobileDeviceHUD.DragToZoomButtonPressed -= MobileDeviceHUD_DragToZoomButtonPressed;
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
        mobileDeviceHUD.ShowOneButton(mobileDeviceHUD.dragToZoomButton);
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

      touchFingerCursorsInput.enabled = false;

      if (currentState.ActivateTask)
      {
        mobileDeviceHUD.ShowOneButton(mobileDeviceHUD.ActivateTaskButton);
      }
      else
      {
        mobileDeviceHUD.ShowOneButton(mobileDeviceHUD.NextStateButton);
      }
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

    protected virtual void MobileDeviceHUD_DragToZoomButtonPressed(bool activated)
    {
      OnSetDragToZoomSync(activated);
    }
  }
}