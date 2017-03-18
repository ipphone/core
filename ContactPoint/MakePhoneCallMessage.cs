using System.Collections.Generic;
using System.Linq;

namespace ContactPoint
{
    public class MakePhoneCallMessage
    {
        public List<KeyValuePair<string, string>> Attributes { get; set; } = new List<KeyValuePair<string, string>>();
        public string Destination { get; set; }

        public static MakePhoneCallMessage CreateFromCommandLine(string cmd)
        {
            if (string.IsNullOrEmpty(cmd))
            {
                return null;
            }

            var parts = cmd.Split('&');
            return new MakePhoneCallMessage()
            {
                Destination = parts[0],
                Attributes = parts.Length > 1
                    ? parts.Skip(1)
                        .Select(x => x.Split(new[] {'='}, 2))
                        .Select(x => new KeyValuePair<string, string>(x[0], x.Length > 1 ? x[1] : null))
                        .ToList()
                    : new List<KeyValuePair<string, string>>()
            };
        }
    }
}
