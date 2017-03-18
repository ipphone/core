using System;
using System.Collections.Generic;
using System.Text;
using ContactPoint.Common;

namespace ContactPoint.Plugins.HotKeys.Actions
{
    public class UserAction
    {
        public virtual string Name { get; set; }

        public virtual Action<ICore> Command { get; set; }

        public UserAction()
        {
            Name = "";
            Command = x => { };
        }
    }
}
