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

    protected GridSyncConfigureMessage gridConfigureMessage = new GridSyncConfigureMessage();
    protected GridSyncTransformMessage gridTransformMessage = new GridSyncTransformMessage();

    // MonoBehaviour methods

    protected override void Awake()
    {
      base.Awake();

      MessageTypes.Add(gridConfigureMessage.MessageType);
      MessageTypes.Add(gridTransformMessage.MessageType);

      Grid.ConfigureSync += Grid_ConfigureSync;
      Grid.CompleteSync += Grid_CompleteSync;
    }

    protected virtual void OnDestroy()
    {
      Grid.ConfigureSync -= Grid_ConfigureSync;
      Grid.CompleteSync -= Grid_CompleteSync;
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

    protected virtual void Grid_ConfigureSync()
    {
      SendToServer(gridConfigureMessage);
    }

    protected virtual void Grid_CompleteSync()
    {
      gridTransformMessage.completed = true;
      SendToServer(gridTransformMessage);
    }

    protected virtual DevicesSyncMessage ProcessReceivedMessage(NetworkMessage netMessage, bool onClient)
    {
      DevicesSyncMessage devicesSyncMessage = null;
      if (!onClient || (onClient && !isServer))
      {
        devicesSyncMessage = ProcessReceivedMessage<GridSyncConfigureMessage>(netMessage, gridConfigureMessage.MessageType,
          (gridConfigureMessage) =>
          {
            grid.SetConfiguration();
          });

        devicesSyncMessage = ProcessReceivedMessage<GridSyncTransformMessage>(netMessage, gridTransformMessage.MessageType,
          (gridTransformMessage) =>
          {
            gridTransformMessage.Restore(Grid);
          });
      }
      return devicesSyncMessage;
    }
  }
}
