using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;


public class TutorialV2 : MonoBehaviour
{
    Architector ar;
    EnemyManager enemyManager;
    public GameObject Player;
    public Elevator elevator;
    public Transform CamCube;
    public GameObject FilePrefab;
    public GameObject MiniVanPref;
    public GameObject EnemyPref;
    public GUI GUI;
    public CamScriptV2 camScript;
    public GameObject TablePref;
    public AudioClip[] audios;
    AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        ar = GameObject.FindGameObjectWithTag("Architector").GetComponent<Architector>();
        Debug.Log("Scene is "+SceneManager.GetActiveScene().name);
        enemyManager = GameObject.FindGameObjectsWithTag("EnemyManager")[0].GetComponent<EnemyManager>();
        source = GetComponent<AudioSource>();

    }
    public IEnumerator TutorialCoroutine()
    {
        //Происходит загрузка локации, игрок пока находится в изолированном лифте
        yield return null;
        Player.GetComponent<Rigidbody>().useGravity = false;
        Player.GetComponent<NavMeshAgent>().enabled = Player.GetComponent<PlayerControl>().Freedom = Player.GetComponent<PlayerControl>().CursorLook = false;
        elevator.transform.position = new Vector3(1000, 1000, 1000);
        Player.transform.position = elevator.PlaceToTransport.position; Player.transform.rotation = elevator.PlaceToTransport.rotation;
        CamCube.transform.position = Player.transform.position;
        ar.PutFurniture(TablePref, 16, 15);

        //Ждём пока она загрузится
        while (!ar.Finished)
        {
            yield return null;

        }
        
        //Лифт должен приехать в нужном месте
        ar.cells[3][10].WestWall.SetActive(false);
        elevator.transform.position = new Vector3(30.27f, 5.74f, 104.57f);
        Player.transform.position = elevator.PlaceToTransport.position; Player.transform.rotation = elevator.PlaceToTransport.rotation;
        CamCube.transform.position = Player.transform.position;
        Player.GetComponent<Rigidbody>().useGravity = true;
        //Ждём когда игрок сменит масштаб камеры
        GUI.Center.Print("Вы можете сменить масштаб камеры клавишей Shift");
        while(!Input.GetKeyDown(KeyCode.LeftShift))
        {
            yield return null;
        }
        source.clip = audios[0]; source.Play();
        GUI.Center.Print("");


        Player.GetComponent<NavMeshAgent>().enabled = Player.GetComponent<PlayerControl>().Freedom = Player.GetComponent<PlayerControl>().CursorLook = true;


        //Появляются враги
        Vector3[] p = { new Vector3(74.6f,2.2f,108f), new Vector3(99.9f,2.2f,108), new Vector3(150.3f,2.2f,108), new Vector3(150.3f, 2.2f, 67.6f), new Vector3(147.3f,2.2f,154.7f), new Vector3(147.3f,2.2f,35.7f) };
        foreach (Vector3 place in p)
        {
            // GameObject guy = Instantiate(EnemyPref,place, new Quaternion());
            enemyManager.Spawn(place, 0);
        }
        yield return elevator.OpenDoors();

        GUI.Center.Print("Найдите разведданные");
        yield return new WaitForSeconds(4);
        GUI.Center.Print("");



        //Игрок ищет файл
        FileToShow file = Instantiate(FilePrefab).GetComponent<FileToShow>();
        file.transform.position = new Vector3(162.52f, 7.09f, 152.65f);

        while (Vector3.Distance(file.transform.position, Player.transform.position)>40)
        {
            yield return null;
        }
        GUI.Center.Print("Вы можете открыть папку клавишей Е, находясь рядом с ней.");
        source.clip = audios[1]; source.Play();
        while (!file.Showed)
        {
            yield return null;
        }
        GUI.Center.Print("");

        //Нашли новый файл

        source.clip = audios[2]; source.Play();
        GameObject van = Instantiate(MiniVanPref);
        Minivan minivan = van.GetComponent<Minivan>();
        camScript.Subject = van.transform;
        van.transform.position = new Vector3(260.3f, 4.5f, 126.2f);
        Vector3 PlaceToStop= new Vector3(231.9f,2.2f,104.0f);
        Vector3 Del = (PlaceToStop - van.transform.position) / 40;
        van.GetComponent<Rigidbody>().isKinematic = true;
        for (int i = 0; i < 40; i++)
        {
            van.transform.position = van.transform.position + Del;
           
            yield return null;
        }
        van.GetComponent<Rigidbody>().isKinematic = false;

        yield return new WaitForSeconds(4);
        yield return minivan.OpenDoors();

        enemyManager.state = EnemyManager.State.Search;
        enemyManager.InitiateSearch();
        camScript.Subject = Player.transform;
        for (int i = 0; i < 6; i++)
        {
            enemyManager.Spawn(minivan.Inside.transform.position, 0);
            
            yield return new WaitForSeconds(2);
        }
        yield return minivan.CloseDoors();

        GUI.Center.Print("Зачистите уровень");
        
        yield return new WaitForSeconds(2);
        GUI.Center.Print("");

        while (enemyManager.soldeirs.ToArray().Length > 0)
        {
            yield return null;
        }
        source.clip = audios[3]; source.Play();

        GUI.Center.Print("Садитесь в машину");
        yield return new WaitForSeconds(2);
        GUI.Center.Print("");


        while (Vector3.Distance(van.transform.position, Player.transform.position) > 14)
        {
            yield return null;
        }
        
        GUI.Center.Print("туториал завершён");
        Player.GetComponent<PlayerControl>().Freedom = Player.GetComponent<PlayerControl>().CursorLook = false;
        Player.GetComponent<NavMeshAgent>().destination = minivan.Outside.transform.position;
        yield return minivan.OpenDoors();

        //Dialog
        source.clip = audios[4]; source.Play();
        yield return new WaitForSeconds(20);

        Player.GetComponent<Character>().HP = -1;
        Destroy(GameObject.FindGameObjectWithTag("tutorial"));
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
