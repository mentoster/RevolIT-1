using UnityEngine;
namespace Bhaptics.Tact.Unity
{
    public class BhapticConnect : MonoBehaviour
    {
        HapticSender _tactSender;
        [HideInInspector]
        public Transform shootingPoint;
        void Start()
        {
            _tactSender = GetComponent<HapticSender>();
        }
        public void Play(RaycastHit raycastHit)
        {
            var detect = raycastHit.collider.gameObject.GetComponent<HapticReceiver>();
            var pos = PositionTag.Default;
            if (detect == null)
            {
                ///// THIS IS ONLY FOR DEMO CASE.
                var custom = raycastHit.collider.gameObject.GetComponent<BhapticsCustomHapticReceiver>();
                if (custom != null)
                {
                    custom.ReflectHandle(raycastHit.point, _tactSender);
                    return;
                }
            }
            else
                pos = detect.PositionTag;
            if (_tactSender != null)
            {
                print($"Send effect to {raycastHit}, pos = {pos}");
                _tactSender.Play(pos, raycastHit);
            }
        }
    }
}
