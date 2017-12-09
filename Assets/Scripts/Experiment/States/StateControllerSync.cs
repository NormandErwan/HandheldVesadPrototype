using DevicesSyncUnity;
using DevicesSyncUnity.Messages;
using UnityEngine.Networking;

namespace NormandErwan.MasterThesis.Experiment.Experiment.States
{
  /// <summary>
  /// Synchronize the experiment between devices with <see cref="StateControllerMessage"/>.
  /// </summary>
  public class StateControllerSync : DevicesSync
  {
    // Editor Fields

    public StateController stateController;

    // Properties

    protected override int DefaultChannelId { get { return Channels.DefaultReliable; } }

    // Variables

    protected StateControllerMessage currentStateMessage = new StateControllerMessage();

    // Methods

    protected override void Awake()
    {
      base.Awake();

      DeviceDisconnected += DevicesInfoSync_DeviceDisconnected;
      stateController.RequestCurrentStateSync += StateManager_RequestCurrentStateSync;

      MessageTypes.Add(currentStateMessage.MessageType);
    }

    protected virtual void OnDestroy()
    {
      DeviceDisconnected -= DevicesInfoSync_DeviceDisconnected;
      stateController.RequestCurrentStateSync -= StateManager_RequestCurrentStateSync;
    }

    protected override DevicesSyncMessage OnServerMessageReceived(NetworkMessage netMessage)
    {
      currentStateMessage = netMessage.ReadMessage<StateControllerMessage>();
      currentStateMessage.Restore(stateController);
      return currentStateMessage;
    }

    protected override DevicesSyncMessage OnClientMessageReceived(NetworkMessage netMessage)
    {
      var stateMessage = netMessage.ReadMessage<StateControllerMessage>();
      if (!isServer)
      {
        currentStateMessage = stateMessage;
        currentStateMessage.Restore(stateController);
      }
      return stateMessage;
    }

    protected virtual void DevicesInfoSync_DeviceDisconnected(int deviceId)
    {
      if (deviceId != NetworkManager.client.connection.connectionId)
      {
        currentStateMessage.Update(stateController.CurrentState);
        SendToClient(deviceId, currentStateMessage);
      }
    }

    protected virtual void StateManager_RequestCurrentStateSync(State currentState)
    {
      currentStateMessage.Update(currentState);
      SendToServer(currentStateMessage);
    }
  }
}
