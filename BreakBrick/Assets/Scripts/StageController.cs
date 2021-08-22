using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class StageController : MonoBehaviour
{
    [Header("Game")]
    [SerializeField]
    private StageData  stageData;
    [SerializeField]
    private Image[]    heartImageArray;  // heart �迭
    [SerializeField]
    private Sprite     hitImage;         // heart �̹��� ������ ����
    private int        heart;            // ���� ����
    private int        maxHeart;         // �ִ� ����
    [SerializeField]
    private GameObject hitPanel;

    [Header("Ball")]
    [SerializeField]
    private GameObject ball;

    [Header("UI")]
    [SerializeField]
    private TextMeshProUGUI textStageName;

    public int Heart
    {
        set => heart = Mathf.Clamp(value, 0, maxHeart);
        get => heart;
    }

    private void Awake()
    {
        textStageName.text = SceneManager.GetActiveScene().name;

        // Stage ���� ����
        int stageNum = 1;
        int.TryParse(textStageName.text.Substring(6), out stageNum);
        PlayerPrefs.SetInt("StageNum", stageNum+1);

        // ü�� ���� �ҷ�����
        maxHeart = PlayerPrefs.GetInt("MaxHeart");
        Heart    = PlayerPrefs.GetInt("Heart");
    }

    private void Update()
    {
        // ��� ���� �ı��� GameClear
        if ( FindObjectsOfType<Brick>().Length == 0 ) { GameClear(); }

        // Ball �ٴ� �浹�� ü�� ����
        if ( ball && (ball.transform.position.y <= stageData.LimitMin.y) ) { Hit(); }

        // Heart UI ������Ʈ
        UpdateHeartUI();
    }

    private void GameClear()
    {
        // ü��+1
        PlayerPrefs.SetInt("Heart", Heart+1);

        // ���� Stage�� �̵�
        int stageNum    = PlayerPrefs.GetInt("StageNum");
        int endStageNum = PlayerPrefs.GetInt("EndStageNum");
        if ( stageNum > endStageNum )
        {
            SceneManager.LoadScene("GameClear");
        }
        else
        { 
            string nextStage = (stageNum > 9) ? "Stage" + stageNum : "Stage0" + stageNum;
            SceneManager.LoadScene(nextStage);
        }
    }

    private IEnumerator GameOver()
    {
        Destroy(ball);

        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene("GameOver");
    }

    private void Hit()
    {
        StartCoroutine("HitAnimation");

        // ü�� ����
        Heart--;

        // ü���� 0�� �Ǹ� GameOver
        if ( Heart == 0 ) { StartCoroutine(GameOver()); }
    }

    private void UpdateHeartUI()
    {
        for ( int i=0; i<maxHeart; ++i )
        {
            if ( i >= Heart ) { heartImageArray[i].sprite = hitImage; }
        }
    }

    private IEnumerator HitAnimation()
    {
        hitPanel.SetActive(true);

        yield return new WaitForSeconds(0.1f);

        hitPanel.SetActive(false);
    }
}
