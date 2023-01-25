using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Def;

// 필드 생성 및 초기화에 사용하는 함수. 인게임에서만 활동함. 
public class FieldManager : MonoBehaviour
{
    #region [인스턴스화]
    static FieldManager _uniqueInstance;
    public static FieldManager m
    {
        get { return _uniqueInstance; }
    }
    #endregion


    public uint curStageCode = 101; // 기본 1-1부터 시작함
    [System.NonSerialized] public Tile[,] curTiles;
    [System.NonSerialized] public byte curMaxRow;
    [System.NonSerialized] public byte curMaxCol;
    [System.NonSerialized] public List<Monster> curMonsters = new List<Monster>();
    [System.NonSerialized] public const byte maxStage = 103;
    
    Vector3 initPos;
    [System.NonSerialized] public Dictionary<uint, stFieldInfo> fieldInfo = new Dictionary<uint, stFieldInfo>();
    Dictionary<uint, eTileType[,]> fieldTileInfo = new Dictionary<uint, eTileType[,]>();
    Dictionary<uint, stTilePos> playerPosInfo = new Dictionary<uint, stTilePos>();
    [System.NonSerialized] public const float interval = 1.05f;
    bool firstSetEnd = false;
    bool nowPlaying = false;
    float timer = 0;

    private void Awake()
    {
        _uniqueInstance = this;
        initPos = transform.position;
        TableLoader.m.FieldTableLoad();
        #region [Tile Setting]
        CreateTileInfo();
        #endregion
    }

