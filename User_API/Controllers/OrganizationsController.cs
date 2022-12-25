using Microsoft.AspNetCore.Mvc;
using User_Command.Organizations_cmd.GetAll;
using User_Command.Organizations_cmd.Insert;
using User_Command.Organizations_cmd.Update;
using User_Database.Domain;

namespace User_API.Controllers
{
    public class OrganizationsController : BaseController
    {
        [HttpPost]
        public async Task<ActionResult> Insert([FromBody] Insert data)
        {
            try
            {
                Response response = await Mediator.Send(data).ConfigureAwait(true);
                return Ok(response);
            }
            catch (Exception ex)
            {
                string contname = typeof(OrganizationsController).Name;
                string actionname = "Insert";
                Logger.Error(ex.Message + "~ API Deails :- " + contname + "/" + actionname + "");
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult> Update([FromBody] Update data)
        {
            try
            {
                Response response = await Mediator.Send(data).ConfigureAwait(true);
                return Ok(response);
            }
            catch (Exception ex)
            {
                string contname = typeof(OrganizationsController).Name;
                string actionname = "update";
                Logger.Error(ex.Message + "~ API Deails :- " + contname + "/" + actionname + "");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                Response response = await Mediator.Send(new GetAll() { }).ConfigureAwait(true);
                return Ok(response);
            }
            catch (Exception ex)
            {
                string contname = typeof(OrganizationsController).Name;
                string actionname = "getall";
                Logger.Error(ex.Message + "~ API Deails :- " + contname + "/" + actionname + "");
                return BadRequest(ex.Message);
            }
        }
    }
}