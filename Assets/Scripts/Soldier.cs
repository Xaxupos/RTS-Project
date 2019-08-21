using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : Unit, ISelectable
{
    [Header("Soldier")]
    [Range(0, 0.3f), SerializeField]
    float shootingDuration = 0;
    [SerializeField]
    ParticleSystem muzzleEffect, impactEffect;
    [SerializeField]
    LayerMask shootingLayerMask;
    LineRenderer lineEffect;

    protected override void Awake()
    {
        base.Awake();
        lineEffect = muzzleEffect.GetComponent<LineRenderer>();
        impactEffect.transform.SetParent(null);
        EndShootEffect();
    }

    public void SetSelected(bool selected)
    {
        healthBar.gameObject.SetActive(selected);
    }

    private void Command(Vector3 destination)
    {
        nav.SetDestination(destination);
        task = Task.move;
        target = null;
    }
    private void Command(Soldier soldierToFollow)
    {
        target = soldierToFollow.transform;
        task = Task.follow;
    }
    private void Command(Dragon dragonToAttack)
    {
        target = dragonToAttack.transform;
        task = Task.chase;
    }

    public override void DealDamage()
    {
        if(shoot()) base.DealDamage();
    }

    bool shoot()
    {
        Vector3 start = muzzleEffect.transform.position;
        Vector3 direction = transform.forward;



        RaycastHit hit;
        if (Physics.Raycast(start, direction, out hit, shootingDistance, shootingLayerMask))
        {
            StartShootEffect(start, hit.point, true);
            var unit = hit.collider.gameObject.GetComponent<Unit>();
            if (unit) return true;
            return unit;
        }
        StartShootEffect(start, start + direction*shootingDistance, false);
        return false;
    }

    void StartShootEffect(Vector3 lineStart, Vector3 lineEnd, bool hitSomething)
    {
        if(hitSomething)
        {
            impactEffect.transform.position = lineEnd;
            impactEffect.Play();
        }

        lineEffect.SetPositions(new Vector3[] { lineStart, lineEnd });
        lineEffect.enabled = true;
        muzzleEffect.Play();
        Invoke("EndShootEffect", shootingDuration);
    }
    void EndShootEffect()
    {
        lineEffect.enabled = false;
    }

    public override void ReceiveDamage(float damage, Vector3 damageDealerPosition)
    {
        base.ReceiveDamage(damage, damageDealerPosition);
        animator.SetTrigger("Get Hit");
    }

}
