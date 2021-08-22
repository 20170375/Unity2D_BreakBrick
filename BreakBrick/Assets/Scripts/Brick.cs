using UnityEngine;

public class Brick : MonoBehaviour
{
    [SerializeField]
    private int            life = 1;
    [SerializeField]
    private Color[]        colors;
    [SerializeField]
    private GameObject     brickEffect;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer       = GetComponent<SpriteRenderer>();
        spriteRenderer.color = colors[life];
    }

    private void Break()
    {
        Instantiate(brickEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    public void Collision()
    {
        // life ����
        life--;
        GameObject clone = Instantiate(brickEffect, transform.position, Quaternion.identity);
        clone.GetComponent<ParticleSystem>().startColor = colors[life+1];

        // life�� 0�� �Ǹ� �ı�
        if ( life == 0 ) { Break(); }

        // Brick ���� life�� ���� ����
        spriteRenderer.color = colors[life];
    }
}
