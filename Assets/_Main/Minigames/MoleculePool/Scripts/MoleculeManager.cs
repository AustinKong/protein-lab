using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MoleculeGroup {
  public List<Molecule> molecules;
  public float dissolveTimer;
}

public class MoleculeManager : MonoBehaviour
{
  public static MoleculeManager Instance;

  [SerializeField] private Tilemap tilemap;
  [SerializeField] private GameObject moleculePrefab;
  [SerializeField] private TMP_Text scoreDisplay;
  [SerializeField] private TMP_Text timerDisplay;
  [SerializeField] private Sprite completionSprite;

  private List<MoleculeGroup> moleculeGroups = new();
  private int points = 0;
  private int timer = 90;

  private const float GRID_TOP = 5f;
  private const float GRID_BOTTOM = -1f;
  private const float GRID_LEFT = -9f;
  private const float GRID_RIGHT = 9f;
  private const int SEED_COUNT = 2;
  private const int DISSOLVE_TIMER = 40;

  private void Awake() {
    if (Instance == null) {
      Instance = this;
    } else {
      Destroy(gameObject);
    }
  }

  private void Start() {
    for (int i = 0; i < SEED_COUNT; i++) {
      GenerateSeed();
    }
    InvokeRepeating("Tick", 1, 1);
  }
  
  private void GenerateSeed() {
    Vector2 pos = 
      AlignToGrid(
        new Vector2(
        UnityEngine.Random.Range(GRID_LEFT, GRID_RIGHT),
        UnityEngine.Random.Range(GRID_BOTTOM, GRID_TOP))
      );
    Collider2D[] cols = Physics2D.OverlapPointAll(pos);

    foreach (Collider2D col in cols) {
      if (col.GetComponent<Molecule>() != null) {
        GenerateSeed();
        return;
      }
    }

    GameObject seed = Instantiate(
      moleculePrefab,
      pos,
      Quaternion.identity);
    CreateGroup(seed.GetComponent<Molecule>());
  }

  private void Tick() {
    if (timer == 1) {
      MinigameManager.Instance.Completion(completionSprite, "Great Job", points / 2000f);
      timer -= 1;
      timerDisplay.text = timer.ToString() + "s";
      return;
    } else if (timer < 1) {
      return;
    }

    timer -= 1;
    timerDisplay.text = timer.ToString() + "s";

    List<MoleculeGroup> scheduleRemoval = new();
    foreach (MoleculeGroup group in moleculeGroups) {
      group.dissolveTimer--;
      if (group.dissolveTimer <= 0) {
        scheduleRemoval.Add(group);
      }
    }

    foreach (MoleculeGroup group in scheduleRemoval) {
      if (group.molecules.Count >= 6) {
        points += group.molecules.Count * 100;
        scoreDisplay.text = points.ToString();
      }

      moleculeGroups.Remove(group);
      StartCoroutine(DissolveRoutine(group.molecules));
    }

    // Replenish seeds
    int seedsNeeded = SEED_COUNT - moleculeGroups.Count;
    for (int i = 0; i < seedsNeeded; i++) {
      GenerateSeed();
    }
  }

  private const float ANIMATION_DURATION = 0.8f;

  private IEnumerator DissolveRoutine(List<Molecule> molecules) {
    List<Vector3> originalPositions = new();
    Vector3 center = Vector2.zero;
    foreach (Molecule molecule in molecules) {
      center += molecule.transform.position;
      originalPositions.Add(molecule.transform.position);
      molecule.GetComponent<Collider2D>().enabled = false;
    }
    center /= molecules.Count;

    List<Vector3> packedPositions = GeneratePackedCirclePositions(molecules.Count, center, 0.8f);

    float elapsedTime = 0;
    while (elapsedTime < ANIMATION_DURATION) {
      elapsedTime += Time.deltaTime;
      for (int i = 0; i < molecules.Count; i++) {
        molecules[i].transform.position = Vector3.Slerp(originalPositions[i], packedPositions[i], elapsedTime / ANIMATION_DURATION);
      }
      yield return null;
    }
    yield return new WaitForSeconds(0.4f);

    foreach (Molecule molecule in molecules) {
      ParticlePoolManager.Instance.PlayParticle("Shards", molecule.transform.position);
      Destroy(molecule.gameObject);
    }
  }

  private void CreateGroup(Molecule molecule) {
    moleculeGroups.Add(new MoleculeGroup(){
      molecules = new List<Molecule>{ molecule },
      dissolveTimer = DISSOLVE_TIMER + UnityEngine.Random.Range(-2, 3)
    });
  }

  public void Merge(Molecule a, Molecule b) {
    MoleculeGroup groupA = moleculeGroups.Find(group => group.molecules.Contains(a));
    MoleculeGroup groupB = moleculeGroups.Find(group => group.molecules.Contains(b));

    if (a.transform.position.y < -3f) {
      Destroy(a.gameObject);
      return;
    }
    if (b.transform.position.y < -3f) {
      Destroy(b.gameObject);
      return;
    }

    if (groupA == groupB || (groupA == null && groupB == null)) {
      CreateGroup(a);
      groupA = moleculeGroups.Find(group => group.molecules.Contains(a));
      groupA.molecules.Add(b);
    } else if (groupA == null && groupB != null) {
      groupB.molecules.Add(a);
      groupB.dissolveTimer = DISSOLVE_TIMER / groupB.molecules.Count;
    } else if (groupB == null && groupA != null) {
      groupA.molecules.Add(b);
      groupA.dissolveTimer = DISSOLVE_TIMER / groupA.molecules.Count;
    } else {
      groupA.molecules.AddRange(groupB.molecules);
      groupA.dissolveTimer = DISSOLVE_TIMER / groupA.molecules.Count;
      moleculeGroups.Remove(groupB);
    }
  }

  public Vector2 AlignToGrid(Vector2 pos) {
    return tilemap.GetCellCenterWorld(tilemap.WorldToCell(pos));
  }

  private List<Vector3> GeneratePackedCirclePositions(int count, Vector3 center, float spacing) {
    List<Vector3> positions = new List<Vector3>();
    int layer = 0;
    int added = 0;

    while (added < count) {
      if (layer == 0) {
        positions.Add(center);
        added++;
      } else {
        int numPoints = 6 * layer; // Number of points in the current layer
        float angleStep = 360f / numPoints;
        float radius = layer * spacing; // Radius increases with each layer

        for (int i = 0; i < numPoints && added < count; i++) {
          float angle = i * angleStep * Mathf.Deg2Rad;
          Vector3 position = center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
          positions.Add(position);
          added++;
        }
      }
      layer++;
    }

    return positions;
  }
}
