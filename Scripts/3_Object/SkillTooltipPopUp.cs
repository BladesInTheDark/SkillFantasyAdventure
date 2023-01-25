using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 스킬 툴팁
public class SkillTooltipPopUp : MonoBehaviour
{
    [SerializeField] Animator ownAnim;
    [SerializeField] Image ex_SkillIcon;
    [SerializeField] Image ex_SkillOutline;
    [SerializeField] TextMeshProUGUI ex_SkillRank;
    [SerializeField] TextMeshProUGUI ex_SkillName;
    [SerializeField] TextMeshProUGUI ex_SkillExplain;

    public void InitSet(UserSkill us)
    {
        ex_SkillIcon.sprite = us.icon;
        if (us.type > Def.eSkillType.UPA) ex_SkillOutline.color = Res.m.aSkillColor;
        else ex_SkillOutline.color = Res.m.pSkillColor;
        ex_SkillName.text = string.Format("{0}({1})", us.skillName, us.rank);
        ex_SkillName.color = Res.m.rankColor[(int)us.rank];
        ex_SkillRank.text = us.rank.ToString();
        ex_SkillRank.color = Res.m.rankColor[(int)us.rank];
        ex_SkillExplain.text = us.tooltip;
    }

    public void On()
    {
        ownAnim.SetBool("isOn", true);
    }
    public void Off()
    {
        ownAnim.SetBool("isOn", false);
    }
}
