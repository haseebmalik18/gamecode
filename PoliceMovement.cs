using UnityEngine;

public class PoliceMovement : MonoBehaviour
{
    public Transform joker;
    public float speed = 4.5f;
    public float offset = 2f;
    float dir = 1f;

    public void SetTarget(Transform t)
    {
        joker = t;
        transform.position = new Vector3(joker.position.x, joker.position.y - offset, 0);
    }

    void Update()
    {
        if (joker == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.position += new Vector3(dir * speed * Time.deltaTime, 0, 0);

        if (transform.position.x > 7f) dir = -1f;
        if (transform.position.x < -7f) dir = 1f;

        transform.position = new Vector3(transform.position.x, joker.position.y - offset, 0);
    }
}
