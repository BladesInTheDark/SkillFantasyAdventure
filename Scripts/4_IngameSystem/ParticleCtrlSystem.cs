using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Def;

// 인게임에서 각각 스킬에 맞는 파티클을 실행해주는 클래스
public class ParticleCtrlSystem : MonoBehaviour
{
    //[SerializeField] TextMeshProUGUI seeDmg;
    [SerializeField] GameObject[] particles;
    public void ParticleOn(R_Particle ptc, float time = 0)
    {
        int idx = (int)ptc;
        if (particles[idx].activeSelf) particles[idx].SetActive(false);
        particles[idx].SetActive(true);

        if (time != 0)
        {
            StartCoroutine(ParticleOffAfter(idx, time));
        }
    }
    IEnumerator ParticleOffAfter(int idx, float time)
    {
        yield return new WaitForSeconds(time);
        particles[idx].SetActive(false);
    }
    public void ParticleOff(R_Particle ptc)
    {
        int idx = (int)ptc;
        particles[idx].SetActive(false);
    }
}
