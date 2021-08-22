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
    private Image[]    heartImageArray;  // heart 배열
    [SerializeField]
    private Sprite     hitImage;         // heart 이미지 변경을 위함
    private int        heart;            // 남은 생명
    private int        maxHeart;         // 최대 생명
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

        // Stage 정보 갱신
        int stageNum = 1;
        int.TryParse(textStageName.text.Substring(6), out stageNum);
        PlayerPrefs.SetInt("StageNum", stageNum+1);

        // 체력 정보 불러오기
        maxHeart = PlayerPrefs.GetInt("MaxHeart");
        Heart    = PlayerPrefs.GetInt("Heart");
    }

    private void Update()
    {
        // 모든 벽돌 파괴시 GameClear
        if ( FindObjectsOfType<Brick>().Length == 0 ) { GameClear(); }

        // Ball 바닥 충돌시 체력 감소
        if ( ball && (ball.transform.position.y <= stageData.LimitMin.y) ) { Hit(); }

        // Heart UI 업데이트
        UpdateHeartUI();
    }

    private void GameClear()
    {
        // 체력+1
        PlayerPrefs.SetInt("Heart", Heart+1);

        // 다음 Stage로 이동
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

        // 체력 감소
        Heart--;

        // 체력이 0이 되면 GameOver
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
