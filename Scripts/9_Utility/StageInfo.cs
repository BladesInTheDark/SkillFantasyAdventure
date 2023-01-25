using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// DB에서 받아온(나중에 DB 씀, 지금은 수동) 스테이지 데이터(필드별 배치 정보 등을 가짐)를 가지고 있으며 FieldManager의 기능까지 전달해주는 함수.
// 인포를 저장하고 있는 클래스
public class StageInfo : MonoBehaviour
{
    static StageInfo _uniqueInstance;
    public static StageInfo m
    {
        get { return _uniqueInstance; }
    }

    private void Awake()
    {
        _uniqueInstance = this;
    }
}
