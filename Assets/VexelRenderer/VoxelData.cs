using System.Collections;
using System.Collections.Generic;
using UnityEngine;




namespace VoxelRender
{
	

	[System.Serializable]
	public class VoxelData:IDG.ISerializable {
		// 序列化数据
		public V3 size;

		// public string materialPath;
		// public string texturePath;
		public List<VoxelInfo> voxels=new List<VoxelInfo>();
		public void Serialize(IDG.ByteProtocol protocol){
			protocol.push(voxels.Count);
			foreach (var v in voxels)
			{
				v.Serialize(protocol);
			}
		
		}
		public void Deserialize(IDG.ByteProtocol protocol){
			voxels=new List<VoxelInfo>();
			var len=protocol.getInt32();
			for (int i = 0; i < len; i++)
			{
				voxels.Add(new VoxelInfo().Deserialize(protocol));
			}
		}
		public VoxelData ParseVox(Vox vox){
			size=vox.size;
			voxels.Clear();
			foreach (var voxel in vox.voxels)
			{
				float h,s,v;
				Color.RGBToHSV(vox.colors[voxel.colorIndex].ToColor(),out h,out s,out v);
				var index=VoxelRenderManager.HSVToIndex(h,s,v);
				voxels.Add(new VoxelInfo(voxel.pos,index));
			}
			return this;
		}
		// public void SetSceneScale(int x,int y,int z){
		// 	size.x=x;
		// 	size.y=y;
		// 	size.z=z;
		// }
		
		
		
	}
	public static class V3Extend{
		public static V3 ToV3(this Vector3 vector3){
			var v3=new V3();
			v3.x=(int)vector3.x;
			v3.y=(int)vector3.y;
			v3.z=(int)vector3.z;
			return v3;
		}
	}
	[System.Serializable]
	public struct V3{
		public int x;
		public int y;
		public int z;
		public V3(int init){
			x=init;
			y=init;
			z=init;
		}

		public V3 MixMin(V3 other){
			var v=new V3();
			v.x=Mathf.Min(x,other.x);
			v.y=Mathf.Min(y,other.y);
			v.z=Mathf.Min(z,other.z);
			return v;
		}
		public V3 MixMax(V3 other){
				var v=new V3();
			v.x=Mathf.Max(x,other.x);
			v.y=Mathf.Max(y,other.y);
			v.z=Mathf.Max(z,other.z);
			return v;
		}
		public Vector3 ToVector3(){
			return new Vector3(x,y,z);
		}

	}
	[System.Serializable]
	public struct ColorInfo{
		public byte r;
		public byte g;
		public byte b;
		public byte a;

		public Color ToColor(){
			return new Color(r/255f,g/255f,b/255f,a/255f);
		}
	
	}
	[System.Serializable]
	public struct VoxelInfo{
		public int colorIndex;
		public V3 pos;
		public VoxelInfo(V3 pos,int index){
			this.pos=pos;
			colorIndex=index;
		}
		public void Serialize(IDG.ByteProtocol protocol){
			protocol.push(colorIndex);
			protocol.push(pos.x);
			protocol.push(pos.y);
			protocol.push(pos.z);
		}
		public VoxelInfo Deserialize(IDG.ByteProtocol protocol){
			colorIndex=protocol.getInt32();
			pos.x=protocol.getInt32();
			pos.y=protocol.getInt32();
			pos.z=protocol.getInt32();
			return this;
		}
	}
	

}