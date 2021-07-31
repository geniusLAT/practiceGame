using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int Damage;
   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public IEnumerator Shoot(Vector3 where)
    {
        transform.LookAt(where);
        GetComponent<Rigidbody>().AddForce(transform.forward);
        for (int i = 0; i < 250; i++)
        {
            yield return null;
        }
       
        
    }
    public IEnumerator Shoot()
    {
        GetComponent<Rigidbody>().AddForce(transform.forward);
        for (int i = 0; i < 250; i++)
        {
            yield return null;
        }
        try
        {
            if (gameObject != null)
            {
                Destroy(gameObject);
            }
        }
        catch (System.Exception)
        {
            Debug.Log("That bug exists");
           // throw;
        }
       
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.transform.name);

        GameObject Target = collision.gameObject;

        if (Target.GetComponent<Character>())
        {
            Target.GetComponent<Character>().GetShot(Damage,transform.position);
        }

        if (Target.GetComponent<Rigidbody>())
        {
            Target.GetComponent<Rigidbody>().AddForce(transform.forward*300);
        }
        if (Target.GetComponent<Destroyable>())
        {
            Target.GetComponent<Destroyable>().GetDamage(20);
        }



        StopAllCoroutines();
        Destroy(gameObject);
    }
}
