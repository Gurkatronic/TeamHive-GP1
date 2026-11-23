using UnityEngine;

public class Relic : MonoBehaviour
{
    public static Relic Instance;

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

    PlayerLogic holder;
    Collider relicCollider;
    float cooldownTimer = 0f;

    private void Awake()
    {
        relicCollider = GetComponent<Collider>();
        relicCollider.isTrigger = true;

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
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
}
