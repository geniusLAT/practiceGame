using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyable : MonoBehaviour
{
    public int HP;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void Destruction()
    {
        Debug.Log(transform.name+" was destroyed");
    }


    public virtual void GetDamage(int Damage)
    {
        Debug.Log(transform.name + " was damaged");
        if (HP > 1)
        {
            Debug.Log("HP > 1");
            HP -= Damage;
            Debug.Log(HP);
            if (HP < 1)
            {
                Destruction();


            }
        }
    }
}
