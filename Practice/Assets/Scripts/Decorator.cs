using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decorator : MonoBehaviour
{
    public GameObject[] decors;
    // Start is called before the first frame update
    void Start()
    {
        
    }

   public virtual void Decorate(Architector.Room room)
    {
        Debug.Log("Abstract Decoration");
        Destroy(gameObject);
    }
   
}
