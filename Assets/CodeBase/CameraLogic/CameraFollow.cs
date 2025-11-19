using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CodeBase.CameraLogic
{
    public class CameraFollow: MonoBehaviour
    {
        public float RotationAngelX;
        public float Distance;
        public float OffSetY;

        [SerializeField]
        private Transform _following;
        
        private void LateUpdate()
        {
            if (_following == null) return;

            Quaternion rotation = Quaternion.Euler(RotationAngelX, 0f, 0f);
            
            var position = rotation * new Vector3(0, 0, -Distance) + FollowingPointPosition();

            transform.rotation = rotation;
            transform.position = position;

        }

        public void Follow(GameObject following) =>
            _following = following.transform;
        

        private Vector3 FollowingPointPosition()
        {
            Vector3 followingPosition = _following.position;
            followingPosition.y += OffSetY;

            return followingPosition;
        }
    }
}
