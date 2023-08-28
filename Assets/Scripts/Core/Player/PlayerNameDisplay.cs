using System;
using System.Collections;
using TMPro;
using Unity.Collections;
using UnityEngine;

public class PlayerNameDisplay : MonoBehaviour
{
    [SerializeField] private PlayerInstance player;
    [SerializeField] private TMP_Text playerNameText;

    private void Start()
    {
        HandlePlayerNameChanged(string.Empty, player.playerName.Value);
        player.playerName.OnValueChanged += HandlePlayerNameChanged;
    }

    private void HandlePlayerNameChanged(FixedString32Bytes previousName, FixedString32Bytes newName)
    {
        playerNameText.text = newName.ToString();
    }

    private void OnDestroy()
    {
        player.playerName.OnValueChanged -= HandlePlayerNameChanged;

    }
}
