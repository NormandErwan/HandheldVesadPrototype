using NormandErwan.MasterThesis.Experiment.Experiment.States;
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

      if (stateController.CurrentState.ActivateTask)
      {
        foreach (var independentVariable in stateController.independentVariables)
        {
          // TODO
        }
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
