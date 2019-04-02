using UnityEngine; 
using System.Collections; 
public class CameraProjectionChange : MonoBehaviour
{
	public Camera cam;
    public Transform target;
    [Range(0,1)] public float lerp; 

 
	private void LateUpdate() {
        var size=((cam.transform.position-target.position).magnitude)*Mathf.Tan(cam.fieldOfView/2);
		var ratio = Screen.width / (float)Screen.height;
		var a = Matrix4x4.Perspective(cam.fieldOfView, ratio, cam.nearClipPlane, cam.farClipPlane);
		var b = Matrix4x4.Ortho(-size * ratio, size * ratio, -size, size, cam.nearClipPlane, cam.farClipPlane);
        cam.projectionMatrix = Lerp(a, b, lerp);
    }
    Matrix4x4 Lerp(Matrix4x4 a, Matrix4x4 b, float lerp)
    { 
		var result = new Matrix4x4();
        result.SetRow(0, Vector4.Lerp(a.GetRow(0), b.GetRow(0), lerp));
        result.SetRow(1, Vector4.Lerp(a.GetRow(1), b.GetRow(1), lerp));
        result.SetRow(2, Vector4.Lerp(a.GetRow(2), b.GetRow(2), lerp));
        result.SetRow(3, Vector4.Lerp(a.GetRow(3), b.GetRow(3), lerp)); 
		return result;
    }

    public void Ortho(bool ortho){
        if(ortho){
            lerp=1;
        }else
        {
            lerp=0;
        }
    }
}