using UnityEngine;
using UnityEngine.Networking;

namespace NormandErwan.MasterThesisExperiment.Utilities
{
  public class NetworkAutoConnect : MonoBehaviour
  {
    public enum Type
    {
      Host,
      Client,
      Server
    }

    // Editor field

    public Type type;

    // Methods

    protected void Start()
    {
      var manager = NetworkManager.singleton;

      switch (type)
      {
        case Type.Host: manager.StartHost(); break;
        case Type.Client: manager.StartClient(); break;
        case Type.Server: manager.StartServer(); break;
      }
    }
  }
}
