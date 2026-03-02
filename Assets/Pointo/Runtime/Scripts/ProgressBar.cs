using System.Collections;
using Pointo.Resource;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Resource resource;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image foregroundImage;

    [SerializeField] private float updateSpeedSeconds = 0.2f;

    private bool isShown;

    private void Awake()
    {
        foregroundImage.color = resource.ResourceColor;
    }

    private void OnEnable()
    {
        resource.onProgressChanged += HandleProgress;
    }

    private void OnDisable()
    {
        resource.onProgressChanged -= HandleProgress;
    }

    private void HandleProgress(float progressPercentage)
    {
        if (!isShown)
        {
            var backgroundColor = backgroundImage.color;
            backgroundColor.a = 1f;
            backgroundImage.color = backgroundColor;
            isShown = true;
        }

        StartCoroutine(ChangePercentage(progressPercentage));
    }

    private IEnumerator ChangePercentage(float progressPercentage)
    {
        var prePercentage = foregroundImage.fillAmount;
        var elapsed = 0f;

        while (elapsed < updateSpeedSeconds)
        {
            elapsed += Time.deltaTime;
            foregroundImage.fillAmount = Mathf.Lerp(prePercentage, progressPercentage, elapsed / updateSpeedSeconds);
            yield return null;
        }

        foregroundImage.fillAmount = progressPercentage;
    }
}