using UnityEngine;
using Mirror;
using Steamworks;
using TMPro;
using UnityEngine.UI;

public class SteamLobby : MonoBehaviour
{
	public static SteamLobby instance;

	//Callbacks
	protected Callback<LobbyCreated_t> LobbyCreated;
	protected Callback<GameLobbyJoinRequested_t> JoinRequest;
	protected Callback<LobbyEnter_t> LobbyEntered;

	//Variables
	public ulong CurrentLobbyID;
	private const string HostAddressKey = "HostAddress";
	public CustomNetworkManager manager;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		if(!SteamManager.Initialized) { Debug.Log("Steam not Open;"); return; }

		LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
		JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
		LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
	}

	public void HostLobby()
	{
		SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, manager.maxConnections);
	}

	private void OnLobbyCreated(LobbyCreated_t callback)
	{
		if (callback.m_eResult != EResult.k_EResultOK) { return; }

		Debug.Log("Lobby created Succesfully!");

		manager.StartHost();

		SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());
		SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", SteamFriends.GetPersonaName().ToString() + "'s Lobby.");
	}

	private void OnJoinRequest(GameLobbyJoinRequested_t callback)
	{
		Debug.Log("Request to join Lobby.");

		SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
	}

	private void OnLobbyEntered(LobbyEnter_t callback)
	{
		//Everyone
		CurrentLobbyID = callback.m_ulSteamIDLobby;

		//Client
		if (NetworkServer.active) { return; }

		manager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);

		manager.StartClient();
	}
}
