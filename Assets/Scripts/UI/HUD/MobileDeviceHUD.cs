using System;
using UnityEngine;
using UnityEngine.UI;

namespace NormandErwan.MasterThesis.Experiment.UI.HUD
{
  public class MobileDeviceHUD : MonoBehaviour
  {
    // Editor Fields

    [SerializeField]
    private Button activateTaskButton;

    [SerializeField]
    private Button nextStateButton;

    [SerializeField]
    private Button zoomModeToggleButton;

    // Properties

    public Button ActivateTaskButton { get { return activateTaskButton; } set { activateTaskButton = value; } }
    public Button NextStateButton { get { return nextStateButton; } set { nextStateButton = value; } }
    public Button ZoomModeToggleButton { get { return zoomModeToggleButton; } set { zoomModeToggleButton = value; } }

    // Events

    public event Action ActivateTaskButtonPressed = delegate { };
    public event Action NextStateButtonPressed = delegate { };
    public event Action<bool> ZoomModeToggleButtonPressed = delegate { };

    // Variables

    protected bool zoomMode = false;

    // Methods

    public virtual void ShowToggleButton(Button button)
    {
      ActivateTaskButton.gameObject.SetActive(false);
      NextStateButton.gameObject.SetActive(false);
      ZoomModeToggleButton.gameObject.SetActive(false);

      if (button != null)
      {
        button.gameObject.SetActive(true);
      }
    }

    protected virtual void Start()
    {
      ActivateTaskButton.onClick.AddListener(activateTaskButton_onClick);
      NextStateButton.onClick.AddListener(nextStateButtonButton_onClick);
      ZoomModeToggleButton.onClick.AddListener(zoomModeToggleButtonButton_onClick);

      ShowToggleButton(null);
    }

    protected virtual void OnDestroy()
    {
      ActivateTaskButton.onClick.RemoveListener(activateTaskButton_onClick);
      NextStateButton.onClick.RemoveListener(nextStateButtonButton_onClick);
      ZoomModeToggleButton.onClick.RemoveListener(zoomModeToggleButtonButton_onClick);
    }

    protected virtual void activateTaskButton_onClick()
    {
      ActivateTaskButtonPressed();
    }

    protected virtual void nextStateButtonButton_onClick()
    {
      NextStateButtonPressed();
    }

    protected virtual void zoomModeToggleButtonButton_onClick()
    {
      zoomMode = !zoomMode;
      ZoomModeToggleButtonPressed(zoomMode);
    }
  }
}
