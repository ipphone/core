using System.Collections.Generic;

#pragma warning disable 1591

namespace ExceptionReporter.SystemInfo
{
	/// <summary>
	/// SysInfoResult holds results from a (ultimately WMI) query into system information
	/// </summary>
	public class SysInfoResult
    {
        private readonly string _name;
        private readonly List<string> _nodes = new List<string>();
        private readonly List<SysInfoResult> _childResults = new List<SysInfoResult>();

        public SysInfoResult(string name)
        {
            _name = name;
        }

		public void AddNode(string node)
		{
			_nodes.Add(node);
		}

		public void AddChildren(IEnumerable<SysInfoResult> children)
		{
			ChildResults.AddRange(children);
		}

        public List<string> Nodes
        {
        	get { return _nodes; }
        }

		private void Clear()
		{
			_nodes.Clear();
		}

		private void AddRange(IEnumerable<string> nodes)
		{
			_nodes.AddRange(nodes);
		}

		public string Name
        {
            get { return _name; }
        }

        public List<SysInfoResult> ChildResults
        {
            get { return _childResults; }
        }

		public SysInfoResult Filter(string[] filterStrings)
		{
            List<string> filteredNodes = new List<string>();

            foreach (string node in ChildResults[0].Nodes)
                foreach (string filter in filterStrings)
                    if (node.Contains(filter + " = "))
                        filteredNodes.Add(node);

			ChildResults[0].Clear();
			ChildResults[0].AddRange(filteredNodes);
			return this;
		}
    }
}
#pragma warning restore 1591
