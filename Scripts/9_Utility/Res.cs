using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Def;

// 리소스 매니저. 처음부터 할당으로 무언가 값을 가지고있어야 하는 것이 있다면 이곳에 할당하여 불러다 사용.
public class Res : MonoBehaviour
{
    #region [인스턴스화]
    static Res _uniqueInstance;
    public static Res m
    {
        get { return _uniqueInstance; }
    }
    #endregion
    [Header("프리팹")]
    [SerializeField] public GameObject optionWndPrefab;
    [SerializeField] public GameObject tilePrefab;
    [SerializeField] public GameObject monsterPrefab;
    [SerializeField] public GameObject playerPrefab;
    [SerializeField] public GameObject skillMngObjPrefab;
    [SerializeField] public GameObject draggingObj;
    [SerializeField] public GameObject auraColliderObj;
    [Header("배경/타일")]
    [SerializeField] Sprite[] bgs = new Sprite[3];
    [SerializeField] Sprite[] tiles = new Sprite[5];
    [SerializeField] Sprite[] monsters = new Sprite[5];
    [SerializeField] Sprite[] playerIcons;
    [Header("음악/소리")]
    [SerializeField] AudioClip[] bgmClips = new AudioClip[2];
    [SerializeField] AudioClip[] seClips = new AudioClip[1];
    [Header("스킬이미지")]
    [SerializeField] Sprite[] userSkillIcons;
    [SerializeField] Sprite[] monsterSkillIcons;
    [Header("스킬색")]
    public Sprite pSkillOutline;
    public Sprite aSkillOutline;
    public Color pSkillColor;
    public Color aSkillColor;
    public Color[] rankColor = new Color[11];


    private void Awake()
    {
        _uniqueInstance = this;
        DontDestroyOnLoad(this);
    }

    public Sprite GetBGSprite(R_Background idx)
    {
        return bgs[(int)idx];
    }
    public AudioClip GetBGM(R_BGM idx)
    {
        return bgmClips[(int)idx];
    }
    public AudioClip GetSE(R_SE idx)
    {
        return seClips[(int)idx];
    }
    public Sprite GetUserSkillSprite(eUSkillCode sc)
    {
        int idx = 0;
        if (sc != eUSkillCode.None) idx = (int)sc - 999;
        return userSkillIcons[idx];

        //switch (st)
        //{
        //    case eSkillType.UPS_Stat:
        //        return UPS_Stat_S[idx];
        //    case eSkillType.UPS_Mvp:
        //        return UPS_Mvp_S[idx];
        //    case eSkillType.UPS_Aura:
        //        return UPS_Aura_S[idx];
        //    case eSkillType.UPS_Scope:
        //        return UPS_Scope_S[idx];
        //    case eSkillType.UPS_Random:
        //        return UPS_Random_S[idx];
        //    case eSkillType.UPS_Interact:
        //        return UPS_Interact_S[idx];
        //    case eSkillType.UPS_Etc:
        //        return UPS_Etc_S[idx];

        //    case eSkillType.UAS_Attack:
        //        return UAS_Attack_S[idx];
        //    case eSkillType.UAS_Deffence:
        //        return UAS_Deffence_S[idx];
        //    case eSkillType.UAS_Buff:
        //        return UAS_Buff_S[idx];
        //    case eSkillType.UAS_Debuff:
        //        return UAS_Debuff_S[idx];
        //    case eSkillType.UAS_Etc:
        //        return UAS_Etc_S[idx];

        //    case eSkillType.MPS_Goblin:
        //        return MPS_Goblin_S[idx];
        //    case eSkillType.MPS_Bat:
        //        return MPS_Bat_S[idx];
        //    case eSkillType.MPS_Slime:
        //        return MPS_Slime_S[idx];
        //    case eSkillType.MPS_Wolf:
        //        return MPS_Wolf_S[idx];
        //    case eSkillType.MPS_Hyena:
        //        return MPS_Hyena_S[idx];

        //    case eSkillType.MAS_Goblin:
        //        return MAS_Goblin_S[idx];
        //    case eSkillType.MAS_Bat:
        //        return MAS_Bat_S[idx];
        //    case eSkillType.MAS_Slime:
        //        return MAS_Slime_S[idx];
        //    case eSkillType.MAS_Wolf:
        //        return MAS_Wolf_S[idx];
        //    case eSkillType.MAS_Hyena:
        //        return MAS_Hyena_S[idx];

        //    case eSkillType.MPS_Boss1:
        //        return MPS_Boss1_S[idx];
        //    case eSkillType.MAS_Boss1:
        //        return MAS_Boss1_S[idx];

        //    default:
        //        return null;
        //}
    }
    public Sprite GetMonsterSkillSprite(eUSkillCode sc)
    {
        int idx = 0;
        if (sc != eUSkillCode.None) idx = (int)sc - 999;
        return userSkillIcons[idx];
    }

    public Sprite GetTileSprite(eTileType idx)
    {
        return tiles[(int)idx];
    }
    public Sprite GetMonsterSprite(eMonster idx)
    {
        return monsters[(int)idx];
    }
    public Sprite GetPlayerIconSprite(int idx)
    {
        return playerIcons[idx];
    }
    public static eMoveDir Op(eMoveDir tmpDir)
    {
        switch (tmpDir)
        {
            case eMoveDir.Left:
                return eMoveDir.Right;
            case eMoveDir.Right:
                return eMoveDir.Left;
            case eMoveDir.Up:
                return eMoveDir.Down;
            case eMoveDir.Down:
                return eMoveDir.Up;
            default:
                return eMoveDir.Right;
        }
    }
}
