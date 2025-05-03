using FitFlare.Core.Entities;
using FitFlare.Infrastructure.Data;
using FitFlare.Infrastructure.Repositories.Interfaces;

namespace FitFlare.Infrastructure.Repositories;

public class FollowRepository(AppDbContext context) : BaseRepository<Follow>(context), IFollowRepository;