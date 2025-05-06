using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;

public class HandAnimator : MonoBehaviour
{
    #region Variables
    enum Hand
    {
        Left,
        Right
    }

    const int chainLength = 3;

    [SerializeField]
    Hand hand;

    [Header("Hand Bones")]
    [Tooltip("The thumb's root bone")]
    [SerializeField]
    Transform thumbBase;

    [SerializeField]
    Transform indexFingerBase;

    [SerializeField]
    Transform middleFingerBase;

    [SerializeField]
    Transform ringFingerBase;

    [SerializeField]
    Transform pinkyFingerBase;


    Quaternion[] relaxedIndexFingerRotations = new Quaternion[chainLength];
    Quaternion[] relaxedOtherFingerRotations = new Quaternion[chainLength];
    Quaternion[] relaxedThumbRotations = new Quaternion[chainLength];

    [Header("Contracted rotations for each finger [Base -> Middle -> End]")]
    [SerializeField]
    Vector3[] contractedIndexFingerRotations = new Vector3[chainLength];

    [SerializeField]
    Vector3[] contractedOtherFingerRotations = new Vector3[chainLength];

    [SerializeField]
    Vector3[] contractedThumbRotations = new Vector3[chainLength];

    Transform[] indexFingerBones = new Transform[chainLength];
    Transform[] middleFingerBones = new Transform[chainLength];
    Transform[] ringFingerBones = new Transform[chainLength];
    Transform[] pinkyFingerBones = new Transform[chainLength];
    Transform[] thumbBones = new Transform[chainLength];

    [Header("Grip")]
    [SerializeField]
    XRInputValueReader<float> m_GripInput = new XRInputValueReader<float>("Grip");
    [SerializeField]
    float gripValue;

    [Header("Trigger")]
    [SerializeField]
    XRInputValueReader<float> m_TriggerInput = new XRInputValueReader<float>("Trigger");
    [SerializeField]
    float triggerValue;

    [Header("Thumbstick")]
    [SerializeField]
    XRInputValueReader<float> m_StickHoverInput = new XRInputValueReader<float>("Primary2DAxisTouch");
    [SerializeField]
    float thumbstickHoverValue;


    [Header("Finger Angles")]
    Quaternion[] indexFingerAngles = new Quaternion[3];
    Quaternion[] otherFingerAngles = new Quaternion[3];
    Quaternion[] thumbAngles = new Quaternion[3];
    #endregion


    void Awake()
    {
        CacheBones();  
    }
    
    void CacheBones()
    {
        // Takes a root bone for each digit and automatically finds all of the child bones.
        // Start from the base bone
        Transform indexChild = indexFingerBase;
        Transform middleChild = middleFingerBase;
        Transform ringChild = ringFingerBase;
        Transform pinkyChild = pinkyFingerBase;
        Transform thumbChild = thumbBase;
        for (int i = 0; i < chainLength; i++)
        {
            // Set the current index to this child, storing it
            indexFingerBones[i] = indexChild;
            relaxedIndexFingerRotations[i] = indexChild.localRotation;
            // Get the next child
            indexChild = indexFingerBones[i].GetChild(0);

            middleFingerBones[i] = middleChild;
            relaxedOtherFingerRotations[i] = middleChild.localRotation;
            middleChild = middleFingerBones[i].GetChild(0);

            ringFingerBones[i] = ringChild;
            ringChild = ringFingerBones[i].GetChild(0);

            pinkyFingerBones[i] = pinkyChild;
            pinkyChild = pinkyFingerBones[i].GetChild(0);

            thumbBones[i] = thumbChild;
            relaxedThumbRotations[i] = thumbChild.localRotation;
            thumbChild = thumbBones[i].GetChild(0);
        }
    }

    void LateUpdate()
    {
        ReadTriggerInput();
        ReadGripInput();
        ReadThumbstickInput();
        ApplyAngles();
    }

    void ReadTriggerInput()
    {
        // Get a value from the triggers between 0 and 1
        triggerValue = m_TriggerInput.ReadValue();

        // Interpolate between the relaxed and contracted rotations based on this value
        for(int i = 0; i < chainLength; i++)
        {
            indexFingerAngles[i] = Quaternion.Lerp(relaxedIndexFingerRotations[i], Quaternion.Euler(contractedIndexFingerRotations[i]), triggerValue);
        }
    }
    
    void ReadGripInput()
    {
        gripValue = m_GripInput.ReadValue();
        for (int i = 0; i < chainLength; i++)
        {
            otherFingerAngles[i] = Quaternion.Lerp(relaxedOtherFingerRotations[i], Quaternion.Euler(contractedOtherFingerRotations[i]), gripValue);
        }
    }

    void ReadThumbstickInput()
    {
        thumbstickHoverValue = m_StickHoverInput.ReadValue();
        for (int i = 0; i < chainLength; i++)
        {
            thumbAngles[i] = Quaternion.Lerp(relaxedThumbRotations[i], Quaternion.Euler(contractedThumbRotations[i]), thumbstickHoverValue);
        }
    }

    void ApplyAngles()
    {
        // Loop through the all bones, from the base to the end
        for(int i = 0; i < chainLength; i++)
        {
            indexFingerBones[i].localRotation = indexFingerAngles[i];
            ApplyOtherFingerRotations(i, otherFingerAngles[i]);
            thumbBones[i].localRotation = thumbAngles[i];
        }
    }

    void ApplyOtherFingerRotations(int i, Quaternion angle)
    {
        middleFingerBones[i].localRotation = ringFingerBones[i].localRotation = pinkyFingerBones[i].localRotation = angle;
    }
}
