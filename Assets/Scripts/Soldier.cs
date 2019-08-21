using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : Unit, ISelectable
{

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
        //todo
    }

}
