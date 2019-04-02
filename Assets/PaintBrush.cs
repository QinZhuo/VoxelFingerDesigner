using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelRender;
public enum PaintAction
{
	none,
	paint,
	rotate,
	delete,
}
public enum RotateType{
	none,
	X,
	Y,
}
public class PaintSetting{
	public static float MoveXSpeed=130;
	public static float MoveYSpeed=130;
}
public class PaintBrush : MonoBehaviour {
	public Transform canvas;
	 V3 size;
	public Material colorMatrial;
	public Camera cam;
	public Transform camRoot;
	public GameObject VoxelPrefab; 

	 GameObject brush;
	RaycastHit hit;
	 PaintAction action;
	 V3 curentPos;
	Vector3 lastPos;
	public Transform[,,] voxels;
	public Dictionary<V3,int> voxleColors;
	 MeshRenderer renderer;
	 MeshFilter meshFilter;
	public List<V3> curentPaintList;
	private void Start() {
		Init(new V3(5));
	}
	public void SetSize(string size){
		Init(new V3(int.Parse(size) ));
	}
	public void SaveVoxelData(){
		
		var voxelData=VoxelRenderManager.Parse(voxleColors);
		var path=Application.persistentDataPath+"/TestData"+GetHashCode()+".voxData";
		IDGData.SerialData(path,voxelData);
		Debug.Log("SaveDataOver Path: "+path);
	}
	public void Init(V3 size){
		camRoot.localScale=size.ToVector3();
		canvas.localScale=size.ToVector3();
		camRoot.position=size.ToVector3()*GridSize/2;
		this.size=size;
		voxels=new Transform[size.x,size.y,size.z];
		action=PaintAction.none;
		voxleColors=new Dictionary<V3, int>();
		FingerInput.ScaleChange=ChangeCamDistance;
	}
	bool CanPain{
		get{
			return succesPos&&!curentPaintList.Contains(curentPos);
		}
	}
	bool succesPos{
		get{
			if(curentPos.x>=0&&curentPos.x<size.x&&
			curentPos.y>=0&&curentPos.y<size.y&&
			curentPos.z>=0&&curentPos.z<size.z
			){
				return true;
			}
			return false;
		}
	}
	float GridSize=1;
	int colorIndex;
	int brushColor{
		set{
			meshFilter.mesh=VoxelRenderManager.CreateViewMesh(value);
			colorIndex=value;
		}
		get{
			return colorIndex;
		}
	}
	// Use this for initialization
	Vector3 GetPaintPoint(){
	
		// for (int i = 0; i < hits.Length; i++)
		// {
		// 	lastPos=hits[i].point;
		// 	return hits[i].point+hits[i].normal*GridSize/2;
		// }
		if(Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition),out hit)){
			return hit.point+hit.normal*GridSize/2;
		}
		return Vector3.one*-1;
	}

	void ShowBrush(Vector3 pos){
		if(brush==null){
			brush=Instantiate(VoxelPrefab,pos,Quaternion.identity);
			renderer=brush.GetComponentInChildren<MeshRenderer>();
			meshFilter=brush.GetComponentInChildren<MeshFilter>();
			renderer.material=colorMatrial;
			brushColor=0;
		}
		lastPos=pos;
		curentPos=(VoxelRenderManager.Fixed(pos-Vector3.one*GridSize/2,GridSize)/GridSize).ToV3();
		if(!CanPain){brush.SetActive(false);return;}
		if(!brush.activeSelf){brush.SetActive(true);}
		brush.transform.position=curentPos.ToVector3()*GridSize;
		
	}
	private void OnDrawGizmos() {
		Gizmos.color=Color.red;
		Gizmos.DrawSphere(curentPos.ToVector3()*GridSize,0.01f);
			Gizmos.color=Color.green;
		Gizmos.DrawSphere(lastPos,0.01f);
	}
	void Paint(){
		if(action!=PaintAction.none&&action!=PaintAction.paint)return;
		if(Input.GetKeyDown(KeyCode.Mouse0)&&CanPain){
			action=PaintAction.paint;
		}
		if(Input.GetKey(KeyCode.Mouse0)&&CanPain){
			curentPaintList.Add(curentPos);
			voxels[curentPos.x,curentPos.y,curentPos.z]=Instantiate(brush,brush.transform.position,Quaternion.identity).transform;
			if(voxleColors.ContainsKey(curentPos)){
				voxleColors[curentPos]=colorIndex;
			}
		}
		if(Input.GetKeyUp(KeyCode.Mouse0)){
			brushColor=Random.RandomRange(0,256);
			PaintOver();
		}
	}
	public RotateType rotateType;
	public void Rotate(){
		if(action!=PaintAction.none&&action!=PaintAction.rotate)return;
		
		if(Input.GetKeyDown(KeyCode.Mouse0)&&!succesPos){
			action=PaintAction.rotate;
		}
		if(action==PaintAction.rotate){
			if(Input.GetKey(KeyCode.Mouse0)){
				if(rotateType==RotateType.none){
					var x= Input.GetAxis("Mouse X");
					var y=Input.GetAxis("Mouse Y");
					if(Mathf.Abs(x)*Time.deltaTime>=0.01f||Mathf.Abs(y)*Time.deltaTime>=0.01f){
						if(Mathf.Abs(y)>Mathf.Abs(x)){
							rotateType=RotateType.Y;
						}else
						{
							rotateType=RotateType.X;
						}
					}
				}
				var rot=Vector3.zero;
				if(rotateType==RotateType.X){
					rot=new Vector3(0,Input.GetAxis("Mouse X"),0);
					camRoot.transform.Rotate(rot,Time.deltaTime*PaintSetting.MoveXSpeed);
				}else if(rotateType==RotateType.Y){
					rot=new Vector3(-Input.GetAxis("Mouse Y"),0,0);
					camRoot.GetChild(0).transform.Rotate(rot,Time.deltaTime*PaintSetting.MoveYSpeed);
				}
				
				
			}
			if(Input.GetKeyUp(KeyCode.Mouse0)){
				action=PaintAction.none;
				rotateType=RotateType.none;
			}
		}
	
	}
	public void PaintOver(){
		if(action!=PaintAction.paint)return;
		foreach (var v in curentPaintList)
		{
			voxels[v.x,v.y,v.z].GetChild(0).gameObject.AddComponent<MeshCollider>();
		}
		curentPaintList.Clear();
		action=PaintAction.none;
	}
	public void ChangeCamDistance(float value){
		camRoot.localScale=camRoot.localScale+Vector3.one*value;
	}
	// Update is called once per frame
	void Update () {
	
		var pos=GetPaintPoint();
		ShowBrush(pos);
		Paint();
		Rotate();

	}
}
