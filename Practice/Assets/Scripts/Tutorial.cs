using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;



public class Tutorial : MonoBehaviour
{
    public GameObject Player;
    public Light lightAtRoom;
    public Phone phone;
    public Door RoomDoor;
    public Door AnotherDoor;
    public Reloader reloader;

    public AudioClip[] audios;
    AudioSource source;
    public Character DoorKnocker;
    public GUI GUI;
    public Enemy[] enemies;
    public Elevator[] elevators;
    public Texture PhoneImage;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(tutorial());
        source = GetComponent<AudioSource>();
        
    }

   



    IEnumerator tutorial()
    {
        //Темно
        yield return new WaitForSeconds(0.2f);
        phone.Call();
        lightAtRoom.enabled = false;
        Player.transform.position = new Vector3(-75, 3.7f, 87.3f);
        Player.transform.rotation = new Quaternion(0, -0.7f, -0.7f, 0);
        Player.GetComponent<NavMeshAgent>().enabled = false;
        Player.GetComponent<PlayerControl>().Freedom = false;
        Player.GetComponent<PlayerControl>().CursorLook = false;
        yield return new WaitForSeconds(0.8f);
        source.clip = audios[0]; source.Play();//Switch
        yield return new WaitForSeconds(0.4f);
        source.clip = audios[1]; source.Play();//Waking Up


        //Чел лежит в кровати
        lightAtRoom.enabled = true;
        yield return new WaitForSeconds(0.8f);

        //Игрок свободно идёт до телефона
        Player.transform.position = new Vector3(-70.8f, 2.1f, 87.3f);
        Player.GetComponent<NavMeshAgent>().enabled = Player.GetComponent<PlayerControl>().Freedom = Player.GetComponent<PlayerControl>().CursorLook = true;
        GUI.Center.Print("С помощью WASD подойдите к телефону, возьмите трубку с помощью E");

        while (!phone.IsUsed)
        {
            yield return null;
        }
        GUI.Center.Print("");
        GUI.ShowImage(PhoneImage,0);
        //Игрок взял трубку
        Player.GetComponent<NavMeshAgent>().enabled = Player.GetComponent<PlayerControl>().Freedom = Player.GetComponent<PlayerControl>().CursorLook = false;

        Player.GetComponent<Rigidbody>().freezeRotation = true;
        Player.GetComponent<Rigidbody>().isKinematic = true;
        Player.transform.rotation = phone.PhoneT.transform.rotation;
        phone.PhoneT.GetComponent<BoxCollider>().enabled = false;
        phone.PhoneT.transform.position = new Vector3(-86.5f, 5.1f, 84.8f);
        source.clip = audios[2]; source.Play();//Talking
        yield return new WaitForSeconds(21f);



        //Разговор кончился
        GUI.HideImage();
        Player.GetComponent<NavMeshAgent>().enabled = Player.GetComponent<PlayerControl>().Freedom = Player.GetComponent<PlayerControl>().CursorLook = true;
        phone.PhoneT.GetComponent<BoxCollider>().enabled = true;
        phone.PhoneT.AddComponent<Rigidbody>();
        phone.PhoneT.transform.parent = null;
        StartCoroutine(DoorKnocker.TakeNearestGun());
        DoorKnocker.GetComponent<NavMeshAgent>().speed = 70;
        DoorKnocker.GetComponent<NavMeshAgent>().destination = new Vector3(-108.6f, 2.72f, 64.03f);
        yield return new WaitForSeconds(2f);

        //В дверь стучат
        GUI.Center.Print("Незапертые двери открываются с помощью E, откройте дверь в номер");
        source.clip = audios[3]; source.Play();//Knocking
        RoomDoor.Lock(false);
        Debug.Log(RoomDoor.Check());
        
        while (RoomDoor.Check())
        {
            Debug.Log("Check");
            yield return null;
        }
       


        //Дверь открылась
        GUI.Center.Print("");
        
        DoorKnocker.GetComponent<NavMeshAgent>().stoppingDistance = 5f;
        Player.GetComponent<PlayerControl>().Freedom = Player.GetComponent<PlayerControl>().CursorLook = false;
        Player.GetComponent<NavMeshAgent>().destination = new Vector3(-104.5f, 1.4f, 68.7f);
        yield return new WaitForSeconds(0.7f);
        DoorKnocker.GetComponent<NavMeshAgent>().destination = Player.transform.position;


        //Игрок пришёл к вооружённому челику
        DoorKnocker.transform.LookAt(Player.transform);
        GUI.Center.Print("Атакуйте врага атакой ближнего боя  нажатием клавиши Q");
        while (!(Input.GetKey(KeyCode.Q)))
        {
            DoorKnocker.GetComponent<NavMeshAgent>().destination = Player.transform.position;
            yield return null;
        }
        yield return Player.GetComponent<PlayerControl>().Melee();
        Player.GetComponent<PlayerControl>().Freedom = Player.GetComponent<PlayerControl>().CursorLook = false;

        //Враг повержен
        yield return new WaitForSeconds(0.5f);
        GUI.Center.Print("Подберите упавший пистолет нажатием клавиши F");
        while (!(Input.GetKey(KeyCode.F)))
        {
            yield return null;
        }
        yield return Player.GetComponent<Character>().TakeNearestGun();
        Player.GetComponent<PlayerControl>().CursorLook = true;

        //Пистолет подобран, на подходе первый враг.
        enemies[0].gameObject.SetActive(true);
        GUI.Center.Print("Застрелите следующего врага нажатием левой клавиши мыши, используете мышь для прицела");
        Character he = enemies[0].GetComponent<Character>();
        while (he.HP > 1)
        {
            yield return null;
            if (Input.GetKey(KeyCode.Mouse0))
            {
                Transform Cursor = (Player.GetComponent<PlayerControl>().Cursor);
                Transform RH = Player.GetComponent<Character>().RH.transform;
                if (Vector3.Distance(transform.position, Cursor.position) > 6)
                {
                    RH.LookAt(Cursor);
                    Player.transform.rotation = new Quaternion(0, RH.rotation.y, 0, RH.rotation.w);
                   
                }


                {
                    StartCoroutine(Player.GetComponent<Character>().RHobject.GetComponent<Gun>().Atack(Cursor.position));


                }
            }
        }
        //Новый враг повержен
        Player.GetComponent<NavMeshAgent>().enabled = Player.GetComponent<PlayerControl>().Freedom = Player.GetComponent<PlayerControl>().CursorLook = true;
        GUI.Center.Print("Теперь нужно выбираться отсюда");
        yield return new WaitForSeconds(0.7f);
        GUI.Center.Print("");

        //Игрок идёт в сторону лифта
        GUI.Center.Print("");
        while (Player.transform.position.x>-200)
        {
            yield return null;
        }

        //Игрок дошёл до конца коридора
        enemies[1].gameObject.SetActive(true);
       
        enemies[3].gameObject.SetActive(true);
        AnotherDoor.Lock(false);
        GUI.Center.Print("Нужно вызвать лифт нажатием клавиши E");

        //Игрок вызвал лифт, а он приехал.
        while (!elevators[0].Opened)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.7f);
        GUI.Center.Print("");

        enemies[2].gameObject.SetActive(true);
        yield return new WaitForSeconds(0.7f);
        GUI.Center.Print("Лифт приехал, можно уехать на нём нажав клавишу Е внутри");

        //Игрок уехал на лифте
        while (!elevators[0].Transporting)
        {
            yield return null;
        }
        Player.GetComponent<NavMeshAgent>().enabled = Player.GetComponent<PlayerControl>().Freedom = Player.GetComponent<PlayerControl>().CursorLook = false;
        Player.transform.position = elevators[0].PlaceToTransport.position; Player.transform.rotation = elevators[0].PlaceToTransport.rotation;
        yield return new WaitForSeconds(0.7f);
        Player.transform.position = elevators[1].PlaceToTransport.position;
        reloader.Stop();
        yield return new WaitForSeconds(2f);
        DontDestroyOnLoad(this.gameObject);
        SceneManager.LoadScene("Generated", LoadSceneMode.Single);
        yield return new WaitForSeconds(8f);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
