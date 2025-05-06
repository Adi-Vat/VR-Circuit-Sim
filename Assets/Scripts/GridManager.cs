using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    [SerializeField]
    int gridSpacing;
    [SerializeField]
    Material ghostMaterial;

    List<Vector3> usedPoints = new List<Vector3>();

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public bool TryPlaceComponent(GameObject component, Vector3 RayHit, Quaternion targetRotation)
    {
        Vector3 roundedHit = CalculateGridPosition(RayHit);

        if(!CheckFreeSquare(roundedHit)) return false;

        // Spawn component there
        Instantiate(component, roundedHit, targetRotation, transform);
        usedPoints.Add(roundedHit);
        // Return true if possible to spawn component
        return true;
    }

    public GameObject PlaceGhostComponent(GameObject component, Vector3 RayHit, Quaternion targetRotation)
    {
        Vector3 roundedHit = CalculateGridPosition(RayHit);
        // Spawn component there
        GameObject ghostComponent = Instantiate(component, roundedHit, Quaternion.identity);
        ghostComponent.GetComponentInChildren<MeshRenderer>().material = ghostMaterial;
        return ghostComponent;
    }

    public Vector3 CalculateGridPosition(Vector3 inputPosition)
    {
        Vector3 roundedHit = Vector3.zero;
        // Find nearest grid point to RayHit
        roundedHit.x = Mathf.Round(inputPosition.x / gridSpacing) * gridSpacing;
        roundedHit.z = Mathf.Round(inputPosition.z / gridSpacing) * gridSpacing;
        return roundedHit;
    }

    public bool CheckFreeSquare(Vector3 roundedHit)
    {
        // If something is already at that point, you can't spawn anything there.
        if (usedPoints.Contains(roundedHit)) return false;
        else return true;
    }
}
