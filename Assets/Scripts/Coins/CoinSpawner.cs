using UnityEngine;

public class CoinSpawner : MonoBehaviour {

    [SerializeField] GameObject _coinPrefab;
    [SerializeField] Transform _spawnPoint;

    [Header("ThrowForce")]
    [SerializeField] Vector3 _throwDir = new Vector3(0f, 1f, 1f);
    [SerializeField] Vector2 _throwSpeedRange = new Vector2(10f, 25f);
    // [SerializeField] float _throwDirVariance = .2f;
    [SerializeField] Vector3 _impulseAngularVelocity;

    public void SpawnCoin() {
        GameObject spawnObj = Instantiate(_coinPrefab, _spawnPoint.position, _coinPrefab.transform.rotation);
        // Rigidbody rigidbody = spawnObj.GetComponent<Rigidbody>();
        // if (rigidbody != null) {
        //     float speed = Random.Range(_throwSpeedRange.x, _throwSpeedRange.y);
        //     rigidbody.AddForce(_throwDir.normalized * speed, ForceMode.Impulse);
        // }
    }

    void OnDrawGizmos() {
        if (_spawnPoint == null) return;
        Gizmos.color = Color.green;
        Vector3 start = _spawnPoint.position;
        Vector3 end = _spawnPoint.position + _throwDir.normalized * 2;
        Gizmos.DrawLine(start, end);
    }
}
