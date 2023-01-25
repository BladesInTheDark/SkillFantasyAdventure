using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Def;

// 플레이어 조작을 관리하는 클래스
public class PlayerCtrlManager : MonoBehaviour
{
    #region [인스턴스화]
    static PlayerCtrlManager _uniqueInstance;
    public static PlayerCtrlManager m
    {
        get { return _uniqueInstance; }
    }
    #endregion

    public Player curPlayer;
    public UserSkill[] pSkills = new UserSkill[3];
    public UserSkill[] aSkills = new UserSkill[3];

    private void Awake()
    {
        _uniqueInstance = this;
    }

    public void InitSet(Player p)
    {
        curPlayer = p;
    }
    public void PassiveSkillSet(PassiveSkillButton psb, byte idx)
    {
        // 패시브 스킬은 이게 들어오면 버튼에 보내주고 플레이어 스탯 등에 처리함
        psb.InitSet(SkillManager.m.psb[idx]);
        if (psb.ownSkill == null) return;
        eUSkillCode code = (eUSkillCode)(psb.ownSkill.code / 10);
        switch (code)
        {
            case eUSkillCode.UPS_1000_Attack_Up:
                UPS_1000_Attack_Up(SkillManager.m.psb[idx]);
                break;
            case eUSkillCode.UPS_1001_Defence_Up:
                UPS_1001_Defence_Up(SkillManager.m.psb[idx]);
                break;
            case eUSkillCode.UPS_1002_HP_Up:
                UPS_1002_HP_Up(SkillManager.m.psb[idx]);
                break;
            case eUSkillCode.UPS_1003_Mvp_Up:
                UPS_1003_Mvp_Up(SkillManager.m.psb[idx]);
                break;
            case eUSkillCode.UPS_1004_Mvp_Max_Up:
                UPS_1004_Mvp_Max_Up(SkillManager.m.psb[idx]);
                break;
            case eUSkillCode.UPS_1005_Mvp_Cooltime_Down:
                UPS_1005_Mvp_Cooltime_Down(SkillManager.m.psb[idx]);
                break;
            case eUSkillCode.UPS_1006_Aura_Attack_Down:
                UPS_1006_Aura_Attack_Down(SkillManager.m.psb[idx]);
                break;
            case eUSkillCode.UPS_1007_Aura_Acc_Down:
                UPS_1007_Aura_Acc_Down(SkillManager.m.psb[idx]);
                break;
            case eUSkillCode.UPS_1008_Scope_Normal_1x2:
                UPS_1008_Scope_Normal_1x2(SkillManager.m.psb[idx]);
                break;
            case eUSkillCode.UPS_1009_HighLisk_HighReturn:
                UPS_1009_HighLisk_HighReturn(SkillManager.m.psb[idx]);
                break;
            case eUSkillCode.UPS_1010_Goblin:
                UPS_1010_Goblin(SkillManager.m.psb[idx]);
                break;
            case eUSkillCode.UPS_1011_Wolf:
                UPS_1011_Wolf(SkillManager.m.psb[idx]);
                break;

        }
    }
    public void ActiveSkillSet(ActiveSkillButton asb, byte idx)
    {
        // 액티브 스킬은 이게 들어오면 버튼에 AddListener함
        aSkills[idx] = SkillManager.m.asb[idx];
        asb.InitSet(aSkills[idx]);
        if (aSkills[idx] == null) return;
        eUSkillCode skillCode = (eUSkillCode)(aSkills[idx].code / 10);
        curPlayer.skillAreas[idx] = aSkills[idx].attackArea;
        switch (skillCode)
        {
            case eUSkillCode.UAS_1012_OneInchPunch:
                asb.btn.onClick.AddListener(delegate { UAS_1012_OneInchPunch(idx); });
                break;
            case eUSkillCode.UAS_1013_SpinArround:
                asb.btn.onClick.AddListener(delegate { UAS_1013_SpinArround(idx); });
                break;
            case eUSkillCode.UAS_1014_HardSkin:
                asb.btn.onClick.AddListener(delegate { UAS_1014_HardSkin(idx); });
                break;
            case eUSkillCode.UAS_1015_ProtectionHand:
                asb.btn.onClick.AddListener(delegate { UAS_1015_ProtectionHand(idx); });
                break;
            case eUSkillCode.UAS_1016_SkillDmgEnhance:
                asb.btn.onClick.AddListener(delegate { UAS_1016_SkillDmgEnhance(idx); });
                break;
            case eUSkillCode.UAS_1017_Curse:
                asb.btn.onClick.AddListener(delegate { UAS_1017_Curse(idx); });
                break;
            case eUSkillCode.UAS_1018_Roaring:
                asb.btn.onClick.AddListener(delegate { UAS_1018_Roaring(idx); });
                break;
        }
        
    }

