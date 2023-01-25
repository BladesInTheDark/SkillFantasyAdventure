using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Def;


// 플레이어가 버프 스킬을 사용할 때 생성되는 콜라이더에 들어가는 클래스
// 현재는 관련 기능 부재로 실제로 사용하지 않음
public class BuffCollider : MonoBehaviour
{
    [SerializeField] SpriteRenderer spr;
    [SerializeField] BoxCollider2D col;
    eUSkillCode ownSkill;
    float val;
    stTilePos plusPos;

    public void InitSet(Transform tf, eUSkillCode sc, float v, int rowIdx, int colIdx)
    {
        if (tf == null)
        {
            spr.enabled = false;
            col.enabled = false;
        }
        else
        {
            spr.enabled = true;
            col.enabled = true;
            transform.position = tf.position;
        }
        ownSkill = sc;
        val = v;
        plusPos = new stTilePos(rowIdx, colIdx);
    }
    public void Moved(stTilePos pPos)
    {
        Tile t = FieldManager.m.TileWhere(pPos + plusPos);
        if (t == null)
        {
            spr.enabled = false;
            col.enabled = false;
        }
        else
        {
            spr.enabled = true;
            col.enabled = true;
            transform.position = t.transform.position;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (ownSkill)
        {
            case eUSkillCode.UPS_1006_Aura_Attack_Down:
                Monster m1006 = collision.GetComponent<Monster>();
                m1006.atk -= val;
                break;
            case eUSkillCode.UPS_1007_Aura_Acc_Down:
                Monster m1007 = collision.GetComponent<Monster>();
                m1007.acc -= val;
                break;
        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        switch (ownSkill)
        {
            case eUSkillCode.UPS_1006_Aura_Attack_Down:
                Monster m1006 = collision.GetComponent<Monster>();
                m1006.atk += val;
                break;
            case eUSkillCode.UPS_1007_Aura_Acc_Down:
                Monster m1007 = collision.GetComponent<Monster>();
                m1007.acc += val;
                break;
        }
    }
}
