using UnityEngine;

public class Relic : MonoBehaviour, IDamageable
{
    public static Relic Instance;
    [SerializeField] Rigidbody rb;
    [SerializeField] int maxHealth = 100;
    [SerializeField] int currentHealth;
    [SerializeField] float maxHoldTime = 10f;
    [SerializeField] float holdSpeedMultiplier = 1f;
    [SerializeField] float backwardsSpeedMultiplier = 1f;
    [SerializeField] float slowInterval = 2f;
    [SerializeField] float slowPerStep = 0.1f;
    [SerializeField] float minSpeedMultiplier = 0.3f;
    [SerializeField] float stunDuration = 2f;
    [SerializeField] float pickupCooldown = 1f;
    [SerializeField] Vector3 holdOffset = new Vector3(0f, 2f, 0f);
    [SerializeField] float holdTimer = 0f;

    [Header("Throw settings")]
    [SerializeField] float upwardLift;
    [SerializeField] float throwStrength;

    [Header("Damage protection")]
    [SerializeField] bool damageProtectActive = false;
    [SerializeField] float protectMaxTime = 2;
    ActionTimer damageProtectTimer;

    [Header("Force Field after taken damage settings")]
    public float radius = 5f;
    public float pushForce = 500f;
    public float upwardsModifier = 0.5f;
    public LayerMask affectedLayers = 0;
    Vector3 directionForThrow;

    PlayerLogic holder;
    Collider relicCollider;
    float cooldownTimer = 0f;

    private void Awake()
    {
        damageProtectTimer = new(protectMaxTime);
        relicCollider = GetComponent<Collider>();
        relicCollider.isTrigger = true;

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Initialize();
    }

    void Initialize()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (holder != null)
        {
            holdTimer += Time.deltaTime * holdSpeedMultiplier;

            if (holdTimer >= maxHoldTime)
            {
                holdTimer = maxHoldTime;
                OnMaxTimeReached();
            }
            else
            {
                int slowSteps = Mathf.FloorToInt(holdTimer / slowInterval);
                holder.relicSpeedMultiplier = Mathf.Max(minSpeedMultiplier, 1f - slowPerStep * slowSteps);
            }
        }
        else
        {
            holdTimer = Mathf.Max(0f, holdTimer - Time.deltaTime * backwardsSpeedMultiplier);

            if (!relicCollider.enabled)
            {
                cooldownTimer -= Time.deltaTime;
                if (cooldownTimer <= 0f)
                {
                    relicCollider.enabled = true;
                }
            }
        }


        if(damageProtectActive) {
            if(damageProtectTimer.UpdateTimer()) {
                damageProtectActive = false;
            }
        }

        rb.useGravity = !holder;
    }

    private void LateUpdate()
    {
        if (holder != null)
        {
            transform.position = holder.transform.position + holdOffset;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (holder != null || other.isTrigger) return;

        PlayerLogic player = other.GetComponentInParent<PlayerLogic>();
        if (player == null || player.knockedDown || player.stunned) return;

        holder = player;
        holder.hasRelic = true;
        holder.throwChargeStartTime = 0f;
        relicCollider.enabled = false;
        Debug.Log("relic picked up by " + player.name);
    }

    public void Drop()
    {
        transform.position = holder.transform.position;
        holder.hasRelic = false;
        holder.relicSpeedMultiplier = 1f;
        holder = null;
        cooldownTimer = pickupCooldown;
        Debug.Log("relic dropped");
    }

    void OnMaxTimeReached()
    {
        Debug.Log("relic max time reached, stunning " + holder.name);
        PlayerLogic player = holder;
        Drop();
        player.Stun(stunDuration);
    }

    public int OnDamage(int damage)
    {
        if (damageProtectActive)
            return 0;

        TriggerPush();
        currentHealth -= damage;
        damageProtectActive = true;
        if (currentHealth < 0)
        {
            Debug.Log("Relic is destroyed");
            return 1;
        }
        else
            return 0;
    }

    public void TriggerPush()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius, affectedLayers);

        foreach (Collider hit in hitColliders)
        {
            if (hit.gameObject == this.gameObject) continue;

            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.AddExplosionForce(pushForce, transform.position, radius, upwardsModifier, ForceMode.Impulse);
            }
        }
    }

    public void ThrowRelic(float charge)
    {
        directionForThrow = holder.transform.forward;
        holder.relicSpeedMultiplier = 1f;
        holder.hasRelic = false;
        holder = null;
        cooldownTimer = pickupCooldown;
        rb.AddForce((directionForThrow * charge * throwStrength), ForceMode.Impulse);
        
        
    }
}
