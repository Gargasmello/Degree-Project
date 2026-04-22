using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Pointo.Unit;
using System.Linq;
using UnityEngine.UI;

public class HealthUiController : MonoBehaviour
{
    [SerializeField] private GameObject segment;
    [SerializeField] private float health;
    [SerializeField] private List<GameObject> segments;

    private void OnEnable()
    {
        Unit.OnHurt += UpdateHealthUi;
    }

    private void OnDisable()
    {
        Unit.OnHurt -= UpdateHealthUi;
    }

    private void Start()
    {
        for (int i = 0; i < health; i++)
        {
            var currentSegment = Instantiate(segment);
            currentSegment.transform.SetParent(transform, false);
            segments.Add(currentSegment);
        }
    }

    private void UpdateHealthUi()
    {
        float health = GetComponentInParent<Unit>().Health;
        Debug.Log($"Health: {health}");
        foreach (var segment in segments)
        {
            segment.GetComponent<Image>().enabled = false;

        }
        
        for (int i = 0; i < health; i++)
        {
            segments[i].GetComponent<Image>().enabled = true;
        }
    }

}
