using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ContactPoint.Common.Contacts;

namespace ContactPoint.Plugins.LocalAddressBook.Contracts
{
    [DataContract]
    internal class ContactTag : Versionable, IContactTag
    {
        [DataMember]
        public string Key { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Color { get; set; }

        public ContactTag()
        {
            Key = Guid.NewGuid().ToString("D");
        }

        private static ContactTag CreateFromContactTag(IContactTag tag)
        {
            var result = new ContactTag()
                {
                    Color = tag.Color,
                    Key = tag.Key,
                    Name = tag.Name
                };

            result.SetVersionKey(tag.VersionKey);

            return result;
        }

        public static ContactTag GetLocalObject(IContactTag tag)
        {
            var result = tag as ContactTag;
            return result ?? CreateFromContactTag(tag);
        }
    }
}
