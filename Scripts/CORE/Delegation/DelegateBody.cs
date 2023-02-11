using System.Collections.Generic;

namespace BCCH
{
    public class DelegateBody
    {
        /// <summary>
        /// this class is for restricting delegation attachment, 
        /// whenever there is another body trying to attach a new method to a delegation,
        /// it have to pass into a delegate body for registration.
        /// As such, we can safly record attached bodies, and when we are going to destroy our delegate body, 
        /// we can safly inform all binding bodies to remove themself from this.
        /// 
        /// Thus, we need a HashSet to record objects that implements IDelegateLock interface,
        /// and need to provide a method to remove all biding delegates by traversing the hash set;
        /// </summary>
        HashSet<IDelegateLock> delegateLocks = new HashSet<IDelegateLock>();

        public bool RegisteringDelegation(IDelegateLock input)
        {
            //Debug.Log("On Registration");
            if (!delegateLocks.Contains(input))
            {
                delegateLocks.Add(input);
                input.isDelegateAttached = true;
                //Debug.Log("Rceiving delegation registration");
                return true;
            }
            else
            {
                //Debug.LogError("DELEGATE ATTACHMENT ERROR");
                return false;
            }
        }

        public bool RemoveDelegation(IDelegateLock input)
        {
            if (delegateLocks.Contains(input))
            {

                delegateLocks.Remove(input);
                //Debug.Log("Detaching delegation registration");
                return true;
            }
            else
            {
                //Debug.LogError("DELEGATE DETACHMENT ERROR");
                return false;
            }
        }

        public void RemoveAllDelegations()
        {
            foreach (IDelegateLock var in delegateLocks)
            {
                if (var != null && var.isDelegateAttached)
                {
                    var.OnDelegateDetach();
                    var.isDelegateAttached = false;
                }
            }
            delegateLocks.Clear();
        }
    }
}