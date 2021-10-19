namespace ToDoLater.Containerized.Infrastructure.Abstraction
{
    using System.Threading;
    using System.Threading.Tasks;
    using ToDoLater.Containerized.Infrastructure.Models;

    public abstract class ContainerizedService : IContainerizedService
    {
        protected string ContainerId { get; private set; }

        public ServiceState State { get; protected set; }

        public abstract Task InitializeAsync(string networkId, string identifier, string domainName, CancellationToken cancellationToken);

        public virtual Task BuildAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public virtual Task<bool> IsReadyAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public virtual Task<bool> PrepareAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public abstract ValueTask DisposeAsync();
    }
}
