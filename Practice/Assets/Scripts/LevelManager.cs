using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public EnemyManager enemyManager;
    public Transform player;
    public List<Gun> guns;
    public List<Character> Alives;
    public List<Interactive> interactives;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator Move(Transform Thing, Vector3 finish, int frames)
    {
        Vector3 Start = Thing.transform.position;
        Vector3 Delta = (finish - Start) / 15;
        for (int i = 0; i < 15; i++)
        {
            Thing.position = Thing.position + Delta;
            yield return null;
        }
        yield return null;
    }

    public Gun findGunNear(Vector3 pos)
    {
        Debug.Log("F3");
        float min = 1000;
        Gun c=null;
        foreach (Gun that in guns)
        {
            float Distance = Vector3.Distance(that.transform.position, pos);
            if (Distance < min)
            {
                min = Distance;
                c = that;
            }
        }
        if (min > 5) c = null;
        Debug.Log(c);
        return c;
    }

    public Interactive findInteractiveNear(Vector3 pos)
    {
        Debug.Log("F3");
        float min = 1000;
        Interactive c = null;
        foreach (Interactive that in interactives)
        {
            float Distance = Vector3.Distance(that.transform.position, pos);
            if (Distance < min)
            {
                min = Distance;
                c = that;
            }
        }
        if (min > 10) c = null;
        Debug.Log(c);
        return c;
    }

    public Character findCharacterNear(Vector3 pos, Character exception)
    {
        Debug.Log("F3");
        float min = 1000;
        Character c = null;
        foreach (Character that in Alives)
        {
            float Distance = Vector3.Distance(that.transform.position, pos);
            if (Distance < min)
            {
                if (that != exception)
                {
                    min = Distance;
                    c = that;
                }
            }
        }
        if (min > 5) c = null;
        Debug.Log(c);
        return c;
    }

    public IEnumerator Shoot(Vector3 coord)
    {
        yield return null;
    }
}
