using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestarauntDecorator : Decorator
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public override void Decorate(Architector.Room room)
    {
        Architector ar = room.architector;
        //Putting BookShelfs
        List<Architector.Cell> cells = room.RoomCells;
        List<Architector.Cell> potentials = new List<Architector.Cell>(); potentials.AddRange(cells);
        foreach (Architector.Cell c in cells)
        {
            if (c.busy != null || c.NorthWall == null)
            {
                potentials.Remove(c);
            }

        }
        foreach (Architector.Cell that in potentials)
        {
            if (Random.Range(0, 3) < 1)
            {
                
                ar.PutFurniture(decors[0], that.a, that.b);
            }
        }


        //Putting tables
        for (int i = 0; i < Random.Range(1, 4); i++)
        {
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
                        if (ar.cells[c.a+x][c.b+y] == null)
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
            if (potentials.ToArray().Length>0)
            {
                Architector.Cell place = potentials[Random.Range(0, potentials.ToArray().Length)];
                
                GameObject table = room.architector.PutFurniture(decors[1], place.a, place.b);
                ar.cells[place.a + 1][place.b + 1].busy = ar.cells[place.a + 1][place.b].busy = ar.cells[place.a][place.b + 1].busy = table;
            }
           
        }
       
        base.Decorate(room);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
