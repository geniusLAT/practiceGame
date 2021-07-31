using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactive : MonoBehaviour
{
    protected LevelManager levelManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
       
    }

    protected void GetStarted()
    {
        levelManager = GameObject.FindWithTag("Manager").GetComponent<LevelManager>();

        levelManager.interactives.Add(this);
    }

    public virtual void GetUsed(Character User)
    {
        Debug.Log(transform.name + " was used by " + User.transform.name);
    }

    private void OnDestroy()
    {
        Debug.Log("HHHHHHHHHHHHHHHH");
        levelManager.interactives.Remove(this);
    }
}
