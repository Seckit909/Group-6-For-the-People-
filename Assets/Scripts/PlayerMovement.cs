using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField, Range(-10f,20f)] float maxSpeed = 5f;
    [SerializeField] Vector2 moveInput;
    
    [Space, Header("Scriptable Objects")]
    [SerializeField] InputReader inputReader;
    [SerializeField] PlayerData playerData;

    [Space, Header("Locomotion")]
    [SerializeField] int groundContactCount;
    [SerializeField] int steepContactCount;
    [SerializeField] int maxJumps = 1;
    [SerializeField] float jumpHeight = 2f;
    [SerializeField] float maxGroundAcceleration = 10f;
    [SerializeField] float maxAirAcceleration = 10f;
    [SerializeField] Vector2 steepContactNormal;
    [SerializeField] Vector2 groundContactNormal;

    [Space, Header("Movement Angles")]
    [SerializeField, Range(0f, 90f)] float maxGroundAngle = 25f;
    [SerializeField, Range(0f, 90f)] float maxStairAngle = 50f;

    [Space, Header("Submergence")]
    [SerializeField] float submergenceOffset = .5f;
    [SerializeField, Min(.1f)] float submergenceRange = 1f;
    [SerializeField] float submergence;
    [SerializeField] float waterDrag = 1f;
    
    [Space, Header("Physics")]
    [SerializeField] Vector2 upAxis;
    [SerializeField] Vector2 rightAxis;
    [SerializeField] Vector2 desiredVelocity;
    [SerializeField] Vector2 velocity;
    
    [Space, Header("Layers")]
    [SerializeField] LayerMask waterLayer = 4;
    
    Animator anim;
    Rigidbody2D playerRigidbody;
    
    bool OnGround => groundContactCount > 0;
    bool OnSteep => steepContactCount > 0;
    bool InWater => submergence > 0f;
    bool CanJump => maxJumps > 0;

    void OnEnable()
    {
        inputReader.MoveInputEvent += v => moveInput.Set(v.x, v.y);
        inputReader.JumpInputEvent += OnJumpInput;
        inputReader.JumpInputCancelledEvent += OnJumpInputCancelled;
    }

    void OnValidate()
    {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
        minStairDotProduct = Mathf.Cos(maxStairAngle * Mathf.Deg2Rad);
    }

    void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        playerData.ResetPlayerPosition();
        upAxis = Vector2.up;
        rightAxis = Vector2.right;
    }

    void Update()
    {
        ClampInputVectorMagnitude();
        
        desiredVelocity = moveInput * maxSpeed;
    }

    void FixedUpdate()
    {
        UpdateState();
        UpdateVelocity();
        EvaluateJump();
        UpdateRigidbodyVelocity();
        
        playerData.PlayerPosition = playerRigidbody.position;
        
        if (InWater)
            velocity *= 1f - waterDrag * submergence * Time.deltaTime;
        
        ClearState();
    }
    
    void UpdateVelocity()
    {
        Vector2 xAxis = ProjectDirectionOnPlane(rightAxis, groundContactNormal);
        Vector2 yAxis = ProjectDirectionOnPlane(upAxis, groundContactNormal);
        
        float currentX = Vector2.Dot(velocity, xAxis);
        float currentY = Vector2.Dot(velocity, yAxis);

        float acceleration = OnGround ? maxGroundAcceleration : maxAirAcceleration;
        float maxSpeedChange = acceleration * Time.deltaTime;
        
        float newX = Mathf.MoveTowards(currentX, desiredVelocity.x, maxSpeedChange);
        float newY = Mathf.MoveTowards(currentY, desiredVelocity.y, maxSpeedChange);
        
        velocity += xAxis * (newX - currentX) + yAxis * (newY - currentY);
    }
    
    void UpdateRigidbodyVelocity()
    {
        velocity += Physics2D.gravity * Time.deltaTime;
        playerRigidbody.velocity = velocity;
    }

    void ClearState()
    {
        ResetContactCount();
        ResetContactNormals();
        submergence = 0f;
    }

    void ClampInputVectorMagnitude() => moveInput = Vector2.ClampMagnitude(moveInput, maxLength: 1f);

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
        var origin = playerRigidbody.position + upAxis * submergenceOffset;

        RaycastHit2D hit = Physics2D.Raycast(origin, -upAxis, submergenceRange, waterLayer);
        if(hit)
            submergence = 1f - hit.distance / submergenceRange;
    }

    bool hasJumpInput;
    
    void OnJumpInput() => hasJumpInput = true;
    void OnJumpInputCancelled() => hasJumpInput = false;
    
    void ResetContactCount() => steepContactCount = groundContactCount = 0;
    void ResetContactNormals() => steepContactNormal = groundContactNormal = Vector2.zero;
    
    void OnDisable()
    {
        inputReader.JumpInputEvent -= OnJumpInput;
        inputReader.JumpInputCancelledEvent -= OnJumpInputCancelled;
    }

    // HEREFTER :: MÅSKE UNØDVENDIG
    //
    //
    //

    [SerializeField] LayerMask groundProbeMask = -1;
    [SerializeField] LayerMask stairProbeMask = -1;
    
    [SerializeField] float minGroundDotProduct;
    [SerializeField] float minStairDotProduct;

    [SerializeField] int jumpPhase;
    [SerializeField] int groundedPhysicsStepsSinceLast;
    [SerializeField] int jumpingPhysicsStepsSinceLast;
    [SerializeField] float maxSnapToGroundSpeed;
    [SerializeField] float snapToProbeDistance;

    bool CanJumpWhenNotGrounded => InWater;

    void UpdateState()
    {
        groundedPhysicsStepsSinceLast += 1;
        jumpingPhysicsStepsSinceLast += 1;

        velocity = playerRigidbody.velocity;

        if (OnGround || SnapToGround() || CheckSteepContacts())
        {
            groundedPhysicsStepsSinceLast = 0;
            EvaluateFalseLanding();
            if (groundContactCount > 1)
                groundContactNormal.Normalize();
        }
        else
            groundContactNormal = upAxis;
    }
    
    void EvaluateJump()
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

        float jumpSpeed = Mathf.Sqrt(2f * Physics2D.gravity.magnitude * jumpHeight);
        jumpDirection = (jumpDirection + upAxis).normalized;
        float alignedSpeed = Vector2.Dot(velocity, jumpDirection);
        
        if (alignedSpeed > 0f)
            jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);

        velocity += jumpDirection * jumpSpeed;
    }
    
    void EvaluateFalseLanding() => jumpPhase = jumpingPhysicsStepsSinceLast > 1 ? 0 : jumpPhase;
    void PreventExtraJumpAfterFallFromSurface() => jumpPhase = jumpPhase == 0 ? 1 : jumpPhase;
    
    void OnCollisionEnter2D(Collision2D collision) => EvaluateCollision(collision);
    void OnCollisionStay2D(Collision2D collision) => EvaluateCollision(collision);

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
    
    void GetUpDotProduct(out float upDot, Vector2 rhs) => upDot = GetUpDotProduct(rhs);
    float GetUpDotProduct(Vector2 rhs) => Vector2.Dot(upAxis, rhs);
    float GetMinDotProduct(int layer) => (stairProbeMask & (1 << layer)) == 0 ? minGroundDotProduct : minStairDotProduct;
    
    static Vector2 ProjectDirectionOnPlane(Vector2 direction, Vector2 normal) => (direction - normal * Vector2.Dot(direction, normal)).normalized;
}