using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Def;
using UnityEngine.UI;
using System.Linq;
using TMPro;

// 유저가 스킬을 관리하기 위해 조작하면 반응하는 클래스
// 읽어온 스킬을 UI들에 적용
public class SkillMngWindow : MonoBehaviour
{
    [SerializeField] Animator ownAnim;
    [SerializeField] RectTransform contents;
    [SerializeField] Image ex_SkillIcon;
    [SerializeField] Image ex_SkillOutline;
    [SerializeField] TextMeshProUGUI ex_SkillName;
    [SerializeField] TextMeshProUGUI ex_SkillExplain;
    [HideInInspector] public Dictionary<uint, ushort> skillList = new Dictionary<uint, ushort>();
    [HideInInspector] public Dictionary<uint, SkillMngIcon> skillIcons = new Dictionary<uint, SkillMngIcon>();
    [HideInInspector] public List<UserSkill> useASkills = new List<UserSkill>();
    [HideInInspector] public List<UserSkill> usePSkills = new List<UserSkill>();
    [SerializeField] SkillMng_Passive[] smP = new SkillMng_Passive[3];
    [SerializeField] SkillMng_Active[] smA = new SkillMng_Active[3];
    uint curTooltipSkillCode;
    
    private void Start()
    {
        InitSet();
    }
    public void UnuseSkill(UserSkill us)
    {
        if (skillList.ContainsKey(us.code))
            skillList[us.code]++;
        else
        {
            Debug.LogError("Error! : UnuseSkill");
        }
    }
    public void InitSet()
    {
        // 근데 이거 DontDestroy하는 게 퍼포먼스 더 늘릴 수 있지 않겠음? 고민좀

        // 스킬 아이콘 프리팹을 불러올 수 있어야 함(드래그 되는 것으로)
        // 불러온 다음 리스트를 긁으면서 스킬 추가. 
        for (int i = 0; i < 3; i++)
        {
            usePSkills.Add(null);
            useASkills.Add(null);
        }
        
        foreach (UserSkill us in SkillManager.m.ownSkill)
        {
            if (skillList.ContainsKey(us.code))
                skillList[us.code]++;
            else
                skillList.Add(us.code, 1);
        }
        // 아이콘 프리팹을 불러온 다음 개수도 세팅.
        // 불러오면서 해당 UI를 InitSet에서 전달해줌
        List<GameObject> gos = new List<GameObject>();
        foreach (KeyValuePair<uint, ushort> sl in skillList)
        {
            GameObject go = Instantiate(Res.m.skillMngObjPrefab, contents);
            go.name = sl.Key.ToString();
            SkillMngIcon skillIcon = go.GetComponent<SkillMngIcon>();
            skillIcon.InitSet(this, SkillManager.m.uSkillDict[sl.Key], sl.Value);
            skillIcons.Add(sl.Key, skillIcon);
            gos.Add(go);
        }
        contents.sizeDelta = new Vector2(contents.sizeDelta.x, ((contents.childCount / 4 + 1) * 205f));
        List<GameObject> newgos = gos.OrderBy(goo => goo.name).ToList();
        for (int i = 0; i < newgos.Count; i++)
            newgos[i].transform.SetSiblingIndex(i);
        if (SkillManager.m.ownSkill.Count > 0)
            ExplainSkill(SkillManager.m.ownSkill[0], (SkillManager.m.ownSkill[0].type > eSkillType.UPA));
        LoadEquipInfo();
        SkillManager.m.SkillSet(usePSkills, useASkills);
    }
    public void ExplainSkill(UserSkill us, bool isA)
    {
        if (curTooltipSkillCode == us.code) return;
        curTooltipSkillCode = us.code;
        ex_SkillIcon.sprite = us.icon;
        if (isA) ex_SkillOutline.color = Res.m.aSkillColor;
        else ex_SkillOutline.color = Res.m.pSkillColor;
        ex_SkillName.text = string.Format("{0}({1})", us.skillName, us.rank);
        ex_SkillName.color = Res.m.rankColor[(int)us.rank];
        ex_SkillExplain.text = us.tooltip;
    }

