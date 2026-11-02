$ErrorActionPreference = 'Stop'
$base = 'http://localhost:5087/api'
$stamp = Get-Date -Format 'MMddHHmmss'

function Post-Json($uri, $body) {
    Invoke-RestMethod -Method Post -Uri $uri -ContentType 'application/json' -Body ($body | ConvertTo-Json -Depth 20)
}

function Put-Json($uri, $body) {
    Invoke-RestMethod -Method Put -Uri $uri -ContentType 'application/json' -Body ($body | ConvertTo-Json -Depth 20)
}

function Get-ServiceId($services, $name) {
    return ($services | Where-Object serviceName -eq $name | Select-Object -First 1).id
}

$customer = Post-Json "$base/customers" @{ name = "Acceptance $stamp"; phone = "900$stamp"; location = 'Kondotty' }
$services = Invoke-RestMethod "$base/services"
$ceiling = Get-ServiceId $services 'Ceiling'
$electrical = Get-ServiceId $services 'Electrical Work'
$putty = Get-ServiceId $services 'Putty'
$painting = Get-ServiceId $services 'Painting'
$lights = Get-ServiceId $services 'Lights'
$curtains = Get-ServiceId $services 'Curtains'
$wardrobe = Get-ServiceId $services 'Wardrobe & Dressing Room'
$bed = Get-ServiceId $services 'Bed Set'
$glass = Get-ServiceId $services 'Glass Partition'

$section1Items = @(
    @{ serviceId = $ceiling; description = 'Ceiling'; displayOrder = 1; isCustomItem = $false }
    @{ serviceId = $electrical; description = 'Electrical Work'; displayOrder = 2; isCustomItem = $false }
    @{ serviceId = $putty; description = 'Putty'; displayOrder = 3; isCustomItem = $false }
    @{ serviceId = $painting; description = 'Painting'; displayOrder = 4; isCustomItem = $false }
    @{ serviceId = $lights; description = 'Lights'; displayOrder = 5; isCustomItem = $false }
    @{ serviceId = $curtains; description = 'Curtains'; displayOrder = 6; isCustomItem = $false }
)

$section2Items = @(
    @{ serviceId = $wardrobe; description = 'Wardrobe & Dressing Room'; displayOrder = 1; isCustomItem = $false }
    @{ serviceId = $bed; description = 'Bed Set'; displayOrder = 2; isCustomItem = $false }
    @{ serviceId = $glass; description = 'Glass Partition'; displayOrder = 3; isCustomItem = $false }
)

$estimate = Post-Json "$base/estimates" @{
    customerId = $customer.id
    estimateDate = (Get-Date -Format 'yyyy-MM-dd')
    pricingMode = 2
    status = 3
    sections = @(
        @{ sectionNumber = '01'; title = 'Ceiling, Electrical & Finishing Works'; displayOrder = 1; manualSectionAmount = 100000; items = $section1Items }
        @{ sectionNumber = '02'; title = 'Furniture & Glass Partition Works'; displayOrder = 2; manualSectionAmount = 170000; items = $section2Items }
    )
}

$payment1 = Post-Json "$base/payments" @{ customerId = $customer.id; estimateId = $estimate.id; paymentDate = (Get-Date -Format 'yyyy-MM-dd'); amount = 50000; paymentMethodId = 2; transactionReference = 'Acceptance GPay' }
$payment2 = Post-Json "$base/payments" @{ customerId = $customer.id; estimateId = $estimate.id; paymentDate = (Get-Date -Format 'yyyy-MM-dd'); amount = 30000; paymentMethodId = 1; transactionReference = 'Acceptance Cash' }

$estimateDetail = Invoke-RestMethod "$base/estimates/$($estimate.id)"
$summary = Invoke-RestMethod "$base/customers/$($customer.id)/summary"
$statement = Invoke-RestMethod "$base/customers/$($customer.id)/statement"
$dashboard = Invoke-RestMethod "$base/dashboard"
$outstanding = Invoke-RestMethod "$base/reports/outstanding"

