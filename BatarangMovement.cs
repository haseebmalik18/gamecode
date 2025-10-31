using UnityEngine;

public class BatarangMovement : MonoBehaviour
{
    public float speed = 10f;

    void Update()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Joker"))
        {
            other.GetComponent<JokerMovement>().Pop();
            Destroy(gameObject);
        }

        if (other.CompareTag("Police"))
        {
            Destroy(gameObject);
        }
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}