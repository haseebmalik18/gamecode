using UnityEngine;
using TMPro;

public class JokerMovement : MonoBehaviour
{
    public float speed = 3f;
    public float growAmount = 0.1f;
    public float growInterval = 1f;
    public float maxSize = 3f;

    public GameObject jokerPrefab;
    public GameObject policePrefab;
    public TextMeshProUGUI scoreText;

    static int score;
    Vector2 dir = Vector2.right;
    GameObject myPolice;

    void Start()
    {
        myPolice = Instantiate(policePrefab);
        myPolice.GetComponent<PoliceMovement>().SetTarget(transform);
        InvokeRepeating(nameof(Grow), growInterval, growInterval);
        scoreText.text = score.ToString();
    }

    void Update()
    {
        transform.Translate(dir * speed * Time.deltaTime);

        if (transform.position.x > 8f) dir = Vector2.left;
        if (transform.position.x < -8f) dir = Vector2.right;
    }

    void Grow()
    {
        if (transform.localScale.x < maxSize)
            transform.localScale += Vector3.one * growAmount;
        else
            Respawn(false);
    }

    public void Pop()
    {
        float size = transform.localScale.x;
        int gained = Mathf.RoundToInt(Mathf.Clamp(40f / size, 5f, 40f));
        score += gained;
        scoreText.text = score.ToString();
        Respawn(true);
    }

    void Respawn(bool scored)
    {
        Vector3 newPos = new Vector3(Random.Range(-8f, 8f), Random.Range(-3f, 3f), 0);
        GameObject j = Instantiate(jokerPrefab, newPos, Quaternion.identity);
        j.transform.localScale = Vector3.one;
        Destroy(myPolice);
        Destroy(gameObject);
    }
}
