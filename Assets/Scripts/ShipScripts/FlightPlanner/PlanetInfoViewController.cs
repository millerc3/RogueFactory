using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlanetInfoViewController : MonoBehaviour
{
    [SerializeField] private TMP_Text planetTypeText;
    [SerializeField] private Image planetImage;
    [SerializeField] private TMP_Text planetDescriptionText;
    [SerializeField] private Transform creatureHLayoutGroup;
    [SerializeField] private GameObject creatureLayoutImagePrefab;
    [SerializeField] private Transform resourceHLayoutGroup;
    [SerializeField] private GameObject resourceLayoutImagePrefab;

    private FlightPlannerPlanetData planetData;
    private Sprite planetSprite;

    public void SetPlanetData(FlightPlannerPlanetData _planetData, Sprite _planetSprite)
    {
        foreach (Transform child in resourceHLayoutGroup)
        {
            Destroy(child.gameObject);
        }

        planetData = _planetData;
        planetSprite = _planetSprite;
        planetTypeText.text = planetData.TypeName;
        planetImage.sprite = planetSprite;
        planetDescriptionText.text = planetData.Description;

        foreach (InventoryItemData resourceItem in planetData.PlanetResources)
        {
            Instantiate(resourceLayoutImagePrefab, resourceHLayoutGroup).GetComponent<Image>().sprite = resourceItem.Icon;
        }
    }
}
