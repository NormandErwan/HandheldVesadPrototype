using System;
using UnityEngine;
using UnityEngine.UI;

namespace NormandErwan.MasterThesis.Experiment.UI.HUD
{
  public class MobileDeviceHUD : MonoBehaviour
  {
    // Editor Fields

    [SerializeField]
    private Button validateButton;

    // Events

    public event Action ValidateButtonPressed = delegate { };

    // Methods

    public virtual void ShowValidateButton(bool value)
    {
      validateButton.gameObject.SetActive(value);
    }

    protected virtual void OnEnable()
    {
      validateButton.onClick.AddListener(validateButton_onClick);
    }

    protected virtual void OnDisable()
    {
      validateButton.onClick.AddListener(validateButton_onClick);
    }

    protected virtual void Start()
    {
      ShowValidateButton(false);
    }

    protected virtual void validateButton_onClick()
    {
      ValidateButtonPressed();
    }
  }
}
