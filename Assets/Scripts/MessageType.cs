using System.Collections.Generic;
using UnityEngine.Networking;

namespace NormandErwan.MasterThesis.Experiment
{
  /// <summary>
  /// Container class for networking messages types used in this experiment.
  /// </summary>
  public class MessageType : DevicesSyncUnity.Messages.MessageType
  {
    // Constants

    /// <summary>
    /// See <see cref="DevicesSyncUnity.Messages.MessageType.Smallest"/>.
    /// </summary>
    public static new short Smallest { get { return smallest; } set { smallest = value; } }

    /// <summary>
    /// Networking message for communicating <see cref="States.StateManagerMessage"/>
    /// </summary>
    public static short StateManager { get { return (short)(Smallest + 1); } }

    /// <summary>
    /// Networking message for communicating <see cref="Variables.IndependentVariablesMessage"/>
    /// </summary>
    public static short IndependentVariables { get { return (short)(Smallest + 2); } }

    public static short DeviceController { get { return (short)(Smallest + 3); } }

    public static short GridConfigure { get { return (short)(Smallest + 4); } }

    public static short GridEvents { get { return (short)(Smallest + 5); } }

    public static short GridDrag { get { return (short)(Smallest + 6); } }

    public static short GridZoom { get { return (short)(Smallest + 7); } }

    public static short ProjectedCursors { get { return (short)(Smallest + 8); } }

    /// <summary>
    /// See <see cref="DevicesSyncUnity.Messages.MessageType.Highest"/>.
    /// </summary>
    public static new short Highest { get { return ProjectedCursors; } }

    // Variables

    private static short smallest = (short)(DevicesSyncUnity.Messages.MessageType.Highest + 1);
    private static Dictionary<short, string> messageTypeStrings = new Dictionary<short, string>()
    {
      { StateManager, "State" },
      { IndependentVariables, "IndependentVariables" },
      { DeviceController, "DeviceController" },
      { GridConfigure, "GridConfigure" },
      { GridEvents, "GridEvents" },
      { GridDrag, "GridDrag" },
      { GridZoom, "GridZoom" },
      { ProjectedCursors, "ProjectedCursors" },
    };

    // Methods

    /// <summary>
    /// See <see cref="MsgType.MsgTypeToString(short)"/>.
    /// </summary>
    public static new string MsgTypeToString(short value)
    {
      string messageTypeString;
      if (messageTypeStrings.TryGetValue(value, out messageTypeString))
      {
        return messageTypeString;
      }
      return MsgType.MsgTypeToString(value);
    }
  }
}
