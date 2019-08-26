using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// using System.IO;
namespace VoxelRender
{
public class VoxelRenderManager{
	

	public static Vector3 Fixed(Vector3 position,float GridSize){
		var newPos=new Vector3(
			Fixed(position.x,GridSize),
			Fixed(position.y,GridSize),
			Fixed(position.z,GridSize)
		);
		return newPos;
	}
	public static float Fixed(float value,float GridSize){
		var fixeScale=(value)%GridSize;
		if(Mathf.Abs(fixeScale)<GridSize/2){
			return value-fixeScale;
		}else
		{
			return value+FixedInvert(fixeScale,GridSize);
		}
		
	}
	public static float FixedInvert(float value,float GridSize){
		if(value>=0){
			return GridSize-value;
		}else
		{
			return -(GridSize+value);
		}
	}
	public static GameObject CreateViewObj(VoxelData voxelData,Material material,VoxelInfo voxelInfo,float gridSize,Transform parent){
	
		var obj=new GameObject("Voxel " );
		obj.transform.position=parent.position+(voxelInfo.pos.ToVector3()-voxelData.size.ToVector3()/2+voxelData.size.y*Vector3.up/2+Vector3.one/2)*gridSize;
		obj.transform.localScale=Vector3.one*gridSize;
		obj.transform.parent=parent;
	
		
		obj.hideFlags=HideFlags.HideInHierarchy;
		var mesh=CreateViewMesh(voxelInfo.colorIndex);

		obj.AddComponent<MeshFilter>().mesh=mesh;
		obj.AddComponent<MeshRenderer>().material=material;
		return obj;
	}
	public static GameObject CreateViewObj(int colorIndex,Material material,float gridSize,Transform parent){
	
		var obj=new GameObject(colorIndex.ToString());
		
		obj.transform.localScale=Vector3.one*gridSize;
		obj.transform.parent=parent;
	
		
		obj.hideFlags=HideFlags.HideInHierarchy;
		var mesh=CreateViewMesh(colorIndex);

		obj.AddComponent<MeshFilter>().mesh=mesh;
		obj.AddComponent<MeshRenderer>().material=material;
		return obj;
	}
	public static Texture2D CreatTexture(){
		var colors=new Color[360*10*10];
		var texture=new Texture2D(360,10*10,TextureFormat.RGBA32,false);
		for (int h = 0; h < 360; h++)
		{
			for (int s = 0; s < 10; s++)
			{
				for (int v = 0; v < 10; v++)
				{
					colors[h+s*360*10+v*360]= Color.HSVToRGB(h/360f,s/10f,v/10f);
				}
			}
		}
		
		texture.SetPixels(colors);
		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.Apply(false);
		return texture;
	}
	public static int HSVToIndex(float h,float s,float v){
		return (int)(h*359)+((int)(s*9)*10+(int)(v*9))*360;
	}
	public static Vector2 IndexToUv(int index){
		int x=index%360;
		int y=index/360;
		return new Vector2(x/360f,y/100f);
	}
	public static float halfSize = 0.5f;
	public static Vector3[] vertexs = new Vector3[] {
			new Vector3 (halfSize, -halfSize, halfSize),
			new Vector3 (-halfSize, -halfSize, halfSize),
			new Vector3 (halfSize, halfSize, halfSize),
			new Vector3 (-halfSize, halfSize, halfSize),
			new Vector3 (halfSize, halfSize, -halfSize),
			new Vector3 (-halfSize, halfSize, -halfSize),
			new Vector3 (halfSize, -halfSize, -halfSize),
			new Vector3 (-halfSize, -halfSize, -halfSize),
			new Vector3 (halfSize, halfSize, halfSize),
			new Vector3 (-halfSize, halfSize, halfSize),
			new Vector3 (halfSize, halfSize, -halfSize),
			new Vector3 (-halfSize, halfSize, -halfSize),
			new Vector3 (halfSize, -halfSize, -halfSize),	
			new Vector3 (halfSize, -halfSize, halfSize),
			new Vector3 (-halfSize, -halfSize, halfSize),
			new Vector3 (-halfSize, -halfSize, -halfSize),
			new Vector3 (-halfSize, -halfSize, halfSize),
			new Vector3 (-halfSize, halfSize, halfSize),
			new Vector3 (-halfSize, halfSize, -halfSize),
			new Vector3 (-halfSize, -halfSize, -halfSize),
			new Vector3 (halfSize, -halfSize, -halfSize),
			new Vector3 (halfSize, halfSize, -halfSize),
			new Vector3 (halfSize, halfSize, halfSize),
			new Vector3 (halfSize, -halfSize, halfSize),
			};

