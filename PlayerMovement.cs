using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public GameObject pinPrefab;
    public float throwCooldown = 0.4f;

    Rigidbody2D rb;
    SpriteRenderer sr;

    float moveX;
    float moveY;
    Vector3 originalScale;
    float nextThrowTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;
    }

    void Update()
    {
        moveX = Input.GetAxisRaw("Horizontal");
        moveY = Input.GetAxisRaw("Vertical");

        if (moveX > 0) sr.flipX = false;
        else if (moveX < 0) sr.flipX = true;

        if ((Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Space)) && Time.time >= nextThrowTime)
        {
            Instantiate(pinPrefab, transform.position, Quaternion.identity);
            StartCoroutine(ThrowAnimation());
            nextThrowTime = Time.time + throwCooldown;
        }
    }

    IEnumerator ThrowAnimation()
    {
        transform.localScale = new Vector3(originalScale.x * 1.3f, originalScale.y * 0.7f, originalScale.z);
        yield return new WaitForSeconds(0.05f);

        transform.localScale = new Vector3(originalScale.x * 0.8f, originalScale.y * 1.2f, originalScale.z);
        yield return new WaitForSeconds(0.05f);

        transform.localScale = originalScale;
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
