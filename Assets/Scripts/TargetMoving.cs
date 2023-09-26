using UnityEngine;
using UnityEngine.SocialPlatforms;

public class TargetMoving : MonoBehaviour
{
    [SerializeField]
    private AgentAvoidance agent = null;

    //Η ελάχιστη ταχύτητα του target θα κυμαίνεται μεταξύ 0.5 και 25.0
    [SerializeField]
    [Range(0.5f, 25.0f)]
    private float minSpeed = 5.0f;

    //Η μέγιστη ταχύτητα του target θα κυμαίνεται  μεταξύ 5.0 και 150.0
    [SerializeField]
    [Range(5.0f, 150.0f)]
    private float maxSpeed = 150.0f;

    //θα επιλέγεται τυχαία μεταξύ της ελάχιστης και μέγιστης ταχύτητας που μπορεί να έχει
    private float speed = 0;

    //η μέγιστη απόσταση που μπορεί να φτάσει μακρυά στον z άξονα το target
    [SerializeField]
    private float maxDistance = -3.5f;

    // Η default θέση που πρέπει να έχει το target κάθε φορά που επαναφέρεται
    private Vector3 originalPosition;

    private Quaternion originalRotation;

    void Awake()
    {
        //Η μέθοδος που ενεργοποιείται στην αρχή η οποία πρέπει να τοποθετήσει το target στην default
        //θέση του και του επιλέγει μια τυχαία ταχύτητα μεταξύ της μέγιστης και της ελάχιστης 
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
        speed = Random.Range(minSpeed, maxSpeed);
    }

    void Update()
    {
        // αν βρισκόμαστε πέρα από τη μέγιστη απόσταση επανεκκινούμε τη θέση του
        if (transform.localPosition.z <= maxDistance)
        {
            transform.localPosition = originalPosition;
            transform.localRotation = originalRotation;
        }
        else
        {
            // Το μετακινούμε προς τη μέγιστη απόσταση με την συγκεκριμενη ταχυτητα που εχει επιλεγεί
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y,
                transform.localPosition.z - (Time.deltaTime * speed));
        }
    }

    public void ResetTarget()
    {
        //Η μεθοδος που επαναφέρει το target στην αρχική του θέση και ξανά επιλέγει τυχαία ταχύτητα
        speed = Random.Range(minSpeed, maxSpeed);
        transform.localPosition = originalPosition;
        transform.localRotation = originalRotation;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag.ToLower() == "player")
        {
            //Αν ο target κάνει collide με τον player τότε πρεπει να τιμωρήσουμε τον agent
            //οπότε καλούμε την μέθοδο του TakeAwayPoints
            Debug.Log("Points taken away");
            agent.TakeAwayPoints();
        }
        else if (collision.transform.tag.ToLower() == "wall")
        {
            //Αν ο target κάνει collide με τον τοίχο που είναι και το σωστό τότε πρεπει να
            //επιβραβεύσουμε τον agent οπότε καλούμε την μέθοδο του GivePoints
            Debug.Log("Points gained");
            agent.GivePoints();
        }
    }
}
