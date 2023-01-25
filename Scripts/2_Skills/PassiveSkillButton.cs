using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Def;
using TMPro;

// 인게임 조작시 패시브 스킬 버튼에 반응
// 사실 지금 상황으로는 스크립트 따로 필요 없긴 한데 나중에 확장성(몇초당 한번씩 뭔가 효과가 발동한다든가 하는 패시브)을 고려하여 만들어두었음
public class PassiveSkillButton : MonoBehaviour
{
    [SerializeField] byte ownIndex; // 0, 1, 2로 Start에서 SkillManager에 자기 번호랑 같이 보내줌
    [SerializeField] Image icon;
    public UserSkill ownSkill;
    [SerializeField] TextMeshProUGUI rankText;

    public void UISetCall()
    {
        PlayerCtrlManager.m.PassiveSkillSet(this, ownIndex);
    }
    public void InitSet(UserSkill us)
    {
        ownSkill = us;
        if (us == null)
        {
            icon.sprite = Res.m.GetUserSkillSprite(eUSkillCode.None);
            rankText.text = string.Empty;
        }
        else
        {
            icon.sprite = us.icon;
            rankText.text = us.rank.ToString();
            rankText.color = Res.m.rankColor[(int)us.rank];
        }
        
    }
}
