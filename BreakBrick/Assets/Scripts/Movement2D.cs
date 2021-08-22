using UnityEngine;

public enum MoveType { LeftRight = 0, UpDown }

public class Movement2D : MonoBehaviour
{
    [Header("Raycast Collision")]
    [SerializeField]
    private LayerMask collisionLayer;               // 광선과 부딪히는 충돌 레이어

    [Header("Raycast")]
    [SerializeField]
    private int     horizontalRayCount = 4;         // x축 방향으로 발사되는 광선의 개수
    [SerializeField]
    private int     verticalRayCount = 4;           // y축 방향으로 발사되는 광선의 개수
    private float   horizontalRaySpacing;           // x축 방향의 광선 사이 간격
    private float   verticalRaySpacing;             // y축 방향의 광선 사이 간격

    [Header("Movement")]
    [SerializeField]
    private float   moveSpeed = 0.0f;               // 이동 속도
    [SerializeField]
    private Vector3 moveDirection = Vector3.zero;   // 이동 방향

    private readonly float   skinWidth = 0.015f;    // 오브젝트 안쪽으로 파고드는 소량의 범위
    private Collider2D       collider2D;            // 광선 발사 위치 설정을 위한 충돌 범위
    private ColliderCorner   colliderCorner;        // 광선 발사를 위한 모서리 점
    private CollisionChecker collisionChecker;      // 4면의 충돌 여부 체크

    public  CollisionChecker IsCollision => collisionChecker;   // 플레이어의 4면 충돌 여부 확인용
    public  Transform        HitTransform { private set; get; } // 플레이어에게 부딪힌 오브젝트 정보

    private void Awake()
    {
        collider2D = GetComponent<Collider2D>();
    }

    private void Update()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        CalculateRaySpacing();      // 광선 사이의 거리 갱신
        UpdateColliderCorner();     // 충돌 범위의 외곽 위치 갱신
        collisionChecker.Reset();   // 충돌 여부 초기화 (위/아래/좌/우)

