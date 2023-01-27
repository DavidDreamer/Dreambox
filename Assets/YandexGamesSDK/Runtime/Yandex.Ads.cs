using System.Runtime.InteropServices;
using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;

namespace YandexGamesSDK
{
	public partial class Yandex
	{
		[DllImport("__Internal")]
		public static extern void ShowFullscreenAdv();

		private UniTaskCompletionSource AdvCompletionSource { get; set; }
		
		public async UniTask ShowFullscreenAdvAsync(CancellationToken token)
		{
			AdvCompletionSource = new UniTaskCompletionSource();
			token.Register(() => AdvCompletionSource.TrySetCanceled());
			ShowFullscreenAdv();
			await AdvCompletionSource.Task;
		}

		[UsedImplicitly]
		public void OnFullscrenAdvClosed() => AdvCompletionSource.TrySetResult();
		
		[UsedImplicitly]
		public void OnFullscrenAdvError() => AdvCompletionSource.TrySetResult();
	}
}
