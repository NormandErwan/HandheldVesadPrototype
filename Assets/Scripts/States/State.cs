using UnityEngine;

namespace NormandErwan.MasterThesisExperiment.States
{
    public class State : MonoBehaviour
    {
        // Editor fields

        public string id;
        public string title;

        [TextArea]
        public string instructions;
    }
}