    public void PlayerMove(eMoveDir emd)
    {
        switch (emd)
        {
            case eMoveDir.Up:
                int targetRowU = curPlayer.pPos.row - 1;
                if (targetRowU >= 0)
                {
                    if (curPlayer.mvp > 0)
                    {
                        if (!FieldManager.m.curTiles[targetRowU, curPlayer.pPos.col].isUnitOn)
                        {
                            // 행동력 있음, 이동 처리
                            FieldManager.m.TileWhere(curPlayer.pPos).UnitOff();
                            curPlayer.pPos.row = targetRowU;
                            curPlayer.PosSet(curPlayer.pPos);
                            IngameMainCV.m.nowProcessing = false;
                            return;
                        }
                    }
                }
                AudioManager.m.PlaySE(R_SE.PlayerCantMove);
                break;
            case eMoveDir.Down:
                int targetRowD = curPlayer.pPos.row + 1;
                if (targetRowD < FieldManager.m.curMaxRow)
                {
                    if (curPlayer.mvp > 0)
                    {
                        if (!FieldManager.m.curTiles[targetRowD, curPlayer.pPos.col].isUnitOn)
                        {
                            // 행동력 있음, 이동 처리
                            FieldManager.m.TileWhere(curPlayer.pPos).UnitOff();
                            curPlayer.pPos.row = targetRowD;
                            curPlayer.PosSet(curPlayer.pPos);
                            IngameMainCV.m.nowProcessing = false;
                            return;
                        }
                    }
                }
                AudioManager.m.PlaySE(R_SE.PlayerCantMove);
                break;
            case eMoveDir.Left:
                int targetColL = curPlayer.pPos.col - 1;
                if (targetColL >= 0)
                {
                    if (curPlayer.mvp > 0)
                    {
                        if (!FieldManager.m.curTiles[curPlayer.pPos.row, targetColL].isUnitOn)
                        {
                            // 행동력 있음, 이동 처리
                            FieldManager.m.TileWhere(curPlayer.pPos).UnitOff();
                            curPlayer.pPos.col = targetColL;
                            curPlayer.PosSet(curPlayer.pPos);
                            curPlayer.SeeLeft();
                            IngameMainCV.m.nowProcessing = false;
                            return;
                        }
                    }
                }
                curPlayer.SeeLeft();
                AudioManager.m.PlaySE(R_SE.PlayerCantMove);
                break;
            case eMoveDir.Right:
                int targetColR = curPlayer.pPos.col + 1;
                if (targetColR < FieldManager.m.curMaxCol)
                {
                    if (curPlayer.mvp > 0)
                    {
                        if (!FieldManager.m.curTiles[curPlayer.pPos.row, targetColR].isUnitOn)
                        {
                            // 행동력 있음, 이동 처리
                            FieldManager.m.TileWhere(curPlayer.pPos).UnitOff();
                            curPlayer.pPos.col = targetColR;
                            curPlayer.PosSet(curPlayer.pPos);
                            curPlayer.SeeRight();
                            IngameMainCV.m.nowProcessing = false;
                            return;
                        }
                    }
                }
                curPlayer.SeeRight();
                AudioManager.m.PlaySE(R_SE.PlayerCantMove);
                break;
        }
        IngameMainCV.m.nowProcessing = false;
    }
    public void FieldReset()
    {
        curPlayer.mvp = 0;
    }
    public void KnockBackByMonsterSkill(stTilePos stp)
    {
        FieldManager.m.TileWhere(curPlayer.pPos).UnitOff();
        curPlayer.PosSet(stp, false);
    }

