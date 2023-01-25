using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Def;

public class M_Bat : Monster
{
    public const eMonster ownType = eMonster.Bat;

    public M_Bat(stStatus st) : base(st)
    {
        type = st.type;
        atk = st.atk;
        dfn = st.dfn;
        acc = st.acc;
        avo = st.avo;
        crt = st.crt;
        crt_dmg = st.crt_dmg;
        hp = st.hp;
        nowHP = hp;
        mvp = st.mvp;
        mvp_delay = st.mvp_delay;
        algorithms = st.algorithms;
    }
    private void Update()
    {
        if (isDead) return;
        if (isStart)
        {
            timer += Time.deltaTime;
            MvpBarSet(timer / mvp_delay);
            if (timer > mvp_delay)
            {
                AlgAction();
                timer = 0;
                MvpBarSet(0);
            }
        }
    }
    void AlgAction()
    {
        string algLog = string.Empty;
        #region [알고리즘 설명]
        /*
         * 박쥐 알고리즘 순서 
         * (0) 게으른가? -> 5%
         * (1) 확산 음파가 쿨타임인가? -> 확산 음파 범위 안에 적이 있는가?
         * -> YES : 확률 계산하여 공격, 실패시 밑으로
         * ->  NO : 다음으로
         * (2) 초음파가 쿨타임인가 -> 범위 안에 있는가
         * -> YES : 확률 계싼 공격, 실패시 밑
         * -> NO : 다음으로
         * (3) 일반 공격을 하면 맞는가?
         * -> YES : 확률 계산 공격, 실패시 밑
         * ->  NO : 이동한다
         */
        #endregion

        #region [게으름]
        if (Random.Range(0, 100f) < 5)
        {
            algLog += "귀찮아서 행동하지 않았습니다.\n";
            Debug.Log(algLog);
            return;
        }
        #endregion

        #region [공격 확인]
        stTilePos checkPos = player.pPos - ownPos;
        List<stTilePos> areaPoses = skills[(int)MS_Bat.MAS_1006_Bat_WideWave].attackArea;
        if (isRight)
        {
            algLog += "→";
            #region [확산 음파 사용 체크]
            algLog += "확산 음파";
            if (Random.Range(0, 100f) < 50) // 확산 음파 사용 확률 50%
            {
                algLog += "(성공) 쿨";
                if (CanUseSkill(skills[(int)MS_Bat.MAS_1006_Bat_WideWave]))
                {
                    algLog += "(성공) 범위";
                    if (InArea_isR(checkPos, areaPoses))
                    {
                        // 사용 가능하면 때리고 끝낸다
                        algLog += "(성공) 발동\n";
                        WideWave(areaPoses);
                        skills[(int)MS_Bat.MAS_1006_Bat_WideWave].lastUse = Time.time;
                        Debug.Log(algLog);
                        return;
                    }
                    else if (InArea_isL(checkPos, areaPoses))
                    {
                        algLog += "반대임, 다시";
                        // 반대편에 있다면 
                        timer = mvp_delay * 0.5f; // 행동력 절반만 소모해서
                        SeeLeft(); // 왼쪽 바라봄
                        Debug.Log(algLog);
                        return;
                    }
                }
            }
            algLog += "(실패) : ";
            #endregion
            #region [초음파 사용 체크]
            areaPoses = skills[(int)MS_Bat.MAS_1005_Bat_Wave].attackArea;
            algLog += "초음파";
            if (Random.Range(0, 100f) < 70) // 초음파 사용 확률 70%
            {
                algLog += "(성공) 쿨";
                if (CanUseSkill(skills[(int)MS_Bat.MAS_1005_Bat_Wave]))
                {
                    algLog += "(성공) 범위";
                    if (InArea_isR(checkPos, areaPoses))
                    {
                        // 사용 가능하면 때리고 끝낸다
                        algLog += "(성공) 발동\n";
                        Wave(areaPoses);
                        skills[(int)MS_Bat.MAS_1005_Bat_Wave].lastUse = Time.time;
                        Debug.Log(algLog);
                        return;
                    }
                    else if (InArea_isL(checkPos, areaPoses))
                    {
                        algLog += "반대임, 다시";
                        // 반대편에 있다면 
                        timer = mvp_delay * 0.5f; // 행동력 절반만 소모해서
                        SeeLeft(); // 왼쪽 바라봄
                        Debug.Log(algLog);
                        return;
                    }
                }
            }
            algLog += "(실패) : ";
            #endregion
            #region [일반 공격]
            areaPoses = skills[(int)MS_Bat.MAS_1004_Bat_Attack].attackArea;
            algLog += "일반 공격";
            if (Random.Range(0, 100f) < 95) // 일반공격 사용 확률 95%
            {
                algLog += "(성공) 범위";
                if (InArea_isR(checkPos, areaPoses))
                {
                    // 사용 가능하면 때리고 끝낸다
                    algLog += "(성공) 발동\n";
                    Wave(areaPoses);
                    Debug.Log(algLog);
                    return;
                }
                else if (InArea_isL(checkPos, areaPoses))
                {
                    algLog += "반대임, 다시";
                    // 반대편에 있다면 
                    timer = mvp_delay * 0.5f; // 행동력 절반만 소모해서
                    SeeLeft(); // 왼쪽 바라봄
                    Debug.Log(algLog);
                    return;
                }
            }
            algLog += "(실패)\n";
            #endregion
        }
        else // 왼쪽 보고 있을 때
        {
            algLog += "←";
            #region [확산 음파 사용 체크]
            algLog += "확산 음파";
            if (Random.Range(0, 100f) < 50) // 확산 음파 사용 확률 50%
            {
                algLog += "(성공) 쿨";
                if (CanUseSkill(skills[(int)MS_Bat.MAS_1006_Bat_WideWave]))
                {
                    algLog += "(성공) 범위";
                    if (InArea_isL(checkPos, areaPoses))
                    {
                        // 사용 가능하면 때리고 끝낸다
                        algLog += "(성공) 발동\n";
                        WideWave(areaPoses);
                        skills[(int)MS_Bat.MAS_1006_Bat_WideWave].lastUse = Time.time;
                        Debug.Log(algLog);
                        return;
                    }
                    else if (InArea_isR(checkPos, areaPoses))
                    {
                        algLog += "반대임, 다시";
                        // 반대편에 있다면 
                        timer = mvp_delay * 0.5f; // 행동력 절반만 소모해서
                        SeeRight(); // 오른쪽 바라봄
                        Debug.Log(algLog);
                        return;
                    }
                }
            }
            algLog += "(실패) : ";
            #endregion
            #region [초음파 사용 체크]
            areaPoses = skills[(int)MS_Bat.MAS_1005_Bat_Wave].attackArea;
            algLog += "초음파";
            if (Random.Range(0, 100f) < 70) // 초음파 사용 확률 70%
            {
                algLog += "(성공) 쿨";
                if (CanUseSkill(skills[(int)MS_Bat.MAS_1005_Bat_Wave]))
                {
                    algLog += "(성공) 범위";
                    if (InArea_isL(checkPos, areaPoses))
                    {
                        // 사용 가능하면 때리고 끝낸다
                        algLog += "(성공) 발동\n";
                        Wave(areaPoses);
                        skills[(int)MS_Bat.MAS_1005_Bat_Wave].lastUse = Time.time;
                        Debug.Log(algLog);
                        return;
                    }
                    else if (InArea_isR(checkPos, areaPoses))
                    {
                        algLog += "반대임, 다시";
                        // 반대편에 있다면 
                        timer = mvp_delay * 0.5f; // 행동력 절반만 소모해서
                        SeeRight(); // 오른쪽 바라봄
                        Debug.Log(algLog);
                        return;
                    }
                }
            }
            algLog += "(실패) : ";
            #endregion
            #region [일반 공격]
            areaPoses = skills[(int)MS_Bat.MAS_1004_Bat_Attack].attackArea;
            algLog += "일반 공격";
            if (Random.Range(0, 100f) < 95) // 일반공격 사용 확률 95%
            {
                algLog += "(성공) 범위";
                if (InArea_isL(checkPos, areaPoses))
                {
                    // 사용 가능하면 때리고 끝낸다
                    algLog += "(성공) 발동\n";
                    Wave(areaPoses);
                    Debug.Log(algLog);
                    return;
                }
                else if (InArea_isR(checkPos, areaPoses))
                {
                    algLog += "반대임, 다시";
                    // 반대편에 있다면 
                    timer = mvp_delay * 0.5f; // 행동력 절반만 소모해서
                    SeeRight(); // 오른쪽 바라봄
                    Debug.Log(algLog);
                    return;
                }
            }
            algLog += "(실패)\n";
            #endregion
        }
        #endregion

        #region [이동 확인]
        algLog += "이동 : ";
        stTilePos targetPos = player.pPos;
        if (checkPos.col < 0)
        {
            algLog += "대상 좌표는 플레이어 오른쪽\n";
            targetPos = player.pPos + new stTilePos(0, 1);
        }
        else if (checkPos.col > 0)
        {
            algLog += "대상 좌표는 플레이어 왼쪽\n";
            targetPos = player.pPos - new stTilePos(0, 1);
        }
        else
        {
            algLog += "대상 좌표는 플레이어 좌우 중 랜덤";
            if (Random.Range(0, 2) == 0)
            {
                targetPos = player.pPos + new stTilePos(0, 1);
                algLog += "(오른쪽)\n";
            }
            else
            {
                targetPos = player.pPos - new stTilePos(0, 1);
                algLog += "(왼쪽)\n";
            }
        }
        checkPos = targetPos - ownPos;
        eMoveDir dir = eMoveDir.Up;
        algLog += "이동";
        if (checkPos.col > 0)
        {
            if (checkPos.row > 0)
            {
                if (Random.Range(0, 2) == 0)
                {
                    algLog += "(오른쪽) ";
                    dir = eMoveDir.Right;
                }
                else
                {
                    algLog += "(아래) ";
                    dir = eMoveDir.Down;
                }
            }
            else if (checkPos.row < 0)
            {
                if (Random.Range(0, 2) == 0)
                {
                    algLog += "(오른쪽) ";
                    dir = eMoveDir.Right;
                }
                else
                {
                    algLog += "(위) ";
                    dir = eMoveDir.Up;
                }
            }
            else
            {
                algLog += "(오른쪽) ";
                dir = eMoveDir.Right;
            }
        }
        else if (checkPos.col < 0)
        {
            if (checkPos.row > 0)
            {
                if (Random.Range(0, 2) == 0)
                {
                    algLog += "(왼쪽) ";
                    dir = eMoveDir.Left;
                }
                else
                {
                    algLog += "(아래) ";
                    dir = eMoveDir.Down;
                }
            }
            else if (checkPos.row < 0)
            {
                if (Random.Range(0, 2) == 0)
                {
                    algLog += "(왼쪽) ";
                    dir = eMoveDir.Left;
                }
                else
                {
                    algLog += "(위) ";
                    dir = eMoveDir.Up;
                }
            }
            else
            {
                algLog += "(왼쪽) ";
                dir = eMoveDir.Right;
            }
        }
        else if (checkPos.row > 0)
        {
            algLog += "(아래) ";
            dir = eMoveDir.Down;
        }
        else if (checkPos.row < 0)
        {
            algLog += "(위) ";
            dir = eMoveDir.Up;
        }

        algLog += "판단 : ";
        float correctDirRate = 70f;
        float rdDirRate = Random.Range(0, 100f);
        if (rdDirRate < correctDirRate)
        {
            algLog += "정상, 이동 시도";
            if (!MonsterMove(dir))
            {
                algLog += "(실패)";
                eMoveDir oppoDir = Res.Op(dir);
                // 근데 이동하지 못했음
                eMoveDir newDir = (eMoveDir)Random.Range(0, System.Enum.GetValues(typeof(eMoveDir)).Length);
                while (newDir == dir || newDir == oppoDir)
                    newDir = (eMoveDir)Random.Range(0, System.Enum.GetValues(typeof(eMoveDir)).Length);
                // 새로운 방향을 잡고 이동
                if (!MonsterMove(newDir))
                {
                    algLog += "(실패)";
                    // 기존 두개 중 하나의 반대방향 랜덤하게 설정하여 그쪽으로 이동
                    eMoveDir tmpDir = oppoDir;
                    if (Random.Range(0, 2) == 0)
                        tmpDir = Res.Op(newDir);
                    if (!MonsterMove(tmpDir))
                    {
                        // 그것도 안되면 마지막 하나 남은 거 찾아서 그쪽으로
                        algLog += "(실패)";
                        eMoveDir lastDir = oppoDir;
                        if (tmpDir == oppoDir)
                            lastDir = Res.Op(newDir);
                        if (!MonsterMove(lastDir))
                        {
                            // 그것도 안되면 포기~
                            algLog += "(실패), 이동하지 않고 행동력 소모함\n";
                            Debug.Log(algLog);
                            return;
                        }
                    }
                }
            }
        }
        else
        {
            algLog += "멍청, 이동 시도 ";
            eMoveDir newDir = Res.Op(dir);
            if (!MonsterMove(newDir))
            {
                algLog += "(실패)";
                eMoveDir tmpDir = (eMoveDir)Random.Range(0, System.Enum.GetValues(typeof(eMoveDir)).Length);
                // 기존 2개 제외한 새 방향을 찾음
                while (tmpDir == dir || tmpDir == newDir)
                    tmpDir = (eMoveDir)Random.Range(0, System.Enum.GetValues(typeof(eMoveDir)).Length);
                if (!MonsterMove(tmpDir))
                {
                    algLog += "(실패)";
                    // 마지막 남은 방향은 tmpDir의 반대방향이므로 구해옴
                    eMoveDir lastDir = Res.Op(tmpDir);
                    if (!MonsterMove(lastDir))
                    {
                        algLog += "(실패)";
                        // 근데 거기도 이동 못한다? 그럼 어쩔 수 없이 처음 가기로 했던 곳 감
                        if (!MonsterMove(dir))
                        {
                            // 움직일 수 있는 곳이 아예 없음...
                            algLog += "(실패), 이동하지 않고 행동력 소모함\n";
                            Debug.Log(algLog);
                            return;
                        }
                    }
                }
            }
        }
        algLog += "(성공)\n";
        Debug.Log(algLog);
        #endregion
    }

