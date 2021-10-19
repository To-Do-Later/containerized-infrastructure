namespace ToDoLater.Containerized.Infrastructure.Abstraction
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IContainerizedInfrastructure : IAsyncDisposable
    {
        /// <summary>
        /// Gets the infrastructure identifier.
        /// </summary>
        /// <value>
        /// The infrastructure identifier.
        /// </value>
        string Identifier { get; }

        /// <summary>
        /// Initializes the infrastructure asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task InitializeAsync(CancellationToken cancellationToken);
    }
}
