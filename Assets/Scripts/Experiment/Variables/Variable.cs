using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Experiment.Variables
{
  public abstract class Variable : MonoBehaviour
  {
    // Editor Fields

    [SerializeField]
    private string id;

    [SerializeField]
    private string name;

    [SerializeField]
    private string title;

    // Properties
    public string Id { get { return id; } protected set { id = value; } }
    public string Name { get { return name; } protected set { name = value; } }
    public string Title { get { return title; } protected set { title = value; } }
  }
}
