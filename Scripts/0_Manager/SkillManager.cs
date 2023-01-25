using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Def;

// 스킬을 관리하는 매니저, 테이블을 읽어와서 고유 코드를 키로 딕셔너리화하는 역할.
public class SkillManager : MonoBehaviour
{
    static SkillManager _uniqueInstance;
    public static SkillManager m
    {
        get { return _uniqueInstance; }
    }

    string[,] Sentence;
    [HideInInspector] public Dictionary<uint, UserSkill> uSkillDict = new Dictionary<uint, UserSkill>();
    [HideInInspector] public Dictionary<uint, MonsterSkill> mSkillDict = new Dictionary<uint, MonsterSkill>();
    public List<UserSkill> ownSkill = new List<UserSkill>();
    int lineSize, rowSize;
    // 여기에 스킬 기능 다 있어야 함
    public UserSkill[] psb = new UserSkill[3];
    public UserSkill[] asb = new UserSkill[3];
    Player curPlayer;

    private void Awake()
    {
        _uniqueInstance = this;
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        TableLoader.m.USkillTableLoad();
        TableLoader.m.MSkillTableLoad();
    }
    public void UserSkillSet()
    {
        foreach (uint code5 in UserInfo.m.userData.skills)
        {
            ownSkill.Add(uSkillDict[code5]);
        }
    }
    

    public void SkillSet(List<UserSkill> upsList, List<UserSkill> uasList)
    {
        // 로비에 있던 윈도우에서 스킬 정보를 받아와서 임시 저장함
        for (int i = 0; i < 3; i++)
        {
            psb[i] = upsList[i];
            asb[i] = uasList[i];
        }
        // 임시저장 한 다음 게임이 시작하면 버튼에 세팅해줘야 함. 
    }

    public bool CooltimeCheck()
    {
        return false;
    }
    
}
