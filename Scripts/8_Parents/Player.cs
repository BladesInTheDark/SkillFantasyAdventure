using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Def;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] SpriteRenderer playerImg;
    [SerializeField] Animator ownAnim;
    [SerializeField] ParticleCtrlSystem particles;
    [SerializeField] DamagePrintSystem dmgPrint;

    public float atk;
    float atkMul = 1;
    Queue<float> dmgBuffQ = new Queue<float>();
    public float dfn;
    int atkImmune;
    public float acc;
    public float avo;
    public float crt;
    public float crt_dmg;
    public float nowHP;
    public float maxHP;
    public byte mvp;
    public byte startMvp = 0;
    public byte mvp_max;
    public float mvp_delay;
    public int lck;

    public UserSkill[] usePSkills = new UserSkill[3];
    public UserSkill[] useASkills = new UserSkill[3];

    public stTilePos pPos;
    Slider hpBar;
    List<stTilePos> attackArea = new List<stTilePos>(); // 본인 기준 범위
    public List<stTilePos>[] skillAreas = new List<stTilePos>[3];
    List<List<AuraCollider>> auraArea = new List<List<AuraCollider>>();
    List<BuffCollider> buffCols = new List<BuffCollider>();
    [System.NonSerialized] public bool isStart = false;
    [System.NonSerialized] public bool isRight = true; // 플레이어가 현재 향한 방향
    [SerializeField] public PlayerAnimationEvents aniEvents;
    bool isDead = false;
    
    float timer;

    private void Update()
    {
        if (isDead) return;
        if (isStart)
        {
            if (mvp == mvp_max)
            {
                IngameMainCV.m.MvpMaskSet(1);
                return;
            } 
            else
            {
                timer += Time.deltaTime;
                IngameMainCV.m.MvpMaskSet(timer / mvp_delay);
                if (timer > mvp_delay)
                {
                    timer = 0;
                    mvp++;
                    IngameMainCV.m.MvpTextSet(mvp);
                }
            }
        }
    }

    public void InitSet(stTilePos spp)
    {
        PlayerCtrlManager.m.InitSet(this);
        PosSet(spp, false);
        CopyInfo();
        //playerImg.sprite = Res.m.GetPlayerIconSprite(UserInfo.m.iconIdx);
        attackArea.Add(new stTilePos(0, 1));
        foreach (Monster m in FieldManager.m.curMonsters)
        {
            m.player = this;
        }
    }
    public void PosSet(stTilePos spp, bool mvpDelete = true)
    {
       
        Tile targetTile = FieldManager.m.TileWhere(spp);
        if (targetTile == null) return;
        pPos = spp;
        if (mvpDelete)
        {
            UseMvp();
            StartCoroutine(MoveAnim());
            AudioManager.m.PlaySE(R_SE.PlayerMove);
        }
        AuraMoved();
        transform.SetParent(targetTile.transform);
        transform.position = targetTile.transform.position;
        targetTile.UnitOn(this);
    }
    public void SkillAnimationPlay()
    {
        ownAnim.SetFloat("SkillState", 1);
    }
    IEnumerator MoveAnim()
    {
        ownAnim.SetBool("Run", true);
        yield return new WaitForSeconds(0.5f);
        ownAnim.SetBool("Run", false);
    }
    public void SeeRight()
    {
        isRight = true;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        dmgPrint.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
    }
    public void SeeLeft()
    {
        isRight = false;
        transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        dmgPrint.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
    }
    public void AttackAnimPlay()
    {
        if (mvp <= 0)
        {
            AudioManager.m.PlaySE(R_SE.PlayerCantMove);
            IngameMainCV.m.attackButton.interactable = true;
            return;
        }
        UseMvp();
        aniEvents.AttackAnimCall();
    }
    public void Attack(float mul = 1)
    {
        if (dmgBuffQ.Count > 0)
        {
            mul = mul * dmgBuffQ.Dequeue();
            if (dmgBuffQ.Count <= 0)
            {
                particles.ParticleOff(R_Particle.AttackBuff);
            }
        }
        if (isRight)
        {
            foreach (stTilePos stp in attackArea)
            {
                int targetRow = pPos.row + stp.row;
                int targetCol = pPos.col + stp.col;
                if (targetRow < 0 || targetRow >= FieldManager.m.curMaxRow)
                    continue; // 공격할 수 없음
                if (targetCol < 0 || targetCol >= FieldManager.m.curMaxCol)
                    continue;
                Tile tTile = FieldManager.m.curTiles[targetRow, targetCol];
                if (tTile.curM != null) tTile.curM.particles.ParticleOn(R_Particle.Punched);
                AudioManager.m.PlaySE(R_SE.PlayerAttack);
                tTile.PlayerAttackOn(atk * atkMul * mul, acc);
            }
        }
        else
        {
            foreach (stTilePos stp in attackArea)
            {
                int targetRow = pPos.row + stp.row;
                int targetCol = pPos.col - stp.col;
                if (targetRow < 0 || targetRow >= FieldManager.m.curMaxRow)
                    continue; // 공격할 수 없음
                if (targetCol < 0 || targetCol >= FieldManager.m.curMaxCol)
                    continue;
                Tile tTile = FieldManager.m.curTiles[targetRow, targetCol];
                if (tTile.curM != null) tTile.curM.particles.ParticleOn(R_Particle.Punched);
                AudioManager.m.PlaySE(R_SE.PlayerAttack);
                tTile.PlayerAttackOn(atk * atkMul * mul, acc);
                
            }
        }
        
        IngameMainCV.m.nowProcessing = false;
    }
    #region [스킬]
    public void OneInchPunchAnimCall(int idx, float mul)
    {
        if (mvp <= 0)
        {
            AudioManager.m.PlaySE(R_SE.PlayerCantMove);
            IngameMainCV.m.aSkill[idx].CoolTimeReset();
            IngameMainCV.m.nowProcessing = false;
            return;
        }
        UseMvp();
        aniEvents.SkillAnimCall(eUSkillCode.UAS_1012_OneInchPunch, idx, mul);
    }
    public void OneInchPunch(int idx, float mul)
    {
        if (dmgBuffQ.Count > 0)
        {
            mul = mul * dmgBuffQ.Dequeue();
            if (dmgBuffQ.Count <= 0)
            {
                particles.ParticleOff(R_Particle.AttackBuff);
            }
        }
        if (isRight)
        {
            foreach (stTilePos stp in PlayerCtrlManager.m.aSkills[idx].attackArea)
            {
                int targetRow = pPos.row + stp.row;
                int targetCol = pPos.col + stp.col;
                if (targetRow < 0 || targetRow >= FieldManager.m.curMaxRow)
                    continue; // 공격할 수 없음
                if (targetCol < 0 || targetCol >= FieldManager.m.curMaxCol)
                    continue;
                Tile tTile = FieldManager.m.curTiles[targetRow, targetCol];
                if (tTile.curM != null) tTile.curM.particles.ParticleOn(R_Particle.Punched);
                AudioManager.m.PlaySE(R_SE.OneInchPunch);
                tTile.PlayerAttackOn(atk * mul, acc);
                
            }
        }
        else
        {
            foreach (stTilePos stp in PlayerCtrlManager.m.aSkills[idx].attackArea)
            {
                int targetRow = pPos.row + stp.row;
                int targetCol = pPos.col - stp.col;
                if (targetRow < 0 || targetRow >= FieldManager.m.curMaxRow)
                    continue; // 공격할 수 없음
                if (targetCol < 0 || targetCol >= FieldManager.m.curMaxCol)
                    continue;
                Tile tTile = FieldManager.m.curTiles[targetRow, targetCol];
                if (tTile.curM != null) tTile.curM.particles.ParticleOn(R_Particle.Punched);
                AudioManager.m.PlaySE(R_SE.OneInchPunch);
                tTile.PlayerAttackOn(atk * mul, acc);
            }
        }
        IngameMainCV.m.nowProcessing = false;
    }
    public void SpinArroundAnimCall(int idx, float dmg)
    {
        // 고정대미지
        if (mvp <= 0)
        {
            AudioManager.m.PlaySE(R_SE.PlayerCantMove);
            IngameMainCV.m.aSkill[idx].CoolTimeReset();
            IngameMainCV.m.nowProcessing = false;
            return;
        }
        UseMvp();
        aniEvents.SkillAnimCall(eUSkillCode.UAS_1013_SpinArround, idx, dmg);
    }
    public void SpinArround(int idx, float dmg)
    {
        
        if (dmgBuffQ.Count > 0)
        {
            dmg = dmg * dmgBuffQ.Dequeue();
            if (dmgBuffQ.Count <= 0)
            {
                particles.ParticleOff(R_Particle.AttackBuff);
            }
        }
        foreach (stTilePos stp in PlayerCtrlManager.m.aSkills[idx].attackArea)
        {
            int targetRow = pPos.row + stp.row;
            int targetCol = pPos.col + stp.col;
            if (targetRow < 0 || targetRow >= FieldManager.m.curMaxRow)
                continue; // 공격할 수 없음
            if (targetCol < 0 || targetCol >= FieldManager.m.curMaxCol)
                continue;
            Tile tTile = FieldManager.m.curTiles[targetRow, targetCol];
            if (tTile.curM != null) tTile.curM.particles.ParticleOn(R_Particle.Sworded);
            AudioManager.m.PlaySE(R_SE.SpinArround);
            tTile.PlayerAttackOn(dmg, acc);
        }
        IngameMainCV.m.nowProcessing = false;
    }
    public void HardSkin(int idx, float sec, float dfnAdd)
    {
        UseMvp();
        particles.ParticleOn(R_Particle.DeffenceBuff, sec);
        AudioManager.m.PlaySE(R_SE.PlayerBuffed);
        StartCoroutine(BuffOnOff(eUSkillCode.UAS_1014_HardSkin, sec, dfnAdd));
    }
    public void ProtectionHand(int idx, float sec, int cnt)
    {
        UseMvp();
        particles.ParticleOn(R_Particle.Shield, sec);
        AudioManager.m.PlaySE(R_SE.PlayerBuffed);
        StartCoroutine(BuffOnOff(eUSkillCode.UAS_1015_ProtectionHand, sec, cnt));
    }
    #endregion
    public void UseMvp()
    {
        mvp--;
        IngameMainCV.m.MvpTextSet(mvp);
    }
    public void FieldChange(stTilePos spp)
    {
        // 필드를 클리어하고 넘어갈 때
        // hp 그대로, pos 초기화, mvp도 초기화
        mvp = startMvp;
        MvpTextSet();
    }
    public void CopyInfo()
    {
        UserInfo u = UserInfo.m;
        atk = u.atk;
        dfn = u.dfn;
        acc = u.acc;
        avo = u.avo;
        crt = u.crt;
        crt_dmg = u.crt_dmg;
        maxHP = u.hp;
        nowHP = maxHP;
        mvp = u.mvp;
        mvp_max = u.mvp_max;
        mvp_delay = u.mvp_delay;
        lck = u.lck;
    }
    public void Damaged(float dmg, float ac)
    {
        
        float avoid = avo - ac;
        float canAvoid = Random.Range(0, 100f);
        if (canAvoid < avoid)
        {
            // 회피
            Debug.Log("플레이어가 공격을 회피했습니다.");
            dmgPrint.PrintDamage("Avoid!", Color.white);
            return;
        }
        if (atkImmune > 0)
        {
            Debug.Log("보호막이 데미지를 무시했습니다!");
            dmgPrint.PrintDamage("Shield!", Color.blue);
            atkImmune--;
            if (atkImmune <= 0)
            {
                particles.ParticleOff(R_Particle.Shield);
            }
            return;
        }
        particles.ParticleOn(R_Particle.Punched);
        AudioManager.m.PlaySE(R_SE.PlayerDamaged);
        float finalDmg = dmg - dfn;
        float critical = Random.Range(0, 100f);
        // 크리티컬 계산을 몬스터쪽에서?
        // 아직 유저가 받는 크리티컬 대미지는 구현되지 않음
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
    public void HPBarSet()
    {
        if (hpBar == null) hpBar = IngameMainCV.m.hpBar;
        if (nowHP <= 0) nowHP = 0;
        hpBar.value = nowHP / maxHP;
        IngameMainCV.m.hpText.text = string.Format("{0} / {1}", Mathf.Round(nowHP * 100) / 100f, maxHP);
    }
    public void MvpTextSet()
    {
        IngameMainCV.m.MvpTextSet(mvp);
    }
    public void MvpMaxTextSet()
    {
        IngameMainCV.m.MvpMaxTextSet(mvp_max);
    }
    public void AuraSet(List<stTilePos> area, eUSkillCode sc, float value)
    {
        List<AuraCollider> auraList = new List<AuraCollider>();
        for (int i = 0; i < area.Count; i++)
        {
            GameObject go = Instantiate(Res.m.auraColliderObj);
            AuraCollider aura = go.GetComponent<AuraCollider>();
            auraList.Add(aura);
            Tile t = FieldManager.m.TileWhere(pPos + area[i]);
            if (t == null) aura.InitSet(null, sc, value, area[i].row, area[i].col);
            else go.GetComponent<AuraCollider>().InitSet(t.transform, sc, value, area[i].row, area[i].col);
        }
        auraArea.Add(auraList);
    }
    public void AuraMoved()
    {
        for (int i = 0; i < auraArea.Count; i++)
        {
            for (int j = 0; j < auraArea[i].Count; j++)
            {
                auraArea[i][j].Moved(pPos);
            }
        }
    }
    public void NormalAttackAreaChange(List<stTilePos> atkArea, float mul)
    {
        // 현재 1x1 = 플레이어 좌표 + [0, 1]에 쏨
        if (atkArea.Count > attackArea.Count)
        {
            attackArea = atkArea;
            atkMul = mul;
        }
        else
        {
            // 더 좁은 범위는 무시됨
        }
    }
    IEnumerator BuffOnOff(eUSkillCode code4, float sec, float val)
    {
        switch (code4)
        {
            case eUSkillCode.UAS_1014_HardSkin:
                dfn += val;
                break;
            case eUSkillCode.UAS_1015_ProtectionHand:
                atkImmune += (int)val;
                break;
        }
        yield return new WaitForSeconds(sec);
        switch (code4)
        {
            case eUSkillCode.UAS_1014_HardSkin:
                dfn -= val;
                break;
            case eUSkillCode.UAS_1015_ProtectionHand:
                atkImmune -= (int)val;
                if (atkImmune < 0) atkImmune = 0;
                break;
        }
    }
    public void DmgBuffAdd(int cnt, float val)
    {
        UseMvp();
        for (int i = 0; i < cnt; i++)
        {
            dmgBuffQ.Enqueue(1 + val);
        }
        particles.ParticleOn(R_Particle.AttackBuff);
        AudioManager.m.PlaySE(R_SE.PlayerBuffed);
    }
    public void Dead()
    {
        ownAnim.SetTrigger("Die");
        FieldManager.m.GameEnd();
        Destroy(this.gameObject);
    }
}
