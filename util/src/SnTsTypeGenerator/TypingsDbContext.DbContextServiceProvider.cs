namespace SnTsTypeGenerator;

public partial class TypingsDbContext
{
    internal sealed class DbContextServiceProvider : IServiceProvider
    {
        private readonly object _entity;
        private readonly TypingsDbContext _dbContext;
        private readonly IServiceProvider _backingServiceProvider;

        internal DbContextServiceProvider(TypingsDbContext dbContext, IServiceProvider backingServiceProvider, object entity)
        {
            _dbContext = dbContext;
            _entity = entity;
            _backingServiceProvider = backingServiceProvider;
        }

        public object? GetService(Type serviceType)
        {
            if (serviceType is null)
                return null;
            if (serviceType.IsInstanceOfType(_entity))
                return _entity;
            if (serviceType.IsInstanceOfType(_dbContext._logger))
                return _dbContext._logger;
            return _backingServiceProvider.GetService(serviceType);
        }
    }
}
