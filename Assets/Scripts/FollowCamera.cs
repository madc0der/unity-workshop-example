using Unity.Mathematics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace DefaultNamespace
{
    public class FollowCamera : MonoBehaviour
    {
        public Transform player;
        public Vector3 cameraOffset = new Vector3(0f, 2f, -5f);
        public Vector3 playerOffset = new Vector3(0, 1f, 0);
        public float positionLerpValue;
        
        private Transform cameraTransform;

        private void Start()
        {
            cameraTransform = GetComponent<Camera>().transform;
            cameraTransform.position = player.position + cameraOffset;
            cameraTransform.rotation = Quaternion.LookRotation(
                (player.position - cameraTransform.position).normalized,
                Vector3.up
            );
        }

        private void LateUpdate()
        {
            var playerForward = player.forward;
            var playerRight = player.right;
            var playerPosition = player.position 
                                 + playerForward * playerOffset.z
                                 + Vector3.up * playerOffset.y
                                 + playerRight * playerOffset.x;
            var cameraPosition = cameraTransform.position;
            var targetPosition = playerPosition
                                 + playerForward * cameraOffset.z
                                 + Vector3.up * cameraOffset.y
                                 + playerRight * cameraOffset.x;
            cameraTransform.position = Vector3.Lerp(cameraPosition, targetPosition, positionLerpValue);
            cameraTransform.rotation =
                quaternion.LookRotation((playerPosition - cameraTransform.position).normalized, Vector3.up);
/*
                Quaternion.Slerp(
                cameraTransform.rotation,
                quaternion.LookRotation((targetPosition - cameraPosition).normalized, Vector3.up), 
                    positionLerpValue);
*/
        }
    }
}