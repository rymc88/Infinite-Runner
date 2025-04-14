using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Scripts.LiveObjects
{
    public class EndZone : MonoBehaviour
    {
        PlayerInputActions _playerActions;

        private void Awake()
        {
            _playerActions = new PlayerInputActions();
        }

        private void OnEnable()
        {
            InteractableZone.onZoneInteractionComplete += InteractableZone_onZoneInteractionComplete;

            _playerActions.Player.Enable();
        }

        private void InteractableZone_onZoneInteractionComplete(InteractableZone zone)
        {
            if (zone.GetZoneID() == 7)
            {
                InteractableZone.CurrentZoneID = 0;
                _playerActions.Player.Disable();
                _playerActions.Drone.Disable();
                _playerActions.ForkLift.Disable();
                _playerActions.Crate.Disable();
                SceneManager.LoadScene(0);
            }
        }

        private void OnDisable()
        {
            _playerActions.Player.Disable();
            _playerActions.Drone.Disable();
            _playerActions.ForkLift.Disable();
            _playerActions.Crate.Disable();

            InteractableZone.onZoneInteractionComplete -= InteractableZone_onZoneInteractionComplete;
        }
    }
}