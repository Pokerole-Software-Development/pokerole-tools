using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Pokerole.Tools.InitUpdate
{
	class PokeApiHandler
	{
		private const bool awaitConfig = false;
		private static readonly String cachePath = Path.GetFullPath(Path.Combine(".", "pokeapi.co cache"));
		public static PokeApiHandler Instance { get; } = new PokeApiHandler();
		private readonly SemaphoreSlim throttle;
		private PokeApiHandler()
		{
			//don't overwhelm the api server
			throttle = new SemaphoreSlim(5);
			//see https://stackoverflow.com/a/47199380/1366594
			AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
			{
				throttle.Dispose();
			};
		}
		public async IAsyncEnumerable<JObject> PerformRequest(String endpointRequest)
		{
			String request = endpointRequest;
			if (request.StartsWith("https://pokeapi.co/api/v2/"))
			{
				//25 = "https://pokeapi.co/api/v2/".Length
				request = request[25..];
			}
			using WebClient client = new WebClient();
			//Note: calling the first page "page 1" since we don't know if we have a next one or not
			String? nextPage = null;
			for (int i = 1; i == 1 || nextPage != null; i++)
			{
				JObject page = await FetchResult(request, i, nextPage, client).ConfigureAwait(awaitConfig);
				nextPage = (String?)page["next"];
				yield return page;
			}
		}
		private async Task<JObject> FetchResult(String endpointRequest, int page, String? nextPage,
			WebClient client)
		{
			String filename = JsonFileForRequest(endpointRequest, page);
			if (File.Exists(filename))
			{
				using JsonReader reader = new JsonTextReader(new StreamReader(File.OpenRead(filename)));
				return await JObject.LoadAsync(reader).ConfigureAwait(awaitConfig);
			}
			//not cached, fetch it
			String fetchUri = nextPage ?? String.Concat("https://pokeapi.co/api/v2/", endpointRequest);
			if (!Uri.TryCreate(fetchUri, UriKind.Absolute, out Uri? uri))
			{
				throw new ArgumentException("Invalid Uri", nameof(endpointRequest));
			}
			//don't overwhelm the api server
			await throttle.WaitAsync();
			String json;
			try
			{
				json = await client.DownloadStringTaskAsync(uri).ConfigureAwait(awaitConfig);
			}
			finally
			{
				throttle.Release();
			}
			String parentDir = Path.GetDirectoryName(filename) ?? throw new WasNullException();
			Directory.CreateDirectory(parentDir);
			var task = File.WriteAllTextAsync(filename, json);
			var result = JObject.Parse(json);
			await task.ConfigureAwait(awaitConfig);
			return result;
		}
		private String JsonFileForRequest(String endpointRequest, int page)
		{
			String fileName = String.Concat(endpointRequest.TrimEnd('\\','/'), " page ", page, ".json");
			return Path.Combine(cachePath, fileName);
		}
	}
}
;