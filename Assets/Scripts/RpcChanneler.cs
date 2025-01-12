using Cysharp.Net.Http;
using Grpc.Net.Client;
using MagicOnion.Client;

public class RpcChanneler
{
	public T Create<T>()
	where
		T : MagicOnion.IService<T>
	{
		var handler = new YetAnotherHttpHandler();
		handler.Http2Only = true;
		var options = new GrpcChannelOptions();
		options.HttpHandler = handler;
		options.UnsafeUseInsecureChannelCallCredentials = false;
		var channel = GrpcChannel.ForAddress("http://localhost:5000", options);
		return MagicOnionClient.Create<T>(channel);
	}
}
