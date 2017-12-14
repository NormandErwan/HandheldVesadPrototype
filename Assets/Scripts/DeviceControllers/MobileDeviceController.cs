using NormandErwan.MasterThesis.Experiment.Experiment.States;
using NormandErwan.MasterThesis.Experiment.Inputs;
using NormandErwan.MasterThesis.Experiment.UI.HUD;
using UnityEngine;

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

    // Methods

    protected override void Start()
    {
      base.Start();

      camera.orthographic = true;
      camera.orthographicSize = 0.5f * Grid.transform.localScale.y * (Grid.ElementScale.y + Grid.ElementMargin.y);

      TouchFingerCursorsInput.Configure(maxSelectableDistance);
      TouchFingerCursorsInput.gameObject.SetActive(false);

      MobileDeviceHUD.ActivateTaskButtonPressed += MobileDeviceHUD_ActivateTaskButtonPressed;
      MobileDeviceHUD.NextStateButtonPressed += MobileDeviceHUD_NextStateButtonPressed;
      MobileDeviceHUD.ZoomModeToggleButtonPressed += MobileDeviceHUD_ZoomModeToggleButtonPressed;

      // TODO: remove, for debug testing only
      /*StateController.BeginExperiment();
      ActivateTask();*/
    }

    protected override void OnDestroy()
    {
      base.OnDestroy();

      MobileDeviceHUD.ActivateTaskButtonPressed -= MobileDeviceHUD_ActivateTaskButtonPressed;
      MobileDeviceHUD.NextStateButtonPressed -= MobileDeviceHUD_NextStateButtonPressed;
      MobileDeviceHUD.ZoomModeToggleButtonPressed -= MobileDeviceHUD_ZoomModeToggleButtonPressed;
    }

    public override void ActivateTask()
    {
      base.ActivateTask();

      if (ivTechnique.CurrentCondition.useLeapInput)
      {
        MobileDeviceHUD.ShowToggleButton(MobileDeviceHUD.ZoomModeToggleButton);
      }
      else if (ivTechnique.CurrentCondition.useTouchInput)
      {
        TouchFingerCursorsInput.gameObject.SetActive(true);
      }
    }

    protected override void StateController_CurrentStateUpdated(State currentState)
    {
      base.StateController_CurrentStateUpdated(currentState);

      TouchFingerCursorsInput.gameObject.SetActive(false);

      if (currentState.ActivateTask)
      {
        MobileDeviceHUD.ShowToggleButton(MobileDeviceHUD.ActivateTaskButton);
      }
      else
      {
        MobileDeviceHUD.ShowToggleButton(MobileDeviceHUD.NextStateButton);
      }
    }

    protected virtual void MobileDeviceHUD_ActivateTaskButtonPressed()
    {
      MobileDeviceHUD.ShowToggleButton(null);
      OnActivateTaskSync();
    }

    protected virtual void MobileDeviceHUD_NextStateButtonPressed()
    {
      MobileDeviceHUD.ShowToggleButton(null);
      StateController.NextState();
    }

    protected virtual void MobileDeviceHUD_ZoomModeToggleButtonPressed(bool zoomModeActivated)
    {
      OnToogleZoomModeSync(zoomModeActivated);
    }
  }
}