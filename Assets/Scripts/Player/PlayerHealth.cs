using System;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    [Header("Settings")]
    public int maxHealth = 100;

    // FishNet SyncVar
    public readonly SyncVar<int> currentHealth = new SyncVar<int>();

    [Header("References")]
    private Player player;

    // UI veya diğer sistemler için Eventler
    public static event Action<int, int> OnLocalHealthChanged;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void OnEnable()
    {
        currentHealth.OnChange += HealthSync;
    }

    private void OnDisable()
    {
        currentHealth.OnChange -= HealthSync;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        currentHealth.Value = maxHealth;
    }

    public override void OnOwnershipClient(NetworkConnection prevOwner)
    {
        base.OnOwnershipClient(prevOwner);
        if (IsOwner)
        {
            OnLocalHealthChanged?.Invoke(currentHealth.Value, maxHealth);
        }
    }

    [Server]
    public void TakeDamage(int damageAmount)
    {
        if (currentHealth.Value <= 0) return; // Zaten ölüyse işlem yapma (Guard Clause)

        currentHealth.Value -= damageAmount;

        if (currentHealth.Value <= 0)
        {
            DieObserversRpc();
        }
    }

    // İstemcinin kendini öldürme isteği (Örn: zehir, düşme, intihar komutu)
    public void Die()
    {
        if (currentHealth.Value <= 0) return;
        ForceKillServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void ForceKillServerRpc()
    {
        if (currentHealth.Value <= 0) return; // Sunucuda çifte ölümü engelle

        currentHealth.Value = 0;
        DieObserversRpc();
    }

    [ObserversRpc(RunLocally = true)]
    private void DieObserversRpc()
    {
        // State zaten Dead ise hiçbir şey yapma
        if (player.stateMachine.currentState == player.deadState) return;

        player.stateMachine.ChangeState(player.deadState);
    }
    
    private void HealthSync(int oldVal, int newVal, bool asServer)
    {
        // Sadece bu objenin sahibi olan istemcide UI güncellenir
        if (IsOwner)
        {
            OnLocalHealthChanged?.Invoke(newVal, maxHealth);
        }
    }
}