$expectedBefore = [pscustomobject]@{
    Estimate = $estimateDetail.totalAmount
    Received = $summary.totalReceived
    Balance = $summary.outstanding
    StatementLines = $statement.lines.Count
    DashboardEstimate = $dashboard.totalEstimatedValue
    DashboardReceived = $dashboard.totalAmountReceived
    OutstandingReport = ($outstanding | Where-Object customerCode -eq $summary.customerCode | Select-Object -First 1).outstanding
}

Invoke-RestMethod -Method Post -Uri "$base/payments/$($payment2.id)/void" -ContentType 'application/json' -Body (@{ reason = 'Acceptance void test' } | ConvertTo-Json)
$afterVoid = Invoke-RestMethod "$base/customers/$($customer.id)/summary"
$voidedPayment = Invoke-RestMethod "$base/payments/$($payment2.id)"

$newService = Post-Json "$base/services" @{ serviceName = 'Modular Kitchen'; description = 'Acceptance dynamic service'; displayOrder = 99 }
$dynamic = Post-Json "$base/estimates" @{
    customerId = $customer.id
    estimateDate = (Get-Date -Format 'yyyy-MM-dd')
    pricingMode = 2
    status = 3
    sections = @(
        @{ sectionNumber = '01'; title = 'Dynamic Works'; displayOrder = 1; manualSectionAmount = 100000; items = @(
            @{ serviceId = $ceiling; description = 'Ceiling'; displayOrder = 1; isCustomItem = $false }
            @{ serviceId = $newService.id; description = 'Modular Kitchen'; displayOrder = 2; isCustomItem = $false }
            @{ description = 'Decorative Wall Panel'; displayOrder = 3; isCustomItem = $true }
        ) }
    )
}
Invoke-WebRequest -Method Patch -Uri "$base/services/$($newService.id)/active?isActive=false" -UseBasicParsing | Out-Null
$dynamicReload = Invoke-RestMethod "$base/estimates/$($dynamic.id)"

$output = Join-Path $PSScriptRoot 'test-output'
New-Item -ItemType Directory -Path $output -Force | Out-Null
$pdfs = @{
    estimate = "$base/estimates/$($estimate.id)/pdf"
    paymentSheet = "$base/payments/$($customer.id)/sheet/pdf"
    paymentReceipt = "$base/payments/$($payment1.id)/receipt/pdf"
    customerSummary = "$base/customers/$($customer.id)/summary-pdf"
    customerStatement = "$base/customers/$($customer.id)/statement-pdf"
}
foreach ($key in $pdfs.Keys) { Invoke-WebRequest -Uri $pdfs[$key] -OutFile (Join-Path $output "$key.pdf") -UseBasicParsing }
$xlsx = @{
    customers = "$base/exports/customers.xlsx"
    payments = "$base/exports/payments.xlsx"
    outstanding = "$base/exports/outstanding.xlsx"
    statement = "$base/exports/customers/$($customer.id)/statement.xlsx"
}
foreach ($key in $xlsx.Keys) { Invoke-WebRequest -Uri $xlsx[$key] -OutFile (Join-Path $output "$key.xlsx") -UseBasicParsing }

[pscustomobject]@{
    CustomerId = $customer.id
    CustomerCode = $summary.customerCode
    EstimateId = $estimate.id
    PaymentIds = @($payment1.id, $payment2.id)
    BeforeVoid = $expectedBefore
    AfterVoid = [pscustomobject]@{ Received = $afterVoid.totalReceived; Outstanding = $afterVoid.outstanding; VoidStatus = $voidedPayment.status }
    DynamicItems = ($dynamicReload.sections | ForEach-Object { $_.items | ForEach-Object description }) -join ' | '
    ArtifactDirectory = $output
} | ConvertTo-Json -Depth 10
