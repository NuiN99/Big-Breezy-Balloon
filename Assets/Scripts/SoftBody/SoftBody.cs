using NuiN.NExtensions;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
using UnityEngine;

public class SoftBody : MonoBehaviour
{
    [SerializeField] private SoftBodyParams defaultParams = new SoftBodyParams();

#if UNITY_EDITOR
    [SerializeField] private bool _centerBoneColliders = true;


    private void Reset()
    {
        Transform armature;
        if (name != "Armature")
        {
            armature = transform.Find("Armature");
            if (armature == null)
                return;
        }
        else
            armature = transform;

        RB = armature.GetComponent<Rigidbody>();
        if (RB == null)
            RB = armature.AddComponent<Rigidbody>();
    
        List<SpringJoint> springJoints = GetComponents<SpringJoint>().ToList();
        while (springJoints.Count < armature.childCount)
        {
            SpringJoint sj = gameObject.AddComponent<SpringJoint>();
            springJoints.Add(sj);
        }

        while (springJoints.Count > armature.childCount)
            springJoints.RemoveAt(springJoints.Count - 1);

        bonePositions = new Vector3[springJoints.Count];
        boneRotations = new Quaternion[springJoints.Count];

        for (int i = 0; i < armature.childCount; i++)
        {
            Transform child = armature.GetChild(i);
            Rigidbody rb = child.GetComponent<Rigidbody>();

            if (rb == null)
                rb = child.AddComponent<Rigidbody>();

            bonePositions[i] = rb.transform.localPosition;
            boneRotations[i] = rb.rotation;

            rb.constraints = RigidbodyConstraints.FreezeRotation;

            SphereCollider collider = child.GetComponent<SphereCollider>();
            if (collider == null)
                collider = child.AddComponent<SphereCollider>();

            if (_centerBoneColliders && child.childCount > 0)
            {
                Transform boneTip = child.GetChild(0);
                Vector3 centerPoint = child.InverseTransformPoint(boneTip.position - child.position);
                collider.center = centerPoint;
            }
            else
                collider.center = Vector3.zero;

            springJoints[i].connectedBody = rb;
            ApplySpringConfig(springJoints[i], defaultParams);
        }

        bones = armature.GetComponentsInChildren<Rigidbody>();

        Collider m_collider = armature.gameObject.GetComponent<Collider>();
        if (m_collider == null)
            Debug.LogWarning("You should have a collider on your armature");
        else
        {
            for(int i = 0; i < bones.Length - 1; i++)
                bones[i] = bones[i + 1];

            System.Array.Resize(ref bones, bones.Length - 1);
        }
    }

    [MethodButton("Update Springs")]
    public void EditorUpdateSprings()
    {
        UpdateSprings(defaultParams);
    }
    

    [MethodButton("ToggleFilled")]
    private void EditorToggleFilled()
    {
        ToggleFilled();
    }
#endif

    [SerializeField] private Rigidbody RB;
    [SerializeField, HideInInspector] Vector3[] bonePositions;
    [SerializeField, HideInInspector] Quaternion[] boneRotations;
    [SerializeField] Rigidbody[] bones;

    private bool isFilled;

    //private void Update()
    //{
    //    if(Input.GetKeyDown(KeyCode.Space))
    //        ToggleFilled();
    //}

    public void ToggleFilled()
    {
        if (!isFilled)
            SetRigid();
        else
            SetSoft();
    }

    public void SetRigid()
    {
        if (isFilled)
            return;

        for (int i = 0; i < bones.Length; i++)
        {
            bones[i].constraints = RigidbodyConstraints.FreezeAll;
            bones[i].isKinematic = true;
            bones[i].transform.localPosition = bonePositions[i];
        }

        isFilled = true;
    }

    public void SetSoft()
    {
        if (!isFilled)
            return;

        for (int i = 0; i < bones.Length; i++)
        {
            bones[i].isKinematic = false;
            bones[i].constraints = RigidbodyConstraints.FreezeRotation;
        }

        isFilled = false;
    }

    public void UpdateSprings(SoftBodyParams sbParams)
    {
        SpringJoint[] springJoints = GetComponentsInChildren<SpringJoint>();
        for (int i = 0; i < springJoints.Length; i++)
            ApplySpringConfig(springJoints[i], sbParams);
    }

    private void ApplySpringConfig(SpringJoint springJoint, SoftBodyParams sbParams)
    {
        springJoint.spring = sbParams.Spring;
        springJoint.damper = sbParams.Damper;
        springJoint.minDistance = sbParams.MinDistance;
        springJoint.maxDistance = sbParams.MaxDistance;
        springJoint.tolerance = sbParams.Tolerance;
    }

    public void DistributeForce(Vector3 force, ForceMode forceMode)
    {
        RB.AddForce(force, forceMode);
        for (int i = 0; i < bones.Length; i++)
            bones[i].AddForce(force, forceMode);
    }
}

//#if UNITY_EDITOR
//[CustomEditor(typeof(SoftBody))]
//public class SoftBody_Editor : Editor
//{
//    private SoftBody Target => target as SoftBody;

//    public override void OnInspectorGUI()
//    {
//        if (GUILayout.Button("Set spring strength"))
//            UpdateSpringStrength();

//        base.OnInspectorGUI();
//    }

//    private void UpdateSpringStrength()
//    {
//        SpringJoint[] springJoints = Target.GetComponents<SpringJoint>();
//        for (int i = 0; i < springJoints.Length; i++)
//            springJoints[i].spring = Target.SpringStrength;
//    }
//}
//#endif
