using UnityEngine;
/// <summary>
/// Matching the VR controllers and headset to their targets
/// </summary>
[System.Serializable]
public class VRMap
{
    [SerializeField]
    private Transform vrTarget, rigTarget;

    [SerializeField]
    public Vector3 trackingPositionOffSet, trackingRotationOffSet;

    /// <summary>
    /// Sets the position and rotation of the "rig target" to be the "VR target"
    /// </summary>
    public void Mapping()
    {
        rigTarget.position = vrTarget.TransformPoint(trackingPositionOffSet);
        rigTarget.rotation = vrTarget.rotation * Quaternion.Euler(trackingRotationOffSet);
    }
}

public class VRRig : MonoBehaviour
{
    [Header("VR objects")]
    [SerializeField]
    private VRMap head;
    [SerializeField]
    private VRMap leftHand;
    [SerializeField]
    private VRMap rightHand;

    [Space]

    [SerializeField]
    private Transform headConstraint;

    private Vector3 headBodyOffSet;
    [SerializeField]
    private float turnSmoothness;


    void Start()
    {
        headBodyOffSet = transform.position - headConstraint.position;
    }

    void LateUpdate()
    {
        transform.position = headConstraint.position + headBodyOffSet;
        transform.forward = Vector3.Lerp(transform.forward,
            Vector3.ProjectOnPlane(headConstraint.up, Vector3.up).normalized, Time.deltaTime * turnSmoothness);

        head.Mapping();
        leftHand.Mapping();
        rightHand.Mapping();
    }
}
