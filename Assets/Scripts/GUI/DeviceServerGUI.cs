using NormandErwan.MasterThesisExperiment.States;
using UnityEngine;
using UnityEngine.UI;

namespace NormandErwan.MasterThesisExperiment.GUI
{
    public abstract class DeviceServerGUI : MonoBehaviour
    {
        // Editor Fields

        public StateManager stateManager;
        public Text progressText;

        // Methods

        protected virtual void Start()
        {
            if (stateManager.CurrentState != null)
            {
                StateManager_CurrentStateUpdated(stateManager.CurrentState);
            }
            stateManager.CurrentStateUpdated += StateManager_CurrentStateUpdated;
        }

        protected virtual void OnDestroy()
        {
            stateManager.CurrentStateUpdated -= StateManager_CurrentStateUpdated;
        }

        protected abstract void StateManager_CurrentStateUpdated(State currentState);
    }
}
