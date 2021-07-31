using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamScript : MonoBehaviour
{
    public Transform Cursor;
    public Transform Subject;
    Camera c;
    Vector3 m;

    // Start is called before the first frame update
    void Start()
    {
        c = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Subject.transform.position + new Vector3(1.678f, 24.96f, -31.48f);

        Ray ray = c.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            m = hit.point;
        }
        Cursor.position = m + new Vector3(0, 1, 0);
    }
}
