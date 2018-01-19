using NormandErwan.MasterThesis.Experiment.Experiment.States;
using NormandErwan.MasterThesis.Experiment.Inputs;
using NormandErwan.MasterThesis.Experiment.UI.HUD;
using UnityEngine;
using System.Collections;
using NormandErwan.MasterThesis.Experiment.Experiment.Variables;
using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;

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

    public MobileDeviceHUD MobileDeviceHUD { get { return mobileDeviceHUD; } set { mobileDeviceHUD = value; } }
    public TouchFingerCursorsInput TouchFingerCursorsInput { get { return touchFingerCursorsInput; } set { touchFingerCursorsInput = value; } }

    // Variables

    protected IVTechnique technique;

    // MonoBehaviour methods

    protected override void Start()
    {
      base.Start();

      camera.orthographic = true;
      camera.orthographicSize = 0.5f * Grid.transform.localScale.y * (Grid.ElementScale.y + Grid.ElementMargin.y);

      technique = StateController.GetIndependentVariable<IVTechnique>();

      TouchFingerCursorsInput.Configure(maxSelectableDistance);
      TouchFingerCursorsInput.enabled = false;

      MobileDeviceHUD.ActivateTaskButtonPressed += MobileDeviceHUD_ActivateTaskButtonPressed;
      MobileDeviceHUD.NextStateButtonPressed += MobileDeviceHUD_NextStateButtonPressed;
      MobileDeviceHUD.DragToZoomButtonPressed += MobileDeviceHUD_DragToZoomButtonPressed;

      // TODO: remove, for debug testing only
      //StartCoroutine(StartTaskDebug());
    }

    protected override IEnumerator StartTaskDebug()
    {
      yield return base.StartTaskDebug();
      MobileDeviceHUD.HideAllButtons();
    }

    protected override void OnDestroy()
    {
      base.OnDestroy();

      MobileDeviceHUD.ActivateTaskButtonPressed -= MobileDeviceHUD_ActivateTaskButtonPressed;
      MobileDeviceHUD.NextStateButtonPressed -= MobileDeviceHUD_NextStateButtonPressed;
      MobileDeviceHUD.DragToZoomButtonPressed -= MobileDeviceHUD_DragToZoomButtonPressed;
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
        MobileDeviceHUD.ShowOneButton(MobileDeviceHUD.dragToZoomButton);
      }
      if (technique.CurrentCondition.useTouchInput)
      {
        TouchFingerCursorsInput.enabled = true;
      }
    }

    protected override void StateController_CurrentStateUpdated(State currentState)
    {
      base.StateController_CurrentStateUpdated(currentState);

      TouchFingerCursorsInput.enabled = false;

      if (currentState.ActivateTask)
      {
        MobileDeviceHUD.ShowOneButton(MobileDeviceHUD.ActivateTaskButton);
      }
      else
      {
        MobileDeviceHUD.ShowOneButton(MobileDeviceHUD.NextStateButton);
      }
    }

    protected virtual void MobileDeviceHUD_ActivateTaskButtonPressed()
    {
      MobileDeviceHUD.HideAllButtons();
      OnActivateTaskSync();
    }

    protected virtual void MobileDeviceHUD_NextStateButtonPressed()
    {
      MobileDeviceHUD.HideAllButtons();
      StateController.NextState();
    }

    protected virtual void MobileDeviceHUD_DragToZoomButtonPressed(bool activated)
    {
      OnSetDragToZoomSync(activated);
    }
  }
}