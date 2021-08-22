using UnityEngine;

public class IntroController : MonoBehaviour
{
    [SerializeField]
    private int heart = 4;       // ���� heart
    [SerializeField]
    private int stageNum = 1;    // ���� stage ��ȣ
    [SerializeField]
    private int endStageNum = 4; // ���� stage ��ȣ

    public void GameStart()
    {
        // ���� ���� ����
        PlayerPrefs.SetInt("StageNum", stageNum);
        PlayerPrefs.SetInt("EndStageNum", endStageNum);
        PlayerPrefs.SetInt("Heart", heart);
        PlayerPrefs.SetInt("MaxHeart", heart);

        string nextStage = (stageNum > 9) ? "Stage" + stageNum : "Stage0" + stageNum;
        SceneLoader.LoadScene(nextStage);
    }
}
