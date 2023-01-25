using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Def;

// 인게임에서 필드 타일 수만큼 존재하는 클래스
public class Tile : MonoBehaviour
{
    [SerializeField] SpriteRenderer sp;
    [SerializeField] SpriteRenderer playerAttackArea;
    [SerializeField] SpriteRenderer monsterAttackArea;
    public eTileType type = eTileType.Blank;
    public bool _isUnitHere = false;
    public stTilePos ownPos;
    public Monster curM;
    public Player curP;

    public eTileType t
    {
        get { return type; }
    }
    public bool isUnitOn
    {
        get { return _isUnitHere; }
    }

    public void InitSet(eTileType tileType, Vector2 pos, int row, int col)
    {
        type = tileType;
        transform.localPosition = pos;
        sp.sprite = Res.m.GetTileSprite(type);
        ownPos = new stTilePos(row, col);
        playerAttackArea.enabled = false;
        monsterAttackArea.enabled = false;
    }
    public void UnitOn(Player p)
    {
        _isUnitHere = true;
        curP = p;
    }
    public void UnitOn(Monster m)
    {
        _isUnitHere = true;
        curM = m;
    }
    public void UnitOff()
    {
        _isUnitHere = false;
        if (curM != null) curM = null;
        if (curP != null) curP = null;
    }
    public void PlayerAttackOn(float dmg, float ac)
    {
        playerAttackArea.enabled = true;
        StartCoroutine(PlayerAttackOff());
        if (curM != null)
            curM.Damaged(dmg, ac);
    }
    IEnumerator PlayerAttackOff()
    {
        yield return new WaitForSeconds(0.5f);
        playerAttackArea.enabled = false;
    }
    public void MonsterAttackOn(float dmg, float ac)
    {
        monsterAttackArea.enabled = true;
        StartCoroutine(MonsterAttackOff());
        if (curP != null)
            curP.Damaged(dmg, ac);
    }
    IEnumerator MonsterAttackOff()
    {
        yield return new WaitForSeconds(0.5f);
        monsterAttackArea.enabled = false;
    }
}
