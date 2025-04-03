using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; //need this to access the Input Action Asset
using System.Collections; 

public class PlayerController : MonoBehaviour
{
    /*
     * 1. Get a reference at start and an instance of our input actions
     * 2. Enable Input Action Map 
     * 3. Subscribe/Unsubscribe to the Input Event functions
     */

    [Header("Ball Movement")]
    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private float _jumpPowerForce = 15f;
    Vector2 _move;

    [Header("World Movement")]
    [SerializeField] Transform _world; //level to be tilted
    [SerializeField] float _tiltSpeed = 5f; //Speed the world tilts
    [SerializeField] float _moveSpeed = 10f; // Force applied to move the ball
    [SerializeField] float _tiltMax = 15f; //Max tilt in degrees

    [Header("UI")]
    [SerializeField] Slider _jumpMeter;
    bool _buttonHeld = false;
 

    private bool _isGrounded;
    PlayerInputActions _input;
    Rigidbody _rb;


    private void Awake()
    {
        _input = new PlayerInputActions();
        _rb = GetComponent<Rigidbody>();

        if (_rb == null)
        {
            Debug.LogError("Rigidbody is null");
        }
    }

    private void OnEnable()
    {
        _input.Player.Disable();
        _input.World.Disable();
        _input.Mobile.Enable();

        _input.Player.Jump.performed += Player_Jump_performed;

        _input.World.Jump.performed += World_Jump_performed;

        _input.Mobile.Jump.performed += Mobile_Jump_performed;
        _input.Mobile.PowerJump.started += Mobile_PowerJump_started;
        _input.Mobile.PowerJump.canceled += Mobile_PowerJump_canceled;
    }

    private void OnDisable()
    {
        _input.Player.Jump.performed -= Player_Jump_performed;

        _input.World.Jump.performed -= World_Jump_performed;

        _input.Mobile.Jump.performed -= Mobile_Jump_performed;
        _input.Mobile.PowerJump.started -= Mobile_PowerJump_started;
        _input.Mobile.PowerJump.canceled -= Mobile_PowerJump_canceled;
    }

    private void Mobile_Jump_performed(InputAction.CallbackContext context)
    {
        if (_isGrounded)
        {
            _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
            Debug.Log("Jump");
        }
    }
    private void Mobile_PowerJump_started(InputAction.CallbackContext context)
    {
        _buttonHeld = true;
    }

    private void Mobile_PowerJump_canceled(InputAction.CallbackContext context)
    {
        _buttonHeld = false;


        if (_isGrounded)
        {
            _rb.AddForce(Vector3.up * (_jumpPowerForce * _jumpMeter.value), ForceMode.Impulse);
        }

        StartCoroutine(JumpMeterCoolDown());
    }
  

    IEnumerator JumpMeterCoolDown()
    {
        while(_jumpMeter.value > 0)
        {
            _jumpMeter.value -= Time.deltaTime;
            yield return null;
        }

        _jumpMeter.value = 0;
    }

    public void Player_Jump_performed(InputAction.CallbackContext context)
    {
        if (_isGrounded)
        {
            _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        }
    }

    public void World_Jump_performed(InputAction.CallbackContext context)
    {
        if (_isGrounded)
        {
            _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        }
    }

    private void Update()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            _input.Player.Disable();
            _input.World.Enable();
        }

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            _input.Player.Enable();
            _input.World.Disable();
        }

        if (_input.Player.enabled)
        {
            //Input for Player Action Map
            _move = _input.Player.Movement.ReadValue<Vector2>();
        }

        if (_input.Mobile.enabled)
        {
            //Input for Player Action Map
            _move = _input.Mobile.Move.ReadValue<Vector2>();
        }

        if (_buttonHeld && _isGrounded)
        {
            _jumpMeter.value += Time.deltaTime;
            Mathf.Clamp(_jumpMeter.value, 0, 1);
        }

        if (_input.World.enabled)
        {
            //Input & Movement for World Action Map
            float tiltX = _input.World.TiltX.ReadValue<float>() * _tiltSpeed * Time.deltaTime;
            float tiltz = _input.World.TiltZ.ReadValue<float>() * _tiltSpeed * Time.deltaTime;

            // Get the current rotation in the -180 to 180 range using Mathf.DeltaAngle
            float currentX = Mathf.DeltaAngle(0, _world.transform.localEulerAngles.x);
            float currentZ = Mathf.DeltaAngle(0, _world.transform.localEulerAngles.z);

            // Clamp the rotation within the tiltMax range
            float newX = Mathf.Clamp(currentX + tiltX, -_tiltMax, _tiltMax);
            float newZ = Mathf.Clamp(currentZ + tiltz, -_tiltMax, _tiltMax);

            _world.transform.rotation = Quaternion.Euler(newX, 0, newZ);
        }
    }
    private void FixedUpdate()
    {
        if (_input.Player.enabled)
        {
            //Force for Player Action Map
            _rb.AddForce(new Vector3(_move.x, 0, _move.y) * _speed, ForceMode.Force);
        }

        if (_input.Mobile.enabled)
        {
            //Force for Player Action Map
            _rb.AddForce(new Vector3(_move.x, 0, _move.y) * _speed, ForceMode.Force);
        }


        if (_input.World.enabled)
        {
            //Force for World Action Map
            // Apply force to move the ball in the direction of gravity
            Vector3 gravityDirection = -_world.up; // The downward direction of the world
            _rb.AddForce(gravityDirection * _moveSpeed, ForceMode.Acceleration);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        _isGrounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        _isGrounded = false;
    }
}
