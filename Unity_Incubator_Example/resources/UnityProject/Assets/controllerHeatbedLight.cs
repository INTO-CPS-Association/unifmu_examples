using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controllerHeatbedLight : MonoBehaviour
{
    public GameObject controllerLight;
    Renderer lightRenderer;
    public bool lightOn = false;
    // Start is called before the first frame update
    void Start()
    {
        lightRenderer = controllerLight.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (lightOn == false)
        {
            lightRenderer.material.SetColor("_Color", Color.red);
        }
        else
        {
            lightRenderer.material.SetColor("_Color", Color.green);
        }
    }
}
