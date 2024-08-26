using System;
using System.Collections.Generic;

namespace pizza_backend_api.Models;

public partial class Size
{
    public int Id { get; set; }

    public string? Code { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
