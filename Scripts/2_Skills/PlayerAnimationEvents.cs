using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Def;


// 원활한 애니메이션 판정 싱크를 위해 만든 클래스
public class PlayerAnimationEvents : MonoBehaviour
{
    [SerializeField] Player p;
    [SerializeField] public Animator ownAnim;
    [System.NonSerialized] public eUSkillCode curCode4;
    [System.NonSerialized] public int tmpI0, tmpI1, tmpI2, tmpI3, tmpI4, tmpI5;
    [System.NonSerialized] public float tmpF0, tmpF1, tmpF2, tmpF3, tmpF4, tmpF5;
    bool nowAttack = false;
    bool nowSkill = false;

    // 값을... 이렇게 깡으로 전달해줘도 되나 싶긴 한데......
    // 돌아가긴 함
    public void AttackAnimCall(float mul = 1)
    {
        if (nowAttack) return;
        nowAttack = true;
        tmpF0 = mul;
        ownAnim.SetBool("Attack", true);
    }
    public void AttackAnimEnd()
    {
        ownAnim.SetBool("Attack", false);
        p.Attack(tmpF0);
    }
    public void CanMove()
    {
        nowAttack = false;
        nowSkill = false;
        if (IngameMainCV.m != null)
            IngameMainCV.m.attackButton.interactable = true;
    }
    public void SkillAnimCall(eUSkillCode code4, int idx, params float[] val)
    {
        nowSkill = true;
        curCode4 = code4;
        switch (curCode4)
        {
            case eUSkillCode.UAS_1012_OneInchPunch:
                tmpI0 = idx; tmpF0 = val[0];
                break;
            case eUSkillCode.UAS_1013_SpinArround:
                tmpI0 = idx; tmpF0 = val[0];
                break;
        }
        ownAnim.SetBool("Skill", true);
    }

    public void SkillAnimEnd()
    {
        ownAnim.SetBool("Skill", false);
        switch (curCode4)
        {
            case eUSkillCode.UAS_1012_OneInchPunch:
                p.OneInchPunch(tmpI0, tmpF0);
                break;
            case eUSkillCode.UAS_1013_SpinArround:
                p.SpinArround(tmpI0, tmpF0);
                break;
        }
    }
}
