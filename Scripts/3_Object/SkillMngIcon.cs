using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Def;

// 스킬관리창에서 보유 목록에 들어가는 아이콘들
// 소환되며 드래그할 수 있는 아이콘
public class SkillMngIcon : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [SerializeField] RectTransform ownRect;
    [SerializeField] public Image icon;
    [SerializeField] Image outline;
    [SerializeField] public TextMeshProUGUI rankText;
    ushort cnt;
    [SerializeField] TextMeshProUGUI cntText;
    [HideInInspector] public UserSkill ownSkill;

    [SerializeField] Vector2 _dragOffest = Vector2.zero;
    GameObject _draggingObj;
    RectTransform _canvasRect;
    [HideInInspector] public SkillMngWindow wnd;
    [HideInInspector] public bool isActiveSKill;

    public void InitSet(SkillMngWindow smwnd, UserSkill us, ushort cntt)
    {
        icon.sprite = Res.m.GetUserSkillSprite((eUSkillCode)(us.code / 10));
        ownSkill = us;
        if (ownSkill.type < eSkillType.UPA)
        {
            isActiveSKill = false;
            outline.color = Res.m.pSkillColor;
        }
        else if (ownSkill.type > eSkillType.UPA)
        {
            // 액티브 스킬임
            isActiveSKill = true;
            outline.color = Res.m.aSkillColor;
        }
        rankText.text = us.rank.ToString();
        rankText.color = Res.m.rankColor[(int)us.rank];
        cnt = cntt;
        cntText.text = cnt.ToString();
        wnd = smwnd;
    }

    #region [클릭 인식 및 드래그 진행]
    public void OnPointerClick(PointerEventData eventData)
    {
        wnd.ExplainSkill(ownSkill, isActiveSKill);
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (cnt == 0)
            return;

        cntText.text = (cnt - 1).ToString();
        wnd.ExplainSkill(ownSkill, isActiveSKill);

        if (_draggingObj != null)
            Destroy(_draggingObj);

        // 드래그 중인 아이콘 생성
        _draggingObj = Instantiate(Res.m.draggingObj, wnd.transform);
        // 드래그 아이콘을 해당 캔버스에 종속 후 가장 마지막으로 옮겨서 최상위에 그려지도록 한다
        _draggingObj.transform.SetAsLastSibling();
        _draggingObj.transform.GetComponent<RectTransform>().sizeDelta = ownRect.sizeDelta;
        _draggingObj.transform.localScale = Vector3.one;

        // 블록레이캐스트 속성을 블록되지 않게 한다
        SkillMngIcon_Dragging drag = _draggingObj.GetComponent<SkillMngIcon_Dragging>();
        drag.InitSet(icon.sprite, outline.color, ownSkill.rank);

        _canvasRect = drag.rectTf;
        UpdateDraggingObjPos(eventData);
    }
    public void OnDrag(PointerEventData eventData)
    {
        UpdateDraggingObjPos(eventData);
    }
    void UpdateDraggingObjPos(PointerEventData eventData)
    {
        if (_draggingObj != null)
        {
            // 드래그중 아이콘의 스크린좌표
            Vector3 screenPos = eventData.position + _dragOffest;

            Vector3 newPos = Vector3.zero;
            Camera cam = eventData.pressEventCamera;

            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(_canvasRect, screenPos, cam, out newPos))
            {
                _draggingObj.transform.position = newPos;
                _draggingObj.transform.rotation = _canvasRect.rotation;
                //wnd.ScrollRectSet(false);
            }

        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        cntText.text = cnt.ToString();
        //wnd.ScrollRectSet(true);
        Destroy(_draggingObj);
    }
    #endregion
    

    public void CntSub()
    {
        cnt--;
        cntText.text = cnt.ToString();
        if (cnt == 0)
        {
            icon.color = Color.gray;
        }
    }
    public void CntAdd()
    {
        if (cnt == 0)
        {
            icon.color = Color.white;
        }
        cnt++;
        cntText.text = cnt.ToString();
    }
}
