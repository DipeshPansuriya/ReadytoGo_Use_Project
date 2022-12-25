using AutoMapper;
using MediatR;
using User_Database;
using User_Database.Domain;
using User_Infrastructure.Interface;

namespace User_Command.Organizations_cmd.Insert
{
    public class Insert : IRequest<Response>
    {
        public OrganizationsInsertDTO? insertdata { get; set; }

        public class InsertHandler : IRequestHandler<Insert, Response>
        {
            private readonly IMapper mapper;
            private readonly IDapperRepository dapper;
            private readonly IGenericRepository<Organizations> repository;

            public InsertHandler(IMapper mapper, IDapperRepository dapper, IGenericRepository<Organizations> repository)
            {
                this.mapper = mapper;
                this.dapper = dapper;
                this.repository = repository;
            }

            public async Task<Response> Handle(Insert request, CancellationToken cancellationToken)
            {
                Organizations ent = mapper.Map<Organizations>(request.insertdata);
                await repository.Add(ent);
                await repository.SaveAsync();
                Response response = new()
                {
                    ResponseObject = ent
                };
                return response;
            }
        }
    }
}