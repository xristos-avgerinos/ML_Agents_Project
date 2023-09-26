using TMPro;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class AgentAvoidance : BaseAgent
{
    //Μεταβλητή που αναπαριστά την ταχύτητα που θα κινείται δεξιά-αριστερά ο agent-player
    [SerializeField]
    private float speed = 50.0f;

    //Η προκαθορισμένη θέση που πρέπει αν εχει ο player κάθε φορά που επαναφέρεται
    [SerializeField]
    private Vector3 presetPosition = Vector3.zero;

    //Η μέγιστη θέση που μπορεί να φτάσει στα αριστερά
    [SerializeField]
    private Vector3 leftPosition = Vector3.zero;

    //Η μέγιστη θέση που μπορεί να φτάσει στα δεξιά
    [SerializeField]
    private Vector3 rightPosition = Vector3.zero;

    private TargetMoving targetMoving = null;


    //Η μεταβλητή αυτή αναπαριστά την επόμενη κατεύθυνση που θα κινηθούν
    private Vector3 moveTo = Vector3.zero;

    /*Πρέπει να γνωρίζω τη προηγούμενη θέση του agent κάθε φορά γιατί οι agents είναι έξυπνοι και αν
    συνηδητοποιήσουν οτι αν κάτσουν σε μια συγκεκριμένη θέση δεξιά η αριστερά θα έχουν παντα επιτυχία τότε
    θα κάθονται πάντα εκεί.Γιαυτο χρειάζομαστε αυτη τη πληροφορία ώστε αν κάτσουν στην ίδια θέση 2-3 φορές
    συνεχόμενα να τους τιμωρήσωγια να καταλάβουν οτι δεν τους επιτρέπεται αυτο.*/
    private Vector3 prevPosition = Vector3.zero;

    private int punishCounter;

    void Awake()
    {
        //Αρχικοποιήση της μεταβλητής targetMoving
        targetMoving = transform.parent.GetComponentInChildren<TargetMoving>();
    }

    public override void OnEpisodeBegin()
    {
        //Η μέθοδος που ενεργοποιείται στην αρχή καποιου episode και πρέπει να τοποθετήσει τον agent στην προκαθορισμένη θέση
        //του και αρχικοποιεί την προηγούμενη θέση του και την κατεύθυνση που θα πρεπει να κινηθεί με την προκαθορισμένη θέση
        transform.localPosition = presetPosition;
        moveTo = prevPosition = presetPosition;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // 3 observations - x, y, z
        sensor.AddObservation(transform.localPosition);

        // 3 observations - x, y, z
        sensor.AddObservation(targetMoving.transform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var vectorAction = actions.DiscreteActions.Array;

        prevPosition = moveTo; //ενημερώνω ποια ήταν η προηγούμενη θέση του agent
        int direction = Mathf.FloorToInt(vectorAction[0]); //βρίσκω ποια είναι η κατεύθυνση του agent αυτη τη στιγμή δηλ αν ειναι στη μέση,
                                                           //δεξιά η αριστερά γιαυτό κρατάω και το ακέραιο μέρος του.
        moveTo = presetPosition;

        switch (direction)
        {
            //ενημερώνω την κετεύθυνση που θα κινηθεί αναλογα με το direction
            case 0:
                moveTo = presetPosition;
                break;
            case 1:
                moveTo = leftPosition;
                break;
            case 2:
                moveTo = rightPosition;
                break;
        }
        //κινώ τον agent προς την κατεύθυνση που εχει το moveTo με την ταχυτητα που έχουμε επιλέξει
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, moveTo, Time.fixedDeltaTime * speed); 

        if (prevPosition == moveTo)
        {
            //το κανω για να βρώ ποσες φορές κάθεται συνεχόμενα στην ίδια θέση ωστε να τον τιμωρήσω
            punishCounter++;
        }

        if (punishCounter > 3.0f)
        {
            //Αν έχει κάτσει 3 φορές στην ίδια θέση συνεχόμενα τότε τιμωρώ τον agent
            AddReward(-0.01f);
            punishCounter = 0;
        }
    }
    


    public void TakeAwayPoints()
    {
        //Η μέθοδος που τιμωρεί τον agent όποτε κριθεί οτι ειναι αναγκαίο
        AddReward(-0.01f);
        targetMoving.ResetTarget();

        EndEpisode();
        StartCoroutine(SwapGroundMaterial(failureMaterial, 0.5f));//ενημερώνουμε το material του εδάφους με κοκκινο καθως θα εχουμε ανεπιτυχία
    }

   

    public void GivePoints()
    {
        //Η μέθοδος που επιβραβεύσει τον agent όποτε κριθεί οτι ειναι αναγκαίο
        AddReward(1.0f);
        targetMoving.ResetTarget();

        EndEpisode();
        StartCoroutine(SwapGroundMaterial(successMaterial, 0.5f));//ενημερώνουμε το material του εδάφους με πράσινο καθως θα εχουμε επιτυχία
    }

  

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        //Αν επιλέξω behavior type = heuristic only στον player τοτε με αυτή τη μέθοδο  θα έχω πλήρη
        //έλεγχο με βάση τα βελάκια που θα πατάω εγώ για να κινηθώ
        var descreteActions = actionsOut.DiscreteActions.Array;

        //Προκαθορισμένη θέση
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            descreteActions[0] = 0;
        }

        //κίνηση αριστερά
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            descreteActions[0] = 1;
        }

        //κίνηση δεξιά
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            descreteActions[0] = 2;
        }
    }
}
