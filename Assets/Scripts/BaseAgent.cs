using System.Collections;
using Unity.MLAgents;
using UnityEngine;


public class BaseAgent : Agent
{
    //Κλάση που εχει φτιαχτεί για να κληρονομεί η AgentAvoidance και να ρυθμίζει τις αλλαγές
    //στο material του εδάφους κάθε φορά που έχουμε επιτυχία η αποτύχια.
    [SerializeField]
    protected MeshRenderer groundMeshRenderer;

    [SerializeField]
    protected Material successMaterial;

    [SerializeField]
    protected Material failureMaterial;

    [SerializeField]
    protected Material defaultMaterial;

    protected IEnumerator SwapGroundMaterial(Material mat, float time) 
    {
        //Η μέθοδος που αλλάζει το material του εδάφους και περιμένει μερικά δευτερόλεπτα
        //για να επαναφέρει μετά στο αρχικό default material
        groundMeshRenderer.material = mat;
        yield return new WaitForSeconds(time);
        groundMeshRenderer.material = defaultMaterial;
    }
}
