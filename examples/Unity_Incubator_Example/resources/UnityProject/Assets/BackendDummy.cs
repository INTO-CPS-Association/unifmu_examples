using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackendDummy : MonoBehaviour
{
    private int i = 0;
    public Model model;
    // Start is called before the first frame update
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    void Start()
    {
        System.Environment.SetEnvironmentVariable("UNIFMU_REFS_TO_ATTRS", "{\"0\": \"T_heater_in\", \"1\": \"T_bair_in\", \"2\": \"heater_on_in\"}");
        var references = new List<uint>{0,1};
        var values = new List<double>{50f, 25f};
        model.Fmi2SetReal(references, values);
        var breferences = new List<uint>{2};
        var bvalues = new List<bool>{false};
        model.Fmi2SetBoolean(breferences, bvalues);
    }

    // Update is called once per frame
    void Update()
    {
        i++;
        if (i == 200)
        {
            var references = new List<uint>{0,1};
            var values = new List<double>{60f, 30f};
            model.Fmi2SetReal(references, values);
            var breferences = new List<uint>{2};
            var bvalues = new List<bool>{true};
            model.Fmi2SetBoolean(breferences, bvalues);
        }
    }
}
