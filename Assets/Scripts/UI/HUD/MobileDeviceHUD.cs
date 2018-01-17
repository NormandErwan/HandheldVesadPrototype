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
    private Button toggleZoomButton;

    [SerializeField]
    private Text toggleZoomButtonText;

    // Properties

    public Button ActivateTaskButton { get { return activateTaskButton; } set { activateTaskButton = value; } }
    public Button NextStateButton { get { return nextStateButton; } set { nextStateButton = value; } }
    public Button dragToZoomButton { get { return toggleZoomButton; } set { toggleZoomButton = value; } }

    // Events

    public event Action ActivateTaskButtonPressed = delegate { };
    public event Action NextStateButtonPressed = delegate { };
    public event Action<bool> DragToZoomButtonPressed = delegate { };

    // Variables

    protected bool dragToZoomButtonActivated = false;
    protected Color toggleZoomButtonDefaultNormalColor;

    // MonoBehaviour methods

    protected virtual void Start()
    {
      ActivateTaskButton.onClick.AddListener(activateTaskButton_onClick);
      NextStateButton.onClick.AddListener(nextStateButtonButton_onClick);
      dragToZoomButton.onClick.AddListener(toggleDragToZoomButton_onClick);

      toggleZoomButtonDefaultNormalColor = dragToZoomButton.colors.normalColor;

      HideAllButtons();
    }

    protected virtual void OnDestroy()
    {
      ActivateTaskButton.onClick.RemoveListener(activateTaskButton_onClick);
      NextStateButton.onClick.RemoveListener(nextStateButtonButton_onClick);
      dragToZoomButton.onClick.RemoveListener(toggleDragToZoomButton_onClick);
    }

    // Methods

    public virtual void HideAllButtons()
    {
      ActivateTaskButton.gameObject.SetActive(false);
      NextStateButton.gameObject.SetActive(false);
      dragToZoomButton.gameObject.SetActive(false);
    }

    public virtual void ShowOneButton(Button button)
    {
      HideAllButtons();
      if (button == dragToZoomButton)
      {
        dragToZoomButtonActivated = false;
      }
      button.gameObject.SetActive(true);
    }

    protected virtual void activateTaskButton_onClick()
    {
      ActivateTaskButtonPressed();
    }

    protected virtual void nextStateButtonButton_onClick()
    {
      NextStateButtonPressed();
    }

    protected virtual void toggleDragToZoomButton_onClick()
    {
      dragToZoomButtonActivated = !dragToZoomButtonActivated;
      UpdateDragToZoomButton();
      DragToZoomButtonPressed(dragToZoomButtonActivated);
    }

    protected virtual void UpdateDragToZoomButton()
    {
      var colors = dragToZoomButton.colors;
      if (!dragToZoomButtonActivated)
      {
        toggleZoomButtonText.text = "Activer le mode zoom";
        colors.normalColor = toggleZoomButtonDefaultNormalColor ;
      }
      else
      {
        toggleZoomButtonText.text = (!dragToZoomButtonActivated) ? "Activer le mode zoom" : "Désactiver le mode zoom";
        colors.normalColor = colors.pressedColor;
      }
      dragToZoomButton.colors = colors; 
    }
  }
}
