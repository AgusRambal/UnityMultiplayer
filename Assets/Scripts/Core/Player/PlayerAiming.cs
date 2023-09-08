using Unity.Netcode;
using UnityEngine;


public class PlayerAiming : NetworkBehaviour
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform turretTransfoorm;

    private void LateUpdate()
    {
        if (!IsOwner)
            return;

        if (!SingeltonGameManaher.instance.startGame.Value)
            return;

        Vector2 aimScreenPosition = inputReader.AimPosition;
        Vector2 aimWorldPosition = Camera.main.ScreenToWorldPoint(aimScreenPosition);

        turretTransfoorm.up = new Vector2(aimWorldPosition.x - turretTransfoorm.position.x, aimWorldPosition.y - turretTransfoorm.position.y);
    }
}
