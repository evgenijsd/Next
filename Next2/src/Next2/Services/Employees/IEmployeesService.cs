using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using Next2.Models.API.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Next2.Services.Employees
{
    public interface IEmployeesService
    {
        Task<AOResult<IEnumerable<EmployeeModelDTO>>> GetEmployeesAsync();
    }
}
