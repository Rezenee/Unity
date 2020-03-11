using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    [SerializeField] private Gun currentGun;

    private Transform cameraTransform;
    private void Start()
    {
        cameraTransform = Camera.main.transform;
    }
    private void gpdate()
    {
        CheckForShooting();
    }

    private void CheckForShooting()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit whatIHit;
            if (Physics.Raycast(cameraTransform.position, transform.forward, out whatIHit, Mathf.Infinity))
            {
                IDamageable damageable = whatIHit.collider.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.DealDamage(currentGun.maximumDamage);
                }
            }
        }
    }
}
