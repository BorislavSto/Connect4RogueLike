using UnityEngine;
using UnityEngine.UI;

public class TextureManager : MonoBehaviour
{
    public static TextureManager instance {get; private set; }

     public Sprite simplePlayerPiece;
     public Sprite simpleAIPiece;
     public Sprite breakCirlcePiece;
     

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
}