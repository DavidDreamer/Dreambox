using System.Runtime.InteropServices;
using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace YandexGamesSDK
{
	public partial class Yandex : MonoBehaviour
	{
		[DllImport("__Internal")]
		public static extern void SavePlayerData(string data);

		[DllImport("__Internal")]
		public static extern void LoadPlayerData();

		[DllImport("__Internal")]
		public static extern void UpdateLeaderboard(string name, int value);

		[DllImport("__Internal")]
		public static extern void RateGame();

		private UniTaskCompletionSource<string> CompletionSource { get; set; }

		public async UniTask<string> LoadPlayerDataAsync(CancellationToken token)
		{
			CompletionSource = new UniTaskCompletionSource<string>();
			token.Register(() => CompletionSource.TrySetCanceled());
			LoadPlayerData();
			return await CompletionSource.Task;
		}

		[UsedImplicitly]
		public void SetPlayerData(string data) => CompletionSource.TrySetResult(data);
	}
}
