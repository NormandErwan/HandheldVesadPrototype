using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
using System;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Inputs.Cursors
{
  [RequireComponent(typeof(Collider))]
  public class LookCursor : BaseCursor
  {
    // Editor fields

    [SerializeField]
    private Renderer mesh;

    // Properties

    public override CursorType Type { get { return CursorType.Look; } set { var dummy = value; } }
    public bool IsReady { get; protected set; }

    // Events

    public event Action<LookCursor> Ready = delegate { };
    public event Action<LookCursor> Tapped = delegate { };

    // Methods

    public void SetReady(bool value)
    {
      IsReady = true;
      Ready(this);
    }

    public void SetTapped()
    {
      Tapped(this);
    }
  }
}