using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Scripts.LiveObjects;
using Unity.Cinemachine;
using TMPro;
using UnityEngine.Windows;
using UnityEngine.InputSystem;

namespace Game.Scripts.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class Player : MonoBehaviour
    {
        private CharacterController _controller;
        private Animator _anim;
        [SerializeField]
        private float _speed = 5.0f;
        [SerializeField] private float _rotateSpeed = .5f;
        private bool _playerGrounded;
        [SerializeField]
        private Detonator _detonator;
        private bool _canMove = true;
        [SerializeField]
        private CinemachineCamera _followCam;
        [SerializeField]
        private GameObject _model;

        [SerializeField] private PlayerInput _playerInput;
        //private PlayerInputActions _playerActions;
        

        private void Awake()
        {
            //_playerActions = new PlayerInputActions();

            //if(_playerActions == null)
            //{
            //    Debug.Log("Player Input Actions is null", this.gameObject);
            //}

            //_playerInput.GetComponent<PlayerInput>();

            if(_playerInput == null)
            {
                Debug.Log("Player Input is null", this.gameObject);
            }
        }

        private void OnEnable()
        {
            InteractableZone.onZoneInteractionComplete += InteractableZone_onZoneInteractionComplete;
            Laptop.onHackComplete += ReleasePlayerControl;
            Laptop.onHackEnded += ReturnPlayerControl;
            Forklift.onDriveModeEntered += ReleasePlayerControl;
            Forklift.onDriveModeExited += ReturnPlayerControl;
            Forklift.onDriveModeEntered += HidePlayer;
            Drone.OnEnterFlightMode += ActivateDroneControls;
            Drone.onExitFlightmode += ReturnPlayerControl;

            //_playerActions.Player.Enable();
            //_playerActions.Drone.Enable();
            //_playerActions.ForkLift.Enable();
            //_playerActions.Crate.Enable();


            SwitchActionMap("Player");

        }

        private void Start()
        {
            _controller = GetComponent<CharacterController>();

            if (_controller == null)
                Debug.LogError("No Character Controller Present");

            _anim = GetComponentInChildren<Animator>();

            if (_anim == null)
                Debug.Log("Failed to connect the Animator");
        }



        private void Update()
        {
            if (_canMove == true)
                CalcutateMovement();

        }

        public void SwitchActionMap(string mapName)
        {
            if (_playerInput == null)
            {
                Debug.LogWarning("PlayerInput reference not set.");
                return;
            }

            var inputAsset = _playerInput.actions;

            if (inputAsset.FindActionMap(mapName) == null)
            {
                Debug.LogWarning($"Action Map '{mapName}' not found in the InputActionAsset.");
                return;
            }

            _playerInput.SwitchCurrentActionMap(mapName);
            Debug.Log($"Switched to action map: {mapName}");

            foreach (var map in inputAsset.actionMaps)
            {
                Debug.Log($"Map: {map.name} | Enabled: {map.enabled}");
            }
        }

        private void ActivateDroneControls()
        {
            ReleasePlayerControl();
            SwitchActionMap("Drone");
        }

        //public void SwitchToActionMap(string mapName)
        //{
        //    _playerInput.SwitchCurrentActionMap(mapName);
        //}

        private void CalcutateMovement()
        {
            if (_playerInput.currentActionMap.name == "Player")
            {
                _playerGrounded = _controller.isGrounded;

                //float h = Input.GetAxisRaw("Horizontal");
                //float v = Input.GetAxisRaw("Vertical");

                //Vector2 move = _playerActions.Player.Move.ReadValue<Vector2>();
                Vector2 move = _playerInput.actions["Move"].ReadValue<Vector2>();

                //transform.Rotate(transform.up, h);
                transform.Rotate(transform.up, (move.x * _rotateSpeed));

                //var direction = transform.forward * v;
                var direction = transform.forward * move.y;

                var velocity = direction * _speed;

                //transform.Rotate(transform.up, h);
                //transform.Rotate(transform.up, move.x);


                _anim.SetFloat("Speed", Mathf.Abs(velocity.magnitude));


                if (_playerGrounded)
                    velocity.y = 0f;
                if (!_playerGrounded)
                {
                    velocity.y += -20f * Time.deltaTime;
                }

                _controller.Move(velocity * Time.deltaTime);
            }


        }

        private void InteractableZone_onZoneInteractionComplete(InteractableZone zone)
        {
            switch(zone.GetZoneID())
            {
                case 1: //place c4
                    _detonator.Show();
                    break;
                case 2: //Trigger Explosion
                    TriggerExplosive();
                    break;
               
            }
        }

        private void ReleasePlayerControl()
        {
            _canMove = false;
            _followCam.Priority = 9;
        }

        private void ReturnPlayerControl()
        {
            _model.SetActive(true);
            _canMove = true;
            _followCam.Priority = 10;
        }

        private void HidePlayer()
        {
            _model.SetActive(false);
        }
               
        private void TriggerExplosive()
        {
            _detonator.TriggerExplosion();
        }

        private void OnDisable()
        {
            InteractableZone.onZoneInteractionComplete -= InteractableZone_onZoneInteractionComplete;
            Laptop.onHackComplete -= ReleasePlayerControl;
            Laptop.onHackEnded -= ReturnPlayerControl;
            Forklift.onDriveModeEntered -= ReleasePlayerControl;
            Forklift.onDriveModeExited -= ReturnPlayerControl;
            Forklift.onDriveModeEntered -= HidePlayer;
            Drone.OnEnterFlightMode -= ReleasePlayerControl;
            Drone.onExitFlightmode -= ReturnPlayerControl;

            //_playerActions.Player.Disable();

           
        }

    }
}

