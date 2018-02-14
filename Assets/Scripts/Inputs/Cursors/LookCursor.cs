using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Inputs.Cursors
{
  [RequireComponent(typeof(Collider))]
  public class LookCursor : BaseCursor
  {
    // Editor fields

    [SerializeField]
    private Renderer mesh;

    // ICursor properties

    public override CursorType Type { get { return CursorType.Look; } set { var dummy = value; } }

    // Methods


  }
}