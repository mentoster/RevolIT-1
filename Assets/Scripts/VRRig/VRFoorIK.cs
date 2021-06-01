using UnityEngine;
public class VRFootIK : MonoBehaviour
{
    public Vector3 footOffset;
    private Animator animator;
    [Range(0,1)]
    public float footPosWeight = 1;
    [Range(0, 1)]
    public float footRotWeight = 1;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        SetFootIK(AvatarIKGoal.RightFoot);
        SetFootIK(AvatarIKGoal.LeftFoot);
    }

    private void SetFootIK(AvatarIKGoal foot)
    {
        Vector3 footPos = animator.GetIKPosition(foot);
        RaycastHit hit;

        bool hasHit = Physics.Raycast(footPos + Vector3.up, Vector3.down, out hit);
        if (hasHit)
        {
            animator.SetIKPositionWeight(foot, footPosWeight);
            animator.SetIKPosition(foot, hit.point + footOffset);
            Quaternion footRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, hit.normal), hit.normal);
            animator.SetIKRotationWeight(foot, footRotWeight);
            animator.SetIKRotation(foot, footRotation);
        }
        else
        {
            animator.SetIKPositionWeight(foot, 0);
        }
    }
}
