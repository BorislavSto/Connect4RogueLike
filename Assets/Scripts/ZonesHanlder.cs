using UnityEngine;
using UnityEngine.EventSystems;

public class ZonesHanlder : MonoBehaviour
{
    public static ZonesHanlder instance {get; private set; }
    
    [SerializeField] private RectTransform playZone;
    [SerializeField] private RectTransform playerArea;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        { 
            instance = this;
        }
    }
    
    public bool IsInPlayZone(PointerEventData eventData)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(playZone, Input.mousePosition, eventData.enterEventCamera);
    }
    
    public bool IsInPlayZone()
    {
        return RectTransformUtility.RectangleContainsScreenPoint(playZone, Input.mousePosition);
    }  
    
    public bool IsInPlayerZone(PointerEventData eventData)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(playerArea, Input.mousePosition, eventData.enterEventCamera);
    }
    
    public bool IsInPlayerZone()
    {
        return RectTransformUtility.RectangleContainsScreenPoint(playerArea, Input.mousePosition);
    }
}