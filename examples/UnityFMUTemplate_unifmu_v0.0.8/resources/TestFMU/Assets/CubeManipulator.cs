using UnityEngine;

public class CubeManipulator : MonoBehaviour
{
    public GameObject cube;
    public Vector3 position;
    // Start is called before the first frame update
    void Start()
    {
        position = cube.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
