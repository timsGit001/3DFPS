using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    private int winPlayer;
    private int winNpc;

    public int killPlayer;
    public int killNpc1;
    public int killNpc2;
    public int killNpc3;

    public int deadPlayer;
    public int deadNpc1;
    public int deadNpc2;
    public int deadNpc3;

    [Header("玩家 - 勝利次數")]
    public Text textPlayer;
    [Header("電腦 - 勝利次數")]
    public Text textNpc;
    [Header("玩家 - 資料")]
    public Text textDataPlayer;
    [Header("電腦1 - 資料")]
    public Text textDataNpc1;
    [Header("電腦2 - 資料")]
    public Text textDataNpc2;
    [Header("電腦3 - 資料")]
    public Text textDataNpc3;

    [Header("畫布管理")]
    public CanvasGroup cGroup;

    /// <summary>
    /// 更新殺敵數量
    /// </summary>
    /// <param name="kill"></param> ref表示傳址
    /// <param name="textKill"></param>
    /// <param name="content"></param>
    /// <param name="dead"></param>
    public void UpdateDataKill(ref int kill, Text textKill, string content, int dead)
    {
        kill++;
        textKill.text = content + "      " + kill + " | " + dead;
    }

    private int enemyDeadCount;
    /// <summary>
    /// 更新死亡數量
    /// </summary>
    /// <param name="kill"></param>
    /// <param name="textKill"></param>
    /// <param name="content"></param>
    /// <param name="dead"></param>
    public void UpdateDataDead(int kill, Text textKill, string content, ref int dead)
    {
        dead++;
        textKill.text = content + "      " + kill + " | " + dead;

        if (content == "玩家:") 
        {
            StartCoroutine(ShowFinal());
            winNpc++;
            textNpc.text = "電腦勝利次數: " + winNpc;
        }
        else if (content.Contains("電腦"))
        {
            enemyDeadCount++;
            if (enemyDeadCount >= 3)
            {
                StartCoroutine(ShowFinal());
                winPlayer++;
                textPlayer.text = "玩家勝利次數: " + winPlayer;
            }
        }
    }

    private IEnumerator ShowFinal()
    {
        while (cGroup.alpha < 1)
        {
            cGroup.alpha += 0.1f;
            yield return new WaitForSeconds(0.2f);
        }
    }
}
