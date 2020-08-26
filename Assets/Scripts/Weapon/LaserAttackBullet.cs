using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserAttackBullet : MonoBehaviour
{
    private int damage;
    private float chargeWidth = 0.2f;
    private float shootWidth = 1;

    private LineRenderer lr;
    private List<GameObject> damagedObjects = new List<GameObject>();

    public IEnumerator LaserBulletRoutine(int _damage, Vector2 startPos, Vector2 endPos, float chargeTime, float shootTime)
    {
        damage = _damage;

        lr = GetComponent<LineRenderer>();

        lr.SetPosition(0, startPos);
        lr.SetPosition(1, endPos);
        
        GetComponent<BoxCollider2D>().size = new Vector2(Vector2.Distance(startPos, endPos), shootWidth);

        LaserAttackBulletStunDebuff debuff = new LaserAttackBulletStunDebuff();
        debuff.Init(chargeTime + shootTime);
        Game.inst.player.AddBuffable(debuff);

        lr.startWidth = chargeWidth;
        yield return new WaitForSeconds(chargeTime);

        lr.startWidth = shootWidth;
        GetComponent<Collider2D>().enabled = true;
        yield return new WaitForSeconds(shootTime);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!damagedObjects.Contains(collision.gameObject))
        {
            damagedObjects.Add(collision.gameObject);

            collision.GetComponent<IDamagable>()?.GetDamaged(damage);
        }
    }
}

public class LaserAttackBulletStunDebuff : Buffable
{
    public override void StartDebuff(Player player)
    {

    }

    public override void Apply(Player player)
    {
        player.SetPlayerControllable(false);
    }

    public override void EndDebuff(Player player)
    {
        player.SetPlayerControllable(true);
    }
}