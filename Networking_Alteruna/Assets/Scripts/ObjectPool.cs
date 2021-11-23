using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpawnerSynchronizable))]
public class ObjectPool : MonoBehaviour
{
	public int objectsPerPlayer;

	private SpawnerSynchronizable _spawner;
	private List<int> _availableIndices = new List<int>();
	private Dictionary<GameObject, int> _usedIndices = new Dictionary<GameObject, int>();
	private UInt16 _playerId;

	private void Awake()
	{
		PlayerInputManager.Instance.onPlayerJoined += OnPlayerJoin;
		_spawner = GetComponent<SpawnerSynchronizable>();
	}

	private void OnPlayerJoin()
	{
		PlayerInputManager.Instance.onPlayerJoined -= OnPlayerJoin;
		
		PlayerMovementSync _player = GetComponentInParent<PlayerMovementSync>();
		if (_player != PlayerInputManager.Instance.MyPlayer)
			return;
		
		_playerId = PlayerInputManager.Instance.MyPlayerId;

		for (int i = 0; i < objectsPerPlayer; i++)
		{
			_spawner.Spawn(Vector3.zero, Quaternion.identity, Vector3.one);
			_availableIndices.Add(i);
		}
	}

	public GameObject GetObject()
	{
		if (_playerId != PlayerInputManager.Instance.MyPlayerId)
			return null;
		
		int index = _availableIndices[0];
		_availableIndices.RemoveAt(0);
		GameObject obj = _spawner.GetObjects()[_playerId * objectsPerPlayer + index];
		_usedIndices.Add(obj, index);

		return obj;
	}

	public void ReturnObject(GameObject obj)
	{
		if (_playerId != PlayerInputManager.Instance.MyPlayerId)
			return;
		
		if (!_usedIndices.ContainsKey(obj))
		{
			Debug.LogWarning("Tried to return object but it hasn't been claimed");
			return;
		}
		
		_availableIndices.Add(_usedIndices[obj]);
		_usedIndices.Remove(obj);
	}
}