using UnityEngine;
using Mirror;
using Steamworks;

public class PlayerObjectController : NetworkBehaviour
{
	//Player Data
	[SyncVar] public int ConnectionID;
	[SyncVar] public int PlayerIdNumber;
	[SyncVar] public ulong PlayerSteamID;
	[SyncVar(hook = nameof(PlayerNameUpdate))] public string PlayerName;
	[SyncVar(hook = nameof(PlayerReadyUpdate))] public bool Ready;

	private CustomNetworkManager manager;

	private CustomNetworkManager Manager
	{
		get
		{
			if(manager != null)
			{
				return manager;
			}

			return manager = CustomNetworkManager.singleton as CustomNetworkManager;
		}
	}

	public override void OnStartAuthority()
	{
		CmdSetPlayerName(SteamFriends.GetPersonaName().ToString());
		gameObject.name = "LocalGamePlayer";
		LobbyController.instance.FindLocalPlayer();
		LobbyController.instance.UpdateLobbyName();
	}

	public override void OnStartClient()
	{
		Manager.GamePlayers.Add(this);
		LobbyController.instance.UpdateLobbyName();
		LobbyController.instance.UpdatePlayerList();
	}

	public override void OnStopClient()
	{
		Manager.GamePlayers.Remove(this);
		LobbyController.instance.UpdatePlayerList();
	}

	void PlayerReadyUpdate(bool oldValue, bool newValue)
	{
		if(isServer)
		{
			this.Ready = newValue;
		}
		if(isClient)
		{
			LobbyController.instance.UpdatePlayerList();
		}
	}

	public void ChangeReady()
	{
		if(hasAuthority)
		{
			CMDSetPlayerReady();
		}
	}


	[Command]
	private void CMDSetPlayerReady()
	{
		this.PlayerReadyUpdate(this.Ready, !this.Ready);
	}

	[Command]
	private void CmdSetPlayerName(string PlayerName)
	{
		this.PlayerNameUpdate(this.PlayerName, PlayerName);
	}

	public void PlayerNameUpdate(string OldValue, string NewValue)
	{
		if(isServer)
		{
			this.PlayerName = NewValue;
		}
		if(isClient)
		{
			LobbyController.instance.UpdatePlayerList();
		}
	}
}
