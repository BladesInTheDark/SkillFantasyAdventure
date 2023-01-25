using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Def;

// 설정창을 조작에 반응하는 클래스
public class OptionWnd : MonoBehaviour
{
    bool isBgmMute = false;
    bool isSeMute = false;
    Animator ownAnim;

    [SerializeField] Image bgmMuteImg;
    [SerializeField] GameObject bgmMuteX;
    [SerializeField] Slider bgmSlider;
    [SerializeField] Image seMuteImg;
    [SerializeField] GameObject seMuteX;
    [SerializeField] Slider seSlider;

    private void Awake()
    {
        ownAnim = GetComponent<Animator>();
    }

    public void OpenWindow()
    {
        AudioManager.m.PlaySE(R_SE.ButtonClick);
        if (UserInfo.m.userData == null)
        {
            isBgmMute = AudioManager.m.bgmAS.mute;
            isSeMute = AudioManager.m.seAS.mute;
            bgmMuteX.SetActive(isBgmMute);
            seMuteX.SetActive(isSeMute);
            bgmSlider.value = AudioManager.m.bgmAS.volume;
            seSlider.value = AudioManager.m.seAS.volume;
        }
        else
        {
            isBgmMute = UserInfo.m.userData.isBGMMute;
            isSeMute = UserInfo.m.userData.isSEMute;
            bgmMuteX.SetActive(isBgmMute);
            seMuteX.SetActive(isSeMute);
            bgmSlider.value = UserInfo.m.userData.bgmVol;
            seSlider.value = UserInfo.m.userData.seVol;
        }
        if (isBgmMute) bgmSlider.interactable = false;
        else bgmSlider.interactable = true;
        if (isSeMute) seSlider.interactable = false;
        else seSlider.interactable = true;
        ownAnim.SetTrigger("Open");
    }
    public void CloseWindow()
    {
        AudioManager.m.PlaySE(R_SE.ButtonClick);
        ownAnim.SetTrigger("Close");
        SaveToUserInfo();
    }
    public void BGMMuteClicked()
    {
        isBgmMute = !isBgmMute;

        if (isBgmMute)
        {
            bgmSlider.interactable = false;
        }
        else
        {
            bgmSlider.interactable = true;
        }

        bgmMuteX.SetActive(isBgmMute);
        AudioManager.m.MuteBGM(isBgmMute);
    }
    public void BGMSlider()
    {
        AudioManager.m.ChangeBGMVolume(bgmSlider.value);
    }
    public void SEMuteClicked()
    {
        isSeMute = !isSeMute;

        if (isSeMute)
        {
            seSlider.interactable = false;
        }
        else
        {
            seSlider.interactable = true;
        }

        seMuteX.SetActive(isSeMute);
        AudioManager.m.MuteSE(isSeMute);
    }
    public void SESlider()
    {
        AudioManager.m.ChangeSEVolume(seSlider.value);
    }
    public void SaveToUserInfo()
    {
        if (UserInfo.m.userData == null) return;
        UserInfo.m.userData.isBGMMute = isBgmMute;
        UserInfo.m.userData.isSEMute = isSeMute;
        UserInfo.m.userData.bgmVol = bgmSlider.value;
        UserInfo.m.userData.seVol = seSlider.value;
        UserInfo.m.SaveInfo();
    }
}
