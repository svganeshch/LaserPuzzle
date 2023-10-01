using Unity.VisualScripting;
using UnityEngine;

public class MouseToRotation
{
    private Vector2 rotation;
    private Quaternion xQuat;
    private Quaternion yQuat;

    public MouseToRotation()
    {
        rotation = Vector2.zero;
        xQuat = Quaternion.identity;
        yQuat = Quaternion.identity;
    }

    public Quaternion GetRotation(Vector2 val)
    {
        rotation += new Vector2(val.x, val.y);

        rotation.y = Mathf.Clamp(rotation.y, -90, 90);
        xQuat = Quaternion.AngleAxis(rotation.x, Vector3.up);
        yQuat = Quaternion.AngleAxis(rotation.y, Vector3.left);

        return xQuat * yQuat;
    }

    public Quaternion GetMirrorRotation(Vector2 val, float snap)
    {
        rotation += new Vector2(val.x * snap, val.y * snap);

        rotation.x = Mathf.Clamp(rotation.x, -90, 90);
        rotation.y = Mathf.Clamp(rotation.y, -90, 90);

        xQuat = Quaternion.AngleAxis(rotation.x, Vector3.up);
        yQuat = Quaternion.AngleAxis(rotation.y, Vector3.left);

        return xQuat * yQuat;
    }
}
