using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Architector : MonoBehaviour
{
    public GameObject DoorWayPref;
    public GameObject FloorPref;
    public GameObject WallPref;
    public GameObject LightPref;
    public Texture[] WallTexs;
    public Texture[] FloorTexs;
    PlotManager plot;
    public Vector3 StartPoint = new Vector3(0, 0.6f, 0);

    public Cell[][] cells;//a,b coordinates
    public List<Room> AllRooms;
    Architector architector;
    List<Vector2Int> NorthDoors;
    List<Vector2Int> WestDoors;
    public bool RunTutorial = false;
    public bool Finished;
    public TutorialV2 tutorial;
    public GameObject plotManagerPrefab;
    public EnemyManager enemyManager;
    public LevelManager levelManager;
    public GameObject[] decoratorsPref;



    public enum RoomType
    {
        corridor,
        bathroom,
        Kitchen,
        Office,
        Parking,
        Restaraunt,
        HotelHall,
        outdoor,
        Living
    }
    Vector2Int[] dirs = { new Vector2Int(0, 1), new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(-1, 0) };

    public class Cell
    {
        public int a;
        public int b;
        public GameObject Floor;
        public GameObject NorthWall;
        public GameObject WestWall;
        public Room room;
        public GameObject busy;
        public Cell(int a, int b)
        {
            this.a = a;
            this.b = b;
        }
        public bool IsBusy()
        {
            if (busy == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    public class Room
    {
        public Architector architector;
        public List<Cell> RoomCells;
        public RoomType type;
        public int id;
        public List<Room> Connected;
        public Vector2Int rdBorder;
        public Vector2Int luBorder;
        public bool access;
        public int SquareLimit = 0;

        void DecorateWall(GameObject Wall, bool ToCenter)
        {
            MeshRenderer Plane;
            if (ToCenter)
            {
                Plane = Wall.GetComponent<Wall>().ToCenter;
            }
            else
            {
                Plane = Wall.GetComponent<Wall>().OutOfcenter;
            }
            Texture texture = architector.WallTexs[0];
            switch (type)
            {
                case RoomType.corridor:
                    // Debug.Log("corridor");
                    texture = architector.WallTexs[0];
                    break;
                case RoomType.bathroom:
                    //Debug.Log("bathroom");
                    texture = architector.WallTexs[1];
                    break;
                case RoomType.Kitchen:

                    texture = architector.WallTexs[2];
                    break;
                case RoomType.Office:

                    texture = architector.WallTexs[3];
                    break;
                case RoomType.Parking:

                    texture = architector.WallTexs[4];
                    break;
                case RoomType.Restaraunt:

                    texture = architector.WallTexs[5];
                    break;
                case RoomType.HotelHall:

                    texture = architector.WallTexs[6];
                    break;
                case RoomType.Living:

                    texture = architector.WallTexs[7];
                    break;
                default:
                    Plane.material.SetColor("_Color", Color.red);
                    break;
            }
            //Debug.Log("this is " + type+" wall");
            Plane.material.SetTexture("_MainTex", texture);
        }
        void DecoratFloor(GameObject Floor)
        {
            MeshRenderer Plane = Floor.GetComponent<MeshRenderer>();




            Texture texture = architector.FloorTexs[0];
            switch (type)
            {
                case RoomType.corridor:
                    //Debug.Log("corridor");
                    texture = architector.FloorTexs[0];
                    break;
                case RoomType.bathroom:
                    // Debug.Log("bathroom");
                    texture = architector.FloorTexs[1];
                    break;
                case RoomType.Kitchen:

                    texture = architector.FloorTexs[2];
                    break;
                case RoomType.Office:

                    texture = architector.FloorTexs[3];
                    break;
                case RoomType.Parking:

                    texture = architector.FloorTexs[4];
                    break;
                case RoomType.Restaraunt:

                    texture = architector.FloorTexs[5];
                    break;
                case RoomType.HotelHall:

                    texture = architector.FloorTexs[6];
                    break;
                case RoomType.Living:

                    texture = architector.FloorTexs[7];
                    break;
                default:
                    Plane.material.SetColor("_Color", Color.red);
                    break;
            }
            Plane.material.SetTexture("_MainTex", texture);
            //Debug.Log("this is " + type + " floor");
        }
        public Room(Vector2Int Start, Vector2Int Finish, RoomType type, Architector arc)
        {
            this.type = type;
            SquareLimit = CalculateLimit();
            architector = arc;
            Connected = new List<Room>();
            //id = architector.AllRooms.Count;
            // Debug.Log("id"+id);
            // Debug.Log(architector);
            architector.AllRooms.Add(this);
            RoomCells = new List<Cell>();
            for (int a = Mathf.Min(Start.x, Finish.x); a < Mathf.Max(Start.x, Finish.x); a++)
            {
                for (int b = Mathf.Min(Start.y, Finish.y); b < Mathf.Max(Start.y, Finish.y); b++)
                {
                    Cell cell = new Cell(a, b);
                    cell.room = this;
                    //Debug.Log(architector);
                    architector.cells[a][b] = cell;
                    RoomCells.Add(cell);
                }
            }
            rdBorder = new Vector2Int(Mathf.Min(Start.x, Finish.x), Mathf.Min(Start.y, Finish.y));
            luBorder = new Vector2Int(Mathf.Max(Start.x, Finish.x), Mathf.Max(Start.y, Finish.y));


        }
        public Vector3 GetCenter()
        {
            Vector2Int CentralCell = new Vector2Int((rdBorder.x + luBorder.x) / 2, (rdBorder.y + luBorder.y) / 2);
            //Debug.Log("rd" + rdBorder + " lu" + luBorder + " Central" + CentralCell);
            return architector.PlaceOfCell(CentralCell);
        }
        public void GetLights()
        {
            GameObject L = Instantiate(architector.LightPref);
            int n = Random.Range(0, RoomCells.Count);
            L.transform.position = new Vector3(10 * RoomCells[n].a, 10, 10 * RoomCells[n].b);
        }
        public void GetWalls()
        {

            foreach (Cell one in RoomCells)
            {
                if (NeedWall(one, 1, 0))
                {
                    //Debug.Log("df "+NeedWall(one, 1, 0) );
                    if (one.NorthWall == null)
                    {
                        one.NorthWall = architector.PutWall(one.a, one.b, true);
                    }

                    DecorateWall(one.NorthWall, true);
                }
                if (NeedWall(one, 0, 1))
                {
                    //Debug.Log("ff " + NeedWall(one, 1, 0));
                    if (one.WestWall == null)
                    {
                        one.WestWall = architector.PutWall(one.a, one.b, false);
                    }

                    DecorateWall(one.WestWall, true);
                }

                if (NeedWall(one, -1, 0))
                {
                    bool T = true;
                    if (architector.cells[one.a - 1][one.b] == null)//Если нет соседней клетки, то её надо создать
                    {
                        //Debug.Log("Empty");
                        Cell cell = new Cell(one.a - 1, one.b);
                        architector.cells[one.a - 1][one.b] = cell;
                        T = false;
                    }
                    if (architector.cells[one.a - 1][one.b].NorthWall == null)//Если в этой соседней клетке нет стены, то её надо создать
                    {
                        architector.cells[one.a - 1][one.b].NorthWall = architector.PutWall(one.a - 1, one.b, true);
                    }
                    T = true;
                    if (T)
                    {
                        DecorateWall(architector.cells[one.a - 1][one.b].NorthWall, false);
                    }

                }
                if (NeedWall(one, 0, -1))
                {

                    if (architector.cells[one.a][one.b - 1] == null)//Если нет соседней клетки, то её надо создать
                    {
                        Cell cell = new Cell(one.a, one.b - 1);
                        architector.cells[one.a][one.b - 1] = cell;
                        //Debug.Log("Empty West");
                    }
                    if (architector.cells[one.a][one.b - 1].WestWall == null)//Если в этой соседней клетке нет стены, то её надо создать
                    {
                        architector.cells[one.a][one.b - 1].WestWall = architector.PutWall(one.a, one.b - 1, false);
                    }
                    DecorateWall(architector.cells[one.a][one.b - 1].WestWall, false);

                }
            }


        }
        public void PlaceFloors()
        {
            foreach (Cell one in RoomCells)
            {
                one.Floor = architector.PutFloor(one.a, one.b);
                DecoratFloor(one.Floor);
            }
        }
        bool NeedWall(Cell one, int deltaA, int DeltaB)
        {
            if (architector.cells[one.a + deltaA][one.b + DeltaB] != null)
            {

                if (architector.cells[one.a + deltaA][one.b + DeltaB].room == this)
                {
                    return false;
                }
                else
                {
                    // Debug.Log("Not our room");

                }
            }
            else
            {
                // Debug.Log("Null");

            }
            return true;
        }
        int CalculateLimit()
        {
            switch (type)
            {
                case RoomType.corridor:
                    return 0;
                    break;
                case RoomType.bathroom:
                    return 8;
                    break;
                case RoomType.Kitchen:

                    return 30;
                    break;
                case RoomType.Office:

                    return 40;
                    break;
                case RoomType.Parking:

                    return 0;
                    break;
                case RoomType.Restaraunt:

                    return 35;
                    break;
                case RoomType.HotelHall:

                    return 0;
                    break;
                case RoomType.Living:
                    return 25;
                default:
                    return 0;
                    break;
            }
        }
        public void GetDecorator()
        {
            switch (type)
            {
                case RoomType.corridor:
                    // Debug.Log("corridor");
                    //texture = architector.WallTexs[0];
                    break;
                case RoomType.bathroom:
                    //Debug.Log("bathroom");
                    //texture = architector.WallTexs[1];
                    Instantiate(architector.decoratorsPref[1]).GetComponent<Decorator>().Decorate(this);
                    break;
                case RoomType.Kitchen:

                    //texture = architector.WallTexs[2];
                    Instantiate(architector.decoratorsPref[2]).GetComponent<Decorator>().Decorate(this);
                    break;
                case RoomType.Office:

                    Instantiate(architector.decoratorsPref[3]).GetComponent<Decorator>().Decorate(this);
                    //texture = architector.WallTexs[3];
                    break;
                case RoomType.Parking:

                   // texture = architector.WallTexs[4];
                    break;
                case RoomType.Restaraunt:

                    Instantiate(architector.decoratorsPref[5]).GetComponent<Decorator>().Decorate(this);
                    // texture = architector.WallTexs[5];
                    break;
                case RoomType.HotelHall:

                    //texture = architector.WallTexs[6];
                    break;
                case RoomType.Living:
                    Instantiate(architector.decoratorsPref[7]).GetComponent<Decorator>().Decorate(this);
                    
                    break;
                default:
                   // Plane.material.SetColor("_Color", Color.red);
                    break;
            }
        }



        public bool expand(Vector2Int Exp, Vector2Int Max)
        {
            if (SquareLimit > 0)
            {
                if (RoomCells.ToArray().Length > SquareLimit)
                {
                    return false;
                }
            }


            if (Exp == new Vector2Int(1, 0))
            {
                int expanded = luBorder.x;
                bool CanBeDone = true;

                if (expanded > Max.x)//Расширение невозможно за край карты
                {
                    CanBeDone = false;
                }


                for (int i = rdBorder.y; i < luBorder.y; i++)//Проверяем можно ли туда расшириться
                {
                    if (architector.cells[expanded][i] != null)
                    {
                        CanBeDone = false;
                    }
                }
                if (CanBeDone)//Если можно расшириться, то расширяемся
                {
                    for (int i = rdBorder.y; i < luBorder.y; i++)
                    {
                        int a = expanded;
                        int b = i;
                        Cell cell = new Cell(a, b);
                        cell.room = this;
                        //Debug.Log(architector);
                        architector.cells[a][b] = cell;
                        RoomCells.Add(cell);
                    }
                    luBorder.x += 1;
                }
                return CanBeDone;

            }
            if (Exp == new Vector2Int(0, 1))
            {
                Debug.Log("It was Called");
                int expanded = luBorder.y;
                bool CanBeDone = true;

                if (expanded > Max.y)//Расширение невозможно за край карты
                {
                    CanBeDone = false;
                }

                for (int i = rdBorder.x; i < luBorder.x; i++)
                {
                    if (architector.cells[i][expanded] != null)
                    {
                        CanBeDone = false;
                    }
                }
                if (CanBeDone)
                {
                    for (int i = rdBorder.x; i < luBorder.x; i++)
                    {
                        int a = i;
                        int b = expanded;
                        Cell cell = new Cell(a, b);
                        cell.room = this;
                        //Debug.Log(architector);
                        architector.cells[a][b] = cell;
                        RoomCells.Add(cell);
                    }
                    luBorder.y += 1;
                }
                return CanBeDone;

            }

            if (Exp == new Vector2Int(-1, 0))
            {
                int expanded = rdBorder.x - 1;
                bool CanBeDone = true;

                if (expanded < 2)//Расширение невозможно за край карты
                {
                    CanBeDone = false;
                }


                for (int i = rdBorder.y; i < luBorder.y; i++)//Проверяем можно ли туда расшириться
                {
                    if (architector.cells[expanded][i] != null)
                    {
                        CanBeDone = false;
                    }
                }
                if (CanBeDone)//Если можно расшириться, то расширяемся
                {
                    for (int i = rdBorder.y; i < luBorder.y; i++)
                    {
                        int a = expanded;
                        int b = i;
                        Cell cell = new Cell(a, b);
                        cell.room = this;
                        //Debug.Log(architector);
                        architector.cells[a][b] = cell;
                        RoomCells.Add(cell);
                    }
                    rdBorder.x += -1;
                }
                return CanBeDone;

            }
            if (Exp == new Vector2Int(0, -1))
            {
                int expanded = rdBorder.y - 1;
                bool CanBeDone = true;

                if (expanded < 2)//Расширение невозможно за край карты
                {
                    CanBeDone = false;
                }


                for (int i = rdBorder.x; i < luBorder.x; i++)//Проверяем можно ли туда расшириться
                {
                    if (architector.cells[i][expanded] != null)
                    {
                        CanBeDone = false;
                    }
                }
                if (CanBeDone)//Если можно расшириться, то расширяемся
                {
                    for (int i = rdBorder.x; i < luBorder.x; i++)
                    {
                        int a = i;
                        int b = expanded;
                        Cell cell = new Cell(a, b);
                        cell.room = this;
                        //Debug.Log(architector);
                        architector.cells[a][b] = cell;
                        RoomCells.Add(cell);
                    }
                    rdBorder.y += -1;
                }
                return CanBeDone;

            }



            return false;
        }
        public bool PromoteAccess()
        {
            if (access) {
                bool Any = false;
                foreach (Room that in Connected)
                {
                    if (!that.access)
                    {
                        Any = true;
                        that.access = true;
                    }
                }
                return Any;
            } else
            {
                return false;
            }
        }
    }

    public Vector3 PlaceOfCell(int a, int b)
    {
        return StartPoint + new Vector3(10 * a, 0, 10 * b);
    }
    public Vector3 PlaceOfCell(Vector2Int Cell)
    {
        return PlaceOfCell(Cell.x, Cell.y);
    }
    GameObject PutFloor(int a, int b)
    {
        GameObject floor = Instantiate(FloorPref);
        floor.transform.position = StartPoint + new Vector3(10 * a, 0, 10 * b);
        return floor;
    }
    GameObject PutWall(int a, int b, bool North)
    {
        GameObject wall = Instantiate(WallPref);
        if (North)
        {
            wall.GetComponent<Wall>().t = 1;
            wall.transform.position = StartPoint + new Vector3(10 * a, 0, 10 * b) + new Vector3(5.2f, 5.5f);
        }
        else
        {
            wall.GetComponent<Wall>().t = 2;

            wall.transform.Rotate(new Vector3(0, -90, 0));
            wall.transform.position = StartPoint + new Vector3(10 * a, 0, 10 * b) + new Vector3(0, 5.5f, 5.2f);
        }
        return wall;
    }

    GameObject PutDoor(int a, int b, bool North)
    {
        Cell cell;
        if (cells[a][b] == null)
        {
            Debug.Log("Cell created");
            cell = new Cell(a, b);
            cells[a][b] = cell;
        }
        cell = cells[a][b];

        bool ConnectFailed = false;
        if (cell.room == null) ConnectFailed = true;
        Room A = cell.room;
        Room B;


        GameObject Door;
        Vector3 Place;
        if (North)
        {

            Place = StartPoint + new Vector3(10 * a, 0, 10 * b) + new Vector3(4.48f, 0.69f, 4.27f);
            Door = Instantiate(DoorWayPref, Place, new Quaternion(0, 0.7f, 0, 0.7f));

        }
        else
        {



            Place = StartPoint + new Vector3(10 * a, 0, 10 * b) + new Vector3(-4.51f, 0.69f, 4.45f);
            Door = Instantiate(DoorWayPref, Place, new Quaternion(0, 0, 0, 1));

            //Door.transform.Rotate(new Vector3(0, -90, 0));
        }

        if (North)
        {
            if (cell.NorthWall != null)
            {
                cell.NorthWall.SetActive(false);
                Debug.Log("Unactive");
            }
            {
                Debug.Log("Unactive failed");
            }

            cell.busy = Door;
            if (cells[a + 1][b] == null)
            {
                Debug.Log("Cell created");
                Cell NorthCell = new Cell(a + 1, b);
                cells[a + 1][b] = NorthCell;

            }
            cells[a + 1][b].busy = Door;
            if (cells[a + 1][b].room == null) ConnectFailed = true;
            B = cells[a + 1][b].room;
        }
        else
        {
            if (cell.WestWall != null)
            {
                cell.WestWall.SetActive(false);
            }

            cell.busy = Door;
            if (cells[a][b + 1] == null)
            {
                Debug.Log("Cell created");
                Cell NorthCell = new Cell(a, b + 1);
                cells[a][b + 1] = NorthCell;

            }
            cells[a][b + 1].busy = Door;
            if (cells[a][b + 1].room == null) ConnectFailed = true;

            B = cells[a][b + 1].room;
        }
        if ((A != null) && (B != null))
        {
            A.Connected.Add(B); //Debug.Log("A: " + A.Connected.ToArray().Length);
            B.Connected.Add(A);// Debug.Log("B: " + B.Connected.ToArray().Length);
        }


        return Door;
    }
    public GameObject PutFurniture(GameObject prefab, int a, int b)
    {
        GameObject that = Instantiate(prefab);
        that.transform.position = StartPoint + new Vector3(10 * a, 0, 10 * b);
        cells[a][b].busy = that;
        return that;
    }

    // Start is called before the first frame update
    void Start()
    {
        //GetComponent<NavMeshSurface>().BuildNavMesh();

        NorthDoors = new List<Vector2Int>();
        WestDoors = new List<Vector2Int>();
        Debug.Log("1");
        architector = this;
        AllRooms = new List<Room>();
        cells = new Cell[50][];
        for (int i = 0; i < 50; i++)
        {
            cells[i] = new Cell[60];
        }




        StartCoroutine(Build());

    }

    //Выдаёт координаты в строении для координат в мире
    public Vector2Int ArCor(Vector3 position)
    {
        position = position - StartPoint;
        int a = Mathf.RoundToInt(position.x / 10);
        int b = Mathf.RoundToInt(position.z / 10);

        return new Vector2Int(a, b);
    }
    public void PutRoom(Vector2Int Start, Vector2Int Finish, RoomType type)
    {
        new Room(Start, Finish, RoomType.corridor, architector);

    }
    public void PutRoom(int a1, int a2, int b1, int b2, RoomType type)
    {
        new Room(new Vector2Int(a1, b1), new Vector2Int(a2, b2), type, architector);

    }
    public void PutRoom(int a1, int a2, int b1, int b2, RoomType type, out Room room)
    {
        room = new Room(new Vector2Int(a1, b1), new Vector2Int(a2, b2), type, architector);

    }
    void GetDoors()
    {
        foreach (Vector2Int c in NorthDoors)
        {
            PutDoor(c.x, c.y, true);
        }
        foreach (Vector2Int c in WestDoors)
        {
            PutDoor(c.x, c.y, false);
        }
    }

    void TutorialPreBuild()
    {
        Architector ar = this;


        ar.PutRoom(2, 7, 20 - 9, 20 - 11, Architector.RoomType.corridor);
        ar.PutRoom(10, 7, 20 - 7, 20 - 11, Architector.RoomType.corridor);
        ar.PutRoom(8, 12, 20 - 6, 20 - 7, Architector.RoomType.corridor);
        ar.PutRoom(10, 12, 20 - 7, 20 - 16, Architector.RoomType.HotelHall);//Холл
        ar.PutRoom(11, 14, 20 - 4, 20 - 6, Architector.RoomType.corridor);
        ar.PutRoom(14, 17, 20 - 1, 20 - 6, Architector.RoomType.Office);//Офис
        ar.PutRoom(12, 17, 20 - 6, 20 - 14, Architector.RoomType.Restaraunt);//Ресторан
        ar.PutRoom(12, 14, 20 - 14, 20 - 17, Architector.RoomType.bathroom);//Туалет
        ar.PutRoom(14, 19, 20 - 14, 20 - 18, Architector.RoomType.Kitchen);//Кухня
        ar.PutRoom(17, 21, 20 - 13, 20 - 14, Architector.RoomType.corridor);
        ar.PutRoom(19, 22, 20 - 14, 20 - 18, Architector.RoomType.Office);//Офис
        ar.PutRoom(18, 27, 20 - 6, 20 - 13, Architector.RoomType.Parking);//Паркинг

        NorthDoors.Add(new Vector2Int(6, 10));
        NorthDoors.Add(new Vector2Int(13, 15));
        NorthDoors.Add(new Vector2Int(11, 7));

        WestDoors.Add(new Vector2Int(8, 12));
        WestDoors.Add(new Vector2Int(10, 12));
        WestDoors.Add(new Vector2Int(11, 13));
        WestDoors.Add(new Vector2Int(15, 5));
        WestDoors.Add(new Vector2Int(18, 5));
        WestDoors.Add(new Vector2Int(19, 5));
        WestDoors.Add(new Vector2Int(20, 6));
        WestDoors.Add(new Vector2Int(13, 5));
    }


    public IEnumerator Build()
    {
        Debug.Log("RunTutorial " + RunTutorial);
        RunTutorial = true;
        if (GameObject.FindGameObjectsWithTag("tutorial").Length == 0)
        {
            RunTutorial = false;
            if (GameObject.FindGameObjectsWithTag("plot").Length == 0)
            {
                plot = Instantiate(plotManagerPrefab).GetComponent<PlotManager>();
            }
            else
            {
                plot = GameObject.FindGameObjectsWithTag("plot")[0].GetComponent<PlotManager>();
            }
            plot.levelManager = levelManager;
            plot.NewLocation();
           
        }
        //RunTutorial = true;//-----------------------------------------------------------------
        if (RunTutorial)
        {
            TutorialPreBuild();
            
            StartCoroutine(tutorial.TutorialCoroutine());
        }
        else
        {
            yield return (ProceduralLevel());

        }



        yield return null;
        foreach (Room room in AllRooms)
        {
            //yield return new WaitForSeconds(0.2f);
            Debug.Log("room" + room);
            room.PlaceFloors();

            room.GetWalls();
            room.GetLights();
            
        }
        GetDoors();
        
        //Прочая установка постоянных объектов, мб мебели.
        foreach (Room room in AllRooms)
        {
            //yield return new WaitForSeconds(0.2f);
            room.GetDecorator();

        }

        //Запуск врагов
        Finished = true;


    }
    bool Unseparate(Room room, Vector2 Max)
    {
        bool r = false;




        List<Vector2Int> perimetr = new List<Vector2Int>();
        List<Vector2Int> perimetrUp = new List<Vector2Int>();
        List<Vector2Int> perimetrDown = new List<Vector2Int>();
        List<Vector2Int> perimetrRight = new List<Vector2Int>();
        List<Vector2Int> perimetrLeft = new List<Vector2Int>();
        for (int i = room.rdBorder.y; i < room.luBorder.y; i++)//Верхние
        {
            if(cells[room.luBorder.x][i]==null)
                perimetrUp.Add(new Vector2Int(room.luBorder.x, i));
        }
        for (int i = room.rdBorder.y; i < room.luBorder.y; i++)//Нижние
        {
            if (cells[room.rdBorder.x - 1][i] == null)
                perimetrDown.Add(new Vector2Int(room.rdBorder.x-1, i));
        }
        for (int i = room.rdBorder.x; i < room.luBorder.x; i++)//Левые
        {
            if (cells[i][room.luBorder.y ] == null)
                perimetrLeft.Add(new Vector2Int(i, room.luBorder.y));
        }
        for (int i = room.rdBorder.x; i < room.luBorder.x; i++)//Правые
        {
            if (cells[i][room.rdBorder.y - 1] == null)
                perimetrRight.Add(new Vector2Int(i, room.rdBorder.y-1));
        }


        perimetr.AddRange(perimetrDown); perimetr.AddRange(perimetrUp); perimetr.AddRange(perimetrRight); perimetr.AddRange(perimetrLeft);
        //foreach (Vector2Int II in perimetr)
        //{
        //    PutRoom(II.x, II.x+1, II.y, II.y+1, RoomType.corridor);
        //}

        List<PotentialConnector> connectors = new List<PotentialConnector>();
        List<PotentialConnector> OtherConnectors = new List<PotentialConnector>();


        foreach (Vector2Int start in perimetrRight)
        {
            int x = start.x;
            int y = start.y;
            for (int i = y; i >0 ; i--)
            {
                if (cells[x][i] != null)
                {
                    Debug.Log("Right");
                    if (cells[x][i].room.access)
                    {
                        connectors.Add(new PotentialConnector(new Vector2Int(x, y+1), new Vector2Int(x+1, i+1 ), i-y));
                    }
                    else
                    {
                        OtherConnectors.Add(new PotentialConnector(new Vector2Int(x, y+1), new Vector2Int(x+1, i+1 ), i - y));

                    }
                    break;
                }
            }
        }

        foreach (Vector2Int start in perimetrDown)
        {
            int x = start.x;
            int y = start.y;
            for (int i = x; i > 0; i--)
            {
                if (cells[i][y] != null)
                {
                    if (cells[i][y].room.access)
                    {
                        connectors.Add(new PotentialConnector(new Vector2Int(x+1, y), new Vector2Int(i+1, y+1), i - x));
                    }
                    else
                    {
                        OtherConnectors.Add(new PotentialConnector(new Vector2Int(x+1, y), new Vector2Int(i+1, y + 1), i - x));

                    }
                    break;
                }
            }
        }


        foreach (Vector2Int start in perimetrLeft)
        {
            int x = start.x;
            int y = start.y;
            for (int i = y; i < Max.y; i++)
            {
                if (cells[x][i] != null)
                {
                    if (cells[x][i].room.access)
                    {
                        connectors.Add(new PotentialConnector(new Vector2Int(x, y), new Vector2Int(x+1, i ), i - y));
                    }
                    else
                    {
                        OtherConnectors.Add(new PotentialConnector(new Vector2Int(x, y), new Vector2Int(x+1, i ), i - y));

                    }
                    break;
                }
            }
        }
        foreach (Vector2Int start in perimetrUp)
        {
            int x = start.x;
            int y = start.y;
            for (int i = x; i < Max.x; i++)
            {
                if (cells[i][y] != null)
                {
                    if (cells[i][y].room.access)
                    {
                        connectors.Add(new PotentialConnector(new Vector2Int(x, y), new Vector2Int(i, y+1), i - y));
                    }
                    else
                    {
                        OtherConnectors.Add(new PotentialConnector(new Vector2Int(x, y), new Vector2Int(i, y + 1), i - y));

                    }
                    break;
                }
            }
        }
        PotentialConnector connector = null;
        bool GetAccess = false;
        if (connectors.ToArray().Length > 0)
        {
            GetAccess = true;
             connector = connectors[0];
            foreach (PotentialConnector thatconnector in connectors)
            {
                if (thatconnector.cost < connector.cost)
                {
                    connector = thatconnector;
                }
            }
        }
        else
        {
            if (OtherConnectors.ToArray().Length > 0)
            {
                connector = OtherConnectors[0];
                foreach (PotentialConnector thatconnector in OtherConnectors)
                {
                    if (thatconnector.cost < connector.cost)
                    {
                        connector = thatconnector;
                    }
                }
            }
            else
            {
               // AllRooms.Remove(room);
            }
        }

        if (connector != null)
        {
            r = true;
            Room connectorRoom;
            PutRoom(connector.Start.x, connector.Finish.x, connector.Start.y, connector.Finish.y, RoomType.corridor, out connectorRoom);
            if (GetAccess)
            {
                room.access = connectorRoom.access = true;
            }
        }
        else
        {
            r = false;

        }
        return r;
    }
    class PotentialConnector
    {
        public Vector2Int Start;
        public Vector2Int Finish;
        public int cost;

        public PotentialConnector(  Vector2Int Start,Vector2Int Finish,int cost)
        {
            this.Start=Start;
            this.Finish = Finish;
            this.cost = cost;
        }
    }


    // Поставь минивэн в 79.9f,2.2f,-14.3f
    IEnumerator ProceduralLevel()
    {
        int type=Random.Range(1,6);
       // type = 1;
        Debug.Log("type=" + type);
        int MaxA = 15;
        int MaxB = 15;
        Vector2Int Max = new Vector2Int(MaxA, MaxB);

        Room Entrance; PutRoom(5, 10, 2, 4, RoomType.corridor,out Entrance);
        Entrance.access = true;
        Entrance.SquareLimit = 1;
        WestDoors.Add(new Vector2Int(6,1));


        RoomType[] roomTypes = new RoomType[0];
        switch (type)
        {
            case 0:
                for (int i = 0; i < 14; i++)
                {
                    int a = Random.Range(2, MaxA);
                    int b = Random.Range(2, MaxB);
                    //Debug.Log(a + "," + b);
                    while (cells[a][b] != null)
                    {
                        a = Random.Range(2, MaxA);
                        b = Random.Range(2, MaxB);
                    }
                    PutRoom(a, a + 1, b, b + 1, RoomType.bathroom);
                }
                break;
            case 1:
                roomTypes = new RoomType[]{ RoomType.Office, RoomType.Office, RoomType.Office, RoomType.Office, RoomType.Kitchen, RoomType.Office, RoomType.Office, RoomType.Office, RoomType.Office, RoomType.bathroom, RoomType.HotelHall, RoomType.HotelHall, RoomType.HotelHall, RoomType.Restaraunt };
                break;
            case 2:
                roomTypes = new RoomType[] { RoomType.Office, RoomType.Restaraunt, RoomType.HotelHall, RoomType.Kitchen, RoomType.corridor, RoomType.corridor, RoomType.bathroom, RoomType.Living, RoomType.corridor };
                break;
            case 3:
                roomTypes = new RoomType[] { RoomType.Living, RoomType.Living, RoomType.Living, RoomType.bathroom, RoomType.Restaraunt, RoomType.Kitchen };
                break;
            case 5:
                roomTypes = new RoomType[] { RoomType.Living,RoomType.bathroom,RoomType.Restaraunt};
                break;
            case 4:
                MaxA = 30;
                MaxB = 30;
                Max = new Vector2Int(MaxA, MaxB);
                PutRoom(7, 9, 4, MaxB-2, RoomType.corridor);//Рисуем коридор
                //В случайном месте разместим перекрёсток
                int p = Random.Range(4, MaxB - 2);

                Room corridor;
                PutRoom(9, MaxA-2, p, p+2, RoomType.corridor);//Коридор вверх
                PutRoom(MaxA - 2, MaxA - 1, p, p + 2, RoomType.bathroom);//Коридор вверх
                int g = 9;
                for (int i = 0; i < Random.Range(2,4); i++)//Номера по пути наверх
                {
                    g+= Random.Range(2, 5);
                    PutRoom(g, g+1, p+2, p + 3, RoomType.Living);
                    if(g> MaxA - 2)
                    {
                        break;
                    }
                }
                g = 9;
                for (int i = 0; i < Random.Range(2, 4); i++)//Номера по пути наверх
                {
                    g += Random.Range(2, 5);
                    PutRoom(g, g + 1, p -1, p -2, RoomType.Living);
                    if (g > MaxA - 2)
                    {
                        break;
                    }
                }
                PutRoom(7, 2, p, p + 2, RoomType.corridor);
                PutRoom(7, 2, p - 1, p - 2, RoomType.Kitchen);
                PutRoom(7, 2, p+ 2, p+ 3, RoomType.Restaraunt);


                break;
        }


        //Typical room putting
        foreach (RoomType roomtype in roomTypes)
        {
            bool CanBePut = false;
            int Count = 0;
            while (!CanBePut && (Count < 250))
            {
                int a = Random.Range(2, MaxA);
                int b = Random.Range(2, MaxB);
                CanBePut = true;
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (cells[a + i][b + j] != null)
                        {
                            CanBePut = false;
                        }
                    }
                }
                if (CanBePut)
                {
                    PutRoom(a, a + 3, b, b + 3, roomtype);
                }

            }


        }










        //Expand
        bool Expangeable = true;
        Vector2Int[] Direction= { new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(-1, 0), new Vector2Int(0, -1) };
        while(Expangeable)
        {
            Expangeable = false;
            foreach (Vector2Int Dir in Direction)
            {


                foreach (Room room in AllRooms)
                {
                    if (room.expand(Dir, Max))
                    {
                        
                        Expangeable = true;
                    }


                }
            }
        }
        
        //Putting Doors
        foreach (Room room in AllRooms)
        {
            
            for (int y = room.rdBorder.y; y < room.luBorder.y; y++)
            {
                
                
                if (cells[room.luBorder.x][y] != null)
                {
                    int x = room.luBorder.x;
                    if (cells[x][y].room != null)
                    {
                        if (!room.Connected.Contains(cells[x][y].room))
                        {

                            Room that = cells[x][y].room;
                            bool w = true; int newY = y;
                            while (w)
                            {

                                newY++;
                                if(newY== room.luBorder.y)
                                {
                                    newY--;
                                    break;
                                }
                                if (cells[x][newY] == null || cells[x][newY].room != that)
                                {
                                    w = false;
                                }

                            }
                            int door_y =Mathf.FloorToInt( (newY+y)/2f);






                            NorthDoors.Add(new Vector2Int(x-1, door_y));
                            that.Connected.Add(room);
                            room.Connected.Add(that);
                            y = room.rdBorder.y;


                        }
                       
                    }
                }
               
            }
            for (int x = room.rdBorder.x; x < room.luBorder.x; x++)
            {


                if (cells[x][room.luBorder.y] != null)
                {
                    int y = room.luBorder.y;
                    if (cells[x][y].room != null)
                    {
                        if (!room.Connected.Contains(cells[x][y].room))
                        {

                            Room that = cells[x][y].room;
                            bool w = true; int newX = x;
                            while (w)
                            {

                                newX++;
                                if (newX == room.luBorder.x)
                                {
                                    newX--;
                                    break;
                                }
                                if (cells[newX][y] == null || cells[newX][y].room != that)
                                {
                                    w = false;
                                }

                            }
                            int door_x = Mathf.FloorToInt((newX + x) / 2f);






                            WestDoors.Add(new Vector2Int(door_x, y-1));
                            that.Connected.Add(room);
                            room.Connected.Add(that);
                            x = room.rdBorder.x;


                        }

                    }
                }

            }

        }


        //Checking if all have access to exit
        bool AccessCounting = true;
        while (AccessCounting)
        {
            AccessCounting = false;
            foreach (Room room in AllRooms)
            {
                if (room.PromoteAccess())
                {
                    AccessCounting = true;
                }
            }
        }

        List<Room> separated = new List<Room>();
        foreach (Room room in AllRooms)
        {
            if (!room.access)
            {
                separated.Add(room);
            }
        }
        Debug.Log("separated"+separated.ToArray().Length);
        //Building corridors for cases when room has no access
        bool BCounting = true;
        List<Room> unseparated = new List<Room>();
        while (BCounting)
        {
            BCounting = false;
            foreach (Room unsep in separated)
            {
                if (Unseparate(unsep, Max))
                {
                    BCounting = true;
                    unseparated.Add(unsep);
                }
            }
            foreach (Room t in unseparated)
            {
                separated.Remove(t);
            }
           
        }


        //Putting Doors
        foreach (Room room in AllRooms)
        {

            for (int y = room.rdBorder.y; y < room.luBorder.y; y++)
            {


                if (cells[room.luBorder.x][y] != null)
                {
                    int x = room.luBorder.x;
                    if (cells[x][y].room != null)
                    {
                        if (!room.Connected.Contains(cells[x][y].room))
                        {

                            Room that = cells[x][y].room;
                            bool w = true; int newY = y;
                            while (w)
                            {

                                newY++;
                                if (newY == room.luBorder.y)
                                {
                                    newY--;
                                    break;
                                }
                                if (cells[x][newY] == null || cells[x][newY].room != that)
                                {
                                    w = false;
                                }

                            }
                            int door_y = Mathf.FloorToInt((newY + y) / 2f);






                            NorthDoors.Add(new Vector2Int(x - 1, door_y));
                            that.Connected.Add(room);
                            room.Connected.Add(that);
                            y = room.rdBorder.y;


                        }

                    }
                }

            }
            for (int x = room.rdBorder.x; x < room.luBorder.x; x++)
            {


                if (cells[x][room.luBorder.y] != null)
                {
                    int y = room.luBorder.y;
                    if (cells[x][y].room != null)
                    {
                        if (!room.Connected.Contains(cells[x][y].room))
                        {

                            Room that = cells[x][y].room;
                            bool w = true; int newX = x;
                            while (w)
                            {

                                newX++;
                                if (newX == room.luBorder.x)
                                {
                                    newX--;
                                    break;
                                }
                                if (cells[newX][y] == null || cells[newX][y].room != that)
                                {
                                    w = false;
                                }

                            }
                            int door_x = Mathf.FloorToInt((newX + x) / 2f);






                            WestDoors.Add(new Vector2Int(door_x, y - 1));
                            that.Connected.Add(room);
                            room.Connected.Add(that);
                            x = room.rdBorder.x;


                        }

                    }
                }

            }

        }




        StartCoroutine(enemyManager.ProceduralSpawn(plot.Hard));


        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
