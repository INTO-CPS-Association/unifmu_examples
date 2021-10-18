using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatbedTemperature : MonoBehaviour
{
    public GameObject heatbed;
    public float temperature = 10f;
    Renderer heatbedRenderer;
    // Start is called before the first frame update
    void Start()
    {
        heatbedRenderer = heatbed.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Color c = GetTemperatureColor(temperature);
        heatbedRenderer.material.SetColor("_Color", c);
    }


    private Color GetTemperatureColor(double tempC)
    {
        float minTemp = 10;
        float maxTemp = 60;
        float diffTemp = maxTemp - minTemp;
        float tempFactor = (float)tempC/diffTemp;
        return Color.Lerp(Color.cyan, Color.red, tempFactor);
    }
}
