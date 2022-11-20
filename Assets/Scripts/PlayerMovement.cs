using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField] InputReader inputReader;
    [SerializeField] PlayerData playerData;

    [Space, Header("Game Objects")]
    [SerializeField] GameObject spawnPoint;

    [Space, Header("Locomotion")]
    [SerializeField, Range(0f, 100f)] float maxSpeed = 5f;
    [SerializeField, Range(0f, 100f)] float maxSwimSpeed = 5f;
    [Space]
    [SerializeField] int groundContactCount;
    [SerializeField] int steepContactCount;
    [Space]
    [SerializeField] int maxJumps = 1;
    [SerializeField] float jumpHeight = 2f;
    [Space]
    [SerializeField, Range(0f, 100f)] float maxGroundAcceleration = 40f;
    [SerializeField, Range(0f, 100f)] float maxAirAcceleration = 10f;
    [SerializeField, Range(0f, 100f)] float maxSwimAcceleration = 5f;
    [Space]
    [SerializeField] Vector2 steepContactNormal;
    [SerializeField] Vector2 groundContactNormal;

    [Space, Header("Movement Angles")]
    [SerializeField, Range(0f, 90f)] float maxGroundAngle = 25f;
    [SerializeField, Range(0f, 90f)] float maxStairAngle = 50f;

    [Space, Header("Swimming")]
    [SerializeField] float submergenceOffset = .5f;
    [SerializeField] float submergence;
    [SerializeField, Min(.1f)] float submergenceRange = 1f;
    [SerializeField, Min(0f)] float buoyancy = 1f;
    [SerializeField, Range(0f, 10f)] float waterDrag = 1f;
    [SerializeField, Range(.01f, 1f)] float swimThreshold = .5f;
    
    [Space, Header("Physics")]
    [SerializeField] int groundedPhysicsStepsSinceLast;
    [SerializeField] int jumpingPhysicsStepsSinceLast;
    [SerializeField] Vector2 upAxis;
    [SerializeField] Vector2 rightAxis;
    [SerializeField] Vector2 desiredVelocity;
    [SerializeField] Vector2 velocity;

    [Space, Header("Layers")]
    [SerializeField] LayerMask groundProbeMask = -1;
    [SerializeField] LayerMask stairProbeMask = -1;
    [SerializeField] LayerMask waterLayer = 4;

    [Space, Header("Other")]
    [SerializeField] int jumpPhase;
    [SerializeField] float minGroundDotProduct;
    [SerializeField] float minStairDotProduct;
    [SerializeField] float maxSnapToGroundSpeed;
    [SerializeField] float snapToProbeDistance;
    [SerializeField] Vector2 minPlayerPosition;
    [SerializeField] Vector2 maxPlayerPosition;
    
    // COMPONENTS
    Animator anim;
    Camera mainCam;
    Rigidbody2D playerRigidbody;

    // INPUT
    bool hasJumpInput;
    Vector2 moveInput;

    // PROPERTIES
    bool OnGround => groundContactCount > 0;
    bool OnSteep => steepContactCount > 0;
    bool InWater => submergence > 0f;
    bool IsSwimming => submergence >= swimThreshold;
    bool CanJump => maxJumps > 0;
    bool CanJumpWhenNotGrounded => CanJump && InWater;

    Vector2 PlayerPosition2D => new(transform.position.x, transform.position.y);
    static Vector2 Gravity => Physics2D.gravity;

    bool hasRightMouseInput;
    
    void OnEnable()
    {
        inputReader.MoveInputEvent += v => moveInput.Set(v.x, v.y);
        inputReader.RightMouseInputEvent += b => hasRightMouseInput = b;
        inputReader.JumpInputEvent += OnJumpInput;
        inputReader.JumpInputCancelledEvent += OnJumpInputCancelled;
    }
    
    void OnDisable()
    {
        inputReader.JumpInputEvent -= OnJumpInput;
        inputReader.JumpInputCancelledEvent -= OnJumpInputCancelled;
    }

    void OnValidate()
    {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
        minStairDotProduct = Mathf.Cos(maxStairAngle * Mathf.Deg2Rad);
    }

    void Awake()
    {
        anim = GetComponent<Animator>();
        mainCam = Camera.main;
        playerRigidbody = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        playerData.ResetPlayerPositionData();
        upAxis = Vector2.up; // TODO Kan måske droppes.
        rightAxis = Vector2.right; // TODO Kan måske droppes.
    }

    void Update()
    {
       CheckPlayerPosition();
       ClampInputVectorMagnitude(ref moveInput); 
       if(hasRightMouseInput)
           ClampInputVectorMagnitude(ref mouseMovementInput);
       CalculateDesiredVelocityVector();
       
       GetMousePosition();
    }

    void CheckPlayerPosition()
    {
        if (PlayerPosition2D.y > minPlayerPosition.y) return;
        ResetPlayerPosition();
    }

    void ResetPlayerPosition() => transform.position = spawnPoint.transform.position;

    void FixedUpdate()
    {
        playerData.PlayerPosition = playerRigidbody.position;
        
        UpdateState();
        
        if (InWater)
            velocity *= 1f - waterDrag * submergence * Time.deltaTime;
        
        AdjustVelocity();
        CheckIfCanJump();
        UpdateRigidbodyVelocity();
        ClearState();
    }
    
    void AdjustVelocity()
    {
        float acceleration;
        float speed;
        Vector2 xAxis;

        if (InWater)
        {
            const float max_depth_movement_factor = 1f;
            float depthMovement = submergence / swimThreshold;
            float swimFactor = Mathf.Min(max_depth_movement_factor, depthMovement);
            acceleration = Mathf.LerpUnclamped(OnGround ? maxGroundAcceleration : maxAirAcceleration, maxSwimAcceleration, swimFactor);
            speed = Mathf.LerpUnclamped(maxSpeed, maxSwimSpeed, swimFactor);
            xAxis = rightAxis;
        }
        else
        {
            acceleration = OnGround ? maxGroundAcceleration : maxAirAcceleration;
            speed = maxSpeed;
            xAxis = rightAxis;
        }
        
        xAxis = ProjectDirectionOnPlane(xAxis, groundContactNormal);

        Vector2 relativeVelocity = velocity - desiredVelocity;
        float currentX = Vector2.Dot(relativeVelocity, xAxis);
       
        float maxSpeedChange = acceleration * Time.deltaTime;
        float newX = Mathf.MoveTowards(currentX, moveInput.x * speed, maxSpeedChange);
        
        AddToVelocityVector(xAxis * (newX - currentX));
        AdjustSwimVelocity(relativeVelocity, speed, maxSpeedChange);
    }

    void AdjustSwimVelocity(Vector2 relativeVelocity, float speed, float maxSpeedChange)
    {
        if (!IsSwimming) return;
        
        float currentY = Vector2.Dot(relativeVelocity, upAxis);
        float newY = Mathf.MoveTowards(currentY, mouseMovementInput.y * speed, maxSpeedChange);
        AddToVelocityVector(upAxis * (newY - currentY));
    }
    
    void UpdateRigidbodyVelocity()
    {
        if (InWater)
            AddToVelocityVector(Gravity * ((1f - buoyancy * submergence) * Time.deltaTime));
        else if (OnGround && velocity.sqrMagnitude < .01f)
            AddToVelocityVector(groundContactNormal * Vector2.Dot(Gravity, groundContactNormal) * Time.deltaTime);
        else
            AddToVelocityVector(Gravity * Time.deltaTime);
        
        playerRigidbody.velocity = velocity;
    }

    void ClearState()
    {
        steepContactCount = groundContactCount = 0;
        steepContactNormal = groundContactNormal = Vector2.zero;
        submergence = 0f;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
       EvaluateWaterLayer(other);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        EvaluateWaterLayer(other);
    }

    void EvaluateWaterLayer(Component other)
    {
        if ((waterLayer & (1 << other.gameObject.layer)) != 0)
            EvaluateSubmergence();
    }
    
    void EvaluateSubmergence()
    {
        const float guard_against_invalid_submergence_value = 1f;
        
        var origin = playerRigidbody.position + upAxis * submergenceOffset;

        RaycastHit2D hit = Physics2D.Raycast(origin, -upAxis, submergenceRange + guard_against_invalid_submergence_value, waterLayer);
        if (hit)
            submergence = 1f - hit.distance / submergenceRange;
        else
            submergence = 1f;
    }
    
    void UpdateState()
    {
        groundedPhysicsStepsSinceLast += 1;
        jumpingPhysicsStepsSinceLast += 1;

        velocity = playerRigidbody.velocity;

        if (CheckIfSwimming() || OnGround || SnapToGround() || CheckSteepContacts())
        {
            groundedPhysicsStepsSinceLast = 0;
            EvaluateFalseLanding();
            if (groundContactCount > 1)
                groundContactNormal.Normalize();
        }
        else
            groundContactNormal = upAxis;
    }
    
    void CheckIfCanJump()
    {
        if (!hasJumpInput) return;
        hasJumpInput = false;
        Jump();
    }
    
    void Jump()
    {
        Vector2 jumpDirection;

        if (OnGround)
            jumpDirection = groundContactNormal;
        else if (OnSteep)
        {
            jumpDirection = steepContactNormal;
            jumpPhase = 0;
        }
        else if (CanJumpWhenNotGrounded && jumpPhase <= maxJumps)
        {
            PreventExtraJumpAfterFallFromSurface();
            jumpDirection = groundContactNormal;
        }
        else return;

        jumpingPhysicsStepsSinceLast = 0;
        jumpPhase += 1;

        float jumpSpeed = Mathf.Sqrt(2f * Gravity.magnitude * jumpHeight);
        jumpDirection = (jumpDirection + upAxis).normalized;
        float alignedSpeed = Vector2.Dot(velocity, jumpDirection);
        
        if (alignedSpeed > 0f)
            jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);

        AddToVelocityVector(jumpDirection * jumpSpeed);
    }

    void EvaluateCollision(Collision2D collision)
    {
        float minDotProduct = GetMinDotProduct(collision.gameObject.layer);
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector2 normal = collision.GetContact(i).normal;
            GetUpDotProduct(out float upDot, normal);
            if (upDot >= minDotProduct)
            {
                groundContactCount += 1;
                groundContactNormal += normal;
            }
            else if (upDot > -.01f)
            {
                steepContactCount += 1;
                steepContactNormal += normal;
            }
        }
    }
    
    bool SnapToGround()
    {
        if (groundedPhysicsStepsSinceLast > 1 || jumpingPhysicsStepsSinceLast <= 2) return false;

        float speed = velocity.magnitude;
        if (speed > maxSnapToGroundSpeed) return false;

        var hit = Physics2D.Raycast(playerRigidbody.position, -upAxis, snapToProbeDistance, groundProbeMask);
        if (!hit) return false;
        if (GetUpDotProduct(hit.normal) < GetMinDotProduct(hit.collider.gameObject.layer)) return false;
        
        groundContactCount = 1;
        groundContactNormal = hit.normal;
        float dot = Vector2.Dot(velocity, hit.normal);
        if (dot <= 0f) return false;
        
        velocity = (velocity - hit.normal * dot).normalized * speed;
        return true;
    }
    
    bool CheckSteepContacts()
    {
        if (steepContactCount <= 1) return false;
        
        steepContactNormal.Normalize();
        if (!(GetUpDotProduct(steepContactNormal) >= minGroundDotProduct)) return false;

        groundContactCount = 1;
        groundContactNormal = steepContactNormal;
        return true;
    }

    bool CheckIfSwimming()
    {
        if (!IsSwimming) return false;
        
        groundContactCount = 0;
        groundContactNormal = upAxis;
        return true;
    }
    
    void OnJumpInput() => hasJumpInput = true;
    void OnJumpInputCancelled() => hasJumpInput = false;
    
    void EvaluateFalseLanding() => jumpPhase = jumpingPhysicsStepsSinceLast > 1 ? 0 : jumpPhase;
    
    void PreventExtraJumpAfterFallFromSurface() => jumpPhase = jumpPhase is 0 ? 1 : jumpPhase;
    
    void OnCollisionEnter2D(Collision2D collision) => EvaluateCollision(collision);
    void OnCollisionStay2D(Collision2D collision) => EvaluateCollision(collision);
    
    void CalculateDesiredVelocityVector() => desiredVelocity = moveInput * maxSpeed;
    
    void AddToVelocityVector(Vector2 toAdd) => velocity += toAdd;
    
    void GetUpDotProduct(out float upDot, Vector2 rhs) => upDot = GetUpDotProduct(rhs);
    float GetUpDotProduct(Vector2 rhs) => Vector2.Dot(upAxis, rhs);
    
    float GetMinDotProduct(int layer) => (stairProbeMask & (1 << layer)) == 0 ? minGroundDotProduct : minStairDotProduct;
    
    Vector2 mouseMovementInput;
    Vector2 mouseMovementDirection;
    
    void GetMousePosition()
    {
        if (!InWater || !IsSwimming) return;
        mouseMovementInput = mainCam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    }
    
    static void ClampInputVectorMagnitude(ref Vector2 input)
    {
        const float max_input_length_magnitude = 1f;
        input = Vector2.ClampMagnitude(input, max_input_length_magnitude);
    }
    
    static Vector2 ProjectDirectionOnPlane(Vector2 direction, Vector2 normal) => (direction - normal * Vector2.Dot(direction, normal)).normalized;
    
    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, upAxis);

        if (!InWater || !IsSwimming) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(mouseMovementInput, new Vector3(.2f, .2f, 0f));

        if (!hasRightMouseInput) return;
        Gizmos.DrawRay(transform.position, new Vector3(mouseMovementInput.x, mouseMovementInput.y, 0f).normalized);
    }
}