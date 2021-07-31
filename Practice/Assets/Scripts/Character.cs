using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour
{
    public int HP;
    public GameObject RH;
    public GameObject RHobject;
    LevelManager levelManager;


    public GameObject BloodOnFloorPrefab;
    public GameObject KnifeBloodOnFloorPrefab;
    public GameObject Eye;
    public GameObject DeadEye;
    public GameObject BodyEffect;
    public Transform  RayStart;

    public Texture[] textures;
    public bool MainBluntObject;
    
    // Start is called before the first frame update
    void Start()
    {

        levelManager = GameObject.FindWithTag("Manager").GetComponent<LevelManager>();
        levelManager.Alives.Add(this);
    }

    GameObject Look(Vector3 Where)
    {
        RayStart.LookAt(Where);
        Ray ray = new Ray(RayStart.position, RayStart.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            return hit.transform.gameObject;
           
        }
        else
        {
            return null;
        }
    }

    GameObject findParent(GameObject original)
    {
        if (original != null)
        {
            if (original.transform.parent == null)
            {
                return original;
            }
            else
            {
                GameObject final = original.transform.parent.gameObject;
                while (final.transform.parent != null)
                {
                    final = final.transform.parent.gameObject;
                }
                return final;
            }
        }
        else
        {
            return original;
        }
      
    }
    public bool DoISee(GameObject he)
    {
        if (findParent(Look(he.transform.position)) == he)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (RHobject != null)
        {
            RHobject.transform.position = RH.transform.position;
            RHobject.transform.rotation = RH.transform.rotation;
        }
    }
    public IEnumerator TakeNearestGun()
    {
        Debug.Log("F2");
        Gun gun = levelManager.findGunNear(transform.position);
        yield return TakeGun(gun);
    }
    public IEnumerator TakeGun(Gun gun)
    {
        if (gun != null)
        {
            StartCoroutine(gun.BeTaken(this));
        }
        yield return null;
    }
    public IEnumerator TakeGun(GameObject g)
    {
        if (g != null)
        {
            if (g.GetComponent<Gun>() != null)
            {
                StartCoroutine(g.GetComponent<Gun>().BeTaken(this));
            }
        }
       
        yield return null;
    }
    public IEnumerator DropGun()
    {
        if (RHobject != null)
        {
            Gun gun = RHobject.GetComponent<Gun>();
            if (gun != null)
            {
                Debug.Log("FF2");
                StartCoroutine(gun.BeDropped());
            }
        }
       
        yield return null;
    }

    public void GetShot(int Damage,Vector3 Damager)
    {
        if (HP > 1)
        {
            HP -= Damage;
            if (HP < 1)
            {
                Death(Damager);
                GetWound(1);
                StartCoroutine(PutBloodOnFloor());
                
            }
        }
       
    }

    void GetWound(int type)
    {
        BodyEffect.SetActive(true);
        if (type == 1)
        {

            BodyEffect.transform.GetComponent<MeshRenderer>().materials[0].SetTexture("_MainTex", textures[0]);
            StartCoroutine(PutBloodOnFloor());
        }
        if (type == 2)
        {
            Debug.Log("GetWound 2");

            BodyEffect.transform.GetComponent<MeshRenderer>().materials[0].SetTexture("_MainTex", textures[Random.Range(1,3)]);
            StartCoroutine(PutKnifeBloodOnFloor());
            
        }

    }

    IEnumerator PutBloodOnFloor()
    {
        yield return new WaitForSeconds(2f);
        GameObject Blood = Instantiate(BloodOnFloorPrefab);
        Blood.transform.position = new Vector3(transform.position.x, 0.73f, transform.position.z);

        Blood.transform.Rotate(new Vector3(0, Random.Range(0f, 360f), 0));
        for (float i = 0; i < 1; i+=0.001f)
        {
           
            Blood.transform.localScale = new Vector3(i, i, i);
            yield return null;
        }
        

    }
    IEnumerator PutKnifeBloodOnFloor()
    {
        
        GameObject Blood = Instantiate(KnifeBloodOnFloorPrefab);
        Blood.transform.position = new Vector3(transform.position.x, 0.73f, transform.position.z)+transform.right*4;
        Blood.transform.Rotate(new Vector3(0, Blood.transform.eulerAngles.y, 0));

        for (float i = 0; i < 0.4f; i += 0.001f)
        {

            Blood.transform.localScale = new Vector3(i, i, i);
            yield return null;
        }


    }
    public void Death(Vector3 Damager)
    {
        StartCoroutine(DropGun());
        GetComponent<NavMeshAgent>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().freezeRotation = false;
        Eye.SetActive(false);
        DeadEye.SetActive(true);
        levelManager.Alives.Remove(this);
        Debug.Log("Dead");

        if (GetComponent<Enemy>())
        {

            GetComponent<Enemy>().Stop();
            GetComponent<Enemy>().enabled = false;
        }
        if (GetComponent<PlayerControl>())
        {

            
            GetComponent<NavMeshAgent>().enabled = GetComponent<PlayerControl>().Freedom =GetComponent<PlayerControl>().CursorLook = false;
        }

    }
    public void Melee()
    {
        Debug.Log("Melee");
        Character Victim = levelManager.findCharacterNear(transform.position,this);
        if (Victim != null)
        {
            Victim.GetMeleeAttacked(80, MainBluntObject, transform.position);
        }
    }

    public void Interact()
    {
        Debug.Log("Interact");
        Interactive Victim = levelManager.findInteractiveNear(transform.position);
        if (Victim != null)
        {
            Victim.GetUsed(this);
        }
    }

    public void GetMeleeAttacked(int Damage,bool blunt, Vector3 Damager)
    {

        Debug.Log("GetMeleeAttacked");
        Debug.Log(HP);
        if (HP > 1)
        {
            Debug.Log("HP > 1");
            HP -= Damage;
            Debug.Log(HP);
            if (HP < 1)
            {
                //GetComponent<Rigidbody>().AddTorque(new Vector3(1, 1, 1));
                transform.Rotate(new Vector3(-25, 0, 0));
                Death(Damager);
                GetWound(2);
                

            }
        }

    }
}
