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
    protected GridSyncTransformMessage gridTransformMessage;
    protected GridSyncEventsMessage gridEventsMessage;

    // MonoBehaviour methods

    protected override void Awake()
    {
      base.Awake();

      gridConfigureMessage = new GridSyncConfigureMessage(Grid, () =>
      {
        SendToServer(gridConfigureMessage, Channels.DefaultReliable);
      });
      gridTransformMessage = new GridSyncTransformMessage();
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
      if (!onClient || (onClient && !isServer))
      {
        GridSyncConfigureMessage gridConfigureReceived;
        if (TryReadMessage(netMessage, gridConfigureMessage.MessageType, out gridConfigureReceived))
        {
          gridConfigureReceived.ConfigureGrid(Grid);
          return gridConfigureReceived;
        }

        GridSyncTransformMessage gridTransformReceived;
        if (TryReadMessage(netMessage, gridTransformMessage.MessageType, out gridTransformReceived))
        {
          gridTransformReceived.Restore(Grid);
          return gridTransformReceived;
        }

        GridSyncEventsMessage gridEventsReceived;
        if (TryReadMessage(netMessage, gridEventsMessage.MessageType, out gridEventsReceived))
        {
          gridEventsReceived.SyncGrid(Grid);
          return gridEventsReceived;
        }
      }
      return null;
    }
  }
}
