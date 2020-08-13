using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace SqlDb.Data.Serialization
{
	public static class JsonConvert
	{
		public static DataContractJsonSerializerSettings DefaultSettings
		{
			get;
		} = new DataContractJsonSerializerSettings
		{
			UseSimpleDictionaryFormat = true
		};


		public static TObject Deserialize<TObject>(string json)
		{
			DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(TObject), DefaultSettings);
			using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
			{
				return (TObject)dataContractJsonSerializer.ReadObject((Stream)stream);
			}
		}

		public static object Deserialize(string json, Type type)
		{
			DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(type, DefaultSettings);
			using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
			{
				return dataContractJsonSerializer.ReadObject((Stream)stream);
			}
		}

		public static TObject Deserialize<TObject>(string json, DataContractJsonSerializerSettings settings)
		{
			DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(TObject), settings);
			using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
			{
				return (TObject)dataContractJsonSerializer.ReadObject((Stream)stream);
			}
		}

		public static TObject Deserialize<TObject>(byte[] bytes)
		{
			DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(TObject), DefaultSettings);
			using (MemoryStream stream = new MemoryStream(bytes))
			{
				return (TObject)dataContractJsonSerializer.ReadObject((Stream)stream);
			}
		}

		public static TObject Deserialize<TObject>(byte[] bytes, DataContractJsonSerializerSettings settings)
		{
			DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(TObject), settings);
			using (MemoryStream stream = new MemoryStream(bytes))
			{
				return (TObject)dataContractJsonSerializer.ReadObject((Stream)stream);
			}
		}

		public static string Serialize<TObject>(TObject data)
		{
			DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(data.GetType(), DefaultSettings);
			using (MemoryStream memoryStream = new MemoryStream())
			{
				dataContractJsonSerializer.WriteObject((Stream)memoryStream, (object)data);
				byte[] bytes = memoryStream.ToArray();
				return Encoding.UTF8.GetString(bytes);
			}
		}

		public static string Serialize<TObject>(TObject data, DataContractJsonSerializerSettings settings)
		{
			DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(data.GetType(), settings);
			using (MemoryStream memoryStream = new MemoryStream())
			{
				dataContractJsonSerializer.WriteObject((Stream)memoryStream, (object)data);
				byte[] bytes = memoryStream.ToArray();
				return Encoding.UTF8.GetString(bytes);
			}
		}

		public static byte[] SerializeBytes<TObject>(TObject data)
		{
			DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(data.GetType(), DefaultSettings);
			using (MemoryStream memoryStream = new MemoryStream())
			{
				dataContractJsonSerializer.WriteObject((Stream)memoryStream, (object)data);
				return memoryStream.ToArray();
			}
		}

		public static byte[] SerializeBytes<TObject>(TObject data, DataContractJsonSerializerSettings settings)
		{
			DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(data.GetType(), settings);
			using (MemoryStream memoryStream = new MemoryStream())
			{
				dataContractJsonSerializer.WriteObject((Stream)memoryStream, (object)data);
				return memoryStream.ToArray();
			}
		}
	}
}
