using System;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

namespace Game.Scripts.LiveObjects
{
    public class Forklift : MonoBehaviour
    {
        [SerializeField]
        private GameObject _lift, _steeringWheel, _leftWheel, _rightWheel, _rearWheels;
        [SerializeField]
        private Vector3 _liftLowerLimit, _liftUpperLimit;
        [SerializeField]
        private float _speed = 5f, _liftSpeed = 1f, _rotateSpeed = .1f;
        [SerializeField]
        private CinemachineCamera _forkliftCam;
        [SerializeField]
        private GameObject _driverModel;
        private bool _inDriveMode = false;
        [SerializeField]
        private InteractableZone _interactableZone;

        public static event Action onDriveModeEntered;
        public static event Action onDriveModeExited;

        private void OnEnable()
        {
            InteractableZone.onZoneInteractionComplete += EnterDriveMode;
        }


        private void EnterDriveMode(InteractableZone zone)
        {
            if (_inDriveMode !=true && zone.GetZoneID() == 5) //Enter ForkLift
            {
                _inDriveMode = true;
                _forkliftCam.Priority = 11;
                onDriveModeEntered?.Invoke();
                _driverModel.SetActive(true);
                _interactableZone.CompleteTask(5);
            }
        }

        private void ExitDriveMode()
        {
            _inDriveMode = false;
            _forkliftCam.Priority = 9;            
            _driverModel.SetActive(false);
            onDriveModeExited?.Invoke();
            
        }

        private void Update()
        {
            if (_inDriveMode == true)
            {
                LiftControls();
                CalcutateMovement();

                if(_interactableZone.playerInput != null)
                {
                    if (_interactableZone.playerInput.actions["Exit"].IsPressed())
                    {
                        ExitDriveMode();
                    }
                }
            }
        }

        private void CalcutateMovement()
        {
            if(_interactableZone.playerInput != null)
            {
                float h = _interactableZone.playerInput.actions["Horizontal"].ReadValue<float>();
                float v = _interactableZone.playerInput.actions["Vertical"].ReadValue<float>();

                var direction = new Vector3(0, 0, v);
                var velocity = direction * _speed;

                transform.Translate(velocity * Time.deltaTime);

                if (Mathf.Abs(v) > 0)
                {
                    var tempRot = transform.rotation.eulerAngles;
                    tempRot.y += h * _speed * _rotateSpeed;
                    transform.rotation = Quaternion.Euler(tempRot);
                }

            }
        }

      
        private void LiftControls()
        {
            if(_interactableZone.playerInput != null)
            {
                if (_interactableZone.playerInput.actions["LiftUp"].IsPressed())
                {
                    LiftUpRoutine();
                }
                else if (_interactableZone.playerInput.actions["LiftDown"].IsPressed())
                {
                    LiftDownRoutine();
                }
            }
        }

        private void LiftUpRoutine()
        {
            if (_lift.transform.localPosition.y < _liftUpperLimit.y)
            {
                Vector3 tempPos = _lift.transform.localPosition;
                tempPos.y += Time.deltaTime * _liftSpeed;
                _lift.transform.localPosition = new Vector3(tempPos.x, tempPos.y, tempPos.z);
            }
            else if (_lift.transform.localPosition.y >= _liftUpperLimit.y)
                _lift.transform.localPosition = _liftUpperLimit;
        }

        private void LiftDownRoutine()
        {
            if (_lift.transform.localPosition.y > _liftLowerLimit.y)
            {
                Vector3 tempPos = _lift.transform.localPosition;
                tempPos.y -= Time.deltaTime * _liftSpeed;
                _lift.transform.localPosition = new Vector3(tempPos.x, tempPos.y, tempPos.z);
            }
            else if (_lift.transform.localPosition.y <= _liftUpperLimit.y)
                _lift.transform.localPosition = _liftLowerLimit;
        }

        private void OnDisable()
        {
            InteractableZone.onZoneInteractionComplete -= EnterDriveMode;

        }

    }
}