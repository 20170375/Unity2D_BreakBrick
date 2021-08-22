using UnityEngine;

public class Ball : MonoBehaviour
{
    private Movement2D       movement2D;
    private CircleCollider2D circleCollider2D;

    private void Awake()
    {
        movement2D       = GetComponent<Movement2D>();
        circleCollider2D = GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        // Ball�� ��/�� �浹�� ���
        if ( movement2D.IsCollision.up || movement2D.IsCollision.down )
        {
            movement2D.Bound(MoveType.UpDown);
            CollisionToBrick();
        }
        // Ball�� ��/�� �浹�� ���
        else if ( movement2D.IsCollision.left || movement2D.IsCollision.right )
        {
            movement2D.Bound(MoveType.LeftRight);
            CollisionToBrick();
        }
    }

    private void CollisionToBrick()
    {
        if ( movement2D.HitTransform.CompareTag("Brick") )
        {
            movement2D.HitTransform.GetComponent<Brick>().Collision();
        }
    }
}
