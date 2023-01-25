using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Def;

public class TableLoader : MonoBehaviour
{
    #region [인스턴스화]
    static TableLoader _uniqueInstance;
    public static TableLoader m
    {
        get { return _uniqueInstance; }
    }
    #endregion

    public TextAsset uSkillTable;
    public TextAsset mSkillTable;
    public TextAsset monsterTable;
    public TextAsset fieldTable;

    private void Awake()
    {
        _uniqueInstance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void FieldTableLoad()
    {
        TextAsset fTable = fieldTable;
        string curText = fTable.text.Substring(0, fTable.text.Length);
        string[] line = curText.Split('\n');

        for (int i = 1; i < line.Length; i++)
        {
            string[] rows = line[i].Split('\t');
            if (!uint.TryParse(rows[0], out uint row0))
                Debug.LogError(string.Format("Field Table Load Error!(row0)"));
            
            if (!uint.TryParse(rows[1], out uint row1))
                Debug.LogError(string.Format("Field Table Load Error!(row1)"));
            
            if (!int.TryParse(rows[2], out int row2))
                Debug.LogError(string.Format("Field Table Load Error!(row2)"));
            
            if (!int.TryParse(rows[3], out int row3))
                Debug.LogError(string.Format("Field Table Load Error!(row3)"));
            
            if (!int.TryParse(rows[4], out int row4))
                Debug.LogError(string.Format("Field Table Load Error!(row4)"));
            
            if (!int.TryParse(rows[5], out int row5))
                Debug.LogError(string.Format("Field Table Load Error!(row5)"));
            
            uint code = row0 * 100 + row1;
            int row = row2;
            int col = row3;
            int min = row4;
            int max = row5;
            List<eMonster> mList = new List<eMonster>();
            List<float> rList = new List<float>();

            for (int j = 6; j < rows.Length; j++)
            {
                if (!float.TryParse(rows[j], out float rate))
                    Debug.LogError(string.Format("Field Table Load Error!(row{0})", j));

                if (rate == 0) continue;
                mList.Add((eMonster)(j - 6));
                if (rList.Count == 0) rList.Add(rate);
                else rList.Add(rList[rList.Count - 1] + rate);
            }

            stFieldInfo tmp = new stFieldInfo(row, col, min, max, mList, rList);
            FieldManager.m.fieldInfo.Add(code, tmp);
        }
    }
    
    public void MonsterTableLoad()
    {
        TextAsset mTable = monsterTable;
        string curText = mTable.text.Substring(0, mTable.text.Length);
        string[] line = curText.Split('\n');

        for (int i = 1; i < line.Length; i++)
        {
            string[] row = line[i].Split('\t');
            if (!System.Enum.TryParse(row[0], out eMonster tmpM))
                Debug.LogError(string.Format("Monster Table Load Error!(row{0})", i));

            eMonster type = tmpM;


            if (!float.TryParse(row[1], out float row1))
                Debug.LogError(string.Format("Field Table Load Error!(row1)"));

            if (!float.TryParse(row[2], out float row2))
                Debug.LogError(string.Format("Field Table Load Error!(row2)"));

            if (!float.TryParse(row[3], out float row3))
                Debug.LogError(string.Format("Field Table Load Error!(row3)"));

            if (!float.TryParse(row[4], out float row4))
                Debug.LogError(string.Format("Field Table Load Error!(row4)"));

            if (!float.TryParse(row[5], out float row5))
                Debug.LogError(string.Format("Field Table Load Error!(row5)"));

            if (!float.TryParse(row[6], out float row6))
                Debug.LogError(string.Format("Field Table Load Error!(row3)"));

            if (!float.TryParse(row[7], out float row7))
                Debug.LogError(string.Format("Field Table Load Error!(row4)"));

            if (!float.TryParse(row[8], out float row8))
                Debug.LogError(string.Format("Field Table Load Error!(row5)"));

            stStatus tmpStat = new stStatus(type, row1, row2, row3, row4, row5, row6, row7, row8, row[9]);
            MonsterManager.m.info.Add(type, tmpStat);
        }
    }
    public void USkillTableLoad()
    {
        TextAsset table = uSkillTable;
        string curText = table.text.Substring(0, table.text.Length);
        string[] line = curText.Split('\n');

        for (int i = 1; i < line.Length; i++)
        {
            UserSkill us = new UserSkill(line[i]);
            SkillManager.m.uSkillDict.Add(us.code, us);
        }
    }
    public void MSkillTableLoad()
    {
        TextAsset table = mSkillTable;
        string curText = table.text.Substring(0, table.text.Length);
        string[] line = curText.Split('\n');

        for (int i = 1; i < line.Length; i++)
        {
            MonsterSkill ms = new MonsterSkill(line[i]);
            SkillManager.m.mSkillDict.Add(ms.code, ms);
        }
    }
}
