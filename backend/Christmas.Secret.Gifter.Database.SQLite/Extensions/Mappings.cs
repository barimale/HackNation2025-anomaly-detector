using AutoMapper;
using MSSql.Domain;
using MSSql.Infrastructure.Entities;

namespace MSSql.Infrastructure.Extensions {
    public class Mappings : Profile
    {
        public Mappings()
        {
            CreateMap<AlgorithmResult, AlgorithmResultEntry>()
                .ReverseMap();

            CreateMap<AlgorithmSummary, AlgorithmSummaryEntry>()
                .ReverseMap();

            CreateMap<Ranking, RankingEntry>();
            CreateMap<RankingEntry, Ranking>();
        }
    }
}
