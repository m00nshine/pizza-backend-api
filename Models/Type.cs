using System;
using System.Collections.Generic;

namespace pizza_backend_api.Models;

public partial class Type
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int CategoryId { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
