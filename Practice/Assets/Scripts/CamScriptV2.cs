using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamScriptV2 : MonoBehaviour
{
    public Transform Little;
    public Transform Big;
    public Transform Cursor;
    public Transform Subject;
    public GameObject CamCube;
    bool Zoomed = true;
    Camera c;
    Vector3 m;
    bool SubjectExist;
    Rigidbody rigidbody;
    float Max = 0;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = CamCube.GetComponent<Rigidbody>();
        c = GetComponent<Camera>();
        if (Subject != null)
        {
            SubjectExist = true;
            CamCube.transform.position = Subject.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Max< Vector3.Distance(CamCube.transform.position, Subject.transform.position))
        {
            Max = Vector3.Distance(CamCube.transform.position, Subject.transform.position);
            //Debug.Log(Max);
        }


        if (SubjectExist)
        {
            float delta = Vector3.Distance(CamCube.transform.position, Subject.transform.position);
            if (delta > 6.5)
            {
                Vector3 t = Subject.transform.position - CamCube.transform.position;
                rigidbody.AddForce(t * 2);
            }
            if (delta > 7.5)
            {
                Vector3 t = Subject.transform.position - CamCube.transform.position;
                rigidbody.AddForce(t*3);
            }
            if (delta > 16)
            {
                Vector3 t = Subject.transform.position - CamCube.transform.position;
                rigidbody.AddForce(t*10);
            }
        }




        Ray ray = c.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            m = hit.point;
        }
        Cursor.position = m + new Vector3(0, 1, 0);
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            Zoomed = !Zoomed;
            if (Zoomed)
            {
                c.transform.position = Little.position;
            }
            else
            {
                c.transform.position = Big.position;

            }
        }
    }
}
