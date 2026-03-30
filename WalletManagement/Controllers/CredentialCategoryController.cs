using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WalletManagement.Core.Domain.Services;
using WalletManagement.Core.Domain.Services.Communication;
using WalletManagement.Core.DTOs;

namespace WalletManagement.Controllers
{
    [Route("api/[controller]")]
    public class CredentialCategoryController : BaseApiController
    {
        private readonly ICategoryService _categoryService;

        public CredentialCategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var ViewModel = new List<CategoryViewModel>();

            var categoryList = await _categoryService.ListCategoryAsync();

            if (categoryList == null)
            {
                return Ok(new APIResponse()
                {
                    Success = false,
                    Message = "Category List Empty",
                    Result = new { }
                });
            }

            foreach (var category in categoryList)
            {
                var CategoryViewModel = new CategoryViewModel
                {
                    Id = category.Id,
                    Name = category.Name,
                    CreatedDate = category.CreatedDate,
                    Description = category.Description,
                    Status = category.Status,
                };
                ViewModel.Add(CategoryViewModel);
            }

            ViewModel = ViewModel
                   .OrderByDescending(x => x.CreatedDate)
                   .ToList();

            return Ok(new APIResponse()
            {
                Success = true,
                Message = "Successfully fetched Category",
                Result = ViewModel
            });
        }

        [HttpPost("save")]
        [Consumes("application/json")]
        public async Task<IActionResult> Save([FromBody][Required] CategoryViewModel ViewModel)
        {
            var category = new Core.Domain.Models.Category()
            {
                Name = ViewModel.Name,
                Description = ViewModel.Description,
                CreatedBy = "SYSTEM",
                ModifiedBy = "SYSTEM",
                CategoryUid = Guid.NewGuid().ToString(),
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
            };

            var response = await _categoryService.CreateCategoryAsync(category);

            return Ok(new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Result
            });
        }

        [HttpPost("update")]
        [Consumes("application/json")]
        public async Task<IActionResult> Update([FromBody][Required] CategoryEditViewModel ViewModel)
        {
            var categoryinDb = await _categoryService.GetCategoryAsync(ViewModel.Id);
            if (categoryinDb == null)
            {
                return Ok(new APIResponse()
                {
                    Success = false,
                    Message = "Category not found",
                    Result = new { }
                });
            }

            categoryinDb.Id = ViewModel.Id;
            categoryinDb.Name = ViewModel.Name;
            categoryinDb.Description = ViewModel.Description;
            categoryinDb.ModifiedBy = "System";
            categoryinDb.ModifiedDate = DateTime.UtcNow;

            var response = await _categoryService.UpdateCategoryAsync(categoryinDb);

            return Ok(new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Result
            });
        }

        [HttpGet("GetCategoryDetailsById/{id:int:min(1)}")]
        public async Task<IActionResult> GetCategoryDetailsById(int id)
        {
            var response = await _categoryService.GetCategorybyIdAsync(id);
            return Ok(new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            });
        }
    }
}