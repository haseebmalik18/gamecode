using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class JokerMovement : MonoBehaviour
{
    public float speed = 3f;
    public float growAmount = 0.1f;
    public float growInterval = 1f;
    public float maxSize = 2f;

    public GameObject jokerPrefab;
    public GameObject policePrefab;
    public TextMeshProUGUI scoreText;
    public int pointsPerPolice = 15;
    public int maxPolice = 3;

    SpriteRenderer spriteRenderer;

    static int score;
    static int policeCount;
    static List<GameObject> policeOfficers = new List<GameObject>();
    static List<float> usedYPositions = new List<float> { -5f, -3f, -1f };

    Vector2 dir = Vector2.right;
    bool isPopping = false;

    void Awake()
    {
        if (FindObjectsOfType<JokerMovement>().Length == 1)
        {
            score = 0;
            policeCount = 0;
            
            foreach (GameObject p in policeOfficers)
            {
                if (p != null) Destroy(p);
            }
            policeOfficers.Clear();
            usedYPositions = new List<float> { -5f, -3f, -1f };
        }
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (policeCount == 0)
        {
            GameObject[] existingPolice = GameObject.FindGameObjectsWithTag("Police");
            if (existingPolice.Length == 0)
            {
                SpawnPolice(-4f, 1f);
            }
            else
            {
                foreach (GameObject p in existingPolice)
                {
                    policeOfficers.Add(p);
                    float y = Mathf.Round(p.transform.position.y);
                    usedYPositions.Remove(y);
                }
            }
            policeCount = Mathf.Max(1, policeOfficers.Count);
            
            foreach (GameObject cop in policeOfficers)
            {
                if (cop != null)
                    cop.GetComponent<PoliceMovement>().SetSpeedMultiplier(policeCount);
            }
        }

        InvokeRepeating(nameof(Grow), growInterval, growInterval);
        scoreText.text = score.ToString();
    }

    void SpawnPolice(float startX, float direction)
    {
        if (usedYPositions.Count == 0) return;

        int index = Random.Range(0, usedYPositions.Count);
        float yPos = usedYPositions[index];
        usedYPositions.RemoveAt(index);

        GameObject p = Instantiate(policePrefab);
        PoliceMovement pm = p.GetComponent<PoliceMovement>();
        pm.SetPosition(startX, yPos, direction);
        pm.SetSpeedMultiplier(policeCount);
        policeOfficers.Add(p);

        foreach (GameObject cop in policeOfficers)
        {
            if (cop != null)
                cop.GetComponent<PoliceMovement>().SetSpeedMultiplier(policeCount + 1);
        }
    }

    void Update()
    {
        if (isPopping) return;

        transform.Translate(dir * speed * Time.deltaTime);

        if (transform.position.x > 8f) dir = Vector2.left;
        if (transform.position.x < -8f) dir = Vector2.right;
    }

    void Grow()
    {
        if (isPopping) return;

        if (transform.localScale.x < maxSize)
            transform.localScale += Vector3.one * growAmount;
        else
            Respawn(false);
    }

    public void Pop()
    {
        if (isPopping) return;
        isPopping = true;

        float size = transform.localScale.x;
        int gained = Mathf.RoundToInt(size * 2);
        score += gained;
        scoreText.text = score.ToString();

        int expectedPolice = 1 + (score / pointsPerPolice);
        while (policeCount < expectedPolice && policeCount < maxPolice)
        {
            float startX = Random.Range(-6f, 6f);
            float direction = Random.value > 0.5f ? 1f : -1f;
            SpawnPolice(startX, direction);
            policeCount++;
            
            foreach (GameObject cop in policeOfficers)
            {
                if (cop != null)
                    cop.GetComponent<PoliceMovement>().SetSpeedMultiplier(policeCount);
            }
        }

        CancelInvoke(nameof(Grow));
        StartCoroutine(PopAnimation());
    }

    IEnumerator PopAnimation()
    {
        Vector3 originalScale = transform.localScale;
        Color originalColor = spriteRenderer.color;

        for (int i = 0; i < 3; i++)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.05f);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(0.05f);
        }

        float duration = 0.3f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
            transform.Rotate(0, 0, 720 * Time.deltaTime);
            yield return null;
        }

        Respawn(true);
    }

    void Respawn(bool scored)
    {
        Vector3 newPos = new Vector3(Random.Range(-7f, 7f), Random.Range(0f, 4f), 0);
        GameObject j = Instantiate(jokerPrefab, newPos, Quaternion.identity);
        j.transform.localScale = Vector3.one;
        Destroy(gameObject);
    }
}
