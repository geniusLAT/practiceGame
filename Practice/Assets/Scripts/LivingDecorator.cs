using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingDecorator : Decorator
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void Decorate(Architector.Room room)
    {
        Architector ar = room.architector;
        List<Architector.Cell> cells = room.RoomCells;
        List<Architector.Cell> potentials = new List<Architector.Cell>(); potentials.AddRange(cells);


        //Ставим тумбу
        foreach (Architector.Cell c in cells)
        {
            if (c.busy != null || c.NorthWall == null)
            {
                potentials.Remove(c);
            }

        }
        Architector.Cell place = potentials[Random.Range(0, potentials.ToArray().Length)];
        room.architector.PutFurniture(decors[0], place.a, place.b);


        //Ставим кровать
        potentials = new List<Architector.Cell>(); potentials.AddRange(cells);

        foreach (Architector.Cell c in cells)
        {

            int count = 0;
            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    count++;
                    //room.architector.PutFurniture(decors[2], c.a + x, c.b + y);
                    if (ar.cells[c.a + x][c.b + y] == null)
                    {
                        potentials.Remove(c);

                    }
                    else
                    {
                        if (ar.cells[c.a + x][c.b + y].busy != null)
                        {
                            potentials.Remove(c);
                        }
                        else
                        {

                            if (ar.cells[c.a + x][c.b + y].room != room)
                            {
                                //Debug.Log("ar.cells[" + (c.a + x) + "][" + (c.b + y) + "] is not place");
                                potentials.Remove(c);
                            }

                        }

                    }
                }

            }

            Debug.Log("count " + count);






        }
        Debug.Log("We have " + potentials.ToArray().Length + " options");
        if (potentials.ToArray().Length > 0)
        {
             place = potentials[Random.Range(0, potentials.ToArray().Length)];

            GameObject table = room.architector.PutFurniture(decors[1], place.a, place.b);
            ar.cells[place.a + 1][place.b + 1].busy = ar.cells[place.a + 1][place.b].busy = ar.cells[place.a][place.b + 1].busy = table;
        }

        //Ставим кресла
        potentials = new List<Architector.Cell>();
        for (int i = 0; i < Random.Range(1,4); i++)
        {
            potentials.AddRange(cells);


           
            foreach (Architector.Cell c in cells)
            {
                if (c.busy != null )
                {
                    potentials.Remove(c);
                }

            }
            place = potentials[Random.Range(0, potentials.ToArray().Length)];
            room.architector.PutFurniture(decors[2], place.a, place.b);
        }


        //Ставим шкаф
        potentials = new List<Architector.Cell>();
        potentials.AddRange(cells);
        foreach (Architector.Cell c in cells)
        {
            if (c.busy != null || c.WestWall == null)
            {
                potentials.Remove(c);
            }

        }
        if (potentials.ToArray().Length > 0)
        {
            place = potentials[Random.Range(0, potentials.ToArray().Length)];
            room.architector.PutFurniture(decors[3], place.a, place.b);
        }





    }

}
