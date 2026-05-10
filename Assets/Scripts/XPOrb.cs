using System.Collections.Generic;
using UnityEngine;

public class XPOrb : MonoBehaviour
{
    public float minSpeed = 2f;
    public float maxSpeed = 10f;
    public float maxDistance = 20f;
    public int XPValue = 10;
    public Transform target;

    [Header("Orb Merging Settings")]
    public float orbAttractionDistance = 5f;
    public int minOrbsForAttraction = 3;
    public Color baseColor = Color.white;

    private static List<XPOrb> activeOrbs = new List<XPOrb>();
    private SpriteRenderer spriteRenderer;
    private Color currentColor;
    private bool isMerged = false;
    private Rigidbody2D rb;
    private GameObject colorOverlay;
    private SpriteRenderer overlayRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            currentColor = spriteRenderer.color;
            baseColor = currentColor;
        }

        CreateColorOverlay();
    }

    private void CreateColorOverlay()
    {
        colorOverlay = new GameObject("ColorOverlay");
        colorOverlay.transform.SetParent(transform, false);
        colorOverlay.transform.localPosition = Vector3.zero;
        colorOverlay.transform.localScale = Vector3.one;

        overlayRenderer = colorOverlay.AddComponent<SpriteRenderer>();
        overlayRenderer.sprite = spriteRenderer != null ? spriteRenderer.sprite : null;
        overlayRenderer.sortingOrder = (spriteRenderer != null ? spriteRenderer.sortingOrder : 0) + 1;
        overlayRenderer.color = Color.clear;
    }

    private void OnEnable()
    {
        if (!activeOrbs.Contains(this))
        {
            activeOrbs.Add(this);
        }
    }

    private void OnDisable()
    {
        activeOrbs.Remove(this);
    }

    private void OnDestroy()
    {
        activeOrbs.Remove(this);
    }

    public void Init(int xp, Transform ta)
    {
        XPValue = xp;
        target = ta;
        
        if (spriteRenderer != null)
        {
            baseColor = spriteRenderer.color;
            currentColor = baseColor;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isMerged) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            player.AddXP(XPValue);
            Destroy(gameObject);
            return;
        }

        XPOrb otherOrb = other.GetComponent<XPOrb>();
        if (otherOrb != null && otherOrb != this && !otherOrb.isMerged)
        {
            if (GetInstanceID() > otherOrb.GetInstanceID())
            {
                MergeWith(otherOrb);
            }
        }
    }

    private void MergeWith(XPOrb otherOrb)
    {
        if (isMerged || otherOrb.isMerged) return;

        isMerged = true;
        otherOrb.isMerged = true;

        XPOrb largerOrb;
        XPOrb smallerOrb;

        if (XPValue >= otherOrb.XPValue)
        {
            largerOrb = this;
            smallerOrb = otherOrb;
        }
        else
        {
            largerOrb = otherOrb;
            smallerOrb = this;
        }

        largerOrb.XPValue += smallerOrb.XPValue;
        largerOrb.ShiftColor();
        Destroy(smallerOrb.gameObject);
        largerOrb.isMerged = false;
    }

    private void ShiftColor()
    {
        if (overlayRenderer == null) return;

        float goldAmount = Mathf.Min((float)XPValue / 100f, 1f);

        Color overlayColor = new Color(1f, 0.85f, 0f, goldAmount * 0.7f);
        
        overlayRenderer.color = overlayColor;
    }

    void Update()
    {
        activeOrbs.RemoveAll(orb => orb == null);

        if (target != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, target.position);
            
            if (distanceToPlayer > maxDistance && activeOrbs.Count >= minOrbsForAttraction)
            {
                XPOrb nearestOrb = FindNearestOrb();
                if (nearestOrb != null)
                {
                    float distanceToOrb = Vector3.Distance(transform.position, nearestOrb.transform.position);
                    if (distanceToOrb <= orbAttractionDistance && distanceToOrb < distanceToPlayer)
                    {
                        MoveTowardsTarget(nearestOrb.transform);
                        return;
                    }
                }
            }
            
            MoveTowardsTarget(target);
        }
        else if (activeOrbs.Count >= minOrbsForAttraction)
        {
            XPOrb nearestOrb = FindNearestOrb();
            if (nearestOrb != null)
            {
                float distanceToOrb = Vector3.Distance(transform.position, nearestOrb.transform.position);
                if (distanceToOrb <= orbAttractionDistance)
                {
                    MoveTowardsTarget(nearestOrb.transform);
                }
            }
        }
    }

    private XPOrb FindNearestOrb()
    {
        XPOrb nearest = null;
        float nearestDistance = float.MaxValue;

        foreach (XPOrb orb in activeOrbs)
        {
            if (orb == null || orb == this) continue;

            float distance = Vector3.Distance(transform.position, orb.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = orb;
            }
        }

        return nearest;
    }

    private void MoveTowardsTarget(Transform targetTransform)
    {
        if (targetTransform == null) return;

        float distance = Vector3.Distance(transform.position, targetTransform.position);
        float factor = 1f - Mathf.Clamp01(distance / maxDistance);
        float speed = Mathf.Lerp(minSpeed, maxSpeed, factor);

        Vector2 dir = ((Vector2)targetTransform.position - (Vector2)transform.position).normalized;
        Vector2 moveAmount = dir * speed * Time.deltaTime;
        
        if (rb != null)
        {
            rb.MovePosition(rb.position + moveAmount);
        }
        else
        {
            transform.position += (Vector3)moveAmount;
        }
    }
}