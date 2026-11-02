$ErrorActionPreference = 'Stop'
$base = 'http://localhost:5087/api'
function Post-Json($uri, $body) { Invoke-RestMethod -Method Post -Uri $uri -ContentType 'application/json' -Body ($body | ConvertTo-Json -Depth 20) }
function Test-Mode($mode, $amount, $items) {
    $stamp = Get-Date -Format 'MMddHHmmssfff'
    $customer = Post-Json "$base/customers" @{ name = "Pricing Mode $mode $stamp"; phone = "8$stamp" }
    $estimate = Post-Json "$base/estimates" @{ customerId=$customer.id; estimateDate=(Get-Date -Format 'yyyy-MM-dd'); pricingMode=$mode; status=1; manualOverallAmount=($(if($mode -eq 3){$amount}else{$null})); sections=@(@{sectionNumber='01';title="Mode $mode";displayOrder=1;manualSectionAmount=($(if($mode -eq 2){$amount}else{$null}));items=$items}) }
    $detail = Invoke-RestMethod "$base/estimates/$($estimate.id)"
    [pscustomobject]@{ Mode=$mode; EstimateId=$estimate.id; Total=$detail.totalAmount }
}
$itemPayload=@(
    @{ description='Ceiling'; quantity=1; rate=30000; displayOrder=1; isCustomItem=$true }
    @{ description='Electrical'; quantity=1; rate=20000; displayOrder=2; isCustomItem=$true }
    @{ description='Painting'; quantity=1; rate=50000; displayOrder=3; isCustomItem=$true }
)
$sectionPayload=@(
    @{ description='Ceiling'; displayOrder=1; isCustomItem=$true }
    @{ description='Electrical'; displayOrder=2; isCustomItem=$true }
)
$overallPayload=@(
    @{ description='Descriptive Ceiling'; displayOrder=1; isCustomItem=$true }
)
@(
    (Test-Mode 1 0 $itemPayload)
    (Test-Mode 2 100000 $sectionPayload)
    (Test-Mode 3 270000 $overallPayload)
) | ConvertTo-Json
