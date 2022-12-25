using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using User_Database;
using User_Database.Domain;
using User_Infrastructure.Interface;

namespace User_Command.Organizations_cmd.GetAll
{
    public class GetAll : IRequest<Response>
    {
        public class GetAllHandler : IRequestHandler<GetAll, Response>
        {
            private readonly IMapper mapper;
            private readonly IDapperRepository dapper;
            private readonly IGenericRepository<Organizations> repository;

            public GetAllHandler(IMapper mapper, IDapperRepository dapper, IGenericRepository<Organizations> repository)
            {
                this.mapper = mapper;
                this.dapper = dapper;
                this.repository = repository;
            }

            public async Task<Response> Handle(GetAll request, CancellationToken cancellationToken)
            {
                List<GetallDTO> getalls = new();
                var data = await dapper.GetDataListAsync<GetallDTO>("select * from Organizations", null, System.Data.CommandType.Text, APISetting.UserDBConnection);
                getalls = data.ToList();

                Response response = new()
                {
                    ResponseObject = getalls
                };

                return response;
            }
        }
    }
}