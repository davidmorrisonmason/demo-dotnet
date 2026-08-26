namespace Demo.Model.Domain
{
    public interface IAggregateRoot
    {
        /// <summary>
        /// Hook for creation initialisation
        /// </summary>
        void OnCreated();

        /// <summary>
        /// Hook for deletion
        /// </summary>
        void OnDeleted();

    }
}
