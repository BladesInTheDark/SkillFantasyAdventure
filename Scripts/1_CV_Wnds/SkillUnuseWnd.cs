using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Def;

// 스킬 관리에서 스킬 장착을 해제할 때 한번 확인하는 창에 반응하는 클래스 
public class SkillUnuseWnd : MonoBehaviour
{
    [SerializeField] Animator ownAnim;
    [SerializeField] RectTransform ownRect;
    [SerializeField] public Image icon;
    [SerializeField] Image outline;
    [SerializeField] public TextMeshProUGUI rankText;
    [SerializeField] TextMeshProUGUI explain;
    bool isActiveSkill;
    SkillMng_Passive tmpSmp;
    SkillMng_Active tmpSma;
    public void OpenWindow(UserSkill us, SkillMng_Passive smp)
    {
        tmpSmp = smp;
        icon.sprite = Res.m.GetUserSkillSprite((eUSkillCode)(us.code / 10));
        isActiveSkill = false;
        outline.color = Res.m.pSkillColor;

        rankText.text = us.rank.ToString();
        rankText.color = Res.m.rankColor[(int)us.rank];
        ownAnim.SetTrigger("Open");
    }
    public void OpenWindow(UserSkill us, SkillMng_Active sma)
    {
        tmpSma = sma;
        icon.sprite = Res.m.GetUserSkillSprite((eUSkillCode)(us.code / 10));
        isActiveSkill = true;
        outline.color = Res.m.aSkillColor;

        rankText.text = us.rank.ToString();
        rankText.color = Res.m.rankColor[(int)us.rank];
        ownAnim.SetTrigger("Open");
    }
    public void ClickSure()
    {
        if (!isActiveSkill)
        {
            tmpSmp.ResetSkill();
        }
        else
        {
            tmpSma.ResetSkill();
        }
        CloseWindow();
    }
    public void ClickCancel()
    {
        CloseWindow();
    }
    public void CloseWindow()
    {
        ownAnim.SetTrigger("Close");
    }
}
