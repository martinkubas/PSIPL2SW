using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt
{
    public class ACL
    {
        public List<ACE> acl = new List<ACE>();

        public void addACE(ACE ace)
        {
            acl.Insert(0, ace);
        }
        public void removeACE(int index)
        {
            if (index >= 0 && index < acl.Count)
            {
                acl.RemoveAt(index);
            }
        }
        public bool allowPacket()
        {
            foreach (ACE ace in acl)
            {
                //check if allow or deny
            }
            return false;
        }

        public override string ToString()
        {
            if (acl.Count == 0)
                return "No rules in ACL";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"ACL contains {acl.Count} rules:");
            foreach (var rule in acl)
            {
                sb.AppendLine(rule.ToString());  
            }
            return sb.ToString();
        }


    }
}
