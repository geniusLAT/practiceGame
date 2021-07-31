using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public Architector architector;
    LevelManager levelManager;
    public List<Architector.Room> UnChecked;
    public bool ShouldPatrol = false;

    public enum State
    {
        Normal,
        Search,
        Battle

    }
    public List<Enemy> soldeirs;
    public Enemy Commander;
    public State state;
    public List<GameObject> sus;
    public GameObject[] gunPrefs;
    public GameObject enemyPref;
    Coroutine SearchCoroutine;
    Coroutine BattleCoroutine;
    public int PlayerAggression=0;

    enum BattlePlan
    {
        Wait, assault, Classic
        //В ожидании враги занимают соседние комнаты, стреляют из них. Wait
        //В штурме, все разом выбегают из соседних комнат в комнату игрока assault
        //В классическом режиме, выбирается один атакующий, остальные пытаются занять соседние комнаты
    }


    public void InitiateSearch()
    {
         UnChecked = new List<Architector.Room>();
        UnChecked.AddRange(architector.AllRooms.ToArray());
        if (SearchCoroutine == null)
        {
            SearchCoroutine = StartCoroutine(Search());
            Debug.Log("Начать поиски");
        }
        else
        {
            Debug.Log("Обыскать всё заново.");
           
        }
        
    }

    public void InitiateBattle()
    {
        if (SearchCoroutine != null)
        {
            StopCoroutine(SearchCoroutine);
        }
        state = State.Battle;
        BattleCoroutine = StartCoroutine(Battle());
      

    }

    IEnumerator Battle()
    {
        bool RoomChanged = false;
        BattlePlan plan = BattlePlan.Classic;
        GameObject player = sus[0];
        foreach (Enemy Soldeir in soldeirs)
        {
            Soldeir.Fury = true;
            Soldeir.Target = player.transform;
            Soldeir.Stop();
        }
        Enemy Rusher = null;

        yield return new WaitForSeconds(5f);
        Architector.Room LastRoom=null;
        Architector.Room[] nears= { };
        List<Architector.Room> empty;
        List<Vector3> roomplaces = new List<Vector3>();
        PlayerAggression = 1000;//Показатель агрессии игрока
        while (state == State.Battle)
        {
            Debug.Log("TacTic");
            PlayerAggression--;//Каждый тактический тик, если игрок ничего не делает, то его уровень агрессивности снижается
            int soldeirsCount = soldeirs.ToArray().Length;//Кол-во войск до тика
            yield return new WaitForSeconds(0.2f);
            int newSoldeirsCount = soldeirs.ToArray().Length;//Войск после тика
            soldeirsCount = soldeirsCount - newSoldeirsCount;//Разница войск
            PlayerAggression += soldeirsCount * 200;

            //Надо определить комнату игрока

            Vector2Int t = architector.ArCor(player.transform.position);
            Architector.Cell cell = architector.cells[t.x][t.y];
            if (cell == null)
            {
                state = State.Search;
                InitiateSearch();
                break;
            }
            Architector.Room PlayerRoom=cell.room;
            if (PlayerRoom == null)
            {
                state = State.Search;
                InitiateSearch();
                break;
            }





            //Игрок сменил комнату
            if (PlayerRoom != LastRoom)//----------------------------------------------------------------------------------------------------------
            {
                LastRoom = PlayerRoom;
                RoomChanged = true;
               // Debug.Log("New room");
                PlayerAggression += 15;//Смена комнаты даёт немного очков агрессии 
                nears = PlayerRoom.Connected.ToArray(); Debug.Log("nears 1: " + PlayerRoom.Connected.ToArray().Length);
                empty = PlayerRoom.Connected;
                //What rooms are empty
              
                //foreach (Architector.Room YourRoom in nears)
                //{
                //    Debug.Log("YourRoom is "+YourRoom.type);
                //    foreach (Enemy Soldeir in soldeirs)
                //    {
                //        if (Soldeir.MyRoom() == YourRoom)
                //        {
                //            empty.Remove(YourRoom);
                //        }
                //    }
                //}
              



                //Coordinates to go (centres of near room)
                roomplaces = new List<Vector3>();
                foreach (Architector.Room YourRoom in nears)
                {
                    
                    roomplaces.Add(YourRoom.GetCenter());
                }
               // Debug.Log("roomplaces 1: " + roomplaces.ToArray().Length);
               



            }
            
            //Roles
            if(newSoldeirsCount>2 && (Rusher==null || !soldeirs.Contains(Rusher)))
            {
                Rusher = soldeirs[1];
                Rusher.Stop();
                Rusher.transform.name = "Rusher";
                //Если у нас достаточно людей, а рашера нет (его не назначали или прошлый умер), то выбирается новый рашер.
                //Рашер в классическом плане всё равно прёт на пролом, создавая динамику
            }
            //Plans
            bool PlanChanged = false;
            if (plan == BattlePlan.Classic)
            {
                
                if (PlayerAggression < 100)
                {
                    plan = BattlePlan.assault;
                    PlanChanged = true;
                }
                if (PlayerAggression > 10000)
                {
                    plan = BattlePlan.Wait;
                    PlanChanged = true;
                }
            }
            if (plan == BattlePlan.Wait)
            {

                if (PlayerAggression < 900)
                {
                    plan = BattlePlan.Classic; PlanChanged = true;

                }
               
            }
            if (plan == BattlePlan.assault)
            { 
                if (PlayerAggression > 150)
                {
                    plan = BattlePlan.Wait; PlanChanged = true;
                }
            }
            if (PlanChanged)
            {
                foreach (Enemy Soldeir in soldeirs)
                {
                    Soldeir.Stop();
                }
            }




            //Common
            foreach (Enemy Soldeir in soldeirs)//Do we need to put that guy closer
            {
                bool Need = true;
                if (Soldeir!=Rusher && Soldeir != Commander)//Checking that it is not Commander or somebody like that
                {
                    foreach (Architector.Room YourRoom in nears)
                    {
                        if (Soldeir.MyRoom() == YourRoom)
                        {
                            Need = false;
                            break;
                        }

                    }
                }
                else
                {
                    if (Soldeir == Commander)
                    {
                        Need = false;
                        if (Soldeir.Current == null||RoomChanged)
                        {
                            List<Architector.Room> forbidden = new List<Architector.Room>();
                            forbidden.Add(PlayerRoom); forbidden.AddRange(nears);
                            Soldeir.StartEscape(forbidden);
                        }
                    }
                    if (plan == BattlePlan.Classic)
                    {
                        if (Soldeir == Rusher)
                        {
                            if (Soldeir.Current == null)
                            {
                                Soldeir.StartBraveTactic();
                                Need = false;
                            }
                        }
                    }
                    if (plan == BattlePlan.Wait)
                    {
                        if (Soldeir == Rusher)
                        {
                            if (Soldeir.Current == null)
                            {
                               
                                Need = true;
                            }
                        }
                    }
                   
                }
                if((plan == BattlePlan.assault))//Мы не ждём во время штурма
                {
                    Need = false;
                    if (Soldeir != Commander)
                    {
                        if (Soldeir.Current == null)
                        {
                            Soldeir.StartBraveTactic();
                        }
                    }
                  
                }
                if (Need)
                {
                    //Debug.Log("roomplaces 1: " + roomplaces.ToArray().Length);
                    //Debug.Log("nears 5: " + nears.Length);
                    if ((Soldeir.Current == null)||RoomChanged)
                    {
                        Soldeir.StartGetAny(roomplaces.ToArray());
                    }
                    else
                    {
                        Debug.Log("He is busy");
                    }
                }
                else
                {
                    if (plan == BattlePlan.Classic)
                    {
                        if (Soldeir.Current == null)
                        {
                            Soldeir.StartPositionalTactic();

                        }
                    }
                }


            }



            Debug.Log(plan);
            RoomChanged = false;
        }
    }



    IEnumerator Search()
    {
        Debug.Log("Поиски начались");
       
        while (UnChecked.ToArray().Length > 0)
        {
            yield return new WaitForSeconds(1.5f);
            foreach (Enemy Soldeir in soldeirs)
            {
                if (Soldeir.Current == null)
                {
                    Soldeir.StartChecking();
                }
            }
            Debug.Log("Комнат осталось " + UnChecked.ToArray().Length);
        }
        yield return null;
        state = State.Normal;
        Debug.Log("Поиски завершены, игрок скрылся.");
    }
    public void Noise(Vector3 place, float range)
    {
        if ((state==State.Normal)|| (state == State.Search))
        {
            foreach (Enemy Soldeir in soldeirs)
            {
                float d = Vector3.Distance(Soldeir.transform.position, place);
               if (  d < range)
                {

                    //Debug.Log("   Reaction");
                    Soldeir.ReactSound(place);
                }
                else
                {
                   // Debug.Log(d + ">" + range);
                }
            }
        }
    }
    public void Spawn(Vector3 place, int gunNumber)
    {
        GameObject guy= Instantiate(enemyPref, place, new Quaternion());
        //guy.GetComponent<Enemy>().enemyManager = this;
        if ((gunNumber > -1) && (gunNumber < gunPrefs.Length))
        {

            StartCoroutine(guy.GetComponent<Character>().TakeGun(Instantiate(gunPrefs[gunNumber])));
        }


    }
    // Start is called before the first frame update
    void Start()
    {
        List<Architector.Room> UnChecked = new List<Architector.Room>();
        levelManager = GameObject.FindWithTag("Manager").GetComponent<LevelManager>();
        if (levelManager.player != null)
        {
            sus.Add(levelManager.player.gameObject);
        }
        StartCoroutine(EnemyUpdate());
       // InitiateBattle();
        
    }
    IEnumerator EnemyUpdate()
    {
        yield return new WaitForSeconds(0.2f);
        //InitiateSearch();//Remove it
        while (true)
        {
            yield return new WaitForSeconds(0.2f);


            if (state == State.Normal)
            {
                foreach (Enemy Soldeir in soldeirs)
                {
                    if ((Soldeir.Current == null) && (Soldeir != Commander))
                    {
                        Debug.Log("Patrol");
                        Soldeir.StartPatrolling();
                    }
                }
            }

            if ((Commander == null)||!soldeirs.Contains(Commander))
            {
                if (soldeirs.ToArray().Length > 0)
                {
                    Commander = soldeirs[0];
                    Debug.Log("new Commander");
                    Commander.transform.name = "Commander";
                    Commander.Stop();
                }
            }
            if ((state == State.Normal) || (state == State.Search))
            {
                foreach (Enemy Soldeir in soldeirs)
                {
                    if (Soldeir.Fury == false)
                    {
                        Debug.Log("Seeing");
                        foreach (GameObject s in sus)
                        {
                            Debug.Log("Seeing " + s);
                            if (Soldeir.character.DoISee(s))
                            {
                                if (s.GetComponent<PlayerControl>())
                                {
                                    Debug.Log("It is Player");

                                    Soldeir.Target = s.transform;
                                    Soldeir.StartBraveTactic();
                                    Soldeir.Fury = true;

                                    Architector.Room room=Soldeir.MyRoom();
                                    foreach (Enemy SecondSoldeir in soldeirs)
                                    {
                                        if (SecondSoldeir != Soldeir)
                                        {
                                            if (SecondSoldeir.MyRoom() == room)
                                            {
                                                SecondSoldeir.StartAlarm(State.Battle);
                                            }
                                        }
                                    }

                                        break;
                                }
                                else
                                {
                                    if (s.GetComponent<Enemy>())
                                    {

                                        Debug.Log("Deadman was founded");
                                        Soldeir.StartReactDeadMan(s);
                                        //sus.Remove(s);

                                    }
                                }
                                Debug.Log("I do see" + s);
                               
                               
                            }
                        }
                    }
                }
            }
        }
    }
    public IEnumerator ProceduralSpawn(int Score)
    {
        while (!architector.Finished)
        {
            yield return null;
        }

        Architector.Room[] rooms = architector.AllRooms.ToArray();
        List<Architector.Cell> cells = new List<Architector.Cell>();
        foreach(Architector.Room room in rooms)
        {
            cells.AddRange(room.RoomCells);
        }
        Architector.Cell[] celAr = cells.ToArray();
        while (Score > 0)
        {
            Score -= 20;
            int gunnumber = Random.Range(0, 3);
            Score -= gunnumber * 10;

            int number=Random.Range(0, celAr.Length);
            Architector.Cell c = celAr[number];
            Spawn(architector.StartPoint + new Vector3(10 * c.a, 0, 10 * c.b), gunnumber);

        }
    }
}
