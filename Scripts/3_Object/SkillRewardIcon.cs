using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Def;
using UnityEngine.EventSystems;

// 스킬 보상에서 사용하는 스킬아이콘 출력용
public class SkillRewardIcon : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image skillIcon;
    [SerializeField] Image outline;
    [SerializeField] TextMeshProUGUI rankText;
    [SerializeField] SkillTooltipPopUp tooltip;
    [SerializeField] GameObject clickMask;

    private void Start()
    {
        clickMask.SetActive(false);
    }
    public void SetSkill(UserSkill us)
    {
        skillIcon.sprite = us.icon;
        rankText.text = us.rank.ToString();
        rankText.color = Res.m.rankColor[(int)us.rank];
        if (us.type < eSkillType.UPA) outline.sprite = Res.m.pSkillOutline;
        else outline.sprite= Res.m.aSkillOutline;
        tooltip.InitSet(us);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        tooltip.transform.position = eventData.position;
        clickMask.SetActive(true);
        tooltip.On();
    }
    public void AnotherClick()
    {
        clickMask.SetActive(false);
        tooltip.Off();
    }
}
