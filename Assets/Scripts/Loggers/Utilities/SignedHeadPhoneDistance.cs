using NormandErwan.MasterThesis.Experiment.Experiment;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Loggers.Utilities
{
  public class SignedHeadPhoneDistance : Counter<Vector3>
  {
    public SignedHeadPhoneDistance(MobileDeviceTracking phone, Transform head) : base()
    {
      Phone = phone;
      Head = head;
    }

    // Properties

    public MobileDeviceTracking Phone { get; protected set; }
    public Transform Head { get; protected set; }

    // Variables

    protected Vector3 previous;

    // Methods

    protected override bool UpdateThisFrame()
    {
      return Phone.IsTracking;
    }

    protected override Vector3 GetCurrent()
    {
      return Phone.transform.position - Head.position;
    }

    protected override void UpdateTotal()
    {
      Total += Current - Previous;
    }
  }
}
