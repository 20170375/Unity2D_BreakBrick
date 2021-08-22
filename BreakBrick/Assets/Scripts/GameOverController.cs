using UnityEngine;

public class GameOverController : MonoBehaviour
{
    [SerializeField]
    private int stageNum = 1;    // ���� stage ��ȣ

    public void Retry()
    {
        // Stage ���� �ʱ�ȭ
        string nextStage = (stageNum > 9) ? "Stage" + stageNum : "Stage0" + stageNum;
        PlayerPrefs.SetInt("StageNum", stageNum+1);

        // ü�� �ʱ�ȭ
        int maxHeart = PlayerPrefs.GetInt("MaxHeart");
        PlayerPrefs.SetInt("Heart", maxHeart);

        // Scene �����
        SceneLoader.LoadScene(nextStage);
    }
}
