using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanController : MonoBehaviour
{
    public GameObject fan;
    public float speed = 2000f;

    
    // Start is called before the first frame update
    void Start()
    {  
        fan.transform.Rotate(0,0,(float)speed* Time.deltaTime);
    }

    // Update is called once per frame
    void Update()
    {
        fan.transform.Rotate(0,0,(float)speed* Time.deltaTime);
    }
}
