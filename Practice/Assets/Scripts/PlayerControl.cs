using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class PlayerControl : MonoBehaviour
{
    Rigidbody rb;
    public bool CursorLook = true;
    public bool Freedom = true;
    public float speed;
    public Transform Cursor;
    public Character character;
    NavMeshAgent agent;
    Transform RH;
    public bool turned;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        character = GetComponent<Character>();
        RH = character.RH.transform;
        agent = GetComponent<NavMeshAgent>();
    }
    public  IEnumerator Melee()
    {
        Freedom = false;
        character.Melee();

        yield return new WaitForSeconds(0.5f);
        Freedom = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (Freedom)
        {
            Vector3 Where = transform.position;
            
            if (Input.GetKeyDown(KeyCode.F))
            {
                
                if (character.RHobject == null)
                {
                    StartCoroutine(character.TakeNearestGun());
                }
                else
                {
                    Debug.Log("FF");
                    StartCoroutine(character.DropGun());
                }
            }
            if (Input.GetKey(KeyCode.Mouse0))
            {
               
                if (character.RHobject != null)
                {
                    if (character.RHobject.GetComponent<Gun>())
                    {
                        StartCoroutine(character.RHobject.GetComponent<Gun>().Atack(Cursor.position));
                    }
                }
            }
            if (turned)
            {
                if (Input.GetKey(KeyCode.A))
                {
                    //rb.AddForce(Vector3.forward * speed);
                    Where += speed * new Vector3(0, 0, 1);

                }
                if (Input.GetKey(KeyCode.S))
                {
                    Where += -speed * new Vector3(1, 0, 0);

                }
                if (Input.GetKey(KeyCode.D))
                {
                    Where += -speed * new Vector3(0, 0, 1);


                }
                if (Input.GetKey(KeyCode.W))
                {
                    Where += speed * new Vector3(1, 0, 0);

                }
            }
            else
            {
                if (Input.GetKey(KeyCode.W))
                {
                    //rb.AddForce(Vector3.forward * speed);
                    Where += speed * new Vector3(0, 0, 1);

                }
                if (Input.GetKey(KeyCode.A))
                {
                    Where += -speed * new Vector3(1, 0, 0);

                }
                if (Input.GetKey(KeyCode.S))
                {
                    Where += -speed * new Vector3(0, 0, 1);


                }
                if (Input.GetKey(KeyCode.D))
                {
                    Where += speed * new Vector3(1, 0, 0);

                }

            }
            if (agent.enabled)
            {
                agent.destination = Where;
            }
            
            if (Input.GetKeyDown(KeyCode.E))
            {
                character.Interact();
            }

            if (Input.GetKey(KeyCode.Q))
            {
                StartCoroutine(Melee());
            }
            if (CursorLook)
            {
                if (Vector3.Distance(transform.position, Cursor.position) > 6)
                {
                    RH.LookAt(Cursor);
                    transform.rotation = new Quaternion(0, RH.rotation.y, 0, RH.rotation.w);
                    //transform.Rotate(new Vector3(0, RH.eulerAngles.y, 0));
                    //Debug.Log(Vector3.Distance(transform.position, Cursor.position));
                }
                
            }
        }
      
    }
}
