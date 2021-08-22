using UnityEngine;

public class IntroController : MonoBehaviour
{
    [SerializeField]
    private int heart = 4;       // 시작 heart
    [SerializeField]
    private int stageNum = 1;    // 시작 stage 번호
    [SerializeField]
    private int endStageNum = 4; // 종료 stage 번호

    public void GameStart()
    {
        // 게임 정보 저장
        PlayerPrefs.SetInt("StageNum", stageNum);
        PlayerPrefs.SetInt("EndStageNum", endStageNum);
        PlayerPrefs.SetInt("Heart", heart);
        PlayerPrefs.SetInt("MaxHeart", heart);

        string nextStage = (stageNum > 9) ? "Stage" + stageNum : "Stage0" + stageNum;
        SceneLoader.LoadScene(nextStage);
    }
}
