using UnityEngine;

public class IsEnabled : MonoBehaviour
{
    public int needToUnlockScore;
    public int needToUnlockAllCubes;
    public Material blackMaterial;

    private void Start()
    {
        if (PlayerPrefs.GetInt("score") < needToUnlockScore || PlayerPrefs.GetInt("allCubes") < needToUnlockAllCubes)
        {
            GetComponent<MeshRenderer>().material = blackMaterial;
            //GetComponent<Animator>().enabled = false;
        }
            
    }
}
