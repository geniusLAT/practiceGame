using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : Interactive
{
    public bool CanBeCalled;
    public bool Opened;
    public float TimeToCame;
    public bool Transporting=false;
    public GameObject LeftDoor;
    public GameObject RightDoor;
    public Transform LeftOpened;
    public Transform LeftClosed;
    public Transform RightOpened;
    public Transform RightClosed;
    public Transform PlaceToTransport;
    public AudioClip[] audios;
    AudioSource source;
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        GetStarted();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public IEnumerator OpenDoors()
    {
        source.clip = audios[0];
        source.Play();
        Vector3 RightStart = RightDoor.transform.position; Vector3 LeftStart = LeftDoor.transform.position;
        Vector3 RightDel = (RightOpened.position - RightStart) / 15;
        Vector3 LeftDel = (LeftOpened.position - RightStart) / 15;
        for (int i = 0; i < 15; i++)
        {
            RightDoor.transform.position = RightDoor.transform.position + RightDel;
            LeftDoor.transform.position = LeftDoor.transform.position + LeftDel;
            yield return null;
        }
    }

    public IEnumerator CloseDoors()
    {
        Vector3 RightStart = RightDoor.transform.position; Vector3 LeftStart = LeftDoor.transform.position;
        Vector3 RightDel = (RightClosed.position - RightStart) / 15;
        Vector3 LeftDel = (LeftClosed.position - RightStart) / 15;
        for (int i = 0; i < 15; i++)
        {
            RightDoor.transform.position = RightDoor.transform.position + RightDel;
            LeftDoor.transform.position = LeftDoor.transform.position + LeftDel;
            yield return null;
        }
    }

    public override void GetUsed(Character User)
    {
        source.clip = audios[1];
        source.Play();
        Debug.Log("Elevator was used");
        if (CanBeCalled && !Opened)
        {
            StartCoroutine(Arrive(TimeToCame));
        } 
        if (CanBeCalled && Opened)
        {
            StartCoroutine(CloseDoors());
            Transporting = true;
        }
    }

    IEnumerator Arrive(float time)
    {
        yield return new WaitForSeconds(time);
        yield return OpenDoors();
        Opened = true;
    }
}
