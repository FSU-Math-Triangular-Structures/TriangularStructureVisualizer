using UnityEngine;
using UnityEngine.UI;

/// # ListCreator.cs

/// (Maybe) Legacy class used to create the settings list of past values.
/// Everything is currently commented out, so it doesn't do anything, but if 
/// that functionality is desired in the future the foundation for it
/// is right here. 
public class ListCreator : MonoBehaviour
{
    /*
    [SerializeField]
    private Transform SpawnPoint = null;

    [SerializeField]
    private GameObject item = null;

    [SerializeField]
    private RectTransform content = null;

    [SerializeField]
    public static int numberOfItems = 0;

    public GameObject nonEuclideanTriangle;
    public static int index = 1;
    public static int num = 1;
    public string title;


    // Use this for initialization
    public void Save()
    {
        var objects = GameObject.FindGameObjectsWithTag("Data");

        foreach (var o in objects)
            Destroy(o);

        num = 1;
        numberOfItems = 0;
        index = 1;

        if (nonEuclideanTriangle.activeSelf)
        {
            int size = NonEuclideanTriangle.queue.Count;

            for (int i = 0; i < size; i++)
            {
                title = num + ":   " + "(a: " + NonEuclideanTriangle.queue[i].alphaPoint + "    b: " + NonEuclideanTriangle.queue[i].betaPoint + "    c: " + NonEuclideanTriangle.queue[i].gammaPoint + ")";
                num++;

                numberOfItems++;
                //setContent Holder Height;
                content.sizeDelta = new Vector2(0, numberOfItems * 60);

                // 60 width of item
                float spawnY = index * 60;
                index++;
                //newSpawn Position
                Vector3 pos = new Vector3(750, -spawnY, 0);
                //instantiate item
                GameObject SpawnedItem = Instantiate(item, pos, SpawnPoint.rotation);
                //setParent
                SpawnedItem.transform.SetParent(SpawnPoint, false);
                //get ItemDetails Component
                ItemDetails itemDetails = SpawnedItem.GetComponent<ItemDetails>();
                //set name
                itemDetails.text.text = title;
            }
        }
       
    }

    public void delete() 
    {
        var objects = GameObject.FindGameObjectsWithTag("Data");

        foreach (var o in objects)
            Destroy(o);
    }
    */
}
