using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
namespace IDG
{
	public class Serialization  {

		public static Byte[] Serialize(ISerializable obj){
			return Serialize(obj.Serialize);
		}
		public static void Deserialize(ISerializable obj,byte[] byteData){
			Deserialize(obj.Deserialize,byteData);
		}
		public static Byte[] Serialize(Action<ByteProtocol> serialize){
			var pro=new ByteProtocol();
			serialize(pro);
			return pro.GetByteStream();
		}

		public static void Deserialize(Action<ByteProtocol> deserialize,byte[] byteData ){
			var pro=new ByteProtocol();
			pro.InitMessage(byteData);
			deserialize(pro);
		}

	}	

	public interface ISerializable {
		void Serialize(ByteProtocol protocol);
		void Deserialize(ByteProtocol protocol);
	}

	public class DataFile{
		public static void SerializeToFile(string filePath,ISerializable obj)
	     {
	     	using (System.IO.FileStream fs = System.IO.File.Create(filePath))
			{
				var bytes=Serialization.Serialize(obj);
				fs.Write(bytes,0,bytes.Length);
				fs.Close();
	   		}
		}
		public static void DeserializeToData(string filePath,ISerializable obj)
		{
			using (System.IO.FileStream fs = System.IO.File.Open(filePath, System.IO.FileMode.Open))
			{
				var bytes=new Byte[fs.Length];
				fs.Read(bytes,0,(int)fs.Length);
				fs.Close();
				Serialization.Deserialize(obj,bytes);
			}
		}
	}


}
