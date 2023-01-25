using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Def;

public class M_Slime : Monster
{
    public const eMonster ownType = eMonster.Slime;

    public M_Slime(stStatus st) : base(st)
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

        // 알고리즘 재귀로 하면 할 수 있음
        // 한번에 다 검색하는 것이 아니라 차례차례 하면 할 수 있다???

        string algLog = string.Empty;
        #region [알고리즘 설명]
        /*
         * 슬라임 알고리즘 순서 
         * (0) 게으른가? -> 10%
         * (1) 일반 공격을 하면 맞는가?
         * -> YES : 밀려듦!이 쿨타임인가? -> 쿨타임이면 다음으로, 아니면 확률적으로 사용함 
         * ->  NO : 다음으로
         * (2) 이동할 수 있는가? 
         * -> YES : 평범한 이동 판정 저장(실행x)
         * ->  NO : 이동 안하고 반환함
         * (3) 미끄러짐!이 쿨타임인가?
         * -> YES : 일반 이동 실행
         * ->  NO : (사용확률60%) 미끄러짐! 사용, 1칸씩 총 2칸 이동 시도하지만 도중에 멈췄을 못했을 경우 쿨타임 반환받음
         * (4) 이동 이후 인접한 타일 체크, 3마리 이상일 경우 네임드 진화(나중에 구현 예정)
         * 
         * + 피격 알고리즘
         * 크리티컬인가? -> 바로 사망함 << 이걸 어떻게 인식해야 하는가? 고민 필요
         */
        #endregion
        if (player == null) player = PlayerCtrlManager.m.curPlayer;
        #region [게으름]
        if (Random.Range(0, 100f) < 10)
        {
            algLog += "귀찮아서 행동하지 않았습니다.\n";
            Debug.Log(algLog);
            return;
        }
        #endregion

