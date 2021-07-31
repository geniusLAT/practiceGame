using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecoratorBathroom : Decorator
{
    //0 - toilet, it has to be on North wall
    // Start is called before the first frame update
    void Start()
    {
        
    }


    public override void Decorate(Architector.Room room)
    {
        //Putting Toilet
        List <Architector.Cell> cells = room.RoomCells;
        List<Architector.Cell> potentials = new List<Architector.Cell>(); potentials.AddRange(cells);
        foreach (Architector.Cell c in cells)
        {
            if(c.busy!=null || c.NorthWall == null)
            {
                potentials.Remove(c);
            }
           
        }
        Debug.Log("potentials " + potentials.ToArray().Length);
        Architector.Cell place = potentials[Random.Range(0, potentials.ToArray().Length)];
        room.architector.PutFurniture(decors[0], place.a, place.b);
        base.Decorate(room);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
