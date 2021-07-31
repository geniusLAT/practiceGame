using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.Rotate(new Vector3(0, 90 * Random.Range(0, 5), 0));
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
