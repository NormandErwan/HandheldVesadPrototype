using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
using NormandErwan.MasterThesis.Experiment.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Inputs.Cursors
{
  [RequireComponent(typeof(SphereCollider))]
  public class Cursor : BaseCursor
  {
    // Editor fields

    [SerializeField]
    private CursorType type;

    // ICursor properties

    public override CursorType Type { get { return type; } set { type = value; } }

    // Properties

    public bool IsTracked { get; set; }
    public float MaxSelectableDistance { get; set; }

    // Variables

    protected static Dictionary<ITransformable, Dictionary<Cursor, Vector3>> latestCursorPositions
      = new Dictionary<ITransformable, Dictionary<Cursor, Vector3>>();

    protected SortedDictionary<TriggerType, SortedDictionary<int, List<Collider>>> triggeredColliders;
    protected List<ICursorTriggerIInteractable> interactableTriggers;

    protected new Renderer renderer;
    protected new Collider collider;

    // MonoBehaviour methods

    protected virtual void Awake()
    {
      triggeredColliders = new SortedDictionary<TriggerType, SortedDictionary<int, List<Collider>>>();
      for (TriggerType triggerType = 0; triggerType < TriggerType.Count; triggerType++)
      {
        triggeredColliders.Add(triggerType, new SortedDictionary<int, List<Collider>>(new DescendingComparer<int>()));
      }

      interactableTriggers = new List<ICursorTriggerIInteractable>()
      {
        new CursorTriggerIFocusable() { Cursor = this },
        new CursorTriggerILongPressable() { Cursor = this },
        new CursorTriggerITappable() { Cursor = this },
        new CursorTriggerIZoomable() { Cursor = this },
        new CursorTriggerIDraggable() { Cursor = this },
      };

      renderer = GetComponent<Renderer>();
      collider = GetComponent<Collider>();
      SetVisible(false); // Set by CursorsInput every frame
      SetActive(false); // Set by DeviceController when TaskGrid is configured
    }

    protected virtual void Update()
    {
      foreach (var triggerTypes in triggeredColliders)
      {
        foreach (var colliders in triggerTypes.Value)
        {
          foreach (var collider in colliders.Value)
          {
            foreach (var trigger in interactableTriggers)
            {
              trigger.OnTrigger(triggerTypes.Key, collider);
            }
          }
          colliders.Value.Clear();
        }
      }

      foreach (var trigger in interactableTriggers)
      {
        trigger.ProcessPriorityLists();
      }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
      AddToTriggeredColliders(TriggerType.Enter, other);
    }

    protected virtual void OnTriggerStay(Collider other)
    {
      AddToTriggeredColliders(TriggerType.Stay, other);
    }

    protected virtual void OnTriggerExit(Collider other)
    {
      AddToTriggeredColliders(TriggerType.Exit, other);
    }

    // Methods

    public override void SetVisible(bool value)
    {
      base.SetVisible(value);
      renderer.enabled = IsVisible;
    }

    public override void SetActive(bool value)
    {
      base.SetActive(value);
      collider.enabled = IsActive;
    }

    protected virtual void AddToTriggeredColliders(TriggerType triggerType, Collider other)
    {
      var interactable = other.GetComponent<IInteractable>();
      if (interactable != null)
      {
        if (!triggeredColliders[triggerType].ContainsKey(interactable.Priority))
        {
          triggeredColliders[triggerType].Add(interactable.Priority, new List<Collider>());
        }
        triggeredColliders[triggerType][interactable.Priority].Add(other);
      }
    }
  }
}