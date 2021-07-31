using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Printer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(transform.name+" "+transform.position+ " "+transform.rotation);
       
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(transform.name + " " + transform.position + " " + transform.rotation);

    }
}
