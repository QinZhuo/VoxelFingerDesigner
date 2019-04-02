using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VoxelRender;
public class ColorSelect : MonoBehaviour {
	public Material _mat;
	public Image colorPrefab;
	public Material material{
		get{
			return _mat;
		}	
		set{
			_mat=value;
		}
	}

	public void InitColor(){
		for (int i = 0; i < 256; i++)
		{
				
		}
	}
}
