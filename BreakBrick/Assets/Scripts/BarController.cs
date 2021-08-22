using UnityEngine;

public class BarController : MonoBehaviour
{
    [SerializeField]
    private StageData  stageData;
    private Movement2D movement2D;

    private float      dragDistance = 10.0f;   // 드래그 거리
    private Vector3    touchStart;             // 터치 시작 위치
    private Vector3    touchEnd;               // 터치 종료 위치

    private void Awake()
    {
        movement2D = GetComponent<Movement2D>();
    }

    private void Update()
    {
        // 좌/우 방향키를 눌러 이동 방향 설정
        float x = Input.GetAxisRaw("Horizontal");

        movement2D.MoveTo(new Vector3(x, 0, 0));

        // 터치로 이동 조작
        if (Application.isMobilePlatform)
        {
            OnMobilePlatform();
        }
        else
        {
            OnPCPlatform();
        }
    }

    private void LateUpdate()
    {
        // 플레이어 캐릭터가 화면 범위 바깥으로 나가지 못하도록 함.
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, stageData.LimitMin.x, stageData.LimitMax.x),
                                         Mathf.Clamp(transform.position.y, stageData.LimitMin.y, stageData.LimitMax.y));
    }

    private void OnMobilePlatform()
    {
        // 현재 화면을 터치하고 있지 않으면 메소드 종료
        if ( Input.touchCount == 0 ) return;

        // 첫 번째 터치 정보 불러오기
        Touch touch = Input.GetTouch(0);

        // 터치 시작
        if ( touch.phase == TouchPhase.Began )
        {
            touchStart = touch.position;
        }
        // 터치 & 드래그
        else if ( touch.phase == TouchPhase.Moved )
        {
            touchEnd = touch.position;

            OnDrag();
        }
    }

    private void OnPCPlatform()
    {
        // 터치 시작
        if ( Input.GetMouseButtonDown(0) )
        {
            touchStart = Input.mousePosition;
        }
        // 터치 & 드래그
        else if ( Input.GetMouseButton(0) )
        {
            touchEnd = Input.mousePosition;

            OnDrag();
        }
    }

    private void OnDrag()
    {
        // 터치 상태로 드래그 범위가 dragDistance보다 클 때
        if ( Mathf.Abs(touchEnd.x - touchStart.x) >= dragDistance )
        {
            movement2D.MoveTo(new Vector3(Mathf.Sign(touchEnd.x - touchStart.x), 0, 0));
        }
    }
}
