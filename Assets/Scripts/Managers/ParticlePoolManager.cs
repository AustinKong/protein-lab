using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class ParticlePoolManager : MonoBehaviour
{
  public static ParticlePoolManager Instance;

  [System.Serializable]
  public class ParticlePrefab {
    public string particleName;
    public GameObject particlePrefab;
  }

  [SerializeField] private List<ParticlePrefab> particlePrefabs;
  private const int INITIAL_POOL_SIZE = 0;

  private Dictionary<string, Queue<ParticleSystem>> particlePools;

  public void Awake() {
    if (Instance == null) {
      Instance = this;
    } else {
      Destroy(gameObject);
    }
  }

  public void Start() {
    InitializePools();
  }

  private void InitializePools() {
    particlePools = new Dictionary<string, Queue<ParticleSystem>>();

    foreach (ParticlePrefab particlePrefab in particlePrefabs) {
      Queue<ParticleSystem> pool = new Queue<ParticleSystem>();
      for (int i = 0; i < INITIAL_POOL_SIZE; i++) {
        pool.Enqueue(CreateParticleInstance(particlePrefab.particlePrefab));
      }
      particlePools[particlePrefab.particleName] = pool;
    }
  }

  private ParticleSystem CreateParticleInstance(GameObject prefab) {
    GameObject obj = Instantiate(prefab, transform);
    obj.SetActive(false);
    return obj.GetComponent<ParticleSystem>();
  }

  public void PlayParticle(string particleName, Vector3 position) {
    if (particlePools.TryGetValue(particleName, out Queue<ParticleSystem> pool)) {
      if (pool.Count == 0) {
        ParticlePrefab particlePrefab = particlePrefabs.Find(p => p.particleName == particleName);
        pool.Enqueue(CreateParticleInstance(particlePrefab.particlePrefab));
      }

      ParticleSystem particleSystem = pool.Dequeue();
      particleSystem.gameObject.SetActive(true);
      particleSystem.transform.position = position;
      particleSystem.Play();
      StartCoroutine(CleanUpParticle(particleSystem, pool));
    } else {
      Debug.LogError($"Unknown particle system with name {particleName}");
    }
  }

  private IEnumerator CleanUpParticle(ParticleSystem particleSystem, Queue<ParticleSystem> pool) {
    yield return new WaitForSeconds(particleSystem.main.duration);
    pool.Enqueue(particleSystem);
    particleSystem.gameObject.SetActive(false);
  }
}
