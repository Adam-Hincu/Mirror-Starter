using UnityEngine;
using Mirror;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Steamworks;

public class CustomNetworkManager : NetworkManager
{
	[SerializeField] private PlayerObjectController GamePlayerPrefab;
	public List<PlayerObjectController> GamePlayers { get; } = new List<PlayerObjectController>();

	public override void OnServerAddPlayer(NetworkConnection conn)
	{
		if(SceneManager.GetActiveScene().name == "Lobby")
		{
			PlayerObjectController GamePlayerInstance = Instantiate(GamePlayerPrefab);

			GamePlayerInstance.ConnectionID = conn.connectionId;
			GamePlayerInstance.PlayerIdNumber = GamePlayers.Count + 1;
			GamePlayerInstance.PlayerSteamID = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.instance.CurrentLobbyID, GamePlayers.Count);

			NetworkServer.AddPlayerForConnection(conn, GamePlayerInstance.gameObject);
		}
	}

	void GetTransport()
	{
		Transport.activeTransport = GetComponent<Transport>();
	}

	private void Start()
	{
		GetTransport();
	}

}