        // 이동 업데이트
        UpdateMovement();
    }

    public void MoveTo(Vector3 direction)
    {
        moveDirection = direction;
    }

    public void Bound(MoveType moveType)
    {
        if ( moveType == MoveType.LeftRight )
        {
            moveDirection = new Vector3(-moveDirection.x, moveDirection.y, moveDirection.z);
        }
        else if ( moveType == MoveType.UpDown )
        {
            moveDirection = new Vector3(moveDirection.x, -moveDirection.y, moveDirection.z);
        }
    }

    private void UpdateMovement()
    {
        // 현재 프레임에 적용될 실제 속력
        Vector3 currentVelocity = moveDirection * moveSpeed * Time.deltaTime;

        // 속력이 0이 아닐 때 광선을 발사해 이동가능 여부 조사
        if ( currentVelocity.x != 0 )
        {
            RaycastsHorizontal(ref currentVelocity);
        }
        if ( currentVelocity.y != 0 )
        {
            RaycastsVertical(ref currentVelocity);
        }

        // 오브젝트 이동
        Vector3 direction = new Vector3(Mathf.Sign(currentVelocity.x), Mathf.Sign(currentVelocity.y), Mathf.Sign(currentVelocity.z));
        MoveTo(direction);
    }

    /// <summary>
    /// x축 방향으로 광선 발사 (좌/우 이동)
    /// </summary>
    private void RaycastsHorizontal(ref Vector3 velocity)
    {
        float direction     = Mathf.Sign(velocity.x);            // 이동 방향 (오른쪽:1, 왼쪽:-1)
        float distance      = Mathf.Abs(velocity.x) + skinWidth; // 광선 길이
        Vector2 rayPosition = Vector2.zero;                      // 광선이 발사되는 위치
        RaycastHit2D hit;

        for ( int i=0; i<horizontalRayCount; ++i )
        {
            rayPosition = (direction == 1) ? colliderCorner.bottomRight : colliderCorner.bottomLeft;
            rayPosition += Vector2.up * (horizontalRaySpacing * i);

            hit = Physics2D.Raycast(rayPosition, Vector2.right * direction, distance, collisionLayer);

            if ( hit )
            {
                // x축 속력을 광선과 오브젝트 사이의 거리로 설정 (거리가 0이면 속력 0)
                velocity.x = (hit.distance - skinWidth) * direction;
                // 다음에 발사되는 광선의 거리 설정
                distance = hit.distance;

                // 현재 진행방향, 부딪힌 방향의 정보가 true로 변경
                collisionChecker.left  = (direction == -1);
                collisionChecker.right = (direction == 1);

                // 부딪힌 오브젝트의 Transform 정보
                HitTransform = hit.transform;
            }

            // Debug : 발사되는 광선을 Scene View에서 확인
            Debug.DrawLine(rayPosition, rayPosition + Vector2.right * direction * distance, Color.yellow);
        }
    }

    /// <summary>
    /// y축 방향으로 광선 발사 (상/하 이동)
    /// </summary>
    private void RaycastsVertical(ref Vector3 velocity)
    {
        float direction     = Mathf.Sign(velocity.y);             // 이동 방향 (위:1, 아래:-1)
        float distance      = Mathf.Abs(velocity.y) + skinWidth;  // 광선 길이
        Vector2 rayPosition = Vector2.zero;                       // 광선이 발사되는 위치
        RaycastHit2D hit;

        for ( int i=0; i<verticalRayCount; ++i )
        {
            rayPosition = (direction == 1) ? colliderCorner.topLeft : colliderCorner.bottomLeft;
            rayPosition += Vector2.right * (verticalRaySpacing * i + velocity.x);

            hit = Physics2D.Raycast(rayPosition, Vector2.up * direction, distance, collisionLayer);

            if ( hit )
            {
                // y축 속력을 광선과 오브젝트 사이의 거리로 설정 (거리가 0이면 속력 0)
                velocity.y = (hit.distance - skinWidth) * direction;
                // 다음에 발사되는 광선의 거리 설정
                distance = hit.distance;

                // 현재 진행방향, 부딪힌 방향의 정보가 true로 변경
                collisionChecker.down = (direction == -1);
                collisionChecker.up = (direction == 1);

                // 부딪힌 오브젝트의 Transform 정보
                HitTransform = hit.transform;
            }

            // Debug : 발사되는 광선을 Scene View에서 확인
            Debug.DrawLine(rayPosition, rayPosition + Vector2.up * direction * distance, Color.yellow);
        }
    }

    /// <summary>
    /// 한 축으로 발사되는 광선 사이의 간격 (광선 개수에 따라 달라짐)
    /// </summary>
    private void CalculateRaySpacing()
    {
        Bounds bounds = collider2D.bounds;
        bounds.Expand(skinWidth * -2);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing   = bounds.size.x / (verticalRayCount - 1);
    }

    /// <summary>
    /// 충돌 범위(Collider)의 외곽 점 위치
    /// </summary>
    private void UpdateColliderCorner()
    {
        // 현재 오브젝트의 위치 기준으로 Collider의 정보를 받아옴
        Bounds bounds = collider2D.bounds;
        bounds.Expand(skinWidth * -2);

        colliderCorner.topLeft     = new Vector2(bounds.min.x, bounds.max.y);
        colliderCorner.bottomLeft  = new Vector2(bounds.min.x, bounds.min.y);
        colliderCorner.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
    }

    private struct ColliderCorner
    {
        public Vector2 topLeft;
        public Vector2 bottomLeft;
        public Vector2 bottomRight;
    }

    /// <summary>
    /// 상, 하, 좌, 우 충돌 여부 판정
    /// </summary>
    public struct CollisionChecker
    {
        public bool up;
        public bool down;
        public bool left;
        public bool right;

        public void Reset()
        {
            up = false;
            down = false;
            left = false;
            right = false;
        }
    }

    private void OnDrawGizmos()
    {
        // 그려지는 도형의 색상
        Gizmos.color = Color.blue;
        // 좌/우에 표시되는 광선 발사 위치
        for (int i = 0; i < horizontalRayCount; ++i)
        {
            Vector2 position = Vector2.up * horizontalRaySpacing * i;
            // 구형태의 도형을 그림 (위치, 반지름)
            Gizmos.DrawSphere(colliderCorner.bottomRight + position, 0.1f);
            Gizmos.DrawSphere(colliderCorner.bottomLeft + position, 0.1f);
        }
        // 상/하에 표시되는 광선 발사 위치
        for (int i = 0; i < verticalRayCount; ++i)
        {
            Vector2 position = Vector2.right * verticalRaySpacing * i;
            // 구형태의 도형을 그림 (위치, 반지름)
            Gizmos.DrawSphere(colliderCorner.topLeft + position, 0.1f);
            Gizmos.DrawSphere(colliderCorner.bottomLeft + position, 0.1f);
        }
    }
}
