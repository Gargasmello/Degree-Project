using UnityEngine;

namespace Pointo.Resource
{
    [CreateAssetMenu(fileName = "Resource_", menuName = "Resource/New Resource", order = 0)]
    public class ResourceSO : ScriptableObject
    {
        public string resourceName;
        public ResourceType resourceType;
        public Material mat;

        [Tooltip("How long does it take to get this resource")]
        public float timeToFinish = 5f;

        [Tooltip("Particles to show then it's collected")]
        public GameObject collectFX;

        [Tooltip("How much does this resource give?")]
        public float amountToCollect = 1f;
    }
}