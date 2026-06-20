using System.Collections.Generic;

namespace Dreambox.Rendering.Core
{
    public interface IOutlinePass
	{
		HashSet<OutlineRenderer> Targets { get; }
	}
}
