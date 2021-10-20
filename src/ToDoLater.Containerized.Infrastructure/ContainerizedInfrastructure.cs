namespace ToDoLater.Containerized.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using ToDoLater.Containerized.Infrastructure.Abstraction;

    public abstract class ContainerizedInfrastructure : IContainerizedInfrastructure
    {
        protected readonly ICollection<IContainerizedService> containerizedServices;

        protected ContainerizedInfrastructure(ICollection<IContainerizedService> containerizedServices)
        {
            this.Identifier = Guid.NewGuid().ToString();
            this.containerizedServices = containerizedServices;
        }

        public string Identifier { get; protected set; }

        public abstract Task InitializeAsync(CancellationToken cancellationToken);

        public virtual async ValueTask DisposeAsync()
        {
            if (containerizedServices != null && containerizedServices.Any())
            {
                foreach (var containerizedService in containerizedServices)
                {
                    try
                    {
                        await containerizedService.DisposeAsync().ConfigureAwait(false);
                    }
                    catch (Exception)
                    {
                       // todo: these need to be logged, we need to choose an "independent" logger
                    }
                }
            }

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
            GC.SuppressFinalize(this);
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
        }
    }
}
