using Microsoft.AspNetCore.Mvc;

namespace User_API.Controllers
{
    public class VerifyAPIController : BaseController
    {
        [HttpGet]
        public IActionResult GetActionResult()
        {
            try
            {
                Random random = new();
                return Ok(random.Next());
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message + "~ API Deails :- VerifyAPIController/GetActionResult");
                return BadRequest(ex.Message);
            }
        }
    }
}