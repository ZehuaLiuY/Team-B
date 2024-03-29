using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Voice.Unity;
using Hashtable = ExitGames.Client.Photon.Hashtable;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviourPun, IPunObservable
    {
        [Header("Player")] [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("How fast the character turns to face movement direction")] [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")] public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        public float Sensitivity = 1f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;
        
        [Header("Stamina Bar")]
        public float Stamina = 1.0f;
        public float StaminaDecreaseRate = 0.2f;
        public float StaminaRecoveryRate = 0.1f;
        public GameObject FlameThrower;

        // leader board ui
        public GameObject leaderboardUIPrefab;
        private GameObject leaderboardInstance;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDMotionSpeed;

#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;
        private bool _rotateOnMove = true;

        private bool _hasAnimator;

        private bool canSprint = true;
        private bool canShift = true;
        private bool canPickup = true;

        // current pos and rot
        public Vector3 currentPos;
        public Quaternion currentRot;

        private Vector2 _lastMoveInput;
        private Vector2 _lastLookInput;

        private MiniMapController _miniMapController;
        private Transform _cachedTransform;

        public int caughtCheeseCount = 0;
        
        public void EnableSprinting(bool enable)
        {
            canShift = enable;
        }

        private Recorder _recorder;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }


        private void Awake()
        {
            _input = GetComponent<StarterAssetsInputs>();
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }

            _hasAnimator = TryGetComponent(out _animator);

            if (_hasAnimator)
            {
                AssignAnimationIDs();
            }

            _cachedTransform = transform;
            _miniMapController = FindObjectOfType<MiniMapController>();
            _recorder = GetComponent<Recorder>();

            Hashtable props = new Hashtable {
                { "CheeseCount", caughtCheeseCount }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

            _controller = GetComponent<CharacterController>();

#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif
            

            currentPos = transform.position;
            currentRot = transform.rotation;

            GameObject canvasObject = GameObject.Find("Canvas");
            

        }

        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);

            if (photonView.IsMine)
            {
                JumpAndGravity();
                GroundedCheck();
                Move();
                pickup();

                if (Vector3.Distance(transform.position, currentPos) > 0.1f)
                {
                    _miniMapController.UpdatePlayerIcon(gameObject, _cachedTransform.position, _cachedTransform.rotation);
                }

                // voice control
                if (Input.GetKeyDown(KeyCode.V))
                {
                    _recorder.TransmitEnabled = true;
                }
                if (Input.GetKeyUp(KeyCode.V))
                {
                    _recorder.TransmitEnabled = false;
                }

                // leaderboard display when player hold tab
                if (Input.GetKeyDown(KeyCode.Tab))
                {

                    if (leaderboardInstance == null)
                    {
                        leaderboardInstance = Instantiate(leaderboardUIPrefab);
                        leaderboardInstance.SetActive(true);
                    }
                }

                if (Input.GetKeyUp(KeyCode.Tab))
                {
                    if (leaderboardInstance != null)
                    {
                        Destroy(leaderboardInstance);
                        leaderboardInstance = null;
                    }
                }
            }
            else
            {
                UpdateOther();
            }
        }

        private void IncrementCheeseCount() {
            caughtCheeseCount++;
            Hashtable props = new Hashtable() {
                { "CheeseCount", caughtCheeseCount }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
            Debug.Log("Cheese count: " + caughtCheeseCount);
        }


        private void pickup()
        {
            if (photonView.IsMine && Keyboard.current.rKey.wasPressedThisFrame && canPickup) {
                canPickup = false;
                photonView.RPC("SetPlayerIK_FlameThrower", RpcTarget.All,false);
                _animator.SetTrigger("pickup");
                photonView.RPC("TriggerPickupAnimation", RpcTarget.All);
                
                // 在人类周围创建一个盒形区域，检测是否存在 Cheese
                int layerMask = 1 << LayerMask.NameToLayer("BoxColliderLayer");
                Collider[] colliders = Physics.OverlapBox(transform.position, GetComponent<BoxCollider>().size, Quaternion.identity, layerMask);
                foreach (Collider collider in colliders)
                {
                    if (collider.gameObject.CompareTag("Target"))
                    {
                        Debug.Log("cheese is there");
                        PhotonView targetPhotonView = collider.gameObject.GetComponent<PhotonView>();

                        if (targetPhotonView != null)
                        {
                            // 调用目标上的RPC方法来显示DeiUI
                            targetPhotonView.RPC("showDeiUI", targetPhotonView.Owner, null);
                            if (HumanFightUI.Instance != null)
                            {
                                HumanFightUI.Instance.showCheeseCaught();
                                IncrementCheeseCount();
                            }
                        }

                        break;
                    }
                }
                StartCoroutine(ActivatePlayerIK_FlameThrowerAfterDelay());
                StartCoroutine(ResetPickupAfterDelay());
            }
        }
        [PunRPC]
        void SetPlayerIK_FlameThrower(bool state)
        {
            FlameThrower.SetActive(state);
            PlayerIK playerIK = GetComponent<PlayerIK>();
            playerIK.EnableIK(state);
        }
        
       IEnumerator ActivatePlayerIK_FlameThrowerAfterDelay()
        {
            yield return new WaitForSeconds(2f);
            
            photonView.RPC("SetPlayerIK_FlameThrower", RpcTarget.All,true);
        }
       
        private IEnumerator ResetPickupAfterDelay()
        {
            yield return new WaitForSeconds(2f); 
            canPickup = true;
        }
        
        //private void OnControllerColliderHit(ControllerColliderHit hit)
        //{
        //    if (hit.gameObject.CompareTag("Target"))
        //    {

        //        PhotonView targetPhotonView = hit.gameObject.GetComponent<PhotonView>();


        //        if (targetPhotonView != null && Input.GetKeyDown(KeyCode.R))
        //        {
        //            // 调用目标上的RPC方法来显示DeiUI
        //            targetPhotonView.RPC("showDeiUI", targetPhotonView.Owner, null);
        //        }
        //    }
        //}
        //private void OnTriggerEnter(Collider other)
        //{


        [PunRPC]
        void TriggerPickupAnimation()
        {
            _animator.SetTrigger("pickup");
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        void UpdateOther()
        {
            transform.position = Vector3.Lerp(transform.position, currentPos, Time.deltaTime * 10);
            transform.rotation = Quaternion.Slerp(transform.rotation, currentRot, Time.deltaTime * 500);

            _miniMapController.UpdatePlayerIcon(gameObject, _cachedTransform.position, _cachedTransform.rotation);
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier * Sensitivity;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier * Sensitivity;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        private void Move()
        {
            float targetSpeed = (_input.move != Vector2.zero && canSprint && Stamina > 0 && _input.sprint && canShift) ? SprintSpeed : MoveSpeed;
            if (_input.sprint && canSprint && Stamina > 0 && _input.move != Vector2.zero && canShift)
            {
                Stamina -= StaminaDecreaseRate * Time.deltaTime;
                if (Stamina <= 0)
                {
                    Stamina = 0;
                    canSprint = false; // 体力耗尽，不能再奔跑
                }
            }

            UpdateStamina();

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                if (_rotateOnMove)
                {
                    transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
                }
            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            // move the player
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        private void UpdateStamina()
        {
            if (!_input.sprint || !canSprint || _input.move != Vector2.zero || !canSprint)
            {
                Stamina += StaminaRecoveryRate * Time.deltaTime;
                if (Stamina >= 1)
                {
                    Stamina = 1;
                    canSprint = true; // 体力完全恢复，现在可以再次奔跑
                }
            }

            if (HumanFightUI.Instance != null)
            {
                HumanFightUI.Instance.UpdateStaminaBar(Stamina);
            }
        }
        
        private void JumpAndGravity()
        {
            if (Grounded)
            {

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump - DISABLED
                // if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                // {
                //     _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                //
                //     if (_hasAnimator)
                //     {
                //         _animator.SetBool(_animIDJump, true);
                //     }
                // }
                
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        public void SetSensitivity(float newSensitivity)
        {
            Sensitivity = newSensitivity;
        }

        public void SetRotateOnMove(bool newRotateOnMove)
        {
            _rotateOnMove = newRotateOnMove;
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {

                bool positionChanged = Vector3.Distance(transform.position, currentPos) > 0.1f;
                bool rotationChanged = Quaternion.Angle(transform.rotation, currentRot) > 5.0f;
                bool moveChanged = _lastMoveInput != _input.move;
                bool lookChanged = _lastLookInput != _input.look;

                stream.SendNext(positionChanged);
                stream.SendNext(rotationChanged);
                stream.SendNext(moveChanged);
                stream.SendNext(lookChanged);

                if(positionChanged)
                {
                    stream.SendNext(transform.position);
                    currentPos = transform.position;
                }
                if(rotationChanged)
                {
                    stream.SendNext(transform.rotation);
                    currentRot = transform.rotation;
                }

                if(moveChanged)
                {
                    stream.SendNext(_input.move);
                    _lastMoveInput = _input.move;
                }
                if(lookChanged)
                {
                    stream.SendNext(_input.look);
                    _lastLookInput = _input.look;
                }
                // stream.SendNext(_input.shoot);
            }
            else
            {

                bool positionChanged = (bool)stream.ReceiveNext();
                bool rotationChanged = (bool)stream.ReceiveNext();
                bool moveChanged = (bool)stream.ReceiveNext();
                bool lookChanged = (bool)stream.ReceiveNext();

                if(positionChanged)
                {
                    currentPos = (Vector3)stream.ReceiveNext();
                }
                if(rotationChanged)
                {
                    currentRot = (Quaternion)stream.ReceiveNext();
                }
                if(moveChanged)
                {
                    _input.move = (Vector2)stream.ReceiveNext();
                }
                if(lookChanged)
                {
                    _input.look = (Vector2)stream.ReceiveNext();
                }

                // _input.shoot = (bool)stream.ReceiveNext();
            }
        }
    }
}