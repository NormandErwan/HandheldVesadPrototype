using DevicesSyncUnity;
using DevicesSyncUnity.Messages;
using UnityEngine;
using UnityEngine.Networking;

namespace NormandErwan.MasterThesis.Experiment.Experiment.Task.Sync
{
  public class GridSync : DevicesSync
  {
    // Editor fields

    [SerializeField]
    private Grid grid;

    // Properties
    
    public Grid Grid { get { return grid; } set { grid = value; } }

    // Variables

    protected GridSyncConfigureMessage gridConfigureMessage;
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
      MessageTypes.Add(gridEventsMessage.MessageType);
    }

    // DevicesSync methods

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
