using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class CountDown : NetworkBehaviour
{
    [SerializeField] private int timer;
    private CountDownUI _countDownUI;
    private NetworkVariable<int> _timer;
    private GameState _gameState;

    private void Awake()
    {
        _countDownUI = FindObjectOfType<CountDownUI>();
        _gameState = FindObjectOfType<GameState>();
        _timer = new NetworkVariable<int>(timer);
        _timer.OnValueChanged += (_, newValue) => { _countDownUI.UpdateUI(newValue); };
        _countDownUI.UpdateUI(_timer.Value);
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnClientConnected(ulong obj)
    {
        if (_timer.Value <= 0)
        {
            _countDownUI.Hide();
        }
    }

    private void Start()
    {
        if (!IsServer) return;
        StartCoroutine(Count());
    }

    private IEnumerator Count()
    {
        yield return new WaitForSeconds(.5f);
        while (_timer.Value > 0)
        {
            yield return new WaitForSeconds(.5f);
            _timer.Value--;
        }

        _gameState.StartGameServerRpc();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (NetworkManager.Singleton == null) return;
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
    }
}