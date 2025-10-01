using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Enums;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class GenerateSO : MonoBehaviour
{
    [SerializeField] private List<Sprite> sprites = new();

    private readonly CardElement[] elementsOrder = new CardElement[]
    {
        CardElement.Fire, CardElement.Water, CardElement.Energy,
        CardElement.Earth, CardElement.Nature,
        CardElement.Magic
    };

#if UNITY_EDITOR
    private void Start()
    {
        CreateCardDataAssets();
    }

    private void CreateCardDataAssets()
    {
        string folderPath = "Assets/ScriptableObjects";

        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets", "ScriptableObjects");
        }

        int spritesPerElement = 5;
        int totalElements = elementsOrder.Length;
        int valueStart = 10;

        for (int i = 0; i < sprites.Count; i++)
        {
            CardData newCard = ScriptableObject.CreateInstance<CardData>();
            newCard.sprite = sprites[i];

            int elementIndex = i / spritesPerElement;
            if (elementIndex >= totalElements) break;

            newCard.element = elementsOrder[elementIndex];

            int valueOffset = i % spritesPerElement;
            newCard.value = valueStart - valueOffset;

            string assetPath = $"{folderPath}/{newCard.element}_{newCard.value}_{sprites[i].name}.asset";
            AssetDatabase.CreateAsset(newCard, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
#endif
}
