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

    // Variables

    protected StateController stateController;
    protected IVTechnique technique;

    // Methods

    protected virtual void Start()
    {
      ShowContent(false);
    }

    public void Configure(StateController stateController)
    {
      this.stateController = stateController;
      technique = stateController.GetIndependentVariable<IVTechnique>();
    }

    public void ShowContent(bool value)
    {
      progressText.gameObject.SetActive(value);
      stateTextsParent.SetActive(value);
    }

    public void UpdateInstructionsProgress()
    {
      progressText.text = "État courant : " + stateController.CurrentState.Title + " - "
          + "Progression : " + (stateController.StatesProgress * 100f / stateController.StatesTotal).ToString("F1") + "%";

      stateTextsParent.SetActive(true);
      stateTitleText.text = stateController.CurrentState.Title;
      stateInstructionsText.text = stateController.CurrentState.Instructions;
      if (stateController.CurrentState.ActivateTask)
      {
        stateInstructionsText.text += "\n\n" + technique.CurrentCondition.instructions;
      }
    }
  }
}
