using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Def;

// 씬 전환에 사용하는 매니저
public class SceneCtrlManager : MonoBehaviour
{
    #region [인스턴스화]
    static SceneCtrlManager _uniqueInstance;
    public static SceneCtrlManager m
    {
        get { return _uniqueInstance; }
    }
    #endregion

    eSceneType _nowSceneType;
    eSceneType _oldSceneType;

    [SerializeField] SpriteRenderer mainBG;
    [SerializeField] Animator loadingAnim;
    [SerializeField] TextMeshProUGUI loadingGauge;

    private void Awake()
    {
        _uniqueInstance = this;
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        DontDestroyOnLoad(mainBG.gameObject);
        DontDestroyOnLoad(loadingAnim.gameObject);
        mainBG.sprite = Res.m.GetBGSprite((int)eSceneType.Login);
        StartLoginScene();
    }

    public void StartLoginScene()
    {
        _oldSceneType = _nowSceneType;
        _nowSceneType = eSceneType.Login;

        StartCoroutine(LoaddingScene(_nowSceneType));
    }
    public void StartLobbyScene()
    {
        _oldSceneType = _nowSceneType;
        _nowSceneType = eSceneType.Lobby;

        StartCoroutine(LoaddingScene(_nowSceneType));
    }
    public void StartIngameScene()
    {
        _oldSceneType = _nowSceneType;
        _nowSceneType = eSceneType.Ingame;

        StartCoroutine(LoaddingScene(_nowSceneType));
    }
    IEnumerator LoaddingScene(eSceneType e)
    {
        switch (e)
        {
            case eSceneType.Ingame:
                AudioManager.m.StopBGM();
                break;
        }
        loadingGauge.text = "0%";
        loadingAnim.SetBool("CanSee", true);
        yield return new WaitForSeconds(1f);
        string sceneName = e.ToString() + "Scene";
        AsyncOperation aOper = SceneManager.LoadSceneAsync(sceneName);

        while (!aOper.isDone)
        {
            loadingGauge.text = Mathf.Round(aOper.progress * 100).ToString() + "%";
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        loadingGauge.text = "100%";

        Sprite bg = null;
        switch (e)
        {
            case eSceneType.Login:
                AudioManager.m.SetBGM(R_BGM.Lobby);
                bg = Res.m.GetBGSprite(R_Background.OnLogin);
                mainBG.transform.localScale = new Vector3(0.7f, 0.7f, 1);
                break;

            case eSceneType.Lobby:
                if (_oldSceneType != eSceneType.Login) AudioManager.m.SetBGM(R_BGM.Lobby);
                bg = Res.m.GetBGSprite(R_Background.OnLobby);
                mainBG.transform.localScale = new Vector3(0.7f, 0.7f, 1);
                LobbyMainCV.m.InitSet();
                break;

            case eSceneType.Ingame:
                AudioManager.m.SetBGM(R_BGM.IngameStage1);
                bg = Res.m.GetBGSprite(R_Background.OnIngameStage1);
                mainBG.transform.localScale = Vector3.one;
                break;
        }

        mainBG.sprite = bg;
        //loadingAnim.SetTrigger("Close");
        loadingAnim.SetBool("CanSee", false);
        AudioManager.m.PlayBGM();
    }

}
