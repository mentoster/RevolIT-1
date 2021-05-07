using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
namespace Bhaptics.Tact.Unity
{
    public class BhapticConnect : MonoBehaviour
    {
        HapticSender _tactSender;
        public Transform shootingPoint;
        void Start()
        {
            _tactSender = GetComponent<HapticSender>();
        }

        void Shoot()
        {
            var targetPosition = shootingPoint.position;
            var direction = shootingPoint.forward;
            var length = 10f;
            Ray ray = new Ray(targetPosition, direction);
            RaycastHit raycastHit;
            Vector3 endPosition = targetPosition + (length * direction);
            if (Physics.Raycast(ray, out raycastHit, length))
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
                {
                    pos = detect.PositionTag;
                }
                if (_tactSender != null)
                {
                    _tactSender.Play(pos, raycastHit);
                }
            }
        }
    }
}
