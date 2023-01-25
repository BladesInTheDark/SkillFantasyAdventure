using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Def
{
    // 타일 클래스가 가지고 있을 타입, 개체 이동시 이걸로 체크해서 갈 수 있는 곳인지 판단함. 
    public enum eTileType
    {
        Blank, // 타일 이미지 아예 없음
        Normal, // 평범한 타일
        NSpawn, // 평범한 타일인데 몬스터가 랜덤스폰할 수 있는 타일
        Wall, // 벽(못지나감)
        Trap // 함정(지나갈 순 있으나 밟으면 최초 판정시 터짐)
    }

    // 개체 이동시 타입. 방향을 알려주고 방향에 따라 타일 매니저에서 계산함.
    public enum eMoveDir
    {
        Up,
        Down,
        Left,
        Right
    }
    

    // 씬전환에 사용
    public enum eSceneType
    {
        Login,
        Lobby,
        Ingame
    }
    
    // 몬스터 타입 체크에 사용
    public enum eMonster
    {
        Goblin,
        Bat,
        Slime,
        Wolf,
        Hyena,
        Special
    }

    #region [리소스에서 불러오기 위해 사용]
    // Res와 직접 대응됨. 
    public enum R_BGM
    {
        Lobby,
        IngameStage1,
    }
    public enum R_Background
    {
        OnLogin,
        OnLobby,
        OnIngameStage1,
    }
    public enum R_SE
    {
        ButtonClick,
        PlayerMove,
        PlayerAttack,
        PlayerCantMove,


        PlayerDamaged,
        PlayerBuffed,
        MonsterDebuffed,
        OneInchPunch,
        SpinArround,
        Roaring,
    }
    public enum R_Particle
    {
        Punched,
        Sworded,

        Shield,

        AttackBuff,
        DeffenceBuff,

        Debuff,
        Cursed,
        Stuned,
        Immune,
    }
    #endregion

    #region [스킬 관리]
    public enum eSkillType
    {
        UPS_Stat,
        UPS_Mvp,
        UPS_Aura,
        UPS_Scope,
        UPS_Random,
        UPS_Interact,
        UPS_Etc,

        UPA,

        UAS_Attack,
        UAS_Deffence,
        UAS_Buff,
        UAS_Debuff,
        UAS_Etc,

        UMS,

        MPS_Goblin,
        MPS_Bat,
        MPS_Slime,
        MPS_Wolf,
        MPS_Hyena,
        
        MPS,

        MAS_Goblin,
        MAS_Bat,
        MAS_Slime,
        MAS_Wolf,
        MAS_Hyena,

        MPS_Boss1,
        MAS_Boss1,
    }
    public enum eSkillRank
    {
        F,
        E,
        D,
        C,
        B,
        A,
        S,
        SS,
        SSS,
        L,
        U
    }
    public enum eUSkillCode
    {
        None,

        UPS_1000_Attack_Up = 1000,
        UPS_1001_Defence_Up,
        UPS_1002_HP_Up,
        UPS_1003_Mvp_Up,
        UPS_1004_Mvp_Max_Up,
        UPS_1005_Mvp_Cooltime_Down,
        UPS_1006_Aura_Attack_Down,
        UPS_1007_Aura_Acc_Down,
        UPS_1008_Scope_Normal_1x2,
        UPS_1009_HighLisk_HighReturn,
        UPS_1010_Goblin,
        UPS_1011_Wolf,

        UAS_1012_OneInchPunch,
        UAS_1013_SpinArround,
        UAS_1014_HardSkin,
        UAS_1015_ProtectionHand,
        UAS_1016_SkillDmgEnhance,
        UAS_1017_Curse,
        UAS_1018_Roaring,
        
        maxIdx
    }
    public enum eUParticle
    {
        버프,
        디버프,
        피격,
        베기,
        보호막,

        None,
        UPS_1000_Attack_Up = 1000,
        UPS_1001_Defence_Up,
        UPS_1002_HP_Up,
        UPS_1003_Mvp_Up,
        UPS_1004_Mvp_Max_Up,
        UPS_1005_Mvp_Cooltime_Down,
        UPS_1006_Aura_Attack_Down,
        UPS_1007_Aura_Acc_Down,
        UPS_1008_Scope_Normal_1x2,
        UPS_1009_HighLisk_HighReturn,
        UPS_1010_Goblin,
        UPS_1011_Wolf,

        UAS_1012_OneInchPunch,
        UAS_1013_SpinArround,
        UAS_1014_HardSkin,
        UAS_1015_ProtectionHand,
        UAS_1016_SkillDmgEnhance,
        UAS_1017_Curse,
        UAS_1018_Roaring,
    }
    public enum eMSkillCode
    {
        None,
        MPS_1000_Goblin_Passive = 1000,
        MAS_1001_Goblin_Attack,
        MAS_1002_Goblin_StrongAttack,

        MPS_1003_Bat_Passive,
        MAS_1004_Bat_Attack,
        MAS_1005_Bat_Wave,
        MAS_1006_Bat_WideWave,

        MPS_1007_Slime_Passive,
        MAS_1008_Slime_Attack,
        MAS_1009_Slime_Slip,
        MAS_1010_Slime_Rush,

        MPS_1011_Wolf_Passive,
        MAS_1012_Wolf_Attack,
        MAS_1013_Wolf_Scratch,
        MAS_1014_Wolf_Tackle,
        MAS_1015_Wolf_Bite,

        MPS_1016_Hyena_Passive,
        MAS_1017_Hyena_Attack,
        MAS_1018_Hyena_Unite,
        MAS_1019_Hyena_Crisis,
    }
    public enum eM_GoblinParticle
    {

    }
    public enum UPS_Stat
    {
        공격력_증가,
        방어력_증가,
        체력_증가,
    }
    public enum UPS_Mvp
    {
        시작_행동력_증가,
        최대_행동력_증가,
        행동력_쿨타임_감소,
    }
    public enum UPS_Aura
    {
        주변_공격력_감소,
        주변_명중률_감소,
    }
    public enum UPS_Scope
    {
        일반공격_범위_증가_1x2,
    }
    public enum UPS_Random
    {
        자업자득,
    }
    public enum UPS_Interact
    {
        고블린_언어,
        늑대_언어,
    }
    public enum UPS_Etc
    {
        
    }
    public enum UAS_Attack
    {
        원인치_펀치,
        주변_휩쓸기,
    }
    public enum UAS_Defence
    {
        단단한_피부,
    }
    public enum UAS_Buff
    {
        보호의_손길,
        일시적_강화,
    }
    public enum UAS_Debuff
    {
        저주,
        사자후,
    }
    public enum UAS_Etc
    {

    }

    public enum MS_Goblin
    {
        MPS_1000_Goblin_Passive = 0,
        MAS_1001_Goblin_Attack,
        MAS_1002_Goblin_StrongAttack,
    }
    public enum MS_Bat
    {
        MPS_1003_Bat_Passive = 0,
        MAS_1004_Bat_Attack,
        MAS_1005_Bat_Wave,
        MAS_1006_Bat_WideWave,
    }
    public enum MS_Slime
    {
        MPS_1007_Slime_Passive = 0,
        MAS_1008_Slime_Attack,
        MAS_1009_Slime_Slip,
        MAS_1010_Slime_Rush,
    }
    public enum MS_Wolf
    {
        MPS_1011_Wolf_Passive = 0,
        MAS_1012_Wolf_Attack,
        MAS_1013_Wolf_Scratch,
        MAS_1014_Wolf_Tackle,
        MAS_1015_Wolf_Bite,
    }
    public enum MS_Hyena
    {
        MPS_1016_Hyena_Passive = 0,
        MAS_1017_Hyena_Attack,
        MAS_1018_Hyena_Unite,
        MAS_1019_Hyena_Crisis,
    }
    public enum MPS_Goblin
    {

    }
    public enum MPS_Bat
    {

    }
    public enum MPS_Slime
    {

    }
    public enum MPS_Wolf
    {

    }
    public enum MPS_Hyena
    {

    }
    public enum MAS_Goblin
    {

    }
    public enum MAS_Bat
    {

    }
    public enum MAS_Slime
    {

    }
    public enum MAS_Wolf
    {

    }
    public enum MAS_Hyena
    {

    }

    public enum MPS_Boss1
    {

    }
    public enum MAS_Boss1
    {

    }
    #endregion

    #region [몬스터 관리]
    public struct stStatus
    {
        eMonster _type;
        float _atk;
        float _dfn;
        float _acc;
        float _avo;
        float _crt;
        float _crt_dmg;
        float _hp;
        byte _mvp;
        float _mvp_delay;
        float[] _algorithms;

        public eMonster type { get { return _type; } }
        public float atk { get { return _atk; } }
        public float dfn { get { return _dfn; } }
        public float acc { get { return _acc; } }
        public float avo { get { return _avo; } }
        public float crt { get { return _crt; } }
        public float crt_dmg { get { return _crt_dmg; } }
        public float hp { get { return _hp; } }
        public byte mvp { get { return _mvp; } }
        public float mvp_delay { get { return _mvp_delay; } }
        public float[] algorithms { get { return _algorithms; } }

        public stStatus(eMonster t, float at, float df, float ac, float av
                    , float cr, float cr_dmg, float h, float mv_delay, string alg)
        {
            _type = t;
            _atk = at;
            _dfn = df;
            _acc = ac;
            _avo = av;
            _crt = cr;
            _crt_dmg = cr_dmg;
            _hp = h;
            _mvp = 0;
            _mvp_delay = mv_delay;
            string[] tmpss = alg.Split(',');
            float[] tmpr = new float[tmpss.Length];
            for (int i = 0; i < tmpss.Length; i++)
            {
                if (float.TryParse(tmpss[i], out float tmpF))
                    tmpr[i] = tmpF;
                else
                    Debug.LogError(string.Format("stStatus Error! on Define"));
            }
                
            _algorithms = tmpr;
        }
    }
    #endregion

    public struct stTilePos
    {
        public int row;
        public int col;

        public stTilePos(int r, int c)
        {
            row = r;
            col = c;
        }

        public static stTilePos operator +(stTilePos stp1, stTilePos stp2)
        {
            return new stTilePos(stp1.row + stp2.row, stp1.col + stp2.col);
        }
        public static stTilePos operator -(stTilePos stp1, stTilePos stp2)
        {
            return new stTilePos(stp1.row - stp2.row, stp1.col - stp2.col);
        }
        public static stTilePos operator -(stTilePos stp)
        {
            return new stTilePos(stp.row, -stp.col);
        }
        public static bool operator ==(stTilePos stp1, stTilePos stp2)
        {
            if (stp1.row == stp2.row)
                if (stp1.col == stp2.col)
                    return true;
            return false;
        }
        public static bool operator !=(stTilePos stp1, stTilePos stp2)
        {
            if (stp1.row == stp2.row)
                if (stp1.col == stp2.col)
                    return false;
            return true;
        }
        public static stTilePos Opposite(stTilePos stp)
        {
            return new stTilePos(stp.row, -stp.col);
        }
        public override string ToString()
        {
            return string.Format("[{0}, {1}]", row, col);
        }
    }

    public struct stFieldInfo
    {
        public int row;
        public int col;
        public int min;
        public int max;
        public List<eMonster> monsters;
        public List<float> rates;

        public stFieldInfo(int r, int c, int mn, int mx, List<eMonster> mL, List<float> rL)
        {
            row = r;
            col = c;
            min = mn;
            max = mx;
            monsters = mL;
            rates = rL;
        }
    }

    
}

