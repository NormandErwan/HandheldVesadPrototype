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

    // Methods

    protected override void Start()
    {
      base.Start();

      ParticipantLogger.DeviceControllerName = "mobile";

      camera.orthographic = true;
      camera.orthographicSize = 0.5f * Grid.transform.localScale.y * (Grid.ElementScale.y + Grid.ElementMargin.y);

      touchFingerCursorsInput.Configure(maxSelectableDistance);
      touchFingerCursorsInput.gameObject.SetActive(false);

      mobileDeviceHUD.ActivateTaskButtonPressed += MobileDeviceHUD_ActivateTaskButtonPressed;
      mobileDeviceHUD.NextStateButtonPressed += MobileDeviceHUD_NextStateButtonPressed;
      mobileDeviceHUD.ZoomModeToggleButtonPressed += MobileDeviceHUD_ZoomModeToggleButtonPressed;

      // TODO: remove, for debug testing only
      /*StateController.BeginExperiment();
      ActivateTask();*/
    }

    protected override void OnDestroy()
    {
      base.OnDestroy();
      mobileDeviceHUD.ActivateTaskButtonPressed -= MobileDeviceHUD_ActivateTaskButtonPressed;
      mobileDeviceHUD.NextStateButtonPressed -= MobileDeviceHUD_NextStateButtonPressed;
      mobileDeviceHUD.ZoomModeToggleButtonPressed -= MobileDeviceHUD_ZoomModeToggleButtonPressed;
    }

    public override void ActivateTask()
    {
      base.ActivateTask();

      if (ivTechnique.CurrentCondition.useLeapInput)
      {
        mobileDeviceHUD.ShowToggleButton(mobileDeviceHUD.ZoomModeToggleButton);
      }
      else if (ivTechnique.CurrentCondition.useTouchInput)
      {
        touchFingerCursorsInput.gameObject.SetActive(true);
      }
    }

    protected override void StateController_CurrentStateUpdated(State currentState)
    {
      base.StateController_CurrentStateUpdated(currentState);

      touchFingerCursorsInput.gameObject.SetActive(false);

      if (currentState.ActivateTask)
      {
        mobileDeviceHUD.ShowToggleButton(mobileDeviceHUD.ActivateTaskButton);
      }
      else
      {
        mobileDeviceHUD.ShowToggleButton(mobileDeviceHUD.NextStateButton);
      }
    }

    protected virtual void MobileDeviceHUD_ActivateTaskButtonPressed()
    {
      mobileDeviceHUD.ShowToggleButton(null);
      OnActivateTaskSync();
    }

    protected virtual void MobileDeviceHUD_NextStateButtonPressed()
    {
      mobileDeviceHUD.ShowToggleButton(null);
      StateController.NextState();
    }

    protected virtual void MobileDeviceHUD_ZoomModeToggleButtonPressed(bool zoomModeActivated)
    {
      OnToogleZoomModeSync(zoomModeActivated);
    }
  }
}