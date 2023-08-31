using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class LeaderboardEntityDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text displayText;
    [SerializeField] private Color myColour;
    public ulong clientID { get; private set; }
    public int kills{ get; private set; }

    private FixedString32Bytes playerName;

    public void Initialize(ulong clientID, FixedString32Bytes playerName, int kills)
    { 
        this.clientID = clientID;
        this.playerName = playerName;
        this.kills = kills;

        if (clientID == NetworkManager.Singleton.LocalClientId)
        {
            displayText.color = myColour;
        }

        UpdateKills(kills);
    }

    public void UpdateKills(int kills)
    {
        this.kills = kills;
        UpdateText();
    }

    public void UpdateText()
    {
        displayText.text = $"{transform.GetSiblingIndex() + 1}. {playerName} ({kills})";
    }
}