    private void Start()
    {
        CreateTile();
    }
    private void Update()
    {
        if (nowPlaying)
            timer += Time.deltaTime;
    }
    public void InitField()
    {
        if (curMonsters.Count > 0) curMonsters.Clear();
        transform.position = initPos;
        if (PlayerCtrlManager.m.curPlayer != null)
        {
            PlayerCtrlManager.m.curPlayer.transform.SetParent(PlayerCtrlManager.m.transform);
        }    
        Tile[] childList = gameObject.GetComponentsInChildren<Tile>(); 
        if (childList != null)
            for (int i = 0; i < childList.Length; i++)
                Destroy(childList[i].gameObject);
            

        CreateTile();
    }
    public void CreateTile()
    {
        stFieldInfo fi = fieldInfo[curStageCode];
        eTileType[,] tileSet = fieldTileInfo[curStageCode];
        curMaxRow = (byte)fi.row;
        curMaxCol = (byte)fi.col;

        curTiles = new Tile[curMaxRow, curMaxCol];
        float sPosX = (fi.col / 2) * -interval;
        float sPosY = (fi.row / 2) * interval;
        if (fi.col % 2 == 0) sPosX += interval * 0.5f;
        if (fi.row % 2 == 0) sPosY -= interval * 0.5f;
        
        List<Tile> spawnableTiles = new List<Tile>();

        for (int i = 0; i < fi.row; i++)
        {
            for (int j = 0; j < fi.col; j++)
            {
                Vector2 pos = new Vector2(sPosX + (j * interval), sPosY - (i * interval));
                GameObject go = Instantiate(Res.m.tilePrefab, transform);
                Tile tl = go.GetComponent<Tile>();
                curTiles[i, j] = tl;
                tl.InitSet(tileSet[i, j], pos, i, j);
                if (tl.t == eTileType.NSpawn) spawnableTiles.Add(tl);
            }
        }

        SpawnMonsters(fi, spawnableTiles);
        
        if (!firstSetEnd)
        {
            SpawnPlayer(playerPosInfo[curStageCode]);
            IngameMainCV.m.UISetCall();
            firstSetEnd = true;
            StartCoroutine(WaitFirstStart());
        }
        else
        {
            ReusePlayer(playerPosInfo[curStageCode]);
        }
        
    }
    IEnumerator WaitFirstStart()
    {
        yield return new WaitForSeconds(2f);
        GameStart();
    }
    public void SpawnMonsters(stFieldInfo fi, List<Tile> sTiles)
    {
        // 먼저 소환할 몬스터 수 정하기
        int mCnt = Random.Range(fi.min, fi.max + 1);

        for (int i = 0; i < mCnt; i++) // 그만큼 소환하기
        {
            // 소환될 타일 정하기
            int tileIdx = Random.Range(0, sTiles.Count);
            Tile targetTile = sTiles[tileIdx];
            sTiles.RemoveAt(tileIdx);

            // 소환될 몬스터 정하기
            float monRate = Random.Range(0, 100f);
            for (int idx = 0; idx < fi.monsters.Count; idx++)
            {
                if (monRate < fi.rates[idx])
                {
                    eMonster targetMon = fi.monsters[idx];
                    GameObject go = Instantiate(Res.m.monsterPrefab, targetTile.transform);

                    // 소환하기
                    // curMonsters를 GameObject로 해서 관리하기?
                    switch (targetMon)
                    {
                        case eMonster.Goblin:
                            M_Goblin m0 = go.AddComponent<M_Goblin>();
                            m0.InitSet(MonsterManager.m.info[targetMon], targetTile.ownPos);
                            curMonsters.Add(m0);
                            targetTile.UnitOn(m0);
                            break;
                        case eMonster.Bat:
                            M_Bat m1 = go.AddComponent<M_Bat>();
                            m1.InitSet(MonsterManager.m.info[targetMon], targetTile.ownPos);
                            curMonsters.Add(m1);
                            targetTile.UnitOn(m1);
                            break;
                        case eMonster.Slime:
                            M_Slime m2 = go.AddComponent<M_Slime>();
                            m2.InitSet(MonsterManager.m.info[targetMon], targetTile.ownPos);
                            curMonsters.Add(m2);
                            targetTile.UnitOn(m2);
                            break;
                        case eMonster.Wolf:
                            M_Wolf m3 = go.AddComponent<M_Wolf>();
                            m3.InitSet(MonsterManager.m.info[targetMon], targetTile.ownPos);
                            curMonsters.Add(m3);
                            targetTile.UnitOn(m3);
                            break;
                        case eMonster.Hyena:
                            M_Hyena m4 = go.AddComponent<M_Hyena>();
                            m4.InitSet(MonsterManager.m.info[targetMon], targetTile.ownPos);
                            curMonsters.Add(m4);
                            targetTile.UnitOn(m4);
                            break;
                    }
                    break;
                }
            }
        }
    }
    public void SpawnPlayer(stTilePos spp)
    {
        GameObject go = Instantiate(Res.m.playerPrefab, curTiles[spp.row, spp.col].transform);
        Player p = go.GetComponent<Player>();
        p.InitSet(spp);
    }
    public void ReusePlayer(stTilePos spp)
    {
        PlayerCtrlManager.m.curPlayer.PosSet(spp, false);
        PlayerCtrlManager.m.curPlayer.SeeRight();
    }
    public void DeleteMonster(Monster mon)
    {
        int targetIdx = 0;
        for (int i = 0; i < curMonsters.Count; i++)
        {
            if (curMonsters[i] == mon)
            {
                targetIdx = i;
                break;
            }
        }
        curMonsters.RemoveAt(targetIdx);
        if (curMonsters.Count == 0)
        {
            PauseGame();
            // 원래 창 띄우고 결과 낸 다음 확인 눌렀을 때 NextStageCall 해야 함
            // 이건 임시
            if (curStageCode == maxStage)
            {
                GameEnd();
            }
            else
            {
                IngameMainCV.m.NextStageCall();
            }
        }
    }
    public void FieldCodePlus()
    {
        curStageCode++;
        if (curStageCode % 100 > 10)
            curStageCode = (curStageCode / 100) * 100 + 100;
    }
    public void GameStart()
    {
        foreach (Monster m in curMonsters)
            m.isStart = true;
        PlayerCtrlManager.m.curPlayer.isStart = true;
        for (int i = 0; i < 3; i++)
            IngameMainCV.m.aSkill[i].isStart = true;
    }
    public void PauseGame()
    {
        if (curMonsters.Count > 0)
            foreach (Monster m in curMonsters)
                m.isStart = false;
        PlayerCtrlManager.m.curPlayer.isStart = false;
        for (int i = 0; i < 3; i++)
            IngameMainCV.m.aSkill[i].isStart = false;
    }
    public void GameEnd(bool isWin = true)
    {
        if (isWin)
        {
            UserSkill rewardSkill = GetReward(curStageCode);
            IngameMainCV.m.GameEndResultSet(rewardSkill);
            UserInfo.m.userData.skills.Add(rewardSkill.code);
            SkillManager.m.ownSkill.Add(rewardSkill);
        }
        else
        {
            IngameMainCV.m.GameEndResultSet(null);
        }
        if (UserInfo.m.userData.bestStage < curStageCode)
            UserInfo.m.userData.bestStage = curStageCode;
        // 경험치 및 골드도 나중에 넣어야함
        UserInfo.m.SaveInfo();
    }
    UserSkill GetReward(uint stageCode)
    {
        UserSkill result = null;
        uint finalStage = stageCode / 100;
        List<eSkillRank> rankL = new List<eSkillRank>();
        List<float> rateL = new List<float>();
        switch (finalStage)
        {
            case 1:
                rankL.Add(eSkillRank.E);
                rateL.Add(10f);
                rankL.Add(eSkillRank.F);
                rateL.Add(100f);
                break;
            case 2:
                rankL.Add(eSkillRank.D);
                rateL.Add(5f);
                rankL.Add(eSkillRank.E);
                rateL.Add(30f);
                rankL.Add(eSkillRank.F);
                rateL.Add(100f);
                break;
            case 3:
                rankL.Add(eSkillRank.C);
                rateL.Add(3f);
                rankL.Add(eSkillRank.D);
                rateL.Add(13f);
                rankL.Add(eSkillRank.E);
                rateL.Add(50f);
                rankL.Add(eSkillRank.F);
                rateL.Add(100f);
                break;
            default:
                Debug.LogError("GameEnd Error! 접근할 수 없는 스테이지에 접근했습니다!");
                break;
        }

        float rate = Random.Range(0, 100f);
        eSkillRank targetRank = eSkillRank.F;
        for (int i = 0; i < rankL.Count; i++)
        {
            if (rateL[i] < rate)
            {
                targetRank = rankL[i];
                break;
            }
        }

        int rdCode4 = Random.Range(1000, (int)eUSkillCode.maxIdx);
        uint code5 = (uint)(rdCode4 * 10) + (uint)targetRank;

        // 지금은 U스킬이 F랑 같이 나옴... 이걸 좀 어떻게 하고 싶은데 방법 고안좀

        if (SkillManager.m.uSkillDict.ContainsKey(code5))
        {
            result = SkillManager.m.uSkillDict[code5];
        }
        else
        {
            // 랭크가 없는 언어류의 경우 이쪽으로 빠져서 자동으로 코드5 마지막자리를 0으로 만들어줌
            result = SkillManager.m.uSkillDict[(uint)(rdCode4 * 10)];
        }

        return result;
    }
    void CreateTileInfo()
    {
        // 3x3
        eTileType[,] stage101 =
        {
            { eTileType.Normal, eTileType.Normal, eTileType.NSpawn },
            { eTileType.Normal, eTileType.Normal, eTileType.NSpawn },
            { eTileType.Normal, eTileType.Normal, eTileType.NSpawn }
        };
        fieldTileInfo.Add(101, stage101);
        stTilePos spp = new stTilePos(1, 0);
        playerPosInfo.Add(101, spp);

        // 3x4
        eTileType[,] stage102 =
        {
            { eTileType.Normal, eTileType.Normal, eTileType.NSpawn, eTileType.NSpawn },
            { eTileType.Normal, eTileType.Normal, eTileType.NSpawn, eTileType.NSpawn },
            { eTileType.Normal, eTileType.Normal, eTileType.NSpawn, eTileType.NSpawn }
        };
        fieldTileInfo.Add(102, stage102);
        spp = new stTilePos(1, 0);
        playerPosInfo.Add(102, spp);
        // 3x5
        eTileType[,] stage103 =
        {
            { eTileType.Normal, eTileType.Normal, eTileType.NSpawn, eTileType.NSpawn, eTileType.NSpawn },
            { eTileType.Normal, eTileType.Normal, eTileType.NSpawn, eTileType.NSpawn, eTileType.NSpawn },
            { eTileType.Normal, eTileType.Normal, eTileType.NSpawn, eTileType.NSpawn, eTileType.NSpawn }
        };
        fieldTileInfo.Add(103, stage103);
        spp = new stTilePos(1, 0);
        playerPosInfo.Add(103, spp);
        // 3x5
        eTileType[,] stage104 =
        {
            { eTileType.Normal, eTileType.Normal, eTileType.NSpawn, eTileType.NSpawn, eTileType.NSpawn },
            { eTileType.Normal, eTileType.Normal, eTileType.NSpawn, eTileType.NSpawn, eTileType.NSpawn },
            { eTileType.Normal, eTileType.Normal, eTileType.NSpawn, eTileType.NSpawn, eTileType.NSpawn }
        };
        fieldTileInfo.Add(104, stage104);
        spp = new stTilePos(1, 0);
        playerPosInfo.Add(104, spp);
        // 4x5
        eTileType[,] stage105 =
        {
            { eTileType.Normal, eTileType.Normal, eTileType.NSpawn, eTileType.NSpawn, eTileType.NSpawn },
            { eTileType.Normal, eTileType.Normal, eTileType.Normal, eTileType.NSpawn, eTileType.NSpawn },
            { eTileType.Normal, eTileType.Normal, eTileType.Normal, eTileType.NSpawn, eTileType.NSpawn },
            { eTileType.Normal, eTileType.Normal, eTileType.NSpawn, eTileType.NSpawn, eTileType.NSpawn }
        };
        fieldTileInfo.Add(105, stage105);
        spp = new stTilePos(2, 0);
        playerPosInfo.Add(105, spp);
        // 4x6
        eTileType[,] stage106 =
        {
            { eTileType.Normal, eTileType.Normal, eTileType.Normal, eTileType.NSpawn, eTileType.NSpawn, eTileType.NSpawn },
            { eTileType.Normal, eTileType.Normal, eTileType.Normal, eTileType.NSpawn, eTileType.NSpawn, eTileType.NSpawn },
            { eTileType.Normal, eTileType.Normal, eTileType.Normal, eTileType.NSpawn, eTileType.NSpawn, eTileType.NSpawn },
            { eTileType.Normal, eTileType.Normal, eTileType.Normal, eTileType.NSpawn, eTileType.NSpawn, eTileType.NSpawn }
        };
        fieldTileInfo.Add(106, stage106);
        spp = new stTilePos(2, 1);
        playerPosInfo.Add(106, spp);
        // 3x6
        eTileType[,] stage107 =
        {
            { eTileType.Normal, eTileType.Normal, eTileType.Normal, eTileType.NSpawn, eTileType.NSpawn, eTileType.NSpawn },
            { eTileType.Normal, eTileType.Normal, eTileType.Normal, eTileType.NSpawn, eTileType.NSpawn, eTileType.NSpawn },
            { eTileType.Normal, eTileType.Normal, eTileType.Normal, eTileType.NSpawn, eTileType.NSpawn, eTileType.NSpawn },
        };
        fieldTileInfo.Add(107, stage107);
        spp = new stTilePos(1, 1);
        playerPosInfo.Add(107, spp);
        // 4x6
        eTileType[,] stage108 =
        {
            { eTileType.Normal, eTileType.Normal, eTileType.Normal, eTileType.NSpawn, eTileType.NSpawn, eTileType.NSpawn },
            { eTileType.Normal, eTileType.Normal, eTileType.Normal, eTileType.NSpawn, eTileType.NSpawn, eTileType.NSpawn },
            { eTileType.Normal, eTileType.Normal, eTileType.Normal, eTileType.NSpawn, eTileType.NSpawn, eTileType.NSpawn },
            { eTileType.Normal, eTileType.Normal, eTileType.Normal, eTileType.NSpawn, eTileType.NSpawn, eTileType.NSpawn }
        };
        fieldTileInfo.Add(108, stage108);
        spp = new stTilePos(2, 1);
        playerPosInfo.Add(108, spp);
        // 5x7
        eTileType[,] stage109 =
        {
            { eTileType.Normal, eTileType.Normal, eTileType.Normal, eTileType.NSpawn, eTileType.NSpawn, eTileType.NSpawn, eTileType.NSpawn },
            { eTileType.Normal, eTileType.Normal, eTileType.Normal, eTileType.NSpawn, eTileType.NSpawn, eTileType.NSpawn, eTileType.NSpawn },
            { eTileType.Normal, eTileType.Normal, eTileType.Normal, eTileType.NSpawn, eTileType.NSpawn, eTileType.NSpawn, eTileType.NSpawn },
            { eTileType.Normal, eTileType.Normal, eTileType.Normal, eTileType.NSpawn, eTileType.NSpawn, eTileType.NSpawn, eTileType.NSpawn },
            { eTileType.Normal, eTileType.Normal, eTileType.Normal, eTileType.NSpawn, eTileType.NSpawn, eTileType.NSpawn, eTileType.NSpawn }
        };
        fieldTileInfo.Add(109, stage109);
        spp = new stTilePos(2, 1);
        playerPosInfo.Add(109, spp);
        // 3x7
        eTileType[,] stage110 =
        {
            { eTileType.Normal, eTileType.Normal, eTileType.Normal, eTileType.NSpawn, eTileType.Normal, eTileType.Normal, eTileType.Normal },
            { eTileType.Normal, eTileType.Normal, eTileType.Normal, eTileType.NSpawn, eTileType.Normal, eTileType.NSpawn, eTileType.Normal },
            { eTileType.Normal, eTileType.Normal, eTileType.Normal, eTileType.NSpawn, eTileType.Normal, eTileType.Normal, eTileType.Normal }
        };
        fieldTileInfo.Add(110, stage110);
        spp = new stTilePos(1, 1);
        playerPosInfo.Add(110, spp);
    }
    public Tile TileWhere(stTilePos stp)
    {
        if (stp.row < 0 || stp.col < 0 || stp.row >= curMaxRow || stp.col >= curMaxCol) return null;
        else return curTiles[stp.row, stp.col];
    }
    public void DebuffToCurMonsters(eUSkillCode code4, int cnt, float sec, float val)
    {
        List<int> rdIdxList = new List<int>();

        // 대상 몬스터보다 현재 존재하는 몬스터 수가 더 적거나 같아서 그냥 다 적용하면 되는 경우에 이쪽
        if (cnt >= curMonsters.Count)
            foreach (Monster m in curMonsters)
                m.DebuffByPlayerSkill(code4, sec, val);
        else
        {
            int[] idxs = new int[cnt];
            for (int i = 0; i < cnt; i++)
            {
                int rdIdx = Random.Range(0, curMonsters.Count);
                bool isSame = false;
                for (int j = 0; j < i; j++)
                {
                    if (idxs[i] == rdIdx)
                    {
                        isSame = true;
                        break;
                    }
                }
                if (isSame) i--;
            }

            for (int i = 0; i < cnt; i++)
                curMonsters[idxs[i]].DebuffByPlayerSkill(code4, sec, val);
        }
    }

    public void KnockBackToCurMonsters(eUSkillCode code4, int cnt, float rate, int tileCnt)
    {
        // 대상 몬스터보다 현재 존재하는 몬스터 수가 더 적거나 같아서 걍 다 적용하면 되는 경우에 이쪽
        if (cnt >= curMonsters.Count)
            foreach (Monster m in curMonsters)
                m.KnockBackByPlayerSkill(code4, rate, tileCnt);
        else
        {
            int[] idxs = new int[cnt];
            for (int i = 0; i < cnt; i++)
            {
                int rdIdx = Random.Range(0, curMonsters.Count);
                bool isSame = false;
                for (int j = 0; j < i; j++)
                {
                    if (idxs[i] == rdIdx)
                    {
                        isSame = true;
                        break;
                    }
                }
                if (isSame) i--;
            }

            for (int i = 0; i < cnt; i++)
                curMonsters[idxs[i]].KnockBackByPlayerSkill(code4, rate, tileCnt);
        }
    }
}