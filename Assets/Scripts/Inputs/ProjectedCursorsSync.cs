using DevicesSyncUnity;
using DevicesSyncUnity.Messages;
using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace NormandErwan.MasterThesis.Experiment.Inputs
{
  public class ProjectedCursorsSync : DevicesSync
  {
    // Editor fields

    [SerializeField]
    private CursorsInput cursorsInput;

    [SerializeField]
    private ProjectedCursor[] projectedCursors;

    // Properties

    public Dictionary<CursorType, ProjectedCursor> ProjectedCursors { get; protected set; }

    // Variables

    protected ProjectedCursorsSyncMessage projectedCursorsMessage;

    // MonoBehaviour methods

    protected override void Awake()
    {
      base.Awake();

      projectedCursorsMessage = new ProjectedCursorsSyncMessage();
      projectedCursorsMessage.cursors = new CursorType[projectedCursors.Length];
      projectedCursorsMessage.isActive = new bool[projectedCursors.Length];
      projectedCursorsMessage.localPositions = new Vector3[projectedCursors.Length];
      MessageTypes.Add(projectedCursorsMessage.MessageType);

      ProjectedCursors = new Dictionary<CursorType, ProjectedCursor>(projectedCursors.Length);
      for (int i = 0; i < projectedCursors.Length; i++)
      {
        projectedCursorsMessage.cursors[i] = projectedCursors[i].Cursor.Type;
        ProjectedCursors.Add(projectedCursors[i].Cursor.Type, projectedCursors[i]);
      }

      cursorsInput.Updated += CursorsInput_Updated;
    }

    protected virtual void OnDestroy()
    {
      cursorsInput.Updated -= CursorsInput_Updated;
    }

    // DevicesSync methods

    protected override DevicesSyncMessage OnServerMessageReceived(NetworkMessage netMessage)
    {
      return netMessage.ReadMessage<ProjectedCursorsSyncMessage>();
    }

    protected override DevicesSyncMessage OnClientMessageReceived(NetworkMessage netMessage)
    {
      var projectedCursorsReceived = netMessage.ReadMessage<ProjectedCursorsSyncMessage>();
      if (projectedCursorsReceived.SenderConnectionId != NetworkManager.client.connection.connectionId)
      {
        projectedCursorsReceived.Restore(ProjectedCursors);
        return projectedCursorsReceived;
      }
      return null;
    }

    // Methods

    protected virtual void CursorsInput_Updated()
    {
      foreach (var projectedCursor in ProjectedCursors)
      {
        projectedCursor.Value.UpdateProjection();
      }

      projectedCursorsMessage.Update(ProjectedCursors);
      if (projectedCursorsMessage.TransformChanged)
      {
        SendToServer(projectedCursorsMessage);
      }
    }
  }
}
