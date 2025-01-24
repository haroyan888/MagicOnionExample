using Cysharp.Net.Http;
using Grpc.Net.Client;
using MagicOnion.Client;

class RpcClientMaker
{
	public static T Create<T>(string host)
	where
		T : MagicOnion.IService<T>
	{
		var handler = new YetAnotherHttpHandler();
		handler.Http2Only = true;
		var options = new GrpcChannelOptions();
		options.HttpHandler = handler;
		options.UnsafeUseInsecureChannelCallCredentials = false;
		var channel = GrpcChannel.ForAddress(host, options);
		return MagicOnionClient.Create<T>(channel);
	}
}