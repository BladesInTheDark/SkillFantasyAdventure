using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Def;
using TMPro;

// 인게임 조작시 액티브 스킬 버튼에 반응
public class ActiveSkillButton : MonoBehaviour
{
    [SerializeField] byte ownIndex; // 0, 1, 2로 Start에서 SkillManager에 자기 번호랑 같이 보내줌
    [SerializeField] public Button btn;
    [SerializeField] Image icon;
    [SerializeField] Image mask;
    [SerializeField] TextMeshProUGUI rankText;
    UserSkill ownSkill;

    public bool isStart = false;
    public bool setEnd = false;
    float timer = 0;
    float recharge;
    [System.NonSerialized] public bool canUse = false;

    public void UISetCall()
    {
        PlayerCtrlManager.m.ActiveSkillSet(this, ownIndex);
    }
    private void Update()
    {
        if (!setEnd) return;
        if (!isStart) return;
        if (canUse) return;
        timer += Time.deltaTime;
        MaskSet(timer / recharge);
        if (timer > recharge)
        {
            timer = 0;
            MaskSet(1);
            canUse = true;
        }
    }
    public void InitSet(UserSkill us)
    {
        ownSkill = us;
        if (us == null)
        {
            icon.sprite = Res.m.GetUserSkillSprite(eUSkillCode.None);
            rankText.text = string.Empty;
            MaskSet(0);
            canUse = false;
            setEnd = false;
        }
        else
        {
            icon.sprite = us.icon;
            rankText.text = us.rank.ToString();
            rankText.color = Res.m.rankColor[(int)us.rank];
            recharge = us.rechargeTime;
            canUse = true;
            MaskSet(1);
            setEnd = true;
            isStart = true;
        }
        
    }
    public void MaskSet(float p)
    {
        mask.fillAmount = 1 - p;
    }
    public void CoolTimeReset()
    {
        if (ownSkill != null)
        {
            timer = 0;
            canUse = true;
            MaskSet(1);
        }
    }

}
