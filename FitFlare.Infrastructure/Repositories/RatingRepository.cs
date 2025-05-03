using FitFlare.Core.Entities;
using FitFlare.Infrastructure.Data;
using FitFlare.Infrastructure.Repositories.Interfaces;

namespace FitFlare.Infrastructure.Repositories;

public class RatingRepository(AppDbContext context) : BaseRepository<Rating>(context), IRatingRepository;