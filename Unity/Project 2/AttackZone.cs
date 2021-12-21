using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackZone : MonoBehaviour
{
    #region Fields

    Cannon cannon;
    SphereCollider zone;
    float radius;

    #endregion

    #region Methods

    public void Awake() {
        zone = GetComponent<SphereCollider>();
        cannon = GetComponentInParent<Cannon>();
    }

    void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    /// <summary>
    /// Change the size of the attack zone by changing the radius of the sphere collider
    /// </summary>
    /// <param name="Radius">radius of the sphere collider</param>
    public void SetRadius(float Radius) {
        radius = Radius;
        zone.radius = Radius;
    }

    void OnTriggerEnter(Collider collider) {
        if (collider.tag == "Unit") {
            cannon.OnTargetEntered(collider.transform);
        }
    }

    void OnTriggerLeave(Collider collider) {
        if (collider.tag == "Unit") {
            cannon.OnTargetLeft(collider.transform);
        }
    }

    #endregion
}
