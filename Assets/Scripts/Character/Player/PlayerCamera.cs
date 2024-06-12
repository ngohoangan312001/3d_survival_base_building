using System.Collections;
using System.Collections.Generic;
using AN;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera Instance;
    public Camera cameraObject;
    public PlayerManager player;
    
    [Header("Camera Settings")] 
    [SerializeField] float leftAndRightRotationSpeed = 220;
    [SerializeField] float upAndDownRotationSpeed = 220;
    [SerializeField] float minimumPivot = -30;//Highest point can look up
    [SerializeField] float maximumPivot = 60;//Lowest point can look down
    [SerializeField] float cameraCollisionRadius = 0.2f;
    [SerializeField] LayerMask colliderWithLayers;
    
    [Header("Camera Values")] 
    [SerializeField] private float leftAndRightLookAngle;
    [SerializeField] private float upAndDownLookAngle;
    [SerializeField] private float cameraZPosition; //Value for camera collision
    [SerializeField] private float aimCameraZDistance = -0.2f;
    [SerializeField] private float aimCameraXDistance = 0.7f;
    [SerializeField] Transform cameraPivotTransform;
    private float cameraSmoothSpeed = 1;
    private Vector3 cameraVelocity;private Vector3 cameraObjectPosition;// Use for camera collision (move camera to this posiotion con collision)
    private float targetCameraZPosition; //Value for camera collision
    private Vector3 aimVelocity;
    
    [Header("Lock On")] 
    [SerializeField] private float lockOnRadius = 20;
    [SerializeField] private float maximumLockOnDistance = 20;
    [SerializeField] private float minimumViewableAngle = -50;
    [SerializeField] private float maximumViewableAngle = 50;
    [SerializeField] private float unlockedCameraHeight = 1.65f;
    [SerializeField] private float lockedCameraHeight = 2;
    [SerializeField] private float cameraHeightSpeed = 0.05f;
    [SerializeField] private float lockTargetFollowSpeed = 1;
    private List<CharacterManager> availableTargets = new List<CharacterManager>();
    public CharacterManager nearestLockOnTarget;
    public CharacterManager leftLockOnTarget;
    public CharacterManager rightLockOnTarget;
    private Coroutine cameraLockOnHeightCoroutine;
    
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        cameraZPosition = cameraObject.transform.localPosition.z;
    }

    public void HandleAllCameraActions()
    {
        if (player != null)
        {
            HandleCameraMode();
            //Aim camera
            HandleAimActionCamera();
            //Collide with object
            HandleCollisions();
            //Follow
            HandleFollowTarget();
            //Rotate around player
            HandleRotations();
            
        }
        
    }
    private void HandleCameraMode()
    {
        if (player.isThirdPersonCamera)
        {
            if(!player.playerNetworkManager.isAiming.Value)
            PlayerUIManager.instance.playerCrosshairManager.ToggleCrosshair(true,false);
            
            //Camera in third person mode
            player.playerMeshRenderer.SetActive(true);
            cameraObjectPosition.x = 0;
        }
        else
        {
            PlayerUIManager.instance.playerCrosshairManager.ToggleCrosshair(true,true);
            //Camera in FPS mode
            player.playerMeshRenderer.SetActive(false);
            cameraObjectPosition.z = 0;
            cameraObjectPosition.x = -cameraPivotTransform.localPosition.x;
            cameraObject.transform.localPosition = cameraObjectPosition;
        }
    }
    
    private void HandleFollowTarget()
    {
        Vector3 targetCameraPosition = Vector3.SmoothDamp(transform.position, player.transform.position,
            ref cameraVelocity, cameraSmoothSpeed * Time.deltaTime);
        transform.position = targetCameraPosition;
    }
    
    private void HandleRotations()
    {
        //Lock on => force rotation toward target
        if (player.playerNetworkManager.isLockOn.Value)
        {
            //If target is out of range, un lock
            float distanceFromTarget = Vector3.Distance(player.transform.position, player.playerCombatManager.currentTarget.characterCombatManager.lockOnTransform.position);
            if (distanceFromTarget > maximumLockOnDistance)
            {
                player.playerNetworkManager.isLockOn.Value = false;
                return;
            }
            //----------------------------------
            
            //Rotate this object (Left & Right)
            Vector3 rotationDirection =
                player.playerCombatManager.currentTarget.characterCombatManager.lockOnTransform.position - transform.position;
            rotationDirection.Normalize();
            rotationDirection.y = 0;

            Quaternion targerRotation = Quaternion.LookRotation(rotationDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targerRotation, lockTargetFollowSpeed);
            
            //Rotate camera pivot (Up & Down)
            rotationDirection = 
                player.playerCombatManager.currentTarget.characterCombatManager.lockOnTransform.position - cameraPivotTransform.position;
            rotationDirection.Normalize();
            targerRotation = Quaternion.LookRotation(rotationDirection);
            cameraPivotTransform.rotation = Quaternion.Slerp(cameraPivotTransform.rotation, targerRotation, lockTargetFollowSpeed);

            //Save current rotation so that when un lock on the camera not return too snappy
            leftAndRightLookAngle = transform.eulerAngles.y;
            upAndDownLookAngle = transform.eulerAngles.x;

        }
        else
        {
            //===Normal rotation===
        
            //Get rotate left and right angle base on leftAndRightRotationSpeed
            leftAndRightLookAngle += PlayerInputManager.Instance.cameraHorizontalInput * leftAndRightRotationSpeed * Time.deltaTime;
        
            //Get rotate up and down angle base on upAndDownRotationSpeed
            upAndDownLookAngle -= PlayerInputManager.Instance.cameraVerticalInput * upAndDownRotationSpeed * Time.deltaTime;

            // Clamp value so it will between minimumPivot and maximumPivot
            upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimumPivot, maximumPivot);

            Vector2 cameraRotation = Vector2.zero;
            Quaternion targetRotation;
        
            //Rotate left and right
            // Quaternion.Euler => Returns a rotation that rotates z degrees around the z axis, x degrees around the x axis, and y degrees around the y axis.
            cameraRotation.y = leftAndRightLookAngle;
            targetRotation = Quaternion.Euler(cameraRotation);
            transform.rotation = targetRotation;

            //Rotate up and down
            cameraRotation = Vector2.zero;
            cameraRotation.x = upAndDownLookAngle;
            targetRotation = Quaternion.Euler(cameraRotation);
            cameraPivotTransform.localRotation = targetRotation;
        }
        
    }

    private void HandleCollisions()
    {
        if (!player.isThirdPersonCamera || player.playerNetworkManager.isAiming.Value) return;
        
        targetCameraZPosition = cameraZPosition;
        
        //Raycast là gì ==> raycast là một tia được gửi từ một vị trí trong không gian 3D hoặc 2D và di chuyển theo một hướng cụ thể.
        
        //Structure used to get information back from a raycast
        RaycastHit hit; 
        
        Vector3 direction = cameraObject.transform.position - cameraPivotTransform.position;
        direction.Normalize(); 
        
        // thực hiện một kiểm tra va chạm giữa một quả cầu (tại vị trí cameraPivotTransform.position) với bán kính cameraCollisionRadius và hướng direction.
        // Nếu có va chạm, kết quả sẽ được lưu trong biến hit.
        if(Physics.SphereCast(cameraPivotTransform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetCameraZPosition),colliderWithLayers))
        {
            // create a Sphere around camera z position that check for collision
            // then calculate and get the position at the collision point to move camera to that point
            float distanceFromHitObject = Vector3.Distance(cameraPivotTransform.position, hit.point);
            targetCameraZPosition = -(distanceFromHitObject - cameraCollisionRadius);
        }

        if (Mathf.Abs(targetCameraZPosition) < cameraCollisionRadius)
        {
            targetCameraZPosition = -cameraZPosition;
        }
        cameraObjectPosition.z = Mathf.Lerp(cameraObject.transform.localPosition.z, targetCameraZPosition,0.15f);
        
        // Camera Object will move on z-axis away or closer from it parrent because we use localPosition
        cameraObject.transform.localPosition = cameraObjectPosition;
    }

    private void HandleAimActionCamera()
    {
        
        if (player.playerNetworkManager.isAiming.Value && !player.isPerformingAction)
        {
            if (player.isThirdPersonCamera)
            {
                cameraObjectPosition.x = aimCameraXDistance;
                cameraObjectPosition.z = aimCameraZDistance;
            }
            cameraObject.transform.localPosition = cameraObjectPosition;
            player.playerAnimatorManager.PlayTargetActionAnimation(player.playerInventoryManager.currentRightHandWeapon.aim_State,false,false,true,true);
        }
        else if(player.isPerformingAction)
        {
            cameraObjectPosition.x = 0;
        }

    }

    public void SwitchCameraMode()
    {
        player.isThirdPersonCamera = !player.isThirdPersonCamera;
    }

    public void HandleLocatingLockOnTargets()
    {
        // Will be used to get the target closest to the player
        float shortestDistance = Mathf.Infinity; 
        
        // Will be used to get the target closest to the right on the axis of current lock on target (+)
        float shortestDistanceOfRightTarget = Mathf.Infinity; 
        
        // Will be used to get the target closest to the left on the axis of current lock on target (-)
        float shortestDistanceOfLeftTarget = -Mathf.Infinity; 
        
        
        //The Physics.OverlapSphere method returns an array of colliders that intersect or are inside the specified sphere.
        //Can be used to find all colliders within the sphere, and then filter them based on your requirements (e.g., by tag or layer).
        Collider[] colliders = Physics.OverlapSphere(player.transform.position, lockOnRadius, WorldUtilityManager.Instance.GetCharacterLayers());

        if(colliders != null && colliders.Length > 0)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                CharacterManager lockOnTarget = colliders[i].GetComponent<CharacterManager>();

                if (lockOnTarget != null)
                {
                    //Check if target are within player field of view
                    Vector3 lockOnTargetDirection = lockOnTarget.transform.position - player.transform.position;
                    
                    float viewableAngle = Vector3.Angle(lockOnTargetDirection, cameraObject.transform.forward); 
                    
                    //If target is dead, check next target
                    if(lockOnTarget.isDead.Value) 
                        continue;
                    
                    //If target is this player, check next target
                    if(lockOnTarget.transform.root == player.transform.root)
                        continue;
                    
                    //If target is outside of view range or blocked by another object, check next target
                    if (viewableAngle > minimumViewableAngle && viewableAngle < maximumViewableAngle)
                    {
                        RaycastHit hit;
                        if (
                            Physics.Linecast(
                                player.playerCombatManager.lockOnTransform.position,
                                lockOnTarget.characterCombatManager.lockOnTransform.position, 
                                out hit, 
                                WorldUtilityManager.Instance.GetEnviromentLayers()
                                )
                            )
                        {
                            //Something in the way, player can't see the target, check next target
                            continue;
                        }
                        else
                        {
                            //If found target, add target to potential target List
                            availableTargets.Add(lockOnTarget);
                        }
                    }
                }
            }


            if (availableTargets != null && availableTargets.Count > 0)
            {
                for (int e = 0; e < availableTargets.Count; e++)
                {
                    float distanceFromTarget = Vector3.Distance(player.transform.position, availableTargets[e].transform.position);

                    if (distanceFromTarget < shortestDistance)
                    {
                        shortestDistance = distanceFromTarget;
                        nearestLockOnTarget = availableTargets[e];
                    }
                    
                    //If already lock on target, search for the nearest left and right targets
                    if (player.playerNetworkManager.isLockOn.Value)
                    {
                        //Phương thức InverseTransformPoint trong Unity được sử dụng để chuyển đổi một vị trí từ không gian thế giới (world space) sang không gian cục bộ (local space).
                        //Đây là hàm ngược lại của Transform.TransformPoint, được sử dụng để chuyển đổi từ không gian cục bộ sang không gian thế giới.
                        Vector3 relativeEnemyPosition =
                            player.transform.InverseTransformPoint(availableTargets[e].transform.position);
                        float distanceFromSideTarget = relativeEnemyPosition.x;
                        
                        if(availableTargets[e] == player.playerCombatManager.currentTarget)
                            continue;

                        if (distanceFromSideTarget <= 0.00 && distanceFromSideTarget > shortestDistanceOfLeftTarget)
                        {
                            shortestDistanceOfLeftTarget = distanceFromSideTarget;
                            leftLockOnTarget = availableTargets[e];
                        }
                        else if (distanceFromSideTarget >= 0.00 &&
                                 distanceFromSideTarget < shortestDistanceOfRightTarget)
                        {
                            shortestDistanceOfRightTarget = distanceFromSideTarget;
                            rightLockOnTarget = availableTargets[e];
                        }

                    }
                }
            }
            else
            {
                ClearLockOnTarget();
                player.playerNetworkManager.isLockOn.Value = false;
            }
        }
    }

    public void SetLockCameraHeight()
    {
        if (cameraLockOnHeightCoroutine != null)
        {
            StopCoroutine(cameraLockOnHeightCoroutine);
        }

        cameraLockOnHeightCoroutine = StartCoroutine(SetCameraHeight());
    }
    
    public void ClearLockOnTarget()
    {
        nearestLockOnTarget = null;
        leftLockOnTarget = null;
        rightLockOnTarget = null;
        availableTargets.Clear();
    }

    public IEnumerator WaitThenFindNewTarget()
    {
        while (player.isPerformingAction)
        {
            yield return null;
        }
        
        PlayerCamera.Instance.ClearLockOnTarget();
        PlayerCamera.Instance.HandleLocatingLockOnTargets();
        
        if (nearestLockOnTarget != null)
        {
            player.playerCombatManager.SetTarget(nearestLockOnTarget);
            player.playerNetworkManager.isLockOn.Value = true;
        }

        yield return null;
    }
    public IEnumerator SetCameraHeight()
    {
        float duration = 1;
        float timer = 0;
        
        Vector3 velocity = Vector3.zero;
        Vector3 newLockedCameraHeight = new Vector3(cameraPivotTransform.transform.localPosition.x, lockedCameraHeight);
        Vector3 newUnLockedCameraHeight = new Vector3(cameraPivotTransform.transform.localPosition.x, unlockedCameraHeight);

        while (timer < duration)
        {
            timer += Time.deltaTime;
            if (player != null)
            {
                if (player.playerCombatManager.currentTarget != null)
                {
                    cameraPivotTransform.transform.localPosition = Vector3.SmoothDamp(
                        cameraPivotTransform.transform.localPosition, newLockedCameraHeight, ref velocity,
                        cameraHeightSpeed);
                    
                    cameraPivotTransform.transform.localRotation = Quaternion.Lerp(
                        cameraPivotTransform.transform.localRotation, Quaternion.Euler(0,0,0),
                        lockTargetFollowSpeed);
                }
                else
                {
                    cameraPivotTransform.transform.localPosition = Vector3.SmoothDamp(
                        cameraPivotTransform.transform.localPosition, newUnLockedCameraHeight, ref velocity,
                        cameraHeightSpeed);
                }
            }

            yield return null;
        }
        
        
        if (player != null)
        {
            if (player.playerCombatManager.currentTarget != null)
            {
                cameraPivotTransform.transform.localPosition = newLockedCameraHeight;

                cameraPivotTransform.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                cameraPivotTransform.transform.localPosition = newUnLockedCameraHeight;
            }
        }

        yield return null;
    }
}
