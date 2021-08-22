using UnityEngine;

public enum MoveType { LeftRight = 0, UpDown }

public class Movement2D : MonoBehaviour
{
    [Header("Raycast Collision")]
    [SerializeField]
    private LayerMask collisionLayer;               // ������ �ε����� �浹 ���̾�

    [Header("Raycast")]
    [SerializeField]
    private int     horizontalRayCount = 4;         // x�� �������� �߻�Ǵ� ������ ����
    [SerializeField]
    private int     verticalRayCount = 4;           // y�� �������� �߻�Ǵ� ������ ����
    private float   horizontalRaySpacing;           // x�� ������ ���� ���� ����
    private float   verticalRaySpacing;             // y�� ������ ���� ���� ����

    [Header("Movement")]
    [SerializeField]
    private float   moveSpeed = 0.0f;               // �̵� �ӵ�
    [SerializeField]
    private Vector3 moveDirection = Vector3.zero;   // �̵� ����

    private readonly float   skinWidth = 0.015f;    // ������Ʈ �������� �İ��� �ҷ��� ����
    private Collider2D       collider2D;            // ���� �߻� ��ġ ������ ���� �浹 ����
    private ColliderCorner   colliderCorner;        // ���� �߻縦 ���� �𼭸� ��
    private CollisionChecker collisionChecker;      // 4���� �浹 ���� üũ

    public  CollisionChecker IsCollision => collisionChecker;   // �÷��̾��� 4�� �浹 ���� Ȯ�ο�
    public  Transform        HitTransform { private set; get; } // �÷��̾�� �ε��� ������Ʈ ����

    private void Awake()
    {
        collider2D = GetComponent<Collider2D>();
    }

    private void Update()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        CalculateRaySpacing();      // ���� ������ �Ÿ� ����
        UpdateColliderCorner();     // �浹 ������ �ܰ� ��ġ ����
        collisionChecker.Reset();   // �浹 ���� �ʱ�ȭ (��/�Ʒ�/��/��)

        // �̵� ������Ʈ
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
        // ���� �����ӿ� ����� ���� �ӷ�
        Vector3 currentVelocity = moveDirection * moveSpeed * Time.deltaTime;

        // �ӷ��� 0�� �ƴ� �� ������ �߻��� �̵����� ���� ����
        if ( currentVelocity.x != 0 )
        {
            RaycastsHorizontal(ref currentVelocity);
        }
        if ( currentVelocity.y != 0 )
        {
            RaycastsVertical(ref currentVelocity);
        }

