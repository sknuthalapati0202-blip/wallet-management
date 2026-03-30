using System;
using System.Collections.Generic;

namespace WalletManagement.Core.Domain.Models;

public partial class WalletConsent
{
    public int Id { get; set; }

    public string Suid { get; set; } = null!;

    public string CredentialId { get; set; } = null!;

    public string ApplicationId { get; set; } = null!;

    public string ConsentData { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
