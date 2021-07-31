using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adder : MonoBehaviour
{
    public float mass;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AddIt());
    }
    IEnumerator AddIt()
    {
        yield return null;
        while (transform.parent != null)
        {
            yield return new WaitForSeconds(2);
        }
        yield return new WaitForSeconds(2);
        gameObject.AddComponent<Rigidbody>().mass=mass;
        Destroy(this);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
