using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarbageBomb : MonoBehaviour
{
    [SerializeField]
    private GameObject Explosion;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.GetComponent<Player>() != null)
        {
            GameObject prefab = Instantiate(Explosion, transform.position, Quaternion.identity)as GameObject;
            Destroy(prefab,0.5f);
            Destroy(transform.parent.gameObject);
            Destroy(gameObject);
        }
    }
}