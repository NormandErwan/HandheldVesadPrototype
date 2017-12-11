using NormandErwan.MasterThesis.Experiment.Experiment.States;
using NormandErwan.MasterThesis.Experiment.Experiment.Variables;
using UnityEngine;
using UnityEngine.UI;

namespace NormandErwan.MasterThesis.Experiment.UI.HUD
{
  public class HMDDeviceHUD : MonoBehaviour
  {
    // Editor Fields

    [SerializeField]
    private Text progressText;

    [SerializeField]
    private GameObject stateTextsParent;

    [SerializeField]
    private Text stateTitleText;

    [SerializeField]
    private Text stateInstructionsText;

    // Methods

    public virtual void ShowContent(bool value)
    {
      progressText.gameObject.SetActive(value);
      stateTextsParent.SetActive(value);
    }

    public virtual void UpdateInstructionsProgress(StateController stateController)
    {
      progressText.text = "État courant : " + stateController.CurrentState.title + " - "
          + "Progression : " + (stateController.StatesProgress * 100f / stateController.StatesTotal).ToString("F1") + "%";

      stateTextsParent.SetActive(true);
      stateTitleText.text = stateController.CurrentState.title;
      stateInstructionsText.text = stateController.CurrentState.Instructions;

      if (stateController.CurrentState.ActivateTask)
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

    protected virtual void Start()
    {
      ShowContent(false);
    }
  }
}
