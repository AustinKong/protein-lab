using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewBufferRecipe", menuName = "Buffer/Recipe")]
public class BufferRecipe : ScriptableObject
{
    /*[Header("General Info")]
    public string recipeName;
    public float targetVolumeML;
    public float targetPH;
    public float targetConductivity;

    [Header("Salt")]
    public string salt1Name;
    public float salt1Amount;

    public string salt2Name;
    public float salt2Amount;

    [Header("Acid/Base")]
    public string acidName;
    public float acidVolumeML;

    public string baseName;
    public string baseForm; // "Solution" or "Pellet"
    public float baseAmount;

    [Header("Equipment Settings")]
    public bool useSmallScaleEquipment;
    public bool usesPelletBase;*/

    [System.Serializable]
    public class SceneItemRequirement
    {
        public int sceneIndex;  // Scene1 = 1, Scene2 = 2 ...
        public string titleName;
        public List<SceneItemPrefabInfo> items;

        [TextArea(3, 10)]
        public List<string> stepDescriptions;
    }

    public List<SceneItemRequirement> sceneItemRequirements;
}
