using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Molecule Settings")]
    public GameObject[] moleculePrefabs;
    private const float MOLECULE_SPAWN_INTERVAL = 0.5f;

    void Start()
    {
        SpawnMolecule(true); // To trigger the start
        StartCoroutine(SpawnMolecules());
    }

    private IEnumerator SpawnMolecules()
    {
        while (true)
        {
            yield return new WaitForSeconds(MOLECULE_SPAWN_INTERVAL);
            SpawnMolecule();
        }
    }

    private void SpawnMolecule(bool toOrigin = false)
    {
        Vector2 spawnPosition = GetRandomEdgePosition();
        Vector2 targetDirection = GetTargetDirection(spawnPosition);
        if (toOrigin) targetDirection = -spawnPosition.normalized;

        GameObject newMolecule = Instantiate(
            moleculePrefabs[Random.Range(0, moleculePrefabs.Length)],
            spawnPosition,
            Quaternion.identity
        );

        Molecule molecule = newMolecule.GetComponent<Molecule>();
        molecule.InitializeMovement(targetDirection);
    }

    private Vector2 GetRandomEdgePosition()
    {
        float screenLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
        float screenRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;
        float screenTop = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0)).y;
        float screenBottom = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).y;

        int edge = Random.Range(0, 4);
        Vector2 spawnPosition = Vector2.zero;

        switch (edge)
        {
            case 0:
                spawnPosition = new Vector2(screenLeft, Random.Range(screenBottom, screenTop));
                break;
            case 1:
                spawnPosition = new Vector2(screenRight, Random.Range(screenBottom, screenTop));
                break;
            case 2:
                spawnPosition = new Vector2(Random.Range(screenLeft, screenRight), screenTop);
                break;
            case 3:
                spawnPosition = new Vector2(Random.Range(screenLeft, screenRight), screenBottom);
                break;
        }

        return spawnPosition;
    }

    private Vector2 GetTargetDirection(Vector2 spawnPosition)
    {
        Vector2 targetDirection = -spawnPosition.normalized;
        float angleOffset = Random.Range(-45f, 45f);
        targetDirection = Quaternion.Euler(0, 0, angleOffset) * targetDirection;

        return targetDirection;
    }
}
