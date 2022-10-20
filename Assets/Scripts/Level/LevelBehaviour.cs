using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerBomber
{
    public class LevelBehaviour : MonoBehaviour
    {
        private CinemachineVirtualCamera _virtualCamera;
        private SpawnPoint[] _playerSpawnPoints;

        private void Awake()
        {
            _virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>(true);
            _playerSpawnPoints = GetComponentsInChildren<SpawnPoint>(true);
        }

        public void SetCameraToFollow(Transform objectToFollow)
        {
            _virtualCamera.Follow = objectToFollow;
        }

        public Vector3 GetPlayerSpawnPoint()
        {
            for (int i = 0; i < _playerSpawnPoints.Length; i++)
            {
                if (!_playerSpawnPoints[i].isOccupied)
                {
                    _playerSpawnPoints[i].isOccupied = true;
                    return _playerSpawnPoints[i].transform.position;
                }
            }

            return Vector3.zero;
        }
    }
}
