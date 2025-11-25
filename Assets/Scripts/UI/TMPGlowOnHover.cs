using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class TMPGlowOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TextMeshProUGUI tmp;
    public Material normalMaterial;
    public Material glowMaterial;

    void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();

        if (tmp == null)
        {
            Debug.LogError("TMPGlowOnHover: No TextMeshProUGUI found on this object!", this);
            return;
        }

        if (normalMaterial != null)
            tmp.fontSharedMaterial = normalMaterial;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tmp != null && glowMaterial != null)
            tmp.fontSharedMaterial = glowMaterial;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tmp != null && normalMaterial != null)
            tmp.fontSharedMaterial = normalMaterial;
    }
}
