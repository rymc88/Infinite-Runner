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

        private void Awake()
        {
            _playerActions = new PlayerInputActions();
        }

        private void OnEnable()
        {
            InteractableZone.onZoneInteractionComplete += InteractableZone_onZoneInteractionComplete;

            _playerActions.Drone.Disable();
            _playerActions.ForkLift.Disable();
            _playerActions.Player.Enable();

            _playerActions.Player.Break.performed += Break_performed;
            

        }


        private void Break_performed(InputAction.CallbackContext context)
        {
            if (_isReadyToBreak)
            {
                if (context.interaction is MultiTapInteraction)
                {
                    BreakPart(true);
                }
                else if (context.interaction is HoldInteraction)
                {
                    BreakPart(false);
                }
            }
            
        }

        private void InteractableZone_onZoneInteractionComplete(InteractableZone zone)
        {
            
            if (_isReadyToBreak == false && _brakeOff.Count >0)
            {
                _wholeCrate.SetActive(false);
                _brokenCrate.SetActive(true);
                _isReadyToBreak = true;
            }

            //if (_isReadyToBreak && zone.GetZoneID() == 6) //Crate zone            
            //{
            //    if (_brakeOff.Count > 0)
            //    {
            //        BreakPart();
            //        StartCoroutine(PunchDelay());
            //    }
            //    else if(_brakeOff.Count == 0)
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

        public void BreakPart(bool mulitTap)
        {
            if (mulitTap)
            {
                if(_brakeOff.Count > 3)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        int rng = Random.Range(0, _brakeOff.Count);
                        _brakeOff[rng].constraints = RigidbodyConstraints.None;
                        _brakeOff[rng].AddForce(new Vector3(1f, 1f, 1f), ForceMode.Force);
                        _brakeOff.Remove(_brakeOff[rng]);
                    }
                    //int rng = Random.Range(0, _brakeOff.Count);
                    //_brakeOff[rng].constraints = RigidbodyConstraints.None;
                    //_brakeOff[rng].AddForce(new Vector3(1f, 1f, 1f), ForceMode.Force);
                    //_brakeOff.Remove(_brakeOff[rng]);
                }
                else
                {
                    for (int i = 0; i < _brakeOff.Count; i++)
                    {
                        int rng = Random.Range(0, _brakeOff.Count);
                        _brakeOff[rng].constraints = RigidbodyConstraints.None;
                        _brakeOff[rng].AddForce(new Vector3(1f, 1f, 1f), ForceMode.Force);
                        _brakeOff.Remove(_brakeOff[rng]);
                    }
                }
            }
            else
            {
                if (_brakeOff.Count > 5)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        int rng = Random.Range(0, _brakeOff.Count);
                        _brakeOff[rng].constraints = RigidbodyConstraints.None;
                        _brakeOff[rng].AddForce(new Vector3(1f, 1f, 1f), ForceMode.Force);
                        _brakeOff.Remove(_brakeOff[rng]);
                    }
                    //int rng = Random.Range(0, _brakeOff.Count);
                    //_brakeOff[rng].constraints = RigidbodyConstraints.None;
                    //_brakeOff[rng].AddForce(new Vector3(1f, 1f, 1f), ForceMode.Force);
                    //_brakeOff.Remove(_brakeOff[rng]);
                }
                else
                {
                    for (int i = 0; i < _brakeOff.Count; i++)
                    {
                        int rng = Random.Range(0, _brakeOff.Count);
                        _brakeOff[rng].constraints = RigidbodyConstraints.None;
                        _brakeOff[rng].AddForce(new Vector3(1f, 1f, 1f), ForceMode.Force);
                        _brakeOff.Remove(_brakeOff[rng]);
                    }
                }
            }

            if (_brakeOff.Count == 0)
            {
                _isReadyToBreak = false;
                _crateCollider.enabled = false;
                _interactableZone.CompleteTask(6);
                Debug.Log("Completely Busted");
            }
            //int rng = Random.Range(0, _brakeOff.Count);
            //_brakeOff[rng].constraints = RigidbodyConstraints.None;
            //_brakeOff[rng].AddForce(new Vector3(1f, 1f, 1f), ForceMode.Force);
            //_brakeOff.Remove(_brakeOff[rng]);            
        }

        IEnumerator PunchDelay()
        {
            float delayTimer = 0;
            while (delayTimer < _punchDelay)
            {
                yield return new WaitForEndOfFrame();
                delayTimer += Time.deltaTime;
            }

            _interactableZone.ResetAction(6);
        }

        private void OnDisable()
        {
            InteractableZone.onZoneInteractionComplete -= InteractableZone_onZoneInteractionComplete;
        }
    }
}
