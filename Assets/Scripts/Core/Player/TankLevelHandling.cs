using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class TankLevelHandling : NetworkBehaviour
{
    [SerializeField] private PlayerInstance player;
    [SerializeField] private Health health;
    [SerializeField] private ProjectileLauncher projectile;

    [SerializeField] private PlayerInstance playerLvl1;
    [SerializeField] private PlayerInstance playerLvl2;
    [SerializeField] private PlayerInstance playerLvl3;

    [SerializeField] private int coinsForLevelTwo = 50;
    [SerializeField] private int coinsForLevelThree = 100;

    private Vector2 lastPos;
    private bool passedLevelTwo = false;
    private bool passedLevelThree = false;

    private void Start()
    {
        if (KDManager.instance.playerLevel == 2)
        {
            passedLevelTwo = true;
        }

        if (KDManager.instance.playerLevel >= 3)
        {
            passedLevelTwo = true;
            passedLevelThree = true;
        }
    }

    public void LevelUpCheck(CoinWallet wallet)
    {
        if (!passedLevelTwo && wallet.totalCoins.Value >= coinsForLevelTwo)
        {
            SetLevel(playerLvl2, 2);
            passedLevelTwo = true;
        }

        if (!passedLevelThree && wallet.totalCoins.Value >= coinsForLevelThree)
        {
            SetLevel(playerLvl3, 3);
            passedLevelThree = true;
        }
    }

    public void SetLevel(PlayerInstance playerLvl, int level)
    {
        lastPos = (Vector2)transform.position;

        KDManager.instance.playerLevel = level;

        EventManager.TriggerEvent(GenericEvents.HandlePlayerLevel, new Hashtable() {
        {GameplayEventHashtableParams.Player.ToString(), player},
        {GameplayEventHashtableParams.PlayerLVL.ToString(), playerLvl},
        {GameplayEventHashtableParams.PlayerPos.ToString(), lastPos}
        });
    }
}
