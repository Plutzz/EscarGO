using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerProjectileManager : NetworkBehaviour
{
    [SerializeField] private FoodProjectile projectile;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner)
        {
            return;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ThrowProjectileServerRpc(Vector3 position, Vector3 forward, Quaternion rotation)
    {
        ThrowProjectileClientRpc(position, forward, rotation);
    }

    [ClientRpc]
    private void ThrowProjectileClientRpc(Vector3 position, Vector3 forward, Quaternion rotation)
    {
        FoodProjectile NewProjectile = Instantiate(projectile, position + forward, rotation);
        NewProjectile.Launch();
    }
}
