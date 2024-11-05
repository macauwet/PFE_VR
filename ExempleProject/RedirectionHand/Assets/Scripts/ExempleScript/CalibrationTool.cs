using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalibrationTool
{
    public static string controlleurType = "Quest2";
    public static void UpdateRendererFromTag(bool renderStatus)
    {
        GameObject rightAnchor = GameObject.FindGameObjectWithTag("RightAnchor");
        GameObject leftAnchor = GameObject.FindGameObjectWithTag("LeftAnchor");
        GameObject rightControlleur = GameObject.FindGameObjectWithTag("RightHand");
        GameObject leftControlleur = GameObject.FindGameObjectWithTag("LeftHand");
        if (rightAnchor != null && leftAnchor != null && rightControlleur != null && leftControlleur != null)
        {
            if (controlleurType == "Quest2")
            {
               /* UpdateRenderQuest2Prefab(renderStatus, rightAnchor, leftAnchor);
                UpdateRenderQuest2Prefab(renderStatus, rightControlleur, leftControlleur);*/
            }
            
        }
        else
        {
            Debug.Log($"Cannot Update Renderer \n tag status: RA {rightAnchor != null};LA {leftAnchor != null};RC {rightControlleur != null};LC {leftControlleur != null}");
        }
    }
    public static void CalibrateFromTag()
    {
        GameObject rightAnchor = GameObject.FindGameObjectWithTag("RightAnchor");
        GameObject leftAnchor = GameObject.FindGameObjectWithTag("LeftAnchor");
        GameObject rightControlleur = GameObject.FindGameObjectWithTag("RightHand");
        GameObject leftControlleur = GameObject.FindGameObjectWithTag("LeftHand");
        if (rightAnchor != null && leftAnchor != null && rightControlleur != null && leftControlleur != null)
        {
            CalibrateFromTransform(rightAnchor.transform, leftAnchor.transform, rightControlleur.transform, leftControlleur.transform);
        }
        else
        {
            Debug.Log($"Cannot calibrate \n tag status: RA {rightAnchor!=null};LA {leftAnchor != null};RC {rightControlleur != null};LC {leftControlleur!=null}");
        }
    }

    public static void UpdateRenderQuest2Prefab(bool renderStatus, GameObject RightPrefab,GameObject LeftPrefab)
    {
        RightPrefab.transform.Find("right_quest2_mesh").gameObject.SetActive(renderStatus);
        LeftPrefab.transform.Find("left_quest2_mesh").gameObject.SetActive(renderStatus);
    }
    public static void CalibrateFromTransform(Transform aRight, Transform aLeft, Transform cRight, Transform cLeft)
    {
        Vector3 aDir = aRight.position - aLeft.position;
        Vector3 cDir = cRight.position - cLeft.position;

        if (cDir.magnitude< aDir.magnitude*0.75f)
        {
            Debug.Log("controlleur not in Anchor");
        }
        else
        {
            
            float angle = Vector3.SignedAngle(Vector3.ProjectOnPlane(cDir, Vector3.up), Vector3.ProjectOnPlane(aDir, Vector3.up), Vector3.up);

            cRight.root.transform.RotateAround(cLeft.position + cDir * 0.5f, Vector3.up, angle);

            Vector3 offset = aLeft.position + aDir * 0.5f - (cLeft.position + cDir * 0.5f);
            //offset.y = 0;

            cRight.root.transform.position = cRight.root.transform.position + offset;
        }

        
    }
    public static IEnumerator CalibTest(Transform aRight, Transform aLeft, Transform cRight, Transform cLeft)
    {
        Vector3 aDir = aRight.position - aLeft.position;
        Vector3 cDir = cRight.position - cLeft.position;

        if (cDir.magnitude < aDir.magnitude * 0.75f)
        {
            Debug.Log("controlleur not in Anchor");
        }
        else
        {

            float angle = Vector3.SignedAngle(Vector3.ProjectOnPlane(cDir, Vector3.up), Vector3.ProjectOnPlane(aDir, Vector3.up), Vector3.up);

            cRight.root.transform.RotateAround(cLeft.position + cDir * 0.5f, Vector3.up, angle);

            Vector3 offset = aLeft.position + aDir * 0.5f - (cLeft.position + cDir * 0.5f);

            cRight.root.transform.position = cRight.root.transform.position + offset;
        }

        yield return new WaitForEndOfFrame();

        aDir = aRight.position - aLeft.position;
        cDir = cRight.position - cLeft.position;
        if (cDir.magnitude < aDir.magnitude * 0.75f)
        {
            Debug.Log("controlleur not in Anchor");
        }
        else
        {

            

            Vector3 offset = aLeft.position + aDir * 0.5f - (cLeft.position + cDir * 0.5f);

            cRight.root.transform.position = cRight.root.transform.position + offset;
        }
    }

}
