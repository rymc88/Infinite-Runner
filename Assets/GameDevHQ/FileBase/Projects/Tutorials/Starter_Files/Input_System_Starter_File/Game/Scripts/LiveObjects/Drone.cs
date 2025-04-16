using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;
using Game.Scripts.UI;
using UnityEngine.InputSystem;

namespace Game.Scripts.LiveObjects
{
    public class Drone : MonoBehaviour
    {
        private enum Tilt
        {
            NoTilt, Forward, Back, Left, Right
        }

        [SerializeField]
        private Rigidbody _rigidbody;
        [SerializeField]
        private float _speed = 5f;
        [SerializeField] float _rotateMultiplier = .15f;
        private bool _inFlightMode = false;
        [SerializeField]
        private Animator _propAnim;
        [SerializeField]
        private CinemachineCamera _droneCam;
        [SerializeField]
        private InteractableZone _interactableZone;
        

        public static event Action OnEnterFlightMode;
        public static event Action onExitFlightmode;


        private void OnEnable()
        {
            InteractableZone.onZoneInteractionComplete += EnterFlightMode;

        }

        private void Exit_performed()
        {
            if (_interactableZone.playerInput != null)
            {
                if (_interactableZone.playerInput.actions["Exit"].IsPressed())
                {
                    _inFlightMode = false;
                    onExitFlightmode?.Invoke();
                    ExitFlightMode();
                }

            }
        }

        private void EnterFlightMode(InteractableZone zone)
        {
            if (_inFlightMode != true && zone.GetZoneID() == 4) // drone Scene
            {
                _propAnim.SetTrigger("StartProps");
                _droneCam.Priority = 11;
                _inFlightMode = true;
                OnEnterFlightMode?.Invoke();
                UIManager.Instance.DroneView(true);
                _interactableZone.CompleteTask(4);
            }
        }

        private void ExitFlightMode()
        {            
            _droneCam.Priority = 9;
            _inFlightMode = false;
            UIManager.Instance.DroneView(false);            
        }

        private void Update()
        {
            if (_inFlightMode)
            {
                CalculateTilt();
                CalculateMovementUpdate();
                Exit_performed();
            }
        }

        private void FixedUpdate()
        {
            _rigidbody.AddForce(transform.up * (9.81f), ForceMode.Acceleration);
            if (_inFlightMode)
                CalculateMovementFixedUpdate();
        }

        private void CalculateMovementUpdate()
        {
            if(_interactableZone.playerInput != null)
            {
                float yaw = _interactableZone.playerInput.actions["Yaw"].ReadValue<float>();

                if (yaw < 0)
                {
                    var tempRot = transform.localRotation.eulerAngles;
                    tempRot.y -= _speed * _rotateMultiplier;
                    transform.localRotation = Quaternion.Euler(tempRot);
                }
                if (yaw > 0)
                {
                    var tempRot = transform.localRotation.eulerAngles;
                    tempRot.y += _speed * _rotateMultiplier;
                    transform.localRotation = Quaternion.Euler(tempRot);
                }
            }
           
        }

        private void CalculateMovementFixedUpdate()
        {
            if(_interactableZone.playerInput != null)
            {
                float thrust = _interactableZone.playerInput.actions["Thrust"].ReadValue<float>();

                if (thrust > 0)
                {
                    _rigidbody.AddForce(transform.up * _speed, ForceMode.Acceleration);
                }
                else if (thrust < 0)
                {
                    _rigidbody.AddForce(-transform.up * _speed, ForceMode.Acceleration);
                }
            }

        }

        private void CalculateTilt()
        {
            if(_interactableZone.playerInput != null)
            {
                Vector2 tilt = _interactableZone.playerInput.actions["Tilt"].ReadValue<Vector2>();

                if (tilt.x < 0)
                {
                    transform.rotation = Quaternion.Euler(00, transform.localRotation.eulerAngles.y, 30);
                }
                else if (tilt.x > 0)
                {
                    transform.rotation = Quaternion.Euler(0, transform.localRotation.eulerAngles.y, -30);
                }
                else if (tilt.y > 0)
                {
                    transform.rotation = Quaternion.Euler(30, transform.localRotation.eulerAngles.y, 0);
                }
                else if (tilt.y < 0)
                {
                    transform.rotation = Quaternion.Euler(-30, transform.localRotation.eulerAngles.y, 0);
                }
                else
                {
                    transform.rotation = Quaternion.Euler(0, transform.localRotation.eulerAngles.y, 0);
                }
            }

        }

        private void OnDisable()
        {
            InteractableZone.onZoneInteractionComplete -= EnterFlightMode;

        }
    }
}
