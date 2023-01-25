using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Def;

// 스킬관리에서 패시브 스킬 칸을 담당하는 클래스
public class SkillMng_Passive : MonoBehaviour, IDropHandler
{
    [SerializeField] public int ownIdx;
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI rankText;
    UserSkill ownSkill;
    [System.NonSerialized] public SkillMngWindow smw;
    [SerializeField] SkillUnuseWnd unuseWnd;

    private void Awake()
    {
        ownSkill = SkillManager.m.psb[ownIdx];
        if (ownSkill == null)
        {
            icon.sprite = Res.m.GetUserSkillSprite(eUSkillCode.None);
            rankText.text = string.Empty;
        }
        else
        {
            icon.sprite = ownSkill.icon;
            rankText.text = ownSkill.rank.ToString();
            rankText.color = Res.m.rankColor[(int)ownSkill.rank];
        }
    }
    public void InitSet(UserSkill us)
    {
        // 저장 정보가 있을 경우
        ownSkill = us;
    }
    public void OnDrop(PointerEventData eventData)
    {
        SkillMngIcon skillIcon = eventData.pointerDrag.GetComponent<SkillMngIcon>();
        if (skillIcon.isActiveSKill) return;
        if (skillIcon != null)
        {
            ownSkill = skillIcon.ownSkill;
            skillIcon.wnd.PSkillIn(this, ownSkill.code, ownIdx);
            return;
        }
    }

    public void SetSkill()
    {
        icon.sprite = ownSkill.icon;
        rankText.text = ownSkill.rank.ToString();
        rankText.color = Res.m.rankColor[(int)ownSkill.rank];
    }

    public void ClickToDelete()
    {
        if (ownSkill == null) return;
        unuseWnd.OpenWindow(ownSkill, this);
    }
    public void ResetSkill()
    {
        ownSkill = null;
        icon.sprite = Res.m.GetUserSkillSprite(eUSkillCode.None);
        rankText.text = string.Empty;

        smw.PSkillOut(ownIdx);
    }
}
