using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    [Header("Movement Stats")]
    public float runSpeed = 8f;
    public float jumpHeight = 3f;
    public float groundDamping = 20f; // how fast do we change direction? higher means faster
    public float inAirDamping = 5f;
    public float gravity = -25f;
    public float maxFallSpeed = -9f;
    public float fallSpeedModifier = 1f;
    public float gravityFallModifier = 1f;

    [Header("Combat Stats")]
    public float attackPower = 10f;
    public float attackSpeed = 1.5f;

    [Header("Health Stats")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keeps the instance between scene loads
            ResetHealth(); // Ensure health is at max at the start
        }
        else
        {
            Destroy(gameObject); // Ensure there's only one instance
        }
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }
}