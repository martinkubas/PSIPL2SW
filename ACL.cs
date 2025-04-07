using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt
{
    internal class ACL
    {
        public List<ACE> acl = new List<ACE>();

        public void addACE(ACE ace)
        {
            //insert so its index 0
        }
        public void removeACE(int index)
        {
            //remove from list on index 
        }
        public bool allowPacket()
        {
            foreach (ACE ace in acl)
            {
                //check if allow or deny
            }
            return false;
        }


    }
}
