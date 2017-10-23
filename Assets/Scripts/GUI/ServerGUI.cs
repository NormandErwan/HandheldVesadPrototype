using NormandErwan.MasterThesisExperiment.States;
using UnityEngine;
using UnityEngine.UI;

namespace NormandErwan.MasterThesisExperiment.GUI
{
    public class ServerGUI : DeviceServerGUI
    {
        protected override void StateManager_CurrentStateUpdated(State currentState)
        {
            progressText.text = stateManager.ToString();
        }
    }
}
