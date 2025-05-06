using NUnit.Framework;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using System.Collections.Generic;

public class GridRayInteractor : MonoBehaviour
{
    [SerializeField]
    LayerMask gridLayer;
    [SerializeField]
    GridManager gridManager;

    LineRenderer lineRenderer;
    Vector3[] lineRendPositions = new Vector3[2];
    [SerializeField]
    Material canPlaceColour;
    [SerializeField]
    Material cannotPlaceColour;

    [SerializeField]
    GameObject heldComponent;
    [SerializeField]
    List<GameObject> allComponents = new List<GameObject>();

    [Header("Right Trigger")]
    [SerializeField]
    XRInputValueReader<float> rightTrigger;
    [SerializeField]
    float rightTriggerValue;

    bool triggerDown;

    GameObject ghostComponent;


    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        if(gridManager == null)
            gridManager = FindFirstObjectByType<GridManager>();   
    }

    void Update()
    {
        GetRightTriggerValue();
        lineRendPositions[0] = transform.position;
    }

    void LateUpdate()
    {
        rightTriggerValue = GetRightTriggerValue();

        if (rightTriggerValue == 1 && !triggerDown)
        {
            triggerDown = true;
        }
        else
        {
            triggerDown = false;
        }

        if (heldComponent != null)
        {
            CastRayToGrid();
        }
    }

    float GetRightTriggerValue()
    {
        return rightTrigger.ReadValue();
    }

    void CastRayToGrid()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 50, gridLayer))
        {
            //if(!lineRenderer.enabled) lineRenderer.enabled = true;

            if (ghostComponent == null) 
            {
                ghostComponent = gridManager.PlaceGhostComponent(heldComponent, hit.point, Quaternion.identity);
            }
            else
            {
                ghostComponent.transform.position = gridManager.CalculateGridPosition(hit.point);
            }

            if (triggerDown)
            {
                gridManager.TryPlaceComponent(heldComponent, hit.point, Quaternion.identity);
            }

            lineRendPositions[1] = hit.point;

            if (gridManager.CheckFreeSquare(gridManager.CalculateGridPosition(hit.point)))
            {
                lineRenderer.material = canPlaceColour;
            }
            else
            {
                lineRenderer.material = cannotPlaceColour;
            }
        }
        else
        {
            //if (lineRenderer.enabled) lineRenderer.enabled = false;
            lineRendPositions[1] = transform.TransformDirection(Vector3.forward) * 5;
            lineRenderer.material = cannotPlaceColour;
        }

        lineRenderer.SetPositions(lineRendPositions);
    }

    void ChangeHeldItem()
    {
        GameObject.Destroy(ghostComponent);
        ghostComponent = null;
    }
}
