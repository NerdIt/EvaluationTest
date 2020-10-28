using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluationTest
{
	class BracketPacket
	{
		public int characterIndex;
		public int pairIndex;
		public bool solved = false;
		public int depth;


		public BracketPacket(int c, bool u = false)
		{
			characterIndex = c;
			solved = u;
		}

	}
}
