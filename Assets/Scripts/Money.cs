using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Money : MonoBehaviour
{

    Text text;
    static Money money;

    [SerializeField]
    ulong moneyAmount;
    [SerializeField]
    ulong maxMoney=1000000;

    private void Awake()
    {
        text = GetComponentInChildren<Text>(true);
        money = this;
    }

    private void Update()
    {
        text.text = moneyAmount + " $";
    }

    public static bool TryAddMoney(uint value)
    {
        money.moneyAmount += value;
        if(money.moneyAmount > money.maxMoney)
        {
            money.moneyAmount = money.maxMoney;
            return false;
        }
        return true;
    }

    public static bool TrySpendMoney(uint value)
    {
        if(money.moneyAmount < value)
        {
            return false;
        }
         money.moneyAmount -= value;
        return true;
    }

    public static bool HaveEnoughMoney(uint value)
    {
        return value <= money.moneyAmount;
    }

}
