using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Pointo.Resource
{
    public class UserResourceManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI treeText;
        [SerializeField] private TextMeshProUGUI ironText;

        private Dictionary<int, Resource> collectedResources;

        private void Start()
        {
            collectedResources = new Dictionary<int, Resource>();
        }

        private void OnEnable()
        {
            Resource.onGlobalResourceCollected += OnResourceCollected;
        }

        private void OnDisable()
        {
            Resource.onGlobalResourceCollected -= OnResourceCollected;
        }

        private void OnResourceCollected(Resource resource)
        {
            if (collectedResources.ContainsKey(resource.gameObject.GetInstanceID())) return;

            collectedResources.Add(resource.gameObject.GetInstanceID(), resource);
            UpdateUI(resource);
        }

        private void UpdateUI(Resource resource)
        {
            float amount = collectedResources
                .Count(res => res.Value.ResourceType == resource.ResourceType);

            // we get the proper amount
            amount *= resource.AmountToCollect;

            switch (resource.ResourceType)
            {
                case ResourceType.Wood:
                    treeText.text = "Trees: " + amount;
                    break;
                case ResourceType.Iron:
                    ironText.text = "Iron: " + amount;
                    break;
                case ResourceType.Fish:
                    break;
                case ResourceType.Coal:
                    break;
                case ResourceType.Gold:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}