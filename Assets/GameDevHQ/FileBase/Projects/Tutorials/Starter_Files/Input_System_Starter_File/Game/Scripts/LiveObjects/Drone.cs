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
        private bool _inFlightMode = false;
        [SerializeField]
        private Animator _propAnim;
        [SerializeField]
        private CinemachineCamera _droneCam;
        [SerializeField]
        private InteractableZone _interactableZone;
        

        public static event Action OnEnterFlightMode;
        public static event Action onExitFlightmode;

        private PlayerInputActions _playerACtions;

        private void Awake()
        {
            _playerACtions = new PlayerInputActions();
        }

        private void OnEnable()
        {
            InteractableZone.onZoneInteractionComplete += EnterFlightMode;

            _playerACtions.Player.Disable();
            _playerACtions.Drone.Enable();

            _playerACtions.Drone.Exit.performed += Exit_performed;

        }

        private void Exit_performed(InputAction.CallbackContext context)
        {
            if (_inFlightMode)
            {
                _inFlightMode = false;
                onExitFlightmode?.Invoke();
                ExitFlightMode();
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

                //if (Input.GetKeyDown(KeyCode.Escape))
                //{
                //    _inFlightMode = false;
                //    onExitFlightmode?.Invoke();
                //    ExitFlightMode();
                //}
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
            float yaw = _playerACtions.Drone.Yaw.ReadValue<float>();

            if(yaw < 0)
            {
                var tempRot = transform.localRotation.eulerAngles;
                tempRot.y -= _speed / 3;
                transform.localRotation = Quaternion.Euler(tempRot);
            }
            if(yaw > 0)
            {
                var tempRot = transform.localRotation.eulerAngles;
                tempRot.y += _speed / 3;
                transform.localRotation = Quaternion.Euler(tempRot);
            }

            //if (Input.GetKey(KeyCode.LeftArrow))
            //{
            //    var tempRot = transform.localRotation.eulerAngles;
            //    tempRot.y -= _speed / 3;
            //    transform.localRotation = Quaternion.Euler(tempRot);
            //}
            //if (Input.GetKey(KeyCode.RightArrow))
            //{
            //    var tempRot = transform.localRotation.eulerAngles;
            //    tempRot.y += _speed / 3;
            //    transform.localRotation = Quaternion.Euler(tempRot);
            //}
        }

        private void CalculateMovementFixedUpdate()
        {
            float thrust = _playerACtions.Drone.Thrust.ReadValue<float>();
            
            if(thrust > 0)
            {
                _rigidbody.AddForce(transform.up * _speed, ForceMode.Acceleration);
            }
            else if(thrust < 0)
            {
                _rigidbody.AddForce(-transform.up * _speed, ForceMode.Acceleration);
            }

            //if (Input.GetKey(KeyCode.Space))
            //{
            //    _rigidbody.AddForce(transform.up * _speed, ForceMode.Acceleration);
            //}
            //if (Input.GetKey(KeyCode.V))
            //{
            //    _rigidbody.AddForce(-transform.up * _speed, ForceMode.Acceleration);
            //}
        }

        private void CalculateTilt()
        {
            Vector2 tilt = _playerACtions.Drone.Tilt.ReadValue<Vector2>();
            //int roll = _frameworkInputs.Drone

            if(tilt.x < 0)
            {
                transform.rotation = Quaternion.Euler(00, transform.localRotation.eulerAngles.y, 30);
            }
            else if(tilt.x > 0)
            {
                transform.rotation = Quaternion.Euler(0, transform.localRotation.eulerAngles.y, -30);
            }
            else if(tilt.y > 0)
            {
                transform.rotation = Quaternion.Euler(30, transform.localRotation.eulerAngles.y, 0);
            }
            else if(tilt.y < 0)
            {
                transform.rotation = Quaternion.Euler(-30, transform.localRotation.eulerAngles.y, 0);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, transform.localRotation.eulerAngles.y, 0);
            }

            //if (Input.GetKey(KeyCode.A)) 
            //    transform.rotation = Quaternion.Euler(00, transform.localRotation.eulerAngles.y, 30);
            //else if (Input.GetKey(KeyCode.D))
            //    transform.rotation = Quaternion.Euler(0, transform.localRotation.eulerAngles.y, -30);
            //else if (Input.GetKey(KeyCode.W))
            //    transform.rotation = Quaternion.Euler(30, transform.localRotation.eulerAngles.y, 0);
            //else if (Input.GetKey(KeyCode.S))
            //    transform.rotation = Quaternion.Euler(-30, transform.localRotation.eulerAngles.y, 0);
            //else 
            //    transform.rotation = Quaternion.Euler(0, transform.localRotation.eulerAngles.y, 0);
        }

        private void OnDisable()
        {
            InteractableZone.onZoneInteractionComplete -= EnterFlightMode;

            _playerACtions.Drone.Exit.performed -= Exit_performed;

            _playerACtions.Drone.Disable();
            _playerACtions.Player.Enable();
            
        }
    }
}
