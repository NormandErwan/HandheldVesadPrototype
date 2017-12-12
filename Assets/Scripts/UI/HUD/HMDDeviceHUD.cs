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
    }

    protected virtual void Start()
    {
      ShowContent(false);
    }
  }
}
