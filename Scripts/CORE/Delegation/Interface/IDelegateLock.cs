namespace BCCH
{
    public interface IDelegateLock
    {
        /// <summary>
        /// the greatest weakness of this design, is that one class implementing this interface, can only attach to one delegation body
        /// we need to figureout how to constrain multiple delegation while making their deregistration seperate
        /// </summary>
        public void OnDelegateAttach();

        public void OnDelegateDetach();

        public void DelegateAttach();

        public void DelegateDetach();

        public bool isDelegateAttached { get; set; }
    }
}