    #region [패시브 스킬들]
    public void UPS_1000_Attack_Up(UserSkill us)
    {
        if (float.TryParse(us.values[0], out float tmp))
            curPlayer.atk += tmp;
    }
    public void UPS_1001_Defence_Up(UserSkill us)
    {
        if (float.TryParse(us.values[0], out float tmp))
            curPlayer.dfn += tmp;
    }
    public void UPS_1002_HP_Up(UserSkill us)
    {
        if (float.TryParse(us.values[0], out float tmp))
            curPlayer.maxHP += tmp;
        curPlayer.nowHP = curPlayer.maxHP;
        curPlayer.HPBarSet();
    }
    public void UPS_1003_Mvp_Up(UserSkill us)
    {
        if (byte.TryParse(us.values[0], out byte tmp))
        {
            if (tmp > curPlayer.mvp_max) tmp = curPlayer.mvp_max;
            curPlayer.mvp = tmp;
            curPlayer.startMvp = tmp;
            curPlayer.MvpTextSet();
        }
    }
    public void UPS_1004_Mvp_Max_Up(UserSkill us)
    {
        if (byte.TryParse(us.values[0], out byte tmp))
        {
            curPlayer.mvp_max += tmp;
            curPlayer.MvpMaxTextSet();
        }
    }
    public void UPS_1005_Mvp_Cooltime_Down(UserSkill us)
    {
        if (float.TryParse(us.values[0], out float tmp))
        {
            curPlayer.mvp_delay -= tmp;
        }
    }
    public void UPS_1006_Aura_Attack_Down(UserSkill us)
    {
        string[] rc = us.values[0].Split('x');
        if (float.TryParse(us.values[1], out float tmp))
        {
            List<stTilePos> tiles = new List<stTilePos>();
            if (!int.TryParse(rc[0], out int row))
                Debug.LogError("1006AuraAttackDown Error! row");
            if (!int.TryParse(rc[1], out int col))
                Debug.LogError("1006AuraAttackDown Error! col");
            int endRow = row / 2;
            int endCol = col / 2;
            for (int i = -endRow; i <= endRow; i++)
            {
                for (int j = -endCol; j <= endCol; j++)
                {
                    if (i == 0 && j == 0) continue;
                    tiles.Add(new stTilePos(i, j));
                }
            }
            curPlayer.AuraSet(tiles, eUSkillCode.UPS_1006_Aura_Attack_Down, tmp);
        }
    }
    public void UPS_1007_Aura_Acc_Down(UserSkill us)
    {
        string[] rc = us.values[0].Split('x');
        if (float.TryParse(us.values[1], out float tmp))
        {
            List<stTilePos> tiles = new List<stTilePos>();
            if (!int.TryParse(rc[0], out int row))
                Debug.LogError("1007AuraAccDown Error! row");
            if (!int.TryParse(rc[1], out int col))
                Debug.LogError("1007AuraAccDown Error! col");
            int endRow = row / 2;
            int endCol = col / 2;
            for (int i = -endRow; i <= endRow; i++)
            {
                for (int j = -endCol; j <= endCol; j++)
                {
                    if (i == 0 && j == 0) continue;
                    tiles.Add(new stTilePos(i, j));
                }
            }
            curPlayer.AuraSet(tiles, eUSkillCode.UPS_1007_Aura_Acc_Down, tmp);
        }
    }
    public void UPS_1008_Scope_Normal_1x2(UserSkill us)
    {
        string[] rc = us.values[0].Split('x');
        if (float.TryParse(us.values[1], out float tmp))
        {
            List<stTilePos> attackArea = new List<stTilePos>();
            attackArea.Add(new stTilePos(0, 1));
            attackArea.Add(new stTilePos(0, 2));
            curPlayer.NormalAttackAreaChange(attackArea, tmp);
        }
    }
    public void UPS_1009_HighLisk_HighReturn(UserSkill us)
    {
        // 나중에 구현 가능
    }
    public void UPS_1010_Goblin(UserSkill us)
    {
        // 나중에 구현 가능
    }
    public void UPS_1011_Wolf(UserSkill us)
    {
        // 나중에 구현 가능
    }
#endregion

