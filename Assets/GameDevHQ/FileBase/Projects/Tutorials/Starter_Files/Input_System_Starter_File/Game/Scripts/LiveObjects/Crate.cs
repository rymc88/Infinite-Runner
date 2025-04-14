using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

namespace Game.Scripts.LiveObjects
{
    public class Crate : MonoBehaviour
    {
        [SerializeField] private float _punchDelay;
        [SerializeField] private GameObject _wholeCrate, _brokenCrate;
        [SerializeField] private Rigidbody[] _pieces;
        [SerializeField] private BoxCollider _crateCollider;
        [SerializeField] private InteractableZone _interactableZone;
        private bool _isReadyToBreak = false;

        private List<Rigidbody> _brakeOff = new List<Rigidbody>();

        PlayerInputActions _playerActions;

        private bool _multiTapTriggered = false;
        private bool _holdTriggered = false;
        int _zoneID;

        private void Awake()
        {
            _playerActions = new PlayerInputActions();
        }

        private void OnEnable()
        {
            InteractableZone.onZoneInteractionComplete += InteractableZone_onZoneInteractionComplete;

            _playerActions.Enable();
            _playerActions.Player.Disable();
            _playerActions.Drone.Disable();
            _playerActions.ForkLift.Disable();

            _playerActions.Crate.Enable();
            _playerActions.Crate.PressHold.performed += PressHold_performed;

        }

        private void PressHold_performed(InputAction.CallbackContext context)
        {
            if(_isReadyToBreak && _brakeOff.Count > 0 && InteractableZone.CurrentZoneID == 6)
            {
                var interactions = context.interaction;

                if (interactions is MultiTapInteraction)
                {
                    BreakPart(2f, 2);
                   
                }
                else if (interactions is HoldInteraction)
                {
                    BreakPart(5f, 5);
                    
                }

                StartCoroutine(PunchDelay());
            }
     
        }

        private void InteractableZone_onZoneInteractionComplete(InteractableZone zone)
        {

            if (_isReadyToBreak == false && _brakeOff.Count > 0)
            {
                _wholeCrate.SetActive(false);
                _brokenCrate.SetActive(true);
                _isReadyToBreak = true;
                //_zoneID = zone.GetZoneID();
            }

            //if (_isReadyToBreak && zone.GetZoneID() == 6) //Crate zone            
            //{
            //    if (_brakeOff.Count > 0)
            //    {
            //        if (_multiTapTriggered)
            //        {
            //            for (int i = 0; i < 2; i++)
            //            {
            //                BreakPart(2f);
            //            }
            //        }
            //        else if (_holdTriggered)
            //        {
            //            for (int i = 0; i < 6; i++)
            //            {
            //                BreakPart(3.5f);
            //            }
            //        }
                    
            //        StartCoroutine(PunchDelay());
            //    }
            //    else if (_brakeOff.Count == 0)
            //    {
            //        _isReadyToBreak = false;
            //        _crateCollider.enabled = false;
            //        _interactableZone.CompleteTask(6);
            //        Debug.Log("Completely Busted");
            //    }
            //}
        }

        private void Start()
        {
            _brakeOff.AddRange(_pieces);

        }

        public void BreakPart(float forceMultiplier, int destroyAmount)
        {
            if (_brakeOff.Count - destroyAmount > 0)
            {
                for (int i = 0; i < destroyAmount; i++)
                {
                    int rng = Random.Range(0, _brakeOff.Count);
                    _brakeOff[rng].constraints = RigidbodyConstraints.None;
                    _brakeOff[rng].AddForce(new Vector3(1f, 1f, 1f) * forceMultiplier, ForceMode.Force);
                    _brakeOff.Remove(_brakeOff[rng]);
                }
            }
            else
            {
                int rng = Random.Range(0, _brakeOff.Count);
                _brakeOff[rng].constraints = RigidbodyConstraints.None;
                _brakeOff[rng].AddForce(new Vector3(1f, 1f, 1f) * forceMultiplier, ForceMode.Force);
                _brakeOff.Remove(_brakeOff[rng]);
            }
            
        }

        IEnumerator PunchDelay()
        {
            float delayTimer = 0;
            while (delayTimer < _punchDelay)
            {
                yield return new WaitForEndOfFrame();
                delayTimer += Time.deltaTime;
            }

            if(_brakeOff.Count == 0)
            {
                _isReadyToBreak = false;
                _crateCollider.enabled = false;
                _interactableZone.CompleteTask(6);
                Debug.Log("Completely Busted");
            }
            else
            {
                _interactableZone.ResetAction(6);
            }
        }

        private void OnDisable()
        {
            InteractableZone.onZoneInteractionComplete -= InteractableZone_onZoneInteractionComplete;
            _playerActions.Player.Disable();
            _playerActions.Drone.Disable();
            _playerActions.ForkLift.Disable();
            _playerActions.Crate.Disable();
            _playerActions.Crate.PressHold.performed -= PressHold_performed;
        }
    }
}
