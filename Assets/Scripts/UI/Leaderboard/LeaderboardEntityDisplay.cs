using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class LeaderboardEntityDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text displayText;
    [SerializeField] private TMP_Text positionText;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text killsText;
    [SerializeField] private TMP_Text deathsText;
    [SerializeField] private TMP_Text ratioText;
    [SerializeField] private TMP_Text pointsText;
    [SerializeField] private Color myColour;
    public ulong clientID { get; private set; }
    public int points { get; private set; }
    public int deaths{ get; private set; }
    public float ratio{ get; private set; }
    public int totalKills { get; private set; }

    public FixedString32Bytes playerName;

    [SerializeField] private bool useExtendDisplay;

    public void Initialize(ulong clientID, FixedString32Bytes playerName, int points, int deaths, int totalKills)
    { 
        this.clientID = clientID;
        this.playerName = playerName;
        this.points = points;
        this.deaths = deaths;
        this.totalKills = totalKills;

        if (useExtendDisplay)
        {
            if (clientID == NetworkManager.Singleton.LocalClientId)
            {
                nameText.color = myColour;
            }
        }

        else
        {
            if (clientID == NetworkManager.Singleton.LocalClientId)
            {
                displayText.color = myColour;
            }
        }

        UpdateKills(points, deaths, totalKills);
    }

    public void UpdateKills(int points, int deaths, int totalKills)
    {
        this.points = points;
        this.deaths = deaths;
        this.totalKills = totalKills;

        if (deaths == 0)
        {
            ratio = totalKills;
        }

        else
        {
            ratio = totalKills / deaths;
        }

        UpdateText();
    }

    public void UpdateText()
    {
        if (useExtendDisplay)
        {
            positionText.text = $"{transform.GetSiblingIndex() + 1}";
            nameText.text = $"{playerName}";
            pointsText.text = $"{points}";
            killsText.text = $"{totalKills}";
            deathsText.text = $"{deaths}";

            if (deaths == 0)
            {
                ratioText.text = $"{ratio}.0";
            }

            else
            {
                ratioText.text = $"{ratio}";
            }
        }

        else
        {
            displayText.text = $"{transform.GetSiblingIndex() + 1}. {playerName} ({points})";
        }
    }
}
