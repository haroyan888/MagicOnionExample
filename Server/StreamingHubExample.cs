using MagicOnion.Server.Hubs;
using UnityEngine;

// Server implementation
// implements : StreamingHubBase<THub, TReceiver>, THub
public class GamingHub : StreamingHubBase<IGamingHub, IGamingHubReceiver>, IGamingHub
{
	// this class is instantiated per connected so fields are cache area of connection.
	IGroup room;
	Player self;
	IInMemoryStorage<Player> storage;

	public async ValueTask<Player[]> JoinAsync(string roomName, string userName, Vector3 position, Quaternion rotation)
	{
		self = new Player() { Name = userName, Position = position, Rotation = rotation };

		// Group can bundle many connections and it has inmemory-storage so add any type per group.
		(room, storage) = await Group.AddAsync(roomName, self);

		// Typed Server->Client broadcast.
		Broadcast(room).OnJoin(self);

		return storage.AllValues.ToArray();
	}

	public async ValueTask LeaveAsync()
	{
		await room.RemoveAsync(Context);
		Broadcast(room).OnLeave(self);
	}

	public async ValueTask MoveAsync(Vector3 position, Quaternion rotation)
	{
		self.Position = position;
		self.Rotation = rotation;
		Broadcast(room).OnMove(self);
	}

	// You can hook OnConnecting/OnDisconnected by override.
	protected override ValueTask OnDisconnected()
	{
		// on disconnecting, if automatically removed this connection from group.
		Broadcast(room).OnLeave(self);
		return ValueTask.CompletedTask;
	}
}
