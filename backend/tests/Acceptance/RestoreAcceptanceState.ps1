$ErrorActionPreference = 'Stop'
$base = 'http://localhost:5087/api'

function Post-Json($uri, $body) {
    Invoke-RestMethod -Method Post -Uri $uri -ContentType 'application/json' -Body ($body | ConvertTo-Json -Depth 20)
}

# The first acceptance run uses the first customer/estimate and creates a second
# auxiliary estimate for dynamic-service verification. Keep that test data, but
# cancel the auxiliary estimate so it does not alter the financial fixture.
$mainCustomerId = 1
$mainEstimateId = 1
$dynamicEstimateId = 2

Invoke-RestMethod -Method Put -Uri "$base/estimates/$dynamicEstimateId/status" -ContentType 'application/json' -Body (@{ status = 5; notes = 'Auxiliary dynamic-service acceptance fixture' } | ConvertTo-Json)
Post-Json "$base/payments" @{ customerId = $mainCustomerId; estimateId = $mainEstimateId; paymentDate = (Get-Date -Format 'yyyy-MM-dd'); amount = 30000; paymentMethodId = 1; transactionReference = 'Acceptance Cash Restored' } | Out-Null

$summary = Invoke-RestMethod "$base/customers/$mainCustomerId/summary"
Write-Output ($summary | ConvertTo-Json)
