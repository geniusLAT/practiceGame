using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileToShow : Interactive
{
    public Texture file;
    public bool Showed;
    // Start is called before the first frame update
    void Start()
    {
        GetStarted();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void GetUsed(Character User)
    {
        if (User.gameObject.GetComponent<PlayerControl>())
        {
            StartCoroutine(Show());
        }
    }
    IEnumerator Show()
    {
        GameObject.FindGameObjectWithTag("GUI").GetComponent<GUI>().ShowImage(file,1);
        yield return new WaitForSeconds(0.5f);
        while (!(      (Input.GetKeyUp(KeyCode.Escape)) || (Input.GetKeyUp(KeyCode.E))||(Input.GetKeyUp(KeyCode.Space))))
        {
            yield return null;
        }
        GameObject.FindGameObjectWithTag("GUI").GetComponent<GUI>().HideImage();
        Showed = true;
        yield return null;
    }
}
