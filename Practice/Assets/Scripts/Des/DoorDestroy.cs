using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorDestroy : Destroyable
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public override void Destruction()
    {
        base.Destruction();
        HingeJoint joint = GetComponent<HingeJoint>();
        Destroy(joint);
        Door door = GetComponent<Door>();
        if (door.coroutine != null)
        {
            StopCoroutine(door.coroutine);

        }
        Destroy(door);
        StartCoroutine(BecameDecor());
        transform.parent = null;
    }
    IEnumerator BecameDecor()
    {
        while( (Mathf.Abs(transform.eulerAngles.x-270)>3) && (Mathf.Abs(transform.eulerAngles.x - 90) > 3))
        {
            yield return new WaitForSeconds(1);
            Debug.Log("Not the Decor");
            Debug.Log(((transform.eulerAngles.x -90)));
            //Debug.Log((Mathf.Abs(transform.eulerAngles.x - 270) > 3));
        }
        Destroy(GetComponent<BoxCollider>());
        Destroy(GetComponent<Rigidbody>());
        yield return null;
        Debug.Log("The Decor");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
