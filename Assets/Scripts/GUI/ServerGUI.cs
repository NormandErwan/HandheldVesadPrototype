using NormandErwan.MasterThesisExperiment.States;
using UnityEngine;
using UnityEngine.UI;

namespace NormandErwan.MasterThesisExperiment.GUI
{
    public class ServerGUI : DeviceGUI
    {
        protected override void StateManager_CurrentStateUpdated(State currentState)
        {
            base.StateManager_CurrentStateUpdated(currentState);
            progressText.text = stateManager.ToString();
        }
    }
}
