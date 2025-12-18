using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightEnabler : MonoBehaviour
{
    public Transform anchorPoint;

    private GameObject lightGameObject;
    private Transform lightTransform;

    private void Start()
    {
        GameObject lightGameObject = new GameObject("CompensatorLight");
        Light compensatorLight = lightGameObject.AddComponent<Light>();
        compensatorLight.type = UnityEngine.LightType.Directional;
        compensatorLight.intensity = 2f;
        compensatorLight.shadows = LightShadows.Hard;

        compensatorLight.useColorTemperature = true;
        compensatorLight.colorTemperature = 5000f;
        compensatorLight.color = Color.white;



        lightGameObject.transform.LookAt(anchorPoint);

        Vector3 midpoint = (transform.position + anchorPoint.position) / 2;

        Vector3 direction = (transform.position - anchorPoint.position);


        lightGameObject.transform.position = midpoint + (direction * 0.35f);

        lightGameObject.transform.SetParent(transform);


        // Set Culling Mask to ONLY affect the planet's layer
        compensatorLight.cullingMask = 1 << this.gameObject.layer;

        lightTransform = lightGameObject.transform;
    }
    private void Update()
    {
        Vector3 lightPosition = lightTransform.position;

        Vector3 inverseDirection = lightPosition - anchorPoint.position;

        lightTransform.rotation = Quaternion.LookRotation(inverseDirection);
    }
}
