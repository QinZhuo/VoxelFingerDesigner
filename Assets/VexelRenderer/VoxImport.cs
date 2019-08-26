using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
//using UnityEditor;
namespace VoxelRender
{
	public class Vox{
		public V3 size;
		public List<VoxelInfo> voxels=new List<VoxelInfo>();
		public List<ColorInfo> colors=new List<ColorInfo>();
		public void SetVoxel(int x,int y, int z,int index){
			var v=new VoxelInfo();
			v.pos.x=x;
			v.pos.y=y;
			v.pos.z=z;
			v.colorIndex=index;
			voxels.Add(v);
		}
	}
	public class VoxImporter{

	
		public static Vox LoadVoxFile(string path){
			if(path==null|| path=="")return null;
			byte[] bytes=File.ReadAllBytes(path);
			using(MemoryStream memoryStream=new MemoryStream(bytes)){
				using(BinaryReader binaryReader=new BinaryReader(memoryStream)){
					return ReadData(binaryReader);
				}
			}
		}
		public static Vox ReadData(BinaryReader reader){
			var voxFlag= reader.ReadBytes(4);
			if(!Compare("VOX ",voxFlag)){
				ErrorCantFind("VOX");
				return null;
			}
			var version=reader.ReadBytes(4);
			var main=reader.ReadBytes(4);
			if(!Compare("MAIN",main)){
				ErrorCantFind("MAIN");
				return null;
			}
			var vox=new Vox();
			int mainSize=reader.ReadInt32();
			int childSize=reader.ReadInt32();
			reader.ReadBytes(mainSize);
			int readSize=0;
			string unKnowNode="";
			while(readSize<childSize){
				var nodeType=reader.ReadBytes(4);
				if(Compare("PACK",nodeType)){
					int packDataSize=reader.ReadInt32();
					int packChildSize=reader.ReadInt32();
					reader.ReadInt32();
					readSize+=packDataSize+packChildSize+4*3;
				}else if(Compare("SIZE",nodeType)){
					readSize+= ReadSize(reader,vox);
				}else if(Compare("XYZI",nodeType)){
					readSize+=ReadVoxelIndex(reader,vox);
				}else if(Compare("RGBA",nodeType)){
					readSize+=ReadColor(reader,vox);
				}else{
					var typeStr=System.Text.Encoding.ASCII.GetString(nodeType);
					if(!unKnowNode.Contains(typeStr)){
						unKnowNode+="["+typeStr+"]";
					}
					
					int chunkContentBytes=reader.ReadInt32();
					int childrenBytes=reader.ReadInt32();
					reader.ReadBytes(chunkContentBytes+childrenBytes);
					readSize+=chunkContentBytes+childrenBytes+12;
				}
			}
			Warning(" 不支持解析节点类型 "+unKnowNode); 
			Log(" 导入体素模型完成 体素数目"+vox.voxels.Count); 
			return vox;
		}
		static int ReadSize(BinaryReader reader,Vox vox){
			int dataSize=reader.ReadInt32();
			int childSize=reader.ReadInt32();
			vox.size=new V3();
			vox.size.x=reader.ReadInt32();
			vox.size.y=reader.ReadInt32();
			vox.size.z=reader.ReadInt32();
			if(childSize>0){
				reader.ReadBytes(childSize);
				Warning("Size节点拥有多余未知数据");
			}
			return dataSize+childSize+4*3;
		}
		static int ReadVoxelIndex(BinaryReader reader,Vox vox){
			int dataSize=reader.ReadInt32();
			int childSize=reader.ReadInt32();
			int voxelsCount=reader.ReadInt32();
			for (int i = 0; i < voxelsCount; i++)
			{
				var x=(int)reader.ReadByte();
				var z=(int)reader.ReadByte();
				var y=(int)reader.ReadByte();
				vox.SetVoxel(x,y,z,(int)reader.ReadByte());
			}
			if(childSize>0){
				reader.ReadBytes(childSize);
				Warning("XYZI节点拥有多余未知数据");
			}
			return dataSize+childSize+4*3;
		}
		static int ReadColor(BinaryReader reader,Vox vox){
			int dataSize=reader.ReadInt32();
			int childSize=reader.ReadInt32();
		
			for (int i = 0; i < 256; i++)
			{
				var colorInfo=new ColorInfo();
				colorInfo.r=reader.ReadByte();
				colorInfo.g=reader.ReadByte();
				colorInfo.b=reader.ReadByte();
				colorInfo.a=reader.ReadByte();
				vox.colors.Add(colorInfo);
			
			}
			if(childSize>0){
				reader.ReadBytes(childSize);
				Warning("RGBA节点拥有多余未知数据");
			}
			return dataSize+childSize+4*3;
		}
		static void ErrorCantFind(string cantFindStr){
			Error("格式出错 找不到"+cantFindStr+"标志");
		}
		static void Error(string error){
			UnityEngine.Debug.LogError("【VoxImporter】解析Vox文件失败 "+error);
		}
		static void Warning(string info){
			UnityEngine.Debug.LogWarning("【VoxImporter】"+info);
		}
		static void Log(string info){
			UnityEngine.Debug.Log("【VoxImporter】"+info);
		}
		public static bool Compare(string str,byte[] bytes,int offset=0){
			for (int i = 0; i < str.Length; i++)
			{
				if(bytes[offset+i]!=str[i]||(offset+i)>=bytes.Length){
					return false;
				}
			}
			return true;
		}
	}

}