    bool NormalAttack(stTilePos area)
    {
        if (isRight)
        {
            int targetRow = ownPos.row + area.row;
            int targetCol = ownPos.col + area.col;
            if (targetRow < 0 || targetRow >= FieldManager.m.curMaxRow)
                return false; // 공격할 수 없음
            if (targetCol < 0 || targetCol >= FieldManager.m.curMaxCol)
                return false;
            FieldManager.m.curTiles[targetRow, targetCol].MonsterAttackOn(atk, acc);
            return true;
        }
        else
        {
            int targetRow = ownPos.row + area.row;
            int targetCol = ownPos.col - area.col;
            if (targetRow < 0 || targetRow >= FieldManager.m.curMaxRow)
                return false; // 공격할 수 없음
            if (targetCol < 0 || targetCol >= FieldManager.m.curMaxCol)
                return false;
            FieldManager.m.curTiles[targetRow, targetCol].MonsterAttackOn(atk, acc);
            return true;
        }
    }
    void Wave(List<stTilePos> areas)
    {
        List<stTilePos> result = new List<stTilePos>();
        if (isRight)
        {
            foreach (stTilePos area in areas)
            {
                int targetRow = ownPos.row + area.row;
                int targetCol = ownPos.col + area.col;
                if (targetRow < 0 || targetRow >= FieldManager.m.curMaxRow)
                    continue; // 공격할 수 없음
                if (targetCol < 0 || targetCol >= FieldManager.m.curMaxCol)
                    continue;
                result.Add(new stTilePos(targetRow, targetCol));
            }
        }
        else
        {
            foreach (stTilePos area in areas)
            {
                int targetRow = ownPos.row + area.row;
                int targetCol = ownPos.col - area.col;
                if (targetRow < 0 || targetRow >= FieldManager.m.curMaxRow)
                    continue; // 공격할 수 없음
                if (targetCol < 0 || targetCol >= FieldManager.m.curMaxCol)
                    continue;
                result.Add(new stTilePos(targetRow, targetCol));
            }
        }
        float.TryParse(skills[(int)MS_Bat.MAS_1005_Bat_Wave].values[0], out float tmp);
        float newAtk = atk * tmp;
        foreach (stTilePos target in result)
        {
            FieldManager.m.curTiles[target.row, target.col].MonsterAttackOn(newAtk, acc);
        }
        
    }
    void WideWave(List<stTilePos> areas)
    {
        List<stTilePos> result = new List<stTilePos>();
        if (isRight)
        {
            foreach (stTilePos area in areas)
            {
                int targetRow = ownPos.row + area.row;
                int targetCol = ownPos.col + area.col;
                if (targetRow < 0 || targetRow >= FieldManager.m.curMaxRow)
                    continue; // 공격할 수 없음
                if (targetCol < 0 || targetCol >= FieldManager.m.curMaxCol)
                    continue;
                result.Add(new stTilePos(targetRow, targetCol));
            }
        }
        else
        {
            foreach (stTilePos area in areas)
            {
                int targetRow = ownPos.row + area.row;
                int targetCol = ownPos.col - area.col;
                if (targetRow < 0 || targetRow >= FieldManager.m.curMaxRow)
                    continue; // 공격할 수 없음
                if (targetCol < 0 || targetCol >= FieldManager.m.curMaxCol)
                    continue;
                result.Add(new stTilePos(targetRow, targetCol));
            }
        }
        float.TryParse(skills[(int)MS_Bat.MAS_1006_Bat_WideWave].values[0], out float tmp);
        float newAtk = atk * tmp;
        foreach (stTilePos target in result)
        {
            FieldManager.m.curTiles[target.row, target.col].MonsterAttackOn(newAtk, acc);
        }
    }
    void Bat_Passive(float atai)
    {
        avo += atai;
    }
    public void InitSet(stStatus st, stTilePos stp)
    {
        if (player == null) player = PlayerCtrlManager.m.curPlayer;
        type = st.type;
        atk = st.atk;
        dfn = st.dfn;
        acc = st.acc;
        avo = st.avo;
        crt = st.crt;
        crt_dmg = st.crt_dmg;
        hp = st.hp;
        nowHP = hp;
        mvp = st.mvp;
        mvp_delay = (st.mvp_delay * Random.Range(0.9f, 1.1f));
        algorithms = st.algorithms;
        ownPos = stp;
        SpriteSet();
        skills = new List<MonsterSkill>();

        // 박쥐
        skills.Add(SkillManager.m.mSkillDict[(int)eMSkillCode.MPS_1003_Bat_Passive]);
        float.TryParse(skills[0].values[0], out float tmp);
        Bat_Passive(tmp);
        skills.Add(SkillManager.m.mSkillDict[(int)eMSkillCode.MAS_1004_Bat_Attack]);
        skills.Add(SkillManager.m.mSkillDict[(int)eMSkillCode.MAS_1005_Bat_Wave]);
        skills.Add(SkillManager.m.mSkillDict[(int)eMSkillCode.MAS_1006_Bat_WideWave]);

        foreach (MonsterSkill ms in skills)
            if (ms.rechargeTime > 0)
                ms.lastUse = Time.time;
    }
}
