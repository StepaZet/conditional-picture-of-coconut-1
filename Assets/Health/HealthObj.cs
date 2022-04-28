using UnityEngine;

namespace Health
{
    [RequireComponent(typeof(HealthObj))]
    public class HealthObj : MonoBehaviour
    {
        public HealthSystem Health { get; set; }
        public void OnEnable()
        {
            Health = new HealthSystem(10);
        }
    }
}
