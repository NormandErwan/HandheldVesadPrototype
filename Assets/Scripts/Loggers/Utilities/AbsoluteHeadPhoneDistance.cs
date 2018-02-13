using NormandErwan.MasterThesis.Experiment.Experiment;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Loggers.Utilities
{
  public class AbsoluteHeadPhoneDistance : PositionDistance
  {
    public AbsoluteHeadPhoneDistance(MobileDeviceTracking phone, Transform head) : base()
    {
      Phone = phone;
      Head = head;
    }

    public MobileDeviceTracking Phone { get; protected set; }
    public Transform Head { get; protected set; }

    protected override bool UpdateThisFrame()
    {
      return Phone.IsTracking;
    }

    protected override Vector3 GetCurrentPosition()
    {
      return Phone.transform.position - Head.position;
    }
  }
}
