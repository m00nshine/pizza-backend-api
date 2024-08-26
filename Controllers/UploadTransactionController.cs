using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using pizza_backend_api.DataTransferObjects;
using pizza_backend_api.Models;

namespace pizza_backend_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadTransactionController : ControllerBase
    {
        private readonly EhrlichContext _context;

        public UploadTransactionController(EhrlichContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Upload transactions
        /// </summary>
        /// <param name="csv file"></param>
        /// <returns>
        ///     number of uploaded transactions
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

                                        List<Transaction> pendingTransaction = new List<Transaction>();

                                        foreach (TransactionUploadDto order in orders)
                                        {
                                            Transaction newData = MapCsvDataToSqlModel(order);
                                            if (newData != null)
                                            {
                                                pendingTransaction.Add(newData);
                                            }
                                        }

                                        var existingOrders = _context.Transactions
                                            .Where(x => pendingTransaction.Select(p => p.Id).Contains(x.Id))
                                            .ToList();

                                        List<Transaction> filterPendingTransaction = pendingTransaction
                                                        .Where(x => !existingOrders.Select(y => y.Id).Contains(x.Id)).ToList();

                                        foreach (Transaction transaction in filterPendingTransaction)
                                        {
                                            _context.Transactions.Add(transaction);
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

        private IEnumerable<TransactionUploadDto> ReadCsvFile(Stream fileStream)
        {
            try
            {
                using (var reader = new StreamReader(fileStream))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var records = csv.GetRecords<TransactionUploadDto>();
                    return records.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error reading CSV file", ex);
            }
        }

        private Transaction MapCsvDataToSqlModel(TransactionUploadDto data)
        {
            Transaction transaction = new Transaction();

            transaction.Id = Convert.ToInt32(data.order_id);
            
            DateTime transacationDate = DateTime.Parse(data.date);

            TimeSpan timeValue;
            if (TimeSpan.TryParse(data.time, out timeValue))
            {
                transacationDate = transacationDate.Add(timeValue);
            }
            else
            {
                return null;
            }

            transaction.CreatedDate = transacationDate;

            return transaction;

        }
    }
}
