using NormandErwan.MasterThesisExperiment.Experiment.States;
using NormandErwan.MasterThesisExperiment.Experiment.Variables;
using UnityEngine;
using UnityEngine.UI;

namespace NormandErwan.MasterThesisExperiment.UI.HUD
{
  public class DeviceHUD : ServerHUD
  {
    // Editor Fields

    public GameObject stateTextsParent;
    public Text stateTitleText;
    public Text stateInstructionsText;

    // Methods

    protected override void StateManager_CurrentStateUpdated(State currentState)
    {
      progressText.text = "État courant : " + stateController.CurrentState.title + " - "
          + "Progression : " + (stateController.StatesProgress * 100f / stateController.StatesTotal).ToString("F1") + "%";

      stateTextsParent.SetActive(true);
      stateTitleText.text = stateController.CurrentState.title;
      stateInstructionsText.text = stateController.CurrentState.instructions;

      if (currentState.id == stateController.taskBeginState.id || currentState.id == stateController.taskTrialState.id)
      {
        foreach (var independentVariable in stateController.independentVariables)
        {
          var ivClassificationDifficulty = independentVariable as IVClassificationDifficulty;
          if (ivClassificationDifficulty != null)
          {
            stateInstructionsText.text += "\n\n" + ivClassificationDifficulty.title + " : " + ivClassificationDifficulty.CurrentCondition.title;
          }

          var ivTextSize = independentVariable as IVTextSize;
          if (ivTextSize != null)
          {
            stateInstructionsText.text += "\n\n" + ivTextSize.title + " : " + ivTextSize.CurrentCondition.title;
          }

          var ivTechnique = independentVariable as IVTechnique;
          if (ivTechnique != null)
          {
            stateInstructionsText.text += "\n\n" + ivTechnique.title + " : " + ivTechnique.CurrentCondition.title;
            if (ivTechnique.CurrentCondition.instructions.Length > 0)
            {
              stateInstructionsText.text += "\n" + ivTechnique.CurrentCondition.instructions;
            }
          }
        }
      }
    }

    protected override void validateStateButton_onClik()
    {
      if (stateController.CurrentState.id == stateController.taskTrialState.id)
      {
        stateTextsParent.SetActive(false);
      }
      else
      {
        stateController.NextState();
      }
    }
  }
}
