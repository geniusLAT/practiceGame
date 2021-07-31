using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public class Door : Interactive
{
    public bool IsOpened;
    public bool IsLocked;
    public Coroutine coroutine;
    float NormalRot;
    Quaternion rot;
    // Start is called before the first frame update
    void Start()
    {
        GetStarted();
        NormalRot = transform.eulerAngles.y;
        rot = transform.rotation;
        Lock(IsLocked);
    }
    public void Lock(bool State)
    {
        Debug.Log("U");
        if (State)
        {
            IsLocked = GetComponent<Rigidbody>().isKinematic = true;
            transform.rotation = rot;
            GetComponent<NavMeshObstacle>().enabled = true;
        }
        else
        {
            IsLocked = GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<NavMeshObstacle>().enabled = false;
        }
    } 
    // Update is called once per frame
    void Update()
    {
       
    }
  
    public  bool Check()
    {
      
        if (Mathf.Abs(transform.eulerAngles.y - NormalRot) < 5)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public override void GetUsed(Character User)
    {
        
        base.GetUsed(User);

        if (!IsLocked)
        {
            IsOpened = !Check();
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            if (IsOpened)
            {
               
                coroutine = StartCoroutine(Close());
                
            }
            else
            {
                coroutine = StartCoroutine(Open());

                
            }
        }
       
    }
    IEnumerator Open()
    {
        HingeJoint joint = GetComponent<HingeJoint>();
        joint.useSpring = false;
        GetComponent<Rigidbody>().AddForce(-transform.forward * 200);
        yield return null;
        
    }
    IEnumerator Close()
    {
        HingeJoint joint = GetComponent<HingeJoint>();
        joint.useSpring = true;
        int z = 0;
        while ((!Check()) && (z < 300))
        {
            z++;
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForSeconds(3);
        GetComponent<Rigidbody>().isKinematic = joint.useSpring  =false;
        transform.rotation = rot;

    }
    IEnumerator BeInvisible()
    {
        GetComponent<BoxCollider>().enabled = false;
        yield return Open();
        yield return new WaitForSeconds(2);
        yield return Close();
        GetComponent<BoxCollider>().enabled = true;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.GetComponent<Enemy>())
        {
            if (collision.transform.GetComponent<Character>().HP>0)
            {
                StartCoroutine(BeInvisible());
            }
            
        }
    }
}
