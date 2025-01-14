using System.Collections.Generic;
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
  private List<MoleculeGroup> moleculeGroups = new();
  private int points = 0;

  private const int GRID_WIDTH = 20;
  private const int GRID_HEIGHT = 5;
  private const int SEED_COUNT = 2;
  private const int DISSOLVE_TIMER = 8;

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
    GameObject seed = Instantiate(
      moleculePrefab, 
      tilemap.CellToWorld(new Vector3Int(
        UnityEngine.Random.Range(-1 * GRID_WIDTH / 2, GRID_WIDTH / 2), 
        UnityEngine.Random.Range(0, GRID_HEIGHT),
        0)), 
      Quaternion.identity);

    CreateGroup(seed.GetComponent<Molecule>());
  }

  private void Tick() {
    List<MoleculeGroup> scheduleRemoval = new();
    foreach (MoleculeGroup group in moleculeGroups) {
      group.dissolveTimer--;
      if (group.dissolveTimer <= 0) {
        scheduleRemoval.Add(group);
      }
    }

    foreach (MoleculeGroup group in scheduleRemoval) {
      if (group.molecules.Count >= 6) {
        points += 100;
        scoreDisplay.text = points.ToString();
      }

      moleculeGroups.Remove(group);
      foreach (Molecule molecule in group.molecules) {
        ParticlePoolManager.Instance.PlayParticle("Shards", molecule.transform.position);
        Destroy(molecule.gameObject);
      }
    }

    // Replenish seeds
    int seedsNeeded = SEED_COUNT - moleculeGroups.Count;
    for (int i = 0; i < seedsNeeded; i++) {
      GenerateSeed();
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

    if (groupA == groupB || (groupA == null && groupB == null)) {
      return;
    } else if (groupA == null && groupB != null) {
      groupB.molecules.Add(a);
      groupB.dissolveTimer = DISSOLVE_TIMER;
    } else if (groupB == null && groupA != null) {
      groupA.molecules.Add(b);
      groupA.dissolveTimer = DISSOLVE_TIMER;
    } else {
      groupA.molecules.AddRange(groupB.molecules);
      groupA.dissolveTimer = DISSOLVE_TIMER;
      moleculeGroups.Remove(groupB);
    }
  }


  public Vector2 AlignToGrid(Vector2 pos) {
    return tilemap.CellToWorld(tilemap.WorldToCell(pos));
  }
}
