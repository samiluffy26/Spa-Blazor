using Microsoft.AspNetCore.Mvc;
using spa_reservas_blazor.Application.Interfaces;
using spa_reservas_blazor.Shared.Entities;

namespace spa_reservas_blazor.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ISettingRepository _settingRepository;
    private readonly IWebHostEnvironment _env;

    public AdminController(ICategoryRepository categoryRepository, ISettingRepository settingRepository, IWebHostEnvironment env)
    {
        _categoryRepository = categoryRepository;
        _settingRepository = settingRepository;
        _env = env;
    }

    // CATEGORIES
    [HttpGet("categories")]
    public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
    {
        return Ok(await _categoryRepository.GetAllAsync());
    }

    [HttpGet("categories/{id}")]
    public async Task<ActionResult<Category>> GetCategory(string id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null) return NotFound();
        return Ok(category);
    }

    [HttpPost("categories")]
    public async Task<ActionResult> CreateCategory(Category category)
    {
        await _categoryRepository.CreateAsync(category);
        return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
    }

    [HttpPut("categories/{id}")]
    public async Task<ActionResult> UpdateCategory(string id, Category category)
    {
        if (id != category.Id) return BadRequest();
        await _categoryRepository.UpdateAsync(category);
        return NoContent();
    }

    [HttpDelete("categories/{id}")]
    public async Task<ActionResult> DeleteCategory(string id)
    {
        await _categoryRepository.DeleteAsync(id);
        return NoContent();
    }

    // SETTINGS
    [HttpGet("settings")]
    public async Task<ActionResult<AppSettings>> GetSettings()
    {
        return Ok(await _settingRepository.GetSettingsAsync());
    }

    [HttpPost("settings")]
    public async Task<ActionResult> UpdateSettings(AppSettings settings)
    {
        await _settingRepository.UpdateSettingsAsync(settings);
        return NoContent();
    }

    // UPLOAD
    [HttpPost("upload")]
    public async Task<ActionResult<string>> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var url = $"/uploads/{uniqueFileName}";
        return Ok(new { url });
    }
}
