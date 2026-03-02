using System;
using UnityEngine;

namespace Pointo.Resource
{
    [RequireComponent(typeof(MeshRenderer))]
    public class Resource : MonoBehaviour
    {
        // Used by [UserResourceManager] for UI purposes. 
        // Also can be used by an inventory system or something more global/general
        public static Action<Resource> onGlobalResourceCollected = delegate { };
        [SerializeField] private ResourceSO resourceSO;
        private bool hasBeenCollected;

        public Action<float> onProgressChanged = delegate { };

        // Used by the [Unit] to know when to stop working
        public Action<Resource> onResourceCollected = delegate { };
        private float timeRemaining;

        private bool timerIsRunning;
        private float totalTime;
        private int workersOnResource;

        public Color ResourceColor => resourceSO.mat.color;

        public ResourceType ResourceType => resourceSO.resourceType;

        public float AmountToCollect => resourceSO.amountToCollect;

        private void Start()
        {
            timeRemaining = resourceSO.timeToFinish;
            totalTime = timeRemaining;

            if (resourceSO.mat != null) GetComponent<MeshRenderer>().material = resourceSO.mat;
        }

        private void Update()
        {
            if (timerIsRunning)
            {
                if (timeRemaining > 0)
                {
                    timeRemaining -= Time.deltaTime * workersOnResource;

                    var progressMade = 1 - timeRemaining / totalTime;
                    onProgressChanged(progressMade);
                }
                else
                {
                    if (hasBeenCollected) return;

                    hasBeenCollected = true;

                    OnCollect();
                }
            }
        }

        private void OnCollect()
        {
            if (resourceSO.collectFX != null)
                Instantiate(resourceSO.collectFX, transform.position, Quaternion.identity);

            onResourceCollected(this);
            onGlobalResourceCollected(this);
            timeRemaining = 0;
            timerIsRunning = false;

            Destroy(gameObject);
        }

        public void StartGatheringResource()
        {
            timerIsRunning = true;
            workersOnResource++;
        }

        public void StopGatheringResource()
        {
            timerIsRunning = false;
            workersOnResource = 0;
            // we save the last timer so we don't start from scratch
            timeRemaining = resourceSO.timeToFinish - timeRemaining;
        }
    }
}