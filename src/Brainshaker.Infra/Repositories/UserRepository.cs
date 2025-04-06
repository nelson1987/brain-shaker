using Brainshaker.Domain.Entities;
using Brainshaker.Infra.Database;

namespace Brainshaker.Infra.Repositories;

public class UserRepository(DatabaseContext context) : AbstractRepository<Usuario>(context)
{
}