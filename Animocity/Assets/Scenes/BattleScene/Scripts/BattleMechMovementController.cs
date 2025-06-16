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
            // direction.
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