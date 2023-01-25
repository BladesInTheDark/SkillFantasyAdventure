using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Def;

public class M_Goblin : Monster
{
    // 알고리즘 때문에 하위 몬스터들은 다 따로 저장해야 하는 게 맞음
    // 근데 그렇다고 하면 몬스터테이블에서 불러온 스탯 반영은 어떻게 하느냐...
    // 일단 소환까지만 합시다...
    public const eMonster ownType = eMonster.Goblin;

    public M_Goblin(stStatus st) : base(st)
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
                timer = 0;
                AlgAction();
                MvpBarSet(timer / mvp_delay);
            }
        }
    }
    void AlgAction()
    {
        string algLog = string.Empty;
        #region [알고리즘 설명]
        /*
         * 알고리즘과 skills에 들어있는 스킬은 개별 관리함. (편의성 추구 x, 몬스터별로 하나하나 순서도 정하고 손으로 함.)
         * 
         * 고블린 알고리즘 순서 
         * (0) 게으른가? -> 확률적으로 아무 행동도 하지 않음 (15%)
         * (1) 일반 공격을 하면 맞는가?
         * -> YES : (2)로
         * ->  NO : 이동한다
         * (2) 강화 공격이 쿨타임인가? 
         * -> YES : 확률로 일반 공격 발동, 일반 공격 실패시 이동.
         * ->  NO : 확률로 강화 공격 발동, 강화 공격 실패시 일반 공격 발동, 일반 공격까지 실패시 이동.
         */
        #endregion
        if (player == null) player = PlayerCtrlManager.m.curPlayer;
        // 게으를 확률 15%
        #region [게으름]
        if (Random.Range(0, 100f) < 15)
        {
            algLog += "귀찮아서 행동하지 않았다.\n";
            Debug.Log(algLog);
            return;
        }
        #endregion

        #region [공격 확인]
        stTilePos checkPos = player.pPos - ownPos;
        stTilePos areaPos = skills[(int)MS_Goblin.MAS_1001_Goblin_Attack].attackArea[0];
        if (isRight)
        {
            algLog += "→ 일반 공격";
            if (checkPos == areaPos) // 오른쪽 공격 유효
            {
                algLog += "(유효) 강화 공격 쿨체크";
                // 정방향, 공격 바로 실행해도 맞음
                // 강화 공격 체크
                if (CanUseSkill(skills[(int)MS_Goblin.MAS_1002_Goblin_StrongAttack]))
                {
                    algLog += "(성공) 확률 체크";
                    // 강화 공격 쿨타임 아니면 
                    float InvokeRate0 = Random.Range(0, 100f);
                    if (InvokeRate0 < 40f) // 40% 확률로 발동
                    {
                        algLog += "(성공) 강화 공격 실행\n";
                        StrongAttack(skills[(int)MS_Goblin.MAS_1002_Goblin_StrongAttack].attackArea[0]);
                        skills[(int)MS_Goblin.MAS_1002_Goblin_StrongAttack].lastUse = Time.time;
                        Debug.Log(algLog);
                        return;
                    }
                    // 발동 못하면 이쪽으로 내려옴
                }
                // 강화공격 사용가능하지만 발동 못했을 때 || 강화공격을 못쓸때만 내려옴
                algLog += "(실패) 일반 공격 확률 체크";
                // 일반공격 가능
                float InvokeRate1 = Random.Range(0, 100f);
                if (InvokeRate1 < 80f) // 80% 확률로 발동
                {
                    algLog += "(성공) 일반 공격 실행\n";
                    NormalAttack(skills[(int)MS_Goblin.MAS_1001_Goblin_Attack].attackArea[0]);
                    Debug.Log(algLog);
                    return;
                }
                // 일반공격도 하지 못했을 때 내려옴
            }
            else if (checkPos == -areaPos) // 왼쪽 공격 유효
            {
                algLog += "(반 유효) 뒤돌아봄\n";
                // 후방향, 바로 실행 못하고 기다려야 함.
                timer = mvp_delay * 0.5f; // 행동력 절반만 소모해서
                SeeLeft(); // 왼쪽 바라봄
                Debug.Log(algLog);
                return;
            }
            // 범위에 적이 없을때 || 있지만 공격하지 못했을 때 내려옴
            // 이동함
            // 플레이어 범위 추적
        }
        else // 왼쪽 보고 있을 때
        {
            algLog += "← 일반 공격";
            if (checkPos == -areaPos) // 왼쪽 공격 유효면
            {
                algLog += "(유효) 강화 공격 쿨체크";
                // 정방향, 공격 바로 실행해도 맞음
                // 강화 공격 체크
                if (CanUseSkill(skills[(int)MS_Goblin.MAS_1002_Goblin_StrongAttack]))
                {
                    algLog += "(성공) 확률 체크";
                    // 강화 공격 쿨타임 아니면 
                    float InvokeRate0 = Random.Range(0, 100f);
                    if (InvokeRate0 < 40f) // 40% 확률로 발동
                    {
                        algLog += "(성공) 강화 공격 실행\n";
                        StrongAttack(skills[(int)MS_Goblin.MAS_1002_Goblin_StrongAttack].attackArea[0]);
                        skills[(int)MS_Goblin.MAS_1002_Goblin_StrongAttack].lastUse = Time.time;
                        Debug.Log(algLog);
                        return;
                    }
                    // 발동 못하면 이쪽으로 내려옴
                }
                // 강화공격 사용가능하지만 발동 못했을 때 || 강화공격을 못쓸때만 내려옴
                algLog +=  "(실패) 일반 공격 확률 체크";
                // 일반공격 가능
                float InvokeRate1 = Random.Range(0, 100f);
                if (InvokeRate1 < 80f) // 80% 확률로 발동
                {
                    algLog += "(성공) 일반 공격 실행\n";
                    NormalAttack(skills[(int)MS_Goblin.MAS_1001_Goblin_Attack].attackArea[0]);
                    Debug.Log(algLog);
                    return;
                }
                // 일반공격도 하지 못했을 때 내려옴
            }
            else if (checkPos == areaPos) // 오른쪽 공격 유효면
            {
                algLog += "(반 유효) 뒤돌아봄\n";
                // 후방향, 바로 실행 못하고 기다려야 함.
                timer = mvp_delay * 0.5f; // 행동력 절반만 소모해서
                SeeRight(); // 왼쪽 바라봄
                Debug.Log(algLog);
                return;
            }
            // 범위에 적이 없을때 || 있지만 공격하지 못했을 때 내려옴
            // 이동함
            // 플레이어 범위 추적
            // -> 플레이어가 본인 오른쪽에 있으면 checkPos의 col에 양수, 왼쪽에 있으면 음수가 나옴. 
            // 플레이어가 본인 왼쪽에 있다면 플레이어의 바로 오른쪽 지점이 이동 목표
            // 플레이어가 본인 오른쪽에 있다면 플레이어의 바로 왼쪽 지점이 이동 목표
        }
        algLog += "(실패)\n";
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
    bool StrongAttack(stTilePos area)
    {
        if (isRight)
        {
            int targetRow = ownPos.row + area.row;
            int targetCol = ownPos.col + area.col;
            if (targetRow < 0 || targetRow >= FieldManager.m.curMaxRow)
                return false; // 공격할 수 없음
            if (targetCol < 0 || targetCol >= FieldManager.m.curMaxCol)
                return false;
            FieldManager.m.curTiles[targetRow, targetCol].MonsterAttackOn(atk * 2, acc);
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
            FieldManager.m.curTiles[targetRow, targetCol].MonsterAttackOn(atk * 2, acc);
            return true;
        }
    }
    void Goblin_Passive(float val)
    {
        acc += val;
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
        mvp_delay = (st.mvp_delay * Random.Range(0.9f, 1.1f)); // 행동력 살짝 꼬아서 랜덤하게 움직이도록 함 
        algorithms = st.algorithms;
        ownPos = stp;
        SpriteSet();
        skills = new List<MonsterSkill>();

        // 고블린 특별!
        skills.Add(SkillManager.m.mSkillDict[(int)eMSkillCode.MPS_1000_Goblin_Passive]);
        if (float.TryParse(skills[0].values[0], out float tmpfloat))
            Goblin_Passive(tmpfloat);
        else
            Debug.LogError(string.Format("M_Goblin Error!(value)"));
        skills.Add(SkillManager.m.mSkillDict[(int)eMSkillCode.MAS_1001_Goblin_Attack]);
        skills.Add(SkillManager.m.mSkillDict[(int)eMSkillCode.MAS_1002_Goblin_StrongAttack]);

        foreach (MonsterSkill ms in skills)
            if (ms.rechargeTime > 0)
                ms.lastUse = Time.time;

        player = PlayerCtrlManager.m.curPlayer;
    }
}
