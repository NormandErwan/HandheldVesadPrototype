using DevicesSyncUnity;
using DevicesSyncUnity.Messages;
using UnityEngine;
using UnityEngine.Networking;

namespace NormandErwan.MasterThesis.Experiment.Experiment.Task.Sync
{
  public class GridSync : DevicesSyncInterval
  {
    // Editor fields

    [SerializeField]
    private Grid grid;

    [SerializeField]
    private float movementThresholdToSync;

    // Properties
    
    public Grid Grid { get { return grid; } set { grid = value; } }
    public float MovementThresholdToSync { get { return movementThresholdToSync; } set { movementThresholdToSync = value; } }

    // Variables

    protected GridSyncConfigureMessage gridConfigureMessage;
    protected GridSyncTransformMessage gridTransformMessage = new GridSyncTransformMessage();
    protected GridSyncEventsMessage gridEventsMessage;

    // MonoBehaviour methods

    protected override void Awake()
    {
      base.Awake();

      gridConfigureMessage = new GridSyncConfigureMessage(Grid, () =>
      {
        SendToServer(gridConfigureMessage, Channels.DefaultReliable);
      });

      gridEventsMessage = new GridSyncEventsMessage(Grid, () =>
      {
        SendToServer(gridEventsMessage, Channels.DefaultReliable);
      });

      MessageTypes.Add(gridConfigureMessage.MessageType);
      MessageTypes.Add(gridTransformMessage.MessageType);
      MessageTypes.Add(gridEventsMessage.MessageType);
    }

    // DevicesSync methods

    protected override void OnSendToServerIntervalIteration(bool shouldSendThisFrame)
    {
      if (shouldSendThisFrame && Grid.IsConfigured)
      {
        gridTransformMessage.Update(Grid, MovementThresholdToSync);
        if (gridTransformMessage.transformChanged)
        {
          SendToServer(gridTransformMessage);
        }
      }
    }

    protected override DevicesSyncMessage OnServerMessageReceived(NetworkMessage netMessage)
    {
      return ProcessReceivedMessage(netMessage, false);
    }

    protected override DevicesSyncMessage OnClientMessageReceived(NetworkMessage netMessage)
    {
      return ProcessReceivedMessage(netMessage, true);
    }

    // Methods

    protected virtual DevicesSyncMessage ProcessReceivedMessage(NetworkMessage netMessage, bool onClient)
    {
      DevicesSyncMessage devicesSyncMessage = null;
      if (!onClient || (onClient && !isServer))
      {
        devicesSyncMessage = ProcessReceivedMessage<GridSyncConfigureMessage>(netMessage, gridConfigureMessage.MessageType,
          (gridConfigureMessage) =>
          {
            gridConfigureMessage.ConfigureGrid(Grid);
          });

        devicesSyncMessage = ProcessReceivedMessage<GridSyncTransformMessage>(netMessage, gridTransformMessage.MessageType,
          (gridTransformMessage) =>
          {
            gridTransformMessage.Restore(Grid);
          });

        devicesSyncMessage = ProcessReceivedMessage<GridSyncEventsMessage>(netMessage, gridEventsMessage.MessageType,
          (gridTransformMessage) =>
          {
            gridTransformMessage.SyncGrid(Grid);
          });
      }
      return devicesSyncMessage;
    }
  }
}
