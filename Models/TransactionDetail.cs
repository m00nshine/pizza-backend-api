using System;
using System.Collections.Generic;

namespace pizza_backend_api.Models;

public partial class TransactionDetail
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public string PizzaLongCode { get; set; } = null!;

    public int Quantity { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
