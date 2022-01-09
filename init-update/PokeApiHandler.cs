using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Pokerole.Tools.InitUpdate
{
	static class PokeApiHandler
	{
		private static readonly String cachePath = Path.GetFullPath(Path.Combine(".", "pokeapi.co cache"));
		public static async IAsyncEnumerable<JObject> PerformRequest(String endpointRequest)
		{
			using WebClient client = new WebClient();
			//Note: calling the first page "page 1" since we don't know if we have a next one or not
			String? nextPage = null;
			for (int i = 1; i == 1 || nextPage != null; i++)
			{
				JObject page = await FetchResult(endpointRequest, i, nextPage, client);
				nextPage = (String?)page["next"];
				yield return page;
			}
		}
		private static async Task<JObject> FetchResult(String endpointRequest, int page, String? nextPage,
			WebClient client)
		{
			String filename = JsonFileForRequest(endpointRequest, page);
			if (File.Exists(filename))
			{
				using JsonReader reader = new JsonTextReader(new StreamReader(File.OpenRead(filename)));
				return await JObject.LoadAsync(reader);
			}
			//not cached, fetch it
			String fetchUri = nextPage ?? String.Concat("https://pokeapi.co/api/v2/", endpointRequest);
			if (!Uri.TryCreate(fetchUri, UriKind.Absolute, out Uri? uri))
			{
				throw new ArgumentException("Invalid Uri", nameof(endpointRequest));
			}
			String json = await client.DownloadStringTaskAsync(uri);
			String parentDir = Path.GetDirectoryName(filename) ?? throw new WasNullException();
			Directory.CreateDirectory(parentDir);
			var task = File.WriteAllTextAsync(filename, json);
			var result = JObject.Parse(json);
			await task;
			return result;
		}
		private static String JsonFileForRequest(String endpointRequest, int page)
		{
			String fileName = String.Concat(endpointRequest, " page ", page, ".json");
			return Path.Combine(cachePath, fileName);
		}
	}
}
;