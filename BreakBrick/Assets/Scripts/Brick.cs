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
        // life 감소
        life--;
        GameObject clone = Instantiate(brickEffect, transform.position, Quaternion.identity);
        clone.GetComponent<ParticleSystem>().startColor = colors[life+1];

        // life가 0이 되면 파괴
        if ( life == 0 ) { Break(); }

        // Brick 색상 life에 따라 변경
        spriteRenderer.color = colors[life];
    }
}
