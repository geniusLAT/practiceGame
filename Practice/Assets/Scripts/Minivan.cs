using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minivan : MonoBehaviour
{
    public GameObject LeftDoor;
    public GameObject LeftDoorClosed;
    public GameObject LeftDoorOpened;
    public GameObject Inside;
    public GameObject Outside;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    public IEnumerator Move(Transform Thing,  Vector3 finish, int frames) 
    {
        Vector3 Start = Thing.transform.position;
        Vector3 Delta = (finish - Start) / 15;
        for (int i = 0; i < 15; i++)
        {
           Thing.position = Thing.position + Delta;
            yield return null;
        }
        yield return null;
    }
    public IEnumerator OpenDoors()
    {

        yield return Move(LeftDoor.transform, LeftDoorOpened.transform.position, 30);
        yield return null;
    }
    public IEnumerator CloseDoors()
    {

        yield return Move(LeftDoor.transform, LeftDoorClosed.transform.position, 30);
        yield return null;
    }
    // Update is called once per frame
    void Update()
    {
        Debug.Log("rot " + transform.rotation);
    }
}
