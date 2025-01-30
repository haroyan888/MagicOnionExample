using UnityEngine;
using Cysharp.Net.Http;
using Grpc.Net.Client;
using System.Threading.Tasks;

public class PlayerController : MonoBehaviour
{
    private GrpcChannel _channel;
    private GamingHubClient _hub; // 追加
    private Vector3 _position;
    [SerializeField]
    private float _speed = 0.1f;
    [SerializeField]
    private string _serverUrl = "http://localhost:5000";

    void Awake()
    {
        var handler = new YetAnotherHttpHandler();
        handler.Http2Only = true;
        var options = new GrpcChannelOptions();
        options.HttpHandler = handler;
        options.UnsafeUseInsecureChannelCallCredentials = false;
        _channel = GrpcChannel.ForAddress(_serverUrl, options);
        _position = new Vector3(0.0f, 0.0f);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        var id = Random.Range(0, 10000);
        _hub = new GamingHubClient();
        await _hub.ConnectAsync(_channel, "Room", $"Player-{id}");
    }

    // Update is called once per frame
    async void Update()
    {
        if (_hub == null)
        {
            return;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            _position += Vector3.up * _speed;
            await _hub.MoveAsync(_position, new Quaternion());
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            _position += Vector3.down * _speed;
            await _hub.MoveAsync(_position, new Quaternion());
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            _position += Vector3.left * _speed;
            await _hub.MoveAsync(_position, new Quaternion());
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            _position += Vector3.right * _speed;
            await _hub.MoveAsync(_position, new Quaternion());
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("Leave");
            await _hub.LeaveAsync();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            await _hub.LeaveAsync();
            var id = Random.Range(0, 10000);
            await _hub.ConnectAsync(_channel, "Room-1", $"Player-{id}");
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            await _hub.LeaveAsync();
            var id = Random.Range(0, 10000);
            await _hub.ConnectAsync(_channel, "Room", $"Player-{id}");
        }
    }

    async void OnDestroy()
    {
        // 追加
        if (_hub != null)
        {
            await _hub.DisposeAsync();
        }
        if (_channel != null)
        {
            await _channel.ShutdownAsync();
        }
    }
}
