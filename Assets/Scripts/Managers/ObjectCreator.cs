using UnityEngine;
using System.Collections;

public class ObjectCreator : MonoBehaviour
{
    public static ObjectCreator instance;


    [Header("Traps")]
    public GameObject arrowPrefab;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }



    public void CreateObject(GameObject prefab, Transform target, bool shouldBeDestroyed = false,float delay = 0)
    {
        StartCoroutine(CreateObjectCourutine(prefab, target,shouldBeDestroyed, delay));
    }
    private IEnumerator CreateObjectCourutine(GameObject prefab, Transform target, bool shouldBeDestroyed, float delay)
    {
        Vector3 newPosition = target.position;

        yield return new WaitForSeconds(delay);

        GameObject newObject = Instantiate(prefab, newPosition, Quaternion.identity);

        if (shouldBeDestroyed)
            Destroy(newObject, 15);
    }
}
