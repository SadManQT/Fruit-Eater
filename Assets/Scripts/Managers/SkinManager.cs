using UnityEditor.SearchService;
using UnityEngine;

public class SkinManager : MonoBehaviour
{
    public static SkinManager instance;

    public int[] skinId;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void SetSkinId(int id,int playerNumber) => skinId[playerNumber] = id;
    public int GetSkinId(int playerNumber) => skinId[playerNumber];
}
