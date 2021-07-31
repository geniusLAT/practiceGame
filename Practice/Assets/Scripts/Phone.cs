using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Phone : Interactive
{
    public GameObject Place;
    public GameObject PhoneT;
    public bool IsUsed;
    Coroutine Calling;
    AudioSource source;
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        GetStarted();
    }
    public void Call() {
        Calling = StartCoroutine(BeginCalling());
    }
    public override void GetUsed(Character User)
    {
        if (Calling != null)
        {
            StopCoroutine(Calling);
            source.Stop();
        }
        User.transform.position = Place.transform.position;
        IsUsed = true;
        base.GetUsed(User);
    }
    IEnumerator BeginCalling() {
        while (true)
        {
            source.Play();
            yield return new WaitForSeconds(2.5f);
            
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