    public void PSkillIn(SkillMng_Passive sp, uint code, int idx)
    {
        for (int i = 0; i < usePSkills.Count; i++)
        {
            if (usePSkills[i] == null) continue;
            if (i == idx) continue;
            if (usePSkills[i].code / 10 == code / 10)
            {
                Debug.Log("같은 종류 스킬 착용 불가!!");
                return;
            }    
        }

        if (usePSkills[idx] != null)
        {
            skillIcons[usePSkills[idx].code].CntAdd();
        }
        usePSkills[idx] = SkillManager.m.uSkillDict[code];
        skillIcons[usePSkills[idx].code].CntSub();
        sp.SetSkill();


        // 드래그 앤 드롭으로 받음
        // 이건 사실 각 스킬 스크립트에 있어야 함
        // 거기에서 생성될 때 SkillMngWindow를 받아서 버튼 누르면 보내게끔

    }
    public void ASkillIn(SkillMng_Active sa, uint code, int idx)
    {

        for (int i = 0; i < useASkills.Count; i++)
        {
            if (useASkills[i] == null) continue;
            if (useASkills[i].code / 10 == code / 10)
            {
                Debug.Log("같은 종류 스킬 착용 불가!!");
                return;
            }
        }

        if (useASkills[idx] != null)
        {
            skillIcons[useASkills[idx].code].CntAdd();
        }
        useASkills[idx] = SkillManager.m.uSkillDict[code];
        skillIcons[useASkills[idx].code].CntSub();
        sa.SetSkill();

    }

    public void PSkillOut(int idx)
    {
        if (usePSkills[idx] != null)
        {
            skillIcons[usePSkills[idx].code].CntAdd();
        }
        usePSkills[idx] = null;
    }
    public void ASkillOut(int idx)
    {
        if (useASkills[idx] != null)
        {
            skillIcons[useASkills[idx].code].CntAdd();
        }
        useASkills[idx] = null;
    }
    public void PSkillComeBack(int idx, UserSkill us)
    {
        if (usePSkills[idx] != null)
        {
            skillIcons[usePSkills[idx].code].CntAdd();
        }
        //usePSkills[idx] = SkillManager.m.uSkillDict[code];
        //skillIcons[usePSkills[idx].code].CntSub();
        //sp.SetSkill();
    }
    public void OpenWindow()
    {
        AudioManager.m.PlaySE(R_SE.ButtonClick);
        ownAnim.SetTrigger("Open");
    }
    public void CloseWindow()
    {
        AudioManager.m.PlaySE(R_SE.ButtonClick);
        SkillManager.m.SkillSet(usePSkills, useASkills);
        SaveEquipInfo();
        ownAnim.SetTrigger("Close");
        // 윈도우 닫을 때 SkillManager에 현재 장착중인 스킬 인포메이션 전달.
    }

    public void LoadEquipInfo()
    {
        for (int i = 0; i < 3; i++)
        {
            uint pCode = UserInfo.m.userData.equipPSkills[i];
            uint aCode = UserInfo.m.userData.equipASkills[i];
            smP[i].smw = this;
            smA[i].smw = this;
            if (pCode != 0)
            {
                usePSkills[i] = SkillManager.m.uSkillDict[pCode];
                skillIcons[usePSkills[i].code].CntSub();
                smP[i].InitSet(usePSkills[i]);
                smP[i].SetSkill();
            }

            if (aCode != 0)
            {
                useASkills[i] = SkillManager.m.uSkillDict[aCode];
                skillIcons[useASkills[i].code].CntSub();
                smA[i].InitSet(useASkills[i]);
                smA[i].SetSkill();
            }
            
        }
    }
    public void SaveEquipInfo()
    {
        for (int i = 0; i < 3; i++)
        {
            if (usePSkills[i] == null) UserInfo.m.userData.equipPSkills[i] = 0;
            else UserInfo.m.userData.equipPSkills[i] = usePSkills[i].code;

            if (useASkills[i] == null) UserInfo.m.userData.equipASkills[i] = 0;
            else UserInfo.m.userData.equipASkills[i] = useASkills[i].code;
        }
        UserInfo.m.SaveInfo();
    }
}