	public static	int[] triangles = new int[] {
				0, 2, 3, 
				0, 3, 1, 
				8, 4, 5, 
				8, 5, 9, 
				10, 6, 7, 
				10, 7, 11, 
				12, 13, 14, 
				12, 14, 15, 
				16, 17, 18, 
				16, 18, 19, 
				20, 21, 22, 
				20, 22, 23, 
			};

	public static Mesh CreateViewMesh(int colorIndex){
		
		var uv= IndexToUv(colorIndex);
		Vector2[] uvs = new Vector2[] {
			uv,uv,uv,uv,uv,uv,uv,uv,uv,uv,uv,uv,uv,uv,uv,uv,uv,uv,uv,uv,uv,uv,uv,uv
		};
		Mesh mesh = new Mesh ();
		mesh.vertices = vertexs;
		mesh.uv = uvs;
		mesh.colors = null;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();	
		return mesh;
	}
	public static Material CreateMaterial(VoxelData voxel,Texture2D tex){
		var material=new Material (Shader.Find ("Diffuse"));
		material.mainTexture=tex;
		return material;
	}
	// public static VoxelData Parse(Dictionary<V3,int> voxelColors){
	// 	VoxelData voxel=GetData();
	// 	var minPos=new V3(int.MaxValue);
	// 	var maxPos=new V3(int.MinValue);
	// 	foreach (var vc in voxelColors)
	// 	{
	// 		minPos=minPos.MixMin(vc.Key);
	// 		maxPos=maxPos.MixMax(vc.Key);
	// 	}
	// 	voxel.SetSceneScale(maxPos.x-minPos.x,maxPos.y-minPos.y,maxPos.z-minPos.z);
	// 	foreach (var vc in voxelColors)
	// 	{
	// 		voxel.SetVoxel(vc.Key.x-minPos.x,vc.Key.y-minPos.y,vc.Key.z-minPos.z,vc.Value);
	// 	}
	// 	return voxel;
	// }
	// public static VoxelData Parse(Dictionary<V3,Color> voxelColors){
	// 	VoxelData voxel=GetData();
	// 	var colors=new Dictionary<Color,int>();
	// 	var minPos=new V3(int.MaxValue);
	// 	var maxPos=new V3(int.MinValue);
	// 	foreach (var vc in voxelColors)
	// 	{
			
	// 			minPos=minPos.MixMin(vc.Key);
	// 			maxPos=maxPos.MixMax(vc.Key);
			
			
	// 	}
	// 	voxel.SetSceneScale(maxPos.x-minPos.x,maxPos.y-minPos.y,maxPos.z-minPos.z);
	// 	foreach (var vc in voxelColors)
	// 	{
	// 		var colorIndex=-1;
		
	// 		if(colors.ContainsKey(vc.Value)){
	// 			colorIndex=colors[vc.Value];
	// 		}else
	// 		{
	// 			colorIndex=colors.Count;
	// 			colors.Add(vc.Value,colorIndex);
	// 		}

			
	// 		voxel.SetVoxel(vc.Key.x-minPos.x,vc.Key.y-minPos.y,vc.Key.z-minPos.z,colorIndex);
	// 	}
	// 	foreach (var ci in colors)
	// 	{
	// 		var color= new ColorInfo();
	// 		color.r=ci.Key.r;
	// 		color.g=ci.Key.g;
	// 		color.b=ci.Key.b;
	// 		color.a=ci.Key.a;
	// 		voxel.colors[ci.Value]=color;
	// 	}
	// 	return voxel;
	// }
}

}