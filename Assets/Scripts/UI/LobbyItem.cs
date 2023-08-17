using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Services.Lobbies.Models;

public class LobbyItem : MonoBehaviour
{
    [SerializeField] private TMP_Text lobbyNameText;
    [SerializeField] private TMP_Text lobbyPlayersText;

    private LobbiesLists lobbiesLists;
    private Lobby lobby;

    public void Initialize(LobbiesLists lobbiesLists, Lobby lobby)
    { 
        this.lobbiesLists = lobbiesLists;
        this.lobby = lobby;

        lobbyNameText.text = lobby.Name;
        lobbyPlayersText.text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";
    }

    public void Join()
    {
        lobbiesLists.JoinAsync(lobby);
    }
}
