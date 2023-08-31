using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class LeaderboardEntityDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text displayText;
    [SerializeField] private Color myColour;
    public ulong clientID { get; private set; }
    public int coins{ get; private set; }
    public int kills{ get; private set; }

    private FixedString32Bytes playerName;

    public void Initialize(ulong clientID, FixedString32Bytes playerName, int coins, int kills)
    { 
        this.clientID = clientID;
        this.playerName = playerName;
        this.coins = coins;
        this.kills = kills;

        if (clientID == NetworkManager.Singleton.LocalClientId)
        {
            displayText.color = myColour;
        }

        //UpdateCoins(coins);
        UpdateKills(kills);
    }

    public void UpdateCoins(int coins)
    {
        this.coins = coins;
        UpdateText();
    }

    public void UpdateKills(int kills)
    {
        this.kills = kills;
        UpdateText();
    }

    public void UpdateText()
    {
        //displayText.text = $"{transform.GetSiblingIndex() + 1}. {playerName} ({coins})";
        displayText.text = $"{transform.GetSiblingIndex() + 1}. {playerName} ({kills})";
    }
}
