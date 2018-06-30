﻿namespace AssemblyCSharp
{
	using System;
	using System.IO;
	using System.IO.Compression;
	using System.Text;

	public static class FileUtils
	{
		public static void StoreFile(string filePath, string content, bool compress = false)
		{
			if (compress)
				StoreDeflateFile(filePath, content);
			else
				File.WriteAllText(filePath, content);
		}

		public static void StoreFile(string filePath, byte[] content, bool compress = false)
		{
			if (compress)
				StoreDeflateFile(filePath, content);
			else
				File.WriteAllBytes(filePath, content);
		}

		//Based on https://stackoverflow.com/a/7343623
		// -->
#if NETFX_CORE || NET_2_0 || NET_2_0_SUBSET
		public static void CopyTo(this Stream src, Stream dest)
		{
			byte[] bytes = new byte[4096];
			int cnt;
			while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
				dest.Write(bytes, 0, cnt);
		}
#endif

		public static void StoreGZipFile(string filePath, string content)
		{
			StoreGZipFile(filePath, Encoding.UTF8.GetBytes(content));
		}

		public static void StoreGZipFile(string filePath, byte[] content)
		{
			using (MemoryStream msi = new MemoryStream(content))
			using (FileStream fs = File.Create(filePath))
			using (var gs = new GZipStream(fs, CompressionMode.Compress))
				msi.CopyTo(gs);
		}

		public static void StoreDeflateFile(string filePath, string content)
		{
			StoreDeflateFile(filePath, Encoding.UTF8.GetBytes(content));
		}

		public static void StoreDeflateFile(string filePath, byte[] content)
		{
			using (MemoryStream msi = new MemoryStream(content))
			using (FileStream fs = File.Create(filePath))
			using (var ds = new DeflateStream(fs, CompressionMode.Compress))
				msi.CopyTo(ds);
		}

		public static byte[] LoadGZipFile(string filePath)
		{
			using (FileStream fs = File.Create(filePath))
			using (MemoryStream mso = new MemoryStream())
			{
				using (var gs = new GZipStream(fs, CompressionMode.Decompress))
					gs.CopyTo(mso);
				return mso.ToArray();
			}
		}

		public static string LoadGZipFileText(string filePath)
		{
			return Encoding.UTF8.GetString(LoadGZipFile(filePath));
		}

		public static byte[] LoadDeflateFile(string filePath)
		{
			using (FileStream fs = File.Create(filePath))
			using (MemoryStream mso = new MemoryStream())
			{
				using (var gs = new DeflateStream(fs, CompressionMode.Decompress))
					gs.CopyTo(mso);
				return mso.ToArray();
			}
		}

		public static string LoadDeflateFileText(string filePath)
		{
			return Encoding.UTF8.GetString(LoadDeflateFile(filePath));
		}

		public static byte[] UncompressDeflate(byte[] compressed)
		{
			using (var msi = new MemoryStream(compressed))
			using (MemoryStream mso = new MemoryStream())
			{
				using (var gs = new DeflateStream(msi, CompressionMode.Decompress))
					gs.CopyTo(mso);
				return mso.ToArray();
			}
		}
		// <--
	}
}