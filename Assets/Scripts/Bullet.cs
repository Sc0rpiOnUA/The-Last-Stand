using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    [SerializeField]
    private float speed, lifeTime;
    [SerializeField]
    private GameObject playerIceBullet, enemyFireBullet;

    public bool isPlayerBullet;

    IEnumerator DestroyBulletAfterTime()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }

    private void Start()
    {
        StartCoroutine(DestroyBulletAfterTime());
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {

        if(other.gameObject.CompareTag("Player") && gameObject.CompareTag("EnemyBullet"))
            Destroy(gameObject);
        else if (other.gameObject.CompareTag("Enemy") && gameObject.CompareTag("PlayerBullet"))
        {
            //other.gameObject.GetComponent<EnemyController>().HitByBullet();
            Destroy(gameObject);
        }
            
    }
}
