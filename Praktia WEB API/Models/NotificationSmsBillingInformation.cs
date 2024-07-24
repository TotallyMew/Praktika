using System;
using System.Collections.Generic;

namespace Praktia_WEB_API.Models;

public partial class NotificationSmsBillingInformation
{
    public int Id { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public int EnvironmentId { get; set; }

    public string? RegistrationInfo { get; set; }

    public DateTime ActionDate { get; set; }

    public string ActionType { get; set; } = null!;

    public string SmsProviderName { get; set; } = null!;

    public string SmsProviderInfo { get; set; } = null!;
}
