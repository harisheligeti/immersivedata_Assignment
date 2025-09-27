using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ProgressBarUI : MonoBehaviour
{
    public Image fillImage;

    public void Show(float duration)
    {
        gameObject.SetActive(true);
        StartCoroutine(FillRoutine(duration));
    }

    private IEnumerator FillRoutine(float duration)
    {
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            fillImage.fillAmount = t / duration;
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
