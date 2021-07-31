using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tracker : MonoBehaviour
{
    public Architector architector;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       // Debug.Log(transform.rotation);
        Debug.Log("pos "+ architector.ArCor(transform.position)+"   "+ transform.position);
    }
}
