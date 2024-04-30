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
    public void ThrowProjectileServerRpc(Vector3 position, Quaternion rotation, float force, ServerRpcParams serverRpcParams = default)
    {
        
        ThrowProjectileClientRpc(position, rotation, serverRpcParams.Receive.SenderClientId, force);
    }

    [ClientRpc]
    private void ThrowProjectileClientRpc(Vector3 position, Quaternion rotation, ulong thrower, float force)
    {
        FoodProjectile NewProjectile = Instantiate(projectile, position, rotation);
        NewProjectile.Launch(thrower, force);
    }
}
