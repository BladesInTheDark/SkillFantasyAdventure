using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Def;

public class UserInfo : MonoBehaviour
{
    #region [인스턴스화]
    static UserInfo _uniqueInstance;
    public static UserInfo m
    {
        get { return _uniqueInstance; }
    }
    #endregion

    //// 저장될 데이터
    //// 기본
    [HideInInspector] public byte userLevel
    {
        get { return userData == null ? (byte)1 : userData.userLevel; }
    }
    [HideInInspector] public string userName
    {
        get { return userData == null ? "" : userData.userName; }
    }

    //// 자원
    [HideInInspector] public int gold // 이쪽 재화는 버그 등으로 마이너스 갈 가능성 있을 듯
    {
        get { return userData == null ? 0 : userData.gold; }
    }
    [HideInInspector] public int diamond
    {
        get { return userData == null ? 0 : userData.diamond; }
    }
    [HideInInspector] public byte gameTicket
    {
        get { return userData == null ? (byte)0 : userData.gameTicket; }
    }
    [HideInInspector] public uint gachaTicket
    {
        get { return userData == null ? 0 : userData.gachaTicket; }
    }
    //// 보유한 아이템
    //// 게임 기록
    [HideInInspector] public uint tryCnt; // 게임 입장 횟수 총량
    [HideInInspector] public uint bestStage // 최대 달성 스테이지, 102(1-2), 310(3-10) 등으로 표기함
    {
        get { return userData == null ? 100 : userData.bestStage; }
    }
    [HideInInspector] public float bestRecordTime
    {
        get { return userData == null ? 0 : userData.bestRecordTime; }
    }
    //// 이벤트 데이터
    //// 구매 이력
    //// 등등

    //// 플레이어에 반영될 스탯 데이터
    [HideInInspector] public float atk;
    [HideInInspector] public float dfn;
    [HideInInspector] public float acc;
    [HideInInspector] public float avo;
    [HideInInspector] public float crt;
    [HideInInspector] public float crt_dmg;
    [HideInInspector] public float nowHp;
    [HideInInspector] public float hp;
    [HideInInspector] public byte mvp;
    [HideInInspector] public byte mvp_max;
    [HideInInspector] public float mvp_delay;
    [HideInInspector] public int lck;
    // 자주쓰지않는거
    [HideInInspector] public int iconIdx;
    [HideInInspector] public eUSkillCode[] usePSkills = new eUSkillCode[3];
    [HideInInspector] public eUSkillCode[] useASkills = new eUSkillCode[3];

    public UserData userData;

    [HideInInspector] public string path;
    
    public const byte gameTicketMax = 5;
    public bool firstPlay = true;

    private void Awake()
    {
        _uniqueInstance = this;
        DontDestroyOnLoad(this);
        path = Application.dataPath + "/UserData.json";
    }

    private void Start()
    {
        FileInfo fileInfo = new FileInfo(path);
        if (fileInfo.Exists)
        {
            Debug.Log("파일이 존재합니다~");
            // 파일 존재
            string loadData = File.ReadAllText(path);

            if (loadData == null || loadData == string.Empty)
            {
                Debug.Log("근데 아무것도 안 써있습니당~");
                firstPlay = true;
            }
            else
            {
                Debug.Log("데이터가 있습니다");
                LoadInfoSet(loadData);
                firstPlay = false;
            }
        }
        else
        {
            // 파일 없음
            // 파일 생성은 닉네임 생성과 함께 진행
            Debug.Log("파일이 음슴니다~");
            firstPlay = true;
        }
    }

    void InitSet()
    {
        //userLevel = 1;
        atk = 10;
        dfn = 0;
        acc = 0;
        avo = 0;
        crt = 5;
        crt_dmg = 20;
        hp = 50;
        nowHp = hp;
        mvp = 0;
        mvp_max = 3;
        mvp_delay = 3;
        lck = 0;
        iconIdx = 0;

        //gold = 0;
        //diamond = 0;
        //gameTicket = 5;
        //gachaTicket = 0;

        //tryCnt = 0;
        //bestStage = 0;
        //bestRecordTime = 0;
    }
    public bool SetCreatedUserName(string name)
    {
        InitSet();
        userData = new UserData(name);
        
        bool res = SaveInfo();
        return res;
    }

    public void LoadInfoSet(string loadData)
    {
        userData = JsonUtility.FromJson<UserData>(loadData);
        InitSet();
        StatSet();
        AudioManager.m.InitSet();
        SkillManager.m.UserSkillSet();
    }
    public void StatSet()
    {
        int tmp;

        if (userData.userLevel == 10) tmp = userData.userLevel;
        else tmp = userData.userLevel - 1;

        atk += 1 * tmp;
        dfn += 0.5f * tmp;
        acc += 1 * tmp;
        crt += 0.5f * tmp;
        crt_dmg += 0.5f * tmp;
        hp += 5 * tmp;
        mvp_delay -= 0.05f * tmp;
    }

    public bool SaveInfo()
    {
        try
        {
            string data = JsonUtility.ToJson(userData, true);
            File.WriteAllText(path, data);

            return true;
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            return false;
        }
        
    }
}

public class UserData
{
    public byte userLevel;
    public string userName;
    public int iconIdx;

    public int gold; 
    public int diamond;
    public byte gameTicket;
    public uint gachaTicket;

    public List<uint> skills;
    public uint[] equipPSkills;
    public uint[] equipASkills;

    public uint tryCnt;
    public uint bestStage; 
    public float bestRecordTime;

    public bool isBGMMute;
    public bool isSEMute;
    public float bgmVol;
    public float seVol;

    public UserData(string n)
    {
        userLevel = 1;
        userName = n;
        iconIdx = 0;
        gold = 0;
        diamond = 0;
        gameTicket = 5;
        gachaTicket = 0;
        skills = new List<uint>();
        uint[] equipPSkill = { 0, 0, 0 };
        equipPSkills = equipPSkill;
        uint[] equipASkill = { 0, 0, 0 };
        equipASkills = equipASkill;

        tryCnt = 0;
        bestStage = 100;
        bestRecordTime = 0;

        isBGMMute = false;
        isSEMute = false;
        bgmVol = 1;
        seVol = 1;
    }

}
