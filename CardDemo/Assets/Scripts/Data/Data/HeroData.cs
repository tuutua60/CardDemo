using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HeroData
{
    
    public int id;
    public string name;
    public int type;
    public int[] skillIDArr;
    public int maxHp;
    public int atk;
    public int def;
    public int agl;
    public int atkRage;
    public int takeDamageRage;
    public int maxRage;



    public int nowHp;
    public int currentRage;
    public int changeAtkPercent;//增伤变化百分比
    public int changeDamagePercent;//减伤变化百分比
    //座位ID
    public int seatID;
    //队伍类型
    public E_GlobalTeamType globalTeamType;
    //默认数据
    public T_HeroData defaultData;

    public HeroData(int heroID,int seatID, E_GlobalTeamType globalTeamType)
    {
        //读表
        T_HeroData data = BinaryDataMgr.Instance.GetTable<T_HeroDataContainer>().dataDic[heroID];
        defaultData = data;
        //配置默认数据
        this.id = data.id;
        this.name = data.name;
        this.type = data.type;
        string[] tempArr = data.skillIDArr.Split(',');
        this.skillIDArr = new int[] { int.Parse(tempArr[0]), int.Parse(tempArr[1]) };
        this.maxHp = data.hp;
        this.atk = data.atk;
        this.def = data.def;
        this.agl = data.agl;
        this.atkRage = data.atkRage;
        this.takeDamageRage = data.takeDamageRage;
        this.maxRage = data.maxRage;

        currentRage = 0;
        nowHp = maxHp;
        changeDamagePercent = 0;
        this.seatID = seatID;
        this.globalTeamType = globalTeamType;
    }

}
