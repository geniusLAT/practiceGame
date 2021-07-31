using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GUI : MonoBehaviour
{
    public GUItext Center;
    public Texture image;
    public RawImage[] rawImage;
    // Start is called before the first frame update
    void Start()
    {
        Center.Print("");
        rawImage[0].enabled = rawImage[1].enabled = false;
    }
    public void ShowImage(Texture texture, int i)
    {
        rawImage[i].enabled = true;
        rawImage[i].texture = texture;
    }
    public void HideImage()
    {
        rawImage[0].enabled = rawImage[1].enabled = false;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
