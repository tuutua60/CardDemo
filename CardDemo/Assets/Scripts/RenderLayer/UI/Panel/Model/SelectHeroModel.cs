using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectHeroModel
{
    public List<int> playerList;
    public List<int> enemyList;

    public void Initialize()
    {
        playerList = new List<int>();
        enemyList = new List<int>();
        for (int i = 101; i <= 105; i++)
        {
            playerList.Add(-1);
        }
        //敌人可以根据关卡不同更改
        for (int i = 101; i <= 105; i++)
        {
            enemyList.Add(i);
        }
    }
}
