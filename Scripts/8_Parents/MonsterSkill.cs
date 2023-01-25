using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Def;

public class MonsterSkill
{
    public uint code;
    public string skillName;
    public eSkillType type;
    public string tooltip;
    public string[] values;
    public Sprite icon;
    public List<stTilePos> attackArea;
    public float rechargeTime;
    public float lastUse;

    public MonsterSkill(string data)
    {
        string[] datas = data.Split('\t');
        if (uint.TryParse(datas[0], out uint tmpcode)) code = tmpcode;
        else code = 0; // 얘는 4자리 들어옴
        skillName = datas[1];
        if (Enum.TryParse(datas[2], out eSkillType tmpType))
            type = tmpType;
        else
            Debug.LogError(string.Format("MonsterSkill Error!(type) SkillCode:{0}", code));
        values = datas[4].Split(',');
        tooltip = string.Format(datas[3], values);
        if (float.TryParse(datas[5], out float rech))
            rechargeTime = rech;
        else
            Debug.LogError(string.Format("MonsterSkill Error!(rechargeTime) SkillCode:{0}", code));

        attackArea = new List<stTilePos>();
        if (datas[6].Contains(":"))
        {
            string[] range = datas[6].Split(',');
            for (int i = 0; i < range.Length; i++)
            {
                string[] tmp = range[i].Split(':');
                if (int.TryParse(tmp[0], out int row) && int.TryParse(tmp[1], out int col))
                    attackArea.Add(new stTilePos(row, col));
                else
                    Debug.LogError(string.Format("MonsterSkill Error!(attackArea) SkillCode:{0}", code));
            }
        }
    }
}
