using NormandErwan.MasterThesis.Experiment.Experiment.Task;
using System;
using System.Collections.Generic;
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
    private GameObject taskGridButtonsParent;

    [SerializeField]
    private Button taskGridSelectModeButton;

    [SerializeField]
    private Button taskGridPanModeButton;

    [SerializeField]
    private Button taskGridZoomModeButton;

    // Properties

    public Button ActivateTaskButton { get { return activateTaskButton; } set { activateTaskButton = value; } }
    public Button NextStateButton { get { return nextStateButton; } set { nextStateButton = value; } }

    public GameObject TaskGridButtonParent { get { return taskGridButtonsParent; } set { taskGridButtonsParent = value; } }
    public Button TaskGridSelectModeButton { get { return taskGridSelectModeButton; } set { taskGridSelectModeButton = value; } }
    public Button TaskGridPanModeButton { get { return taskGridPanModeButton; } set { taskGridPanModeButton = value; } }
    public Button TaskGridZoomModeButton { get { return taskGridZoomModeButton; } set { taskGridZoomModeButton = value; } }

    // Events

    public event Action ActivateTaskButtonPressed = delegate { };
    public event Action NextStateButtonPressed = delegate { };
    public event Action<TaskGrid.InteractionMode> TaskGridModeButtonPressed = delegate { };

    // Variables

    protected Dictionary<TaskGrid.InteractionMode, Button> taskGridButtons = new Dictionary<TaskGrid.InteractionMode, Button>();
    protected Color taskGridButtonNormalColor;

    // MonoBehaviour methods

    protected void Start()
    {
      taskGridButtons.Add(TaskGrid.InteractionMode.Select, TaskGridSelectModeButton);
      taskGridButtons.Add(TaskGrid.InteractionMode.Pan, TaskGridPanModeButton);
      taskGridButtons.Add(TaskGrid.InteractionMode.Zoom, TaskGridZoomModeButton);
      taskGridButtonNormalColor = TaskGridSelectModeButton.colors.normalColor;

      ActivateTaskButton.onClick.AddListener(activateTaskButton_onClick);
      NextStateButton.onClick.AddListener(nextStateButtonButton_onClick);
      TaskGridSelectModeButton.onClick.AddListener(selectModeButton_onClick);
      TaskGridPanModeButton.onClick.AddListener(panModeButton_onClick);
      TaskGridZoomModeButton.onClick.AddListener(zoomModeButton_onClick);

      HideAllButtons();
    }

    protected void OnDestroy()
    {
      ActivateTaskButton.onClick.RemoveListener(activateTaskButton_onClick);
      NextStateButton.onClick.RemoveListener(nextStateButtonButton_onClick);
      TaskGridSelectModeButton.onClick.RemoveListener(selectModeButton_onClick);
      TaskGridPanModeButton.onClick.RemoveListener(panModeButton_onClick);
      TaskGridZoomModeButton.onClick.RemoveListener(zoomModeButton_onClick);
    }

    // Methods

    public void HideAllButtons()
    {
      ActivateTaskButton.gameObject.SetActive(false);
      NextStateButton.gameObject.SetActive(false);
      taskGridButtonsParent.gameObject.SetActive(false);
    }

    public void ToggleButtons(GameObject button)
    {
      HideAllButtons();
      button.SetActive(true);
    }

    public void SetActiveTaskModeButton(TaskGrid.InteractionMode interactionMode)
    {
      foreach (var taskGridButton in taskGridButtons)
      {
        var colors = taskGridButton.Value.colors;
        colors.normalColor = (taskGridButton.Key == interactionMode) ? taskGridButton.Value.colors.pressedColor : taskGridButtonNormalColor;
        taskGridButton.Value.colors = colors;
      }
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
      SetActiveTaskModeButton(TaskGrid.InteractionMode.Select);
      TaskGridModeButtonPressed(TaskGrid.InteractionMode.Select);
    }

    protected virtual void panModeButton_onClick()
    {
      SetActiveTaskModeButton(TaskGrid.InteractionMode.Pan);
      TaskGridModeButtonPressed(TaskGrid.InteractionMode.Pan);
    }

    protected virtual void zoomModeButton_onClick()
    {
      SetActiveTaskModeButton(TaskGrid.InteractionMode.Zoom);
      TaskGridModeButtonPressed(TaskGrid.InteractionMode.Zoom);
    }
  }
}
