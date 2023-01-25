using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Def;
using UnityEngine.UI;

// 몬스터들은 해당 클래스를 상속받음. 스탯은 이쪽에서 관리 가능.
public class Monster : MonoBehaviour
{
    public eMonster type;

    public float atk;
    public float dfn;
    public float acc;
    public float avo;
    public float crt;
    public float crt_dmg;
    public float nowHP;
    public float hp;
    public byte mvp;
    public float mvp_delay;
    public float[] algorithms;

    public bool isStart = false;
    public bool isDead = false;
    public bool isRight = false;
    protected float timer;
    public stTilePos ownPos;
    public int ownIndex;
    public List<MonsterSkill> skills;

    public Player player;

    SpriteRenderer spr;
    Canvas cv;
    Slider hpBar;
    Slider mvpBar;
    public DamagePrintSystem dmgPrint;
    public ParticleCtrlSystem particles;
    Vector3 compactPos = new Vector3(0, 0.5f, 0);

    public Monster(stStatus st)
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

    protected void SpriteSet()
    {
        transform.localScale = new Vector3(5, 5, 1);
        transform.localPosition = new Vector3(0, 0.5f, 0);
        spr = transform.GetComponent<SpriteRenderer>();
        spr.sprite = Res.m.GetMonsterSprite(type);
        cv = transform.GetChild(0).GetComponent<Canvas>();
        hpBar = cv.transform.GetChild(0).GetComponent<Slider>();
        mvpBar = cv.transform.GetChild(1).GetComponent<Slider>();
        particles = transform.GetChild(1).GetComponent<ParticleCtrlSystem>();
        dmgPrint = transform.GetChild(2).GetComponent<DamagePrintSystem>();
        HPBarSet();
        MvpBarSet(0);
    }
    public void HPBarSet()
    {
        if (nowHP <= 0) hpBar.value = 0;
        else hpBar.value = nowHP / hp;
    }
    public void MvpBarSet(float p)
    {
        mvpBar.value = p;
    }
    public void SeeRight()
    {
        isRight = true;
        transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        hpBar.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
        mvpBar.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
        dmgPrint.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
    }
    public void SeeLeft()
    {
        isRight = false;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        hpBar.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        mvpBar.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        dmgPrint.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
    }
    public void Damaged(float dmg, float ac)
    {
        float avoid = avo - ac;
        float canAvoid = Random.Range(0, 100f);
        if (canAvoid < avoid)
        {
            // 회피
            dmgPrint.PrintDamage("Avoid!", Color.white);
            return;
        }

        float finalDmg = dmg - dfn;
        float criticalRate = Random.Range(0, 100f); // 이게 정해지는 확률.
        float playerCrt = player.crt;
        // criticalRate보다 플레이어의 crt가 더 높으면 크리티컬 대미지임 
        switch (type)
        {
            case eMonster.Slime:
                if (float.TryParse(SkillManager.m.mSkillDict[(int)eMSkillCode.MPS_1007_Slime_Passive].values[0], out float addCrt))
                    playerCrt += addCrt + 40;
                else
                    Debug.LogError("Error! Monster : OnSlimeCritical");
                Debug.LogFormat("CRI : {0}", playerCrt);
                if (criticalRate < playerCrt)
                {
                    Debug.LogFormat("슬라임에게 크리티컬 발생! 즉사합니다!");
                    Dead();
                    return;
                }
                break;
            default:
                if (criticalRate < playerCrt) finalDmg = finalDmg * (1 + player.crt_dmg / 100);
                break;
        }
        if (finalDmg < 0) finalDmg = 0;
        int intDmg = (int)Mathf.Round(finalDmg);
        nowHP -= intDmg;
        dmgPrint.PrintDamage(intDmg.ToString(), Color.red);
        HPBarSet();
        if (nowHP <= 0)
        {
            isDead = true;
            Dead();
        }
    }
    public void Dead()
    {
        spr.enabled = false;
        cv.enabled = false;
        isStart = false;
        FieldManager.m.TileWhere(ownPos).UnitOff();
        FieldManager.m.DeleteMonster(this);
        particles.transform.SetParent(FieldManager.m.transform);
        Destroy(this.gameObject, 1f);
    }
    public void PosSet(stTilePos spp)
    {
        Tile targetTile = FieldManager.m.TileWhere(spp);
        if (targetTile == null) return;
        ownPos = spp;
        transform.SetParent(targetTile.transform);
        transform.position = targetTile.transform.position;
        transform.localPosition = new Vector3(0, 0.5f, 0);
        targetTile.UnitOn(this);
    }
    public bool MonsterMove(eMoveDir emd)
    {
        switch (emd)
        {
            case eMoveDir.Up:
                int targetRowU = ownPos.row - 1;
                if (targetRowU >= 0)
                {
                    if (!FieldManager.m.curTiles[targetRowU, ownPos.col].isUnitOn)
                    {
                        // 행동력 있음, 이동 처리
                        FieldManager.m.TileWhere(ownPos).UnitOff();
                        ownPos.row = targetRowU;
                        PosSet(ownPos);
                        return true;
                    }
                }
                return false;
            case eMoveDir.Down:
                int targetRowD = ownPos.row + 1;
                if (targetRowD < FieldManager.m.curMaxRow)
                {

                    if (!FieldManager.m.curTiles[targetRowD, ownPos.col].isUnitOn)
                    {
                        // 행동력 있음, 이동 처리
                        FieldManager.m.TileWhere(ownPos).UnitOff();
                        ownPos.row = targetRowD;
                        PosSet(ownPos);
                        return true;
                    }
                }
                return false;
            case eMoveDir.Left:
                int targetColL = ownPos.col - 1;
                if (targetColL >= 0)
                {

                    if (!FieldManager.m.curTiles[ownPos.row, targetColL].isUnitOn)
                    {
                        // 행동력 있음, 이동 처리
                        FieldManager.m.TileWhere(ownPos).UnitOff();
                        ownPos.col = targetColL;
                        PosSet(ownPos);
                        SeeLeft();
                        return true;
                    }

                }
                return false;
            case eMoveDir.Right:
                int targetColR = ownPos.col + 1;
                if (targetColR < FieldManager.m.curMaxCol)
                {

                    if (!FieldManager.m.curTiles[ownPos.row, targetColR].isUnitOn)
                    {
                        // 행동력 있음, 이동 처리
                        FieldManager.m.TileWhere(ownPos).UnitOff();
                        ownPos.col = targetColR;
                        PosSet(ownPos);
                        SeeRight();
                        return true;
                    }

                }
                return false;
        }
        return false;
    }
    protected bool InArea_isR(stTilePos checkPos, List<stTilePos> atkArea)
    {
        // 오른쪽으로 공격하면 맞는가?
        foreach (stTilePos atkA in atkArea)
            if (checkPos == atkA)
                return true;
        return false;
    }
    protected bool InArea_isL(stTilePos checkPos, List<stTilePos> atkArea)
    {
        // 왼쪽으록 공격하면 맞는가?
        foreach (stTilePos atkA in atkArea)
            if (checkPos == -atkA)
                return true;
        return false;
    }
    protected bool CanUseSkill(MonsterSkill ms)
    {
        if (Time.time - ms.lastUse > ms.rechargeTime)
            return true;
        return false;
    }
    public void DebuffByPlayerSkill(eUSkillCode code4, float sec, float stats)
    {
        StartCoroutine(DebuffOnOff(code4, sec, stats));
        particles.ParticleOn(R_Particle.Debuff, sec);
    }
    public void KnockBackByPlayerSkill(eUSkillCode code4, float rate, int tileCnt)
    {
        string algLog = string.Empty;

        // 지금 여기 들어오는 건 사자후 스킬밖에 없지만 확장성 고려해서 스위치문으로 만듦
        switch (code4)
        {
            case eUSkillCode.UAS_1018_Roaring:
                algLog += "사자후";
                if (Random.Range(0, 100f) < rate)
                {
                    algLog += "(성공) 밀려날방향";
                    int checkDir = ownPos.col - player.pPos.col;
                    if (checkDir >= 0) // 양수면 내(몹)가 오른쪽, 오른쪽으로 이동해야 함(col에 +tileCnt)
                    {
                        algLog += "(오른쪽) ";
                        // 0도 여기에 들어옴!! (플레이어 위아래도 여기라는 뜻)
                        for (int i = 0; i < tileCnt; i ++) // 최대 tileCnt칸 밀려남
                        {
                            stTilePos targetPos = ownPos + new stTilePos(0, 1);
                            if (targetPos.col < FieldManager.m.curMaxCol && !FieldManager.m.TileWhere(targetPos).isUnitOn) // 이동할 곳이 존재하는 타일이면
                            {
                                algLog += "밀려남+1 ";
                                FieldManager.m.TileWhere(ownPos).UnitOff();
                                ownPos = targetPos;
                                PosSet(ownPos);
                                SeeRight();
                            }
                            else
                            {
                                algLog += "밀려날 수 없음! (행동력 초기화)";
                                particles.ParticleOn(R_Particle.Stuned, 1f);
                                timer = 0;
                                break;
                            }
                        }
                    }
                    else // 음수면 내(몹)가 왼쪽, 왼쪽으로 이동해야 함
                    {
                        algLog += "(왼쪽) ";
                        for (int i = 0; i < tileCnt; i++)
                        {
                            stTilePos targetPos = ownPos - new stTilePos(0, 1);
                            if (targetPos.col < FieldManager.m.curMaxCol && !FieldManager.m.TileWhere(targetPos).isUnitOn) // 이동할 곳이 존재하는 타일이면
                            {
                                algLog += "밀려남+1 ";
                                FieldManager.m.TileWhere(ownPos).UnitOff();
                                ownPos = targetPos;
                                PosSet(ownPos);
                                SeeLeft();
                            }
                            else
                            {
                                algLog += "밀려날 수 없음! (행동력 초기화)";
                                timer = 0;
                                particles.ParticleOn(R_Particle.Stuned, 1f);
                                break;
                            }
                        }
                    }
                }
                algLog += "(실패)\n";
                Debug.Log(algLog);
                break;
        }
    }
    IEnumerator DebuffOnOff(eUSkillCode code4, float sec, float stats)
    {
        switch (code4)
        {
            case eUSkillCode.UAS_1017_Curse:
                atk -= stats;
                dfn -= stats;
                acc -= stats;
                avo -= stats;
                crt -= stats;
                crt_dmg -= stats;
                break;
        }
        yield return new WaitForSeconds(sec);
        switch (code4)
        {
            case eUSkillCode.UAS_1017_Curse:
                atk += stats;
                dfn += stats;
                acc += stats;
                avo += stats;
                crt += stats;
                crt_dmg += stats;
                break;
        }
    }
}