        // ������Ʈ �̵�
        Vector3 direction = new Vector3(Mathf.Sign(currentVelocity.x), Mathf.Sign(currentVelocity.y), Mathf.Sign(currentVelocity.z));
        MoveTo(direction);
    }

    /// <summary>
    /// x�� �������� ���� �߻� (��/�� �̵�)
    /// </summary>
    private void RaycastsHorizontal(ref Vector3 velocity)
    {
        float direction     = Mathf.Sign(velocity.x);            // �̵� ���� (������:1, ����:-1)
        float distance      = Mathf.Abs(velocity.x) + skinWidth; // ���� ����
        Vector2 rayPosition = Vector2.zero;                      // ������ �߻�Ǵ� ��ġ
        RaycastHit2D hit;

        for ( int i=0; i<horizontalRayCount; ++i )
        {
            rayPosition = (direction == 1) ? colliderCorner.bottomRight : colliderCorner.bottomLeft;
            rayPosition += Vector2.up * (horizontalRaySpacing * i);

            hit = Physics2D.Raycast(rayPosition, Vector2.right * direction, distance, collisionLayer);

            if ( hit )
            {
                // x�� �ӷ��� ������ ������Ʈ ������ �Ÿ��� ���� (�Ÿ��� 0�̸� �ӷ� 0)
                velocity.x = (hit.distance - skinWidth) * direction;
                // ������ �߻�Ǵ� ������ �Ÿ� ����
                distance = hit.distance;

                // ���� �������, �ε��� ������ ������ true�� ����
                collisionChecker.left  = (direction == -1);
                collisionChecker.right = (direction == 1);

                // �ε��� ������Ʈ�� Transform ����
                HitTransform = hit.transform;
            }

            // Debug : �߻�Ǵ� ������ Scene View���� Ȯ��
            Debug.DrawLine(rayPosition, rayPosition + Vector2.right * direction * distance, Color.yellow);
        }
    }

    /// <summary>
    /// y�� �������� ���� �߻� (��/�� �̵�)
    /// </summary>
    private void RaycastsVertical(ref Vector3 velocity)
    {
        float direction     = Mathf.Sign(velocity.y);             // �̵� ���� (��:1, �Ʒ�:-1)
        float distance      = Mathf.Abs(velocity.y) + skinWidth;  // ���� ����
        Vector2 rayPosition = Vector2.zero;                       // ������ �߻�Ǵ� ��ġ
        RaycastHit2D hit;

        for ( int i=0; i<verticalRayCount; ++i )
        {
            rayPosition = (direction == 1) ? colliderCorner.topLeft : colliderCorner.bottomLeft;
            rayPosition += Vector2.right * (verticalRaySpacing * i + velocity.x);

            hit = Physics2D.Raycast(rayPosition, Vector2.up * direction, distance, collisionLayer);

            if ( hit )
            {
                // y�� �ӷ��� ������ ������Ʈ ������ �Ÿ��� ���� (�Ÿ��� 0�̸� �ӷ� 0)
                velocity.y = (hit.distance - skinWidth) * direction;
                // ������ �߻�Ǵ� ������ �Ÿ� ����
                distance = hit.distance;

                // ���� �������, �ε��� ������ ������ true�� ����
                collisionChecker.down = (direction == -1);
                collisionChecker.up = (direction == 1);

                // �ε��� ������Ʈ�� Transform ����
                HitTransform = hit.transform;
            }

            // Debug : �߻�Ǵ� ������ Scene View���� Ȯ��
            Debug.DrawLine(rayPosition, rayPosition + Vector2.up * direction * distance, Color.yellow);
        }
    }

    /// <summary>
    /// �� ������ �߻�Ǵ� ���� ������ ���� (���� ������ ���� �޶���)
    /// </summary>
    private void CalculateRaySpacing()
    {
        Bounds bounds = collider2D.bounds;
        bounds.Expand(skinWidth * -2);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing   = bounds.size.x / (verticalRayCount - 1);
    }

    /// <summary>
    /// �浹 ����(Collider)�� �ܰ� �� ��ġ
    /// </summary>
    private void UpdateColliderCorner()
    {
        // ���� ������Ʈ�� ��ġ �������� Collider�� ������ �޾ƿ�
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
    /// ��, ��, ��, �� �浹 ���� ����
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
        // �׷����� ������ ����
        Gizmos.color = Color.blue;
        // ��/�쿡 ǥ�õǴ� ���� �߻� ��ġ
        for (int i = 0; i < horizontalRayCount; ++i)
        {
            Vector2 position = Vector2.up * horizontalRaySpacing * i;
            // �������� ������ �׸� (��ġ, ������)
            Gizmos.DrawSphere(colliderCorner.bottomRight + position, 0.1f);
            Gizmos.DrawSphere(colliderCorner.bottomLeft + position, 0.1f);
        }
        // ��/�Ͽ� ǥ�õǴ� ���� �߻� ��ġ
        for (int i = 0; i < verticalRayCount; ++i)
        {
            Vector2 position = Vector2.right * verticalRaySpacing * i;
            // �������� ������ �׸� (��ġ, ������)
            Gizmos.DrawSphere(colliderCorner.topLeft + position, 0.1f);
            Gizmos.DrawSphere(colliderCorner.bottomLeft + position, 0.1f);
        }
    }
}
