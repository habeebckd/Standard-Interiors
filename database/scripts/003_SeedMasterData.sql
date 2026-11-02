USE [StandardInteriorsDb];
GO

IF NOT EXISTS (SELECT 1 FROM [PaymentMethods])
BEGIN
    INSERT INTO [PaymentMethods] ([Code], [Name], [IsActive], [DisplayOrder]) VALUES
    (N'CASH', N'Cash', 1, 1),
    (N'GPAY', N'GPay', 1, 2),
    (N'PHONEPE', N'PhonePe', 1, 3),
    (N'UPI', N'UPI', 1, 4),
    (N'BANK', N'Bank Transfer', 1, 5),
    (N'CHEQUE', N'Cheque', 1, 6),
    (N'CARD', N'Card', 1, 7),
    (N'OTHER', N'Other', 1, 8);
END
GO

IF NOT EXISTS (SELECT 1 FROM [Services])
BEGIN
    INSERT INTO [Services] ([ServiceCode], [ServiceName], [DisplayOrder], [IsActive], [CreatedAt], [UpdatedAt]) VALUES
    (N'SRV-001', N'Ceiling', 1, 1, SYSUTCDATETIME(), SYSUTCDATETIME()),
    (N'SRV-002', N'Electrical Work', 2, 1, SYSUTCDATETIME(), SYSUTCDATETIME()),
    (N'SRV-003', N'Putty', 3, 1, SYSUTCDATETIME(), SYSUTCDATETIME()),
    (N'SRV-004', N'Painting', 4, 1, SYSUTCDATETIME(), SYSUTCDATETIME()),
    (N'SRV-005', N'Lights', 5, 1, SYSUTCDATETIME(), SYSUTCDATETIME()),
    (N'SRV-006', N'Curtains', 6, 1, SYSUTCDATETIME(), SYSUTCDATETIME()),
    (N'SRV-007', N'Wardrobe & Dressing Room', 7, 1, SYSUTCDATETIME(), SYSUTCDATETIME()),
    (N'SRV-008', N'Bed Set', 8, 1, SYSUTCDATETIME(), SYSUTCDATETIME()),
    (N'SRV-009', N'Glass Partition', 9, 1, SYSUTCDATETIME(), SYSUTCDATETIME());
END
GO

IF NOT EXISTS (SELECT 1 FROM [CompanySettings])
BEGIN
    INSERT INTO [CompanySettings] ([CompanyName], [EstimatePrefix], [PaymentPrefix], [Currency], [DefaultEstimateNote], [LogoPath], [CreatedAt], [UpdatedAt]) VALUES
    (N'STANDARD INTERIORS', N'EST', N'PAY', N'INR', N'This estimate is prepared for the listed interior works. Final cost may vary based on site measurements, material/brand selection, design changes and additional work approved during execution.', N'..\..\..\..\..\..\assets\logo\standard-interiors-logo.png', SYSUTCDATETIME(), SYSUTCDATETIME());
END
GO
