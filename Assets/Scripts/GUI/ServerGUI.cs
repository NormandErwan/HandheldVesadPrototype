using NormandErwan.MasterThesisExperiment.Experiment.States;
using UnityEngine;
using UnityEngine.UI;

namespace NormandErwan.MasterThesisExperiment.GUI
{
  public class ServerGUI : MonoBehaviour
  {
    // Editor Fields

    public StateManager stateManager;
    public Text progressText;
    public Button validateStateButton;

    // Methods

    protected virtual void Start()
    {
      if (stateManager.CurrentState != null)
      {
        StateManager_CurrentStateUpdated(stateManager.CurrentState);
      }
      stateManager.CurrentStateUpdated += StateManager_CurrentStateUpdated;

      validateStateButton.onClick.AddListener(validateStateButton_onClik);
    }

    protected virtual void OnDestroy()
    {
      stateManager.CurrentStateUpdated -= StateManager_CurrentStateUpdated;
      validateStateButton.onClick.RemoveListener(validateStateButton_onClik);
    }

    protected virtual void StateManager_CurrentStateUpdated(State currentState)
    {
      progressText.text = stateManager.ToString();
    }

    protected virtual void validateStateButton_onClik()
    {
      stateManager.NextState();
    }
  }
}
