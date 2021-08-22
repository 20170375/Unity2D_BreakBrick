using UnityEngine;

public class BarController : MonoBehaviour
{
    [SerializeField]
    private StageData  stageData;
    private Movement2D movement2D;

    private float      dragDistance = 10.0f;   // �巡�� �Ÿ�
    private Vector3    touchStart;             // ��ġ ���� ��ġ
    private Vector3    touchEnd;               // ��ġ ���� ��ġ

    private void Awake()
    {
        movement2D = GetComponent<Movement2D>();
    }

    private void Update()
    {
        // ��/�� ����Ű�� ���� �̵� ���� ����
        float x = Input.GetAxisRaw("Horizontal");

        movement2D.MoveTo(new Vector3(x, 0, 0));

        // ��ġ�� �̵� ����
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
        // �÷��̾� ĳ���Ͱ� ȭ�� ���� �ٱ����� ������ ���ϵ��� ��.
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, stageData.LimitMin.x, stageData.LimitMax.x),
                                         Mathf.Clamp(transform.position.y, stageData.LimitMin.y, stageData.LimitMax.y));
    }

    private void OnMobilePlatform()
    {
        // ���� ȭ���� ��ġ�ϰ� ���� ������ �޼ҵ� ����
        if ( Input.touchCount == 0 ) return;

        // ù ��° ��ġ ���� �ҷ�����
        Touch touch = Input.GetTouch(0);

        // ��ġ ����
        if ( touch.phase == TouchPhase.Began )
        {
            touchStart = touch.position;
        }
        // ��ġ & �巡��
        else if ( touch.phase == TouchPhase.Moved )
        {
            touchEnd = touch.position;

            OnDrag();
        }
    }

    private void OnPCPlatform()
    {
        // ��ġ ����
        if ( Input.GetMouseButtonDown(0) )
        {
            touchStart = Input.mousePosition;
        }
        // ��ġ & �巡��
        else if ( Input.GetMouseButton(0) )
        {
            touchEnd = Input.mousePosition;

            OnDrag();
        }
    }

    private void OnDrag()
    {
        // ��ġ ���·� �巡�� ������ dragDistance���� Ŭ ��
        if ( Mathf.Abs(touchEnd.x - touchStart.x) >= dragDistance )
        {
            movement2D.MoveTo(new Vector3(Mathf.Sign(touchEnd.x - touchStart.x), 0, 0));
        }
    }
}
