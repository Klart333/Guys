using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderDamager : MonoBehaviour
{
    [SerializeField]
    private int damage = 1;

    [SerializeField]
    private float frequency = 5;

    private List<PlayerHealth> playerHealths = new List<PlayerHealth>();

    private float timer = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerHealth>(out PlayerHealth health))
        {
            playerHealths.Add(health);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<PlayerHealth>(out PlayerHealth health))
        {
            if (playerHealths.Contains(health))
            {
                playerHealths.Remove(health);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        timer += Time.deltaTime;
        if (timer >= 1.0f / frequency)
        {
            timer = 0;

            int removed = 0;
            for (int i = 0; i < playerHealths.Count - removed; i++)
            {
                if (playerHealths[i - removed].TakeDamage(damage))
                {
                    playerHealths.Remove(playerHealths[i - removed++]);
                }
            }
        }
    }
}