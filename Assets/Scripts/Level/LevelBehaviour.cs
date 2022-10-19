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

        public SpawnPoint GetPlayerSpawnPoint(int id)
        {
            return _playerSpawnPoints[id].GetComponent<SpawnPoint>();
        }
    }
}
