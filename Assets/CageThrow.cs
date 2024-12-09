using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CageThrow : MonoBehaviour
{
    public Rigidbody Cage;
    public float throwPower;
    public Transform cameraTransform;

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Rigidbody cage = Instantiate(Cage, transform.position, Quaternion.identity);

            cage.AddForce(cameraTransform.forward * throwPower, ForceMode.Impulse);
        }
    }
}
