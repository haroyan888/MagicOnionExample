using Assets.Scripts.Shared;
using UnityEngine;

public class ExecuteExample : MonoBehaviour
{
	[SerializeField]
	string host;

	async void Start()
	{
		IMyFirstService client = RpcClientMaker.Create<IMyFirstService>(host);
		var result = await client.SumAsync(1, 2);
		Debug.Log($"Result: {result}");
	}
}