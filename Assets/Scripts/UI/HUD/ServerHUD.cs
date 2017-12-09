using NormandErwan.MasterThesisExperiment.Experiment.States;
using UnityEngine;
using UnityEngine.UI;

namespace NormandErwan.MasterThesisExperiment.UI.HUD
{
  public class ServerHUD : MonoBehaviour
  {
    // Editor Fields

    public StateController stateController;
    public Text progressText;
    public Button validateStateButton;

    // Methods

    protected virtual void Start()
    {
      if (stateController.CurrentState != null)
      {
        StateManager_CurrentStateUpdated(stateController.CurrentState);
      }
      stateController.CurrentStateUpdated += StateManager_CurrentStateUpdated;

      validateStateButton.onClick.AddListener(validateStateButton_onClik);
    }

    protected virtual void OnDestroy()
    {
      stateController.CurrentStateUpdated -= StateManager_CurrentStateUpdated;
      validateStateButton.onClick.RemoveListener(validateStateButton_onClik);
    }

    protected virtual void StateManager_CurrentStateUpdated(State currentState)
    {
      progressText.text = stateController.ToString();
    }

    protected virtual void validateStateButton_onClik()
    {
      stateController.NextState();
    }
  }
}
