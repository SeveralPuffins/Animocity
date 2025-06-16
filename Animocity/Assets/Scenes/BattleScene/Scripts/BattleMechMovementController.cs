using System;
using UnityEngine;

namespace Scenes.BattleScene
{
    public class BattleMechMovementController : MonoBehaviour
    {
        [SerializeField] Rigidbody rigidbody;
        [SerializeField] float moveSpeed;
        private Vector3 _movement;
        
        public void Move(Vector2 direction)
        {
            // mech only walks back and forwards, so discard the Y-Axis for movement
            // possibly use Y-Axis for pitching?
            _movement = new Vector3(direction.x, 0, 0);
        }

        public void StopMoving()
        {
            _movement = Vector2.zero;
        }

        private void Update()
        {
            rigidbody.MovePosition(rigidbody.position + _movement * (moveSpeed * Time.fixedDeltaTime));
        }
    }
}