    #region [액티브 스킬들]
    public void UAS_1012_OneInchPunch(int idx)
    {
        if (curPlayer.mvp <= 0 || !IngameMainCV.m.aSkill[idx].canUse)
        {
            AudioManager.m.PlaySE(R_SE.PlayerCantMove);
            return;
        }
        IngameMainCV.m.aSkill[idx].canUse = false;
        if (float.TryParse(aSkills[idx].values[0], out float mul))
            curPlayer.OneInchPunchAnimCall(idx, mul);
        else
            Debug.LogError("Error! Use UAS_1012_OneInchPunch");
    }
    public void UAS_1013_SpinArround(int idx)
    {
        if (curPlayer.mvp <= 0 || !IngameMainCV.m.aSkill[idx].canUse)
        {
            AudioManager.m.PlaySE(R_SE.PlayerCantMove);
            return;
        }
        IngameMainCV.m.aSkill[idx].canUse = false;
        if (float.TryParse(aSkills[idx].values[0], out float dmg))
            curPlayer.SpinArroundAnimCall(idx, dmg);
        else
            Debug.LogError("Error! Use UAS_1013_SpinArround");
    }
    public void UAS_1014_HardSkin(int idx)
    {
        if (curPlayer.mvp <= 0 || !IngameMainCV.m.aSkill[idx].canUse)
        {
            AudioManager.m.PlaySE(R_SE.PlayerCantMove);
            return;
        }
        IngameMainCV.m.aSkill[idx].canUse = false;
        if (!float.TryParse(aSkills[idx].values[0], out float sec))
            Debug.LogError("Error! Use UAS_1014_HardSkin"); 
        if (!float.TryParse(aSkills[idx].values[1], out float dfn))
            Debug.LogError("Error! Use UAS_1014_HardSkin");
        curPlayer.HardSkin(idx, sec, dfn);
        IngameMainCV.m.nowProcessing = false;
    }
    public void UAS_1015_ProtectionHand(int idx)
    {
        if (curPlayer.mvp <= 0 || !IngameMainCV.m.aSkill[idx].canUse)
        {
            AudioManager.m.PlaySE(R_SE.PlayerCantMove);
            return;
        }
        IngameMainCV.m.aSkill[idx].canUse = false;
        if (!float.TryParse(aSkills[idx].values[0], out float sec))
            Debug.LogError("Error! Use UAS_1015_ProtectionHand");
        if (!int.TryParse(aSkills[idx].values[1], out int cnt))
            Debug.LogError("Error! Use UAS_1015_ProtectionHand");
        curPlayer.ProtectionHand(idx, sec, cnt);
        IngameMainCV.m.nowProcessing = false;
    }
    public void UAS_1016_SkillDmgEnhance(int idx)
    {
        if (curPlayer.mvp <= 0 || !IngameMainCV.m.aSkill[idx].canUse)
        {
            AudioManager.m.PlaySE(R_SE.PlayerCantMove);
            return;
        }
        IngameMainCV.m.aSkill[idx].canUse = false;
        if (!int.TryParse(aSkills[idx].values[0], out int cnt))
            Debug.LogError("Error! Use UAS_1016_SkillDmgEnhance");
        if (!float.TryParse(aSkills[idx].values[1], out float val))
            Debug.LogError("Error! Use UAS_1016_SkillDmgEnhance");
        curPlayer.DmgBuffAdd(cnt, val * 0.01f);
        IngameMainCV.m.nowProcessing = false;
    }
    public void UAS_1017_Curse(int idx)
    {
        if (curPlayer.mvp <= 0 || !IngameMainCV.m.aSkill[idx].canUse)
        {
            AudioManager.m.PlaySE(R_SE.PlayerCantMove);
            return;
        }
        IngameMainCV.m.aSkill[idx].canUse = false;
        curPlayer.UseMvp();
        if (!int.TryParse(aSkills[idx].values[0], out int cnt))
            Debug.LogError("Error! Use UAS_1017_Curse");
        if (!float.TryParse(aSkills[idx].values[1], out float sec))
            Debug.LogError("Error! Use UAS_1017_Curse");
        if (!float.TryParse(aSkills[idx].values[2], out float val))
            Debug.LogError("Error! Use UAS_1017_Curse");
        AudioManager.m.PlaySE(R_SE.MonsterDebuffed);
        FieldManager.m.DebuffToCurMonsters(eUSkillCode.UAS_1017_Curse, cnt, sec, val);
        IngameMainCV.m.nowProcessing = false;
    }
    public void UAS_1018_Roaring(int idx)
    {
        if (curPlayer.mvp <= 0 || !IngameMainCV.m.aSkill[idx].canUse)
        {
            AudioManager.m.PlaySE(R_SE.PlayerCantMove);
            return;
        }
        IngameMainCV.m.aSkill[idx].canUse = false;
        curPlayer.UseMvp();
        if (!int.TryParse(aSkills[idx].values[0], out int cnt))
            Debug.LogError("Error! Use UAS_1018_Roaring");
        if (!float.TryParse(aSkills[idx].values[1], out float rate))
            Debug.LogError("Error! Use UAS_1018_Roaring");
        if (!int.TryParse(aSkills[idx].values[2], out int tileCnt))
            Debug.LogError("Error! Use UAS_1018_Roaring");
        AudioManager.m.PlaySE(R_SE.Roaring);
        FieldManager.m.KnockBackToCurMonsters(eUSkillCode.UAS_1018_Roaring, cnt, rate, tileCnt);
        IngameMainCV.m.nowProcessing = false;
    }
    #endregion
}
