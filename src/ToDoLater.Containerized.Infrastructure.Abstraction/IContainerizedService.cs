namespace ToDoLater.Containerized.Infrastructure.Abstraction
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using ToDoLater.Containerized.Infrastructure.Models;

    public interface IContainerizedService : IAsyncDisposable
    {
        /// <summary>
        /// Initializes the containerized asynchronous.
        /// </summary>
        /// <param name="networkId">The network id of the infrastructure.</param>
        /// <param name="domainName">The domain name of infrastructure.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task InitializeAsync(string networkId, string identifier, string domainName, CancellationToken cancellationToken);

        /// <summary>
        /// Builds the container asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task BuildAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Prepares the container asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<bool> PrepareAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Determines whether [is ready asynchronous] [the specified cancellation token].
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<bool> IsReadyAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Gets the state of the container initialization.
        /// </summary>
        /// <value>
        /// The state of the container initialization.
        /// </value>
        public ServiceState State { get; }
    }
}
