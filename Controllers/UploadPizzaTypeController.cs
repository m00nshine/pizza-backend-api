using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pizza_backend_api.DataTransferObjects;
using pizza_backend_api.Models;

namespace pizza_backend_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadPizzaTypeController : ControllerBase
    {
        private readonly EhrlichContext _context;

        public UploadPizzaTypeController(EhrlichContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult> Upload(IFormFile file)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();

            if (Request.Form.Files.Count > 0)
            {
                if (Request.Form.Files.Count < 2)
                {
                    try
                    {
                        if (file != null && file.Length > 0)
                        {
                            if (file.Length < 2000000)
                            {
                                using (var stream = file.OpenReadStream())
                                {
                                    try
                                    {
                                        var items = ReadCsvFile(stream).ToList();

                                        //items got a new category
                                        List<string> pendingCategories = new List<string>();

                                        pendingCategories = items.Select(x => x.category).Distinct().ToList();

                                        var existingCategory = _context.Categories
                                            .Select(x => x.Name);

                                        List<string> filteredPendingCategories = pendingCategories
                                                        .Where(x => !existingCategory.Contains(x)).ToList();

                                        foreach (string category in filteredPendingCategories)
                                        {
                                            Category newCategory = MapNewCategoryToSqlModel(category);
                                            if (newCategory != null)
                                            {
                                                _context.Categories.Add(newCategory);
                                            }
                                        }

                                        await _context.SaveChangesAsync();

                                        List<PizzaType> pendingType = new List<PizzaType>();

                                        List<Category> availableCategories = new List<Category>();
                                        availableCategories = _context.Categories.ToList();

                                        foreach (PizzaTypeUploadDto item in items)
                                        {
                                            PizzaType newData = MapCsvDataToSqlModel(item, availableCategories);
                                            if (newData != null)
                                            {
                                                pendingType.Add(newData);
                                            }
                                        }

                                        var existingType = _context.PizzaTypes
                                            .Where(x => pendingType.Select(p => p.Code).Contains(x.Code))
                                            .ToList();

                                        List<PizzaType> filterPendingTypes = pendingType
                                                        .Where(x => !existingType.Select(y => y.Code).Contains(x.Code)).ToList();

                                        foreach (PizzaType pizzaType in filterPendingTypes)
                                        {
                                            _context.PizzaTypes.Add(pizzaType);
                                        }

                                        await _context.SaveChangesAsync();

                                        result.Add("Status", "Success");
                                        result.Add("UploadedTypeRows", filterPendingTypes.Count);
                                        result.Add("UploadedCategoryRows", filteredPendingCategories.Count);

                                        return Ok(result);
                                    }
                                    catch (Exception ex)
                                    {
                                        result.Add("Status", "Failed");
                                        result.Add("Error", "Internal server error");
                                        return Ok(result);
                                    }
                                }
                            }
                            else
                            {
                                result.Add("Status", "Failed");
                                result.Add("Error", "File size exceeded the limit of 2Kb.");
                                return Ok(result);
                            }
                        }
                        else
                        {
                            result.Add("Status", "Failed");
                            result.Add("Error", "Please select a valid CSV file.");
                            return Ok(result);
                        }
                    }
                    catch
                    {
                        result.Add("Status", "Failed");
                        result.Add("Error", "Internal server error");
                        return Ok(result);
                    }
                }
                else
                {
                    result.Add("Status", "Failed");
                    result.Add("Error", "No files has been uploaded, please upload one at a time");
                    return Ok(result);
                }
            }
            else
            {
                result.Add("Status", "Failed");
                result.Add("Error", "No file uploaded");
                return Ok(result);
            }
        }

        private bool TransactionExists(int OrderId)
        {
            return _context.Transactions.Any(e => e.Id == OrderId);
        }

        private IEnumerable<PizzaTypeUploadDto> ReadCsvFile(Stream fileStream)
        {
            try
            {
                using (var reader = new StreamReader(fileStream))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var records = csv.GetRecords<PizzaTypeUploadDto>();
                    return records.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error reading CSV file", ex);
            }
        }

        private PizzaType MapCsvDataToSqlModel(PizzaTypeUploadDto data, List<Category> availableCategories)
        {
            PizzaType pizzaType = new PizzaType();


            pizzaType.Code = data.pizza_type_id;
            pizzaType.Name = data.name;
            pizzaType.CategoryId = availableCategories.Where(x => x.Name == data.category).Select(s => s.Id).FirstOrDefault();
            pizzaType.Description = data.ingredients;
            pizzaType.CreatedDate = DateTime.Now;

            return pizzaType;

        }

        private Category MapNewCategoryToSqlModel(string newCategory)
        {
            Category category = new Category();
            category.Name = newCategory;
            category.CreatedDate = DateTime.Now;

            return category;
        }
    }
}
