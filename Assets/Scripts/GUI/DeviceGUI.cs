using NormandErwan.MasterThesisExperiment.States;
using NormandErwan.MasterThesisExperiment.Variables;
using UnityEngine;
using UnityEngine.UI;

namespace NormandErwan.MasterThesisExperiment.GUI
{
    public class DeviceGUI : ServerGUI
    {
        // Editor Fields

        public GameObject stateTextsParent;
        public Text stateTitleText;
        public Text stateInstructionsText;

        // Methods

        protected override void StateManager_CurrentStateUpdated(State currentState)
        {
            progressText.text = "État courant : " + stateManager.CurrentState.title + " - "
                + "Progression : " + (stateManager.StatesProgress * 100f / stateManager.StatesTotal).ToString("F1") + "%";

            stateTextsParent.SetActive(true);
            stateTitleText.text = stateManager.CurrentState.title;
            stateInstructionsText.text = stateManager.CurrentState.instructions;

            if (currentState.id == stateManager.taskBeginState.id || currentState.id == stateManager.taskTrialState.id)
            {
                foreach (var independentVariable in stateManager.independentVariables)
                {
                    var ivDistance = independentVariable as IVClassificationDistance;
                    if (ivDistance != null)
                    {
                        stateInstructionsText.text += "\n\n" + ivDistance.title + " : " + ivDistance.CurrentCondition.title;
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
            if (stateManager.CurrentState.id == stateManager.taskTrialState.id)
            {
                stateTextsParent.SetActive(false);
            }
            else
            {
                stateManager.NextState();
            }
        }
    }
}
