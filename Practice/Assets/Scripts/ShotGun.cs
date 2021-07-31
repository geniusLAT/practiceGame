using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotGun : Gun
{
    public GameObject BulletPrefab;
    List<EnemyManager> enemyManagers;
    public Transform RayStart;
    // Start is called before the first frame update
    void Start()
    {
        audioPlayer = GetComponent<AudioSource>();

        enemyManagers = new List<EnemyManager>();
        foreach (GameObject One in GameObject.FindGameObjectsWithTag("EnemyManager"))
        {
            enemyManagers.Add(One.GetComponent<EnemyManager>());
        }
        body = GetComponent<Rigidbody>();
        levelManager = GameObject.FindWithTag("Manager").GetComponent<LevelManager>();
        if (Keeper == null)
        {
            levelManager.guns.Add(this);
        }
    }
    void Noise(float Range)
    {
        foreach(EnemyManager One in enemyManagers)
        {
            One.Noise(transform.position, 40);
        }
    }
    public override IEnumerator Atack(Vector3 target)
    {
        if (CanShoot && (Ammo > 0))
        {
            audioPlayer.Play();
            // Debug.Log("Boom");
            Vector3 m;
            GameObject Bullet = Instantiate(BulletPrefab);
            Bullet.transform.position = RayStart.position;

            Ray ray = new Ray(RayStart.position, RayStart.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                m = hit.point;
                
                StartCoroutine(Bullet.GetComponent<Bullet>().Shoot(m));
                //Debug.Log(hit.point);
                //Debug.Log("Got");
            }
            else
            {
                Bullet.transform.rotation = transform.rotation;
                StartCoroutine(Bullet.GetComponent<Bullet>().Shoot());
               // Debug.Log("AU");
            }
            Bullet.GetComponent<Bullet>().Damage = 20;



            yield return null;
            CanShoot = false;
            Noise(15);
            yield return new WaitForSeconds(TimePerShot);
            CanShoot = true;
        }
       
        //return base.Atack(target);
    }

    private void OnDestroy()
    {
        levelManager.guns.Remove(this);
    }
}
