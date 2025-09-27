using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text scoreText;
    private int score = 0;

    [Header("Currency Flight")]
    public GameObject coinPrefab; 
    public RectTransform coinStartParent; 
    public RectTransform scoreTarget; 
    public Camera mainCamera;

    private void Awake()
    {
        UpdateScoreText();
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText) scoreText.text = $"${score}";
    }

    public void SpawnFlyingCurrency(Vector3 worldPosition, int amount)
    {
        StartCoroutine(FlyCoins(worldPosition, amount));
    }

    private IEnumerator FlyCoins(Vector3 worldPosition, int amount)
    {
        int pieces = Mathf.Clamp(amount, 1, 10);
        for (int i = 0; i < pieces; i++)
        {
            var coinGO = Instantiate(coinPrefab, coinStartParent);
            var rt = coinGO.GetComponent<RectTransform>();
            // set position from world to canvas
            Vector2 screenPoint = mainCamera.WorldToScreenPoint(worldPosition);
            rt.position = screenPoint;

            StartCoroutine(MoveRectTransform(rt, scoreTarget.position, 0.6f + i * 0.05f));
            yield return new WaitForSeconds(0.05f);
        }
    }

    private IEnumerator MoveRectTransform(RectTransform rt, Vector3 targetScreenPos, float duration)
    {
        Vector3 start = rt.position;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float p = t / duration;
            rt.position = Vector3.Lerp(start, targetScreenPos, Mathf.SmoothStep(0f, 1f, p));
            yield return null;
        }
        Destroy(rt.gameObject);
    }
}
