using UnityEngine;
using UnityEditor;

// File ini HARUS ada di folder bernama "Editor" supaya tidak ikut ke build

[CustomEditor(typeof(SightSensor))]
public class SightSensorEditor : Editor
{
    void OnSceneGUI()
    {
        SightSensor sense = (SightSensor)target;
        Vector3 pos = sense.transform.position;

        // Radius sensor (putih)
        Handles.color = Color.white;
        Handles.DrawWireArc(pos, Vector3.up, Vector3.forward, 360, sense.radius);

        // Dua sudut terluar kerucut pandang (hijau)
        Vector3 viewAngle01 = DirectionFromAngle(sense.transform.eulerAngles.y, -sense.angle / 2);
        Vector3 viewAngle02 = DirectionFromAngle(sense.transform.eulerAngles.y, sense.angle / 2);

        Handles.color = Color.green;
        Handles.DrawLine(pos, pos + viewAngle01 * sense.radius);
        Handles.DrawLine(pos, pos + viewAngle02 * sense.radius);
        Handles.DrawWireArc(pos, Vector3.up, viewAngle01, sense.angle, sense.radius);

        // Garis merah ke target kalau sedang terdeteksi
        if (sense.TargetSensed && sense.Target != null)
        {
            Handles.color = Color.red;
            Handles.DrawLine(pos, sense.Target.position);
        }
    }

    Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}

[CustomEditor(typeof(HearingSensor))]
public class HearingSensorEditor : Editor
{
    void OnSceneGUI()
    {
        HearingSensor sense = (HearingSensor)target;
        Vector3 pos = sense.transform.position;

        // Radius pendengaran (cyan)
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(pos, Vector3.up, sense.radius);

        // Garis kuning ke sumber suara kalau sedang terdengar
        if (sense.HeardSomething)
        {
            Handles.color = Color.yellow;
            Handles.DrawLine(pos, sense.LastHeardPosition);
        }
    }
}