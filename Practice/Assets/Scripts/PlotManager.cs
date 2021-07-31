using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotManager : MonoBehaviour
{
    public EnemyManager enemyManager;
    public LevelManager levelManager;
    public GameObject MinivanPref;
    public int Hard =400;
    public GameObject Player;
    Coroutine Task;
    public AudioClip[] audios;
    AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        

    }
    public void NewLocation()
    {
        enemyManager = GameObject.FindGameObjectsWithTag("EnemyManager")[0].GetComponent<EnemyManager>();
        float likeliness = Random.Range(0, 100);
        Player = levelManager.player.gameObject;
        source = GetComponent<AudioSource>();
        if (Task != null)
        {
            StopCoroutine(Task);
        }
        Task = StartCoroutine(WaitOverKill());

    }

    IEnumerator WaitOverKill()
    {
        
        source.clip = audios[0]; source.Play();
        GameObject van = Instantiate(MinivanPref);
        Minivan minivan = van.GetComponent<Minivan>();
        van.transform.position = new Vector3(79.9f, 2.2f, -14.3f);
        van.transform.Rotate(new Vector3(0, 45));
        Player.GetComponent<PlayerControl>().Freedom = Player.GetComponent<PlayerControl>().CursorLook = false;
        Player.transform.position = minivan.Inside.transform.position;
        yield return new WaitForSeconds(0.1f);



        yield return new WaitForSeconds(0.1f);
        yield return minivan.OpenDoors();
        yield return minivan.CloseDoors();
        Player.GetComponent<Rigidbody>().isKinematic = true;
        yield return minivan.Move(Player.transform, minivan.Outside.transform.position, 120);
        Player.GetComponent<PlayerControl>().Freedom = Player.GetComponent<PlayerControl>().CursorLook = true;
        Player.GetComponent<Rigidbody>().isKinematic = false;
        while (enemyManager.soldeirs.ToArray().Length > 0)
        {
            yield return null;
        }
        source.clip = audios[1]; source.Play();
        while (van!=null&&(Vector3.Distance(van.transform.position, Player.transform.position) > 20))
        {
            yield return null;
        }
        yield return minivan.OpenDoors();
        while (Vector3.Distance(minivan.Outside.transform.position, Player.transform.position) > 5)
        {
            yield return null;
        }
        yield return minivan.Move(Player.transform, minivan.Outside.transform.position, 120);
        yield return minivan.Move(Player.transform, minivan.Inside.transform.position, 30);
        yield return minivan.CloseDoors();

        DontDestroyOnLoad(gameObject);
        yield return new WaitForSeconds(2);
        Player.GetComponent<Character>().HP = -1;
        Hard += 200;
        Destroy(GameObject.FindGameObjectWithTag("tutorial"));
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
