using UnityEngine.Networking;

namespace NormandErwan.MasterThesisExperiment
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
        /// Networking message for communicating <see cref="States.StateMessage"/>
        /// </summary>
        public static short State { get { return (short)(Smallest + 1); } }

        /// <summary>
        /// See <see cref="DevicesSyncUnity.Messages.MessageType.Highest"/>.
        /// </summary>
        public static new short Highest { get { return State; } }

        // Variables

        private static short smallest = (short)(DevicesSyncUnity.Examples.Messages.MessageType.Highest + 1);

        // Methods

        /// <summary>
        /// See <see cref="MsgType.MsgTypeToString(short)"/>.
        /// </summary>
        public static new string MsgTypeToString(short value)
        {
            if (value == State)
            {
                return "State";
            }
            else
            {
                return MsgType.MsgTypeToString(value);
            }
        }
    }
}
