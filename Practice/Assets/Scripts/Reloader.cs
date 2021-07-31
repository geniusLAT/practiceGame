using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Reloader : MonoBehaviour
{ Coroutine coroutine;
    public Character player;
    // Start is called before the first frame update
    void Start()
    {

        
        coroutine= StartCoroutine(Reload());

    }
    IEnumerator Reload()
    {
        yield return null;
        while (true)
        {
            yield return null;
            if (player == null)
            {
                Destroy(this.gameObject);
                break;
                
            }
            if (player.HP < 1)
            {
                string scene = SceneManager.GetActiveScene().name;
                DontDestroyOnLoad(gameObject);
                SceneManager.UnloadSceneAsync(scene);
                yield return new WaitForSeconds(1);
                SceneManager.LoadScene(scene);
                yield return new WaitForSeconds(1);
                Destroy(this.gameObject);
                break;
            }

        }
       
    }
   public void Stop()
    {
        Debug.Log("It was stopped");
        StopCoroutine(coroutine);
        Destroy(this.gameObject);
    }
}
