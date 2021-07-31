using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chair : Destroyable
{
    bool OriginalDestruct;
    // Start is called before the first frame update
    void Start()
    {
           
    }
    public override void Destruction()
    {
        Debug.Log("UF");
        base.Destruction();
        while (transform.childCount > 0)
        {
            Transform part = transform.GetChild(0);
            part.parent = null;
            part.gameObject.AddComponent<Rigidbody>();
        }
        if (OriginalDestruct)
        {
            GetComponent<BoxCollider>().enabled = false;
        }
    }
   
    // Update is called once per frame
    void Update()
    {
        
    }
}
