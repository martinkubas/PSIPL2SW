using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PacketDotNet;


namespace Projekt
{
    public class ACL
    {
        public List<ACE> acl = new List<ACE>();

        public void addACE(ACE ace)
        {
            acl.Add(ace);
        }
        public void removeACE(int index)
        {
            if (index >= 0 && index < acl.Count)
            {
                acl.RemoveAt(index);
            }
        }
        public void clearAllACEs()
        {
            acl.Clear(); 

        }
        public bool allowPacket(Packet packet)
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
            for (int i = 0; i < acl.Count; i++)
            {
                sb.AppendLine($"[{i + 1}] {acl[i].ToString()}");
            }
            return sb.ToString();
        }



    }
}