        #region [공격 확인]
        stTilePos checkPos = player.pPos - ownPos;
        List<stTilePos> areaPoses = skills[(int)MS_Slime.MAS_1008_Slime_Attack].attackArea;
        if (isRight)
        {
            algLog += "→";
            #region [일반 공격]
            areaPoses = skills[(int)MS_Slime.MAS_1008_Slime_Attack].attackArea;
            algLog += "일반 공격";
            if (Random.Range(0, 100f) < 90) // 일반공격 사용 확률 90%
            {
                algLog += "(성공) 범위";
                if (InArea_isR(checkPos, areaPoses))
                {
                    // 사용 가능하면 밀려듦 쿨체크
                    algLog += "(성공) 밀려듦?";
                    if (Random.Range(0, 100f) < 70) // 밀려듦 사용할 확률 70%
                    {
                        algLog += "(성공) 쿨체크";
                        if (CanUseSkill(skills[(int)MS_Slime.MAS_1010_Slime_Rush]))
                        {
                            algLog += "(성공) 발동\n";
                            Rush(areaPoses);
                            skills[(int)MS_Slime.MAS_1010_Slime_Rush].lastUse = Time.time;
                            Debug.Log(algLog);
                            return;
                        }
                    }
                    algLog += "(실패) 일반 공격 발동\n";
                    NormalAttack(areaPoses[0]);
                    Debug.Log(algLog);
                    return;
                }
                else if (InArea_isL(checkPos, areaPoses))
                {
                    // 사용 가능하면 밀려듦 쿨체크
                    algLog += "(반대임) 고개돌리기";
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
            #region [일반 공격]
            areaPoses = skills[(int)MS_Slime.MAS_1008_Slime_Attack].attackArea;
            algLog += "일반 공격";
            if (Random.Range(0, 100f) < 90) // 일반공격 사용 확률 90%
            {
                algLog += "(성공) 범위";
                if (InArea_isL(checkPos, areaPoses))
                {
                    // 사용 가능하면 밀려듦 쿨체크
                    algLog += "(성공) 밀려듦?";
                    if (Random.Range(0, 100f) < 70) // 밀려듦 사용할 확률 70%
                    {
                        algLog += "(성공) 쿨체크";
                        if (CanUseSkill(skills[(int)MS_Slime.MAS_1010_Slime_Rush]))
                        {
                            algLog += "(성공) 발동\n";
                            skills[(int)MS_Slime.MAS_1010_Slime_Rush].lastUse = Time.time;
                            Rush(areaPoses);
                            Debug.Log(algLog);
                            return;
                        }
                    }
                    algLog += "(실패) 일반 공격 발동\n";
                    NormalAttack(areaPoses[0]);
                    Debug.Log(algLog);
                    return;
                }
                else if (InArea_isR(checkPos, areaPoses))
                {
                    algLog += "(반대임) 고개돌리기";
                    timer = mvp_delay * 0.5f; // 행동력 절반만 소모해서
                    SeeRight(); // 왼쪽 바라봄
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

        algLog += "미끄러짐";
        // 미끄러짐! 사용 가능인지 체크
        if (Random.Range(0, 100f) < 60f) // 사용할 확률 60%
        {
            algLog += "(성공) 쿨체크";
            if (CanUseSkill(skills[(int)MS_Slime.MAS_1009_Slime_Slip]))
            {
                algLog += "(성공) 발동";
                Debug.Log(algLog);
                Slip(dir);
                return;
            }
        }
        algLog += "(실패)\n";

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
    void Slip(eMoveDir dir)
    {
        if (!int.TryParse(skills[(int)MS_Slime.MAS_1009_Slime_Slip].values[0], out int maxMove))
            Debug.LogError(string.Format("M_Slime Error! Skill:Slip"));

        string algLog = string.Empty;
        algLog += "판단 : ";
        bool smart = Random.Range(0, 100f) < 70f;
        #region [이동 확인]
        if (smart)
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
                        else
                        {
                            algLog += "(성공) 미끄러짐";
                            if (!MonsterMove(dir))
                            {
                                algLog += "(실패), 쿨타임 반환받음";
                                Debug.Log(algLog);
                                return;
                            }
                        }
                    }
                    else
                    {
                        algLog += "(성공) 미끄러짐";
                        if (!MonsterMove(tmpDir))
                        {
                            algLog += "(실패), 쿨타임 반환받음";
                            Debug.Log(algLog);
                            return;
                        }
                    }
                }
                else
                {
                    algLog += "(성공) 미끄러짐";
                    if (!MonsterMove(newDir))
                    {
                        algLog += "(실패), 쿨타임 반환받음";
                        Debug.Log(algLog);
                        return;
                    }
                }
            }
            else
            {
                algLog += "(성공) 미끄러짐";
                if (!MonsterMove(dir))
                {
                    algLog += "(실패), 쿨타임 반환받음";
                    Debug.Log(algLog);
                    return;
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
                        else
                        {
                            algLog += "(성공) 미끄러짐";
                            if (!MonsterMove(dir))
                            {
                                algLog += "(실패), 쿨타임 반환받음";
                                Debug.Log(algLog);
                                return;
                            }
                        }
                    }
                    else
                    {
                        algLog += "(성공) 미끄러짐";
                        if (!MonsterMove(lastDir))
                        {
                            algLog += "(실패), 쿨타임 반환받음";
                            Debug.Log(algLog);
                            return;
                        }
                    }
                }
                else
                {
                    algLog += "(성공) 미끄러짐";
                    if (!MonsterMove(tmpDir))
                    {
                        algLog += "(실패), 쿨타임 반환받음";
                        Debug.Log(algLog);
                        return;
                    }
                }
            }
            else
            {
                algLog += "(성공) 미끄러짐";
                if (!MonsterMove(newDir))
                {
                    algLog += "(실패), 쿨타임 반환받음";
                    Debug.Log(algLog);
                    return;
                }
            }
        }
        algLog += "(성공)\n";
        skills[(int)MS_Slime.MAS_1009_Slime_Slip].lastUse = Time.time;
        Debug.Log(algLog);
        #endregion
    }
    void Rush(List<stTilePos> areas)
    {
        stTilePos area = areas[0];
        float dmgMul = 1;
        if (!float.TryParse(skills[(int)MS_Slime.MAS_1010_Slime_Rush].values[0], out float mul))
            Debug.LogError(string.Format("M_Slime Error! Skill:Rush"));
        if (isRight)
        {
            stTilePos targetArea = ownPos + area;
            stTilePos rushArea = targetArea + area;
            if (targetArea.row < 0 || targetArea.row >= FieldManager.m.curMaxRow
                || targetArea.col < 0 || targetArea.col >= FieldManager.m.curMaxCol)
            {
                // 공격 못함, 반환
                return;
            }
            // 공격 가능이면 내려와서 체크
            if (rushArea.row < 0 || rushArea.row >= FieldManager.m.curMaxRow
                || rushArea.col < 0 || rushArea.col >= FieldManager.m.curMaxCol
                || FieldManager.m.TileWhere(rushArea).isUnitOn)
            {
                // 공격은 되는데 밀지는 못함
                // 1.2배의 대미지
                dmgMul = dmgMul * mul;
                FieldManager.m.TileWhere(targetArea).MonsterAttackOn(atk * dmgMul, acc);
            }
            else // 한칸 밀어내고 공격함
            {
                PlayerCtrlManager.m.KnockBackByMonsterSkill(rushArea);
                FieldManager.m.TileWhere(ownPos).UnitOff(); // 이거 PosSet 안으로 들어가면 참좋겠는디
                PosSet(targetArea);
                FieldManager.m.TileWhere(rushArea).MonsterAttackOn(atk * dmgMul, acc);
            }
        }
        else
        {
            stTilePos targetArea = ownPos - area;
            stTilePos rushArea = targetArea - area;
            if (targetArea.row < 0 || targetArea.row >= FieldManager.m.curMaxRow
                || targetArea.col < 0 || targetArea.col >= FieldManager.m.curMaxCol)
            {
                // 공격 못함, 반환
                return;
            }
            // 공격 가능이면 내려와서 체크
            if (rushArea.row < 0 || rushArea.row >= FieldManager.m.curMaxRow
                || rushArea.col < 0 || rushArea.col >= FieldManager.m.curMaxCol
                || FieldManager.m.TileWhere(rushArea).isUnitOn)
            {
                // 공격은 되는데 밀지는 못함
                // 1.2배의 대미지
                dmgMul = dmgMul * mul;
                FieldManager.m.TileWhere(targetArea).MonsterAttackOn(atk * dmgMul, acc);
            }
            else // 한칸 밀어내고 공격함
            {
                PlayerCtrlManager.m.KnockBackByMonsterSkill(rushArea);
                FieldManager.m.TileWhere(ownPos).UnitOff(); // 이거 PosSet 안으로 들어가면 참좋겠는디
                PosSet(targetArea);
                FieldManager.m.TileWhere(rushArea).MonsterAttackOn(atk * dmgMul, acc);
            }

        }
        

        
    }
    void Slime_Passive(float val)
    {
        // 기믹으로 들어가있음
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
        
        // 슬라임
        skills.Add(SkillManager.m.mSkillDict[(int)eMSkillCode.MPS_1007_Slime_Passive]);
        skills.Add(SkillManager.m.mSkillDict[(int)eMSkillCode.MAS_1008_Slime_Attack]);
        skills.Add(SkillManager.m.mSkillDict[(int)eMSkillCode.MAS_1009_Slime_Slip]);
        skills.Add(SkillManager.m.mSkillDict[(int)eMSkillCode.MAS_1010_Slime_Rush]);

        foreach (MonsterSkill ms in skills)
            if (ms.rechargeTime > 0)
                ms.lastUse = Time.time;

    }
}
