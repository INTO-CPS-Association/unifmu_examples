using System.Collections.Generic;
using UnityEngine;

public class BackendDummy : MonoBehaviour
{
    public Model model;
    // Start is called before the first frame update
    // [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    void Start()
    {
        // System.Environment.SetEnvironmentVariable("UNIFMU_REFS_TO_ATTRS", "{\"0\": \"x\", \"1\": \"y\", \"2\": \"z\"}");
        var references = new List<uint>{0,1,2};
        var values = new List<double>{2f, 2f, 0f};
        model.Fmi2SetReal(references, values);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
