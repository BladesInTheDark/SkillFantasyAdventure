using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// 인게임에서 대미지를 출력하는 클래스
public class DamagePrintSystem : MonoBehaviour
{
    [SerializeField] float maxPos = 5;
    [SerializeField] TextMeshProUGUI dmgT;

    public void Awake()
    {
        dmgT.gameObject.SetActive(false);
    }
    public void PrintDamage(int dmg, Color c)
    {
        dmgT.gameObject.SetActive(true);
        Vector2 pos = new Vector2(Random.Range(-maxPos, maxPos), Random.Range(-maxPos, maxPos));
        dmgT.transform.localPosition = pos;
        dmgT.text = dmg.ToString();
        dmgT.color = c;
        dmgT.gameObject.SetActive(true);
        StartCoroutine(TextOff());
    }
    public void PrintDamage(string dmg, Color c)
    {
        
        dmgT.gameObject.SetActive(true);
        Vector2 pos = new Vector2(Random.Range(-maxPos, maxPos), Random.Range(-maxPos, maxPos));
        dmgT.transform.localPosition = pos;
        dmgT.text = dmg;
        dmgT.color = c;
        dmgT.gameObject.SetActive(true);
        StartCoroutine(TextOff());
    }
    IEnumerator TextOff()
    {
        yield return new WaitForSeconds(1f);
        dmgT.gameObject.SetActive(false);
    }
}
