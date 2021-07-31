using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GUItext : MonoBehaviour
{
    public Text Second;
    public Text First;
    // Start is called before the first frame update
    void Start()
    {
        First = GetComponent<Text>();
    }

    public void Print(string message)
    {
        if (First == null)
        {
            First = GetComponent<Text>();
        }
        First.text = Second.text = message;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
