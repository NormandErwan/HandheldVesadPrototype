using NormandErwan.MasterThesis.Experiment.Experiment.States;
using NormandErwan.MasterThesis.Experiment.Experiment.Variables;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace NormandErwan.MasterThesis.Experiment.UI.HUD
{
  public class ServerHUD : MonoBehaviour
  {
    // Editor Fields

    [SerializeField]
    private Text progressText;

    [SerializeField]
    private Button nextStateButton;

    [SerializeField]
    private RectTransform experimentConfigurationRect;

    [SerializeField]
    private InputField participantIdInput;

    [SerializeField]
    private InputField conditionsOrderingInput;

    [SerializeField]
    private Toggle participantIsRightHandedToggle;

    [SerializeField]
    private Button beginExperimentButton;

    // Properties

    public int ParticipantId { get { return int.Parse(participantIdInput.text); } }
    public int ConditionsOrdering { get { return int.Parse(conditionsOrderingInput.text); } }
    public bool ParticipantIsRightHanded { get { return participantIsRightHandedToggle.isOn; } }

    // Events

    public event Action RequestNextState = delegate { };
    public event Action RequestBeginExperiment = delegate { };

    // Methods

    public virtual void UpdateInstructionsProgress(StateController stateController)
    {
      progressText.text = stateController.ToString();

      progressText.text += "\n\nConditions :";
      if (stateController.CurrentState.ActivateTask)
      {
        var ivClassificationDifficulty = stateController.GetIndependentVariable<IVClassificationDifficulty>();
        progressText.text += "\n" + ivClassificationDifficulty.title + " : " + ivClassificationDifficulty.CurrentCondition.title;

        var ivTextSize = stateController.GetIndependentVariable<IVTextSize>();
        progressText.text += "\n" + ivTextSize.title + " : " + ivTextSize.CurrentCondition.title;

        var ivTechnique = stateController.GetIndependentVariable<IVTechnique>();
        progressText.text += "\n" + ivTechnique.title + " : " + ivTechnique.CurrentCondition.title;
      }
    }

    public virtual void DeactivateExperimentConfiguration()
    {
      experimentConfigurationRect.gameObject.SetActive(false);
    }

    protected virtual void OnEnable()
    {
      nextStateButton.onClick.AddListener(nextStateButton_onClik);
      beginExperimentButton.onClick.AddListener(beginExperimentButton_onClik);
    }

    protected virtual void OnDisable()
    {
      nextStateButton.onClick.RemoveListener(nextStateButton_onClik);
      beginExperimentButton.onClick.RemoveListener(beginExperimentButton_onClik);
    }

    protected virtual void nextStateButton_onClik()
    {
      RequestNextState();
    }

    protected virtual void beginExperimentButton_onClik()
    {
      RequestBeginExperiment();
    }
  }
}
