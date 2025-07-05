using Core.Contracts;
using Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MedicalAssessmentsWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExaminationsController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        public ExaminationsController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpGet]
        public async Task<ActionResult> GetExaminationsAsync(bool withoutFindingsOnly = false)
        {
            await Task.CompletedTask;
            throw new NotImplementedException();
        }


        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetAsync(int id)
        {
            await Task.CompletedTask;
            throw new NotImplementedException();
        }

        public record UpdateExaminationDto(int Id, string Findings, int DoctorId);

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> UpdateExaminationAsync(int id, [FromBody] UpdateExaminationDto updateExaminationDto)
        {
            await Task.CompletedTask;
            throw new NotImplementedException();
        }

    }
}
