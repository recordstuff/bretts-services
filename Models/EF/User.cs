using System;
using System.Collections.Generic;

namespace bretts_services.Models.EF;

public partial class User
{
    public long Id { get; set; }

    public Guid Guid { get; set; }

    public string Email { get; set; } = null!;

    public byte[] Password { get; set; } = null!;

    public byte[] Salt { get; set; } = null!;

    public string? DisplayName { get; set; }
}
