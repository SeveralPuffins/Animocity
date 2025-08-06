using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Scenes.BattleScene.Scripts
{
    public class BattleMechInputController : MonoBehaviour
    {
        private BattleControls _controls;
        [SerializeField] private BattleMechMovementController movementController;
        [SerializeField] private BattleMechWeaponController primaryWeaponController;

        private void Awake()
        {
            _controls = new BattleControls();
        }

        private void OnEnable()
        {
            _controls.Enable();
            _controls.BattleActions.Movement.performed += MovementOnPerformed;
            _controls.BattleActions.Movement.canceled += MovementOnCancel;
            _controls.BattleActions.PrimaryWeapon.performed += PrimaryFireOnPerformed;
            _controls.BattleActions.PrimaryWeapon.canceled += PrimaryFireOnCancel;
        }
        private void OnDisable()
        {
            _controls.BattleActions.Movement.performed -= MovementOnPerformed;
            _controls.BattleActions.Movement.canceled -= MovementOnCancel;
            _controls.BattleActions.PrimaryWeapon.performed -= PrimaryFireOnPerformed;
            _controls.BattleActions.PrimaryWeapon.canceled -= PrimaryFireOnCancel;
        }

        private void MovementOnCancel(InputAction.CallbackContext context)
        {
            movementController.StopMoving();
        }

        private void MovementOnPerformed(InputAction.CallbackContext context)
        {
            movementController.Move(context.ReadValue<Vector2>());
        }

        private void PrimaryFireOnPerformed(InputAction.CallbackContext obj)
        {
            primaryWeaponController.FireProjectile();
        }

        private void PrimaryFireOnCancel(InputAction.CallbackContext obj)
        {
        }
        
        
    }
}