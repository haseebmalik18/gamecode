using UnityEngine;

public class BatarangMovement : MonoBehaviour
{
    public float baseSpeed = 8f;
    public float speedBoostPerPolice = 1f;
    float speed;

    void Start()
    {
        GameObject[] police = GameObject.FindGameObjectsWithTag("Police");
        speed = baseSpeed + (police.Length * speedBoostPerPolice);
    }

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
