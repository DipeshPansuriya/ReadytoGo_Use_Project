using AutoMapper;
using MediatR;
using User_Database;
using User_Database.Domain;
using User_Infrastructure.Interface;

namespace User_Command.Organizations_cmd.Update
{
    public class Update : IRequest<Response>
    {
        public OrganizationsUpdateDTO? updatedata { get; set; }

        public class UpdateHandler : IRequestHandler<Update, Response>
        {
            private readonly IMapper mapper;
            private readonly IDapperRepository dapper;
            private readonly IGenericRepository<Organizations> repository;

            public UpdateHandler(IMapper mapper, IDapperRepository dapper, IGenericRepository<Organizations> repository)
            {
                this.mapper = mapper;
                this.dapper = dapper;
                this.repository = repository;
            }

            public async Task<Response> Handle(Update request, CancellationToken cancellationToken)
            {
                Organizations ent = mapper.Map<Organizations>(request.updatedata);
                //await repository.GetById(request.updatedata.Id);
                repository.Update(ent);
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