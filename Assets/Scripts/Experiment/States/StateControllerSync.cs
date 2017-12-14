using DevicesSyncUnity;
using DevicesSyncUnity.Messages;
using UnityEngine;
using UnityEngine.Networking;

namespace NormandErwan.MasterThesis.Experiment.Experiment.States
{
  /// <summary>
  /// Synchronize the experiment between devices with <see cref="StateControllerMessage"/>.
  /// </summary>
  public class StateControllerSync : DevicesSync
  {
    // Editor Fields

    [SerializeField]
    private StateController stateController;

    // Properties

    public StateController StateController { get { return stateController; } set { stateController = value; } }
    protected override int DefaultChannelId { get { return Channels.DefaultReliable; } }

    // Variables

    protected StateControllerMessage stateControllerMessage = new StateControllerMessage();

    // Methods

    protected override void Awake()
    {
      base.Awake();

      StateController.CurrentStateSync += StateManager_CurrentStateSync;

      MessageTypes.Add(stateControllerMessage.MessageType);
    }

    protected virtual void OnDestroy()
    {
      StateController.CurrentStateSync -= StateManager_CurrentStateSync;
    }

    protected override DevicesSyncMessage OnServerMessageReceived(NetworkMessage netMessage)
    {
      var stateControllerMessage = netMessage.ReadMessage<StateControllerMessage>();
      StateController.SetCurrentState(stateControllerMessage.currentStateId);
      return stateControllerMessage;
    }

    protected override DevicesSyncMessage OnClientMessageReceived(NetworkMessage netMessage)
    {
      var stateControllerMessage = netMessage.ReadMessage<StateControllerMessage>();
      if (!isServer)
      {
        StateController.SetCurrentState(stateControllerMessage.currentStateId);
      }
      return stateControllerMessage;
    }

    protected virtual void StateManager_CurrentStateSync(State currentState)
    {
      stateControllerMessage.currentStateId = StateController.CurrentState.id;
      SendToServer(stateControllerMessage);
    }
  }
}
