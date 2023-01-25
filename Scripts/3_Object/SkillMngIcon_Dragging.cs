using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Def;

// 드래그 중에 임시생성되는 아이콘
public class SkillMngIcon_Dragging : MonoBehaviour
{
    [SerializeField] public RectTransform rectTf;
    [SerializeField] Image icon;
    [SerializeField] Image outline;
    [SerializeField] TextMeshProUGUI rankText;

    public void InitSet(Sprite skillIcon, Color color, eSkillRank rank)
    {
        rectTf.sizeDelta = new Vector2(200f, 200f);
        icon.sprite = skillIcon;
        outline.color = color;
        rankText.text = rank.ToString();
        rankText.color = Res.m.rankColor[(int)rank];
    }
}
