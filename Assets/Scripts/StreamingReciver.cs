using UnityEngine;
using MagicOnion.Client;
using System.Threading.Tasks;
using Grpc.Core;
using System.Collections.Generic;
using TMPro;

public class GamingHubClient : IGamingHubReceiver
{
	Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();

	IGamingHub client;

	public async ValueTask<GameObject> ConnectAsync(ChannelBase grpcChannel, string roomName, string playerName)
	{
		client = await StreamingHubClient.ConnectAsync<IGamingHub, IGamingHubReceiver>(grpcChannel, this);

		var roomPlayers = await client.JoinAsync(roomName, playerName, Vector3.zero, Quaternion.identity);
		foreach (var player in roomPlayers)
		{
			if (player.Name != playerName)
				(this as IGamingHubReceiver).OnJoin(player);
		}

		return players[playerName];
	}

	// methods send to server.

	public ValueTask LeaveAsync()
	{
		foreach (var player in players)
			Object.Destroy(player.Value);

		return client.LeaveAsync();
	}

	public async ValueTask<GameObject> ChangeRoomAsync(ChannelBase grpcChannel, string roomName, string playerName)
	{
		return await ConnectAsync(grpcChannel, roomName, playerName);
	}

	public ValueTask MoveAsync(Vector3 position, Quaternion rotation)
	{
		return client.MoveAsync(position, rotation);
	}

	// dispose client-connection before channel.ShutDownAsync is important!
	public Task DisposeAsync()
	{
		return client.DisposeAsync();
	}

	// You can watch connection state, use this for retry etc.
	public Task WaitForDisconnect()
	{
		return client.WaitForDisconnect();
	}

	// Receivers of message from server.

	void IGamingHubReceiver.OnJoin(Player player)
	{
		Debug.Log("Join Player:" + player.Name);

		var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		cube.name = player.Name;
		cube.transform.SetPositionAndRotation(player.Position, player.Rotation);
		players[player.Name] = cube;
	}

	void IGamingHubReceiver.OnLeave(Player player)
	{
		GameObject.Find("Log").GetComponent<TextMeshProUGUI>().text = "hoge";

		if (players.TryGetValue(player.Name, out var cube))
		{
			GameObject.Destroy(cube);
		}
	}

	void IGamingHubReceiver.OnMove(Player player)
	{
		Debug.Log("Move Player:" + player.Name);

		if (players.TryGetValue(player.Name, out var cube))
		{
			cube.transform.SetPositionAndRotation(player.Position, player.Rotation);
		}
	}
}