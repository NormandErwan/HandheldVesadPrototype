using NormandErwan.MasterThesis.Experiment.Experiment.Task;
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
    private GameObject taskGridButtons;

    [SerializeField]
    private Button taskGridSelectModeButton;

    [SerializeField]
    private Button taskGridPanModeButton;

    [SerializeField]
    private Button taskGridZoomModeButton;

    // Properties

    public Button ActivateTaskButton { get { return activateTaskButton; } set { activateTaskButton = value; } }
    public Button NextStateButton { get { return nextStateButton; } set { nextStateButton = value; } }

    public GameObject TaskGridButtons { get { return taskGridButtons; } set { taskGridButtons = value; } }
    public Button TaskGridSelectModeButton { get { return taskGridSelectModeButton; } set { taskGridSelectModeButton = value; } }
    public Button TaskGridPanModeButton { get { return taskGridPanModeButton; } set { taskGridPanModeButton = value; } }
    public Button TaskGridZoomModeButton { get { return taskGridZoomModeButton; } set { taskGridZoomModeButton = value; } }

    // Events

    public event Action ActivateTaskButtonPressed = delegate { };
    public event Action NextStateButtonPressed = delegate { };
    public event Action<TaskGrid.InteractionMode> TaskGridModeButtonPressed = delegate { };

    // Variables

    protected Button[] taskGridButtonsList;
    protected Color taskGridButtonNormalColor;

    // MonoBehaviour methods

    protected virtual void Start()
    {
      taskGridButtonsList = new Button[] { TaskGridSelectModeButton, TaskGridPanModeButton, TaskGridZoomModeButton };
      taskGridButtonNormalColor = TaskGridSelectModeButton.colors.normalColor;

      ActivateTaskButton.onClick.AddListener(activateTaskButton_onClick);
      NextStateButton.onClick.AddListener(nextStateButtonButton_onClick);
      TaskGridZoomModeButton.onClick.AddListener(selectModeButton_onClick);
      TaskGridZoomModeButton.onClick.AddListener(panModeButton_onClick);
      TaskGridZoomModeButton.onClick.AddListener(zoomModeButton_onClick);

      HideAllButtons();
    }

    protected virtual void OnDestroy()
    {
      ActivateTaskButton.onClick.RemoveListener(activateTaskButton_onClick);
      NextStateButton.onClick.RemoveListener(nextStateButtonButton_onClick);
      TaskGridZoomModeButton.onClick.RemoveListener(selectModeButton_onClick);
      TaskGridZoomModeButton.onClick.RemoveListener(panModeButton_onClick);
      TaskGridZoomModeButton.onClick.RemoveListener(zoomModeButton_onClick);
    }

    // Methods

    public virtual void HideAllButtons()
    {
      ActivateTaskButton.gameObject.SetActive(false);
      NextStateButton.gameObject.SetActive(false);
      taskGridButtons.gameObject.SetActive(false);
    }

    public virtual void ToggleButtons(GameObject button)
    {
      HideAllButtons();
      button.SetActive(true);
    }

    protected virtual void activateTaskButton_onClick()
    {
      ActivateTaskButtonPressed();
    }

    protected virtual void nextStateButtonButton_onClick()
    {
      NextStateButtonPressed();
    }

    protected virtual void selectModeButton_onClick()
    {
      UpdateTaskGridButtonBackgrounds(TaskGridSelectModeButton);
      TaskGridModeButtonPressed(TaskGrid.InteractionMode.Select);
    }

    protected virtual void panModeButton_onClick()
    {
      UpdateTaskGridButtonBackgrounds(TaskGridPanModeButton);
      TaskGridModeButtonPressed(TaskGrid.InteractionMode.Pan);
    }

    protected virtual void zoomModeButton_onClick()
    {
      UpdateTaskGridButtonBackgrounds(TaskGridZoomModeButton);
      TaskGridModeButtonPressed(TaskGrid.InteractionMode.Zoom);
    }

    protected virtual void UpdateTaskGridButtonBackgrounds(Button activatedButton)
    {
      foreach (var taskGridButton in taskGridButtonsList)
      {
        var colors = taskGridButton.colors;
        colors.normalColor = (taskGridButton == activatedButton) ? taskGridButton.colors.pressedColor : taskGridButtonNormalColor;
        taskGridButton.colors = colors;
      }
    }
  }
}
