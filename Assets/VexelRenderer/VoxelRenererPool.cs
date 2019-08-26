using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace VoxelRender
{
	

public class VoxelRenererPool : MonoBehaviour {
	public static VoxelRenererPool _instance;
	public static VoxelRenererPool Instance{
		get{
			// if(_instance==null){
			// 	_instance=new GameObject("VoxelRenererPool").AddComponent<VoxelRenererPool>();
			// }
			return _instance;
		}
	}

	private void Awake() {
		_instance=this;
	}
	public Material material;
	public Dictionary<int,Stack<Transform>> pools=new Dictionary<int,Stack<Transform>>();
	public Dictionary<Vector3,int> voxelsIndex=new Dictionary<Vector3,int>();
	public Dictionary<Vector3,Transform> voxelsShow=new Dictionary<Vector3,Transform>();
	public Transform GetObj(int colorIndex){
		if(pools.ContainsKey(colorIndex)){
			if(pools[colorIndex].Count>0){
				pools[colorIndex].Peek().gameObject.SetActive(true);
				return pools[colorIndex].Pop();
			}
		}
		return VoxelRenderManager.CreateViewObj(colorIndex,material,0.05f,transform).transform;
	}
	public void RecoverObj(int colorIndex,Transform obj){
		if(pools.ContainsKey(colorIndex)){
			
		}else
		{
			pools[colorIndex]=new Stack<Transform>();
		}
		obj.gameObject.SetActive(false);
		pools[colorIndex].Push(obj);
	}
	public void SetVoxel(Vector3 pos,int colorIndex){
		if(!voxelsIndex.ContainsKey(pos)){
		   voxelsIndex.Add(pos,colorIndex);
		}
		
	}
	private void LateUpdate() {
		ClearShow();
		foreach (var item in voxelsIndex)
		{
			var voxel=GetObj(item.Value);
		 	voxel.transform.position=item.Key;
			voxelsShow.Add(item.Key,voxel);
		}
	//	Debug.LogError(voxelsIndex.Count);
		ClearBuffer();
	}
	public void ClearBuffer(){
		voxelsIndex.Clear();
	}
	public void ClearShow(){
		foreach (var item in voxelsShow)
		{
			RecoverObj(int.Parse(item.Value.name),item.Value);
		}
		voxelsShow.Clear();
	}
}
}
