using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Def;

public class MonsterManager : MonoBehaviour
{
    #region [인스턴스화]
    static MonsterManager _uniqueInstance;
    public static MonsterManager m
    {
        get { return _uniqueInstance; }
    }
    #endregion

    [HideInInspector] public Dictionary<eMonster, stStatus> info = new Dictionary<eMonster, stStatus>();

    private void Awake()
    {
        _uniqueInstance = this;
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        TableLoader.m.MonsterTableLoad();
    }


}
