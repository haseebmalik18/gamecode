using UnityEngine;

public class PoliceMovement : MonoBehaviour
{
    public float baseSpeed = 4f;
    public float speedIncreasePerPolice = 2f;
    float speed;
    float dir = 1f;
    float nextDirectionChange;
    bool speedSet = false;

    public void SetPosition(float startX, float yPos, float direction)
    {
        transform.position = new Vector3(startX, yPos, 0);
        dir = direction;
        SetRandomDirectionChangeTime();
    }

    public void SetSpeedMultiplier(int policeCount)
    {
        speed = baseSpeed + (policeCount * speedIncreasePerPolice);
        speedSet = true;
    }

    void Start()
    {
        if (!speedSet)
            speed = baseSpeed;
        SetRandomDirectionChangeTime();
    }

    void SetRandomDirectionChangeTime()
    {
        nextDirectionChange = Time.time + Random.Range(1f, 3f);
    }

    void Update()
    {
        transform.position += new Vector3(dir * speed * Time.deltaTime, 0, 0);

        if (transform.position.x > 8f) dir = -1f;
        if (transform.position.x < -8f) dir = 1f;

        if (Time.time >= nextDirectionChange)
        {
            dir *= -1f;
            SetRandomDirectionChangeTime();
        }
    }
}
