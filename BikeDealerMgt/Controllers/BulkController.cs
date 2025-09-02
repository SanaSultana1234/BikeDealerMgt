using Microsoft.AspNetCore.Mvc;
using BikeDealerMgtAPI.Models;
using System.ComponentModel;
using OfficeOpenXml;
using Microsoft.EntityFrameworkCore;


namespace BikeDealerMgtAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class BulkController : ControllerBase
	{
		private readonly BikeDealerMgmtDbContext _context;
		private readonly IWebHostEnvironment _env;

		public BulkController(BikeDealerMgmtDbContext context, IWebHostEnvironment env)
		{
			_context = context;
			_env = env;
		}

		//BULK INSERT
		[HttpPost("AddBikes")]
		public async Task<IActionResult> AddBikes(IFormFile file)
		{
			if (file == null || file.Length == 0)
				return BadRequest("File is empty");

			var allowedExtensions = new[] { ".xlsx", ".xls" };
			var extension = Path.GetExtension(file.FileName).ToLower();

			if (!allowedExtensions.Contains(extension))
			{
				return BadRequest("Invalid file type. Please upload an Excel file (.xlsx or .xls).");
			}


			// Store file in Uploads folder
			var uploadsFolder = Path.Combine(_env.ContentRootPath, "Uploads");
			if (!Directory.Exists(uploadsFolder))
				Directory.CreateDirectory(uploadsFolder);

			var filePath = Path.Combine(uploadsFolder, file.FileName);

			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await file.CopyToAsync(stream);
			}


			var bikeStores = new List<BikeStore>();
			var errors = new List<string>();
			ExcelPackage.License.SetNonCommercialPersonal("Sana Sultana");

			using (var package = new ExcelPackage(new FileInfo(filePath)))
			{
				var worksheet = package.Workbook.Worksheets[0];
				int rowCount = worksheet.Dimension.Rows;
				int colCount = worksheet.Dimension.Columns;

				//Validate column headers
				var expectedHeaders = new List<string> { "BikeId", "ModelName", "ModelYear", "EngineCc", "Manufacturer" };
				for (int i = 0; i < expectedHeaders.Count; i++)
				{
					if (worksheet.Cells[1, i + 1].Text.Trim() != expectedHeaders[i])
					{
						return BadRequest($"Invalid column format. Expected '{expectedHeaders[i]}' at position {i + 1}");
					}
				}

				// Process rows
				for (int row = 2; row <= rowCount; row++)
				{
					try
					{
						var bike = new BikeStore
						{
							//BikeId = int.Parse(worksheet.Cells[row, 1].Text),
							ModelName = worksheet.Cells[row, 2].Text,
							ModelYear = string.IsNullOrEmpty(worksheet.Cells[row, 3].Text) ? null : int.Parse(worksheet.Cells[row, 3].Text),
							EngineCc = string.IsNullOrEmpty(worksheet.Cells[row, 4].Text) ? null : int.Parse(worksheet.Cells[row, 4].Text),
							Manufacturer = worksheet.Cells[row, 5].Text
						};

						// Validate mandatory fields
						if (string.IsNullOrWhiteSpace(bike.ModelName))
						{
							errors.Add($"Row {row}: ModelName is required");
							continue;
						}

						bikeStores.Add(bike);
					}
					catch (Exception ex)
					{
						errors.Add($"Row {row}: {ex.Message}");
					}
				}
			}

			if (errors.Any())
				return BadRequest(new { message = "Validation failed", errors });

			// Bulk Insert into DB
			await _context.BikeStores.AddRangeAsync(bikeStores);
			await _context.SaveChangesAsync();

			return Ok(new { message = "Data inserted successfully", total = bikeStores.Count });
		}


		//BULK DELETE
		[HttpPost("DeleteBikes")]
		public async Task<IActionResult> BulkDelete([FromBody] List<int> bikeIds)
		{
			if (bikeIds == null || bikeIds.Count == 0)
			{
				return BadRequest("No Bike IDs provided.");
			}

			// Fetch bikes that match the given IDs
			var bikesToDelete = await _context.BikeStores
				.Where(b => bikeIds.Contains(b.BikeId))
				.ToListAsync();

			if (bikesToDelete.Count == 0)
			{
				return NotFound("No matching bikes found to delete.");
			}

			_context.BikeStores.RemoveRange(bikesToDelete);
			await _context.SaveChangesAsync();

			return Ok(new
			{
				Message = $"{bikesToDelete.Count} bikes deleted successfully."
			});
		}

	}
}
