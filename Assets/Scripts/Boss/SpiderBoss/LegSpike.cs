using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegSpike : MonoBehaviour
{
    float riseDistance;
    float startPoint;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            Game.inst.player.GetDamaged(0.5f);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        riseDistance = GetComponent<BoxCollider2D>().size.y;
        startPoint = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < startPoint + riseDistance)
        {
            transform.position += new Vector3(0, riseDistance / 20);
        }
    }
}
