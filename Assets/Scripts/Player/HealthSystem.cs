using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

[RequireComponent(typeof(SpriteRenderer))]
public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float invincibilityTime = 1f;
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private Color damageColor = Color.red;

    [Header("UI References")]
    [SerializeField] private Image healthFillImage;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private HeartbeatEffect heartbeatEffect;

    [Header("Animation Settings")]
    [SerializeField] private float minSpeed = 1f;
    [SerializeField] private float maxSpeed = 3f;
    [SerializeField] private float criticalHealthThreshold = 30f;

    [Header("Audio Settings")]
    [SerializeField] public AudioClip hitSound;

    public bool IsDead { get; private set; }
    public bool IsInvincible { get; private set; }
    public int CurrentHealth { get; private set; }

    private SpriteRenderer _spriteRenderer;
    private Color _originalColor;
    private bool _deathTriggered;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _originalColor = _spriteRenderer.color;
        CurrentHealth = maxHealth;
        if (healthText != null)
            healthText.text = $"{CurrentHealth}/{maxHealth}";
    }

    public void TakeDamage(int damage)
    {
        if (IsDead || IsInvincible || _deathTriggered) return;

        AudioManager.Instance.PlaySFX(hitSound);

        CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, maxHealth);
        StartCoroutine(DamageFlash());
        UpdateHealthUI();

        if (CurrentHealth <= 0)
        {
            DieWithDelay();
        }
        else
        {
            StartCoroutine(InvincibilityTimer());
        }
    }
    public void SetHealth(int newHealth)
    {
        CurrentHealth = newHealth;
        UpdateHealthUI();
    }

    public void Heal(int healAmount)
    {
        if (IsDead)
        {
            Debug.LogWarning("Нельзя лечить: персонаж мёртв");
            return;
        }

        if (healAmount <= 0)
        {
            Debug.LogWarning($"Некорректное значение лечения: {healAmount}");
            return;
        }

        int newHealth = Mathf.Clamp(CurrentHealth + healAmount, 0, maxHealth);

        CurrentHealth = newHealth;
        UpdateHealthUI();
    }

    private void DieWithDelay()
    {
        _deathTriggered = true;
        IsDead = true;

        if (healthFillImage != null)
            healthFillImage.gameObject.SetActive(false);

        if (healthText != null)
            healthText.gameObject.SetActive(false);

        SceneManager.LoadScene("DeathScreen");
    }

    private IEnumerator DamageFlash()
    {
        _spriteRenderer.color = damageColor;
        yield return new WaitForSeconds(flashDuration);
        _spriteRenderer.color = _originalColor;
    }

    private IEnumerator InvincibilityTimer()
    {
        IsInvincible = true;
        yield return new WaitForSeconds(invincibilityTime);
        IsInvincible = false;
    }

    public void UpdateHealthUI()
    {
        if (healthFillImage != null)
            healthFillImage.fillAmount = (float)CurrentHealth / maxHealth;

        if (healthText != null)
            healthText.text = $"{CurrentHealth}/{maxHealth}";

        if (heartbeatEffect != null)
        {
            float speed = CalculateHeartbeatSpeed();
            heartbeatEffect.SetSpeed(speed);
        }

    }

    private float CalculateHeartbeatSpeed()
    {
        float healthPercent = (float)CurrentHealth / maxHealth * 100f;
        return healthPercent <= criticalHealthThreshold
            ? Mathf.Lerp(maxSpeed * 1.5f, maxSpeed, healthPercent / criticalHealthThreshold)
            : Mathf.Lerp(maxSpeed, minSpeed, healthPercent / 100f);
    }
}