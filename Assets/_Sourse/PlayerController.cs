using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public GameObject pistolObject;
    public GameObject shieldObject;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public int baseDamage = 10;
    public int rhythmDamage = 25;
    public float shootCooldown = 3f;
    public float hitDelay = 0.5f;
    private bool canShoot = true;

    [Header("Shield")]
    public float shieldDuration = 1f;
    public float shieldCooldown = 2f;
    private bool isShielded = false;
    private bool canShield = true;

    [Header("UI")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI shootCooldownText;
    public TextMeshProUGUI shieldCooldownText;

    private bool isDead = false;
    private Coroutine shootRoutine;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    void Update()
    {
        if (isDead) return;

        if (Input.GetKeyDown(KeyCode.W) && canShoot)
        {
            if (shootRoutine != null) StopCoroutine(shootRoutine);
            shootRoutine = StartCoroutine(ShootRoutine());
        }

        if (Input.GetKeyDown(KeyCode.S) && canShield)
        {
            StartCoroutine(UseShield());
        }

        UpdateCooldownUI();
    }

    IEnumerator ShootRoutine()
    {
        canShoot = false;

        if (pistolObject != null) pistolObject.SetActive(true);

        int finalDamage = baseDamage;
        if (Conductor.Instance != null && Conductor.Instance.IsNearBeat())
        {
            finalDamage = rhythmDamage;
        }

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.direction = Vector2.right;

        yield return new WaitForSeconds(hitDelay);

        Enemy enemy = FindObjectOfType<Enemy>();
        if (enemy != null && !enemy.IsShielded())
        {
            enemy.TakeDamage(finalDamage);
        }

        yield return new WaitForSeconds(0.1f);
        if (pistolObject != null) pistolObject.SetActive(false);

        yield return new WaitForSeconds(shootCooldown - hitDelay - 0.1f);
        canShoot = true;
    }

    IEnumerator UseShield()
    {
        canShield = false;
        isShielded = true;

        if (shieldObject != null) shieldObject.SetActive(true);

        yield return new WaitForSeconds(shieldDuration);

        isShielded = false;
        if (shieldObject != null) shieldObject.SetActive(false);

        yield return new WaitForSeconds(shieldCooldown - shieldDuration);
        canShield = true;
    }

    public void TakeDamage(int damage)
    {
        if (isShielded) return;

        currentHealth -= damage;
        UpdateHealthUI();

        if (currentHealth <= 0 && !isDead)
        {
            isDead = true;
            Die();
        }
    }

    void UpdateHealthUI()
    {
        if (healthText != null)
            healthText.text = $"ИГРОК: {currentHealth}/{maxHealth} HP";
    }

    void UpdateCooldownUI()
    {
        if (shootCooldownText != null)
            shootCooldownText.text = canShoot ? "[W] АТАКА" : "[W] ЖДИ";

        if (shieldCooldownText != null)
            shieldCooldownText.text = canShield ? "[S] ЩИТ" : "[S] ЖДИ";
    }

    void Die()
    {
        StopAllCoroutines();
        if (GameManager.Instance != null)
            GameManager.Instance.EnemyWin();
    }

    public bool IsShielded() => isShielded;
}