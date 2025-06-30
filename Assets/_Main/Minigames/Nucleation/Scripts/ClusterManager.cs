// ClusterManager.cs
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClusterManager : MonoBehaviour
{
    public static ClusterManager Instance;

    private InputActionAsset inputActions;
    private InputAction clickAction, pointAction;
    private Camera mainCamera;

    [Header("Prefabs")]
    public GameObject moleculePrefab;
    public GameObject nucleationText;
    public GameObject precipitationText;
    public GameObject gameStartText;
    public Sprite completionSprite;
    public GameObject bonkText;
    public Sprite mainSprite;
    public Sprite[] nonMainSprites;

    [Header("References")]
    public TMP_Text precipitateCountText;
    public TMP_Text crystallineCountText;
    public Transform clusterRoot;
    public Transform indicator;
    public GameObject sunBurst;

    private List<Molecule> cluster = new List<Molecule>();
    private enum GrowthState { FreeForm, Locked, Growing }
    private GrowthState growthState = GrowthState.FreeForm;

    private float SPACING = 0.91f;
    private bool isInitialized = false;

    private int precipitateCount = 0;
    private int crystallineCount = 0;

    void Awake()
    {
        Instance = this;
        mainCamera = Camera.main;
        inputActions = InputManager.Instance.inputActions;
        clickAction = inputActions.FindAction("Game/Click");
        pointAction = inputActions.FindAction("Game/Point");
    }

    void Start()
    {
        MinigameManager.Instance.Initialize(120f, true, true, completionSprite, "Good job nucleating!", 30f);
        Invoke(nameof(GameStart), 1f);
    }

    void Update()
    {
        if (cluster.Count == 0)
        {
            clusterRoot.position = new Vector3(0, 0, 0);
            GameObject m = Instantiate(moleculePrefab, new Vector3(0, 0, 0), Quaternion.identity);
            AddToCluster(m.GetComponent<Molecule>());
        }

        if (growthState != GrowthState.Locked && isInitialized)
        {
            Vector3 pointer = GetPointerWorldPosition();
            Vector3 currentCenter = GetClusterCenter();
            Vector3 delta = pointer - currentCenter;

            if (isDragging)
            {
                // Update cluster
                Vector3 target = clusterRoot.position + delta;
                clusterRoot.position = Vector3.Lerp(clusterRoot.position, target, Time.deltaTime * 2f);
            }

            // Update sunburst
            sunBurst.transform.position = currentCenter;
            sunBurst.transform.localScale = new Vector3(0.2f, 0.2f, 1) * Mathf.Sqrt(cluster.Count);

            // Update indicator
            List<Vector3> targets = GenerateClusterGridPositions(clusterRoot.position, cluster.Count + 1);
            indicator.position = targets[cluster.Count];
        }
    }

    private bool isDragging = false;

    private void OnDragStart(InputAction.CallbackContext context)
    {
        isDragging = true;
    }

    private void OnDragEnd(InputAction.CallbackContext context)
    {
        isDragging = false;
    }

    private void OnEnable()
    {
        clickAction.started += OnDragStart;
        clickAction.canceled += OnDragEnd;
    }

    private void OnDisable()
    {
        clickAction.started -= OnDragStart;
        clickAction.canceled -= OnDragEnd;
    }

    public void AddToCluster(Molecule molecule)
    {
        if (growthState == GrowthState.Locked) return;

        if (!cluster.Contains(molecule))
        {
            // Disable sunburst 
            var slow = molecule.GetComponentInChildren<SlowRotate>();
            if (slow != null) slow.gameObject.SetActive(false);

            molecule.transform.SetParent(clusterRoot);
            cluster.Add(molecule);
            molecule.state = MoleculeState.Clustered;

            if (growthState == GrowthState.Growing) molecule.isMainMoleculeType = true;
        }

        int mainMoleculeCount = cluster.FindAll(m => m.isMainMoleculeType).Count;
        int nonMainMoleculeCount = cluster.Count - mainMoleculeCount;
        if (nonMainMoleculeCount >= 4) AnimatePrecipitation();
        else if (mainMoleculeCount == 6 || mainMoleculeCount == 12 || mainMoleculeCount == 9 || mainMoleculeCount == 16) StartCoroutine(AnimateGrowth());
    }

    public void RemoveFromCluster(Molecule molecule)
    {
        if (cluster.Contains(molecule))
        {
            cluster.Remove(molecule);
            molecule.transform.SetParent(null);
        }
    }

    private static readonly Vector2[] growthOffsets = new Vector2[]
    {
    // Base 3x2 grid (6 molecules)
    new Vector2(-1,  0.5f),
    new Vector2( 0,  0.5f),
    new Vector2( 1,  0.5f),
    new Vector2(-1, -0.5f),
    new Vector2( 0, -0.5f),
    new Vector2( 1, -0.5f),

    // Bottom row (left to right) → 3x3
    new Vector2(-1, -1.5f),
    new Vector2( 0, -1.5f),
    new Vector2( 1, -1.5f),

    // Right column (bottom to top) → 3x4
    new Vector2( 2f, -1.5f),
    new Vector2( 2f,  -0.5f),
    new Vector2( 2f,  0.5f),

    // Top row (right to left) → 4x4
    new Vector2( -1,  1.5f),
    new Vector2( 0,  1.5f),
    new Vector2( 1,  1.5f),
    new Vector2( 2, 1.5f),
    };
    public List<Vector3> GenerateClusterGridPositions(Vector3 center, int count)
    {
        List<Vector3> positions = new();
        for (int i = 0; i < Mathf.Min(count, growthOffsets.Length); i++)
        {
            Vector2 offset = growthOffsets[i] * SPACING;
            positions.Add(center + new Vector3(offset.x, offset.y, 0));
        }
        return positions;
    }

    private IEnumerator AnimateGrowth()
    {
        indicator.gameObject.SetActive(false);
        growthState = GrowthState.Locked;

        // Convert non main molecules to main molecules

        foreach (Molecule m in cluster.FindAll(m => !m.isMainMoleculeType))
        {
            // m.TriggerFall(cluster[0]);
            // RemoveFromCluster(m);
            m.isMainMoleculeType = true;
            m.GetComponent<SpriteRenderer>().sprite = mainSprite;
        }

        int mainMoleculeCount = cluster.Count;

        Vector3 center = clusterRoot.position;
        List<Vector3> targets = GenerateClusterGridPositions(center, mainMoleculeCount);
        List<Vector3> originals = new();

        for (int i = 0; i < mainMoleculeCount; i++)
        {
            originals.Add(cluster[i].transform.position);
        }

        float duration = mainMoleculeCount == 6 ? 1f : 0.2f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            for (int i = 0; i < mainMoleculeCount; i++)
            {
                cluster[i].transform.position = Vector3.Lerp(originals[i], targets[i], t);
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < mainMoleculeCount; i++)
        {
            cluster[i].transform.position = targets[i];
        }

        Instantiate(nucleationText, center, Quaternion.identity);
        SoundManager.Instance.PlaySFX("DM-CGS-45");

        foreach (Molecule m in cluster)
        {
            m.GetComponent<SpriteRenderer>().sprite = mainSprite;
        }

        if (mainMoleculeCount == 16)
        {
            // MinigameManager.Instance.Completion();
            List<Molecule> copy = new List<Molecule>(cluster);

            foreach (Molecule m in copy)
            {
                RemoveFromCluster(m);
            }

            foreach (Molecule m in copy)
            {
                m.TriggerFall(copy[UnityEngine.Random.Range(0, copy.Count)]);
            }

            growthState = GrowthState.FreeForm;
            crystallineCount++;
            crystallineCountText.text = "Crystalline: " + crystallineCount.ToString();
        }
        else
        {
            growthState = GrowthState.Growing;
            indicator.gameObject.SetActive(true);
        }

        MinigameManager.Instance.Score(10);
    }

    private void AnimatePrecipitation()
    {
        precipitateCount++;
        precipitateCountText.text = "Precipitate: " + precipitateCount.ToString();
        Instantiate(precipitationText, GetClusterCenter(), Quaternion.identity);
        SoundManager.Instance.PlaySFX("DM-CGS-48");

        List<Molecule> copy = new List<Molecule>(cluster);

        foreach (Molecule m in copy)
        {
            RemoveFromCluster(m);
        }

        foreach (Molecule m in copy)
        {
            if (m.isMainMoleculeType) m.GetComponent<SpriteRenderer>().sprite = nonMainSprites[UnityEngine.Random.Range(0, nonMainSprites.Length)];
            m.TriggerFall(copy[0]);
        }
    }

    private Vector3 GetPointerWorldPosition()
    {
        Vector3 screenPosition = pointAction.ReadValue<Vector2>();
        Vector3 screenPos = mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 0));
        screenPos.z = 0;
        return screenPos;
    }

    public void HandleCollision(Molecule a, Molecule b)
    {
        // Both clustered: Do nothing
        if (a.state == MoleculeState.Clustered && b.state == MoleculeState.Clustered)
        {
            return;
        }


        // Both free, just bounce
        if (a.state == MoleculeState.Free && b.state == MoleculeState.Free)
        {
            SoundManager.Instance.PlaySFX("SFX-impact-simple-03_wav");
            a.TriggerBounce(b);
            b.TriggerBounce(a);
            return;
        }

        // One free, one clustered: Knock off one or both depending on growthState
        if ((a.state == MoleculeState.Clustered && b.state == MoleculeState.Free) ||
            (b.state == MoleculeState.Clustered && a.state == MoleculeState.Free))
        {
            Molecule clustered = a.state == MoleculeState.Clustered ? a : b;
            Molecule free = a.state == MoleculeState.Free ? a : b;
            free.transform.position = clustered.transform.position + (free.transform.position - clustered.transform.position).normalized * SPACING;

            if (growthState == GrowthState.FreeForm)
            {
                free.state = MoleculeState.Clustered;
                SoundManager.Instance.PlaySFX("DM-CGS-32");
                AddToCluster(free);
                return;
            }

            // if (growthState == GrowthState.FreeForm)
            // {
            //     // RemoveFromCluster(clustered);
            //     // clustered.TriggerFall(free);
            //     // free.TriggerFall(clustered);
            //     AddToCluster(free);
            //     free.state = MoleculeState.Clustered;
            //     return;
            // }
            if (growthState == GrowthState.Growing)
            {
                free.TriggerFall(clustered);
                Instantiate(bonkText, (a.transform.position + b.transform.position) / 2, Quaternion.identity);
                SoundManager.Instance.PlaySFX("DM-CGS-39");
            }

        }
    }

    private void GameStart()
    {
        isInitialized = true;
        Instantiate(gameStartText, GetClusterCenter(), Quaternion.identity);
        MinigameManager.Instance.StartGame();
        SoundManager.Instance.PlaySFX("DM-CGS-36");
    }

    public void HandleIndicatorCollision(Molecule molecule)
    {
        if (growthState != GrowthState.Growing) return;

        molecule.transform.position = indicator.position;
        AddToCluster(molecule);
        SoundManager.Instance.PlaySFX("DM-CGS-32");
    }

    private Vector3 GetClusterCenter()
    {
        Vector3 center = Vector3.zero;
        foreach (Molecule m in cluster)
        {
            center += m.transform.position;
        }
        return center / Math.Max(cluster.Count, 1);
    }
}
