using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public bool CanShoot = true;
    public float TimePerShot;
    public Character Keeper;
    public int Ammo = 20;
    protected Rigidbody body;
    bool istaken;
    protected LevelManager levelManager;
    public AudioClip[] audios;
    public AudioSource audioPlayer;
    // Start is called before the first frame update
    void Start()
    {
        audioPlayer = GetComponent<AudioSource>();
        audioPlayer.clip = audios[1];
        body = GetComponent<Rigidbody>();
        levelManager = GameObject.FindWithTag("Manager").GetComponent<LevelManager>();
        if (Keeper == null)
        {
            levelManager.guns.Add(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public IEnumerator BeTaken(Character character)
    {
        yield return null;
        istaken = true;
        CanShoot = true;
        transform.position = character.RH.transform.position;
        Keeper = character;
        Keeper.RHobject = this.gameObject;
        body.isKinematic = true;
        levelManager.guns.Remove(this);
        GetComponent<BoxCollider>().enabled = false;
        yield return null;

        audioPlayer.clip = audios[0];
        Debug.Log("ZVUUUUUUUUUUUUUUUUK");
        audioPlayer.Play();
        yield return new WaitForSeconds(0.5f);
        audioPlayer.clip = audios[1];
    }

    public IEnumerator BeDropped()
    {
        istaken = false;
        if (Keeper != null)
        {
            Keeper.RHobject = null;
            Keeper = null;
        }
        GetComponent<BoxCollider>().enabled = true;
        levelManager.guns.Add(this);
        body.isKinematic = false;
        yield return null;
    }
    public virtual IEnumerator Atack(Vector3 target)
    {
        Debug.Log("Abstract Boom");
        yield return null;
    }
}
