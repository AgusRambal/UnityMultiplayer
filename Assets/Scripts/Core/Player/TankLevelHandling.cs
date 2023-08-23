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
    private bool inLevelOne = true;
    private bool inLevelTwo = false;
    private bool inLevelThree = false;

    private void Start()
    {
        if (player.level == 1)
        {
            inLevelOne = true;
            inLevelTwo = false;
            inLevelThree = false;
        }

        if (player.level == 2) 
        {
            inLevelOne = false;
            inLevelTwo = true;
            inLevelThree = false;
        }

        if (player.level == 3) 
        {
            inLevelOne = false;
            inLevelTwo = false;
            inLevelThree = true;
        }
    }

    public void LevelUpCheck(CoinWallet wallet)
    {
        if (!inLevelOne && wallet.totalCoins.Value < coinsForLevelTwo) 
        {
            SetLevel(playerLvl1, 1);

            inLevelOne = true;
            inLevelTwo = false;
            inLevelThree = false;
        }

        if (!inLevelTwo && wallet.totalCoins.Value >= coinsForLevelTwo && wallet.totalCoins.Value < coinsForLevelThree)
        {
            SetLevel(playerLvl2, 2);

            inLevelOne = false;
            inLevelTwo = true;
            inLevelThree = false;
        }

        if (!inLevelThree && wallet.totalCoins.Value >= coinsForLevelThree)
        {
            SetLevel(playerLvl3, 3);

            inLevelOne = false;
            inLevelTwo = false;
            inLevelThree = true;
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
