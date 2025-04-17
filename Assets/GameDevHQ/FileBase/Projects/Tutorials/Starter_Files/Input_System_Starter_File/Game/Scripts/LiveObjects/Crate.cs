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


        private void OnEnable()
        {
            InteractableZone.onZoneInteractionComplete += InteractableZone_onZoneInteractionComplete;

        }

        public void Break_performed(InputAction.CallbackContext context)
        {
            if (_isReadyToBreak && _brakeOff.Count > 0)
            {
                if (context.performed)
                {
                    var interactions = context.interaction;


                    if (interactions is MultiTapInteraction)
                    {
                        BreakPart(2f, 2);
                        Debug.Log(interactions);

                    }
                    else if (interactions is HoldInteraction)
                    {
                        BreakPart(5f, 5);
                        Debug.Log(interactions);

                    }

                    StartCoroutine(PunchDelay());
                }
               
            }

        }

        private void InteractableZone_onZoneInteractionComplete(InteractableZone zone)
        {

            if (_isReadyToBreak == false && _brakeOff.Count > 0)
            {
                _wholeCrate.SetActive(false);
                _brokenCrate.SetActive(true);
                _isReadyToBreak = true;
            }

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
        }
    }
}
