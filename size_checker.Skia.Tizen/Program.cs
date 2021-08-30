using Tizen.Applications;
using Uno.UI.Runtime.Skia;

namespace size_checker.Skia.Tizen
{
	class Program
{
	static void Main(string[] args)
	{
		var host = new TizenHost(() => new size_checker.App(), args);
		host.Run();
	}
}
}
