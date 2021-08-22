using UnityEngine;

public class GameOverController : MonoBehaviour
{
    [SerializeField]
    private int stageNum = 1;    // 시작 stage 번호

    public void Retry()
    {
        // Stage 정보 초기화
        string nextStage = (stageNum > 9) ? "Stage" + stageNum : "Stage0" + stageNum;
        PlayerPrefs.SetInt("StageNum", stageNum+1);

        // 체력 초기화
        int maxHeart = PlayerPrefs.GetInt("MaxHeart");
        PlayerPrefs.SetInt("Heart", maxHeart);

        // Scene 재시작
        SceneLoader.LoadScene(nextStage);
    }
}
