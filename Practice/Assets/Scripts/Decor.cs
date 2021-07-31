using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decor : MonoBehaviour
{
    public GameObject[] parts;
    public float U;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(D());
    }
    IEnumerator D()
    {
        yield return new WaitForSeconds(0.2f);
        foreach (GameObject part in parts)
        {
            if (Random.Range(0, U) < 1)
            {
                part.transform.parent = null;
            }
            else
            {
                Destroy(part);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
