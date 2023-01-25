using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Def;

// 오디오 관리
public class AudioManager : MonoBehaviour
{
    #region [인스턴스화]
    static AudioManager _uniqueInstance;
    public static AudioManager m
    {
        get { return _uniqueInstance; }
    }
    #endregion

    [SerializeField] public AudioSource bgmAS;
    [SerializeField] public AudioSource seAS;

    private void Awake()
    {
        _uniqueInstance = this;
        DontDestroyOnLoad(this);
    }

    public void InitSet()
    {
        bgmAS.mute = UserInfo.m.userData.isBGMMute;
        seAS.mute = UserInfo.m.userData.isSEMute;
        bgmAS.volume = UserInfo.m.userData.bgmVol;
        seAS.volume = UserInfo.m.userData.seVol;
    }
    public bool CheckBGMPlaying()
    {
        return bgmAS.isPlaying;
    }
    public void SetBGM(R_BGM res)
    {
        bgmAS.clip = Res.m.GetBGM(res);
    }
    public void PlayBGM()
    {
        if (bgmAS.isPlaying) return;
        else bgmAS.Play();
    }
    public void StopBGM()
    {
        bgmAS.Stop();
    }
    public void MuteBGM(bool state)
    {
        bgmAS.mute = state;
    }
    public void ChangeBGMVolume(float volume)
    {
        bgmAS.volume = volume;
    }


    public void PlaySE(R_SE res)
    {
        seAS.PlayOneShot(Res.m.GetSE(res));
    }
    public void MuteSE(bool state)
    {
        seAS.mute = state;
    }

    public void ChangeSEVolume(float volume)
    {
        seAS.volume = volume;
    }
}
