using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scenes.BattleScene.Scripts
{
    public class BattleMechInputController : MonoBehaviour
    {
        private BattleControls _controls;
        [SerializeField] private BattleMechMovementController _movementController;

        private void Awake()
        {
            _controls = new BattleControls();
        }

        private void OnEnable()
        {
            _controls.Enable();
            _controls.BattleActions.Movement.performed += MovementOnPerformed;
            _controls.BattleActions.Movement.canceled += MovementOnCancel;
        }
        private void OnDisable()
        {
            _controls.BattleActions.Movement.performed -= MovementOnPerformed;
            _controls.BattleActions.Movement.canceled -= MovementOnCancel;
        }

        private void MovementOnCancel(InputAction.CallbackContext context)
        {

            _movementController.StopMoving();
        }

        private void MovementOnPerformed(InputAction.CallbackContext context)
        {
            _movementController.Move(context.ReadValue<Vector2>());
        }
    }
}