using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.Xml;
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
    public class UploadPizzaController : ControllerBase
    {
        private readonly EhrlichContext _context;

        public UploadPizzaController(EhrlichContext context)
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

                                        //items got a new size

                                        List<string> pendingSize = new List<string>();

                                        pendingSize = items.Select(x => x.size).Distinct().ToList();

                                        var existingSize = _context.Sizes
                                            .Select(x => x.Code);

                                        List<string> filteredPendingSizes = pendingSize
                                                        .Where(x => !existingSize.Contains(x)).ToList();

                                        foreach (string size in filteredPendingSizes)
                                        {
                                            Size newSize = MapNewSizeToSqlModel(size);
                                            if (newSize != null) {
                                                _context.Sizes.Add(newSize);
                                            }
                                        }

                                        await _context.SaveChangesAsync();

                                        List<PizzaPrice> pendingItem = new List<PizzaPrice>();
                                        List<Size> availableSizes = new List<Size>();
                                        availableSizes = _context.Sizes.ToList();
                                        List<PizzaType> availableTypes = new List<PizzaType>();
                                        availableTypes = _context.PizzaTypes.ToList();

                                        foreach (PizzaUploadDto item in items)
                                        {
                                            PizzaPrice newData = MapCsvDataToSqlModel(item, availableSizes, availableTypes);
                                            if (newData != null)
                                            {
                                                pendingItem.Add(newData);
                                            }
                                        }

                                        var existingPrice = _context.PizzaPrices
                                            .Where(x => pendingItem.Select(p => p.LongCode).Contains(x.LongCode))
                                            .ToList();

                                        List<PizzaPrice> filterPendingPrice = pendingItem
                                                        .Where(x => !existingPrice.Select(y => y.LongCode).Contains(x.LongCode)).ToList();

                                        foreach (PizzaPrice price in filterPendingPrice)
                                        {
                                            _context.PizzaPrices.Add(price);
                                        }

                                        await _context.SaveChangesAsync();

                                        result.Add("Status", "Success");
                                        result.Add("UploadedPriceRows", filterPendingPrice.Count);
                                        result.Add("UploadedSizeRows", filteredPendingSizes.Count);

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

        private IEnumerable<PizzaUploadDto> ReadCsvFile(Stream fileStream)
        {
            try
            {
                using (var reader = new StreamReader(fileStream))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var records = csv.GetRecords<PizzaUploadDto>();
                    return records.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error reading CSV file", ex);
            }
        }

        private PizzaPrice MapCsvDataToSqlModel(PizzaUploadDto data, List<Size> availableSize, List<PizzaType> availableTypes)
        {
            PizzaPrice price = new PizzaPrice();

            price.TypeId = availableTypes.Where(x => x.Code == data.pizza_type_id).Select(s => s.Id).FirstOrDefault();
            price.LongCode = data.pizza_id;
            price.SizeId = availableSize.Where(x => x.Code == data.size).Select(s => s.Id).FirstOrDefault();

            decimal NewPrice = 0;
            if (decimal.TryParse(data.price, out NewPrice))
            {
                price.Price = NewPrice;
            }
            else
            {
                return null;
            }

            price.CreatedDate = DateTime.Now;

            return price;

        }

        private Size MapNewSizeToSqlModel(string newSize)
        {
            Size size = new Size();
            size.Code = newSize;
            size.CreatedDate = DateTime.Now;

            return size;

        }
    }
}
