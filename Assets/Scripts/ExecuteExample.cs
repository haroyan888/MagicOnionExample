using Assets.Scripts.Shared;
using UnityEngine;

public class ExecuteExample : MonoBehaviour
{
	async void Start()
	{
		IMyFirstService client = new RpcChanneler().Create<IMyFirstService>();
		var result = await client.SumAsync(1, 2);
		Debug.Log($"Result: {result}");
	}
}