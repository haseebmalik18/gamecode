using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public GameObject pinPrefab;

    Rigidbody2D rb;
    SpriteRenderer sr;

    float moveX;
    float moveY;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        moveX = Input.GetAxisRaw("Horizontal");
        moveY = Input.GetAxisRaw("Vertical");

        if (moveX > 0) sr.flipX = false;
        else if (moveX < 0) sr.flipX = true;

        if (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Space))
            Instantiate(pinPrefab, transform.position, Quaternion.identity);
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveX * moveSpeed, moveY * moveSpeed);

        float maxY = -7f;
        float minY = -8f;

        if (transform.position.y > maxY)
            transform.position = new Vector3(transform.position.x, maxY, transform.position.z);

        if (transform.position.y < minY)
            transform.position = new Vector3(transform.position.x, minY, transform.position.z);
    }
}
