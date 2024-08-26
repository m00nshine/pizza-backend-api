using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper.TypeConversion;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using pizza_backend_api.Models;
using pizza_backend_api.DataTransferObjects;
using System.Collections;

namespace pizza_backend_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadTransactionDetailController : ControllerBase
    {
        private readonly EhrlichContext _context;

        public UploadTransactionDetailController(
            EhrlichContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Upload transaction details
        /// </summary>
        /// <param name="csv file"></param>
        /// <returns>
        ///     number of uploaded transaction details
        /// </returns>
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
                                        var orders = ReadCsvFile(stream).ToList();

                                        List<TransactionDetail> pendingTransactionDetail = new List<TransactionDetail>();

                                        foreach (TransactionDetailUploadDto order in orders)
                                        {
                                            TransactionDetail newData = MapCsvDataToSqlModel(order);
                                            if (newData != null)
                                            {
                                                pendingTransactionDetail.Add(newData);
                                            }
                                        }

                                        var existingOrders = _context.TransactionDetails
                                            .Where(x => pendingTransactionDetail.Select(p => p.OrderId).Contains(x.OrderId))
                                            .ToList();

                                        List<TransactionDetail> filterPendingTransaction = pendingTransactionDetail
                                                        .Where(x => !existingOrders.Select(y => y.OrderId).Contains(x.OrderId)).ToList();

                                        foreach (TransactionDetail transaction in filterPendingTransaction)
                                        {
                                            _context.TransactionDetails.Add(transaction);
                                        }

                                        await _context.SaveChangesAsync();

                                        result.Add("Status", "Success");
                                        result.Add("UploadedRows", filterPendingTransaction.Count);

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

        private IEnumerable<TransactionDetailUploadDto> ReadCsvFile(Stream fileStream)
        {
            try
            {
                using (var reader = new StreamReader(fileStream))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var records = csv.GetRecords<TransactionDetailUploadDto>();
                    return records.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error reading CSV file", ex);
            }
        }

        private TransactionDetail MapCsvDataToSqlModel(TransactionDetailUploadDto data) {
            TransactionDetail transactiondetail = new TransactionDetail();

            int NewOrderId = 0;
            if (int.TryParse(data.order_id, out NewOrderId))
            {
                transactiondetail.OrderId = NewOrderId;
            }
            else {
                return null;
            }

            transactiondetail.PizzaLongCode = data.pizza_id;

            int NewQuantity = 0;
            if (int.TryParse(data.quantity, out NewQuantity))
            {
                transactiondetail.Quantity = NewQuantity;
            }
            else
            {
                return null;
            }

            transactiondetail.CreatedDate = DateTime.Now;

            return transactiondetail;

        }
    }
}
