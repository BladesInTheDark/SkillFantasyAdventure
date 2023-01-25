using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Def;

// 구현되지 않은 몬스터(구현된 몬스터 스크립트를 복사해둔 것)
public class M_Hyena : Monster
{
    public const eMonster ownType = eMonster.Hyena;

    public M_Hyena(stStatus st) : base(st)
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
                Debug.Log("====================================");
                timer = 0;
                MvpBarSet(0);
            }
        }
    }
    void AlgAction()
    {
        #region [알고리즘 설명]
        /*
         * 박쥐 알고리즘 순서 
         * (0) 게으른가? -> 5%
         * (1) 일반 공격을 하면 맞는가?
         * -> YES : (2)로
         * ->  NO : 이동한다
         * (2) 강화 공격이 쿨타임인가? 
         * -> YES : 확률로 일반 공격 발동, 일반 공격 실패시 이동.
         * ->  NO : 확률로 강화 공격 발동, 강화 공격 실패시 일반 공격 발동, 일반 공격까지 실패시 이동.
         */
        #endregion
        if (player == null) player = PlayerCtrlManager.m.curPlayer;
        #region [멍충멍충]
        if (Random.Range(0, 100f) < 15)
        {
            Debug.Log("멍청해서 행동하지 않았습니다.");
            return;
        }
        #endregion

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

        skills.Add(SkillManager.m.mSkillDict[(int)eMSkillCode.MPS_1016_Hyena_Passive]);
        skills.Add(SkillManager.m.mSkillDict[(int)eMSkillCode.MAS_1017_Hyena_Attack]);
        skills.Add(SkillManager.m.mSkillDict[(int)eMSkillCode.MAS_1018_Hyena_Unite]);
        skills.Add(SkillManager.m.mSkillDict[(int)eMSkillCode.MAS_1019_Hyena_Crisis]);

        foreach (MonsterSkill ms in skills)
            if (ms.rechargeTime > 0)
                ms.lastUse = Time.time;
    }
}
