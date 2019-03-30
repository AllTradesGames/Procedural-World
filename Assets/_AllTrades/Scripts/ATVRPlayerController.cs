/************************************************************************************

Copyright   :   Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.

Licensed under the Oculus SDK License Version 3.4.1 (the "License");
you may not use the Oculus SDK except in compliance with the License,
which is provided at the time of installation or download, or which
otherwise accompanies this software in either electronic or hard copy form.

You may obtain a copy of the License at

https://developer.oculus.com/licenses/sdk-3.4.1

Unless required by applicable law or agreed to in writing, the Oculus SDK
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

************************************************************************************/

using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

/// <summary>
/// Controls the player's movement in virtual reality.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class ATVRPlayerController : MonoBehaviour
{
    /// <summary>
    /// The rate acceleration during movement.
    /// </summary>
    public float Acceleration = 0.1f;

    /// <summary>
    /// The rate of damping on movement.
    /// </summary>
    public float Damping = 0.3f;

    /// <summary>
    /// The rate of additional damping when moving sideways or backwards.
    /// </summary>
    public float BackAndSideDampen = 0.5f;

    /// <summary>
    /// The force applied to the character when jumping.
    /// </summary>
    public float JumpForce = 0.3f;

    /// <summary>
    /// The rate of rotation when using a gamepad.
    /// </summary>
    public float RotationAmount = 1.5f;

    /// <summary>
    /// The rate of rotation when using the keyboard.
    /// </summary>
    public float RotationRatchet = 45.0f;

    /// <summary>
    /// The player will rotate in fixed steps if Snap Rotation is enabled.
    /// </summary>
    [Tooltip("The player will rotate in fixed steps if Snap Rotation is enabled.")]
    public bool SnapRotation = true;

    /// <summary>
    /// How many fixed speeds to use with linear movement? 0=linear control
    /// </summary>
    [Tooltip("How many fixed speeds to use with linear movement? 0=linear control")]
    public int FixedSpeedSteps;

    /// <summary>
    /// If true, reset the initial yaw of the player controller when the Hmd pose is recentered.
    /// </summary>
    public bool HmdResetsY = true;

    /// <summary>
    /// If true, tracking data from a child OVRCameraRig will update the direction of movement.
    /// </summary>
    public bool HmdRotatesY = true;

    /// <summary>
    /// Modifies the strength of gravity.
    /// </summary>
    public float GravityModifier = 0.379f;

    /// <summary>
    /// If true, each OVRPlayerController will use the player's physical height.
    /// </summary>
    public bool useProfileData = true;

    /// <summary>
    /// The CameraHeight is the actual height of the HMD and can be used to adjust the height of the character controller, which will affect the
    /// ability of the character to move into areas with a low ceiling.
    /// </summary>
    [NonSerialized]
    public float CameraHeight;

    /// <summary>
    /// This event is raised after the character controller is moved. This is used by the OVRAvatarLocomotion script to keep the avatar transform synchronized
    /// with the OVRPlayerController.
    /// </summary>
    public event Action<Transform> TransformUpdated;

    /// <summary>
    /// This bool is set to true whenever the player controller has been teleported. It is reset after every frame. Some systems, such as 
    /// CharacterCameraConstraint, test this boolean in order to disable logic that moves the character controller immediately 
    /// following the teleport.
    /// </summary>
    [NonSerialized] // This doesn't need to be visible in the inspector.
    public bool Teleported;

    /// <summary>
    /// This event is raised immediately after the camera transform has been updated, but before movement is updated.
    /// </summary>
    public event Action CameraUpdated;

    /// <summary>
    /// This event is raised right before the character controller is actually moved in order to provide other systems the opportunity to 
    /// move the character controller in response to things other than user input, such as movement of the HMD. See CharacterCameraConstraint.cs
    /// for an example of this.
    /// </summary>
    public event Action PreCharacterMove;

    /// <summary>
    /// When true, user input will be applied to linear movement. Set this to false whenever the player controller needs to ignore input for
    /// linear movement.
    /// </summary>
    public bool EnableLinearMovement = true;

    /// <summary>
    /// When true, user input will be applied to rotation. Set this to false whenever the player controller needs to ignore input for rotation.
    /// </summary>
    public bool EnableRotation = true;

    /// <summary>
    /// When true, user input will be applied to grabbing movement. Set this to false whenever the player controller needs to ignore input for
    /// grabbing movement.
    /// </summary>
    public bool EnableGrabMovement = true;
    public bool EnableGrabY = false;
    public float GrabMoveThreshold = 0.1f;

    /// <summary>
    /// When true, user input will be applied to hand directional boost. Movement will be applied based on a given hand's facing direction, while
	/// pressing the boost button on that hand's controller. Set this to false whenever the player controller needs to ignore input for
    /// boosting movement.
    /// </summary>
    public bool EnableHandBoost = true;
    public bool EnableBoostY = false;
    public float boostAddPerFrame = 0.001f;
    public float boostDampPerFrame = 0.02f;
    public float maxBoostSpeed = 0.05f;

    public Shader shaderGround;
    public Shader shaderBlur;
    public Material materialTerrain;

    public bool EnableMultipliedAccel = false;
    public float accelThreshold = 0.1f;
    public float accelCap = 5f;
    [HideInInspector]
    public delegate void OnRotate(float rotation); // For outside scripts
    [HideInInspector]
    public OnRotate onRotate;

    public bool EnableLeanMovement = false;
    public float leanSpeed = 0.6f;
    private bool boosting = false;
    private OVRCameraRig ovrScript;

    public Transform targetTransform;

    public bool EnableQuickBoost = true;
    public float qbActivateThreshold = 0.1f;
    public float qbDeactivateThreshold = 0.1f;
    public int qbDetectionFrames = 4;
    private int qbDetectionCount = 0;
    public int qbDurationFrames = 90;
    private int qbDurationCount = 0;

    private Vector3 headAnchorLastPosition;
    private bool isQuickBoosting = false;
    public bool vignetteOnQuickBoost = true;
    [HideInInspector]
    public delegate void OnQuickBoostStart(); // For outside scripts
    [HideInInspector]
    public OnQuickBoostStart onQuickBoostStart;
    [HideInInspector]
    public delegate void OnQuickBoostEnd(); // For outside scripts
    [HideInInspector]
    public OnQuickBoostEnd onQuickBoostEnd;
    public float qbActivateWindow = 0.4f;
    public float quickBoostCooldown = 0.5f;
    public float quickBoostDuration = 0.3f;
    public float quickBoostSpeed = 1f;

    protected CharacterController Controller = null;
    protected OVRCameraRig CameraRig = null;

    private float MoveScale = 1.0f;
    private Vector3 MoveThrottle = Vector3.zero;
    private float FallSpeed = 0.0f;
    private OVRPose? InitialPose;
    public float InitialYRotation { get; private set; }
    private float MoveScaleMultiplier = 1.0f;
    private float RotationScaleMultiplier = 1.0f;
    private bool SkipMouseRotation = true; // It is rare to want to use mouse movement in VR, so ignore the mouse by default.
    private bool HaltUpdateMovement = false;
    private bool prevHatLeft = false;
    private bool prevHatRight = false;
    private float SimulationRate = 60f;
    private float buttonRotation = 0f;
    private bool ReadyToSnapTurn; // Set to true when a snap turn has occurred, code requires one frame of centered thumbstick to enable another snap turn.
    private Vector3 rightGrabPosition;
    private Transform rightHandAnchor;
    private Transform rightWeapon;
    private Vector3 leftGrabPosition;
    private Transform leftHandAnchor;
    private Transform leftWeapon;
    private bool isRightHandOverriding;
    private Vector3 prevFrameMove;
    private float leftBoostReleaseTime = 0f;
    private float rightBoostReleaseTime = 0f;
    private float lastQuickBoostTime = 0f;
    private PostProcessVolume ppv;
    private Vignette vigLayer;
    private PreBoostVigConfig preBoostVigConfig;
    private class PreBoostVigConfig
    {
        public bool enabled = false;
        public float intensity = 0f;
        public float smoothness = 0f;

        public PreBoostVigConfig(bool en, float inten, float sm)
        {
            enabled = en;
            intensity = inten;
            smoothness = sm;
        }
    }

    private Vector3 accelAnchor;
    private Transform headAnchor;

    void Start()
    {
        // Add eye-depth as a camera offset from the player controller
        var p = CameraRig.transform.localPosition;
        p.z = OVRManager.profile.eyeDepth;
        CameraRig.transform.localPosition = p;
    }

    void Awake()
    {
        Controller = gameObject.GetComponent<CharacterController>();

        if (Controller == null)
            Debug.LogWarning("OVRPlayerController: No CharacterController attached.");

        // We use OVRCameraRig to set rotations to cameras,
        // and to be influenced by rotation
        OVRCameraRig[] CameraRigs = gameObject.GetComponentsInChildren<OVRCameraRig>();

        if (CameraRigs.Length == 0)
            Debug.LogWarning("OVRPlayerController: No OVRCameraRig attached.");
        else if (CameraRigs.Length > 1)
            Debug.LogWarning("OVRPlayerController: More then 1 OVRCameraRig attached.");
        else
            CameraRig = CameraRigs[0];

        InitialYRotation = transform.rotation.eulerAngles.y;

        rightHandAnchor = transform.Find("OVRCameraRig/TrackingSpace/RightHandAnchor");
        leftHandAnchor = transform.Find("OVRCameraRig/TrackingSpace/LeftHandAnchor");
        headAnchor = transform.Find("OVRCameraRig/TrackingSpace/CenterEyeAnchor");

    }

    void OnEnable()
    {
        OVRManager.display.RecenteredPose += ResetOrientation;

        if (CameraRig != null)
        {
            CameraRig.UpdatedAnchors += UpdateTransform;
        }
    }

    void OnDisable()
    {
        OVRManager.display.RecenteredPose -= ResetOrientation;

        if (CameraRig != null)
        {
            CameraRig.UpdatedAnchors -= UpdateTransform;
        }
    }

    void Update()
    {
        //Use keys to ratchet rotation
        if (Input.GetKeyDown(KeyCode.Q))
            buttonRotation -= RotationRatchet;

        if (Input.GetKeyDown(KeyCode.E))
            buttonRotation += RotationRatchet;
    }

    protected virtual void UpdateController()
    {
        if (useProfileData)
        {
            if (InitialPose == null)
            {
                // Save the initial pose so it can be recovered if useProfileData
                // is turned off later.
                InitialPose = new OVRPose()
                {
                    position = CameraRig.transform.localPosition,
                    orientation = CameraRig.transform.localRotation
                };
            }

            var p = CameraRig.transform.localPosition;
            if (OVRManager.instance.trackingOriginType == OVRManager.TrackingOrigin.EyeLevel)
            {
                p.y = OVRManager.profile.eyeHeight - (0.5f * Controller.height) + Controller.center.y;
            }
            else if (OVRManager.instance.trackingOriginType == OVRManager.TrackingOrigin.FloorLevel)
            {
                p.y = -(0.5f * Controller.height) + Controller.center.y;
            }
            CameraRig.transform.localPosition = p;
        }
        else if (InitialPose != null)
        {
            // Return to the initial pose if useProfileData was turned off at runtime
            CameraRig.transform.localPosition = InitialPose.Value.position;
            CameraRig.transform.localRotation = InitialPose.Value.orientation;
            InitialPose = null;
        }

        CameraHeight = CameraRig.centerEyeAnchor.localPosition.y;

        if (CameraUpdated != null)
        {
            CameraUpdated();
        }

        UpdateMovement();

        Vector3 moveDirection = Vector3.zero;

        float motorDamp = (1.0f + (Damping * SimulationRate * Time.deltaTime));

        MoveThrottle.x /= motorDamp;
        MoveThrottle.y = (MoveThrottle.y > 0.0f) ? (MoveThrottle.y / motorDamp) : MoveThrottle.y;
        MoveThrottle.z /= motorDamp;

        moveDirection += MoveThrottle * SimulationRate * Time.deltaTime;

        // Gravity
        if (Controller.isGrounded && FallSpeed <= 0)
            FallSpeed = ((Physics.gravity.y * (GravityModifier * 0.002f)));
        else
            FallSpeed += ((Physics.gravity.y * (GravityModifier * 0.002f)) * SimulationRate * Time.deltaTime);

        moveDirection.y += FallSpeed * SimulationRate * Time.deltaTime;


        if (Controller.isGrounded && MoveThrottle.y <= transform.lossyScale.y * 0.001f)
        {
            // Offset correction for uneven ground
            float bumpUpOffset = Mathf.Max(Controller.stepOffset, new Vector3(moveDirection.x, 0, moveDirection.z).magnitude);
            moveDirection -= bumpUpOffset * Vector3.up;
        }

        if (PreCharacterMove != null)
        {
            PreCharacterMove();
            Teleported = false;
        }

        Vector3 predictedXZ = Vector3.Scale((Controller.transform.localPosition + moveDirection), new Vector3(1, 0, 1));

        // Move contoller
        Controller.Move(moveDirection);
        Vector3 actualXZ = Vector3.Scale(Controller.transform.localPosition, new Vector3(1, 0, 1));

        if (predictedXZ != actualXZ)
            MoveThrottle += (actualXZ - predictedXZ) / (SimulationRate * Time.deltaTime);
    }





    public virtual void UpdateMovement()
    {
        if (HaltUpdateMovement)
            return;

        if (EnableLinearMovement)
        {
            bool moveForward = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
            bool moveLeft = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
            bool moveRight = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
            bool moveBack = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);

            bool dpad_move = false;

            if (OVRInput.Get(OVRInput.Button.DpadUp))
            {
                moveForward = true;
                dpad_move = true;

            }

            if (OVRInput.Get(OVRInput.Button.DpadDown))
            {
                moveBack = true;
                dpad_move = true;
            }

            MoveScale = 1.0f;

            if ((moveForward && moveLeft) || (moveForward && moveRight) ||
                (moveBack && moveLeft) || (moveBack && moveRight))
                MoveScale = 0.70710678f;

            // No positional movement if we are in the air
            if (!Controller.isGrounded)
                MoveScale = 0.0f;

            MoveScale *= SimulationRate * Time.deltaTime;

            // Compute this for key movement
            float moveInfluence = Acceleration * 0.1f * MoveScale * MoveScaleMultiplier;

            // Run!
            if (dpad_move || Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                moveInfluence *= 2.0f;

            Quaternion ort = transform.rotation;
            Vector3 ortEuler = ort.eulerAngles;
            ortEuler.z = ortEuler.x = 0f;
            ort = Quaternion.Euler(ortEuler);

            if (moveForward)
                MoveThrottle += ort * (transform.lossyScale.z * moveInfluence * Vector3.forward);
            if (moveBack)
                MoveThrottle += ort * (transform.lossyScale.z * moveInfluence * BackAndSideDampen * Vector3.back);
            if (moveLeft)
                MoveThrottle += ort * (transform.lossyScale.x * moveInfluence * BackAndSideDampen * Vector3.left);
            if (moveRight)
                MoveThrottle += ort * (transform.lossyScale.x * moveInfluence * BackAndSideDampen * Vector3.right);



            moveInfluence = Acceleration * 0.1f * MoveScale * MoveScaleMultiplier;

#if !UNITY_ANDROID // LeftTrigger not avail on Android game pad
            moveInfluence *= 1.0f + OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
#endif

            Vector2 primaryAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);

            // If speed quantization is enabled, adjust the input to the number of fixed speed steps.
            if (FixedSpeedSteps > 0)
            {
                primaryAxis.y = Mathf.Round(primaryAxis.y * FixedSpeedSteps) / FixedSpeedSteps;
                primaryAxis.x = Mathf.Round(primaryAxis.x * FixedSpeedSteps) / FixedSpeedSteps;
            }

            if (primaryAxis.y > 0.0f)
                MoveThrottle += ort * (primaryAxis.y * transform.lossyScale.z * moveInfluence * Vector3.forward);

            if (primaryAxis.y < 0.0f)
                MoveThrottle += ort * (Mathf.Abs(primaryAxis.y) * transform.lossyScale.z * moveInfluence *
                                       BackAndSideDampen * Vector3.back);

            if (primaryAxis.x < 0.0f)
                MoveThrottle += ort * (Mathf.Abs(primaryAxis.x) * transform.lossyScale.x * moveInfluence *
                                       BackAndSideDampen * Vector3.left);

            if (primaryAxis.x > 0.0f)
                MoveThrottle += ort * (primaryAxis.x * transform.lossyScale.x * moveInfluence * BackAndSideDampen *
                                       Vector3.right);
        }

        if (EnableRotation)
        {
            Vector3 anchorDiff = transform.position - accelAnchor;
            Vector3 euler = transform.rotation.eulerAngles;
            float rotateInfluence = SimulationRate * Time.deltaTime * RotationAmount * RotationScaleMultiplier;

            bool curHatLeft = OVRInput.Get(OVRInput.Button.PrimaryShoulder);

            if (curHatLeft && !prevHatLeft)
                euler.y -= RotationRatchet;

            prevHatLeft = curHatLeft;

            bool curHatRight = OVRInput.Get(OVRInput.Button.SecondaryShoulder);

            if (curHatRight && !prevHatRight)
                euler.y += RotationRatchet;

            prevHatRight = curHatRight;

            euler.y += buttonRotation;
            buttonRotation = 0f;


#if !UNITY_ANDROID || UNITY_EDITOR
            if (!SkipMouseRotation)
                euler.y += Input.GetAxis("Mouse X") * rotateInfluence * 3.25f;
#endif

            if (SnapRotation)
            {

                if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickLeft))
                {
                    if (ReadyToSnapTurn)
                    {
                        euler.y -= RotationRatchet;
                        if (onRotate != null)
                            onRotate(-RotationRatchet);
                        anchorDiff = Quaternion.Euler(0, -RotationRatchet, 0) * anchorDiff;
                        accelAnchor = transform.position - anchorDiff;
                        ReadyToSnapTurn = false;
                    }
                }
                else if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickRight))
                {
                    if (ReadyToSnapTurn)
                    {
                        euler.y += RotationRatchet;
                        if (onRotate != null)
                            onRotate(RotationRatchet);
                        anchorDiff = Quaternion.Euler(0, RotationRatchet, 0) * anchorDiff;
                        accelAnchor = transform.position - anchorDiff;
                        ReadyToSnapTurn = false;
                    }
                }
                else
                {
                    ReadyToSnapTurn = true;
                }
            }
            else
            {
                Vector2 secondaryAxis = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
                euler.y += secondaryAxis.x * rotateInfluence;
                if (onRotate != null)
                    onRotate(secondaryAxis.x * rotateInfluence);
            }

            transform.rotation = Quaternion.Euler(euler);
        }

        if (EnableGrabMovement)
        {
            Vector3 offset;

            float rightGripAxis = Input.GetAxis("Oculus_CrossPlatform_SecondaryHandTrigger");
            if (rightHandAnchor != null)
            {
                if (rightGripAxis > 0.5f)
                {
                    if (rightGrabPosition == Vector3.zero)
                    {
                        rightGrabPosition = rightHandAnchor.position;
                        isRightHandOverriding = true;
                    }
                    else if (isRightHandOverriding)
                    {
                        offset = rightGrabPosition - rightHandAnchor.position;
                        prevFrameMove = new Vector3(transform.position.x + offset.x, transform.position.y + (EnableGrabY ? offset.y : 0f), transform.position.z + offset.z) - transform.position;
                        transform.position += prevFrameMove;
                        rightGrabPosition = rightHandAnchor.position;
                        MoveThrottle = Vector3.zero;
                    }
                }
                else if (rightGrabPosition != Vector3.zero)
                {
                    rightGrabPosition = Vector3.zero;
                    if (prevFrameMove.magnitude > GrabMoveThreshold)
                    {
                        MoveThrottle = prevFrameMove;
                    }
                }
            }

            float leftGripAxis = Input.GetAxis("Oculus_CrossPlatform_PrimaryHandTrigger");
            if (leftHandAnchor != null)
            {
                if (leftGripAxis > 0.5f)
                {
                    if (leftGrabPosition == Vector3.zero)
                    {
                        leftGrabPosition = leftHandAnchor.position;
                        isRightHandOverriding = false;
                    }
                    else if (!isRightHandOverriding)
                    {
                        offset = leftGrabPosition - leftHandAnchor.position;
                        prevFrameMove = new Vector3(transform.position.x + offset.x, transform.position.y + (EnableGrabY ? offset.y : 0f), transform.position.z + offset.z) - transform.position;
                        transform.position += prevFrameMove;
                        leftGrabPosition = leftHandAnchor.position;
                        MoveThrottle = Vector3.zero;
                    }
                }
                else if (leftGrabPosition != Vector3.zero)
                {
                    leftGrabPosition = Vector3.zero;
                    if (prevFrameMove.magnitude > GrabMoveThreshold)
                    {
                        MoveThrottle = prevFrameMove;
                    }
                }
            }
        }

        if (EnableHandBoost)
        {
            if (leftWeapon == null)
            {
                leftWeapon = leftHandAnchor.Find("Weapon");
            }
            if (rightWeapon == null)
            {
                rightWeapon = rightHandAnchor.Find("Weapon");
            }

            prevFrameMove = new Vector3(MoveThrottle.x, MoveThrottle.y, MoveThrottle.z);

            if (Input.GetButton("Oculus_CrossPlatform_Button3"))
            {
                if (leftWeapon == null || leftWeapon.gameObject.activeSelf == false)
                {
                    MoveThrottle = EnableBoostY ? leftHandAnchor.forward : new Vector3(leftHandAnchor.forward.x, 0f, leftHandAnchor.forward.z);
                }
                else
                {
                    MoveThrottle = EnableBoostY ? -leftWeapon.right : new Vector3(-leftWeapon.right.x, 0f, -leftWeapon.right.z);
                }
            }
            else
            {
                MoveThrottle = Vector3.zero;
            }

            if (Input.GetButton("Oculus_CrossPlatform_Button1"))
            {
                if (rightWeapon == null || rightWeapon.gameObject.activeSelf == false)
                {
                    MoveThrottle += EnableBoostY ? rightHandAnchor.forward : new Vector3(rightHandAnchor.forward.x, 0f, rightHandAnchor.forward.z);
                }
                else
                {
                    MoveThrottle = EnableBoostY ? -rightWeapon.right : new Vector3(-rightWeapon.right.x, 0f, -rightWeapon.right.z);
                }
            }

            MoveThrottle.Normalize();

            if (MoveThrottle.magnitude == 0)
            {
                float mag = prevFrameMove.magnitude - boostDampPerFrame;
                mag = mag > 0f ? mag : 0f;
                MoveThrottle = prevFrameMove.normalized * mag;
            }
            else
            {
                MoveThrottle *= boostAddPerFrame;
                MoveThrottle += prevFrameMove;
                if (MoveThrottle.magnitude > maxBoostSpeed)
                {
                    if (EnableQuickBoost && isQuickBoosting)
                    {
                        MoveThrottle = prevFrameMove.normalized * (prevFrameMove.magnitude - boostDampPerFrame);
                    }
                    else
                    {
                        MoveThrottle = MoveThrottle.normalized * maxBoostSpeed;
                    }
                }
            }

            if (MoveThrottle.magnitude == 0)
            {
                materialTerrain.shader = shaderGround;
            }
            else
            {
                materialTerrain.shader = shaderBlur;
            }

        }

        if (EnableMultipliedAccel)
        {
            prevFrameMove = new Vector3(MoveThrottle.x, MoveThrottle.y, MoveThrottle.z);
            if (Input.GetAxis("Oculus_CrossPlatform_SecondaryHandTrigger") > 0.8f && Input.GetAxis("Oculus_CrossPlatform_PrimaryHandTrigger") > 0.8f)
            {
                MoveThrottle = new Vector3(headAnchor.position.x - accelAnchor.x, 0f, headAnchor.position.z - accelAnchor.z);
                if (MoveThrottle.magnitude > accelThreshold)
                {
                    MoveThrottle *= 0.01666667f / Time.deltaTime;
                    if (MoveThrottle.magnitude > accelCap)
                    {
                        MoveThrottle = MoveThrottle.normalized * accelCap;
                    }
                }
                else
                {
                    float mag = prevFrameMove.magnitude - (0.01666667f / Time.deltaTime) * accelThreshold;
                    mag = mag > 0f ? mag : 0f;
                    MoveThrottle = prevFrameMove.normalized * mag;
                }
            }
            else
            {
                MoveThrottle = Vector3.zero;
            }
            accelAnchor = headAnchor.position;
        }

        if (EnableLeanMovement)
        {
            if (Input.GetAxis("Oculus_CrossPlatform_SecondaryHandTrigger") > 0.8f && Input.GetAxis("Oculus_CrossPlatform_PrimaryHandTrigger") > 0.8f)
            {
                if (!boosting)
                {
                    OVRManager.display.RecenterPose();
                    boosting = true;
                }
                else
                {
                    MoveThrottle = headAnchor.parent.rotation * new Vector3(headAnchor.localPosition.x, 0f, headAnchor.localPosition.z);
                    if (!isQuickBoosting)
                    {
                        // Debug.Log((MoveThrottle - headAnchorLastPosition).magnitude.ToString("F5"));
                        if ((MoveThrottle - headAnchorLastPosition).magnitude > qbActivateThreshold)
                        {
                            qbDetectionCount++;
                            if (qbDetectionCount >= qbDetectionFrames)
                            {
                                isQuickBoosting = true;
                                qbDurationCount = 0;
                            }
                        }
                        else
                        {
                            qbDetectionCount = 0;
                        }
                        headAnchorLastPosition = MoveThrottle;
                    }

                    if (!isQuickBoosting)
                    {
                        MoveThrottle *= leanSpeed;
                        if (true/*MoveThrottle.magnitude > accelThreshold*/)
                        {
                            if (MoveThrottle.magnitude > (leanSpeed * 0.5f))
                            {
                                MoveThrottle = MoveThrottle.normalized * leanSpeed * 0.5f;
                            }
                        }
                        else
                        {
                            // TODO: slow down
                        }
                    }
                    else
                    {
                        qbDurationCount++;
                        MoveThrottle -= headAnchorLastPosition;
                        MoveThrottle *= leanSpeed * quickBoostSpeed;
                        if (MoveThrottle.magnitude > (leanSpeed * quickBoostSpeed * 0.5f))
                        {
                            MoveThrottle = MoveThrottle.normalized * leanSpeed * quickBoostSpeed * 0.5f;
                        }
                        if (qbDurationCount > qbDurationFrames)
                        {
                            isQuickBoosting = false;
                            qbDetectionCount = 0;
                        }
                    }
                }
            }
            else
            {
                MoveThrottle = Vector3.zero;
                boosting = false;
            }
            accelAnchor = headAnchor.position;
        }

        /*if (EnableQuickBoost)
        {
            if (Time.time > lastQuickBoostTime + quickBoostCooldown)
            {
                if (Input.GetButtonDown("Oculus_CrossPlatform_Button3") && Time.time - leftBoostReleaseTime < qbActivateWindow)
                {
                    lastQuickBoostTime = Time.time;
                    isQuickBoosting = true;
                    if (onQuickBoostStart != null)
                        onQuickBoostStart();
                    MoveThrottle += EnableBoostY ? leftHandAnchor.forward : new Vector3(leftHandAnchor.forward.x, 0f, leftHandAnchor.forward.z);
                }
                if (Input.GetButtonDown("Oculus_CrossPlatform_Button1") && Time.time - rightBoostReleaseTime < qbActivateWindow)
                {
                    lastQuickBoostTime = Time.time;
                    isQuickBoosting = true;
                    if (onQuickBoostStart != null)
                        onQuickBoostStart();
                    MoveThrottle += EnableBoostY ? rightHandAnchor.forward : new Vector3(rightHandAnchor.forward.x, 0f, rightHandAnchor.forward.z);
                }
                if (isQuickBoosting && vignetteOnQuickBoost)
                {
                    ppv = Camera.main.GetComponent<PostProcessVolume>();
                    if (ppv != null)
                    {
                        ppv.profile.TryGetSettings(out vigLayer);
                        if (vigLayer != null)
                        {
                            preBoostVigConfig = new PreBoostVigConfig(vigLayer.enabled.value, vigLayer.intensity.value, vigLayer.smoothness.value);
                            vigLayer.enabled.value = true;
                            vigLayer.intensity.value = 1f;
                            vigLayer.smoothness.value = 1f;
                        }
                    }
                }
            }
            if (isQuickBoosting)
            {
                MoveThrottle = MoveThrottle.normalized * quickBoostSpeed;
                if (Time.time > lastQuickBoostTime + quickBoostDuration)
                {
                    if (onQuickBoostEnd != null)
                        onQuickBoostEnd();
                    isQuickBoosting = false;
                    if (vignetteOnQuickBoost)
                    {
                        ppv = Camera.main.GetComponent<PostProcessVolume>();
                        if (ppv != null)
                        {
                            ppv.profile.TryGetSettings(out vigLayer);
                            if (vigLayer != null && preBoostVigConfig != null)
                            {
                                vigLayer.enabled.value = preBoostVigConfig.enabled;
                                vigLayer.intensity.value = preBoostVigConfig.intensity;
                                vigLayer.smoothness.value = preBoostVigConfig.smoothness;
                            }
                        }
                    }
                }
            }
            if (Input.GetButtonUp("Oculus_CrossPlatform_Button3"))
            {
                leftBoostReleaseTime = Time.time;
            }
            if (Input.GetButtonUp("Oculus_CrossPlatform_Button1"))
            {
                rightBoostReleaseTime = Time.time;
            }
        }*/
    }


    /// <summary>
    /// Invoked by OVRCameraRig's UpdatedAnchors callback. Allows the Hmd rotation to update the facing direction of the player.
    /// </summary>
    public void UpdateTransform(OVRCameraRig rig)
    {
        Transform root = CameraRig.trackingSpace;
        Transform centerEye = CameraRig.centerEyeAnchor;

        if (HmdRotatesY && !Teleported)
        {
            Vector3 prevPos = root.position;
            Quaternion prevRot = root.rotation;

            transform.rotation = Quaternion.Euler(0.0f, centerEye.rotation.eulerAngles.y, 0.0f);

            root.position = prevPos;
            root.rotation = prevRot;
        }

        UpdateController();
        if (TransformUpdated != null)
        {
            TransformUpdated(root);
        }
    }

    /// <summary>
    /// Jump! Must be enabled manually.
    /// </summary>
    public bool Jump()
    {
        if (!Controller.isGrounded)
            return false;

        MoveThrottle += new Vector3(0, transform.lossyScale.y * JumpForce, 0);

        return true;
    }

    /// <summary>
    /// Stop this instance.
    /// </summary>
    public void Stop()
    {
        Controller.Move(Vector3.zero);
        MoveThrottle = Vector3.zero;
        FallSpeed = 0.0f;
    }

    /// <summary>
    /// Gets the move scale multiplier.
    /// </summary>
    /// <param name="moveScaleMultiplier">Move scale multiplier.</param>
    public void GetMoveScaleMultiplier(ref float moveScaleMultiplier)
    {
        moveScaleMultiplier = MoveScaleMultiplier;
    }

    /// <summary>
    /// Sets the move scale multiplier.
    /// </summary>
    /// <param name="moveScaleMultiplier">Move scale multiplier.</param>
    public void SetMoveScaleMultiplier(float moveScaleMultiplier)
    {
        MoveScaleMultiplier = moveScaleMultiplier;
    }

    /// <summary>
    /// Gets the rotation scale multiplier.
    /// </summary>
    /// <param name="rotationScaleMultiplier">Rotation scale multiplier.</param>
    public void GetRotationScaleMultiplier(ref float rotationScaleMultiplier)
    {
        rotationScaleMultiplier = RotationScaleMultiplier;
    }

    /// <summary>
    /// Sets the rotation scale multiplier.
    /// </summary>
    /// <param name="rotationScaleMultiplier">Rotation scale multiplier.</param>
    public void SetRotationScaleMultiplier(float rotationScaleMultiplier)
    {
        RotationScaleMultiplier = rotationScaleMultiplier;
    }

    /// <summary>
    /// Gets the allow mouse rotation.
    /// </summary>
    /// <param name="skipMouseRotation">Allow mouse rotation.</param>
    public void GetSkipMouseRotation(ref bool skipMouseRotation)
    {
        skipMouseRotation = SkipMouseRotation;
    }

    /// <summary>
    /// Sets the allow mouse rotation.
    /// </summary>
    /// <param name="skipMouseRotation">If set to <c>true</c> allow mouse rotation.</param>
    public void SetSkipMouseRotation(bool skipMouseRotation)
    {
        SkipMouseRotation = skipMouseRotation;
    }

    /// <summary>
    /// Gets the halt update movement.
    /// </summary>
    /// <param name="haltUpdateMovement">Halt update movement.</param>
    public void GetHaltUpdateMovement(ref bool haltUpdateMovement)
    {
        haltUpdateMovement = HaltUpdateMovement;
    }

    /// <summary>
    /// Sets the halt update movement.
    /// </summary>
    /// <param name="haltUpdateMovement">If set to <c>true</c> halt update movement.</param>
    public void SetHaltUpdateMovement(bool haltUpdateMovement)
    {
        HaltUpdateMovement = haltUpdateMovement;
    }

    /// <summary>
    /// Resets the player look rotation when the device orientation is reset.
    /// </summary>
    public void ResetOrientation()
    {
        if (HmdResetsY && !HmdRotatesY)
        {
            Vector3 euler = transform.rotation.eulerAngles;
            euler.y = InitialYRotation;
            transform.rotation = Quaternion.Euler(euler);
        }
    }
}

