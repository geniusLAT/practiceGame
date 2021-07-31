using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public GameObject centerPrefab;
    public EnemyManager enemyManager;
    protected LevelManager levelManager;
    public Transform Target;
    NavMeshAgent agent;
    public Character character;
    public Coroutine Current;
    bool IsHearing = true;
    public bool Fury=false;
    float speed=3;
    Vector3[] patrolPoints = null;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        speed = agent.speed;
        levelManager = GameObject.FindWithTag("Manager").GetComponent<LevelManager>();
        Target = levelManager.player;
        character = GetComponent<Character>();

        if (enemyManager == null)
        {
            if (GameObject.FindGameObjectsWithTag("EnemyManager").Length == 0)
            {
                Current = StartCoroutine(BraveTactic());
            }
            else
            {
                enemyManager = GameObject.FindGameObjectsWithTag("EnemyManager")[0].GetComponent<EnemyManager>();
            }
            if (enemyManager != null)
            {
                enemyManager.soldeirs.Add(this);
            }
        }
        

    }




   

    public void StartPatrolling()
    {
        Stop();
        Current = StartCoroutine(Patrolling());
    }

    IEnumerator Patrolling()
    {
        if (Random.Range(0, 2) > 0) patrolPoints = new Vector3[] { transform.position };
        if (patrolPoints == null)
        {
            int f = Random.Range(0, 5);
            Architector.Room room = MyRoom();
            patrolPoints = new Vector3[f];
            for (int i = 0; i < f; i++)
            {
                patrolPoints[i] = room.GetCenter();
                Architector.Room[] potentials= room.Connected.ToArray();
                int p = Random.Range(0, potentials.Length);
                room = potentials[p];
            }
        }


        while (true)
        {
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                yield return Reach(patrolPoints[i], 30);
            }
            //for (int i = patrolPoints.Length; i >0 ; i--)
            //{
            //    yield return Reach(patrolPoints[i], 30);
            //}
            yield return new WaitForSeconds(3);
        }
    }
    //Battle things
    public  void StartGetAny(Vector3[] positions)
    {
        Stop();
        Current = StartCoroutine(GetAny(positions,true,30));
    }
    IEnumerator GetAny(Vector3[] positions,bool needToStop,float Dis)
    {
        yield return null;
        float minimum = 1000000;
        agent.speed = speed;
        if (positions.Length > 0)
        {
            //Debug.Log("positions.Length = " + positions.Length);
            //Понять куда идти
            Vector3 minVector = positions[0];
            foreach (Vector3 that in positions)
            {
                agent.enabled = true;
                agent.destination = that;
                yield return null;
                float Distance = agent.remainingDistance;
                //Debug.Log(Distance+"<"+minimum);
                if (Distance < minimum)
                {
                    minimum = Distance;
                    minVector = that;
                }
            }
           // Debug.Log("minVector"+minVector);
            agent.destination = minVector;
            bool Weaponed = false;
            if (character.RHobject != null)
            {
                if (character.RHobject.GetComponent<Gun>())
                {
                    Weaponed = true;
                }
            }
                //Идти
                while (agent.remainingDistance > Dis)
            {
                yield return new WaitForSeconds(0.5f);
                
                if (character.DoISee(Target.gameObject)&&Weaponed)
                {
                    transform.LookAt(Target);
                    agent.enabled = false;
                    StartCoroutine(character.RHobject.GetComponent<Gun>().Atack(Target.position));
                    yield return new WaitForSeconds(1);
                    agent.enabled = true;
                    yield return new WaitForSeconds(2);
                }
            }
            Debug.Log("На позиции");
            if (needToStop)
            {
                Stop();
            }
            
        }
    }
    public void StartPositionalTactic()
    {
         Stop();
        Current = StartCoroutine(PositionalTactic());
    }
    public void StartEscape(List<Architector.Room> forbidden)
    {
        Stop();
        Current = StartCoroutine(Escape( forbidden));
        Debug.Log("Current is escaping");
    }
    IEnumerator Escape(List<Architector.Room> forbidden)
    {
        Debug.Log("Trying to Escape");
        yield return null;
        Architector architector = enemyManager.architector;
        bool NeedToEscape = false;
        Architector.Room myroom = MyRoom();
        foreach (Architector.Room room in forbidden)
        {
            if(room== myroom)
            {
                NeedToEscape = true;
                break;
            }
        }



        if (NeedToEscape)
        {
            Architector.Room placeToLeave=null;
            NeedToEscape = false;
            foreach (Architector.Room that in myroom.Connected)
            {
                foreach (Architector.Room room in forbidden)
                {
                    if (room == that)
                    {
                        
                        NeedToEscape = true;
                        break;
                    }
                }
                if (!NeedToEscape)
                {
                    placeToLeave = that;
                    break;
                }

            }
            if (placeToLeave != null)
            {
                yield return Reach(placeToLeave.GetCenter(), 10);
                Debug.Log("Escape");

            }
            else
            {
                List<Architector.Room> all = new List<Architector.Room>();
                all.AddRange(architector.AllRooms.ToArray());
                foreach (Architector.Room room in forbidden)
                {
                    all.Remove(room);
                }
                int m = all.ToArray().Length;
                int r = Random.Range(0, m);
                yield return Reach(all[r].GetCenter(), 10);
                Debug.Log("Escape");
            }
        }
        Stop();
    }
    IEnumerator PositionalTactic()
    {
        Vector3 original=transform.position;
        yield return null;
        bool Weaponed = false;
        if (character.RHobject != null)
        {
            if (character.RHobject.GetComponent<Gun>())
            {
                Weaponed = true;
            }
        }
        while (character.HP > 0)
        {
            if (Weaponed)
            {
                yield return GetCloser();
                
                agent.speed = 0.01f;
                for (int i = 0; i < Random.Range(1,5); i++)
                {

                    transform.LookAt(Target);
                    character.RH.transform.LookAt(Target);
                    StartCoroutine(character.RHobject.GetComponent<Gun>().Atack(Target.position));
                    yield return new WaitForSeconds(0.7f);
                }
                agent.speed = speed;
                yield return Reach(original, 10);

            }
            else
            {
                yield return GetWeapon();
            }
            yield return null;
        }
        
    }
    IEnumerator GetCloser()
    {
        agent.destination = Target.position;
        while (!character.DoISee(Target.gameObject))
        {
            yield return new WaitForSeconds(0.3f);
        }
        yield return null;
    }

    IEnumerator GetWeapon()
    {
        Debug.Log("Ищу оружие");
        while (character.RHobject == null)
        {


            while (levelManager.guns.ToArray().Length == 0)
            {
                yield return new WaitForSeconds(3);
            }
            List<Vector3> places = new List<Vector3>();
            foreach (Gun gun in levelManager.guns)
            {
                places.Add(gun.transform.position);
            }
            yield return GetAny(places.ToArray(), false,0.1f);
            StartCoroutine(character.TakeNearestGun());
            Debug.Log("Пытаюсь подобрать ствол");
            if(character.RHobject != null)
            {
                break;
            }
        }
    }
    //Search things
    public void StartReactDeadMan(GameObject DeadMan)
    {
        Stop();
        Current = StartCoroutine(ReactDeadMan(DeadMan));
    }
    IEnumerator ReactDeadMan(GameObject DeadMan)
    {
        yield return Reach(DeadMan.transform.position, 20);
        yield return new WaitForSeconds(2f);
        enemyManager.sus.Remove(DeadMan);
        yield return Alarm(EnemyManager.State.Search);
    }
    public Architector.Room MyRoom()
    {
        Vector2Int t = enemyManager.architector.ArCor(transform.position);
        return enemyManager.architector.cells[t.x][t.y].room;
    }
    public void StartChecking()
    {
        Stop();
        Current = StartCoroutine(Checking());
    }
    IEnumerator Checking()
    {
        if (enemyManager == null)
        {
            Debug.Log("No enemyManager");
        }
        if (enemyManager.UnChecked == null)
        {
            Debug.Log("No UnChecked");
        }
        while (enemyManager.UnChecked.ToArray().Length > 0)
        {
            Architector.Room[] rooms = enemyManager.UnChecked.ToArray();
            yield return null;
            float minDistance = 100000;
            Architector.Room favorite=rooms[0];
            foreach (Architector.Room that in rooms)
            {
                float thatDistance = Vector3.Distance(transform.position, that.GetCenter());
                if (thatDistance < minDistance)
                {
                    minDistance = thatDistance;
                    favorite = that;
                }
            }
            Debug.Log("New room to check"+favorite.type);
            yield return CheckRoom(favorite);
        }
    }
    IEnumerator CheckRoom(Architector.Room room)
    {
        if (MyRoom() != room)
        {
            //if (room.type == Architector.RoomType.Kitchen)
            {
                    Instantiate(centerPrefab, room.GetCenter(), new Quaternion());
            }
           
            yield return Reach(room.GetCenter(),20);
        }
        for (int i = 0; i < 60; i++)
        {
            transform.Rotate(0, 1, 0);
            yield return null;
            //yield return null;
        }
        for (int i = 0; i < 180; i++)
        {
            transform.Rotate(0, -1, 0);
            yield return null;
            //yield return null;
        }
        for (int i = 0; i < 60; i++)
        {
            transform.Rotate(0, 1, 0);
            yield return null;
            //yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        Debug.Log("room "+room.type+" is Clear");
       
        enemyManager.UnChecked.Remove(room);

    }
    public void ReactSound(Vector3 place)
    {
        Debug.Log("Sounds");
        if (IsHearing)
        {
            
                Debug.Log("I heard smt");
                Stop();
                Go(place);
            
        }
        
    }
    public void Go(Vector3 Place)
    {

        Stop();
        Current = StartCoroutine(Reach(Place, 4));
    }
    // Update is called once per frame
    void Update()
    {
      
    }

    public IEnumerator Reach(Transform Target, float Distance)
    {
        yield return null;
        agent.enabled = true;
        while (Vector3.Distance(Target.position, transform.position) > Distance)
        {
            agent.destination = Target.position;
            yield return null;
        }

    }
    public IEnumerator Reach(Vector3 Target, float Distance)
    {
        yield return null;
        agent.enabled = true;
        while (Vector3.Distance(Target, transform.position) > Distance)
        {
            agent.destination = Target;
            yield return null;
        }

    }
    public void StartReactSus(GameObject s)
    {
        Stop();
        Current = StartCoroutine(ReactSus(s));
    }
    public void StartBraveTactic()
    {

        Stop();
        Current = StartCoroutine(BraveTactic());
    }
    public IEnumerator ReactSus(GameObject s)
    {
        yield return Reach(s.transform.position, 2);
        
        yield return new WaitForSeconds(2);
        
        yield return Alarm(EnemyManager.State.Search);
       
        enemyManager.sus.Remove(s);
    }
    public void StartAlarm(EnemyManager.State state)
    {
        Stop();
        Current = StartCoroutine(Alarm(state));
    }
    public IEnumerator Alarm(EnemyManager.State state)
    {
        Transform commander = enemyManager.Commander.transform;
        yield return Reach(commander, 15);
        if ((enemyManager.state == EnemyManager.State.Normal) && ((state == EnemyManager.State.Search) || (state == EnemyManager.State.Battle)))
        {
            enemyManager.state = state;
            if(state== EnemyManager.State.Search)
            {
                enemyManager.InitiateSearch();
            }
            if (state == EnemyManager.State.Battle)
            {
                enemyManager.InitiateBattle();
            }
        }
        if ((enemyManager.state == EnemyManager.State.Search) &&  (state == EnemyManager.State.Battle))
        {
            enemyManager.state = state;
            if (state == EnemyManager.State.Battle)
            {
                enemyManager.InitiateBattle();
            }
        }
        Stop();
    }
    public IEnumerator BraveTactic()
    {
        //while (true)
        //{
        //    yield return null;
        //}
        IsHearing = false;
        Fury = true;
        Debug.Log("BraveTactic started");
        //temporal lines
        yield return new WaitForSeconds(0.3f);
        StartCoroutine(character.TakeNearestGun());


        yield return null;
        Debug.Log("BraveTactic HEre");
        if (character.RHobject != null)
        {
            if (character.RHobject.GetComponent<Gun>())
            {
                Gun gun = character.RHobject.GetComponent<Gun>();
                agent.stoppingDistance = 7;
                while (character.HP > 0)
                {
                    Debug.Log("BraveTactic Shooting");
                    yield return null;
                    if (character.DoISee(Target.gameObject))
                    {
                        Vector3 AIM = Target.position + new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), Random.Range(-2f, 2f));
                        gun.transform.LookAt(AIM);
                        transform.rotation = new Quaternion(0, gun.transform.rotation.y, 0, gun.transform.rotation.w);
                        yield return gun.Atack(AIM);
                    }
                    agent.destination = Target.position;

                }
            }
        }
        else
        {
            agent.stoppingDistance = 2;

            while (character.HP > 0)
            {
                Debug.Log("BraveTactic melee");

                agent.destination = Target.position;

                if (Vector3.Distance(Target.position, transform.position) < 3)
                {
                    Debug.Log("BraveTactic melee");
                    character.Melee();

                    //yield return new WaitForSeconds(1);
                }
                yield return null;
            }
        }
    }
    public void Stop()
    {
        if (Current != null)
        {
            //Debug.Log(Current);
            StopCoroutine(Current);
            Current = null;
           // Debug.Log(Current);
        }
        //StartCoroutine(D());
        
    }

    private void OnDisable()
    {
        Debug.Log("Cool");
        //StopAllCoroutines();
        Stop();
        if (enemyManager != null)
        {
            //Я больше не солдат, я труп
            enemyManager.soldeirs.Remove(this);
            enemyManager.sus.Add(this.gameObject);
        }
    }
}
