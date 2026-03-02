using UnityEngine;

namespace Pointo.Unit
{
    [RequireComponent(typeof(UnitTargetHandler))]
    public class PeonBehaviour : MonoBehaviour
    {
        [HideInInspector] public Resource.Resource targetResource;
        
        private UnitTargetHandler targetHandler;
        private Resource.Resource currentResource;

        private void Start()
        {
            targetHandler = GetComponent<UnitTargetHandler>();
            targetHandler.OnObjectReached = HandleResourceReached;
            targetHandler.OnJobCancelled = CancelJob;
        }

        private void HandleResourceReached(GameObject targetObject)
        {
            var resource = targetObject.GetComponent<Resource.Resource>();

            if (resource == null || targetResource == null || targetResource != resource) return;

            StartCollecting(resource);
            targetResource = null;
        }

        private void CancelJob()
        {
            if (currentResource == null) return;
            
            currentResource.onResourceCollected -= OnResourceCollected;
            currentResource.StopGatheringResource();
            currentResource = null;
        }

        private void StartCollecting(Resource.Resource resource)
        {
            currentResource = resource;
            currentResource.onResourceCollected += OnResourceCollected;
            currentResource.StartGatheringResource();
        }

        private void OnResourceCollected(Resource.Resource resource)
        {
            CancelJob();
        }
    }
}