using System.Runtime.InteropServices;

namespace YandexGamesSDK
{
	public static class Environment
	{
		[DllImport("__Internal")]
		public static extern string GetLanguage();
	}
}
