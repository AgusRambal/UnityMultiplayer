using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class TankLevelHandling : NetworkBehaviour
{
    [SerializeField] private Player player;

    [SerializeField] private Player playerLvl1;
    [SerializeField] private Player playerLvl2;
    [SerializeField] private Player playerLvl3;

    [SerializeField] private int coinsForLevelTwo = 50;
    [SerializeField] private int coinsForLevelThree = 100;

    private Vector2 lastPos;
    private bool passedLevelTwo = false;
    private bool passedLevelThree = false;

    private void Start()
    {
        if (player.level == 2)
        {
            passedLevelTwo = true;
        }

        if (player.level == 3)
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

    public void SetLevel(Player playerLvl, int level)
    {
        lastPos = (Vector2)transform.position;
        player.level = level;

        EventManager.TriggerEvent(GenericEvents.HandlePlayerLevel, new Hashtable() {
        {GameplayEventHashtableParams.Player.ToString(), player},
        {GameplayEventHashtableParams.PlayerLVL.ToString(), playerLvl},
        {GameplayEventHashtableParams.PlayerPos.ToString(), lastPos}
        });
    }
}
