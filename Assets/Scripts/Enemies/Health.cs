using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    //[SerializeField] private float deathDelay = 1f;
    [SerializeField] private float invincibilityTime = 0.5f;

    [Header("Health Bar Settings")]
    [SerializeField] private Vector3 healthBarOffset = new Vector3(0, 1.5f, 0);
    [SerializeField] private float showDuration = 3f;

    // Добавляем поля для ручного назначения спрайтов
    [Header("Health Bar Refs (если не используете префаб)")]
    [SerializeField] private SpriteRenderer healthBarBackground; // Перетащите Background сюда
    [SerializeField] private SpriteRenderer healthBarFill;       // Перетащите Fill сюда

    public event Action OnDamaged;
    public event Action OnDeath;

    private int currentHealth;
    private bool isInvincible = false;
    private float initialFillWidth;
    private Camera mainCamera;

    private void Start()
    {
        currentHealth = maxHealth;
        if (healthBarFill != null)
        {
            initialFillWidth = healthBarFill.size.x;
        }

        SetHealthBarVisible(false);
    }

    private void LateUpdate()
    {
        mainCamera = Camera.main;
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible || currentHealth <= 0) return;

        currentHealth -= damage;
        OnDamaged?.Invoke();

        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            OnDeath?.Invoke();
            Die();
        }
        else
        {
            isInvincible = true;
            Invoke(nameof(ResetInvincibility), invincibilityTime);
        }
    }
    private void UpdateHealthBar()
    {
        if (healthBarFill == null || healthBarBackground == null) return;

        float healthPercentage = (float)currentHealth / maxHealth;
        healthBarFill.transform.localScale = new Vector3(healthPercentage, 1f, 1f);

        SetHealthBarVisible(true);
        CancelInvoke(nameof(HideHealthBar));
        Invoke(nameof(HideHealthBar), showDuration);
    }
    private void HideHealthBar()
    {
        SetHealthBarVisible(false);
    }

    private void SetHealthBarVisible(bool isVisible)
    {
        if (healthBarBackground != null) healthBarBackground.enabled = isVisible;
        if (healthBarFill != null) healthBarFill.enabled = isVisible;
    }

    private void ResetInvincibility()
    {
        isInvincible = false;
    }

    private void Die()
    {
        if (TryGetComponent<Collider2D>(out var collider))
        {
            collider.enabled = false;
        }

        if (TryGetComponent<EnemyAI>(out var ai))
        {
            ai.enabled = false;
        }
    }
    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public float GetHealthPercentage()
    {
        return (float)currentHealth / maxHealth;
    }
}