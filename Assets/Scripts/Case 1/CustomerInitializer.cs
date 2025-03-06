using UnityEngine;

public class CustomerInitializer : MonoBehaviour
{
    [SerializeField] private Customer prefab;
    [SerializeField] private int objectCount = 30; 
    [SerializeField] private float radius = 10f; 

    public static readonly int MaxCustomerCount = 30;
    void Start()
    {
        SpawnObjects();
    }

    void SpawnObjects()
    {
        for (int i = 0; i < MaxCustomerCount; i++)
        {
            float angle = Random.Range(0f, 360f);
            float radian = angle * Mathf.Deg2Rad;
            
            float randomRadius = Mathf.Sqrt(Random.Range(0f, 1f)) * radius;

            Vector3 position = new Vector3(
                Mathf.Cos(radian) * randomRadius, 
                0f,
                Mathf.Sin(radian) * randomRadius
            );
            position += transform.position;
            Customer newCustomer = Instantiate(prefab, transform);
            newCustomer.transform.position = position;
        }
    }
}
