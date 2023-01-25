using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Def;

public class UserSkill
{
    public uint code;
    public string skillName;
    public eSkillType type;
    public eSkillRank rank;
    public string tooltip;
    public string[] values;
    public Sprite icon;
    public List<stTilePos> attackArea;
    public float rechargeTime;

    public UserSkill(string data)
    {
        string[] datas = data.Split('\t');
        if (uint.TryParse(datas[0], out uint tmpCode))
            code = tmpCode;
        else
            Debug.LogError(string.Format("UserSkill Error!(code)"));
        skillName = datas[1];
        if (Enum.TryParse(datas[2], out eSkillType tmpType))
            type = tmpType;
        else
            Debug.LogError(string.Format("UserSkill Error!(type) SkillCode:{0}", code));
        if (Enum.TryParse(datas[3], out eSkillRank tmpRank))
            rank = tmpRank;
        else
            Debug.LogError(string.Format("UserSkill Error!(rank) SkillCode:{0}", code));
        values = datas[5].Split(',');
        tooltip = string.Format(datas[4], values);
        if (float.TryParse(datas[6], out float rech))
            rechargeTime = rech;
        else
            Debug.LogError(string.Format("UserSkill Error!(rechargeTime) SkillCode:{0}", code));
        icon = Res.m.GetUserSkillSprite((eUSkillCode)(code / 10));

        attackArea = new List<stTilePos>();

        if (datas[7].Contains(":"))
        {
            string[] range = datas[7].Split(',');
            for (int i = 0; i < range.Length; i++)
            {
                string[] tmp = range[i].Split(':');
                if (int.TryParse(tmp[0], out int row) && int.TryParse(tmp[1], out int col))
                    attackArea.Add(new stTilePos(row, col));
                else
                    Debug.LogError(string.Format("UserSkill Error!(attackArea) SkillCode:{0}", code));
            }
        }
    }
}
