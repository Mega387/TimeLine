using UnityEngine;
using TMPro;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public bool isDead = false;

    public GameObject pistolObject;
    public GameObject shieldObject;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public int bulletDamage = 15;
    public float hitDelay = 0.5f;

    public BeatAction[] pattern;

    [Header("UI")]
    public TextMeshProUGUI healthText;

    private int currentBeat = 0;
    private int patternIndex = 0;
    private Coroutine shieldCoroutine;
    private bool isShielded = false;

    [System.Serializable]
    public struct BeatAction
    {
        public enum ActionType { Shoot, Shield, Idle }
        public ActionType type;
        public int durationInBeats;
    }

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
        StartCoroutine(PatternLoop());
    }

    IEnumerator PatternLoop()
    {
        yield return new WaitUntil(() => Conductor.Instance.songPosition >= Conductor.Instance.secPerBeat);

        while (!isDead)
        {
            yield return new WaitUntil(() => Conductor.Instance.GetCurrentBeat() != currentBeat);
            currentBeat = Conductor.Instance.GetCurrentBeat();

            ExecuteAction(pattern[patternIndex]);
            patternIndex = (patternIndex + 1) % pattern.Length;
        }
    }

    void ExecuteAction(BeatAction action)
    {
        switch (action.type)
        {
            case BeatAction.ActionType.Shoot:
                StartCoroutine(ShootRoutine());
                break;
            case BeatAction.ActionType.Shield:
                if (shieldCoroutine != null) StopCoroutine(shieldCoroutine);
                shieldCoroutine = StartCoroutine(ActivateShield(action.durationInBeats));
                break;
            case BeatAction.ActionType.Idle:
                HidePistol();
                break;
        }
    }

    IEnumerator ShootRoutine()
    {
        if (pistolObject != null) pistolObject.SetActive(true);

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.direction = Vector2.left;

        yield return new WaitForSeconds(hitDelay);

        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null && !player.IsShielded())
        {
            player.TakeDamage(bulletDamage);
        }

        yield return new WaitForSeconds(0.1f);
        if (pistolObject != null) pistolObject.SetActive(false);
    }

    void HidePistol()
    {
        if (pistolObject != null) pistolObject.SetActive(false);
    }

    IEnumerator ActivateShield(int beats)
    {
        isShielded = true;
        if (shieldObject != null) shieldObject.SetActive(true);
        yield return new WaitForSeconds(Conductor.Instance.secPerBeat * beats);
        isShielded = false;
        if (shieldObject != null) shieldObject.SetActive(false);
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
            healthText.text = $"ВРАГ: {currentHealth}/{maxHealth} HP";
    }

    void Die()
    {
        StopAllCoroutines();
        if (GameManager.Instance != null)
            GameManager.Instance.PlayerWin();
    }

    public bool IsShielded() => isShielded;
}